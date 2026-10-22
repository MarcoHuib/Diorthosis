#!/usr/bin/env python3
"""One-time original AI fixture assembly; refuses to replace a frozen panel."""
import collections
import difflib
import hashlib
import json
from pathlib import Path
import sys
ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT/'benchmarks/phase4/v1'
def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def dump(p,v): p.write_text(json.dumps(v,ensure_ascii=False,indent=2)+'\n')
def main():
    if (BASE/'freeze.json').exists(): raise SystemExit('Already frozen. Create a new version rather than rewriting.')
    rows=[]; counts=collections.Counter()
    for line in (BASE/'authoring.tsv').read_text().splitlines():
        if not line or line.startswith('#'): continue
        language,split,tag,rep,text=line.split('|',4)
        text=text.replace('\\n','\n').replace('\\t','\t')
        # Backslashes in paths are literal; no general unescape or Unicode normalization.
        counts[language,split]+=1
        errors=[]
        for pair in rep.split(';') if rep else []:
            bad,good=pair.split('=>')
            assert text.count(bad)==1,(text,bad)
            start=text.index(bad)
            errors.append(dict(start=len(text[:start].encode('utf-16-le'))//2,length=len(bad.encode('utf-16-le'))//2,
                originalText=bad,category=tag.split(':')[0],acceptedCorrections=[dict(replacementText=good)]))
        rows.append(dict(id=f'p4-{language}-{split}-{counts[language,split]:03}',language={'nl':'dutch','en':'english'}[language],
            inputText=text,errors=sorted(errors,key=lambda e:e['start']),split=split,diagnosticTags=tag.split(':')[1:],
            sourceFamily=f'original-fictional-{language}-{split}-{counts[language,split]:03}',documentId=None,
            provenance=dict(kind='original AI-authored fictional passage',author='OpenAI Codex; model build unavailable',
                rights='Original project fixture; no copied external or private text; project MIT license applies to any copyrightable contribution.',
                labelStatus='AI-generated; preparer inspected; no independent AI or human validation',priorModelExposure='none known; creator knows Phase 3 failure categories',
                confirmationPolicy='Inference-unseen until one locked comparison; author/preparer exposed, not independently sealed held-out data')))
    for lang in ('nl','en'):
        assert [counts[lang,s] for s in ('development','selection','confirmation')]==[10,70,20],counts
        for split,clean in [('development',4),('selection',28),('confirmation',8)]:
            assert sum(not r['errors'] for r in rows if r['id'].startswith('p4-'+lang+'-'+split))==clean,(lang,split)
    assert len({r['inputText'] for r in rows})==200
    files=[]
    for split in ('development','selection','confirmation'):
        p=BASE/(split+'.jsonl');p.write_text(''.join(json.dumps(r,ensure_ascii=False,separators=(',',':'))+'\n' for r in rows if r['split']==split));files.append(p)
    # Search previously committed fixture data; report closest overlap, never claim unseen based on hashes alone.
    historical=[]
    for p in (ROOT/'benchmarks').rglob('*.jsonl'):
        if BASE in p.parents: continue
        for line in p.read_text(errors='replace').splitlines():
            try: r=json.loads(line)
            except ValueError: continue
            if isinstance(r,dict) and isinstance(r.get('inputText'),str): historical.append(r['inputText'])
    historical=set(historical)
    exact=[r['id'] for r in rows if r['inputText'] in historical]
    assert not exact,exact
    near=[]
    for i,a in enumerate(rows):
        for b in rows[i+1:]:
            ratio=difflib.SequenceMatcher(None,a['inputText'].casefold(),b['inputText'].casefold()).ratio()
            if ratio>.72: near.append(dict(a=a['id'],b=b['id'],ratio=ratio))
    dump(BASE/'overlap-audit.json',dict(exactPriorFixtureMatches=exact,distinctHistoricalTextsSearched=len(historical),
        internalPairsAbove072=near,limitation='Character similarity screening only; not proof of independent linguistic families. Prior failure-category exposure declared.'))
    dump(BASE/'freeze.json',dict(version='phase4-panel-1.0.0',createdDate='2026-10-21',passages=200,languages=['dutch','english'],
        splits={'development':20,'selection':140,'confirmation':40},cleanPerLanguage=40,humanValidatedLabels=0,independentAIReviewedLabels=0,
        files={p.name:sha(p) for p in [BASE/'authoring.tsv',*files,BASE/'overlap-audit.json']},
        policy='Freeze texts and acceptable alternatives before inference. Valid unlisted alternatives require separate review, never retroactive reference edits. No population quality claims.'))
    print(json.dumps(dict(counts={str(k):v for k,v in counts.items()},near=near),indent=2))
if __name__=='__main__': main()
