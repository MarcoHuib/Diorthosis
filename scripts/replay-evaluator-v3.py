#!/usr/bin/env python3
"""Developer-only deterministic replay; no inference or third-party modules."""
import hashlib
import json
from pathlib import Path
import subprocess
import sys

root = Path(__file__).resolve().parents[1]
out = Path(sys.argv[1])
out.mkdir(parents=True, exist_ok=False)
dataset = root / 'benchmarks/model-validation/smoke-44-reviewed-v2.jsonl'
cli = root / 'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'
sha = lambda path: hashlib.sha256(path.read_bytes()).hexdigest()
runs = []
for name in ['ministral-3b', 'ministral-3-8b', 'ministral-3-14b', 'eurollm-9b']:
    path = root / 'benchmarks/model-validation/results'
    predictions = path / ('eurollm-9b-v1/predictions.jsonl' if name == 'eurollm-9b' else f'v4/{name}-predictions.jsonl')
    source_report = path / ('eurollm-9b-v1/report.json' if name == 'eurollm-9b' else f'v4/{name}-report.json')
    result = subprocess.run(['dotnet', str(cli), 'evaluate-v3', str(dataset), str(predictions)], check=True, capture_output=True)
    (out / f'{name}.json').write_bytes(result.stdout)
    old = subprocess.run(['dotnet', str(cli), 'evaluate', str(dataset), str(predictions)], check=True, capture_output=True)
    expected = json.loads(source_report.read_text())['metrics']
    assert json.loads(old.stdout) == expected, name
    runs.append(dict(model=name, predictions=str(predictions.relative_to(root)), predictionsSha256=sha(predictions),
                     sourceReport=str(source_report.relative_to(root)), sourceReportSha256=sha(source_report),
                     historicalV1Reproduced=True, resultSha256=sha(out/f'{name}.json')))
manifest = dict(experimentId='evaluator-v3-replay-1', kind='deterministic saved-prediction replay; no new inference',
                repositoryCommit=subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip(),
                sourceState='v3 working tree; implementation committed after artifact creation',
                datasetSha256=sha(dataset), evaluator='reference-groups-3.0.0', prompt='historical diorthosis-correction-v3',
                independentLinguisticReview=False, runs=runs)
(out/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
print('Four historical v1 score arrays reproduced exactly; v3 reports written separately.')
