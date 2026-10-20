#!/usr/bin/env python3
"""Create original AI-authored, exposed development candidates; never human-approved data."""
import hashlib
import json
from pathlib import Path
import re
import sys

def u16(value): return len(value.encode('utf-16-le')) // 2

def build():
    rows = []
    def add(language, key, tags, text, errors=()):
        annotations=[]
        for original, replacement, category in errors:
            assert text.count(original) == 1, (key, original)
            annotations.append(dict(start=u16(text[:text.index(original)]), length=u16(original), originalText=original,
                                    category=category, acceptedCorrections=[dict(replacementText=replacement)]))
        spans=[]
        for match in re.finditer(r'https?://[^\s]+|`[^`]+`|“[^”]+”|\b\d+(?:[.,:/-]\d+)*\b', text):
            spans.append(dict(start=u16(text[:match.start()]), length=u16(match.group()), originalText=match.group()))
        rows.append(dict(id='p2-'+key,language=language,inputText=text,errors=annotations,phenomena=tags.split(','),
                         protectedSpans=spans,sourceFamily='original-scenario:'+key,documentId=None,
                         provenance=dict(kind='original AI-authored fictional passage',author='Codex AI',date='2026-10-20',
                         batch='phase2-diversity-1',rights='Created for this MIT-licensed repository; no copied external or private text.',
                         generationMethod='Distinct hand-composed scenarios and linguistic targets; no name/date substitution template expansion.'),
                         partition='development-exposed',reviewStatus='pending-two-human-reviews-and-adjudication',
                         ambiguity='AI proposal only: verify idiom, regional variants, categories, minimality and all alternatives.'))
    nl=lambda key,tags,text,errors=(): add('Dutch','nl-'+key,tags,text,errors)
    en=lambda key,tags,text,errors=(): add('English','en-'+key,tags,text,errors)
    nl('mail','professional-email,clean,paragraph', 'Beste collega,\n\nDank voor je toelichting. Ik heb de opmerkingen in het verslag verwerkt. Wil je controleren of de beschrijving van de uitzondering overeenkomt met wat we hebben afgesproken? Daarna kan het stuk naar de werkgroep.\n\nMet vriendelijke groet,\nDe projectsecretaris')
    nl('informal','informal,clean', 'Kom je straks ook? Ik zet alvast koffie; neem gerust iemand mee.')
    nl('belgian','regional,clean', 'Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs.')
    nl('diacritics','diacritics,clean', 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.')
    nl('legal','negation,modality,clean', 'De aanvrager mag de gegevens niet verder verspreiden, tenzij de betrokkene daarvoor schriftelijk toestemming geeft.')
    nl('technical','technical,identifiers,clean', 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.')
    nl('quote','quotation,clean', 'In de notulen staat letterlijk: “Het proefschrift is nog niet openbaar.” Laat die formulering ongewijzigd.')
    nl('dates','dates,numbers,clean', 'De proef loopt van 3 mei tot en met 7 juni 2026. Per deelnemer meten we driemaal de reactietijd.')
    nl('mixed','mixed-language,clean', 'Op de knop staat Save changes; in de Nederlandse handleiding noemen we dit Wijzigingen opslaan.')
    nl('inversion','inversion,clean', 'Heb jij het formulier al verstuurd, of wacht je nog op een handtekening?')
    nl('separable','separable-verb,clean', 'We bellen de leverancier morgen terug zodra de monteur de installatie heeft nagekeken.')
    nl('subordinate','subordinate-order,clean', 'Omdat de bus niet reed, besloot ik thuis te werken en het overleg digitaal bij te wonen.')
    nl('compound','compound,clean', 'De klantenservice zoekt een softwareontwikkelaar die ingewikkelde foutmeldingen begrijpelijk kan uitleggen.')
    nl('awkward','awkward-valid,clean', 'Wat betreft de op zichzelf begrijpelijke vraag naar de planning is het antwoord vooralsnog dat daarover nog geen besluit is genomen.')
    nl('tense','tense,clean', 'Toen we aankwamen, had de beheerder de deur al geopend. Hij stond op ons te wachten.')
    nl('list','list,clean', 'Neem het volgende mee:\n- een geldig bewijs van inschrijving;\n- een pen;\n- de bevestiging van uw afspraak.')
    nl('heading','heading,clean', 'Onderhoud van het filter\n\nSchakel eerst het apparaat uit. Verwijder daarna voorzichtig het deksel.')
    nl('recipe','preposition,clean', 'Roer de saus met een houten lepel en laat haar op laag vuur indikken zonder het deksel op de pan te leggen.')
    nl('comparison','numerical-relation,clean', 'Een lagere waarde betekent hier een groter risico; de schaal loopt dus andersom dan bij de vorige meting.')
    nl('pronouns','agent,clean', 'De redacteur vroeg de onderzoeker of zij haar opmerkingen vóór het overleg aan hem kon doorgeven.')
    nl('concession','modality,clean', 'Het kan zijn dat de levering vertraagd is, maar dat hoeft niet te betekenen dat de bestelling is geannuleerd.')
    nl('long-note','longer-passage,clean', 'Tijdens de oefening bleef de nooduitgang vrij. De waarnemer noteerde wanneer de deelnemers het gebouw verlieten, maar verzamelde geen namen. Na afloop besprak de groep welke aanwijzingen onduidelijk waren. Eén deelnemer merkte op dat het bord boven de trap vanuit de gang niet te zien was. Daarom wordt het bord verplaatst voordat de volgende oefening plaatsvindt. Deze afspraak vervangt de eerdere afspraak over een extra sticker bij de deur niet. Beide maatregelen staan in het verslag en worden afzonderlijk gecontroleerd.')
    nl('service-email','professional-email,longer-passage,clean', 'Geachte lezer,\n\nU ontvangt deze herinnering omdat uw afspraak binnenkort plaatsvindt. Als het tijdstip u niet schikt, kunt u telefonisch een andere afspraak maken. Neem geen vertrouwelijke stukken mee die wij niet hebben opgevraagd. De medewerker bekijkt samen met u welke informatie nodig is en geeft de originele documenten direct terug. Kunt u niet zelf komen, laat ons dan weten of iemand u vertegenwoordigt. Wij bespreken vooraf welk bewijs daarvoor nodig is.\n\nMet vriendelijke groet,\nHet serviceteam')
    nl('emoji','emoji,combining-marks,clean', '🌷 Het label Jose\u0301 blijft precies zo staan; de naam is met een gecombineerd accent opgeslagen.')
    nl('dt','dt', 'De controleur controleerd de bon voordat hij vertrekt.', [('controleerd','controleert','grammar')])
    nl('participle','past-participle', 'Zij heeft de uitnodiging gisteren verstuurt.', [('verstuurt','verstuurd','grammar')])
    nl('separable-error','separable-verb', 'Kun je de monteur morgen op bellen?', [('op bellen','opbellen','spelling')])
    nl('order','subordinate-order', 'Ik weet dat hij heeft het bestand opgeslagen.', [('heeft het bestand opgeslagen','het bestand heeft opgeslagen','sentenceStructure')])
    nl('inversion-error','inversion', 'Na het overleg ik stuur de notulen rond.', [('ik stuur','stuur ik','sentenceStructure')])
    nl('split','compound-splitting', 'De fiets reparatie duurt ongeveer een uur.', [('fiets reparatie','fietsreparatie','spelling')])
    nl('compound-error','compound', 'De lange termijnplanning staat op papier.', [('lange termijnplanning','langetermijnplanning','spelling')])
    nl('accent-error','diacritics', 'Het ideeenboek ligt naast de tekeningen.', [('ideeenboek','ideeënboek','spelling')])
    nl('typo','typing', 'Wil je de bijalge bij dit bericht openen?', [('bijalge','bijlage','spelling')])
    nl('homophone','contextual-homophone', 'De chirurg lijdt het operatieteam tijdens de ingreep.', [('lijdt','leidt','grammar')])
    nl('preposition-error','preposition', 'De cursist is geïnteresseerd voor de geschiedenis van de stad.', [('voor','in','grammar')])
    nl('article','article', 'Het afspraak gaat morgen door.', [('Het','De','grammar')])
    nl('punctuation','punctuation', 'Als het regent blijven we binnen.', [('regent','regent,','punctuation')])
    nl('multi','multiple-error,agreement,spelling', 'De kinderen loop naar de biblioteek.', [('loop','lopen','grammar'),('biblioteek','bibliotheek','spelling')])
    nl('context','context-dependent,word-choice', 'De verpleegkundige verving het verband, omdat de wond nog bloeden. Het oude verband werd in de afvalbak geworpen.', [('bloeden','bloedde','grammar')])
    nl('formal','formal,participle', 'Wij hebben uw verzoek zorgvuldig beoordeelt en sturen u binnenkort een toelichting.', [('beoordeelt','beoordeeld','grammar')])
    en('mail','professional-email,clean,paragraph', 'Hello,\n\nThank you for checking the draft. I have added a note explaining the exception. Could you confirm that it reflects our agreement before I send the document to the working group?\n\nKind regards,\nThe project secretary')
    en('informal','informal,clean', "Fancy a coffee later? No worries if you're busy; we can catch up another day.")
    en('uk','UK-variant,clean', 'The organisation will analyse the colour samples at its new centre.')
    en('us','US-variant,clean', 'The neighborhood theater canceled the evening performance while staff repaired the elevator.')
    en('technical','technical,identifiers,clean', 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.')
    en('quote','quotation,clean', 'The minutes state: “The draft is not yet public.” Preserve that wording when preparing the summary.')
    en('dates','dates,numbers,clean', 'The trial runs from 3 May to 7 June 2026. We measure each participant’s response time three times.')
    en('mixed','mixed-language,clean', 'The Dutch button says Opslaan. In the English instructions, we refer to it as Save.')
    en('negation','negation,modality,clean', 'The applicant must not share the information unless the participant has given written consent.')
    en('aspect','tense-aspect,clean', 'I have been waiting since noon, but the technician has not arrived yet.')
    en('contraction','contractions,clean', "I wouldn't have called if I'd known you were still asleep.")
    en('preposition','preposition,clean', 'The key is on the shelf beside the door, beneath a stack of envelopes.')
    en('awkward','awkward-valid,clean', 'As regards the question of when the work might begin, the position at present is that no decision has been made.')
    en('list','list,clean', 'Please bring:\n- proof of registration;\n- a pen;\n- your appointment confirmation.')
    en('heading','heading,clean', 'Cleaning the filter\n\nSwitch the appliance off first. Then remove the cover carefully.')
    en('comparison','numerical-relation,clean', 'A lower value indicates greater risk here, so this scale runs in the opposite direction to the previous one.')
    en('agent','agent,clean', 'The editor asked the researcher whether she could send him her comments before the meeting.')
    en('conditional','modality,clean', 'A delayed delivery does not necessarily mean that the order has been canceled.')
    en('singular-they','regional-usage,clean', 'If a visitor leaves their umbrella behind, ask them to contact reception.')
    en('punctuation','punctuation,clean', 'The first sample failed; the second, however, passed every check.')
    en('technical-paragraph','technical,paragraph,clean', 'The cache key includes the language code. A missing value must remain distinct from an empty string. If validation fails, return the original input and record the reason locally; never send the text to a remote service.')
    en('long-note','longer-passage,clean', 'During the drill, the emergency exit remained clear. The observer recorded when people left the building but did not collect their names. Afterwards, the group discussed which instructions had been unclear. One participant pointed out that the sign above the stairs could not be seen from the corridor. The sign will therefore be moved before the next drill. This arrangement does not replace the earlier plan to put an additional sticker beside the door. Both measures are listed in the report and will be checked separately.')
    en('service-email','professional-email,longer-passage,clean', 'Dear reader,\n\nThis is a reminder of your upcoming appointment. If the time is inconvenient, please call to arrange another one. Do not bring confidential material that we have not requested. A member of staff will discuss which information is needed and return your original documents immediately. If someone will attend on your behalf, let us know in advance so that we can explain what evidence they need to bring.\n\nKind regards,\nThe service team')
    en('emoji','emoji,combining-marks,clean', '🌷 Keep the label Jose\u0301 exactly as stored; its accent is represented by a combining mark.')
    en('agreement','subject-verb-agreement', 'The list of approved items are on the desk.', [('are','is','grammar')])
    en('article-error','article', 'She is a experienced engineer.', [('a experienced','an experienced','grammar')])
    en('prep-error','preposition', 'We discussed about the revised schedule.', [('discussed about','discussed','grammar')])
    en('tense','tense-aspect', 'I have seen the inspector yesterday.', [('have seen','saw','grammar')])
    en('order','word-order', 'Please explain me the procedure.', [('explain me the procedure','explain the procedure to me','sentenceStructure')])
    en('spell','spelling', 'The museum has a seperate entrance for deliveries.', [('seperate','separate','spelling')])
    en('comma','punctuation', 'After the alarm stopped we returned to the workshop.', [('stopped','stopped,','punctuation')])
    en('contract-error','contraction', "Its not clear why the warning appeared.", [('Its',"It's",'grammar')])
    en('choice','contextual-word-choice', 'Please accept my complement on your thoughtful presentation.', [('complement','compliment','grammar')])
    en('past-participle','past-participle', 'The backup has been took off site.', [('took','taken','grammar')])
    en('multi','multiple-error,agreement,spelling', 'The volunteers was carrying the equiptment upstairs.', [('was','were','grammar'),('equiptment','equipment','spelling')])
    en('count','agreement,count-noun', 'There is several reasons to wait.', [('is','are','grammar')])
    en('infinitive','verb-form', 'The guide suggested to take the earlier train.', [('to take','taking','grammar')])
    en('plural','plural-spelling', 'The new policy applies to all branchs.', [('branchs','branches','spelling')])
    en('relative','relative-clause', 'The device, which battery was empty, stopped recording.', [('which battery','whose battery','grammar')])
    en('parallel','parallelism', 'The role involves scheduling visits and to prepare reports.', [('to prepare','preparing','grammar')])
    return rows

def main():
    directory=Path(sys.argv[1]);directory.mkdir(parents=True,exist_ok=False)
    rows=build()
    with (directory/'cases.jsonl').open('x') as f:
        for row in rows:f.write(json.dumps(row,ensure_ascii=False,separators=(',',':'))+'\n')
    manifest=dict(version='phase2-diversity-1',count=len(rows),languages={lang:dict(cases=sum(r['language']==lang for r in rows),proposedClean=sum(r['language']==lang and not r['errors'] for r in rows)) for lang in ['Dutch','English']},
                  approval='NONE; AI-authored development proposals requiring independent annotation review',independentHumanReviewedErrors=0,independentHumanReviewedClean=0,
                  knownLimitations='Same AI author/batch; related NL/EN scenarios; distinct scenario families are not proven independent sources. No selection or held-out set.',
                  sha256=hashlib.sha256((directory/'cases.jsonl').read_bytes()).hexdigest())
    (directory/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
    print(json.dumps(manifest))
if __name__=='__main__':main()
