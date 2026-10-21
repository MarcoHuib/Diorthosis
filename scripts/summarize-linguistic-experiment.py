#!/usr/bin/env python3
"""Aggregate retained counts and AI decisions; no new linguistic evaluator or inference."""
import collections
import importlib
import json
from pathlib import Path
import statistics
import sys

c = importlib.import_module('phase2-analysis')

def summarize(base, output):
    if output.exists():
        raise ValueError('Summary output must be new.')
    cases = {r['id']:r for r in c.lines(base/'panel.jsonl')}
    rows = c.lines(base/'run/trace.jsonl')
    errors = c.lines(base/'ai-error-decisions.jsonl')
    subjects = {s['subjectId']:s for s in c.lines(base/'review-all-candidates/subjects.jsonl')}
    votes = c.lines(base/'preparer-ai-decisions.jsonl')
    lineage = json.loads((base/'review-all-candidates/lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text())
    rowmap = {(r['id'],r['arm']):r for r in rows}
    seen = set()
    for e in errors:
        key = (e['caseId'],e['arm'],e['annotationIndex'])
        case = cases[e['caseId']]
        row = rowmap[(e['caseId'],e['arm'])]
        if (key in seen or e['reviewerKind'] != 'ai' or e['sourceSha256'] != c.digest(case['inputText']) or e['annotation'] != case['errors'][e['annotationIndex']]
            or e['deliveredText'] != c.apply(case['inputText'],row['analysis']['edits']) or e['deliveredSha256'] != c.digest(e['deliveredText'])):
            raise ValueError('Stale/duplicate AI per-error decision.')
        seen.add(key)
    if len(errors) != sum(len(case['errors']) for case in cases.values())*6:
        raise ValueError('Missing AI error-decision coverage.')
    for vote in votes:
        subject = subjects[vote['subjectId']]
        if vote['bindingSha256'] != c.subject(dict(id=subject['caseId'],language=subject['language'],inputText=subject['sourceText'],errors=json.loads(subject['annotationsJson']),sourceFamily=subject['sourceFamily'],documentId=subject['documentId']),
            subject['layer'],subject['kind'],subject['candidateText'],json.loads(subject['annotationsJson']),json.loads(subject['metadataJson']))['bindingSha256'] or vote['reviewerKind'] != 'ai':
            raise ValueError('Stale/non-AI preparer decision.')
    results=[]
    for arm in ('baseline','hints','sample-101','voting-3'):
        reference = json.loads((base/'run'/f'{arm}-metrics.json').read_text())
        for language in ('Dutch','English'):
            language_cases = [case for case in cases.values() if case['language']==language]
            subset = [r for r in rows if r['arm']==arm and r['language'].lower()==language.lower()]
            local_errors = [e for e in errors if e['arm']==arm and e['language']==language]
            clean = [r for r in subset if not cases[r['id']]['errors']]
            accepted_clean = [r for r in clean if r['analysis']['disposition'] in ('accepted','unchanged')]
            judgments=[]
            for vote in votes:
                subject=subjects[vote['subjectId']]
                if subject['language'].lower()!=language.lower() or subject['layer']!='delivered':continue
                if any(link['arm']==arm for link in lineage[vote['subjectId']]):judgments.append(vote['judgment'])
            valid = [j for j in judgments if all(j[k] is not None for k in ('justified','meaningPreserved','allDefectsRepaired','noNewErrors'))]
            success = sum(all(j[k] for k in ('justified','meaningPreserved','allDefectsRepaired','noNewErrors')) for j in valid)
            results.append(dict(arm=arm,language=language,sourceCases=len(language_cases),
                reference=next(m for m in reference['metrics'] if m['language']==language and m['category']=='all'),
                aiLocalRepairs=dict(numerator=sum(e['repaired'] for e in local_errors),denominator=len(local_errors)),
                aiAcceptableAssessedDeliveredPassages=dict(numerator=success,denominator=len(valid)),
                aiPassageJudgments=len(judgments),humanValidated=None,
                cleanAssessableUnchanged=dict(numerator=sum(c.apply(r['input'],r['analysis']['edits'])==r['input'] for r in accepted_clean),denominator=len(clean),assessable=len(accepted_clean)),
                dispositions=dict(collections.Counter(r['analysis']['disposition'] for r in subset)),
                meanLatencyMilliseconds=statistics.mean(r['latencyMilliseconds'] for r in subset),medianLatencyMilliseconds=statistics.median(r['latencyMilliseconds'] for r in subset),
                totalSampledTokens=sum(r.get('inference',{}).get('sampledTokens',r.get('sampledTokens',0)) for r in subset),
                generationProcessCpuMilliseconds=None if arm=='voting-3' else sum(r['cpuMilliseconds'] for r in subset)))
    unknowns=[]
    for r in rows:
        if r['arm']=='baseline':
            for word in r['dictionaryDiagnostics']:
                if not word['recognized']:unknowns.append(dict(caseId=r['id'],language=r['language'],word=word['word'],suggestions=word['suggestions']))
    diversity=[dict(caseId=caseid,raw=len({r['inference']['text'] for r in rows if r['id']==caseid and r['arm'].startswith('sample-')}),
        extracted=len({r['analysis']['extractedCorrection'] for r in rows if r['id']==caseid and r['arm'].startswith('sample-')})) for caseid in cases]
    c.write(output,dict(policy='Reference diagnostics and preparer AI assessments remain separate. No human validation, independence, held-out inference or production approval.',
        inputs={str(p.relative_to(base)):c.sha(p) for p in [base/'run/trace.jsonl',base/'preparer-ai-decisions.jsonl',base/'ai-error-decisions.jsonl']},
        results=results,dictionaryUnknowns=unknowns,diversity=diversity,
        postDiagnosticCounts={arm:dict(collections.Counter(d['status'] for r in rows if r['arm']==arm for d in r['postDiagnostics'])) for arm in ('baseline','hints','sample-101','sample-202','sample-303','voting-3')},
        resources=json.loads((base/'run/resources.json').read_text()),dictionaryResources=json.loads((base/'run/dictionary-resources.json').read_text())))

if __name__=='__main__':
    summarize(Path(sys.argv[1]),Path(sys.argv[2]))
