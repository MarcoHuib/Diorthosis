#!/usr/bin/env python3
"""Project retained external AI judgments onto exact frozen generation occurrences; no new judgments."""
import collections
import importlib
import itertools
import json
from pathlib import Path
import statistics
import sys
import unicodedata

c = importlib.import_module('phase2-analysis')
a = importlib.import_module('analyze-prompt-quality')
imp = importlib.import_module('import-prompt-quality-review')
ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'benchmarks/phase4/v1'
A = 'diorthosis-correction-v3'
B = 'phase4-LiteralJson-Original-1.0.0'
BOOLS = sorted(imp.existing.BOOLS)


def both(values):
    return False if False in values else None if None in values else True


def passage(j, clean, necessity=True):
    # allDefectsRepaired is inapplicable (often null) on clean sources, not missing repair evidence.
    keys = ['meaningPreserved', 'noNewErrors', 'cleanPreserved' if clean else 'allDefectsRepaired']
    if necessity: keys.append('justified')
    return both([j.get(k) for k in keys] + ([False] if j.get('criticalSemanticFailure') is True else []))


def measure(values, target=True):
    return dict(target=target, count=sum(v is target for v in values), contrary=sum(v is not None and v is not target for v in values),
                unknown=sum(v is None for v in values), attempted=len(values))


def unnecessary(j, changed):
    if not changed: return False
    tags = [t.lower().replace('-', '_') for t in j['errorCategories']]
    if j['justified'] is False or any(any(token in t for token in ('unnecessary', 'unwarranted', 'onnodig')) for t in tags): return True
    return False if j['justified'] is True else None


def paired_records(control, variant, field):
    if set(control) != set(variant): raise ValueError('Unpaired source case set')
    gained=[];lost=[];unknown=[];ties=[];differences=[]
    for key in sorted(control):
        left, right = control[key][field], variant[key][field]
        if left is None or right is None:
            unknown.append(dict(caseId=key, control=left, variant=right)); continue
        differences.append(int(right)-int(left))
        if right and not left: gained.append(key)
        elif left and not right: lost.append(key)
        else: ties.append(key)
    return dict(gains=gained, losses=lost, ties=ties, unknown=unknown, pairedCases=len(control), knownPairs=len(differences),
        descriptiveInterval=a.paired_interval(differences),
        worstCaseNetGain=len(gained)-len(lost)-len(unknown), bestCaseNetGain=len(gained)-len(lost)+len(unknown))


def occurrence_index(subjects, lineage):
    index = {}
    for sid, links in lineage.items():
        s = subjects[sid]
        if s['layer'] == 'annotations': continue
        for link in links:
            key = (link['originalCaseId'], link['arm'], s['layer'])
            if key in index: raise ValueError('Repeated generation/layer lineage: '+str(key))
            index[key] = sid
    return index


