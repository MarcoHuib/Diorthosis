#!/usr/bin/env python3
"""Analysis-only replay of saved v4 predictions; never loads weights or runs inference.

Run from any directory with Python 3 and the repository's .NET SDK available.
No third-party Python packages; the linked-source C# driver has no packages and clears feeds.
"""
import copy
import hashlib
import itertools
import json
import os
from pathlib import Path
import random
import subprocess
import tempfile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
DATA = ROOT / 'benchmarks/model-validation/smoke-44-reviewed-v2.jsonl'
V4 = ROOT / 'benchmarks/model-validation/results/v4'
MODELS = ['ministral-3b', 'ministral-3-8b', 'ministral-3-14b']


def read_json(path):
    return json.loads(path.read_text())


def read_lines(path):
    return [json.loads(line) for line in path.read_text().splitlines() if line.strip()]


def write_json(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + '\n')


def write_lines(path, rows):
    path.write_text(''.join(json.dumps(row, ensure_ascii=False, separators=(',', ':')) + '\n' for row in rows))


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def apply(text, edits):
    data = text.encode('utf-16-le')
    for e in sorted(edits, key=lambda e: e['start'], reverse=True):
        start, end = 2 * e['start'], 2 * (e['start'] + e['length'])
        assert data[start:end].decode('utf-16-le') == e['originalText']
        data = data[:start] + e['replacementText'].encode('utf-16-le') + data[end:]
    return data.decode('utf-16-le')


def expected(case):
    return {apply(case['inputText'], [dict(e, replacementText=a['replacementText'])
             for e, a in zip(case['errors'], alternatives)])
            for alternatives in itertools.product(*(e['acceptedCorrections'] for e in case['errors']))}


def run(args):
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE='1')
    env.pop('DIORTHOSIS_TEST_MODEL', None)
    subprocess.run([str(a) for a in args], cwd=ROOT, env=env, check=True)


def all_rows(rows):
    return [r for r in rows if r['category'] == 'all']


def pooled(rows):
    keys = ['annotatedErrors', 'correctlyResolvedErrors', 'suggestedEdits', 'correctSuggestions',
            'falsePositiveSuggestions', 'correctInputs', 'correctInputsChanged']
    out = {k: sum(r[k] for r in rows) for k in keys}
    for key, num, den in [('recall', 'correctlyResolvedErrors', 'annotatedErrors'),
                          ('precision', 'correctSuggestions', 'suggestedEdits'),
                          ('falsePositiveRate', 'correctInputsChanged', 'correctInputs')]:
        out[key] = out[num] / out[den] if out[den] else None
    out['correctTextPreservation'] = 1 - out['falsePositiveRate'] if out['correctInputs'] else None
    return out


