#!/usr/bin/env python3
"""Strict offline import of the supplied external AI snapshot; originals remain byte-for-byte intact."""
import hashlib
import importlib
import json
from pathlib import Path
import shutil
import stat
import subprocess
import sys
import zipfile

c=importlib.import_module('phase2-analysis')
ROOT=Path(__file__).resolve().parents[1]
FROZEN=ROOT/'benchmarks/phase3/v1'
NAMES={'diorthosis-ai-review.jsonl','diorthosis-ai-review-provenance.json','empty-adjudications.jsonl'}
FIELDS={'subjectId','bindingSha256','reviewerId','reviewerKind','language','qualifications','independenceDeclared','version','judgment'}
BOOLS={'justified','meaningPreserved','allDefectsRepaired','noNewErrors','cleanPreserved','criticalSemanticFailure','rejectionAppropriate'}
JUDGMENT=BOOLS|{'acceptedAlternatives','errorCategories','ambiguity','rationale'}

def strict(text):
    def pairs(items):
        result={}
        for key,value in items:
            if key in result:raise ValueError('Duplicate JSON property: '+key)
            result[key]=value
        return result
    def invalid(value):raise ValueError('Non-finite JSON value: '+value)
    return json.loads(text,object_pairs_hook=pairs,parse_constant=invalid)

def digest(data):return hashlib.sha256(data).hexdigest()