def analyze(evidence, out):
    evidence=evidence.resolve();out.mkdir(parents=True,exist_ok=False)
    subjects={s['subjectId']:s for s in c.lines(BASE/'review/subjects.jsonl')}
    lineage=json.loads((BASE/'review/lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text())
    records=c.lines(evidence/'combined/analysis-dataset.jsonl')
    external={r['subject']['subjectId']:r['decision'] for r in records}
    batch={r['subject']['subjectId']:r['batch'] for r in records}
    if len(external)!=len(records) or set(external)!=set(subjects):raise ValueError('External decisions must uniquely cover parent subjects')
    errors=c.lines(evidence/'combined/error-decisions.jsonl')
    em={(e['subjectId'],e['annotationIndex']):e for e in errors}
    if len(em)!=len(errors):raise ValueError('Duplicate supplementary error key')
    index=occurrence_index(subjects,lineage)
    occurrences=[];error_occurrences=[];cases={};reference={};disagreements=[]
    specs=json.loads((BASE/'review-spec.json').read_text())
    for spec in specs:
        panel=Path(spec['panel']).stem
        panel_cases,rows=a.load(ROOT/spec['panel'],[ROOT/p for p in spec['runs']])
        cases.update({r['id']:r for r in panel_cases})
        for r in rows:
            case=cases[r['id']];clean=not case['errors'];eligible=a.eligible(r)
            sid={layer:index.get((r['id'],r['promptVersion'],layer)) for layer in ('raw','parsed','delivered','rejected')}
            for layer in ('raw','parsed','delivered'):
                if sid[layer] is None:raise ValueError('Missing generation layer')
                s=subjects[sid[layer]]
                expected=r['inference']['text'] if layer=='raw' else r['analysis']['extractedCorrection'] if layer=='parsed' else r['delivered']
                if s['candidateText']!=expected or s['sourceText']!=case['inputText'] or json.loads(s['annotationsJson'])!=case['errors']:raise ValueError('Trace/subject content mismatch')
                links=[x for x in lineage[sid[layer]] if x['originalCaseId']==r['id'] and x['arm']==r['promptVersion']]
                if len(links)!=1 or links[0]['run']!=r['run'] or links[0]['traceSha256']!=r['traceSha256']:raise ValueError('Trace lineage identity mismatch')
            js={layer:external[s]['judgment'] for layer,s in sid.items() if s is not None}
            delivered=js['delivered'];parsed=js['parsed'];raw=js['raw']
            delivered_errors=[];parsed_errors=[];raw_errors=[];safe_errors=[]
            for i,error in enumerate(case['errors']):
                local={layer:em.get((subject,i)) for layer,subject in sid.items() if subject is not None}
                values={layer:entry['repaired'] if entry else None for layer,entry in local.items()}
                delivered_value=values.get('delivered') if eligible else False
                safe_value=both([delivered_value, delivered['meaningPreserved'], delivered['noNewErrors']]) if eligible else False
                safe_errors.append(safe_value)
                delivered_errors.append(delivered_value);parsed_errors.append(values.get('parsed'));raw_errors.append(values.get('raw'))
                er=dict(panel=panel,caseId=r['id'],language=r['language'],promptVersion=r['promptVersion'],annotationIndex=i,annotation=error,
                    disposition=r['analysis']['disposition'],deliveryEligible=eligible,subjectIds=sid,reviewedRepairs=values,
                    deliveredRepair=delivered_value,safeDeliveredRepair=safe_value,abstained=not eligible,
                    policy='Ineligible delivery cannot repair an error; reviewed candidate-local judgments are retained separately. No unknown eligible judgment is replaced.',
                    evidence=local)
                error_occurrences.append(er)
            sourceid=next(s['subjectId'] for s in subjects.values() if s['layer']=='annotations' and lineage[s['subjectId']][0]['originalCaseId']==r['id'])
            row=dict(panel=panel,caseId=r['id'],language=r['language'],promptVersion=r['promptVersion'],run=r['run'],traceSha256=r['traceSha256'],
                original=case['inputText'],candidate=r['analysis']['extractedCorrection'],rawOutput=r['inference']['text'] if r['inference'] else None,
                delivered=r['delivered'],originallyClean=clean,annotatedErrors=len(case['errors']),sourceAnnotationSubjectId=sourceid,sourceAssessment=external[sourceid]['judgment'],
                subjectIds=sid,batches={layer:batch[s] for layer,s in sid.items() if s},judgments=js,disposition=r['analysis']['disposition'],
                rejectionStage=r['analysis']['rejectionStage'],rejectionReason=r['analysis']['rejectionReason'],eligible=eligible,
                deliveredErrorRepairs=delivered_errors,safeDeliveredErrorRepairs=safe_errors,parsedErrorRepairs=parsed_errors,rawErrorRepairs=raw_errors,
                fullyAcceptableDelivered=passage(delivered,clean) if eligible else False,
                linguisticallySoundDelivered=passage(delivered,clean,False) if eligible else False,
                cleanPreserved=(delivered['cleanPreserved'] if eligible else False) if clean else None,
                literalCleanPreserved=(eligible and r['delivered']==r['input']) if clean else None,
                unnecessaryModification=unnecessary(delivered,r['delivered']!=r['input']) if eligible else False,
                meaningFailure=(not delivered['meaningPreserved'] if delivered['meaningPreserved'] is not None else None) if eligible else False,
                newErrors=(not delivered['noNewErrors'] if delivered['noNewErrors'] is not None else None) if eligible else False,
                criticalFailure=delivered['criticalSemanticFailure'] if eligible else False,
                validNecessaryCandidateLost=(passage(parsed,False) if any(v is True for v in parsed_errors) else False) if not eligible and not clean else False,
                soundNecessaryCandidateLost=(passage(parsed,False,False) if any(v is True for v in parsed_errors) else False) if not eligible and not clean else False,
                validRawNecessaryCandidateLost=(passage(raw,False) if any(v is True for v in raw_errors) else False) if not eligible and not clean else False,
                rawValidRepairLostAtExtraction=(passage(raw,False) is True and passage(parsed,False) is not True and any(v is True for v in raw_errors)) if not clean else False,
                promptContentTokens=r['promptContentTokens'],sampledOutputTokens=r['sampledTokens'],latencyMilliseconds=r['latencyMilliseconds'],
                sampledPeakProcessWorkingSetBytes=r['sampledPeakProcessWorkingSetBytes'],generationTermination=r['generationTermination'],failure=r['failure'])
            occurrences.append(row)
    if len(occurrences)!=920 or len({(r['caseId'],r['promptVersion']) for r in occurrences})!=920:raise ValueError('Generation occurrence coverage mismatch')
    c.write_lines(out/'occurrences.jsonl',occurrences);c.write_lines(out/'error-occurrences.jsonl',error_occurrences)
    metrics=[]
    for (panel,language,version),group in itertools.groupby(sorted(occurrences,key=lambda r:(r['panel'],r['language'],r['promptVersion'])),lambda r:(r['panel'],r['language'],r['promptVersion'])):
        selected=list(group);clean=[r for r in selected if r['originallyClean']];err=[r for r in selected if not r['originallyClean']]
        er=[v for r in selected for v in r['deliveredErrorRepairs']]
        parsed_er=[v for r in selected for v in r['parsedErrorRepairs']]
        lost=[r for r in selected if not r['eligible']]
        # Categories are heterogeneous reviewer labels, not a standardized grammar detector.
        tagged_new=[r for r in selected if r['newErrors'] is True and set(r['judgments']['delivered']['errorCategories']) &
                    {'spelling_introduced','introduced_spelling','introduced_grammar','nonword','unidiomatic_dutch','incorrect_collocation'}]
        m=dict(panel=panel,language=language,promptVersion=version,attempted=len(selected),originallyErroneous=len(err),originallyCorrect=len(clean),annotatedErrors=len(er),
            deliveredErrorRepairs=measure(er),safeDeliveredErrorRepairs=measure([v for r in selected for v in r['safeDeliveredErrorRepairs']]),parsedErrorRepairs=measure(parsed_er),missedOriginalErrors=sum(v is False for v in er),unknownOriginalErrorRepairs=sum(v is None for v in er),
            abstainedAnnotatedErrors=sum(r['annotatedErrors'] for r in lost),
            validNecessaryCandidatesLost=measure([r['validNecessaryCandidateLost'] for r in selected]),soundNecessaryCandidatesLost=measure([r['soundNecessaryCandidateLost'] for r in selected]),
            validRawNecessaryCandidatesLost=measure([r['validRawNecessaryCandidateLost'] for r in selected]),
            extractionLostValidRawCandidates=sum(r['rawValidRepairLostAtExtraction'] for r in selected),
            localRepairedErrorsWithheld=sum(v is True for r in lost for v in r['parsedErrorRepairs']),
            rejectedRepairAppropriateness=measure([r['judgments'].get('rejected',{}).get('rejectionAppropriate') for r in lost]),
            cleanPreservation=measure([r['cleanPreserved'] for r in clean]),literalCleanPreservation=measure([r['literalCleanPreserved'] for r in clean]),
            cleanTextDamage=measure([not r['judgments']['delivered']['cleanPreserved'] if r['eligible'] and r['judgments']['delivered']['cleanPreserved'] is not None else None if r['eligible'] else False for r in clean]),
            cleanAbstentions=sum(not r['eligible'] for r in clean),
            judgments={field:measure([r[field] for r in selected]) for field in ['fullyAcceptableDelivered','linguisticallySoundDelivered','unnecessaryModification','meaningFailure','newErrors','criticalFailure']},
            introducedSpellingGrammarExplicitlyTaggedLowerBound=len(tagged_new),newErrorTypeNotStandardized=True,
            externalAlternativeListsOnDelivered=sum(bool(r['judgments']['delivered']['acceptedAlternatives']) for r in selected if r['eligible']),
            dispositions=dict(collections.Counter(r['disposition'] for r in selected)),
            eligibleJudgmentCoverage={k:measure([r['judgments']['delivered'][k] for r in selected if r['eligible']]) for k in BOOLS},
            promptContentTokens=dict(mean=statistics.mean(r['promptContentTokens'] for r in selected),total=sum(r['promptContentTokens'] for r in selected)),
            sampledOutputTokens=sum(r['sampledOutputTokens'] for r in selected),
            latencyMilliseconds=dict(mean=statistics.mean(r['latencyMilliseconds'] for r in selected),median=statistics.median(r['latencyMilliseconds'] for r in selected)),
            sampledPeakProcessWorkingSetBytes=max(r['sampledPeakProcessWorkingSetBytes'] for r in selected),humanApprovals=0)
        metrics.append(m)
    c.write(out/'metrics.json',dict(policy='Descriptive external AI evidence, frozen-label denominators. Errors are source annotations, not an exhaustive human inventory. Ineligible deliveries are abstentions, never successes. Unknown judgments remain unknown. Clean allDefectsRepaired is inapplicable: strict full success uses justified, meaningPreserved, noNewErrors and cleanPreserved; erroneous sources use allDefectsRepaired. Sound passage omits justified, keeping edit necessity separate. No critical=true passage can succeed. New-error booleans include formatting/meaning; the explicit spelling/grammar tag count is only a heterogeneous-taxonomy lower bound. Alternative lists are not independently valid-candidate counts.',metrics=metrics))
    paired=[]
    for panel in ('selection','confirmation'):
        for language in ('dutch','english'):
            groups={v:{r['caseId']:r for r in occurrences if r['panel']==panel and r['language']==language and r['promptVersion']==v} for v in {r['promptVersion'] for r in occurrences if r['panel']==panel}}
            for control in (A,B):
                for version in sorted(groups):
                    if version==control:continue
                    co,va=groups[control],groups[version]
                    pair=dict(panel=panel,language=language,control=control,variant=version,
                        metrics={f:paired_records(co,va,f) for f in ['fullyAcceptableDelivered','linguisticallySoundDelivered','meaningFailure','newErrors','criticalFailure','unnecessaryModification']})
                    pair['cleanPreservation']=paired_records({k:r for k,r in co.items() if r['originallyClean']},{k:r for k,r in va.items() if r['originallyClean']},'cleanPreserved')
                    ec={};ev={}
                    for key in co:
                        for i,(cv,vv) in enumerate(zip(co[key]['deliveredErrorRepairs'],va[key]['deliveredErrorRepairs'])):
                            ec[key+f'/error-{i}']={'repaired':cv};ev[key+f'/error-{i}']={'repaired':vv}
                    pair['errorRepairs']=paired_records(ec,ev,'repaired')
                    for key in co:
                        for i,(cv,vv) in enumerate(zip(co[key]['safeDeliveredErrorRepairs'],va[key]['safeDeliveredErrorRepairs'])):
                            ec[key+f'/error-{i}']={'repaired':cv};ev[key+f'/error-{i}']={'repaired':vv}
                    pair['safeErrorRepairs']=paired_records(ec,ev,'repaired');paired.append(pair)
    c.write(out/'paired.json',dict(policy='Each pair compares the exact same source cases and actual occurrences. Shared layer/candidate judgments are reused, not independent trials. Intervals resample known paired case outcomes descriptively; unknown bounds show all unresolved pairs, not confidence limits. Error bootstrap units are annotated errors (mostly one per case), not independent population observations.',comparisons=paired))
    # Differences across review layers are retained, never silently replaced by another layer.
    layer_conflicts=[]
    for caseid in sorted(cases):
        ss=[s for s in subjects.values() if s['layer']!='annotations' and lineage[s['subjectId']][0]['originalCaseId']==caseid]
        for text,group in itertools.groupby(sorted(ss,key=lambda s:repr(s['candidateText'])),lambda s:s['candidateText']):
            same=list(group)
            for l,r in itertools.combinations(same,2):
                fields=['meaningPreserved','allDefectsRepaired','noNewErrors','criticalSemanticFailure']
                diff={k:dict(left=external[l['subjectId']]['judgment'][k],right=external[r['subjectId']]['judgment'][k]) for k in fields if external[l['subjectId']]['judgment'][k] is not None and external[r['subjectId']]['judgment'][k] is not None and external[l['subjectId']]['judgment'][k]!=external[r['subjectId']]['judgment'][k]}
                ediff={i:dict(left=em[l['subjectId'],i]['repaired'],right=em[r['subjectId'],i]['repaired']) for i in range(len(json.loads(l['annotationsJson']))) if (l['subjectId'],i) in em and (r['subjectId'],i) in em and em[l['subjectId'],i]['repaired'] is not None and em[r['subjectId'],i]['repaired'] is not None and em[l['subjectId'],i]['repaired']!=em[r['subjectId'],i]['repaired']}
                if diff or ediff:layer_conflicts.append(dict(caseId=caseid,source=l['sourceText'],candidate=text,leftSubjectId=l['subjectId'],rightSubjectId=r['subjectId'],fields=diff,errorFields=ediff))
    c.write(out/'cross-layer-conflicts.json',layer_conflicts)
    # Existing confirmation judgments have exact subject bindings; selection observations are literal parsed pairs only.
    preparer={r['subjectId']:r for r in c.lines(BASE/'confirmation-preparer-ai/decisions.jsonl')}
    perrors={(r['subjectId'],r['annotationIndex']):r for r in c.lines(BASE/'confirmation-preparer-ai/error-decisions.jsonl')}
    oldobs=c.lines(BASE/'preparer-inspection/preparer-observations.jsonl')
    for sid,old in preparer.items():
        s=subjects[sid];new=external[sid];oj=old['judgment'];nj=new['judgment']
        diff={k:dict(preparer=oj[k],external=nj[k]) for k in BOOLS if oj[k]!=nj[k]}
        ediff={i:dict(preparer=perrors[sid,i]['repaired'],external=em[sid,i]['repaired']) for i in range(len(json.loads(s['annotationsJson']))) if (sid,i) in perrors and (sid,i) in em and perrors[sid,i]['repaired']!=em[sid,i]['repaired']}
        disagreements.append(dict(type='confirmation-bound',subject=s,lineage=lineage[sid],preparer=old,external=new,differences=diff,errorDifferences=ediff,
            preparerFull=passage(oj,s['proposedClean']),externalFull=passage(nj,s['proposedClean']),
            reviewWarranted=any(v['preparer'] is not None and v['external'] is not None for v in diff.values()) or bool(ediff)))
    for old in oldobs:
        matched=[s for s in subjects.values() if s['layer']=='parsed' and s['sourceText']==old['sourceText'] and s['candidateText']==old['candidateText']]
        if len(matched)!=1:raise ValueError('Preparer literal contextual pair not unique')
        s=matched[0];sid=s['subjectId'];nj=external[sid]['judgment']
        oj={k:old[k] for k in ('meaningPreserved','criticalSemanticFailure')}
        oj.update(noNewErrors=not old['newErrorsIntroduced'] if old['newErrorsIntroduced'] is not None else None)
        diff={k:dict(preparer=v,external=nj[k]) for k,v in oj.items() if v!=nj[k]}
        n_unnecessary=unnecessary(nj,s['candidateText']!=s['sourceText'])
        if n_unnecessary!=old['unnecessaryChanges']:diff['unnecessaryChanges']=dict(preparer=old['unnecessaryChanges'],external=n_unnecessary)
        ediff={}
        known=old['knownRepairedAnnotationIndexes']
        if known is not None:
            for i in range(len(json.loads(s['annotationsJson']))):
                if (sid,i) in em and ((i in known)!=em[sid,i]['repaired']):ediff[i]=dict(preparer=i in known,external=em[sid,i]['repaired'])
        disagreements.append(dict(type='selection-literal-parsed',subject=s,lineage=lineage[sid],preparer=old,external=external[sid],differences=diff,errorDifferences=ediff,
            reviewWarranted=any(v['preparer'] is not None and v['external'] is not None for v in diff.values()) or bool(ediff)))
    for d in disagreements:
        s=d['subject'];d['interpretation']=[]
        if s['proposedClean'] and 'allDefectsRepaired' in d['differences']:
            d['interpretation'].append('Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead.')
        if s['layer']=='delivered' and not json.loads(s['metadataJson'])['deliveryEligible']:
            d['interpretation'].append('Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success.')
        if 'noNewErrors' in d['differences']:
            d['interpretation'].append('Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.')
        if d['subject']['sourceText'].find('koffie apparaat')>=0:
            d['interpretation'].append('Preparer credits facultative hyphenation; external review retains orthographic uncertainty. Do not count an unknown repair as a verified validator false rejection.')
        if 'γεια' in d['subject']['sourceText']:
            d['interpretation'].append('Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling.')
        if 'preparerFull' in d:
            d['interpretation'].append('Strict passage conjunction before/after: preparer='+str(d['preparerFull'])+', external='+str(d['externalFull'])+'. Delivery eligibility is applied separately.')
        if not d['interpretation']:
            d['interpretation'].append('No automatic adjudication. Both original rationales remain the linguistic reasoning; sensitivity uses only their explicit fields.')
    c.write_lines(out/'preparer-comparison.jsonl',disagreements)
    # Conservative sensitivity: replace only known contradictory fields by the retained preparer values.
    # Null disagreements remain external; they do not grant new preparer coverage.
    alternate={sid:dict(d['judgment']) for sid,d in external.items()}
    for d in disagreements:
        for k,v in d['differences'].items():
            if k in BOOLS and v['preparer'] is not None and v['external'] is not None:
                alternate[d['subject']['subjectId']][k]=v['preparer']
    # Selection preparer observations are parsed-only; explicitly project exact literal context onto
    # eligible delivered text, retaining original observation identity (not a new review decision).
    for old in oldobs:
        for r in occurrences:
            if r['eligible'] and r['caseId']==old['caseId'] and r['delivered']==old['candidateText']:
                sid=r['subjectIds']['delivered']
                for k,v in {'meaningPreserved':old['meaningPreserved'],'noNewErrors':None if old['newErrorsIntroduced'] is None else not old['newErrorsIntroduced'],'criticalSemanticFailure':old['criticalSemanticFailure']}.items():
                    if v is not None and alternate[sid][k] is not None and v!=alternate[sid][k]:alternate[sid][k]=v
    sensitivity=[]
    for m in metrics:
        selected=[r for r in occurrences if (r['panel'],r['language'],r['promptVersion'])==(m['panel'],m['language'],m['promptVersion'])]
        sensitivity.append(dict(panel=m['panel'],language=m['language'],promptVersion=m['promptVersion'],
            externalSound=m['judgments']['linguisticallySoundDelivered'],
            preparerContradictionSensitivitySound=measure([passage(alternate[r['subjectIds']['delivered']],r['originallyClean'],False) if r['eligible'] else False for r in selected]),
            externalFull=m['judgments']['fullyAcceptableDelivered'],
            preparerContradictionSensitivityFull=measure([passage(alternate[r['subjectIds']['delivered']],r['originallyClean']) if r['eligible'] else False for r in selected])))
    c.write(out/'disagreement-sensitivity.json',dict(policy='Diagnostic counterfactual only, not adjudication: substitute known contradictory preparer booleans on exact bound confirmation subjects and exact contextual selection pairs. Preserve null uncertainty. No new judgment, reference change or extra reviewer inferred.',metrics=sensitivity))
    # Complete examples are evidence excerpts, not fresh adjudications.
    md=['# External GPT-6 versus preparer AI', '', 'Both are non-independent AI evidence. No judgment is changed. Null-to-known differences are coverage changes, not known contradictions. Source/candidate identity and both literal rationales follow. Full matched records (including agreements) are in preparer-comparison.jsonl.', '']
    for d in disagreements:
        if not d['differences'] and not d['errorDifferences']:continue
        s=d['subject'];old=d['preparer'];nj=d['external']['judgment']
        md += [f"## {d['type']} / {s['caseId']} / {s['layer']}",f"Subject: `{s['subjectId']}`",'Source: '+json.dumps(s['sourceText'],ensure_ascii=False),'Candidate: '+json.dumps(s['candidateText'],ensure_ascii=False),
               'Conditions: '+', '.join(sorted({x.get('arm','source-annotations') for x in d['lineage']})),
               'Differences: '+json.dumps(d['differences'],ensure_ascii=False)+'; per-error: '+json.dumps(d['errorDifferences'],ensure_ascii=False),
               'Preparer rationale: '+(old['judgment']['rationale'] if 'judgment' in old else old['rationale']), 'External rationale: '+nj['rationale'],
               'Interpretation: '+' '.join(d['interpretation']), 'Impact: '+('Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.' if d['reviewWarranted'] else 'Different assessment coverage; retain unknowns.'), '']
    (out/'disagreements.md').write_text('\n\n'.join(md).rstrip()+'\n')
    # Rank by actual delivered critical/meaning/new-error evidence, with full failures available to filter by source/condition.
    defects=[r for r in occurrences if any(r[k] is True for k in ('criticalFailure','meaningFailure','newErrors','unnecessaryModification','validNecessaryCandidateLost'))]
    defects.sort(key=lambda r:(not r['criticalFailure'],not r['meaningFailure'],not r['newErrors'],r['caseId'],r['promptVersion']))
    c.write_lines(out/'defects.jsonl',defects)
    md=['# Ranked delivered defects and lost valid repairs', '', 'Mechanical ordering: delivered critical semantic label, then meaning failure, new errors, unnecessary change, withheld otherwise-valid necessary candidate. Labels and rationales are the submitted AI evidence; severity is not human adjudicated. Repeated conditions on the same source are paired occurrences, not independent observations.', '']
    for r in defects:
        md += [f"## {r['panel']} / {r['caseId']} / {r['promptVersion']}",f"Disposition: {r['disposition']}; delivered critical={r['criticalFailure']}, meaning failure={r['meaningFailure']}, new errors={r['newErrors']}, unnecessary={r['unnecessaryModification']}, valid necessary repair withheld={r['validNecessaryCandidateLost']}.",
               'Original: '+json.dumps(r['original'],ensure_ascii=False),'Extracted: '+json.dumps(r['candidate'],ensure_ascii=False),'Delivered: '+json.dumps(r['delivered'],ensure_ascii=False),
               'Delivered assessment: '+r['judgments']['delivered']['rationale'], 'Parsed assessment: '+r['judgments']['parsed']['rationale'],
               'Rejection: '+str(r['rejectionReason']), '']
    (out/'defects.md').write_text('\n\n'.join(md).rstrip()+'\n')
    # Reference-only discrepancies identify recognized local alternatives without rewriting references.
    alternatives=[]
    for panel in ('selection','confirmation'):
        directory=BASE/('selection-analysis' if panel=='selection' else 'confirmation-analysis')
        for r in [x for x in occurrences if x['panel']==panel]:
            path=directory/(r['promptVersion']+'-delivered-reference.json')
            if path not in reference:reference[path]={x['caseId']:x for x in json.loads(path.read_text())['cases']}
            ref=reference[path][r['caseId']]['repairedAnnotations']
            ref_indices={e if isinstance(e,int) else e.get('annotationIndex') for e in ref}
            for i,v in enumerate(r['deliveredErrorRepairs']):
                if v is True and i not in ref_indices:
                    alternatives.append(dict(panel=panel,caseId=r['caseId'],language=r['language'],promptVersion=r['promptVersion'],annotationIndex=i,
                        source=r['original'],candidate=r['delivered'],wholePassageSound=r['linguisticallySoundDelivered'],wholePassageFullyAcceptable=r['fullyAcceptableDelivered'],subjectId=r['subjectIds']['delivered'],external=em[r['subjectIds']['delivered'],i],referenceRepairedAnnotations=ref))
    c.write_lines(out/'local-repairs-vs-reference.jsonl',alternatives)
    for m in metrics:
        matched=[r for r in alternatives if (r['panel'],r['language'],r['promptVersion'])==(m['panel'],m['language'],m['promptVersion'])]
        m['recognizedSoundAlternativeErrorOccurrencesAbsentReference']=sum(r['wholePassageSound'] is True for r in matched)
        m['localRepairCreditsAbsentReference']=len(matched)
        m['localRepairCreditsWithUnknownWholePassage']=sum(r['wholePassageSound'] is None for r in matched)
    metrics_document=json.loads((out/'metrics.json').read_text());metrics_document['metrics']=metrics
    c.write(out/'metrics.json',metrics_document)
    summary=dict(version='phase4-external-analysis-1.0.0',sources=len(cases),subjects=len(subjects),generationOccurrences=len(occurrences),candidateErrorDecisions=len(errors),humanApprovals=0,
        crossLayerContradictions=len(layer_conflicts),crossLayerContradictionSourceCases=len({x['caseId'] for x in layer_conflicts}),
        comparison=dict(matchedRecords=len(disagreements),structuredBooleanDifferences=sum(bool(r['differences']) for r in disagreements),
            knownContradictions=sum(any(v['preparer'] is not None and v['external'] is not None for v in r['differences'].values()) for r in disagreements),
            perErrorDifferences=sum(bool(r['errorDifferences']) for r in disagreements)),
        localDeliveredErrorRepairsAbsentReference=len(alternatives),soundDeliveredAlternativesAbsentReference=sum(r['wholePassageSound'] is True for r in alternatives),
        deliveredCriticalOccurrences=sum(r['criticalFailure'] is True for r in occurrences),deliveredCriticalSourceCases=len({r['caseId'] for r in occurrences if r['criticalFailure'] is True}),
        deliveredMeaningFailureOccurrences=sum(r['meaningFailure'] is True for r in occurrences),
        withheldValidNecessaryOccurrences=sum(r['validNecessaryCandidateLost'] is True for r in occurrences),
        dispositionTotals=dict(collections.Counter(r['disposition'] for r in occurrences)),
        policy='All aggregate occurrences overlap across conditions and layers, and include selection exposure. No population superiority, independent reviewers, exhaustive grammar-type counts, or human validation inferred.')
    c.write(out/'summary.json',summary)
    unicode_rows=[]
    for r in occurrences:
        if any(ord(ch)>127 for ch in r['original']):
            unicode_rows.append(dict(caseId=r['caseId'],panel=r['panel'],language=r['language'],promptVersion=r['promptVersion'],diagnosticTags=cases[r['caseId']]['diagnosticTags'],
                source=r['original'],candidate=r['candidate'],delivered=r['delivered'],disposition=r['disposition'],
                candidateCanonicallyEquivalentToSource=unicodedata.normalize('NFC',r['candidate'])==unicodedata.normalize('NFC',r['original']) if r['candidate'] is not None else None,
                deliveryChanged=r['delivered']!=r['original'],subjectIds=r['subjectIds'],judgments=r['judgments']))
    c.write_lines(out/'unicode-diagnostics.jsonl',unicode_rows)
    overall=[]
    for version in sorted({r['promptVersion'] for r in occurrences}):
        for language in ('dutch','english'):
            selected=[r for r in occurrences if r['promptVersion']==version and r['language']==language]
            overall.append(dict(promptVersion=version,language=language,panels=sorted({r['panel'] for r in selected}),attempted=len(selected),
                localRepairs=measure([v for r in selected for v in r['deliveredErrorRepairs']]),safeRepairs=measure([v for r in selected for v in r['safeDeliveredErrorRepairs']]),
                soundPassages=measure([r['linguisticallySoundDelivered'] for r in selected]),fullPassages=measure([r['fullyAcceptableDelivered'] for r in selected]),
                criticalFailures=measure([r['criticalFailure'] for r in selected])))
    c.write(out/'overall-descriptive.json',dict(policy='A/B pool 70 selection + 20 confirmation cases per language; other arms have selection only. Each original case repeats across arms. These are exposed synthetic descriptive totals, not comparable independent sample estimates or a new confirmation.',metrics=overall))
    labels={A:'A production v3',B:'B literal original','phase4-PlainText-Original-1.0.0':'C plain text',
        'phase4-LiteralJson-Minimal-1.0.0':'D minimal','phase4-LiteralJson-LanguageSpecific-1.0.0':'E language checklist','phase4-LiteralJson-FewShot-1.0.0':'F few-shot'}
    fmt=lambda m:str(m['count'])+(' + '+str(m['unknown'])+'?' if m['unknown'] else '')
    md=['# Per-panel external AI comparisons','','Every attempt is retained. `n + u?` means n known positive outcomes and u unknowns, not n+u successes. Local repairs do not establish passage success. Safe repair additionally requires meaning and no-new-error=true. Sound passage requires meaning/no-new-error plus all original defects repaired (or cleanPreserved on clean source). Strict full success additionally requires justified=true. Abstention never earns repair, passage success or eligible clean preservation. Source clean/error partitions remain frozen; source-review rationales may dispute labels.','']
    for panel in ('selection','confirmation'):
        for language in ('dutch','english'):
            md += [f'## {panel} / {language}','','| Condition | Attempts | Error/clean passages | Local repairs / errors | Safe repairs | Missed / unknown local | Clean preserved / clean | Sound passages | Strict full | Meaning failures | New errors | Critical | Unnecessary | Valid necessary withheld |','|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|']
            for m in sorted([m for m in metrics if m['panel']==panel and m['language']==language],key=lambda m:labels[m['promptVersion']]):
                j=m['judgments']
                md.append('| '+ ' | '.join([labels[m['promptVersion']],str(m['attempted']),f"{m['originallyErroneous']}/{m['originallyCorrect']}",fmt(m['deliveredErrorRepairs'])+'/'+str(m['annotatedErrors']),fmt(m['safeDeliveredErrorRepairs']),f"{m['missedOriginalErrors']}/{m['unknownOriginalErrorRepairs']}",fmt(m['cleanPreservation'])+'/'+str(m['originallyCorrect']),fmt(j['linguisticallySoundDelivered']),fmt(j['fullyAcceptableDelivered']),fmt(j['meaningFailure']),fmt(j['newErrors']),fmt(j['criticalFailure']),fmt(j['unnecessaryModification']),fmt(m['validNecessaryCandidatesLost'])])+' |')
    md += ['','## Dispositions and resources','','| Panel/language/condition | Accepted | Unchanged | Rejected | Needs review | Suppressed | Mean prompt tokens | Median latency ms | Peak RSS bytes |','|---|---:|---:|---:|---:|---:|---:|---:|---:|']
    for m in metrics:
        d=m['dispositions'];md.append('| '+ ' | '.join([m['panel']+'/'+m['language']+'/'+labels[m['promptVersion']],*[str(d.get(k,0)) for k in ('accepted','unchanged','rejected','needsReview','suppressed')],str(round(m['promptContentTokens']['mean'],1)),str(round(m['latencyMilliseconds']['median'])),str(m['sampledPeakProcessWorkingSetBytes'])])+' |')
    md += ['','## Paired gains/losses','','Gains/losses are desirable outcomes for repairs and sound passages. Unknown pairs are not ties. Exact IDs and descriptive intervals are retained in paired.json.','','| Panel/language | Control → variant | Local repair gains/losses | Safe repair gains/losses/unknown | Sound passage gains/losses/unknown | Clean gains/losses/unknown |','|---|---|---:|---:|---:|---:|']
    for p in paired:
        if p['control']==A and p['variant']!=B:continue
        if p['control']==B and p['variant']==A:continue
        nums=lambda d:'/'.join(str(len(d[k])) for k in ('gains','losses','unknown'))
        md.append('| '+ ' | '.join([p['panel']+'/'+p['language'],labels[p['control']]+' → '+labels[p['variant']],nums(p['errorRepairs']),nums(p['safeErrorRepairs']),nums(p['metrics']['linguisticallySoundDelivered']),nums(p['cleanPreservation'])])+' |')
    md += ['','New-error booleans include semantic, formatting and linguistic damage. No standardized introduced-spelling/grammar count can be recovered from heterogeneous reviewer categories; metrics.json exposes an explicit-tag lower bound and the full category evidence remains in occurrences.jsonl. Do not relabel source-error categories as newly introduced errors.','', 'Alternative repairs absent from references are in local-repairs-vs-reference.jsonl; only records with wholePassageSound=true support a sound whole-passage alternative. The 19 local credits include deleted modality, uncertain aspect and singularized pumps, which are not 19 valid corrections.','', 'All 478 unique bound candidate/error judgments are reused across 564 annotated-error generation occurrences and separate layers; repetition does not increase independent evidence. Zero human approvals.']
    (out/'COMPARISON.md').write_text('\n'.join(md)+'\n')
    inputs=[evidence/'combined/analysis-dataset.jsonl',evidence/'combined/error-decisions.jsonl',BASE/'review/subjects.jsonl',BASE/'review/lineage-DO-NOT-SEND-TO-REVIEWERS.json',BASE/'confirmation-preparer-ai/decisions.jsonl',BASE/'confirmation-preparer-ai/error-decisions.jsonl',BASE/'preparer-inspection/preparer-observations.jsonl']
    c.write(out/'manifest.json',dict(inputs={str(p.relative_to(ROOT)) if ROOT in p.parents else str(p):c.sha(p) for p in inputs},outputs={p.name:c.sha(p) for p in out.iterdir() if p.is_file()},scriptSha256=c.sha(Path(__file__))))
    print(json.dumps(summary,indent=2))


if __name__=='__main__':analyze(*map(Path,sys.argv[1:]))
