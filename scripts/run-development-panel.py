#!/usr/bin/env python3
"""Developer-only controlled prompt panel on macOS with OS-enforced network denial."""
import json
import os
from pathlib import Path
import subprocess
import sys
root=Path(__file__).resolve().parents[1]
if sys.platform != 'darwin':
    raise SystemExit('This driver uses macOS sandbox-exec. Use equivalent OS network isolation on other hosts; do not claim an offline run without it.')
model=Path(sys.argv[1]).resolve()
out=Path(sys.argv[2]).resolve();out.mkdir(parents=True,exist_ok=False)
cli=root/'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'
fixtures=root/'benchmarks/rehabilitation/v1/experiments'
env=dict(os.environ,DIORTHOSIS_EXPERIMENT_COMMIT=subprocess.check_output(['git','rev-parse','HEAD'],cwd=root,text=True).strip(),
         DOTNET_EnableDiagnostics='0',DOTNET_CLI_TELEMETRY_OPTOUT='1')
for key,sysctl in [('DIORTHOSIS_EXPERIMENT_CPU','machdep.cpu.brand_string'),('DIORTHOSIS_EXPERIMENT_RAM_BYTES','hw.memsize')]:
    env[key]=subprocess.check_output(['sysctl','-n',sysctl],text=True).strip()
for variant,grammar in [('Baseline',False),('LanguageMinimal',False),('FewShot',False),('StructuredOffsets',False),('StructuredFragments',False),('StructuredOffsets',True)]:
    name=variant+('-grammar' if grammar else '')
    command=['/usr/bin/sandbox-exec','-p','(version 1) (allow default) (deny network*)','dotnet',str(cli),'experiment',str(fixtures/'development-panel-8.jsonl'),str(model),str(out/name),variant,'256',str(fixtures/'ministral-3b-provenance.json')]
    if grammar:command.append('--grammar')
    print(name,flush=True)
    with (out/(name+'-native.log')).open('w') as log:
        result=subprocess.run(command,cwd=root,env=env,stderr=log)
    if result.returncode:
        raise SystemExit(f'{name} failed ({result.returncode}); partial manifest/log retained, subsequent configurations not run.')
(out/'isolation.json').write_text(json.dumps(dict(policy='macOS sandbox-exec (deny network*)',scope='all six inference processes',driver='scripts/run-development-panel.py',developmentOnly=True),indent=2)+'\n')
