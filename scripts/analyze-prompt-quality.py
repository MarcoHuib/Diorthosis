#!/usr/bin/env python3
"""Paired descriptive diagnostics; linguistic judgments stay separate from reference matching."""
import collections
import hashlib
import importlib
import itertools
import json
from pathlib import Path
import random
import statistics
import subprocess
import sys
common=importlib.import_module('phase2-analysis')
ROOT=Path(__file__).resolve().parents[1]
CLI=ROOT/'src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll'
BASELINE='diorthosis-correction-v3'
def eligible(row):return row['analysis']['disposition'] in ('accepted','unchanged')
def wilson(n,d):
    if not d:return dict(numerator=n,denominator=d,estimate=None,lower=None,upper=None)
    z=1.959963984540054;p=n/d;f=1+z*z/d;c=(p+z*z/(2*d))/f;h=z*((p*(1-p)/d+z*z/(4*d*d))**.5)/f
    return dict(numerator=n,denominator=d,estimate=p,lower=max(0,c-h),upper=min(1,c+h))
def paired_interval(values):
    if not values:return None
    rng=random.Random(41021);n=len(values)
    samples=sorted(sum(rng.choices(values,k=n))/n for _ in range(10000))
    return dict(meanDifference=sum(values)/n,lower=samples[249],upper=samples[9749],method='10,000 source-case paired bootstrap resamples, fixed seed 41021; descriptive development diagnostic, no population or post-selection inference')
def load(panel,runs):
    cases=common.lines(panel);byid={c['id']:c for c in cases}
    if len(byid)!=len(cases):raise ValueError('Duplicate cases')
    allrows=[];identities=[];seen=set()
    for run in runs:
        m=json.loads((run/'manifest.json').read_text())
        if m['state']!='complete' or m['datasetSha256']!=common.sha(panel):raise ValueError('Incomplete or unbound run')
        if m['provenance']['modelSha256']!='9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8':raise ValueError('Wrong model')
        identities.append({k:m[k] for k in ('provenance','settings','runtime','nativeHashes','assemblyHashes')})
        arms={a['promptVersion'] for a in m['arms']}
        rows=common.lines(run/'trace.jsonl')
        expected=set(itertools.product(byid,arms))
        observed={(r['id'],r['promptVersion']) for r in rows}
        if len(rows)!=len(observed) or observed!=expected or len(rows)!=m['attempts']:raise ValueError('Incomplete paired coverage')
        for r in rows:
            key=r['id'],r['promptVersion']
            if key in seen:raise ValueError('Repeated case/condition requires explicit replicate treatment')
            seen.add(key)
            if r['input']!=byid[r['id']]['inputText'] or common.digest(r['prompt'])!=r['promptSha256']:raise ValueError('Source/prompt mismatch')
            if common.apply(r['input'],r['analysis']['edits'])!=r['delivered']:raise ValueError('Delivery mismatch')
            r=dict(r,run=str(run.relative_to(ROOT)) if ROOT in run.parents else str(run),traceSha256=common.sha(run/'trace.jsonl'))
            allrows.append(r)
    if any(i!=identities[0] for i in identities[1:]):raise ValueError('Different model/runtime/software/settings between compared runs')
    return cases,allrows

