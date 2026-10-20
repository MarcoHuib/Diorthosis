#!/usr/bin/env python3
"""Explicit developer acquisition; never invoked by the app or experiment runner."""
import base64
import hashlib
import json
from pathlib import Path
import sys
import urllib.request
import zipfile

ROOT = Path(__file__).resolve().parents[1]

def main():
    if len(sys.argv) != 2:
        raise SystemExit('Usage: acquire-linguistic-assets.py <NEW-local-directory>; read ADR 0015 licenses first')
    out = Path(sys.argv[1]).resolve()
    out.mkdir(parents=True,exist_ok=False)
    manifest = json.loads((ROOT/'benchmarks/phase3/v1/assets.json').read_text())
    for row in manifest['files']:
        path = out/row['path']
        path.parent.mkdir(parents=True,exist_ok=True)
        if 'archive' in row:
            with zipfile.ZipFile(out/row['archive']) as archive:
                data = archive.read(Path(row['path']).name)
        else:
            try:
                data = urllib.request.urlopen(row['url'],timeout=60).read()
            except Exception:
                if not row['path'].startswith('nl/'):
                    raise
                url = f'https://api.github.com/repos/OpenTaal/opentaal-hunspell/contents/{path.name}?ref={row["revision"]}'
                meta = json.load(urllib.request.urlopen(url,timeout=60))
                if not meta.get('content'):
                    meta = json.load(urllib.request.urlopen(meta['git_url'],timeout=60))
                data = base64.b64decode(meta['content'])
        if len(data) != row['bytes'] or hashlib.sha256(data).hexdigest() != row['sha256']:
            raise ValueError('Pinned asset bytes mismatch: '+row['path'])
        with path.open('xb') as stream:
            stream.write(data)
        print(row['path'],row['bytes'],row['sha256'])

if __name__ == '__main__':
    main()
