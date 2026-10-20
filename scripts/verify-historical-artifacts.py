#!/usr/bin/env python3
"""Protect frozen benchmark evidence; documentation may evolve after baseline recording."""
import hashlib
import json
from pathlib import Path
root=Path(__file__).resolve().parents[1]
manifest=json.loads((root/'benchmarks/rehabilitation/v1/historical-hashes.json').read_text())
checked=0
for name,expected in manifest.items():
    if not name.startswith('benchmarks/'):
        continue
    actual=hashlib.sha256((root/name).read_bytes()).hexdigest()
    if actual!=expected:raise SystemExit(f'Historical benchmark artifact changed: {name}')
    checked+=1
print(f'{checked} historical benchmark artifact hashes unchanged.')
