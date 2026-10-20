#!/usr/bin/env python3
"""Explicit developer experiment; no downloads. Inference under macOS network denial."""
import json
import os
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
POLICY = '(version 1) (allow default) (deny network*)'

def main():
    if sys.platform != 'darwin' or len(sys.argv) != 4:
        raise SystemExit('Usage on macOS: run-linguistic-experiment.py <existing.gguf> <reviewed-assets-directory> <NEW-output-directory>')
    model, assets, out = map(lambda s: Path(s).resolve(), sys.argv[1:])
    if out.exists():
        raise SystemExit('Output must be new.')
    probe = subprocess.run(['/usr/bin/sandbox-exec', '-p', POLICY, sys.executable, '-c',
        'import socket; socket.create_connection(("127.0.0.1",9),timeout=1)'], capture_output=True, text=True)
    if probe.returncode == 0 or 'Operation not permitted' not in probe.stderr:
        raise SystemExit('Network denial preflight was not established: ' + probe.stderr)
    env = dict(os.environ, DOTNET_EnableDiagnostics='0', DOTNET_CLI_TELEMETRY_OPTOUT='1',
        DIORTHOSIS_EXPERIMENT_COMMIT=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip())
    for name, key in [('DIORTHOSIS_EXPERIMENT_CPU','machdep.cpu.brand_string'),('DIORTHOSIS_EXPERIMENT_RAM_BYTES','hw.memsize')]:
        env[name] = subprocess.check_output(['sysctl','-n',key],text=True).strip()
    command = ['/usr/bin/sandbox-exec','-p',POLICY,'dotnet',str(ROOT/'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'),
        'linguistic-experiment',str(ROOT/'benchmarks/phase3/v1/panel.jsonl'),str(model),str(assets),str(ROOT/'benchmarks/phase3/v1/assets.json'),
        str(ROOT/'benchmarks/rehabilitation/v1/experiments/ministral-3b-provenance.json'),str(out)]
    with (out.parent/(out.name+'-native.log')).open('x') as stderr:
        result = subprocess.run(command,cwd=ROOT,env=env,stderr=stderr)
    if out.exists():
        (out/'isolation.json').write_text(json.dumps(dict(policy=POLICY,mechanism='macOS sandbox-exec',preflight='loopback connect denied with EPERM',
            inferenceExitCode=result.returncode,scope='entire dictionary and inference process',driver='scripts/run-linguistic-experiment.py'),indent=2)+'\n')
    raise SystemExit(result.returncode)

if __name__ == '__main__':
    main()