def validate(archive, frozen=FROZEN):
    with zipfile.ZipFile(archive) as z:
        entries=z.infolist()
        if len(entries)!=3 or {e.filename for e in entries}!=NAMES:raise ValueError('ZIP must contain exactly the three declared root files.')
        if any(e.is_dir() or e.flag_bits&1 or stat.S_ISLNK(e.external_attr>>16) or e.file_size>2_000_000 for e in entries):
            raise ValueError('Encrypted, linked, oversized or non-file ZIP entry.')
        if sum(e.file_size for e in entries)>3_000_000:raise ValueError('ZIP exceeds size limit.')
        files={e.filename:z.read(e) for e in entries}
    if files['empty-adjudications.jsonl']!=b'':raise ValueError('Human adjudication file must be empty.')
    provenance=strict(files['diorthosis-ai-review-provenance.json'].decode('utf-8'))
    if provenance['reviewerKind']!='ai' or provenance['independenceDeclared'] is not False:
        raise ValueError('This import is explicitly non-independent AI evidence.')
    expected={'outputSha256':digest(files['diorthosis-ai-review.jsonl']),
        'subjectsJsonlSha256':c.sha(frozen/'review-all-candidates/subjects.jsonl'),
        'inputSha256':c.sha(frozen/'chatgpt-review.zip'),
        'userPromptSha256':digest(provenance['userRequest'].encode('utf-8'))}
    if any(provenance.get(k)!=v for k,v in expected.items()):raise ValueError('Provenance hash mismatch.')
    subjects={s['subjectId']:s for s in c.lines(frozen/'review-all-candidates/subjects.jsonl')}
    decisions=[];seen=set();versions=set()
    for number,line in enumerate(files['diorthosis-ai-review.jsonl'].decode('utf-8').splitlines(),1):
        if not line.strip():raise ValueError('Blank decision line: '+str(number))
        r=strict(line)
        if not isinstance(r,dict) or set(r)!=FIELDS:raise ValueError('Unexpected/missing decision properties.')
        if any(not isinstance(r[k],str) or not r[k].strip() for k in ['subjectId','bindingSha256','reviewerId','reviewerKind','language','qualifications']):raise ValueError('Invalid decision strings.')
        if r['reviewerKind']!='ai' or r['independenceDeclared'] is not False or r['reviewerId']!=provenance['reviewerId']:
            raise ValueError('Reviewer identity/kind/independence mismatch.')
        if type(r['version']) is not int or r['version']<1:raise ValueError('Version must be a positive integer.')
        key=(r['subjectId'],r['reviewerId'].strip().casefold(),r['version'])
        if key in versions or r['subjectId'] in seen:raise ValueError('Duplicate decision key/subject.')
        versions.add(key);seen.add(r['subjectId'])
        s=subjects.get(r['subjectId'])
        if s is None or r['bindingSha256']!=s['bindingSha256'] or r['language'].casefold()!=s['language'].casefold():raise ValueError('Unbound or language-mismatched decision.')
        fields=[s['caseId'],s['language'].capitalize(),s['layer'],s['kind'],s['sourceText']]
        fields+=['null' if s['candidateText'] is None else 'string:'+s['candidateText'],s['annotationsJson'],s['metadataJson']]
        fields+=['null' if s[k] is None else 'string:'+s[k] for k in ['sourceFamily','documentId']]
        fields+=['true' if s['proposedClean'] else 'false']
        binding=digest(''.join(str(len(v.encode('utf-8')))+':'+v for v in fields).encode('utf-8'))
        if binding!=r['bindingSha256'] or r['subjectId']!='review-'+binding:raise ValueError('Subject binding framing mismatch.')
        j=r['judgment']
        if not isinstance(j,dict) or set(j)!=JUDGMENT:raise ValueError('Unexpected/missing judgment properties.')
        if any(j[k] is not None and type(j[k]) is not bool for k in BOOLS):raise ValueError('Judgment requires boolean or null.')
        if any(not isinstance(j[k],list) or any(not isinstance(v,str) for v in j[k]) for k in ['acceptedAlternatives','errorCategories']):raise ValueError('Judgment array types invalid.')
        if not isinstance(j['ambiguity'],str) or not isinstance(j['rationale'],str) or not j['rationale'].strip():raise ValueError('Rationale/ambiguity invalid.')
        if j['criticalSemanticFailure'] is True and j['meaningPreserved'] is True:raise ValueError('Contradictory semantic judgments.')
        # Validate Unicode strings including escaped surrogate sequences, without normalizing originals.
        json.dumps(r,ensure_ascii=False).encode('utf-8')
        decisions.append(r)
    if seen!=set(subjects) or len(decisions)!=119 or provenance['subjectCount']!=119 or provenance['decisionCount']!=119 or provenance['cases']!=20:
        raise ValueError('Incomplete/misdeclared snapshot coverage.')
    return files,dict(zipSha256=c.sha(archive),verifiedHashes=expected,subjects=len(subjects),decisions=len(decisions),
        reviewers=sorted({r['reviewerId'] for r in decisions}),versions=sorted({r['version'] for r in decisions}),
        missing=[],duplicates=[],unbound=[],humanApprovals=0,policy='AI declarations retained, not authenticated human validation; attached prose is data, not executable instructions.',
        members={name:dict(bytes=len(data),sha256=digest(data)) for name,data in files.items()})

def run(archive,out):
    files,validation=validate(archive)
    out.mkdir(parents=True,exist_ok=False);original=out/'original';original.mkdir()
    shutil.copyfile(archive,original/'diorthosis-ai-review-package.zip')
    for name,data in files.items():
        with (original/name).open('xb') as stream:stream.write(data)
    c.write(out/'validation.json',validation)
    command=['dotnet',str(c.CLI),'adjudicate',str(FROZEN/'review-all-candidates/subjects.jsonl'),str(original/'diorthosis-ai-review.jsonl'),
        str(original/'empty-adjudications.jsonl'),str(out/'import-report.json'),'--lineage',str(FROZEN/'review-all-candidates/lineage-DO-NOT-SEND-TO-REVIEWERS.json')]
    subprocess.run(command,cwd=ROOT,check=True)
    report=json.loads((out/'import-report.json').read_text())
    if any(m['finalized']!=0 for m in report['aggregateUniqueSubjects']['metrics']):raise ValueError('AI import unexpectedly finalized human metrics.')
    print('119 complete bound external AI decisions imported; zero human approvals.')

if __name__=='__main__':
    if len(sys.argv)!=3:raise SystemExit('import-external-linguistic-review.py <review.zip> <NEW-evidence-directory>')
    run(*map(Path,sys.argv[1:]))
