#!/usr/bin/env python3
"""Retain, strictly import and reconcile external AI batches. Never infer missing judgments."""
import argparse
import collections
import importlib
import json
from pathlib import Path
import shutil
import stat
import subprocess
import zipfile

c = importlib.import_module('phase2-analysis')
imp = importlib.import_module('import-prompt-quality-review')
strict = imp.existing.strict
ROOT = Path(__file__).resolve().parents[1]
REVIEW = ROOT / 'benchmarks/phase4/v1/review'
NAMES = {'decisions.jsonl', 'error-decisions.jsonl', 'provenance.json'}


def archive_files(path):
    with zipfile.ZipFile(path) as z:
        entries = z.infolist()
        if len(entries) != 3 or {e.filename for e in entries} != NAMES:
            raise ValueError('Expected exactly three unique regular root files')
        for e in entries:
            mode = stat.S_IFMT(e.external_attr >> 16)
            if e.is_dir() or e.flag_bits & 1 or mode not in (0, stat.S_IFREG) or e.file_size > 2_000_000:
                raise ValueError('Encrypted, nonregular or oversized ZIP member: ' + e.filename)
        if sum(e.file_size for e in entries) > 4_000_000:
            raise ValueError('Oversized ZIP')
        # Reading each member verifies CRC. Never extract arbitrary paths or execute content.
        return {e.filename: z.read(e) for e in entries}


def reconcile(subjects, batches):
    parent = {s['subjectId']: s for s in subjects}
    if len(parent) != len(subjects):
        raise ValueError('Duplicate parent subject')
    seen = set()
    for rows in batches:
        for s in rows:
            sid = s['subjectId']
            if sid in seen:
                raise ValueError('Duplicate subject across batches: ' + sid)
            if sid not in parent or s != parent[sid]:
                raise ValueError('Unexpected or altered parent subject: ' + sid)
            seen.add(sid)
    return sorted(set(parent) - seen)



def validate_error_consistency(decisions, errors):
    main = {d['subjectId']: d['judgment'] for d in decisions}
    for e in errors:
        if main[e['subjectId']]['allDefectsRepaired'] is True and e['repaired'] is False:
            raise ValueError('Contradictory passage/per-error repair judgment: '+e['subjectId']+'/annotation-'+str(e['annotationIndex']))


def prepare_errors(review, original, derived, assessed):
    """Only quarantine source-only annotations; no rebinding or judgment rewriting."""
    subjects = {s['subjectId']: s for s in c.lines(review / 'subjects.jsonl')}
    retained = []; quarantine = []; seen = set()
    for number, raw in enumerate(original.read_bytes().splitlines(keepends=True), 1):
        row = strict(raw.decode('utf-8'))
        key=(row.get('subjectId'),row.get('annotationIndex'))
        if key in seen: raise ValueError('Duplicate supplementary record: '+str(key))
        seen.add(key)
        s = subjects.get(row.get('subjectId'))
        if s and s['kind'] == 'case':
            # Validate everything except the explicitly inapplicable candidate kind.
            if set(row) != {'subjectId','bindingSha256','annotationIndex','repaired','rationale'} or row['bindingSha256'] != s['bindingSha256'] or row['subjectId'] not in assessed:
                raise ValueError('Invalid supplementary source record: ' + str(number))
            annotations = strict(s['annotationsJson'])
            if type(row['annotationIndex']) is not int or not 0 <= row['annotationIndex'] < len(annotations) or row['repaired'] is not False or not isinstance(row['rationale'], str) or not row['rationale'].strip():
                raise ValueError('Invalid supplementary source annotation: ' + str(number))
            quarantine.append(dict(file='error-decisions.jsonl', line=number, subjectId=row['subjectId'],
                bindingSha256=row['bindingSha256'], violatedContract='Supplementary repair judgments require kind passage or rejection; source-only kind case has no candidate.',
                explanation='Reviewer explicitly describes an unrepaired original annotation. Retain as source diagnostic; exclude from candidate repair import.',
                originalLineSha256=c.digest(raw.decode('utf-8')), decision=row))
        else:
            retained.append(raw)
    if quarantine:
        derived.mkdir()
        selected = derived / 'candidate-error-decisions.jsonl'
        selected.write_bytes(b''.join(retained))
    else:
        selected = original
    valid = imp.validate_errors(review, selected, assessed)
    keys = {(r['subjectId'], r['annotationIndex']) for r in valid}
    expected = {(s['subjectId'], i) for s in subjects.values() if s['kind'] in ('passage', 'rejection') and s['subjectId'] in assessed for i in range(len(strict(s['annotationsJson'])))}
    return selected, valid, quarantine, sorted(expected - keys)


