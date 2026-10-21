#!/usr/bin/env python3
"""Paired case-level diagnostics. Reuse exact contextual judgments, never rebind decisions."""
import collections
import importlib
import json
from pathlib import Path
import sys

a=importlib.import_module('analyze-external-linguistic-review')
c=a.c
ROOT=a.ROOT
BASE=a.BASE

def summarize(evidence,out):
    evidence=evidence.resolve();out=out.resolve();out.mkdir(parents=True,exist_ok=False)
    comparison=c.lines(evidence/'analysis/comparison.jsonl')
    paired=c.lines(evidence/'impact/paired.jsonl')
    frozen=c.lines(BASE/'run/trace.jsonl')
    originals={(r['id'],r['arm']):r for r in frozen}
    decisions=c.lines(BASE/'ai-error-decisions.jsonl')
    panel={r['id']:r for r in c.lines(BASE/'panel.jsonl')}
    if len(paired)!=120 or len({(r['caseId'],r['arm']) for r in paired})!=120:
        raise ValueError('Expected exact paired panel coverage.')
    ledger=[]
    for r in paired:
        old=originals[(r['caseId'],r['arm'])]
        if r['input']!=old['input'] or r['before']!=old['analysis'] or r['inference']!=old.get('inference'):
            raise ValueError('Replay changed its frozen source analysis or raw generation.')
        record=dict(caseId=r['caseId'],language=r['language'],arm=r['arm'],source=r['input'],
            clean=not panel[r['caseId']]['errors'],beforeDelivered=r['beforeDelivered'],afterDelivered=r['afterDelivered'],
            deliveredChanged=r['deliveredChanged'],dispositionChanged=r['dispositionChanged'],
            postDictionaryDiagnostics=old.get('postDiagnostics'),before={},after={})
        for stage in ['before','after']:
            disposition=r[stage]['disposition'];accepted=disposition in ['accepted','unchanged']
            text=r[stage+'Delivered']
            matches=[x for x in comparison if x['subject']['sourceText']==r['input'] and x['subject']['candidateText']==text and x['subject']['layer']=='delivered'] if accepted else []
            if len(matches)>1:raise ValueError('Ambiguous contextual decision reuse.')
            match=matches[0] if matches else None
            rejected=[x for x in comparison if x['subject']['layer']=='rejected' and any(l['originalCaseId']==r['caseId'] and l['arm']==r['arm'] for l in x['lineage'])]
            j=match['external']['judgment'] if match else None
            # Frozen local defect judgments are reusable only for identical delivered text,
            # or the sole verified wrapper removal which leaves the original article repair intact.
            repairs=[d for d in decisions if d['caseId']==r['caseId'] and d['arm']==r['arm']]
            reused_repairs=text==r['beforeDelivered'] or (r['caseId']=='p2-en-article-error' and r['beforeDelivered']=='She is an *experienced* engineer.' and text=='She is an experienced engineer.')
            record[stage]=dict(disposition=disposition,deliveryEligible=accepted,
                judgment=j,originalDecisionSubjectId=match['subject']['subjectId'] if match else None,
                projection='exact source/candidate/layer contextual reuse; original decision binding retained, no new review' if match else 'no delivered candidate judgment',
                rejectionJudgments=[x['external'] for x in rejected] if not accepted else [],
                localRepairedErrors=sum(d['repaired'] is True for d in repairs) if reused_repairs else None,
                localErrorDenominator=len(repairs),localErrorAuthority='frozen preparer-AI local-error diagnostics, not independent/human approval')
        ledger.append(record)
    c.write_lines(out/'case-ledger.jsonl',ledger)
    # The one updated text already has a bound subject in the original blinded
    # packet. Preserve that entire packet byte-for-byte; only stage membership changes.
    review_lineage={x['subject']['subjectId']:[dict(link,stage='both; unchanged raw/annotation') for link in x['lineage']] for x in comparison if x['subject']['layer'] in ['raw','annotations']}
    for r in ledger:
        for stage in ['before','after']:
            ids=[r[stage]['originalDecisionSubjectId']] if r[stage]['deliveryEligible'] else [d['subjectId'] for d in r[stage]['rejectionJudgments']]
            for subject_id in ids:
                review_lineage.setdefault(subject_id,[]).append(dict(originalCaseId=r['caseId'],arm=r['arm'],stage=stage,disposition=r[stage]['disposition']))
    if len(review_lineage)!=119:raise ValueError('Before/after review packet coverage differs.')
    c.write(out/'review-lineage-DO-NOT-SEND-TO-REVIEWERS.json',review_lineage)
    (out/'before-after-blinded-review.zip').write_bytes((BASE/'chatgpt-review.zip').read_bytes())
    metrics=[]
    for language in ['dutch','english']:
        for arm in a.ARMS:
            rows=[r for r in ledger if r['language']==language and r['arm']==arm]
            m=dict(language=language,arm=arm,cases=len(rows),changedDelivered=sum(r['deliveredChanged'] for r in rows),changedDispositions=sum(r['dispositionChanged'] for r in rows))
            for stage in ['before','after']:
                accepted=[r for r in rows if r[stage]['deliveryEligible']]
                js=[r[stage]['judgment'] for r in accepted]
                error_total=sum(r[stage]['localErrorDenominator'] for r in rows)
                repaired=sum(r[stage]['localRepairedErrors'] or 0 for r in rows)
                blocked=[r for r in rows if not r[stage]['deliveryEligible']]
                m[stage]=dict(dispositions=dict(collections.Counter(r[stage]['disposition'] for r in rows)),
                    acceptedPassage=a.measure([a.conjunction(j) if j else None for j in js]),
                    meaningFailures=a.measure([j['meaningPreserved'] if j else None for j in js],False),
                    newErrorPassages=a.measure([j['noNewErrors'] if j else None for j in js],False),
                    unjustifiedModifiedPassages=a.measure([r[stage]['judgment']['justified'] if r[stage]['judgment'] else None for r in accepted if r[stage+'Delivered']!=r['source']],False),
                    unnecessaryCleanModifications=dict(numerator=sum(r[stage+'Delivered']!=r['source'] for r in accepted if r['clean']),denominator=sum(r['clean'] for r in accepted)),
                    criticalFailures=a.measure([j['criticalSemanticFailure'] if j else None for j in js]),
                    repairedLocalErrors=dict(numerator=repaired,denominator=error_total,unknown=sum(r[stage]['localErrorDenominator'] for r in rows if r[stage]['localRepairedErrors'] is None)),
                    missedLocalErrors=error_total-repaired,
                    assessableLiteralClean=dict(numerator=sum(r[stage]['deliveryEligible'] and r[stage+'Delivered']==r['source'] for r in rows if r['clean']),denominator=sum(r['clean'] for r in rows)),
                    withheld=len(blocked),withheldJudgmentCoverage=sum(bool(r[stage]['rejectionJudgments']) for r in blocked),
                    reviewerInappropriateRejections=sum(any(j['judgment']['rejectionAppropriate'] is False for j in r[stage]['rejectionJudgments']) for r in blocked),
                    unresolvedLocalRepairCases=sum(r[stage]['localRepairedErrors'] is None for r in rows),humanValidated=None)
            metrics.append(m)
    historical=[]
    for directory in sorted((evidence/'historical-replay').iterdir()):
        old={r['caseId']:r for r in c.lines(ROOT/'benchmarks/phase2/v1/review-analysis'/directory.name/'trace.jsonl')}
        new=c.lines(directory/'trace.jsonl')
        if set(old)!={r['caseId'] for r in new}:raise ValueError('Historical coverage differs.')
        changes=[r['caseId'] for r in new if any(r[k]!=old[r['caseId']][k] for k in ['extractedCorrection','validatedTextEdits','disposition'])]
        historical.append(dict(model=directory.name,cases=len(new),changedCases=changes))
    c.write(out/'summary.json',dict(policy='Paired deterministic replay. AI diagnostic judgments and exact-context projections only; zero new GPT-6 assessments, zero human approvals. Local-error repair labels are preparer AI; whole-passage judgments are external AI. No field is human ground truth.',
        metrics=metrics,historical=historical,deliveredChanges=[dict(caseId=r['caseId'],arm=r['arm'],before=r['beforeDelivered'],after=r['afterDelivered'],beforeSubject=r['before']['originalDecisionSubjectId'],afterSubject=r['after']['originalDecisionSubjectId']) for r in ledger if r['deliveredChanged']],
        limitations=['New-error counts are passages, not independently enumerated new errors.','No new inference, paired inference latency or memory benchmark.','No false-rejection or lost repair observed under this change; unresolved semantic validity is retained.','Clean withheld text is a safe abstention, excluded from assessable literal preservation; it is not silently a success.']))
    c.write(out/'manifest.json',dict(inputs={str(p.relative_to(ROOT)):c.sha(p) for p in [evidence/'analysis/comparison.jsonl',evidence/'impact/paired.jsonl',BASE/'ai-error-decisions.jsonl',BASE/'panel.jsonl']},outputs={p.name:c.sha(p) for p in out.iterdir() if p.is_file()}))

if __name__=='__main__':summarize(*map(Path,sys.argv[1:]))
