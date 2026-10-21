#!/usr/bin/env python3
"""Reconcile bound AI judgments and retained layers; no new linguistic evaluator or inference."""
import collections
import importlib
import json
from pathlib import Path
import sys

c=importlib.import_module('phase2-analysis')
validation=importlib.import_module('import-external-linguistic-review')
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase3/v1'
BOOLS=sorted(validation.BOOLS)
ARMS=['baseline','hints','sample-101','sample-202','sample-303','voting-3']

def conjunction(j):
    values=[j[k] for k in ['justified','meaningPreserved','allDefectsRepaired','noNewErrors']]
    return False if False in values else None if None in values else True

def measure(values,target=True):
    known=[v for v in values if v is not None]
    return dict(numerator=sum(v is target for v in known),denominator=len(known),unknown=len(values)-len(known),total=len(values),
        estimate=sum(v is target for v in known)/len(known) if known else None)

def explain(subject,differences,old,new):
    notes=[]
    if any(old[k] is None or new[k] is None for k in differences if k in BOOLS):notes.append('Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness.')
    if 'allDefectsRepaired' in differences:
        notes.append('All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone.')
    if 'criticalSemanticFailure' in differences:notes.append('Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate.')
    if subject['candidateText'] and 'verzorgt' in subject['candidateText']:notes.append('Regional/collocational uncertainty: both reviewers reject the necessity of the change, but disagree about whether it preserves meaning. Do not implement a word blacklist.')
    if subject['kind']=='rejection' and subject['candidateText'] is None:notes.append('No candidate exists: external approval of abstention is distinct from a linguistic judgment about an output.')
    if 'acceptedAlternatives' in differences:notes.append('Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references.')
    if 'errorCategories' in differences:notes.append('Error taxonomies differ; category-list inequality alone is not an independent language failure.')
    notes.append(f"Full-passage diagnostic conjunction: preparer={conjunction(old)}, external={conjunction(new)}. Human metrics remain unknown; human approvals remain zero.")
    return notes

