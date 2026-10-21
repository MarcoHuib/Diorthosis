#!/usr/bin/env python3
"""Offline verification of Phase 4 frozen fixtures, attempts, public packet and evidence hashes."""
import importlib
import json
import math
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
    for name,digest in manifest['files'].items():
        p=(BASE/name).resolve()
        if BASE.resolve() not in p.parents or c.sha(p)!=digest:raise ValueError('Evidence changed or escaped: '+name)
    spec=json.loads((BASE/'review-spec.json').read_text());attempts=0;casecount=0
    for entry in spec:
        cases,rows=a.load(ROOT/entry['panel'],[ROOT/p for p in entry['runs']]);attempts+=len(rows);casecount+=len(cases)
        for r in rows:
            if not isinstance(r['promptContentTokens'],int) or r['promptContentTokens']<=0:raise ValueError('Missing measured prompt tokens')
            if not math.isfinite(r['latencyMilliseconds']) or r['latencyMilliseconds']<0 or r['sampledPeakProcessWorkingSetBytes']<=0:raise ValueError('Invalid resources')
            if r['inference'] is not None and r['analysis']['rawOutput']!=r['inference']['text']:raise ValueError('Raw output changed')
            if r['inference'] is None and not r['failure']:raise ValueError('Unrecorded failure')
    review=BASE/'review';packet=json.loads((review/'packet-manifest.json').read_text())
    if packet['sourceCases']!=casecount or packet['generationOccurrences']!=attempts or packet['humanApprovals']!=0:raise ValueError('Misdeclared review coverage')
    with zipfile.ZipFile(review/'chatgpt-review.zip') as z:
        if len(z.infolist())!=4 or set(z.namelist())!={'subjects.jsonl','review.html','CHATGPT-REVIEW.md','packet-manifest.json'}:raise ValueError('Packet leaks lineage or judgments')
        for name,digest in packet['files'].items():
            if c.digest(z.read(name).decode('utf-8'))!=digest or z.read(name)!=(review/name).read_bytes():raise ValueError('Public member mismatch')
    lineage=json.loads((review/'lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text());subjects=c.lines(review/'subjects.jsonl')
    if len({s['subjectId'] for s in subjects})!=packet['subjects'] or set(lineage)!={s['subjectId'] for s in subjects}:raise ValueError('Unbound lineage')
    if (review/'empty-decisions.jsonl').read_bytes() or (review/'empty-adjudications.jsonl').read_bytes():raise ValueError('Fabricated external decisions')
    print(f'Phase 4 verified: 200 frozen passages, {casecount} inference sources, {attempts} generations, {len(subjects)} blinded subjects; zero independent assessments/human approvals.')
if __name__=='__main__':main()
