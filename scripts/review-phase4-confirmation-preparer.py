#!/usr/bin/env python3
"""Bind the exposed preparer's manual confirmation reading. Never independent or human evidence."""
import copy
import importlib
import json
from pathlib import Path
import unicodedata
import sys
c=importlib.import_module('phase2-analysis')
ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'benchmarks/phase4/v1'
REQUEST='''Continue Phase 4 on PR #7 from the current checkpoint.
The representation results make literal-Unicode JSON a promising experimental substrate, but not an independently validated winner. Keep production v3 unchanged.
Complete the isolated minimal-edit, language-specific and few-shot comparisons using the frozen protocol. Pay particular attention to accepted semantic drift (`hesje` → `hemdje`, `goesting` → `geest`), Unicode corruption, valid repairs withheld by the validator, and clean-text damage.
Preserve the frozen fixture errata and all unsuccessful cases. Choose and lock one candidate before the confirmation panel. Complete the paired confirmation run only if justified, create the blinded ChatGPT review ZIP, publish the full report and verify CI.
Do not promote any prompt or merge the PR. Stop at the external-review checkpoint.'''
REVIEWER='OpenAI Codex preparer (GPT-6 family; exact build unavailable)'
def judgment(justified,meaning,all_repaired,new_errors,clean,critical,rationale,alternatives=None):
    return dict(justified=justified,meaningPreserved=meaning,allDefectsRepaired=all_repaired,noNewErrors=None if new_errors is None else not new_errors,
        cleanPreserved=clean,criticalSemanticFailure=critical,rejectionAppropriate=None,acceptedAlternatives=alternatives or [],errorCategories=[],ambiguity='No independent assessment; severity/region/Greek orthography may require qualified review.',rationale=rationale)
