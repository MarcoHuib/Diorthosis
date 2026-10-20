#!/usr/bin/env python3
"""Offline deterministic trace replay, layered diagnostics, and blinded review packet generation.
No external modules, inference, network, or mutation of source evidence.
"""
import collections
import hashlib
import html
import itertools
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]
CLI=ROOT/'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'
DATA=ROOT/'benchmarks/model-validation/smoke-44-reviewed-v2.jsonl'
MODELS=['ministral-3b','ministral-3-8b','ministral-3-14b','eurollm-9b']
TAXONOMY=['Missed spelling correction','Incorrect spelling correction','Missed grammatical correction','Incorrect grammatical correction',
          'Word-order failure','Missing punctuation correction','Incorrect punctuation change','Valid alternative missing from the reference',
          'Unnecessary stylistic rewrite','Meaning change','Technical identifier change','Unsupported document structure',
          'Model response-format failure','Incomplete output','Validator false rejection','Ambiguous linguistic judgment']

def sha(path):return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def digest(value):return hashlib.sha256(value.encode('utf-8')).hexdigest()
def lines(path):return [json.loads(s) for s in Path(path).read_text().splitlines() if s.strip()]
def jsontext(value):return json.dumps(value,ensure_ascii=False,separators=(',',':'))
def write(path,value):Path(path).write_text(json.dumps(value,ensure_ascii=False,indent=2)+'\n')
def write_lines(path,rows):Path(path).write_text(''.join(jsontext(row)+'\n' for row in rows))
def command(*args):return subprocess.run(['dotnet',str(CLI),*map(str,args)],cwd=ROOT,check=True,capture_output=True,text=True).stdout

def apply(source,edits):
    raw=source.encode('utf-16-le')
    last=len(raw)//2
    for edit in sorted(edits,key=lambda item:(item['start'],item['length']),reverse=True):
        start,end=edit['start'],edit['start']+edit['length']
        if start<0 or end>last or raw[2*start:2*end].decode('utf-16-le')!=edit['originalText']:
            raise ValueError('Invalid or overlapping UTF-16 source edits')
        raw=raw[:2*start]+edit['replacementText'].encode('utf-16-le')+raw[2*end:];last=start
    return raw.decode('utf-16-le')

