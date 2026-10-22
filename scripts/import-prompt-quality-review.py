#!/usr/bin/env python3
"""Strict offline AI-decision preflight, retaining originals; no human finalization."""
import importlib
import json
from pathlib import Path
import shutil
import subprocess
import sys
c=importlib.import_module('phase2-analysis')
existing=importlib.import_module('import-external-linguistic-review')
ROOT=Path(__file__).resolve().parents[1]
def validate(review,decisions_path,provenance_path):
    subjects=c.lines(review/'subjects.jsonl');subject_map={s['subjectId']:s for s in subjects}
    if len(subject_map)!=len(subjects):raise ValueError('Duplicate subjects')
    p=existing.strict(provenance_path.read_text())
    required={'reviewerId','reviewerKind','independenceDeclared','modelBuild','evaluationDate','priorExposure','reviewLimitations','reviewRequest','reviewRequestSha256','subjectsSha256','decisionsSha256','subjectCount','decisionCount','completeCoverage'}
    if not required<=set(p):raise ValueError('Missing provenance fields: '+str(required-set(p)))
    if p['reviewerKind']!='ai' or p['independenceDeclared'] is not False:raise ValueError('AI evidence must remain explicitly AI and non-human-independent')
    if any(not isinstance(p[k],str) or not p[k].strip() for k in ('reviewerId','modelBuild','evaluationDate','priorExposure','reviewLimitations','reviewRequest')):raise ValueError('Invalid provenance strings')
    if p['subjectsSha256']!=c.sha(review/'subjects.jsonl') or p['decisionsSha256']!=c.sha(decisions_path) or p['reviewRequestSha256']!=c.digest(p['reviewRequest']):raise ValueError('Provenance hash mismatch')
    if type(p['completeCoverage']) is not bool or type(p['subjectCount']) is not int or type(p['decisionCount']) is not int:raise ValueError('Invalid coverage declaration')
    decisions=[];seen=set()
    for line in decisions_path.read_text().splitlines():
        if not line.strip():raise ValueError('Blank decision line')
        r=existing.strict(line)
        if not isinstance(r,dict) or set(r)!=existing.FIELDS:raise ValueError('Unexpected decision schema')
        if any(not isinstance(r[k],str) or not r[k].strip() for k in ('subjectId','bindingSha256','reviewerId','reviewerKind','language','qualifications')):raise ValueError('Invalid decision strings')
        if r['reviewerId']!=p['reviewerId'] or r['reviewerKind']!='ai' or r['independenceDeclared'] is not False:raise ValueError('Identity/kind mismatch')
        if type(r['version']) is not int or r['version']<1:raise ValueError('Invalid version')
        if r['subjectId'] in seen:raise ValueError('Duplicate subject decision')
        seen.add(r['subjectId']);s=subject_map.get(r['subjectId'])
        if s is None or r['bindingSha256']!=s['bindingSha256'] or r['language'].casefold()!=s['language'].casefold():raise ValueError('Unbound decision')
        # Recreate the existing C# byte-framed binding, preserving literal annotation/metadata JSON.
        case=dict(id=s['caseId'],language=s['language'].lower(),inputText=s['sourceText'],errors=[] if s['proposedClean'] else [{}],sourceFamily=s['sourceFamily'],documentId=s['documentId'])
        framed=[s['caseId'],s['language'].capitalize(),s['layer'],s['kind'],s['sourceText'],
            'null' if s['candidateText'] is None else 'string:'+s['candidateText'],s['annotationsJson'],s['metadataJson'],
            'null' if s['sourceFamily'] is None else 'string:'+s['sourceFamily'],'null' if s['documentId'] is None else 'string:'+s['documentId'],
            'true' if s['proposedClean'] else 'false']
        binding=c.digest(''.join(str(len(v.encode('utf-8')))+':'+v for v in framed))
        if binding!=r['bindingSha256'] or r['subjectId']!='review-'+binding:raise ValueError('Stale subject binding')
        j=r['judgment']
        if not isinstance(j,dict) or set(j)!=existing.JUDGMENT:raise ValueError('Unexpected judgment schema')
        if any(j[k] is not None and type(j[k]) is not bool for k in existing.BOOLS):raise ValueError('Invalid judgment booleans')
        if any(not isinstance(j[k],list) or any(not isinstance(v,str) for v in j[k]) for k in ('acceptedAlternatives','errorCategories')):raise ValueError('Invalid judgment arrays')
        if not isinstance(j['ambiguity'],str) or not isinstance(j['rationale'],str) or not j['rationale'].strip():raise ValueError('Missing rationale')
        if j['meaningPreserved'] is True and j['criticalSemanticFailure'] is True:raise ValueError('Contradictory semantic judgment')
        json.dumps(r,ensure_ascii=False).encode('utf-8');decisions.append(r)
    if p['subjectCount']!=len(subjects) or p['decisionCount']!=len(decisions) or not decisions:raise ValueError('Misdeclared or empty coverage')
    if p['completeCoverage']!=(seen==set(subject_map)):raise ValueError('Incomplete/misdeclared snapshot')
    return dict(version='phase4-ai-import-1.0.0',subjects=len(subjects),decisions=len(decisions),missingSubjects=sorted(set(subject_map)-seen),
        complete=p['completeCoverage'],humanApprovals=0,subjectsSha256=c.sha(review/'subjects.jsonl'),decisionsSha256=c.sha(decisions_path),
        provenanceSha256=c.sha(provenance_path),reviewer=p['reviewerId'])
