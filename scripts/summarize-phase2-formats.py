#!/usr/bin/env python3
"""Summarize saved bounded format arms; no inference or source mutations."""
import collections
import hashlib
import json
from pathlib import Path
import sys
root=Path(sys.argv[1]);json_out=Path(sys.argv[2]);md_out=Path(sys.argv[3])
if json_out.exists() or md_out.exists() or json_out.resolve()==md_out.resolve():raise SystemExit('Choose distinct new summary files.')
sha=lambda path:hashlib.sha256(path.read_bytes()).hexdigest()
runs=[];contrasts=[]
for model in ['ministral-3b','ministral-14b','eurollm-9b']:
    model_runs=[]
    for arm in ['baseline','offsets']:
        folder=root/(model+'-'+arm)
        manifest=json.loads((folder/'manifest.json').read_text());assert manifest['completionState']=='complete'
        trace=[json.loads(s) for s in (folder/'trace.jsonl').read_text().splitlines()]
        metrics=json.loads((folder/'metrics.json').read_text());resources=json.loads((folder/'resources.json').read_text())
        assert len(trace)==8
        counts=collections.Counter(row['disposition'] for row in trace)
        record=dict(model=model,arm=arm,sourceCommit=manifest['repositoryCommit'],modelProvenance=manifest['model'],datasetSha256=manifest['datasetSha256'],
                    evaluatorVersion=metrics['evaluatorVersion'],promptVersion=manifest['promptVersion'],inference=manifest['inference'],hardware=dict(cpu=manifest['cpuModel'],ramBytes=manifest['physicalRamBytes'],os=manifest['operatingSystem'],architecture=manifest['architecture']),
                    naturalEog=dict(numerator=sum(r['terminationReason']=='endOfGeneration' for r in trace),denominator=8),jsonValid=dict(numerator=sum(r['jsonValid'] for r in trace),denominator=8),
                    assessableDelivery=dict(numerator=sum(r['disposition'] in ('accepted','unchanged') for r in trace),denominator=8),dispositions=counts,
                    rejectionReasons=dict(collections.Counter((r['rejectionStage'] or '')+': '+(r['rejectionReason'] or '') for r in trace if r['rejectionStage'])),
                    referenceRepairRecall={r['language']:r['repairRecall'] for r in metrics['metrics'] if r['category']=='all'},resources=resources,
                    files={p.name:sha(p) for p in folder.iterdir() if p.is_file()},linguisticPrecision=None,linguisticCompletePassageSuccess=None,
                    limitations=['Eight exposed AI/synthetic cases; unreviewed alternatives.','Natural EOG/JSON validity are format observations, not source alignment or correction quality.','No independent human judgments or model promotion.','OS cache and other developer process load were not isolated; resource/timing figures are diagnostic process measurements.'])
        runs.append(record);model_runs.append((trace,metrics))
    a,b=model_runs;assert [r['caseId'] for r in a[0]]==[r['caseId'] for r in b[0]]
    contrasts.append(dict(model=model,principalVariable='existing full-passage vs existing grammar-constrained offset protocol',
        pairedCases=[dict(caseId=x['caseId'],baselineDisposition=x['disposition'],offsetDisposition=y['disposition'],baselineTermination=x['terminationReason'],offsetTermination=y['terminationReason']) for x,y in zip(a[0],b[0])],decision='No promotion; offsets deliver zero reference repairs on this exposed panel.'))
summary=dict(version='phase2-format-summary-1.0.0',runs=runs,pairedContrasts=contrasts,productionPromptChanged=False,newModelDownloads=False,
             independentHumanJudgments=0,conclusion='Larger local models did not rescue direct model-generated offsets here. Keep production v3 available; do not promote an experimental protocol or select a model from this panel.')
with json_out.open('x') as f:json.dump(summary,f,indent=2);f.write('\n')
rows=['# Bounded format experiment results','',summary['conclusion'],'','| Model | Protocol | Natural EOG | Valid JSON | Assessable delivery | NL reference repairs | EN reference repairs | Mean ms | Sampled peak GiB |','|---|---|---:|---:|---:|---:|---:|---:|---:|']
for r in runs:
    nl=r['referenceRepairRecall']['Dutch'];en=r['referenceRepairRecall']['English']
    rows.append(f"| {r['model']} | {r['arm']} | {r['naturalEog']['numerator']}/8 | {r['jsonValid']['numerator']}/8 | {r['assessableDelivery']['numerator']}/8 | {nl['numerator']}/{nl['denominator']} | {en['numerator']}/{en['denominator']} | {r['resources']['meanLatencyMilliseconds']:.0f} | {r['resources']['sampledPeakProcessWorkingSetBytes']/2**30:.2f} |")
rows+=['','All48 generations were actually run on already local pinned assets, CPU-only under OS network denial. Complete model/native/prompt/dataset/runtime/settings and raw traces are adjacent. JSON is not the baseline output protocol, so its JSON count is not a baseline failure metric.','',
       '3B offsets:8/8 rejected despite natural EOG and valid JSON.14B offsets:6 unchanged empty arrays and2 rejected candidates; no reference repairs. EuroLLM9B offsets:8 rejected, including2 maximum-token incomplete outputs;6 valid JSON responses also fail delivery. Bigger models did not establish usable offsets.','',
       'Important reference ambiguity: baseline en-04 uses a grammatical initial Tomorrow alternative not accepted by the old whole-sentence reference;4/5 total reference repairs does not prove a remaining linguistic defect. Conversely3B changes correct Dutch start to startt in nl-17 and the validator accepts that typo;14B tries OAuth2→OAuth 2 in nl-21 and is correctly rejected by identifier protection. These are explicit AI/mechanical observations, not human precision measurements.','',
       'Memory sampling covers load and inference at100ms and can miss short peaks. OS cache and competing development processes were not isolated; figures are diagnostic, not speed superiority or minimum-device requirements. No quality target, production change or model selection is justified.']
with md_out.open('x') as f:f.write('\n'.join(rows)+'\n')
print('\n'.join(rows[:12]))
