#!/usr/bin/env python3
"""Validate already-published Phase 4 AOT prompt contracts; existing dictionaries, no downloads."""
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
ROOT=Path(__file__).resolve().parents[1]
def main():
    if sys.platform!='darwin' or len(sys.argv)!=5:raise SystemExit('run-phase4-native-validation.py <AOT-probe> <existing-dictionaries> <AOT-cli> <NEW-output-directory>')
    probe,assets,cli,out=[Path(p).resolve() for p in sys.argv[1:]]
    out.mkdir(parents=True,exist_ok=False)
    env=dict(os.environ,DOTNET_ROOT='/private/tmp/no-diorthosis-dotnet-runtime',DOTNET_MULTILEVEL_LOOKUP='0',DOTNET_EnableDiagnostics='0')
    prefix=['/usr/bin/sandbox-exec','-p','(version 1) (allow default) (deny network*)']
    tests=[([str(probe),str(assets)],'prompt-probe'),([str(cli),'--version'],'cli-version'),([str(cli),'--check-native-inference'],'cli-native')]
    checks=[]
    for command,label in tests:
        r=subprocess.run(prefix+command,env=env,capture_output=True,text=True,timeout=120)
        (out/(label+'.stdout')).write_text(r.stdout);(out/(label+'.stderr')).write_text(r.stderr)
        checks.append(dict(label=label,exitCode=r.returncode))
        if r.returncode:raise SystemExit('Native validation failed: '+label)
    result=dict(kind='model-free Native AOT prompt/dictionary/CPU-backend checks, not linguistic quality inference',
        promptProbeSha256=hashlib.sha256(probe.read_bytes()).hexdigest(),cliSha256=hashlib.sha256(cli.read_bytes()).hexdigest(),
        networkPolicy=prefix[2],dotnetRoot=env['DOTNET_ROOT'],checks=checks)
    (out/'result.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
if __name__=='__main__':main()
