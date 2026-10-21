#!/usr/bin/env python3
"""Offline verification of Phase 4 frozen fixtures, attempts, public packet and evidence hashes."""
import importlib
import json
import math
import datetime
from pathlib import Path
import sys
import zipfile
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase4/v1'
c=importlib.import_module('phase2-analysis');a=importlib.import_module('analyze-prompt-quality')
def main():
    frozen=json.loads((BASE/'freeze.json').read_text())
    for name,digest in frozen['files'].items():
        if c.sha(BASE/name)!=digest:raise ValueError('Frozen input changed: '+name)
    manifest=json.loads((BASE/'artifact-manifest.json').read_text())
    actual={str(p.relative_to(BASE)) for p in BASE.rglob('*') if p.is_file() and p.name!='artifact-manifest.json'}
    if actual!=set(manifest['files']):raise ValueError('Unexpected or missing evidence file')
    for name,digest in manifest['files'].items():
        p=(BASE/name).resolve()
        if BASE.resolve() not in p.parents or c.sha(p)!=digest:raise ValueError('Evidence changed or escaped: '+name)
    spec=json.loads((BASE/'review-spec.json').read_text());attempts=0;casecount=0;identities=[]
    for entry in spec:
        cases,rows=a.load(ROOT/entry['panel'],[ROOT/p for p in entry['runs']]);attempts+=len(rows);casecount+=len(cases)
        for path in entry['runs']:
            m=json.loads((ROOT/path/'manifest.json').read_text())
            identities.append({k:m[k] for k in ('provenance','settings','runtime','nativeHashes','assemblyHashes')})
        for r in rows:
            if not isinstance(r['promptContentTokens'],int) or r['promptContentTokens']<=0:raise ValueError('Missing measured prompt tokens')
            if not math.isfinite(r['latencyMilliseconds']) or r['latencyMilliseconds']<0 or r['sampledPeakProcessWorkingSetBytes']<=0:raise ValueError('Invalid resources')
            if r['inference'] is not None and r['analysis']['rawOutput']!=r['inference']['text']:raise ValueError('Raw output changed')
            if r['inference'] is None and not r['failure']:raise ValueError('Unrecorded failure')
    if attempts!=920 or casecount!=180 or any(i!=identities[0] for i in identities[1:]):raise ValueError('Unexpected coverage or cross-stage identity change')
    lock=json.loads((BASE/'candidate-lock.json').read_text());confirmation=json.loads((BASE/'confirmation-run/manifest.json').read_text())
    if lock['candidate']!='phase4-LiteralJson-Original-1.0.0' or lock['confirmationPanelSha256']!=confirmation['datasetSha256'] or lock['confirmationInferredBeforeLock'] is not False:raise ValueError('Invalid candidate lock')
    if datetime.datetime.fromisoformat(lock['lockedUtc'])>=datetime.datetime.fromisoformat(confirmation['startedUtc']):raise ValueError('Confirmation ran before candidate lock')
    if {r['promptVersion'] for r in confirmation['arms']}!={lock['candidate'],lock['productionControl']}:raise ValueError('Unlocked confirmation arm')
    source=json.loads((BASE/'representation/manifest.json').read_text())['softwareIdentity']['sourceSha256']
    if c.sha(ROOT/'src/Diorthosis.Core/Corrections/CorrectionPrompt.cs')!=source['src/Diorthosis.Core/Corrections/CorrectionPrompt.cs']:raise ValueError('Frozen production v3 changed')
    for name,count in (('representation',420),('isolated',420),('confirmation',80)):
        replay=json.loads((BASE/'replay'/f'{name}.json').read_text())
        if replay['attempts']!=count or replay['mismatches']:raise ValueError('Replay mismatch')
    review=BASE/'review';packet=json.loads((review/'packet-manifest.json').read_text())
    if packet['sourceCases']!=casecount or packet['generationOccurrences']!=attempts or packet['humanApprovals']!=0:raise ValueError('Misdeclared review coverage')
    with zipfile.ZipFile(review/'chatgpt-review.zip') as z:
        if len(z.infolist())!=4 or set(z.namelist())!={'subjects.jsonl','review.html','CHATGPT-REVIEW.md','packet-manifest.json'}:raise ValueError('Packet leaks lineage or judgments')
        for name,digest in packet['files'].items():
            if c.digest(z.read(name).decode('utf-8'))!=digest or z.read(name)!=(review/name).read_bytes():raise ValueError('Public member mismatch')
    lineage=json.loads((review/'lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text());subjects=c.lines(review/'subjects.jsonl')
    if len({s['subjectId'] for s in subjects})!=packet['subjects'] or set(lineage)!={s['subjectId'] for s in subjects}:raise ValueError('Unbound lineage')
    if (review/'empty-decisions.jsonl').read_bytes() or (review/'empty-adjudications.jsonl').read_bytes():raise ValueError('Fabricated external decisions')
    batch_ids=set();batch_cases=set()
    for directory in sorted((review/'batches').iterdir()):
        with zipfile.ZipFile(directory/'chatgpt-review.zip') as z:
            if len(z.infolist())!=4 or set(z.namelist())!={'subjects.jsonl','review.html','CHATGPT-REVIEW.md','packet-manifest.json'}:raise ValueError('Batch leaks lineage')
            m=json.loads(z.read('packet-manifest.json'))
            if m['parentSubjectsSha256']!=packet['files']['subjects.jsonl']:raise ValueError('Wrong batch parent')
            for name,digest in m['files'].items():
                if c.digest(z.read(name).decode('utf-8'))!=digest:raise ValueError('Batch member hash mismatch')
            batch=[json.loads(s) for s in z.read('subjects.jsonl').decode('utf-8').splitlines()]
            ids={s['subjectId'] for s in batch};case_ids={s['caseId'] for s in batch}
            if batch_ids&ids or batch_cases&case_ids or len(case_ids)>10:raise ValueError('Overlapping/oversized batch')
            batch_ids|=ids;batch_cases|=case_ids
    if batch_ids!=set(lineage) or len(batch_cases)!=180:raise ValueError('Incomplete review batches')
    ai=json.loads((BASE/'confirmation-preparer-import/import-report.json').read_text())
    if any(m['finalized'] for m in ai['aggregateUniqueSubjects']['metrics']):raise ValueError('AI finalized human metrics')
    print(f'Phase 4 verified: 200 frozen passages, {casecount} inference sources, {attempts} generations, {len(subjects)} blinded subjects; zero independent assessments/human approvals.')
if __name__=='__main__':main()