def candidate_prediction(case,candidate):
    # Diagnostic reconstruction of exactly the parsed text, including incomplete/unsafe candidates.
    # This is never a deliverable edit or authorization to apply it.
    edits=[] if candidate==case['inputText'] else [dict(start=0,length=len(case['inputText'].encode('utf-16-le'))//2,originalText=case['inputText'],replacementText=candidate)]
    return dict(caseId=case['id'],edits=edits)

def subject(case,layer,kind,candidate,annotations,metadata):
    row=dict(subjectId='',bindingSha256='',caseId=case['id'],language=case['language'].capitalize(),layer=layer,kind=kind,
             sourceText=case['inputText'],candidateText=candidate,annotationsJson=jsontext(annotations),metadataJson=jsontext(metadata),
             sourceFamily=case.get('sourceFamily'),documentId=case.get('documentId'),proposedClean=not case['errors'])
    fields=[('null' if row[k] is None else 'string:'+row[k]) if k in ('candidateText','sourceFamily','documentId') else row[k] for k in ['caseId','language','layer','kind','sourceText','candidateText','annotationsJson','metadataJson','sourceFamily','documentId']]+['true' if row['proposedClean'] else 'false']
    row['bindingSha256']=digest(''.join(str(len(v.encode('utf-8')))+':'+v for v in fields))
    row['subjectId']='review-'+row['bindingSha256']
    return row

def delivered_assessable(disposition):return disposition in ('accepted','unchanged')

def annotation_errors(item):
    if 'annotationVariants' not in item:return item['errors']
    if len(item['annotationVariants'])!=1:raise ValueError('Conflicting annotation variants require explicit adjudication; cannot choose the first.')
    return item['annotationVariants'][0]['errors']

def rejection_class(stage,reason):
    value=(str(stage)+' '+str(reason)).lower()
    if not stage:return None
    if 'termination' in value:return 'invalid-generation-termination'
    if any(x in value for x in ['offset','align','source span','source text']):return 'invalid-source-offset-or-mismatch'
    if any(x in value for x in ['protected','identifier','name','date','number','url','quot']):return 'protected-content-violation'
    if any(x in value for x in ['meaning','negation','modal','statement','speech','temporal','agent','pronoun']):return 'semantic-risk-detection'
    if any(x in value for x in ['rewrite','lexical','overlap']):return 'excessive-rewriting'
    if any(x in value for x in ['paragraph','structure','ambig']):return 'structural-ambiguity'
    if any(x in value for x in ['format','response','json','extract']):return 'invalid-response-format'
    return 'unsupported-output-or-unclassified'

def triage(item):
    """Explicit AI screening rules from reading all 263 groups; proposals only."""
    text=item['inputText']; errors=item['errors']; findings=[]
    def flag(action,reason):findings.append(dict(action=action,reason=reason,authority='AI linguistic screening; requires human adjudication'))
    if not errors:flag('verify-proposed-clean','No annotated defect; verify idiom and regional conventions rather than assuming clean.')
    elif all(e['category'].lower()=='spelling' for e in errors):flag('plausible-local-spelling-repair','Proposed spelling replacement looks locally plausible; full passage may contain further errors.')
    else:flag('verify-grammar-category-span-and-alternatives','Review dependency, category and minimal source span; this is not approval.')
    if text.startswith('Voor het gesprek heeft de manager') or text.startswith('Na controle verstuurde zij'):
        flag('propose-withdraw-error-annotation','Source order is grammatical; the annotated reorder appears optional style, not a required repair.')
    if text.startswith('The team have approved'):
        flag('regional-variant-not-required-repair','Collective-noun plural agreement can be standard British usage; do not require American singular agreement without a locale policy.')
    if text.startswith('This data are'):
        flag('missing-alternative-and-category-ambiguity','Mass-data agreement permits This data is; plural-data reading suggests These data are. Check demonstrative and register together.')
    if text.startswith('Please note the deadline') or text.startswith('We received the request but') or text.startswith('We hebben de aanvraag ontvangen maar'):
        flag('punctuation-optionality','Proposed colon/comma may be optional; source may already be acceptable. Do not reward unnecessary punctuation automatically.')
    if any(e['category'].lower()=='punctuation' for e in errors):
        flag('propose-add-punctuation-alternatives','Inspect full stop/semicolon/colon/conjunction alternatives in context, and whether punctuation is obligatory.')
    if 'De team Atlas' in text or 'de team Atlas' in text:
        flag('additional-unannotated-error','Dutch team is neuter: article choice is not covered by the current local spelling/grammar target. Entity use is also unnatural.')
    if text.startswith('Please seperate the two'):
        flag('additional-unannotated-error','The two requires a plural/countable noun; several generated replacements leave singular or mass nouns after the spelling repair.')
    if any(piece in text for piece in ['Hallo planning','Hallo documentatie','Hallo rapportage','Hallo aanvraag','Goedemorgen planning','Goedemorgen rapportage','Goedemorgen aanvraag','Goedemorgen documentatie','Good morning schedule','Good morning application','Good morning documentation','Good morning report','signed Team Atlas','recieve the Team Atlas','recieved the Team Atlas','geinstalleerd in de map']):
        flag('unrealistic-template-context','Template substitution produces unusual addressees, objects or collocations; rewrite as a new candidate, preserve historical source.')
    if any(e['category'].lower()=='sentencestructure' for e in errors):
        flag('broad-span-or-dependent-repair-review','Whole-clause spans can hide multiple dependent edits and alternative clause orders. Review atomic dependency grouping.')
    if any(e['originalText'] in ('have send','Wait lets','informations are') for e in errors):
        flag('mixed-category-dependency-review','Single reference span combines multiple spelling/grammar/punctuation operations; categories and dependency need review.')
    return findings

def main():
    out=Path(sys.argv[1]);out.mkdir(parents=True,exist_ok=False)
    cases=lines(DATA);case_map={r['id']:r for r in cases}
    old_ai={(r['model'],r['caseId']):r for r in lines(ROOT/'benchmarks/model-validation/audits/quality-v1/output-adjudications.jsonl')}
    old_ai.update({(r['model'],r['caseId']):r for r in lines(ROOT/'benchmarks/phase2/v1/eurollm-ai-screen.jsonl')})
    source_notes=json.loads((ROOT/'benchmarks/model-validation/audits/quality-v1/reference-notes.json').read_text())['cases']
    subjects={};lineage=collections.defaultdict(list);failure_rows=[];model_reports=[];input_hashes={str(p.relative_to(ROOT)):sha(p) for p in [ROOT/'benchmarks/phase2/v1/eurollm-ai-screen.jsonl',ROOT/'benchmarks/model-validation/audits/quality-v1/output-adjudications.jsonl',ROOT/'benchmarks/model-validation/audits/quality-v1/reference-notes.json']};all_refs={}
    def add(row,origin=None):
        subjects.setdefault(row['subjectId'],row)
        if origin:lineage[row['subjectId']].append(origin)
        return row['subjectId']
    inventory=[];proposals=[]
    for collection,path in [('smoke-44',DATA),('candidate-groups-263',ROOT/'benchmarks/rehabilitation/v1/dataset/candidate-groups-1.0.0.jsonl'),('diverse-80',ROOT/'benchmarks/phase2/v1/diversity/cases.jsonl')]:
        input_hashes[str(path.relative_to(ROOT))]=sha(path)
        for i,original in enumerate(lines(path),1):
            item=dict(original)
            item['errors']=annotation_errors(item)
            flags=['needs-independent-idiom-category-span-and-alternative-review']
            if collection=='candidate-groups-263':flags+=['unknown-original-template-family','synthetic-template-risk','annotations-not-language-reviewed']
            if not item['errors']:flags+=['proposed-clean-status-needs-verification']
            if len(item['errors'])>1:flags+=['dependency-review-required']
            if collection=='smoke-44':flags+=['AI-reference-notes-available','model-predictions-development-exposed']
            if collection=='diverse-80':flags+=['AI-generated-not-independent','regional-usage-and-realism-review-required']
            notes=source_notes.get(item['id']) if collection=='smoke-44' else None
            if notes and (notes.get('ambiguity') or 'alternative' in str(notes).lower()):flags+=['consult-separate-AI-alternative-and-ambiguity-notes']
            screening=triage(item)
            for finding in screening:
                proposals.append(dict(caseId=item['id'],collection=collection,sourceSha256=sha(path),sourceLine=i,**finding))
            metadata=dict(aiScreening=screening,collection=collection,sourceProvenance=item.get('provenance',dict(kind='historical synthetic development material',sourcePath=str(path.relative_to(ROOT)),sourceSha256=sha(path),line=i)),
                sourceIds=item.get('sourceIds',[item['id']]),partition='development-exposed',reviewStatus='pending',caseCategory=item.get('phenomena',['historical-annotation']),
                protectedSpans=item.get('protectedSpans',[]),knownAmbiguity=item.get('ambiguity','Unknown; inspect source meaning and reference completeness.'),
                flags=flags,aiReferenceNotes=notes,reviewers=[dict(slot=1,reviewerId=None,qualifications=None,decision=None),dict(slot=2,reviewerId=None,qualifications=None,decision=None)],
                adjudication=dict(status='pending',adjudicatorId=None,outcome=None))
            row=subject(item,'annotations','case',None,item['errors'],metadata)
            sid=add(row)
            inventory.append(dict(caseId=item['id'],collection=collection,sourceLine=i,sourceSha256=sha(path),subjectId=sid,language=item['language'],
                                  aiScreening=screening,annotationCount=len(item['errors']),proposedClean=not item['errors'],flags=flags,assessment='AI triage only; no annotation approved',
                                  sourceFamily=item.get('sourceFamily'),humanSignoffRequired=True))
    for model in MODELS:
        base=ROOT/'benchmarks/model-validation/results'
        trace=base/('eurollm-9b-v1/trace.jsonl' if model=='eurollm-9b' else f'v4/{model}-trace.jsonl')
        report=base/('eurollm-9b-v1/report.json' if model=='eurollm-9b' else f'v4/{model}-report.json')
        original_report=json.loads(report.read_text())
        input_hashes[str(trace.relative_to(ROOT))]=sha(trace);input_hashes[str(report.relative_to(ROOT))]=sha(report)
        target=out/model;target.mkdir()
        command('reparse',DATA,trace,report,target/'predictions.jsonl',target/'trace.jsonl',target/'replay-report.json')
        replay=lines(target/'trace.jsonl');original=lines(trace)
        replay_binding=json.loads((target/'replay-report.json').read_text())['replayProvenance']['binding']
        parsed=[]
        for row in replay:
            if row.get('extractedCorrection') is not None:parsed.append(candidate_prediction(case_map[row['caseId']],row['extractedCorrection']))
        write_lines(target/'parsed-predictions.jsonl',parsed)
        refs={}
        for layer,pred in [('delivered',target/'predictions.jsonl'),('parsed',target/'parsed-predictions.jsonl')]:
            refs[layer]=json.loads(command('evaluate-v3',DATA,pred));write(target/(layer+'-references.json'),refs[layer])
        all_refs[model]={layer:{r['caseId']:r for r in value['cases']} for layer,value in refs.items()}
        counts=collections.Counter();rej=collections.Counter();symptoms=collections.Counter();ai_tax=collections.Counter();layer_cases=[]
        for index,row in enumerate(replay):
            item=case_map[row['caseId']];orig=original[index]
            assert orig['caseId']==row['caseId']
            parsed_candidate=row.get('extractedCorrection')
            delivered=apply(item['inputText'],row['validatedTextEdits'])
            delivered_ref=all_refs[model]['delivered'][item['id']];parsed_ref=all_refs[model]['parsed'][item['id']]
            counts['inputs']+=1;counts['rawOutputsAvailable']+=bool(row['rawModelOutput'])
            counts['naturalEog']+=row['terminationReason'].lower()=='endofgeneration';counts['parsedAssessable']+=parsed_candidate is not None
            counts['deliveredAssessable']+=delivered_assessable(row['disposition']);counts['validatedEdits']+=len(row['validatedTextEdits'])
            counts[row['disposition']]+=1
            reason=rejection_class(row.get('rejectionStage'),row.get('rejectionReason'))
            if reason:rej[reason]+=1
            missing=[dict(annotationIndex=j,category=e['category'],source=e['originalText']) for j,e in enumerate(item['errors']) if j not in delivered_ref['repairedAnnotations']]
            symptoms.update('Reference-unrepaired:'+r['category'] for r in missing)
            tags=[]
            if row['terminationReason'].lower()!='endofgeneration':tags+=['Incomplete output']
            ai=old_ai.get((model,item['id']))
            if ai:
                tags+=ai.get('taxonomy',[])
                classification=ai['acceptedClassification']
                if classification=='Incorrect or incomplete correction':
                    categories={e['category'].lower() for e in item['errors']}
                    tagmap={'spelling':'Missed spelling correction','grammar':'Missed grammatical correction','sentencestructure':'Word-order failure','punctuation':'Missing punctuation correction'}
                    tags += [tagmap[category] for category in categories if category in tagmap]
                if model=='ministral-3-8b' and item['id']=='nl-03':tags+=['Incorrect grammatical correction','Word-order failure']
                if classification=='Unnecessary change to already-correct text':tags+=['Unnecessary stylistic rewrite']
                if classification=='Grammatically correct with altered meaning':tags+=['Meaning change']
                if classification in ('Grammatically acceptable but semantically ambiguous','Requires independent human review'):tags+=['Ambiguous linguistic judgment']
                if classification=='Correct minimal correction' and delivered_ref['referenceCompletePassage'] is False and delivered!=item['inputText'] and delivered==ai['accepted']:
                    tags+=['Valid alternative missing from the reference']
                if row['disposition'] in ('rejected','needsReview') and ai['rawClassification']=='Correct minimal correction':tags+=['Validator false rejection']
                if ai.get('rawProtocolDeviation'):tags+=['Model response-format failure']
            if row.get('rejectionStage') in ('ResponseExtraction','responseExtraction'):tags+=['Model response-format failure']
            ai_tax.update(set(tags))
            origins=dict(replayBinding=replay_binding,identityPolicy='Model identity/revision below are original source-report claims, unverified for legacy trace/run pairs without a binding hash.',model=model,modelRevision=original_report.get('modelRevision'),modelSha256=original_report.get('modelSha256'),quantization=original_report.get('quantization'),
                         sourceTrace=str(trace.relative_to(ROOT)),traceSha256=sha(trace),sourceLine=index+1,sourceReport=str(report.relative_to(ROOT)),sourceReportSha256=sha(report),
                         replayTrace=str((target/'trace.jsonl').relative_to(ROOT)) if target.is_relative_to(ROOT) else str(target/'trace.jsonl'),evaluator=refs['delivered']['evaluatorVersion'])
            subject_ids=[]
            meta=dict(reviewStatus='pending',partition='development-exposed',originalCaseId=item['id'],ambiguity='Unreviewed reference annotations and incomplete acceptable alternatives.',
                      independentReviewers=[None,None],adjudication=None)
            for layer,candidate,reference in [('raw',row['rawModelOutput'],None),('parsed',parsed_candidate,parsed_ref),('delivered',delivered,delivered_ref)]:
                if candidate is None or layer=='delivered' and not delivered_assessable(row['disposition']):continue
                layer_meta=dict(meta,assessable=True,disposition=row['disposition']) if layer=='delivered' else meta
                sid=add(subject(item,layer,'passage',candidate,item['errors'],layer_meta),origins);subject_ids.append(sid)
                if reference:
                    for proposal in reference['proposals']:
                        candidate_group=apply(item['inputText'],proposal['edits'])
                        sid=add(subject(item,layer,'proposal',candidate_group,item['errors'],dict(layer_meta,proposalId=proposal['id'],edits=proposal['edits'],annotationIndexes=proposal['annotationIndexes'])),origins);subject_ids.append(sid)
            if row['disposition'] in ('rejected','needsReview'):
                sid=add(subject(item,'rejected','rejection',parsed_candidate,item['errors'],dict(meta,disposition=row['disposition'],stage=row.get('rejectionStage'),reason=row.get('rejectionReason'))),origins);subject_ids.append(sid)
            failure_rows.append(dict(caseId=item['id'],language=item['language'],provenance=origins,input=item['inputText'],rawOutput=row['rawModelOutput'],parsedCandidate=parsed_candidate,
                                     deliveredCandidate=delivered,disposition=row['disposition'],rejectionCategory=reason,rejectionReason=row.get('rejectionReason'),
                                     referenceUnrepaired=missing,referenceUnresolvedGroups=delivered_ref['referenceUnresolvedGroups'],referenceComplete=delivered_ref['referenceCompletePassage'],
                                     parsedReferenceRepairs=len(parsed_ref['repairedAnnotations']),deliveredReferenceRepairs=len(delivered_ref['repairedAnnotations']),
                                     taxonomyAIOrMechanical=sorted(set(tags)),taxonomyPolicy='Historical AI labels describe historical accepted/raw strings retained in historicalAIAssessment, not newly adjudicated replay delivery. Mechanical symptoms describe current replay. Validator false rejection is an AI-flagged hypothesis. Never independent human judgments.',
                                     historicalAIAssessment=ai,linguisticJudgment=None,reviewSubjects=subject_ids))
            layer_cases.append(dict(caseId=item['id'],language=item['language'],clean=not item['errors'],parsed=parsed_candidate is not None,
                                    delivered=delivered_assessable(row['disposition']),disposition=row['disposition']))
        coverage=[]
        for lang in ['Dutch','English']:
            selected=[r for r in layer_cases if r['language'].lower()==lang.lower()]
            ratio=lambda n:dict(numerator=n,denominator=len(selected),estimate=n/len(selected) if selected else None)
            coverage.append(dict(language=lang,inputs=len(selected),cleanPassages=sum(r['clean'] for r in selected),
                                 exactUniqueTexts=len({case_map[r['caseId']]['inputText'] for r in selected}),independentSourceGroups=None,documents=None,
                                 parsedCoverage=ratio(sum(r['parsed'] for r in selected)),deliveredCoverage=ratio(sum(r['delivered'] for r in selected)),
                                 rejected=ratio(sum(r['disposition']=='rejected' for r in selected)),needsReview=ratio(sum(r['disposition']=='needsReview' for r in selected)),
                                 linguisticProposalPrecision=None,linguisticCompletePassageSuccess=None,cleanLinguisticPreservation=None,criticalSemanticFailureRate=None))
        model_reports.append(dict(replayBinding=replay_binding,identityPolicy='Original source report model identity claims; legacy trace/run association is unverified where traceBoundToRun is false.',model=model,modelRevision=original_report.get('modelRevision'),modelSha256=original_report.get('modelSha256'),counts=counts,rejectionReasons=rej,
                                  referenceSymptoms=symptoms,taxonomyAIOrMechanical=ai_tax,coverage=coverage,resources='No inference performed; historical resources remain solely in source report.'))
    paired=[]
    for a,b in itertools.combinations(MODELS,2):
        for language in ['Dutch','English']:
            chosen=[item for item in cases if item['language'].lower()==language.lower()]
            a_only=[];b_only=[];both=[];neither=[]
            for item in chosen:
                x=all_refs[a]['delivered'][item['id']]['referenceCompletePassage'];y=all_refs[b]['delivered'][item['id']]['referenceCompletePassage']
                if x is None or y is None:continue
                (both if x and y else a_only if x else b_only if y else neither).append(item['id'])
            paired.append(dict(models=[a,b],language=language,metric='complete-reference-match; exposed/unreviewed labels',both=both,aOnly=a_only,bOnly=b_only,neither=neither,pairedKnown=len(both+a_only+b_only+neither),populationInference='none'))
    shared=[]
    for item in cases:
        misses={m:[i for i in range(len(item['errors'])) if i not in all_refs[m]['delivered'][item['id']]['repairedAnnotations']] for m in MODELS}
        common=set.intersection(*(set(v) for v in misses.values()))
        if common:shared.append(dict(caseId=item['id'],commonUnrepairedAnnotations=sorted(common),byModel=misses,interpretation='Reference symptom; inspect ambiguity, missing alternatives, safety and linguistic judgments before assigning a cause.'))
    write_lines(out/'annotation-proposals.jsonl',proposals);write_lines(out/'inventory.jsonl',inventory);write_lines(out/'subjects.jsonl',subjects.values());write(out/'lineage-DO-NOT-SEND-TO-REVIEWERS.json',lineage)
    write_lines(out/'failure-analysis.jsonl',failure_rows)
    (out/'decisions.jsonl').touch();(out/'adjudications.jsonl').touch()
    command('adjudicate',out/'subjects.jsonl',out/'decisions.jsonl',out/'adjudications.jsonl',out/'adjudicated-metrics.json','--lineage',out/'lineage-DO-NOT-SEND-TO-REVIEWERS.json')
    taxonomy=[]
    for category in TAXONOMY:
        matching=[r for r in failure_rows if category in r['taxonomyAIOrMechanical']]
        symptom_categories={'Missed spelling correction':['spelling'],'Missed grammatical correction':['grammar'],'Word-order failure':['sentenceStructure'],'Missing punctuation correction':['punctuation']}
        reference_symptoms=[r for r in failure_rows if any(m['category'].lower() in [c.lower() for c in symptom_categories.get(category,[])] for m in r['referenceUnrepaired'])]
        taxonomy.append(dict(category=category,count=len(matching),referenceSymptomPassages=len(reference_symptoms),denominator=len(failure_rows),judgmentKind='mechanical or historical AI only',independentlyHumanConfirmed=0,
                             examples=[dict(model=r['provenance']['model'],caseId=r['caseId'],traceSha256=r['provenance']['traceSha256'],sourceLine=r['provenance']['sourceLine']) for r in matching[:4]],
                             limitation='Zero does not mean absent; specific incorrect/missed linguistic subtypes and document structure lack independent labels. Reference misses are counted separately.'))
    report=dict(version='phase2-trace-analysis-1.0.0',models=model_reports,taxonomy=taxonomy,paired=paired,sharedReferenceMisses=shared,
                independentHumanDecisions=0,policy='Reference matches, validator outcomes, and historical AI labels are separate. No model ranking or population generalization. Rejected unchanged delivery is not an assessable successful correction.',
                subjects=len(subjects),caseInventory=len(inventory),inventoryByCollection=dict(collections.Counter(r['collection'] for r in inventory)),sourceHashes=input_hashes,
                repositoryCommit=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),sourceWorkingTree=subprocess.check_output(['git','status','--porcelain','--untracked-files=no'],cwd=ROOT,text=True).strip() or 'clean tracked implementation',
                implementationHashes={str(p.relative_to(ROOT)):sha(p) for base in ['scripts','src'] for p in (ROOT/base).rglob('*') if p.is_file() and ('bin' not in p.parts and 'obj' not in p.parts) and p.suffix in ('.cs','.py','.sh','.csproj')})
    write(out/'analysis.json',report)
    cards=['<!doctype html><meta charset="utf-8"><title>Diorthosis blind linguistic review</title><style>body{font:16px system-ui;max-width:72em;margin:2em auto}article{border-top:1px solid #999;padding:1em 0;break-inside:avoid}pre{white-space:pre-wrap;background:#f4f4f4;padding:1em}details{margin:1em 0}</style><h1>Provisional review packet</h1><p>Development-exposed AI/synthetic material. No human approval. Model identities are withheld here; distribute this file and subjects.jsonl without the separate lineage/analysis files. Use the bound subject ID for imports. Record two independent reviewers and a separate adjudicator. Blank fields are not approvals. JSONL contains exact UTF-16 annotations and protected spans.</p>']
    for row in subjects.values():
        cards.append('<article><h2>'+html.escape(row['subjectId'])+'</h2><p>'+html.escape(row['language']+' / '+row['layer']+' / '+row['kind']+' / '+row['caseId'])+'</p><h3>Source</h3><pre>'+html.escape(row['sourceText'])+'</pre><h3>Candidate</h3><pre>'+html.escape(row['candidateText'] if row['candidateText'] is not None else '[No candidate; review proposed annotations]')+'</pre><details><summary>Proposed annotations / provenance / ambiguity</summary><pre>'+html.escape(row['annotationsJson']+'\n'+row['metadataJson'])+'</pre></details><p>Reviewer 1: ______ Qualifications: ______ Decision/rationale: ______</p><p>Reviewer 2: ______ Qualifications: ______ Decision/rationale: ______</p><p>Alternatives/category/ambiguity/meaning: ______ Disagreement: ______</p><p>Adjudicator: ______ Status/outcome/version: ______</p></article>')
    (out/'review.html').write_text('\n'.join(cards))
    summary=['# Stored-trace analysis (development diagnostic)','',report['policy'],'', '| Model | Parsed | Delivered assessable | Rejected | Needs review |','|---|---:|---:|---:|---:|']
    for r in model_reports:
        c=r['counts'];summary.append(f"| {r['model']} | {c['parsedAssessable']}/44 | {c['deliveredAssessable']}/44 | {c.get('rejected',0)}/44 | {c.get('needsReview',0)}/44 |")
    summary+=['','All 176 outputs are linked to exact trace/report hashes and model revisions in failure-analysis.jsonl. Original traces and timings were not altered. Independent linguistic quality remains unknown. See analysis.json for reference symptoms, all taxonomy categories, paired case IDs and shared misses.','',f"Inventory: {len(inventory)} cases; {len(subjects)} blinded, content-bound review subjects. All need human sign-off. No source-family independence or document count is invented."]
    (out/'summary.md').write_text('\n'.join(summary)+'\n')
    write(out/'manifest.json',dict(sourceHashes=input_hashes,files={str(p.relative_to(out)):sha(p) for p in sorted(out.rglob('*')) if p.is_file()},reviewWorkflow='linguistic-review-1.0.0',humanApprovals=0))
    print('\n'.join(summary))
if __name__=='__main__':main()