def analyze(panel,runs,out):
    cases,rows=load(panel,runs);byid={c['id']:c for c in cases}
    out.mkdir(parents=True,exist_ok=False)
    versions=sorted({r['promptVersion'] for r in rows});scores={};reports={}
    for version in versions:
        selected=[r for r in rows if r['promptVersion']==version]
        if {r['id'] for r in selected}!=set(byid):raise ValueError('Every condition must attempt every source')
        # Reuse the existing evaluator for each literal layer; no prompt-specific scoring.
        for layer in ('delivered','extracted','raw'):
            preds=[]
            for r in selected:
                text=r['delivered'] if layer=='delivered' else r['analysis']['extractedCorrection'] if layer=='extracted' else r['inference']['text'] if r['inference'] else None
                if text is not None:preds.append(common.candidate_prediction(byid[r['id']],text))
            predpath=out/f'{version}-{layer}-predictions.jsonl';common.write_lines(predpath,preds)
            result=subprocess.run(['dotnet',str(CLI),'evaluate-v3',str(panel),str(predpath)],cwd=ROOT,capture_output=True,text=True,check=True)
            report=json.loads(result.stdout);common.write(out/f'{version}-{layer}-reference.json',report)
            reports[version,layer]=report
            scores[version,layer]={r['caseId']:r for r in report['cases']}
    summary=[];ledger=[]
    for version in versions:
        for language in ('dutch','english'):
            selected=[r for r in rows if r['promptVersion']==version and r['language']==language]
            originals=[byid[r['id']] for r in selected];clean=[r for r in selected if not byid[r['id']]['errors']]
            repair=sum(len(scores[version,'delivered'][r['id']]['repairedAnnotations']) if eligible(r) else 0 for r in selected)
            errors=sum(len(c['errors']) for c in originals)
            summary.append(dict(promptVersion=version,language=language,attempted=len(selected),annotatedErrors=errors,
                referenceRepairs=repair,referenceMisses=errors-repair,
                referenceEligibleComplete=wilson(sum(eligible(r) and scores[version,'delivered'][r['id']]['referenceCompletePassage'] is True for r in selected),len(selected)),
                rawReferenceComplete=wilson(sum(scores[version,'raw'].get(r['id'],{}).get('referenceCompletePassage') is True for r in selected),len(selected)),
                eligibleCleanLiteral=wilson(sum(eligible(r) and r['delivered']==r['input'] for r in clean),len(clean)),
                fallbackInclusiveCleanLiteral=wilson(sum(r['delivered']==r['input'] for r in clean),len(clean)),
                eligibleCleanChanged=sum(eligible(r) and r['delivered']!=r['input'] for r in clean),
                cleanAbstentions=sum(not eligible(r) for r in clean),
                dispositions=dict(collections.Counter(r['analysis']['disposition'] for r in selected)),
                failures=sum(r['failure'] is not None for r in selected),
                terminations=dict(collections.Counter(r['generationTermination'] for r in selected)),
                promptContentTokens=dict(mean=statistics.mean(r['promptContentTokens'] for r in selected),total=sum(r['promptContentTokens'] for r in selected)),
                sampledOutputTokens=sum(r['sampledTokens'] for r in selected),
                latencyMilliseconds=dict(mean=statistics.mean(r['latencyMilliseconds'] for r in selected),median=statistics.median(r['latencyMilliseconds'] for r in selected),maximum=max(r['latencyMilliseconds'] for r in selected)),
                sampledPeakProcessWorkingSetBytes=max(r['sampledPeakProcessWorkingSetBytes'] for r in selected)))
    lookup={(r['id'],r['promptVersion']):r for r in rows};comparisons=[]
    for control in (BASELINE, 'phase4-LiteralJson-Original-1.0.0'):
        if control not in versions:continue
        for version in versions:
            if version==control:continue
            for language in ('dutch','english'):
                differences=[];gain=loss=ties=0;gained_errors=lost_errors=0;changed=[]
                for c in cases:
                    if c['language']!=language:continue
                    a,b=lookup[c['id'],control],lookup[c['id'],version]
                    sa,sb=scores[control,'delivered'][c['id']],scores[version,'delivered'][c['id']]
                    ra=set(sa['repairedAnnotations']) if eligible(a) else set();rb=set(sb['repairedAnnotations']) if eligible(b) else set()
                    gooda=eligible(a) and sa['referenceCompletePassage'] is True;goodb=eligible(b) and sb['referenceCompletePassage'] is True
                    delta=int(goodb)-int(gooda);differences.append(delta);gain+=delta>0;loss+=delta<0;ties+=delta==0
                    gained_errors+=len(rb-ra);lost_errors+=len(ra-rb)
                    row=dict(caseId=c['id'],language=language,baselinePrompt=control,otherPrompt=version,input=c['inputText'],baselineDelivered=a['delivered'],otherDelivered=b['delivered'],
                        baselineDisposition=a['analysis']['disposition'],otherDisposition=b['analysis']['disposition'],
                        baselineReferenceRepaired=sorted(ra),otherReferenceRepaired=sorted(rb),gainedAnnotationIndexes=sorted(rb-ra),lostAnnotationIndexes=sorted(ra-rb),
                        referenceCompleteDifference=delta,literalChanged=a['delivered']!=b['delivered'],judgment='unassessed; reference difference is not a linguistic conclusion')
                    ledger.append(row)
                    if row['literalChanged']:changed.append(c['id'])
                discord=gain+loss
                p=min(1,2*sum(__import__('math').comb(discord,k) for k in range(min(gain,loss)+1))/2**discord) if discord else 1
                comparisons.append(dict(language=language,baseline=control,other=version,attempted=len(differences),referencePassageGains=gain,referencePassageLosses=loss,ties=ties,
                    gainedReferenceErrors=gained_errors,lostReferenceErrors=lost_errors,changedDeliveries=changed,
                    descriptivePairedInterval=paired_interval(differences),exactDiscordanceP=p,
                    caution='Reference labels unvalidated; post-selection/multiple comparisons and narrow synthetic distribution prevent population superiority claim.'))
    common.write(out/'summary.json',dict(version='phase4-comparison-1.1.0',datasetSha256=common.sha(panel),sourceCases=len(cases),generationOccurrences=len(rows),
        policy='Provisional reference diagnostics only; denominator all attempted sources. Abstention never earns a repair. Clean source fallback reported separately. No independently assessed meaning/precision/new-error metrics yet.',
        metrics=summary,paired=comparisons))
    common.write_lines(out/'paired-ledger.jsonl',ledger)
    common.write(out/'manifest.json',dict(datasetSha256=common.sha(panel),runs={str(p):common.sha(p/'trace.jsonl') for p in runs},
        scriptSha256=common.sha(Path(__file__)),files={p.name:common.sha(p) for p in out.iterdir() if p.is_file()}))
    print(json.dumps(summary,indent=2))
if __name__=='__main__':
    if len(sys.argv)<4:raise SystemExit('analyze-prompt-quality.py <panel.jsonl> <NEW-output-directory> <complete-run-directory> [...]')
    analyze(Path(sys.argv[1]).resolve(),[Path(p).resolve() for p in sys.argv[3:]],Path(sys.argv[2]).resolve())
