#!/usr/bin/env python3
"""Offline verification/reproduction of all Phase 4 external AI evidence; never performs inference."""
import argparse
import importlib
import json
from pathlib import Path
import subprocess
import sys

c=importlib.import_module('phase2-analysis')
i=importlib.import_module('integrate-phase4-reviews')
a=importlib.import_module('analyze-phase4-external-reviews')
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase4/v1'
EVIDENCE=ROOT/'benchmarks/phase4/external-gpt6/v1'


def verify(evidence=EVIDENCE, reproduce=None):
    m=json.loads((evidence/'artifact-manifest.json').read_text())
    actual={str(p.relative_to(evidence)) for p in evidence.rglob('*') if p.is_file() and p.name!='artifact-manifest.json'}
    if actual!=set(m['files']):raise ValueError('Unexpected/missing external evidence file')
    for name,digest in m['files'].items():
        p=(evidence/name).resolve()
        if evidence.resolve() not in p.parents or c.sha(p)!=digest:raise ValueError('External evidence changed: '+name)
    for name,digest in m['scripts'].items():
        if c.sha(ROOT/name)!=digest:raise ValueError('Analysis/import implementation changed; regenerate separate evidence version: '+name)
    # Preserve every original tracked Phase 4 byte, even if local untracked copies also exist.
    if c.sha(BASE/'artifact-manifest.json')!=m['frozenManifestSha256']:raise ValueError('Frozen manifest changed')
    frozen=json.loads((BASE/'artifact-manifest.json').read_text())
    for name,digest in frozen['files'].items():
        if c.sha(BASE/name)!=digest:raise ValueError('Frozen Phase 4 changed: '+name)
    parent=c.lines(BASE/'review/subjects.jsonl');batches=[];decisions=[];errorrows=[]
    for number in range(1,19):
        name=f'batch-{number:02}';batch=evidence/'batches'/name;original=batch/'original';packet=BASE/'review/batches'/name
        files=i.archive_files(original/f'diorthosis-review-{name}.zip')
        for filename,data in files.items():
            if data!=(original/filename).read_bytes():raise ValueError('Original extraction changed: '+name+'/'+filename)
        v=i.imp.validate(packet,original/'decisions.jsonl',original/'provenance.json')
        saved=json.loads((batch/'validation.json').read_text())
        if v['decisions']!=saved['validDecisions'] or v['missingSubjects']!=saved['missingSubjectIds']:raise ValueError('Batch validation changed')
        imported=batch/'imported'
        for src,dst in [('decisions.jsonl','original-decisions.jsonl'),('provenance.json','original-provenance.json')]:
            if (original/src).read_bytes()!=(imported/dst).read_bytes():raise ValueError('Imported original changed')
        ep=evidence/saved['importedErrorFile']
        if c.sha(ep)!=saved['importedErrorSha256'] or ep.read_bytes()!=(imported/'original-error-decisions.jsonl').read_bytes():raise ValueError('Imported candidate error subset changed')
        d=c.lines(original/'decisions.jsonl');e=i.imp.validate_errors(packet,ep,{x['subjectId'] for x in d})
        i.validate_error_consistency(d,e)
        decisions+=d;errorrows+=e;batches.append(c.lines(packet/'subjects.jsonl'))
        if (imported/'empty-adjudications.jsonl').read_bytes():raise ValueError('Fabricated adjudications')
        report=json.loads((imported/'import-report.json').read_text())
        if any(r['finalized'] for r in report['aggregateUniqueSubjects']['metrics']):raise ValueError('AI finalized human metrics')
    if i.reconcile(parent,batches):raise ValueError('Missing parent subjects')
    if len(decisions)!=1075 or len({d['subjectId'] for d in decisions})!=1075 or len(errorrows)!=478:raise ValueError('Coverage changed')
    if decisions!=c.lines(evidence/'combined/decisions.jsonl') or errorrows!=c.lines(evidence/'combined/error-decisions.jsonl'):raise ValueError('Combined evidence changed')
    if (evidence/'combined/empty-adjudications.jsonl').read_bytes():raise ValueError('Fabricated combined adjudications')
    report=json.loads((evidence/'combined/import-report.json').read_text())
    if any(r['finalized'] for r in report['aggregateUniqueSubjects']['metrics']):raise ValueError('Combined AI finalized human metrics')
    occurrences=c.lines(evidence/'analysis/occurrences.jsonl')
    if len(occurrences)!=920 or len({(r['caseId'],r['promptVersion']) for r in occurrences})!=920:raise ValueError('Occurrence coverage changed')
    if any(r['fullyAcceptableDelivered'] is True or any(v is True for v in r['deliveredErrorRepairs']) for r in occurrences if not r['eligible']):raise ValueError('Abstention credited as success')
    if reproduce is not None:
        i.integrate(evidence,reproduce)
        a.analyze(reproduce,reproduce/'analysis')
        expected={p.name:p.read_bytes() for p in (evidence/'analysis').iterdir() if p.is_file() and p.name!='manifest.json'}
        actual={p.name:p.read_bytes() for p in (reproduce/'analysis').iterdir() if p.is_file() and p.name!='manifest.json'}
        if expected!=actual:raise ValueError('Analysis replay differs: '+str([n for n in expected if expected.get(n)!=actual.get(n)]))
        if (evidence/'combined/import-report.json').read_bytes()!=(reproduce/'combined/import-report.json').read_bytes():raise ValueError('Combined C# replay differs')
    print('Verified 18 original ZIPs, 1075/1075 bound AI subjects, 478 candidate/error decisions, 920 occurrence mappings; frozen bytes unchanged; zero human approvals.'+(' Full import/C# analysis reproduction matched.' if reproduce else ''))


if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--reproduce',type=Path)
    args=p.parse_args();verify(reproduce=args.reproduce.resolve() if args.reproduce else None)