def main():
    manifest = read_json(HERE / 'source-manifest.json')
    for name, digest in manifest['sha256'].items():
        assert sha(ROOT / name) == digest, f'Frozen source changed: {name}'
    cases = read_lines(DATA)
    assert len(cases) == 44 and len({c['id'] for c in cases}) == 44
    notes = read_json(HERE / 'reference-notes.json')['cases']
    assert set(notes) == {c['id'] for c in cases}
    proposed, reviewed = [], []
    for c in cases:
        p = copy.deepcopy(c)
        for text in notes[c['id']].get('additions', []):
            p['errors'][0]['acceptedCorrections'].append({'replacementText': text})
        for e in p['errors']:
            assert apply(p['inputText'], [dict(e, replacementText=e['originalText'])]) == p['inputText']
        proposed.append(p)
        reviewed.append({'caseId': c['id'], 'original': c, 'proposed': p, 'review': notes[c['id']],
                         'independentHumanValidation': False, 'requiresHumanReview': True,
                         'priorityHumanReview': bool(notes[c['id']]['ambiguity']) or bool(notes[c['id']].get('additions'))})
    changed = [c['id'] for c, p in zip(cases, proposed) if c != p]
    assert changed == ['nl-12', 'en-04', 'en-05', 'en-09']
    write_lines(HERE / 'reviewed-cases.jsonl', reviewed)
    write_lines(HERE / 'smoke-44-proposed-v3.jsonl', proposed)
    by_id = {c['id']: c for c in cases}
    proposed_by_id = {c['id']: c for c in proposed}

    run(['dotnet', 'build', HERE / 'Probe/Probe.csproj', '--configfile', HERE / 'Probe/NuGet.Config',
         '--disable-build-servers', '-m:1', '-nr:false', '-p:UseSharedCompilation=false', '--nologo'])
    driver = HERE / 'Probe/bin/Debug/net10.0/Probe.dll'
    run(['dotnet', driver, 'probe', HERE / 'probe-observations.json'])
    scores, deltas, trace_map = [], [], {}
    with tempfile.TemporaryDirectory(prefix='diorthosis-quality-audit-') as tmp:
        tmp = Path(tmp)
        for model in MODELS:
            traces = read_lines(V4 / f'{model}-trace.jsonl')
            predictions = read_lines(V4 / f'{model}-predictions.jsonl')
            pred_map = {p['caseId']: p['edits'] for p in predictions}
            report = read_json(V4 / f'{model}-report.json')
            assert len(traces) == len(predictions) == len(pred_map) == 44
            assert {t['caseId'] for t in traces} == set(by_id)
            for t in traces:
                assert t['input'] == by_id[t['caseId']]['inputText']
                assert t['terminationReason'] == 'EndOfGeneration'
                assert t['sampledTokenCount'] < 128
                assert t['validatedTextEdits'] == pred_map[t['caseId']]
                trace_map[model, t['caseId']] = t
            replay = tmp / f'{model}-replay.json'
            run(['dotnet', driver, 'replay', V4 / f'{model}-trace.jsonl', replay])
            assert read_json(replay)['identicalReplay']
            raw_path = tmp / f'{model}-raw.jsonl'
            write_lines(raw_path, [{'caseId': t['caseId'], 'edits': t['rawCandidateTextEdits']} for t in traces])
            result = {'model': model, 'modelRevision': report['modelRevision'], 'modelSha256': report['modelSha256'],
                      'quantization': report['quantization'], 'replayIdenticalRows': 44,
                      'rejectedOutputs': report['rejectedModelOutputs'], 'generationTerminations': report['generationTerminations']}
            for layer, path in [('accepted', V4 / f'{model}-predictions.jsonl'), ('rawCandidateEdits', raw_path)]:
                pair = {}
                for revision, data in [('original', DATA), ('proposed', HERE / 'smoke-44-proposed-v3.jsonl')]:
                    output = tmp / f'{model}-{layer}-{revision}.json'
                    run(['dotnet', driver, 'score', data, path, output])
                    pair[revision] = read_json(output)
                historic = report['metrics' if layer == 'accepted' else 'rawCandidateMetrics']
                assert pair['original']['metrics'] == historic, f'Historical metric mismatch: {model} {layer}'
                result[layer] = {rev: {'byLanguage': all_rows(data['metrics']), 'pooled': pooled(all_rows(data['metrics']))}
                                 for rev, data in pair.items()}
                for old, new in zip(pair['original']['cases'], pair['proposed']['cases']):
                    assert old['caseId'] == new['caseId'] and old['output'] == new['output']
                    if old['metrics'] != new['metrics']:
                        deltas.append({'model': model, 'layer': layer, 'caseId': old['caseId'], 'output': old['output'],
                                       'original': old['metrics'], 'proposed': new['metrics'],
                                       'reason': notes[old['caseId']]['judgment']})
            scores.append(result)

    adjudication = read_json(HERE / 'adjudication-notes.json')
    exceptions = adjudication['exceptions']
    packet = read_lines(HERE / 'blind-packet.jsonl')
    key = {r['blindId']: r for r in read_json(HERE / 'blind-key.json')}
    assert len(packet) == len(key) == 132 and set(exceptions) <= set(key)
    shuffled = [(p.stem.removesuffix('-trace'), t['caseId'])
                for p in sorted(V4.glob('*-trace.jsonl')) for t in read_lines(p)]
    random.Random(20261020).shuffle(shuffled)
    for index, (model, case_id) in enumerate(shuffled, 1):
        assert key[f'B{index:03d}'] == {'blindId': f'B{index:03d}', 'caseId': case_id, 'model': model}
    labels = ['Correct minimal correction', 'Correct but unnecessarily rewritten',
              'Grammatically acceptable but semantically ambiguous', 'Grammatically correct with altered meaning',
              'Incorrect or incomplete correction', 'Unnecessary change to already-correct text',
              'Protected-content violation', 'Requires independent human review']

    def ordinary(text, c):
        if text in expected(proposed_by_id[c['id']]):
            clean = not c['errors']
            return labels[0], 'correct preservation' if clean else 'minimal repair', (
                'No edit needed; original clean text preserved.' if clean else notes[c['id']]['judgment'])
        if text == c['inputText'] and c['errors']:
            return labels[4], 'missed repair', notes[c['id']]['judgment'] + ' Output leaves the error unchanged.'
        raise AssertionError(f'Unadjudicated candidate: {c["id"]} {text!r}')

    outputs = []
    for r in packet:
        mapping = key[r['blindId']]
        assert mapping['caseId'] == r['caseId']
        t = trace_map[mapping['model'], r['caseId']]
        c = by_id[r['caseId']]
        accepted = apply(c['inputText'], t['validatedTextEdits'])
        assert (r['raw'], r['extracted'], r['accepted'], r['rejected']) == (
            t['rawModelOutput'], t['extractedCorrection'], accepted, t['rejectionStage'] is not None)
        exception = exceptions.get(r['blindId'])
        if exception:
            raw_label = exception['label']
            extract_label = exception.get('extractedLabel', raw_label)
            reason = exception['reason']
            subtype = 'see reason'
        else:
            extract_label, subtype, reason = ordinary(r['extracted'], c)
            raw_label = extract_label
        if accepted != r['extracted']:
            accept_label, accept_subtype, accept_reason = ordinary(accepted, c)
        else:
            accept_label = exception.get('acceptedLabel', extract_label) if exception else extract_label
            accept_subtype, accept_reason = subtype, reason
        assert raw_label in labels and extract_label in labels and accept_label in labels
        outputs.append({**r, 'model': mapping['model'], 'rawClassification': raw_label,
                        'extractedClassification': extract_label, 'acceptedClassification': accept_label,
                        'acceptedSubtype': accept_subtype, 'reason': reason, 'acceptedReason': accept_reason,
                        'independentHumanReviewRequired': bool(exception and exception.get('humanReview')),
                        'rawProtocolDeviation': r['raw'] != r['extracted'],
                        'rejectionStage': t['rejectionStage'], 'rejectionReason': t['rejectionReason'],
                        'rawCandidateTextEdits': t['rawCandidateTextEdits'], 'validatedTextEdits': t['validatedTextEdits']})
    outputs.sort(key=lambda r: (r['caseId'][:2] != 'nl', r['caseId'], MODELS.index(r['model'])))
    write_lines(HERE / 'output-adjudications.jsonl', outputs)
    counts = {}
    for model in MODELS:
        counts[model] = {}
        for layer in ['rawClassification', 'extractedClassification', 'acceptedClassification']:
            counts[model][layer] = {label: sum(r['model'] == model and r[layer] == label for r in outputs) for label in labels}
        counts[model]['strictSentenceSuccess'] = {}
        for language in ['nl', 'en']:
            selected = [r for r in outputs if r['model'] == model and r['caseId'].startswith(language)]
            counts[model]['strictSentenceSuccess'][language] = {
                'successfulErroneousCases': sum(r['acceptedClassification'] == labels[0] for r in selected if by_id[r['caseId']]['errors']),
                'erroneousCases': 15,
                'preservedCleanCases': sum(r['acceptedClassification'] == labels[0] for r in selected if not by_id[r['caseId']]['errors']),
                'cleanCases': 7,
                'note': 'AI judgments under strict proposal, not independent semantic validation'}
    write_json(HERE / 'sensitivity.json', {
        'status': 'PROVISIONAL annotation-only sensitivity analysis; unchanged evaluator; saved v4 predictions; no inference',
        'baseCommit': manifest['baseCommit'], 'sourceDatasetSha256': sha(DATA),
        'proposedDatasetSha256': sha(HERE / 'smoke-44-proposed-v3.jsonl'),
        'evaluatorSha256': sha(ROOT / 'src/Diorthosis.Benchmarks/BenchmarkEvaluator.cs'),
        'changedAnnotationCases': changed, 'scores': scores, 'caseDeltas': deltas, 'adjudicationCounts': counts,
        'excludedAmbiguousAlternative': 'en-01 had sent is not granted automatic equivalence',
        'noErrorHunkRatesAreNotIndependentSamples': True})

    def code(s):
        return '`' + s.replace('\n', '\\n').replace('`', '\\`') + '`'
    lines = ['# All 44 references and all 132 v4 outputs: quality audit v1', '',
             '**Status: proposed AI judgments; independent bilingual human review pending.**', '',
             'Reference judgments were recorded before opening v4 traces. User examples and historical docs had already disclosed some outputs. '
             'Outputs were shuffled with seed 20261020 and model identities hidden during assessment; identities are restored below. '
             'The packet is reproducible but this was not a controlled independent blind trial.', '',
             'All 44 cases require human sign-off. Priority flags identify additional context, register or annotation questions. '
             '“Correct minimal correction” on clean inputs means correct preservation (zero edits), not an error repair. '
             'Raw labels assess the linguistic response; Markdown/quotes are also recorded as protocol deviations. '
             'For contradictory multi-answer output, raw, extracted and accepted judgments differ.', '']
    for review in reviewed:
        c, p, n = review['original'], review['proposed'], review['review']
        lines += [f'## {c["id"]}', '', '**Input:** ' + code(c['inputText']), '',
                  '**Reference-first assessment:** ' + n['judgment'], '',
                  '**Existing annotations (UTF-16 start/length):**']
        if not c['errors']:
            lines += ['', '- No errors; preserve the input.']
        else:
            lines += ['']
            for e in c['errors']:
                lines += [f'- {e["category"]}, {e["start"]}/{e["length"]}: {code(e["originalText"])} → ' + ', '.join(code(a['replacementText']) for a in e['acceptedCorrections'])]
        lines += ['', '**Proposal:** ' + ('Add ' + ', '.join(code(x) for x in n['additions']) + '; retain existing alternatives.' if n.get('additions') else 'Retain existing annotation.'), '',
                  '**Other alternatives and limits:** ' + n['alternatives'], '', '**Regional conventions:** ' + n['regional'], '',
                  '**Ambiguity / priority human review:** ' + (' '.join(n['ambiguity']) if n['ambiguity'] else 'No special context ambiguity identified; independent review still pending.'), '']
        for r in outputs:
            if r['caseId'] != c['id']:
                continue
            lines += [f'### {r["model"]} · {r["blindId"]}', '', '**Raw:** ' + code(r['raw']), '',
                      '**Extracted:** ' + code(r['extracted']), '', '**Accepted:** ' + code(r['accepted']), '',
                      f'**Judgments:** raw — {r["rawClassification"]}; extracted — {r["extractedClassification"]}; accepted — {r["acceptedClassification"]} ({r["acceptedSubtype"]}).', '',
                      '**Reason:** ' + r['reason'] + (' Accepted layer: ' + r['acceptedReason'] if r['acceptedReason'] != r['reason'] else ''), '',
                      '**Pipeline:** ' + (f'Rejected at {r["rejectionStage"]}: {r["rejectionReason"]}' if r['rejected'] else 'No rejection; accepted edits: ' + str(len(r['validatedTextEdits']))) + '.', '']
    (HERE / 'case-review.md').write_text('\n'.join(lines) + '\n')
    inputs = [ROOT / 'src/Diorthosis.Benchmarks/BenchmarkEvaluator.cs', ROOT / 'src/Diorthosis.Benchmarks/BenchmarkModels.cs']
    inputs += sorted((ROOT / 'src/Diorthosis.Core/Corrections').glob('*.cs'))
    inputs += [ROOT / 'src/Diorthosis.Core/Inference/IModelInferenceAdapter.cs']
    inputs += [ROOT / 'docs/benchmark-quality-audit-v1.md']
    artifacts = [p for p in HERE.iterdir() if p.is_file() and p.name != 'artifact-hashes.json']
    artifacts += list((HERE / 'Probe').glob('*.cs')) + list((HERE / 'Probe').glob('*.csproj')) + [HERE / 'Probe/NuGet.Config']
    write_json(HERE / 'artifact-hashes.json', {str(p.relative_to(ROOT)): sha(p) for p in sorted(set(inputs + artifacts))})
    for name, digest in manifest['sha256'].items():
        assert sha(ROOT / name) == digest, f'Frozen source changed: {name}'
    print('Verified 44 references, 132 trace/prediction pairs, 132 exact parser replays, historical metrics and provisional scores.')
    for model in scores:
        for old, new in zip(model['accepted']['original']['byLanguage'], model['accepted']['proposed']['byLanguage']):
            print(model['model'], new['language'], f'recall {old["correctlyResolvedErrors"]}/16 -> {new["correctlyResolvedErrors"]}/16;',
                  f'precision {old["correctSuggestions"]}/{old["suggestedEdits"]} -> {new["correctSuggestions"]}/{new["suggestedEdits"]}')


if __name__ == '__main__':
    main()