def analyze(evidence,out):
    evidence=evidence.resolve();out=out.resolve()
    validation.validate(evidence/'original/diorthosis-ai-review-package.zip')
    out.mkdir(parents=True,exist_ok=False)
    subjects=c.lines(BASE/'review-all-candidates/subjects.jsonl');sm={s['subjectId']:s for s in subjects}
    external={r['subjectId']:r for r in c.lines(evidence/'original/diorthosis-ai-review.jsonl')}
    preparer={r['subjectId']:r for r in c.lines(BASE/'preparer-ai-decisions.jsonl')}
    lineage=json.loads((BASE/'review-all-candidates/lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text())
    traces=c.lines(BASE/'run/trace.jsonl');trace={(r['id'],r['arm']):r for r in traces}
    comparisons=[];markdown=['# External GPT-6 / preparer AI reconciliation','Both are non-independent AI diagnostic assessments. No reviewer or reference is automatically authoritative. Full records, including taxonomy/alternative differences, are in comparison.jsonl.']
    for s in subjects:
        old=preparer[s['subjectId']]['judgment'];new=external[s['subjectId']]['judgment']
        diff={k:dict(preparer=old[k],external=new[k]) for k in BOOLS+['acceptedAlternatives','errorCategories'] if old[k]!=new[k]}
        boolean_diff={k:v for k,v in diff.items() if k in BOOLS}
        record=dict(subject=s,lineage=lineage[s['subjectId']],preparer=preparer[s['subjectId']],external=external[s['subjectId']],differences=diff,
            explanation=explain(s,diff,old,new) if diff else ['No structured-judgment disagreement. Rationales remain separately preserved.'],
            expectedImpact=dict(preparerPassageConjunction=conjunction(old),externalPassageConjunction=conjunction(new),humanApprovals=0))
        comparisons.append(record)
        if boolean_diff:
            markdown.extend(['',f"## {s['caseId']} / {s['layer']} / {s['subjectId']}",f"Source: {json.dumps(s['sourceText'],ensure_ascii=False)}",f"Candidate: {json.dumps(s['candidateText'],ensure_ascii=False)}",
                'Conditions: '+', '.join(sorted({x['arm'] for x in lineage[s['subjectId']]})),
                'Different fields: '+json.dumps(boolean_diff,ensure_ascii=False),
                'Preparer rationale: '+old['rationale'],'External rationale: '+new['rationale'],
                'Explanation / expected impact: '+' '.join(record['explanation'])])
    c.write_lines(out/'comparison.jsonl',comparisons)
    (out/'disagreements.md').write_text('\n\n'.join(markdown)+'\n')
    failures={}
    for field,value in [('meaningPreserved',False),('noNewErrors',False),('criticalSemanticFailure',True)]:
        bad=[s for s in subjects if external[s['subjectId']]['judgment'][field] is value]
        members=[(s,link) for s in bad for link in lineage[s['subjectId']]]
        failures[field]=dict(subjects=len(bad),knownJudgments=sum(external[s['subjectId']]['judgment'][field] is not None for s in subjects),
            uniqueCandidateTextsWithinSourceCases=len({(s['caseId'],s['candidateText']) for s in bad if s['candidateText'] is not None}),
            originalPassages=len({s['caseId'] for s in bad}),layers=dict(collections.Counter(s['layer'] for s in bad)),
            representedGenerationIds=sorted({link['originalCaseId']+'/'+link['arm'] for s,link in members if link['arm'] in ARMS[:-1]}),
            deliveredArmOccurrences=sorted({link['originalCaseId']+'/'+link['arm'] for s,link in members if s['layer']=='delivered'}),
            deliveredUniqueCandidates=len({(s['caseId'],s['candidateText']) for s in bad if s['layer']=='delivered'}),
            byLanguage=dict(collections.Counter(s['language'].lower() for s in bad)),
            subjectIds=[s['subjectId'] for s in bad])
    metrics=[]
    for language in ['dutch','english']:
        for arm in ARMS:
            for layer in ['raw','delivered','rejected']:
                matched=[s for s in subjects if s['language'].lower()==language and s['layer']==layer and any(link['arm']==arm for link in lineage[s['subjectId']])]
                js=[external[s['subjectId']]['judgment'] for s in matched]
                actual=[r for r in traces if r['language']==language and r['arm']==arm]
                clean=[r for r in actual if next(s for s in subjects if s['sourceText']==r['input'])['proposedClean']]
                metrics.append(dict(language=language,arm=arm,layer=layer,sourceCases=len({s['caseId'] for s in matched}),subjectCount=len(matched),
                    candidateTexts=len({s['candidateText'] for s in matched if s['candidateText'] is not None}),
                    nullCandidates=sum(s['candidateText'] is None for s in matched),
                    completePassage=measure([conjunction(j) for j in js]) if layer!='rejected' else None,
                    judgments={field:measure([j[field] for j in js],target=False if field in ['meaningPreserved','noNewErrors'] else True) for field in BOOLS},
                    conditionSourceCases=len(actual),originalDispositions=dict(collections.Counter(r['analysis']['disposition'] for r in actual)),
                    assessableLiteralClean=dict(numerator=sum(r['analysis']['disposition'] in ['accepted','unchanged'] and c.apply(r['input'],r['analysis']['edits'])==r['input'] for r in clean),denominator=len(clean)),
                    humanValidated=None))
    c.write(out/'metrics.json',dict(policy='AI diagnostic proportions, with known/unknown denominators. Each arm/layer uses one case-bound output; shared candidate judgments are reused, never independent observations. Rejected/null outputs are not delivered successes.',metrics=metrics))
    c.write(out/'summary.json',dict(totalSubjects=len(subjects),byLanguage=dict(collections.Counter(s['language'].lower() for s in subjects)),
        layers=dict(collections.Counter(s['layer'] for s in subjects)),uniqueOriginalPassages=len({s['caseId'] for s in subjects}),
        rawModelGenerations=sum('inference' in r for r in traces),votedOutcomes=sum(r['arm']=='voting-3' for r in traces),
        distinctCandidateTextsWithinSourceCases=len({(s['caseId'],s['candidateText']) for s in subjects if s['candidateText'] is not None}),
        failures=failures,comparisons=dict(structuredDifferences=sum(bool(r['differences']) for r in comparisons),
            booleanDifferences=sum(any(k in BOOLS for k in r['differences']) for r in comparisons),
            booleanContradictions=sum(any(v['preparer'] is not None and v['external'] is not None for k,v in r['differences'].items() if k in BOOLS) for r in comparisons),
            changedConjunction=sum(r['expectedImpact']['preparerPassageConjunction']!=r['expectedImpact']['externalPassageConjunction'] for r in comparisons)),humanApprovals=0))
    c.write(out/'manifest.json',dict(inputs={str(p.relative_to(ROOT)):c.sha(p) for p in [evidence/'original/diorthosis-ai-review.jsonl',BASE/'preparer-ai-decisions.jsonl',BASE/'review-all-candidates/subjects.jsonl',BASE/'review-all-candidates/lineage-DO-NOT-SEND-TO-REVIEWERS.json',BASE/'run/trace.jsonl']},outputs={p.name:c.sha(p) for p in out.iterdir() if p.is_file()}))

if __name__=='__main__':analyze(*map(Path,sys.argv[1:]))
