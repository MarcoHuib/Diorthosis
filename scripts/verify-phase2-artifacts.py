#!/usr/bin/env python3
"""Check frozen Phase 2 evidence and the 165-file inherited baseline, without inference."""
import hashlib
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase2/v1'
def sha(path):return hashlib.sha256(path.read_bytes()).hexdigest()
def require_hash(path,expected):
    if not path.is_file() or sha(path)!=expected:raise SystemExit('Frozen evidence mismatch: '+str(path.relative_to(ROOT)))
old=json.loads((BASE/'baseline-artifacts.json').read_text())
for name,expected in old['files'].items():require_hash(ROOT/name,expected)
manifest=BASE/'manifest.json'
if manifest.exists():
    current=json.loads(manifest.read_text())
    for name,expected in current['files'].items():require_hash(BASE/name,expected)
review=BASE/'review-analysis/manifest.json'
if review.exists():
    packet=json.loads(review.read_text())
    for name,expected in packet['files'].items():require_hash(review.parent/name,expected)
    for name,expected in packet['sourceHashes'].items():require_hash(ROOT/name,expected)
print(f"Phase 2 evidence verified; {len(old['files'])} inherited artifacts unchanged.")
