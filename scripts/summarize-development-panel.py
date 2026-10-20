#!/usr/bin/env python3
"""Summarize saved rights-cleared traces; no inference or annotation changes."""
import hashlib
import json
from pathlib import Path
import sys
base=Path(sys.argv[1])
output=Path(sys.argv[2])
if output.exists():raise SystemExit('Choose a new summary path.')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
rows=[]
for folder in sorted(p for p in base.iterdir() if p.is_dir() and (p/'resources.json').exists()):
    manifest=json.loads((folder/'manifest.json').read_text())
    resources=json.loads((folder/'resources.json').read_text())
    metrics=json.loads((folder/'metrics.json').read_text())
    traces=[json.loads(l) for l in (folder/'trace.jsonl').read_text().splitlines() if l.strip()]
    format_valid=aligned=0
    for trace in traces:
        try:value=json.loads(trace['rawModelOutput'])
        except json.JSONDecodeError:continue
        if not isinstance(value,list):continue
        required={'start','length','originalText','replacementText'}
        if not all(isinstance(e,dict) and set(e)==required and type(e['start']) is int and type(e['length']) is int and isinstance(e['originalText'],str) and isinstance(e['replacementText'],str) for e in value):continue
        format_valid+=1
        source=trace['input'].encode('utf-16-le')
        previous_end=previous_start=-1
        valid=True
        for e in sorted(value,key=lambda e:(e['start'],e['length'])):
            start,length=e['start'],e['length']
            if start<0 or length<0 or start+length>len(source)//2 or len(e['originalText'].encode('utf-16-le'))!=2*length or source[start*2:(start+length)*2]!=e['originalText'].encode('utf-16-le') or start<previous_end or start==previous_start:
                valid=False;break
            previous_start,previous_end=start,start+length
        aligned+=valid
    rows.append(dict(configuration=folder.name,manifestSha256=sha(folder/'manifest.json'),traceSha256=sha(folder/'trace.jsonl'),
        cases=len(traces),naturalEog=sum(t['terminationReason']=='endOfGeneration' for t in traces),
        syntacticallyValidJson=sum(t['jsonValid'] for t in traces),strictOffsetShapeValid=format_valid,literalUtf16AlignedIncludingEmpty=aligned,
        dispositions={s:sum(t['disposition']==s for t in traces) for s in sorted({t['disposition'] for t in traces})},resources=resources,
        languages=[m for m in metrics['metrics'] if m['category']=='all']))
output.write_text(json.dumps(dict(evidence='development diagnostics, not independent linguistic validation',
    alignmentDiagnostic='Literal UTF-16/shape replay; full grapheme and safety checks are enforced by .NET. Missing corrections in an empty array are not repairs.',configurations=rows),indent=2)+'\n')
for row in rows:
    print(row['configuration'],[(m['language'],f"{m['repairRecall']['numerator']}/{m['repairRecall']['denominator']}",f"clean {m['cleanLiteralPreservation']['numerator']}/{m['cleanLiteralPreservation']['denominator']}") for m in row['languages']],row['dispositions'])