def adjudge(case,row):
    """Manual outcome declarations from the complete displayed confirmation table, with exact binding checks."""
    nl=case['language']=='dutch';i=int(case['id'].rsplit('-',1)[1]);a=row['representation']=='escapedJson';source=case['inputText'];text=row['analysis']['extractedCorrection']
    fixes=[dict(start=e['start'],length=e['length'],originalText=e['originalText'],replacementText=e['acceptedCorrections'][0]['replacementText']) for e in case['errors']]
    expected=unicodedata.normalize('NFC',c.apply(source,fixes)) if fixes else source
    repaired=[0] if fixes else []
    j=judgment(True,True,True,False,True if not fixes else None,False,'Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.')
    if nl and i in (3,7,8):
        expected=source;repaired=[];j=judgment(False,True,False,False,None,False,'The original subordinate order, missing trema, or singular inhoud agreement error remains unchanged.')
    if nl and i==10 and a:
        expected='Het bericht met *trace_id* is verzonden, maar nog niet bevestigd.'
        j=judgment(False,True,True,True,None,False,'Local verzondenne typo is repaired, but backticked technical formatting is replaced by emphasis. Withholding the whole candidate is appropriate.')
    if nl and i==12 and a:
        expected='In het café wachten we totdat de regen ophoudt. 😊'
        j=judgment(False,False,True,True,None,False,'The Dutch typo is repaired, but umbrella emoji becomes a smile. The unrelated symbol substitution makes the complete candidate unacceptable.')
    if nl and i==13:
        expected='De patiënt vroeg de diëtist of '+('huitkase' if a else 'hijttenkäse')+' op het menu mocht blijven.'
        j=judgment(False,False,True,True,False,True,'Correct hüttenkäse in a dietary sentence is replaced by malformed text and accepted. This is a critical lexical/meaning failure in the preparer AI judgment; not a human-confirmed event rate.')
    if nl and i==15 and a:
        expected='client_secret blijft geheim; deel alleen public_key met de ontvanger.'
        j=judgment(False,True,True,True,False,False,'Backticks are removed from two technical identifiers in a clean source. Existing guard correctly withholds formatting damage.')
    if nl and i==17 and not a:
        expected="' s Ochtends is het op dit perron meestal rustig."
        j=judgment(False,True,True,True,False,False,'An incorrect space is inserted in the Dutch contracted time expression. Suppression returns the correct source; abstention is not eligible clean preservation.')
    if nl and i==18 and a:
        expected='Het opschrift luidt hola, bonjour en γειά σας.'
        j=judgment(False,True,True,None,False,None,'An accent is added to the Greek greeting in a clean multilingual source. It is unnecessary. Greek orthographic validity is left unassessed rather than calling every Unicode difference corruption.')
    if nl and i==20:
        expected='Hoi allemaal,\n\nDe sleutels liggen bij de buren. Veel plezier dit weekend!'
        j=judgment(False,True,True,True,False,False,'An unrequested extra paragraph break is inserted; the cosmetic output is suppressed and original is returned.')
    if not nl and i==7 and a:
        expected='We haven’t seen that warning before.'
        j=judgment(True,True,True,False,None,False,'Haven’t takes past participle seen. Curly apostrophe is a valid repair alternative, undercredited by the frozen ASCII reference.',[expected])
    if not nl and i==8 and a:
        expected='The session token contains no personal details.'
        j=judgment(False,False,True,True,None,True,'Agreement is locally repaired but exact technical identifier session_token is replaced by ordinary words. Rejection is appropriate.')
    if not nl and i==9 and a:
        expected='The exhibition includes photographs by Ömer and Chloé.'
        j=judgment(False,False,True,True,None,True,'The typo exhibition is repaired but the name Chloë becomes Chloé. Identity change is correctly rejected.')
    if not nl and i==11:
        expected=('We’ll' if a else "We'll")+' meet outside the café tomorrow morning.'
        j=judgment(True,True,True,False,None,False,'The typo tomorrow is repaired; NFD/NFC equivalence and apostrophe choice preserve the sentence. Both otherwise-valid necessary candidates are withheld.',[expected])
    if not nl and i==13:
        expected='We queued outside the chemist’s while the rain soaked our trousers.'
        j=judgment(False,True,True,False,False,False,'A clean British sentence receives only an unnecessary curly-apostrophe style change. No new grammar error or semantic change is claimed.')
    if not nl and i==15:
        expected='They’ve said they won’t need another copy, haven’t they?'
        j=judgment(False,True,True,False,False,False,'Only correct contractions are restyled. Withholding is appropriate because source needed no correction; no valid necessary repair is lost.')
    if not nl and i==16 and a:
        expected='The résumé mentions work in Curauçao and São Paulo.'
        j=judgment(False,False,True,True,False,True,'Correct Curaçao is corrupted to Curauçao. The proper-name error is correctly rejected.')
    if not nl and i==19:
        expected='Well done! 😊 The whole team is pleased with the result.'
        j=judgment(False,False,True,True,False,False,'Balloon is replaced by a smile in already-correct text. Existing symbol validation correctly withholds it.')
    if not nl and i==20:
        expected='Hi folks,\n\nThere’s fresh coffee in the kitchen. Help yourselves.'
        j=judgment(False,True,True,True,False,False,'Unnecessary paragraph and apostrophe changes are withheld; source text is the correct fallback.')
    if text!=expected:raise ValueError('Manual assessment does not bind exact inspected output: '+case['id']+'/'+row['representation']+' '+repr(text))
    return j,repaired

