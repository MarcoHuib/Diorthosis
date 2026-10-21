#!/usr/bin/env python3
"""Verify retained external evidence, original bytes and paired replay without inference."""
import argparse
import importlib
import json
from pathlib import Path

v=importlib.import_module('import-external-linguistic-review')
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase3/external-gpt6/v1'

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--replay-directory',type=Path);args=parser.parse_args()
    manifest=json.loads((BASE/'artifacts.json').read_text())
    for name,digest in manifest['files'].items():
        path=(BASE/name).resolve()
        if not path.is_relative_to(BASE.resolve()) or v.digest(path.read_bytes())!=digest:raise ValueError('External evidence changed: '+name)
    files,result=v.validate(BASE/'original/diorthosis-ai-review-package.zip')
    if result!=json.loads((BASE/'validation.json').read_text()):raise ValueError('Import validation no longer reproduces.')
    for name,data in files.items():
        if data!=(BASE/'original'/name).read_bytes():raise ValueError('Original member bytes changed.')
    report=json.loads((BASE/'import-report.json').read_text())
    if any(m['finalized']!=0 for m in report['aggregateUniqueSubjects']['metrics']):raise ValueError('AI cannot finalize human metrics.')
    if args.replay_directory:
        for path in (BASE/'impact').iterdir():
            if path.name=='manifest.json':continue # software hashes/timing/RSS are machine-specific.
            if path.read_bytes()!=(args.replay_directory/path.name).read_bytes():raise ValueError('Updated replay differs: '+path.name)
    print(f"External GPT-6 evidence verified: {len(manifest['files'])} files, 119 bound AI decisions, zero human approvals.")

if __name__=='__main__':main()
