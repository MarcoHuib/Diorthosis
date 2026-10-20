#!/usr/bin/env python3
"""Replay rights-cleared saved generations without loading a model."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys
root = Path(__file__).resolve().parents[1]
out = Path(sys.argv[1]); out.mkdir(parents=True, exist_ok=False)
cli = root / 'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'
cases = root / 'benchmarks/model-validation/smoke-44-reviewed-v2.jsonl'
sha = lambda path: hashlib.sha256(path.read_bytes()).hexdigest()
runs = []
for name in ['ministral-3b','ministral-3-8b','ministral-3-14b','eurollm-9b']:
    source = root / 'benchmarks/model-validation/results'
    trace = source / ('eurollm-9b-v1/trace.jsonl' if name=='eurollm-9b' else f'v4/{name}-trace.jsonl')
    report = source / ('eurollm-9b-v1/report.json' if name=='eurollm-9b' else f'v4/{name}-report.json')
    subprocess.run(['dotnet',str(cli),'reparse',str(cases),str(trace),str(report),str(out/f'{name}-predictions.jsonl'),str(out/f'{name}-trace.jsonl'),str(out/f'{name}-report.json')], check=True, capture_output=True)
    parsed = json.loads((out/f'{name}-report.json').read_text())
    print(name,parsed['dispositionCounts'],parsed['replaySummary'])
    runs.append(dict(model=name,sourceTraceSha256=sha(trace),sourceReportSha256=sha(report),dispositions=parsed['dispositionCounts'],changes=parsed['replaySummary']))
(out/'manifest.json').write_text(json.dumps(dict(experimentId='safety-replay-1',repositoryCommit=subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip(),
    sourceState='safety working tree; committed after artifact creation',kind='deterministic replay only',datasetSha256=sha(cases),correctionContract='correction-result-2.0.0',
    inferenceTimings='historical, not remeasured',independentSemanticReview=False,runs=runs),indent=2)+'\n')
