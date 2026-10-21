#!/usr/bin/env python3
"""Explicit developer experiment, existing weights only; entire process has network denied."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
ROOT=Path(__file__).resolve().parents[1]
POLICY='(version 1) (allow default) (deny network*)'
def main():
    if sys.platform!='darwin' or len(sys.argv)!=5:
        raise SystemExit('run-prompt-quality.py <existing.gguf> <selection|confirmation> <Representation:Instruction,...> <NEW-output-directory>')
    model=Path(sys.argv[1]).resolve();split=sys.argv[2];arms=sys.argv[3];out=Path(sys.argv[4]).resolve()
    if split not in ('selection','confirmation') or out.exists(): raise SystemExit('Invalid split or existing output.')
    panel=ROOT/'benchmarks/phase4/v1'/f'{split}.jsonl'
    probe=subprocess.run(['/usr/bin/sandbox-exec','-p',POLICY,sys.executable,'-c','import socket; socket.create_connection(("127.0.0.1",9),timeout=1)'],capture_output=True,text=True)
    if probe.returncode==0 or 'Operation not permitted' not in probe.stderr: raise SystemExit('Network denial preflight failed: '+probe.stderr)
    files=subprocess.check_output(['git','ls-files','src','scripts','Directory.Build.props'],cwd=ROOT,text=True).splitlines()
    files+=['src/Diorthosis.Experiments/PromptQuality.cs','src/Diorthosis.Benchmarks/PromptQualityExperiment.cs']
    identity=dict(commit=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT,text=True).strip(),
        worktreeStatus=subprocess.check_output(['git','status','--porcelain'],cwd=ROOT,text=True),
        sourceSha256={p:hashlib.sha256((ROOT/p).read_bytes()).hexdigest() for p in sorted(set(files)) if (ROOT/p).is_file()})
    out.parent.mkdir(parents=True,exist_ok=True)
    identity_path=out.parent/(out.name+'-software.json')
    with identity_path.open('x') as f: json.dump(identity,f,indent=2);f.write('\n')
    env=dict(os.environ,DOTNET_EnableDiagnostics='0',DOTNET_CLI_TELEMETRY_OPTOUT='1')
    for name,key in [('DIORTHOSIS_EXPERIMENT_CPU','machdep.cpu.brand_string'),('DIORTHOSIS_EXPERIMENT_RAM_BYTES','hw.memsize')]: env[name]=subprocess.check_output(['sysctl','-n',key],text=True).strip()
    cmd=['/usr/bin/sandbox-exec','-p',POLICY,'dotnet',str(ROOT/'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'),
        'prompt-quality',str(panel),str(model),str(ROOT/'benchmarks/rehabilitation/v1/experiments/ministral-3b-provenance.json'),arms,str(out),str(panel.parent/'freeze.json'),str(identity_path)]
    with (out.parent/(out.name+'-native.log')).open('x') as log:
        result=subprocess.run(cmd,cwd=ROOT,env=env,stderr=log)
    if out.exists():
        with (out/'isolation.json').open('x') as f:json.dump(dict(policy=POLICY,mechanism='macOS sandbox-exec',preflight='loopback connect denied with EPERM',inferenceExitCode=result.returncode,scope='entire inference process'),f,indent=2);f.write('\n')
    raise SystemExit(result.returncode)
if __name__=='__main__':main()
