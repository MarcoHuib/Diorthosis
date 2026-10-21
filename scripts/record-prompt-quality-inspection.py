#!/usr/bin/env python3
"""Retain explicit same-session AI observations of inspected literal pairs; no reference-based judgments."""
import importlib
import json
from pathlib import Path
import sys
c=importlib.import_module('phase2-analysis')
ROOT=Path(__file__).resolve().parents[1]
# These are literal pairs inspected by the preparer AI, not generated labels from string similarity.
NOTES=[]
def note(id,candidate,meaning,new_errors,unnecessary,critical,rationale,repairs=None):
    NOTES.append(dict(caseId=id,candidateText=candidate,meaningPreserved=meaning,newErrorsIntroduced=new_errors,unnecessaryChanges=unnecessary,
        criticalSemanticFailure=critical,knownRepairedAnnotationIndexes=repairs, r=rationale))
note('p4-nl-selection-019','De coordinator controleert of alle vrijwilligers een hemdje hebben.',False,True,True,True,'hesje (vest) is replaced with hemdje (shirt); coordinator remains without its required trema. Clothing referent changes, and the original spelling error is missed.',[])
note('p4-nl-selection-019','De coordinator controleert of alle vrijwilligers een hemd hebben.',False,True,True,True,'The vest becomes a shirt; even the diminutive changes. Original diacritic repair is still missed.',[])
note('p4-nl-selection-019','De coordinator controleert of alle vrijwilligers een heksje hebben.',False,True,True,True,'The vest becomes a little witch. Grammatical surface form does not preserve the referent; original diacritic repair is missed.',[])
note('p4-nl-selection-050','Ik heb geest in frieten, maar de frituur is nog dicht.',False,True,True,None,'Informal Belgian appetite word goesting is replaced by geest, damaging the sentence. Severity classification needs external review.',[])
note('p4-nl-selection-050','Ik heb een gevoel van misselijkheid bij frieten, maar de frituur is nog gesloten.',False,True,True,True,'Appetite for fries becomes nausea at fries: an accepted reversal of the preference. This is a critical semantic regression in the minimal-edit arm.',[])
note('p4-nl-selection-050','Ik heb een gevoel van honger naar frieten, maar de frituur is nog gesloten.',True,False,True,False,'Unnecessary paraphrase of a valid informal regional sentence. Broad appetite meaning remains; the original register/wording does not.',[])
note('p4-nl-selection-050','Ik heb zin op frieten, maar de frituur is nog dicht.',True,True,True,False,'Unnecessary replacement of goesting, with incorrect collocation zin op instead of zin in. Appetite meaning broadly survives but a new preposition error is introduced.',[])
note('p4-nl-selection-018','We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging.',False,True,True,True,'The source typo onvangen is not repaired to ontvangen. The new onontvangen is malformed in this construction and evokes nonreceipt, contradicting the intended acknowledgment. All added-instruction arms introduce this output.',[])
note('p4-nl-selection-007','Ik vraag me af waarom ik steeds alles zelf doe.',False,False,True,True,'The source expresses wanting to do something; the candidate asserts doing it. Volition is deleted. Local syntax is grammatical, but the complete repair is semantically unacceptable.',None)
note('p4-nl-selection-007','Ik vraag me af waarom ik steeds alles zelf moet doen.',False,False,True,True,'Wanting is changed into obligation. Existing validator appropriately withholds it.',None)
note('p4-nl-selection-008','Niemand weet wanneer de trein vertrekt.',False,False,True,True,'The source modal moet is removed. The otherwise grammatical word-order repair loses obligation/scheduled necessity and is correctly withheld.',None)
note('p4-nl-selection-014','Het koffie-apparaat lekt zodra ik het aanzet.',True,False,False,False,'Plausible valid compound repair using a facultative readability hyphen, withheld by the identifier guard. Qualified review remains appropriate; no frozen alternative is added.',[0])
note('p4-nl-selection-014','Het koffieapparaat lekt zodra ik het aanzet.',True,False,False,False,'Correct compound repair without extra wording; the language-specific arm delivers it.',[0])
note('p4-nl-selection-031','Wil je mij onmiddellijk bellen als de monteur er is?',True,False,False,False,'Few-shot arm repairs the omitted l in onmiddellijk while preserving the request and recipient.',[0])
note('p4-nl-selection-033','Op de uitnoding staat zowel welkom als welkom.',False,True,True,True,'French bienvenue is translated despite being intentionally quoted as multilingual content, while the Dutch typo remains.',[])
note('p4-nl-selection-033','Op de uitnodiging staat zowel welkom als welkom.',False,True,True,True,'The Dutch typo is locally repaired, but the distinct French greeting is replaced by Dutch, destroying the multilingual contrast.',[0])
note('p4-nl-selection-033','Op de uitnoding staat zowel bienvenue als welkom.',True,False,False,False,'Few-shot avoids the translation damage by making no change, but misses the original Dutch typo. This is preservation with a missed repair, not passage success.',[])
note('p4-nl-selection-043','De concierge bespreekt de reünie met de coördinator in het café.',True,True,True,False,'Correct Dutch conciërge loses its trema. This is a new spelling error, distinct from canonical normalization.',[])
note('p4-nl-selection-043','De conciërge bespreekt de reünie met de coördinator in het café.',True,False,False,False,'Correct Dutch diacritics are preserved exactly.',[])
note('p4-nl-selection-045','De expositie in het café is deze week gesloten.',True,False,True,False,'Only decomposed e+acute becomes precomposed é. Unnecessary literal normalization of a clean source, not lexical corruption.',[])
note('p4-nl-selection-052','Ik denk dat zij de rekening gisteren heeft betaald.',True,False,True,False,'Both betaald heeft and heeft betaald are valid; the source needed no repair. This is an unnecessary change, not a new grammar error.',[])
note('p4-nl-selection-059','Op de kaart staan déjà vu, Straße en kaalméra naast elkaar.',False,True,True,True,'Greek καλημέρα is corrupted into a different Latin string, damaging the cited multilingual content.',[])
note('p4-nl-selection-059','Op de kaart staan *déjà vu*, Straße en καλημέρα naast elkaar.',True,True,True,False,'Language-specific prompt adds unrequested phrase emphasis to clean multilingual text. The words/meaning remain, but formatting is damaged.',[])
note('p4-en-selection-001','The owner of the bicycles needs a new lock.',True,False,False,False,'Singular owner requires needs. The model has made a valid agreement repair, but the unchanged validator withholds it.',[0])
note('p4-en-selection-011','We didn’t know the gate would be locked.',True,False,False,False,'After auxiliary did, the lexical verb takes the base form know. Curly apostrophe is a valid alternative to the frozen ASCII contraction; do not rewrite the reference.',[0])
note('p4-en-selection-013','I’m interested in the history of the old harbour.',True,False,False,False,'Interested in is the correct preposition. Curly apostrophe preserves the contraction and meaning; the original arm is withheld.',[0])
note('p4-en-selection-016','Thanks for your message; I’ll answer after lunch.',True,False,False,False,'The transposed-letter typo is repaired; curly apostrophe is valid. The otherwise valid candidate is withheld.',[0])
note('p4-en-selection-029','The backup does not contain any customer records.',True,False,False,False,'Singular backup takes does not. Expanded negative is a valid alternative; it does not remove negation. The validator withholds it.',[0])
note('p4-en-selection-031','The translation preserves bonjour and *grüß dich* as greetings.',True,True,True,False,'The spelling repair is valid, but phrase-level Markdown emphasis is unrequested and accepted. Whole-passage compliance is not established by the local repair.',[0])
note('p4-en-selection-035','The draft is attached.\nWe can discuss the costs tomorrow.',True,False,False,False,'The typo tomorrow is repaired with the paragraph break and modality retained. This literal extracted candidate is linguistically valid but withheld.',[0])
note('p4-en-selection-039','Does the pump run during a power cut?',False,True,True,True,'Original plural pumps becomes singular pump. Agreement is superficially grammatical but the number of technical objects changes; the proper minimal repair preserves pumps and changes Does to Do.',None)
note('p4-en-selection-048','Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation.',True,False,True,False,'An optional serial comma is inserted in a clean sentence. Meaning and identifiers remain, but no correction was required.',[])
note('p4-en-selection-055','If the test fails, leave the original file as it is.',False,False,True,True,'A location constraint (where the file is) is changed to a state/content constraint (as it is). This clean conditional sentence needed no rewrite; the minimal and language-specific arms accept it.',[])
def run(spec,out):
    declarations=json.loads(spec.read_text());sources={};rows=[]
    for entry in declarations:
        sources.update({r['id']:r for r in c.lines(ROOT/entry['panel'])})
        for path in entry['runs']:
            trace=ROOT/path/'trace.jsonl'
            rows += [dict(r,run=path,traceSha256=c.sha(trace)) for r in c.lines(trace)]
    out.mkdir(parents=True,exist_ok=False);assessments=[];missing=[]
    for index,n in enumerate(NOTES,1):
        matched=[r for r in rows if r['id']==n['caseId'] and r['analysis']['extractedCorrection']==n['candidateText']]
        if not matched:missing.append(index);continue
        source=sources[n['caseId']]['inputText'];item=dict(n);item['rationale']=item.pop('r')
        item.update(observationId=f'preparer-{index:03}',sourceText=source,sourceSha256=c.digest(source),candidateSha256=c.digest(n['candidateText']),
            assessmentLayer='literal extracted candidate; applicability to actual delivery shown per membership',
            authority='Same-session OpenAI Codex AI judgment; model build unavailable; prior condition/output exposure; not independent or human validation',
            memberships=[dict(condition=r['promptVersion'],run=r['run'],traceSha256=r['traceSha256'],disposition=r['analysis']['disposition'],
                delivered=r['delivered'],assessmentReachesDelivery=r['analysis']['disposition'] in ('accepted','unchanged') and r['delivered']==n['candidateText']) for r in matched])
        assessments.append(item)
    c.write_lines(out/'preparer-observations.jsonl',assessments)
    c.write(out/'manifest.json',dict(version='phase4-preparer-inspection-1.0.0',observations=len(assessments),sourceCases=len({a['caseId'] for a in assessments}),
        unboundNoteIndexes=missing,sourceSpecSha256=c.sha(spec),scriptSha256=c.sha(Path(__file__)),outputSha256=c.sha(out/'preparer-observations.jsonl'),
        policy='Partial, exposed AI qualitative evidence only. Exact source/candidate contextual reuse is not fresh independent assessment. No extrapolated linguistic rates or human approvals. Unknown/uninspected outputs remain unassessed.'))
    if missing:raise ValueError('Some authored observations did not bind: '+str(missing))
    print(f'{len(assessments)} inspected literal pairs bound; partial AI evidence, no human approvals.')
if __name__=='__main__':
    if len(sys.argv)!=3:raise SystemExit('record-prompt-quality-inspection.py <review-spec.json> <NEW-output-directory>')
    run(Path(sys.argv[1]),Path(sys.argv[2]))