def validate_errors(review,path,assessed_subjects):
    subjects={s['subjectId']:s for s in c.lines(review/'subjects.jsonl')}
    seen=set();rows=[]
    for line in path.read_text().splitlines():
        r=existing.strict(line)
        if not isinstance(r,dict) or set(r)!={'subjectId','bindingSha256','annotationIndex','repaired','rationale'}:raise ValueError('Invalid per-error schema')
        s=subjects.get(r['subjectId'])
        if s is None or r['subjectId'] not in assessed_subjects or s['bindingSha256']!=r['bindingSha256'] or s['kind'] not in ('passage','rejection'):raise ValueError('Unbound per-error judgment')
        annotations=existing.strict(s['annotationsJson'])
        if type(r['annotationIndex']) is not int or r['annotationIndex']<0 or r['annotationIndex']>=len(annotations):raise ValueError('Invalid annotation index')
        if r['repaired'] is not None and type(r['repaired']) is not bool or not isinstance(r['rationale'],str) or not r['rationale'].strip():raise ValueError('Invalid per-error judgment')
        key=(r['subjectId'],r['annotationIndex'])
        if key in seen:raise ValueError('Duplicate per-error judgment')
        seen.add(key);json.dumps(r,ensure_ascii=False).encode('utf-8');rows.append(r)
    return rows

def run(review,decisions,provenance,out,error_path=None):
    result=validate(review,decisions,provenance)
    errors=validate_errors(review,error_path,{r['subjectId'] for r in c.lines(decisions)}) if error_path else None
    if error_path:
        declared=existing.strict(provenance.read_text()).get('errorDecisionsSha256')
        if declared is not None and declared!=c.sha(error_path):raise ValueError('Supplementary error provenance hash mismatch')
    out.mkdir(parents=True,exist_ok=False)
    shutil.copyfile(decisions,out/'original-decisions.jsonl');shutil.copyfile(provenance,out/'original-provenance.json')
    (out/'empty-adjudications.jsonl').write_bytes(b'');c.write(out/'validation.json',result)
    if errors is not None:
        shutil.copyfile(error_path,out/'original-error-decisions.jsonl')
        c.write(out/'error-validation.json',dict(decisions=len(errors),sha256=c.sha(error_path),humanApprovals=0,
            policy='Bound supplementary AI error judgments; partial coverage explicit. Repaired local errors in withheld candidates are not delivered repairs. Do not infer full-passage meaning or acceptability from local repairs.'))
    subprocess.run(['dotnet',str(c.CLI),'adjudicate',str(review/'subjects.jsonl'),str(out/'original-decisions.jsonl'),str(out/'empty-adjudications.jsonl'),str(out/'import-report.json'),
        '--lineage',str(review/'lineage-DO-NOT-SEND-TO-REVIEWERS.json')],cwd=ROOT,check=True)
    report=json.loads((out/'import-report.json').read_text())
    if any(m['finalized'] for m in report['aggregateUniqueSubjects']['metrics']):raise ValueError('AI finalized human metrics')
    print(json.dumps({**{k:v for k,v in result.items() if k!='missingSubjects'},'missingSubjectCount':len(result['missingSubjects'])},indent=2))
if __name__=='__main__':
    if len(sys.argv) not in (5,7) or len(sys.argv)==7 and sys.argv[5]!='--error-decisions':raise SystemExit('import-prompt-quality-review.py <review-directory> <decisions.jsonl> <provenance.json> <NEW-evidence-directory> [--error-decisions <error-decisions.jsonl>]')
    run(*[Path(p).resolve() for p in sys.argv[1:5]],error_path=Path(sys.argv[6]).resolve() if len(sys.argv)==7 else None)