def run(out):
    out.mkdir(parents=True,exist_ok=False)
    cases={r['id']:r for r in c.lines(BASE/'confirmation.jsonl')};rows=c.lines(BASE/'confirmation-run/trace.jsonl')
    inspected={};summary=[]
    for r in rows:inspected[r['id'],r['promptVersion']]=adjudge(cases[r['id']],r)
    for language in ('dutch','english'):
        for version in sorted({r['promptVersion'] for r in rows}):
            selected=[r for r in rows if r['language']==language and r['promptVersion']==version]
            eligible=lambda r:r['analysis']['disposition'] in ('accepted','unchanged')
            good=lambda j:all(j[k] is True for k in ('justified','meaningPreserved','allDefectsRepaired','noNewErrors'))
            repairs=sum(len(inspected[r['id'],version][1]) for r in selected if eligible(r))
            summary.append(dict(language=language,promptVersion=version,attempted=20,annotatedErrors=12,preparerAIRepairedErrors=repairs,preparerAIMissedErrors=12-repairs,
                eligibleCompletePassages=sum(eligible(r) and good(inspected[r['id'],version][0]) for r in selected),
                eligibleCleanPreserved=sum(eligible(r) and not cases[r['id']]['errors'] and inspected[r['id'],version][0]['cleanPreserved'] is True for r in selected),cleanInputs=8,
                acceptedCriticalSemanticFailures=sum(eligible(r) and inspected[r['id'],version][0]['criticalSemanticFailure'] is True for r in selected),
                withheldOtherwiseValidNecessaryCandidates=sum(not eligible(r) and bool(cases[r['id']]['errors']) and good(inspected[r['id'],version][0]) for r in selected),
                abstentions=sum(not eligible(r) for r in selected)))
    review=BASE/'review';lineage=json.loads((review/'lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text());decisions=[];errors=[]
    lookup={(r['id'],r['promptVersion']):r for r in rows}
    for s in c.lines(review/'subjects.jsonl'):
        origins=[x for x in lineage[s['subjectId']] if x['originalCaseId'] in cases]
        if not origins or s['layer']=='raw':continue
        case=cases[origins[0]['originalCaseId']]
        if s['layer']=='annotations':
            j=judgment(True,None,None,None,None,None,'Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.');repaired=[]
        else:
            choices=[]
            for o in origins:
                row=lookup[o['originalCaseId'],o['arm']];j,repaired=copy.deepcopy(inspected[row['id'],row['promptVersion']])
                if s['layer']=='delivered' and not json.loads(s['metadataJson'])['deliveryEligible']:
                    if s['candidateText']!=case['inputText']:raise ValueError('Unexpected fallback')
                    repaired=[];j=judgment(False,True,not bool(case['errors']),False,True if not case['errors'] else None,False,'Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.')
                elif s['candidateText']!=row['analysis']['extractedCorrection']:raise ValueError('Unexpected candidate binding')
                if s['kind']=='rejection':j['rejectionAppropriate']=not (bool(case['errors']) and all(j[k] is True for k in ('justified','meaningPreserved','allDefectsRepaired','noNewErrors')))
                choices.append((j,repaired))
            j,repaired=choices[0]
            if any(x!=choices[0] for x in choices):raise ValueError('Conflicting reused assessment')
        decisions.append(dict(subjectId=s['subjectId'],bindingSha256=s['bindingSha256'],reviewerId=REVIEWER,reviewerKind='ai',language=s['language'].lower(),
            qualifications='AI linguistic diagnostic assessment; not a qualified human',independenceDeclared=False,version=1,judgment=j))
        if s['layer']!='annotations':
            for index in range(len(case['errors'])):errors.append(dict(subjectId=s['subjectId'],bindingSha256=s['bindingSha256'],annotationIndex=index,repaired=index in repaired,
                rationale='Explicit preparer inspection of this source/candidate pair; local repair is separate from whole-passage safety. '+j['rationale']))
    c.write_lines(out/'decisions.jsonl',decisions);c.write_lines(out/'error-decisions.jsonl',errors)
    c.write(out/'summary.json',dict(version='phase4-exposed-confirmation-ai-1.0.0',authority='Complete same-session preparer reading of extracted/delivered confirmation pairs only; not blind, independent or human validation',
        policy='Every attempted case included. Abstention never credited as delivered repair or eligible clean preservation. Frozen references unchanged; valid curly-apostrophe alternative credited separately.',metrics=summary))
    c.write(out/'provenance.json',dict(reviewerId=REVIEWER,reviewerKind='ai',independenceDeclared=False,modelBuild='unavailable',evaluationDate='2026-10-21',
        priorExposure='Full project/conditions, selection results and once-opened locked confirmation outputs in the same development session.',
        reviewLimitations='This request is the latest visible user instruction, not the entire hidden/context prompt. Manual judgments were written after reading all extracted confirmation pairs. No raw-response assessment or human validation is claimed. Helper verifies literal bindings and projects exact contextual reuse; AI judgments are not guaranteed repeatable.',
        reviewRequest=REQUEST,reviewRequestSha256=c.digest(REQUEST),subjectsSha256=c.sha(review/'subjects.jsonl'),decisionsSha256=c.sha(out/'decisions.jsonl'),
        errorDecisionsSha256=c.sha(out/'error-decisions.jsonl'),subjectCount=len(c.lines(review/'subjects.jsonl')),decisionCount=len(decisions),completeCoverage=False))
    print(json.dumps(summary,indent=2))
if __name__=='__main__':
    if len(sys.argv)!=2:raise SystemExit('review-phase4-confirmation-preparer.py <NEW-output-directory>')
    run(Path(sys.argv[1]))
