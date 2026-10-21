#!/usr/bin/env python3
"""Verify frozen Phase 3 evidence and public review-package isolation without inference/network."""
import hashlib
import json
from pathlib import Path
import zipfile

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase3/v1'

def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()

def main():
    manifest=json.loads((BASE/'artifacts.json').read_text())
    for name,digest in manifest['files'].items():
        path=(BASE/name).resolve()
        if not path.is_relative_to(BASE.resolve()) or sha(path)!=digest:
            raise ValueError('Phase 3 artifact changed: '+name)
    rows=[json.loads(line) for line in (BASE/'run/trace.jsonl').read_text().splitlines()]
    if len(rows)!=120 or sum('inference' in r for r in rows)!=100:
        raise ValueError('Inference/voting coverage changed.')
    for row in rows:
        if 'prompt' in row and hashlib.sha256(row['prompt'].encode()).hexdigest()!=row['promptSha256']:
            raise ValueError('Literal prompt binding failed.')
    decisions=[json.loads(line) for line in (BASE/'preparer-ai-decisions.jsonl').read_text().splitlines()]
    if len(decisions)!=119 or any(d['reviewerKind']!='ai' or d['independenceDeclared'] for d in decisions):
        raise ValueError('Preparer judgments must remain explicit, non-independent AI decisions.')
    report=json.loads((BASE/'preparer-import-report.json').read_text())
    if any(m['finalized']!=0 for m in report['aggregateUniqueSubjects']['metrics']):
        raise ValueError('AI feedback cannot create human-finalized metrics.')
    with zipfile.ZipFile(BASE/'chatgpt-review.zip') as archive:
        expected=['CHATGPT-REVIEW.md','subjects.jsonl','review.html']
        if sorted(archive.namelist())!=sorted(expected):raise ValueError('Review archive contains unintended files.')
        for name in expected:
            if archive.read(name)!=(BASE/'review-all-candidates'/name).read_bytes():raise ValueError('Review archive content mismatch.')
    print(f"Phase 3 verified: {len(manifest['files'])} frozen files, 100 generations, 20 votes, 119 AI reviews, zero human approvals.")

if __name__=='__main__':main()