def integrate(source, out, review=REVIEW):
    out.mkdir(parents=True, exist_ok=False)
    parent = c.lines(review / 'subjects.jsonl')
    batches = []; matrix = []; unified = []; combined = []; combined_errors = []; seen_decisions = set()
    provenance_index = []
    for i in range(1, 19):
        name = f'batch-{i:02}'
        packet = review / 'batches' / name
        batch = out / 'batches' / name
        original = batch / 'original'; original.mkdir(parents=True)
        archive = source / f'diorthosis-review-{name}.zip'
        if not archive.exists():
            archive = source / 'batches' / name / 'original' / archive.name
        files = archive_files(archive)
        shutil.copyfile(archive, original / archive.name)
        for filename, data in files.items():
            (original / filename).write_bytes(data)
        m = strict((packet / 'packet-manifest.json').read_text())
        subjects = c.lines(packet / 'subjects.jsonl'); batches.append(subjects)
        if m['batch'] != i or m['subjects'] != len(subjects) or m['parentSubjectsSha256'] != c.sha(review / 'subjects.jsonl'):
            raise ValueError('Original packet identity/count mismatch: ' + name)
        for filename, digest in m['files'].items():
            if c.sha(packet / filename) != digest:
                raise ValueError('Original packet member hash mismatch: ' + name + '/' + filename)
        with zipfile.ZipFile(packet / 'chatgpt-review.zip') as z:
            if len(z.infolist()) != 4 or set(z.namelist()) != set(m['files']) | {'packet-manifest.json'}:
                raise ValueError('Original packet ZIP inventory mismatch')
            for filename in z.namelist():
                if z.read(filename) != (packet / filename).read_bytes():
                    raise ValueError('Original packet ZIP byte mismatch: ' + name + '/' + filename)
        result = imp.validate(packet, original / 'decisions.jsonl', original / 'provenance.json')
        p = strict((original / 'provenance.json').read_text())
        decisions = c.lines(original / 'decisions.jsonl')
        if any(d['version'] != 1 for d in decisions):
            raise ValueError('Batch request requires decision version 1')
        erows = [strict(line) for line in files['error-decisions.jsonl'].decode('utf-8').splitlines()]
        if 'errorDecisionsSha256' in p and p['errorDecisionsSha256'] != c.sha(original / 'error-decisions.jsonl'):
            raise ValueError('Declared supplementary hash mismatch: ' + name)
        if 'errorDecisionCount' in p and (type(p['errorDecisionCount']) is not int or p['errorDecisionCount'] != len(erows)):
            raise ValueError('Declared supplementary count mismatch: ' + name)
        selected, errors, quarantined, missing_errors = prepare_errors(packet, original / 'error-decisions.jsonl', batch / 'mechanical-subset', {d['subjectId'] for d in decisions})
        validate_error_consistency(decisions, errors)
        # Original provenance is never changed. A declared full-file hash must not authenticate a subset.
        if quarantined and 'errorDecisionsSha256' in p:
            raise ValueError('Cannot import derived subset under original supplementary hash')
        command = ['python3', str(ROOT / 'scripts/import-prompt-quality-review.py'), str(packet), str(original / 'decisions.jsonl'), str(original / 'provenance.json'), str(batch / 'imported'), '--error-decisions', str(selected)]
        proc = subprocess.run(command, cwd=ROOT, capture_output=True, text=True)
        (batch / 'import.log').write_text(proc.stdout + proc.stderr)
        if proc.returncode:
            raise RuntimeError('Strict import failed: ' + name + ': ' + proc.stderr)
        status = 'imported; source-only supplementary records quarantined' if quarantined else 'imported'
        r = dict(batch=name, expectedSubjects=len(subjects), receivedDecisions=len(decisions), validDecisions=result['decisions'], missingDecisions=len(result['missingSubjects']),
            missingSubjectIds=result['missingSubjects'], errors=quarantined, coverageComplete=result['complete'], importStatus=status,
            receivedErrorDecisions=len(erows), validCandidateErrorDecisions=len(errors), missingCandidateErrorKeys=missing_errors,
            errorHashDeclaration='verified' if 'errorDecisionsSha256' in p else 'not declared; importer-computed SHA-256 only',
            originalPacketSha256=c.sha(packet / 'chatgpt-review.zip'), originalSubjectsSha256=c.sha(packet / 'subjects.jsonl'),
            archiveSha256=c.sha(archive), members={n:dict(bytes=len(b), sha256=c.digest(b.decode('utf-8'))) for n,b in files.items()},
            importedErrorFile=str(selected.relative_to(out)), importedErrorSha256=c.sha(selected), humanApprovals=0)
        c.write(batch / 'validation.json', r); matrix.append(r)
        provenance_index.append(dict(batch=name, originalProvenance=str((original / 'provenance.json').relative_to(out)), provenanceSha256=c.sha(original / 'provenance.json'),
            archiveSha256=c.sha(archive), reviewerId=p['reviewerId'], independenceDeclared=p['independenceDeclared'], reviewerKind=p['reviewerKind']))
        sm = {s['subjectId']:s for s in subjects}
        for number, d in enumerate(decisions, 1):
            sid = d['subjectId']
            if sid in seen_decisions:
                raise ValueError('Duplicate submitted parent decision: ' + sid)
            seen_decisions.add(sid)
            unified.append(dict(batch=name, originalDecisionLine=number, originalProvenanceSha256=c.sha(original / 'provenance.json'), subject=sm[sid], decision=d))
        # Preserve exact submitted line bytes; a final separator is necessary only if absent.
        data = files['decisions.jsonl']; combined.append(data + (b'\n' if not data.endswith(b'\n') else b''))
        data = selected.read_bytes(); combined_errors.append(data + (b'\n' if data and not data.endswith(b'\n') else b''))
    missing = reconcile(parent, batches)
    parent_ids = {s['subjectId'] for s in parent}
    if seen_decisions - parent_ids:
        raise ValueError('Unexpected parent decision')
    aggregate = out / 'combined'; aggregate.mkdir()
    (aggregate / 'decisions.jsonl').write_bytes(b''.join(combined))
    (aggregate / 'error-decisions.jsonl').write_bytes(b''.join(combined_errors))
    (aggregate / 'empty-adjudications.jsonl').write_bytes(b'')
    c.write(aggregate / 'provenance-index.json', provenance_index)
    c.write_lines(aggregate / 'analysis-dataset.jsonl', unified)
    # No synthetic overall reviewer provenance: each row points to its own original session declaration.
    imp.validate_errors(review, aggregate / 'error-decisions.jsonl', seen_decisions)
    proc = subprocess.run(['dotnet', str(c.CLI), 'adjudicate', str(review / 'subjects.jsonl'), str(aggregate / 'decisions.jsonl'),
        str(aggregate / 'empty-adjudications.jsonl'), str(aggregate / 'import-report.json'), '--lineage', str(review / 'lineage-DO-NOT-SEND-TO-REVIEWERS.json')], cwd=ROOT, capture_output=True, text=True, check=True)
    (aggregate / 'import.log').write_text(proc.stdout + proc.stderr)
    report = strict((aggregate / 'import-report.json').read_text())
    if any(m['finalized'] for m in report['aggregateUniqueSubjects']['metrics']) or report['aggregateUniqueSubjects']['adjudicationHistory']:
        raise ValueError('Unexpected human finalization')
    coverage = dict(version='phase4-external-integration-1.0.0', sourceCases=len({s['caseId'] for s in parent}), parentSubjects=len(parent), validImportedSubjects=len(seen_decisions),
        missingSubjectIds=sorted(parent_ids-seen_decisions), missingBatchSubjectIds=missing, generationOccurrences=920, batches=matrix, humanApprovals=0,
        candidateErrorDecisions=sum(r['validCandidateErrorDecisions'] for r in matrix), quarantinedSourceOnlyErrorDecisions=sum(len(r['errors']) for r in matrix),
        parentSubjectsSha256=c.sha(review / 'subjects.jsonl'), lineageSha256=c.sha(review / 'lineage-DO-NOT-SEND-TO-REVIEWERS.json'),
        policy='AI diagnostic evidence only. Original bytes and separate provenance retained. No independence inferred across conversations; no missing judgments synthesized.')
    c.write(out / 'coverage.json', coverage)
    md = ['# External batch validation', '', 'Main decisions and candidate-error coverage are counted separately. All human approvals: **0**.', '',
          '| Batch | Expected | Received | Valid | Missing | Scope errors | Candidate errors | Coverage | Import |', '|---|---:|---:|---:|---:|---:|---:|---|---|']
    for r in matrix:
        md.append(f"| {r['batch']} | {r['expectedSubjects']} | {r['receivedDecisions']} | {r['validDecisions']} | {r['missingDecisions']} | {len(r['errors'])} | {r['validCandidateErrorDecisions']} | {'complete' if r['coverageComplete'] else 'partial'} | {r['importStatus']} |")
    md += ['', 'Batch 07 includes six explicitly source-only per-error entries. Their exact records remain in the original file and validation report. A byte-preserving candidate-only subset is imported; source records are never counted as candidate repairs. No ID, hash, rationale or judgment is rewritten.', '',
           'Supplementary-file hashes were declared in batches 01, 02 and 18 and match. Other batches did not declare one; computed hashes establish retained-byte identity, not a retrospective reviewer declaration. All required main/provenance hashes match.']
    (out / 'VALIDATION.md').write_text('\n'.join(md) + '\n')
    print(f"Imported {len(seen_decisions)}/{len(parent)} subjects; {coverage['candidateErrorDecisions']} candidate error judgments; {coverage['quarantinedSourceOnlyErrorDecisions']} source-only extras retained; zero human approvals.")


if __name__ == '__main__':
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('source', type=Path); p.add_argument('new_output', type=Path)
    args = p.parse_args(); integrate(args.source.resolve(), args.new_output.resolve())
