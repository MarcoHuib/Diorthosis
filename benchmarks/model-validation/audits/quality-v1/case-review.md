# All 44 references and all 132 v4 outputs: quality audit v1

**Status: proposed AI judgments; independent bilingual human review pending.**

Reference judgments were recorded before opening v4 traces. User examples and historical docs had already disclosed some outputs. Outputs were shuffled with seed 20261020 and model identities hidden during assessment; identities are restored below. The packet is reproducible but this was not a controlled independent blind trial.

All 44 cases require human sign-off. Priority flags identify additional context, register or annotation questions. “Correct minimal correction” on clean inputs means correct preservation (zero edits), not an error repair. Raw labels assess the linguistic response; Markdown/quotes are also recorded as protocol deviations. For contradictory multi-answer output, raw, extracted and accepted judgments differ.

## nl-01

**Input:** `Ik heb met haar gespreekt.`

**Reference-first assessment:** Incorrect irregular past participle: spreken takes gesproken with heb. Existing Grammar annotation and replacement are valid; retain met haar and present-perfect aspect.

**Existing annotations (UTF-16 start/length):**

- Grammar, 16/9: `gespreekt` → `gesproken`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No equally conservative distinct lexical repair needed; sprak would change tense/aspect.

**Regional conventions:** No relevant NL/BE difference.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B075

**Raw:** `Ik heb met haar gesproken.`

**Extracted:** `Ik heb met haar gesproken.`

**Accepted:** `Ik heb met haar gesproken.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Incorrect irregular past participle: spreken takes gesproken with heb. Existing Grammar annotation and replacement are valid; retain met haar and present-perfect aspect.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B024

**Raw:** `Ik heb met haar gesproken.`

**Extracted:** `Ik heb met haar gesproken.`

**Accepted:** `Ik heb met haar gesproken.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Incorrect irregular past participle: spreken takes gesproken with heb. Existing Grammar annotation and replacement are valid; retain met haar and present-perfect aspect.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B067

**Raw:** `Ik heb met haar gesproken.`

**Extracted:** `Ik heb met haar gesproken.`

**Accepted:** `Ik heb met haar gesproken.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Incorrect irregular past participle: spreken takes gesproken with heb. Existing Grammar annotation and replacement are valid; retain met haar and present-perfect aspect.

**Pipeline:** No rejection; accepted edits: 1.

## nl-02

**Input:** `De vergader ruimte is al gereserveerd.`

**Reference-first assessment:** Incorrect separation of a lexical compound. vergaderruimte is the minimal valid repair; existing Spelling annotation is valid.

**Existing annotations (UTF-16 start/length):**

- Spelling, 3/15: `vergader ruimte` → `vergaderruimte`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair; vergaderzaal would substitute a word.

**Regional conventions:** Compound spelling is shared standard Dutch.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B084

**Raw:** `De vergaderruimte is al gereserveerd.`

**Extracted:** `De vergaderruimte is al gereserveerd.`

**Accepted:** `De vergaderruimte is al gereserveerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Incorrect separation of a lexical compound. vergaderruimte is the minimal valid repair; existing Spelling annotation is valid.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B003

**Raw:** `De vergadering is al gereserveerd.`

**Extracted:** `De vergadering is al gereserveerd.`

**Accepted:** `De vergadering is al gereserveerd.`

**Judgments:** raw — Grammatically correct with altered meaning; extracted — Grammatically correct with altered meaning; accepted — Grammatically correct with altered meaning (see reason).

**Reason:** vergader ruimte denotes a meeting room; vergadering denotes a meeting. Reserving a meeting is not the same proposition as reserving the room. The compound-spelling repair should retain ruimte.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B043

**Raw:** `De vergader**ruimte** is al gereserveerd.`

**Extracted:** `De vergaderruimte is al gereserveerd.`

**Accepted:** `De vergaderruimte is al gereserveerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Incorrect separation of a lexical compound. vergaderruimte is the minimal valid repair; existing Spelling annotation is valid.

**Pipeline:** No rejection; accepted edits: 1.

## nl-03

**Input:** `Hij vindt dat de afspraak morgen plaatsvind.`

**Reference-first assessment:** Third-person singular finite verb in the subordinate clause needs plaatsvindt. Existing Grammar repair is valid; Hij vindt and morgen must remain.

**Existing annotations (UTF-16 start/length):**

- Grammar, 33/10: `plaatsvind` → `plaatsvindt`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** No relevant NL/BE difference.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B019

**Raw:** `Hij vindt dat de afspraak morgen plaatsvindt.`

**Extracted:** `Hij vindt dat de afspraak morgen plaatsvindt.`

**Accepted:** `Hij vindt dat de afspraak morgen plaatsvindt.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Third-person singular finite verb in the subordinate clause needs plaatsvindt. Existing Grammar repair is valid; Hij vindt and morgen must remain.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B073

**Raw:** `Hij vindt dat de afspraak morgen **vindt** plaats.`

**Extracted:** `Hij vindt dat de afspraak morgen vindt plaats.`

**Accepted:** `Hij vindt dat de afspraak morgen vindt plaats.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (see reason).

**Reason:** dat de afspraak morgen vindt plaats retains main-clause placement inside a subordinate clause; standard Dutch requires plaatsvindt at the end.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B004

**Raw:** `Hij vindt dat de afspraak morgen **plaats**vindt.`

**Extracted:** `Hij vindt dat de afspraak morgen plaatsvindt.`

**Accepted:** `Hij vindt dat de afspraak morgen plaatsvindt.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Third-person singular finite verb in the subordinate clause needs plaatsvindt. Existing Grammar repair is valid; Hij vindt and morgen must remain.

**Pipeline:** No rejection; accepted edits: 1.

## nl-04

**Input:** `Morgen ik stuur de offerte naar u.`

**Reference-first assessment:** Main-clause V2 violation after fronted Morgen: stuur must precede ik. Existing full-sentence SentenceStructure repair is valid and preserves time, addressee and agency.

**Existing annotations (UTF-16 start/length):**

- SentenceStructure, 0/34: `Morgen ik stuur de offerte naar u.` → `Morgen stuur ik de offerte naar u.`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Ik stuur de offerte morgen naar u is grammatical but shifts information structure; do not prefer it or enumerate arbitrary adverb placements.

**Regional conventions:** V2 requirement holds in standard NL and BE Dutch.

**Ambiguity / priority human review:** Information structure depends on discourse; assume ordinary declarative prose.

### ministral-3b · B093

**Raw:** `Morgen ik stuur de offerte naar u.`

**Extracted:** `Morgen ik stuur de offerte naar u.`

**Accepted:** `Morgen ik stuur de offerte naar u.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Main-clause V2 violation after fronted Morgen: stuur must precede ik. Existing full-sentence SentenceStructure repair is valid and preserves time, addressee and agency. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B056

**Raw:** `"Morgen stuur ik de offerte naar u."`

**Extracted:** `Morgen stuur ik de offerte naar u.`

**Accepted:** `Morgen stuur ik de offerte naar u.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Main-clause V2 violation after fronted Morgen: stuur must precede ik. Existing full-sentence SentenceStructure repair is valid and preserves time, addressee and agency.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B033

**Raw:** `Morgen stuur ik de offerte naar u.`

**Extracted:** `Morgen stuur ik de offerte naar u.`

**Accepted:** `Morgen stuur ik de offerte naar u.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Main-clause V2 violation after fronted Morgen: stuur must precede ik. Existing full-sentence SentenceStructure repair is valid and preserves time, addressee and agency.

**Pipeline:** No rejection; accepted edits: 2.

## nl-05

**Input:** `Hallo hoe gaat het met je?`

**Reference-first assessment:** Greeting and following question need separation in edited prose. Hallo, hoe is a valid minimal punctuation repair. Informal messaging can omit this punctuation.

**Existing annotations (UTF-16 start/length):**

- Punctuation, 0/9: `Hallo hoe` → `Hallo, hoe`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Hallo! Hoe gaat het met je? and Hallo. Hoe gaat het met je? are possible, but introduce separate utterances or emphasis; defer their treatment to a punctuation policy.

**Regional conventions:** No categorical NL/BE difference; punctuation style matters.

**Ambiguity / priority human review:** Register and intended prosody are not specified.

### ministral-3b · B089

**Raw:** `Hallo hoe gaat het met je?`

**Extracted:** `Hallo hoe gaat het met je?`

**Accepted:** `Hallo hoe gaat het met je?`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Greeting and following question need separation in edited prose. Hallo, hoe is a valid minimal punctuation repair. Informal messaging can omit this punctuation. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B047

**Raw:** `Hallo, hoe gaat het met je?`

**Extracted:** `Hallo, hoe gaat het met je?`

**Accepted:** `Hallo, hoe gaat het met je?`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Greeting and following question need separation in edited prose. Hallo, hoe is a valid minimal punctuation repair. Informal messaging can omit this punctuation.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B121

**Raw:** `Hallo, hoe gaat het met je?`

**Extracted:** `Hallo, hoe gaat het met je?`

**Accepted:** `Hallo, hoe gaat het met je?`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Greeting and following question need separation in edited prose. Hallo, hoe is a valid minimal punctuation repair. Informal messaging can omit this punctuation.

**Pipeline:** No rejection; accepted edits: 1.

## nl-06

**Input:** `De gebruikers naam staat in het log bestand.`

**Reference-first assessment:** Two independent compound-spelling errors: gebruikersnaam and logbestand. Both annotations and corrections are valid. Retain two error units.

**Existing annotations (UTF-16 start/length):**

- Spelling, 3/15: `gebruikers naam` → `gebruikersnaam`
- Spelling, 32/11: `log bestand` → `logbestand`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repairs; do not replace technical nouns with synonyms.

**Regional conventions:** Shared standard compound spelling.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B044

**Raw:** `De gebruikersnaam staat in het logbestand.`

**Extracted:** `De gebruikersnaam staat in het logbestand.`

**Accepted:** `De gebruikersnaam staat in het logbestand.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Two independent compound-spelling errors: gebruikersnaam and logbestand. Both annotations and corrections are valid. Retain two error units.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B035

**Raw:** `De gebruikersnaam staat in het logbestand.`

**Extracted:** `De gebruikersnaam staat in het logbestand.`

**Accepted:** `De gebruikersnaam staat in het logbestand.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Two independent compound-spelling errors: gebruikersnaam and logbestand. Both annotations and corrections are valid. Retain two error units.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B074

**Raw:** `De gebruikersnaam staat in het logbestand.`

**Extracted:** `De gebruikersnaam staat in het logbestand.`

**Accepted:** `De gebruikersnaam staat in het logbestand.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Two independent compound-spelling errors: gebruikersnaam and logbestand. Both annotations and corrections are valid. Retain two error units.

**Pipeline:** No rejection; accepted edits: 2.

## nl-07

**Input:** `Er zijn een probleem met de verbinding.`

**Reference-first assessment:** Existential agreement with singular een probleem requires is. Valid Grammar annotation.

**Existing annotations (UTF-16 start/length):**

- Grammar, 3/4: `zijn` → `is`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Changing to problemen would change number and require other changes; not equivalent.

**Regional conventions:** No relevant NL/BE difference.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B087

**Raw:** `Er zijn **een** probleem met de verbinding.`

**Extracted:** `Er zijn een probleem met de verbinding.`

**Accepted:** `Er zijn een probleem met de verbinding.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Existential agreement with singular een probleem requires is. Valid Grammar annotation. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B069

**Raw:** `Er is een probleem met de verbinding.`

**Extracted:** `Er is een probleem met de verbinding.`

**Accepted:** `Er is een probleem met de verbinding.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Existential agreement with singular een probleem requires is. Valid Grammar annotation.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B106

**Raw:** `Er is een probleem met de verbinding.`

**Extracted:** `Er is een probleem met de verbinding.`

**Accepted:** `Er is een probleem met de verbinding.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Existential agreement with singular een probleem requires is. Valid Grammar annotation.

**Pipeline:** No rejection; accepted edits: 1.

## nl-08

**Input:** `Het systeem is onmiddelijk beschikbaar.`

**Reference-first assessment:** onmiddelijk lacks one l; onmiddellijk is valid and minimally repairs spelling.

**Existing annotations (UTF-16 start/length):**

- Spelling, 15/11: `onmiddelijk` → `onmiddellijk`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair; direct is a lexical substitution.

**Regional conventions:** Shared standard spelling.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B102

**Raw:** `Het systeem is onmiddellijk beschikbaar.`

**Extracted:** `Het systeem is onmiddellijk beschikbaar.`

**Accepted:** `Het systeem is onmiddellijk beschikbaar.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** onmiddelijk lacks one l; onmiddellijk is valid and minimally repairs spelling.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B066

**Raw:** `Het systeem is onmiddellijk beschikbaar.`

**Extracted:** `Het systeem is onmiddellijk beschikbaar.`

**Accepted:** `Het systeem is onmiddellijk beschikbaar.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** onmiddelijk lacks one l; onmiddellijk is valid and minimally repairs spelling.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B032

**Raw:** `Het systeem is onmiddellijk beschikbaar.`

**Extracted:** `Het systeem is onmiddellijk beschikbaar.`

**Accepted:** `Het systeem is onmiddellijk beschikbaar.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** onmiddelijk lacks one l; onmiddellijk is valid and minimally repairs spelling.

**Pipeline:** No rejection; accepted edits: 1.

## nl-09

**Input:** `Als hij morgen kom, beginnen we om negen uur.`

**Reference-first assessment:** hij requires komt, not kom. Existing Grammar repair is valid. Als remains conditional and negen uur remains unchanged.

**Existing annotations (UTF-16 start/length):**

- Grammar, 15/3: `kom` → `komt`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Do not change Als to Wanneer or change the time.

**Regional conventions:** No relevant NL/BE difference.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B037

**Raw:** `Als hij morgen komt, beginnen we om negen uur.`

**Extracted:** `Als hij morgen komt, beginnen we om negen uur.`

**Accepted:** `Als hij morgen komt, beginnen we om negen uur.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** hij requires komt, not kom. Existing Grammar repair is valid. Als remains conditional and negen uur remains unchanged.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B098

**Raw:** `Als hij morgen komt, beginnen we om negen uur.`

**Extracted:** `Als hij morgen komt, beginnen we om negen uur.`

**Accepted:** `Als hij morgen komt, beginnen we om negen uur.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** hij requires komt, not kom. Existing Grammar repair is valid. Als remains conditional and negen uur remains unchanged.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B117

**Raw:** `Als hij morgen komt, beginnen we om negen uur.`

**Extracted:** `Als hij morgen komt, beginnen we om negen uur.`

**Accepted:** `Als hij morgen komt, beginnen we om negen uur.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** hij requires komt, not kom. Existing Grammar repair is valid. Als remains conditional and negen uur remains unchanged.

**Pipeline:** No rejection; accepted edits: 1.

## nl-10

**Input:** `Ik heb de adressen gecontrolleerd.`

**Reference-first assessment:** gecontrolleerd has an extra l; gecontroleerd is the valid participle. Spelling category is appropriate for the doubling error.

**Existing annotations (UTF-16 start/length):**

- Spelling, 19/14: `gecontrolleerd` → `gecontroleerd`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Shared standard spelling.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B023

**Raw:** `Ik heb de adressen gecontroleerd.`

**Extracted:** `Ik heb de adressen gecontroleerd.`

**Accepted:** `Ik heb de adressen gecontroleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** gecontrolleerd has an extra l; gecontroleerd is the valid participle. Spelling category is appropriate for the doubling error.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B034

**Raw:** `Ik heb de adressen gecontroleerd.`

**Extracted:** `Ik heb de adressen gecontroleerd.`

**Accepted:** `Ik heb de adressen gecontroleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** gecontrolleerd has an extra l; gecontroleerd is the valid participle. Spelling category is appropriate for the doubling error.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B036

**Raw:** `Ik heb de adressen gecontroleerd.`

**Extracted:** `Ik heb de adressen gecontroleerd.`

**Accepted:** `Ik heb de adressen gecontroleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** gecontrolleerd has an extra l; gecontroleerd is the valid participle. Spelling category is appropriate for the doubling error.

**Pipeline:** No rejection; accepted edits: 1.

## nl-11

**Input:** `De resultaten zijn beter als verwacht.`

**Reference-first assessment:** For standard edited prose, a comparative of inequality takes dan: beter dan verwacht. Existing Grammar repair is justified under that register policy; als is common in speech and regional usage, so the case must not imply speakers are universally ungrammatical.

**Existing annotations (UTF-16 start/length):**

- Grammar, 25/3: `als` → `dan`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Do not accept als as an edited-standard target without changing the benchmark register definition.

**Regional conventions:** Document NL/BE standard versus regional or colloquial usage; see Taaladvies.net groter als/dan.

**Ambiguity / priority human review:** Register-sensitive, with common nonstandard spoken usage.

### ministral-3b · B081

**Raw:** `De resultaten zijn beter dan verwacht.`

**Extracted:** `De resultaten zijn beter dan verwacht.`

**Accepted:** `De resultaten zijn beter dan verwacht.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** For standard edited prose, a comparative of inequality takes dan: beter dan verwacht. Existing Grammar repair is justified under that register policy; als is common in speech and regional usage, so the case must not imply speakers are universally ungrammatical.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B130

**Raw:** `De resultaten zijn beter dan verwacht.`

**Extracted:** `De resultaten zijn beter dan verwacht.`

**Accepted:** `De resultaten zijn beter dan verwacht.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** For standard edited prose, a comparative of inequality takes dan: beter dan verwacht. Existing Grammar repair is justified under that register policy; als is common in speech and regional usage, so the case must not imply speakers are universally ungrammatical.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B123

**Raw:** `De resultaten zijn beter dan verwacht.`

**Extracted:** `De resultaten zijn beter dan verwacht.`

**Accepted:** `De resultaten zijn beter dan verwacht.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** For standard edited prose, a comparative of inequality takes dan: beter dan verwacht. Existing Grammar repair is justified under that register policy; als is common in speech and regional usage, so the case must not imply speakers are universally ungrammatical.

**Pipeline:** No rejection; accepted edits: 1.

## nl-12

**Input:** `Omdat de server opnieuw opstartte. De taak werd onderbroken.`

**Reference-first assessment:** Under continuous-prose interpretation the Omdat clause is stranded. Existing repair is grammatical and preserves causal direction, but unnecessarily reverses clause order. Fronted Omdat plus main-clause inversion preserves the original order and meaning.

**Existing annotations (UTF-16 start/length):**

- SentenceStructure, 0/60: `Omdat de server opnieuw opstartte. De taak werd onderbroken.` → `De taak werd onderbroken omdat de server opnieuw opstartte.`

**Proposal:** Add `Omdat de server opnieuw opstartte, werd de taak onderbroken.`; retain existing alternatives.

**Other alternatives and limits:** Add Omdat de server opnieuw opstartte, werd de taak onderbroken. Do not replace causality with chronology or reverse cause/effect.

**Regional conventions:** Inversion is standard in both NL and BE; no spelling-region conflict.

**Ambiguity / priority human review:** A deliberate answer fragment could be acceptable in dialogue; the dataset supplies no such context.

### ministral-3b · B038

**Raw:** `Omdat de server opnieuw opstartte. De taak werd onderbroken.`

**Extracted:** `Omdat de server opnieuw opstartte. De taak werd onderbroken.`

**Accepted:** `Omdat de server opnieuw opstartte. De taak werd onderbroken.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Under continuous-prose interpretation the Omdat clause is stranded. Existing repair is grammatical and preserves causal direction, but unnecessarily reverses clause order. Fronted Omdat plus main-clause inversion preserves the original order and meaning. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B022

**Raw:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Extracted:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Accepted:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Under continuous-prose interpretation the Omdat clause is stranded. Existing repair is grammatical and preserves causal direction, but unnecessarily reverses clause order. Fronted Omdat plus main-clause inversion preserves the original order and meaning.

**Pipeline:** No rejection; accepted edits: 4.

### ministral-3-14b · B114

**Raw:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Extracted:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Accepted:** `Omdat de server opnieuw opstartte, werd de taak onderbroken.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Under continuous-prose interpretation the Omdat clause is stranded. Existing repair is grammatical and preserves causal direction, but unnecessarily reverses clause order. Fronted Omdat plus main-clause inversion preserves the original order and meaning.

**Pipeline:** No rejection; accepted edits: 4.

## nl-13

**Input:** `We hebben maandag de nieuwe versie geinstalleerd.`

**Reference-first assessment:** The participle requires a diaeresis: geïnstalleerd. Existing Spelling correction is valid and preserves maandag.

**Existing annotations (UTF-16 start/length):**

- Spelling, 35/13: `geinstalleerd` → `geïnstalleerd`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Canonically decomposed i plus diaeresis represents the same spelling, but the evaluator has no Unicode normalization policy; defer code-point equivalence to a versioned proposal.

**Regional conventions:** Shared standard spelling; Unicode representation is a separate technical issue.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B090

**Raw:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Extracted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Accepted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The participle requires a diaeresis: geïnstalleerd. Existing Spelling correction is valid and preserves maandag.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B094

**Raw:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Extracted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Accepted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The participle requires a diaeresis: geïnstalleerd. Existing Spelling correction is valid and preserves maandag.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B072

**Raw:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Extracted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Accepted:** `We hebben maandag de nieuwe versie geïnstalleerd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The participle requires a diaeresis: geïnstalleerd. Existing Spelling correction is valid and preserves maandag.

**Pipeline:** No rejection; accepted edits: 1.

## nl-14

**Input:** `De klant heeft het pakket gisteren ontvangd.`

**Reference-first assessment:** ontvangd is not the standard participle of ontvangen. Existing ontvangen repair is valid and retains perfect aspect and gisteren.

**Existing annotations (UTF-16 start/length):**

- Grammar, 35/8: `ontvangd` → `ontvangen`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Shared irregular participle.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B125

**Raw:** `De klant heeft het pakket gisteren ontvangen.`

**Extracted:** `De klant heeft het pakket gisteren ontvangen.`

**Accepted:** `De klant heeft het pakket gisteren ontvangen.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** ontvangd is not the standard participle of ontvangen. Existing ontvangen repair is valid and retains perfect aspect and gisteren.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B016

**Raw:** `De klant heeft het pakket gisteren ontvangen.`

**Extracted:** `De klant heeft het pakket gisteren ontvangen.`

**Accepted:** `De klant heeft het pakket gisteren ontvangen.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** ontvangd is not the standard participle of ontvangen. Existing ontvangen repair is valid and retains perfect aspect and gisteren.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B057

**Raw:** `De klant heeft het pakket gisteren ontvangen.`

**Extracted:** `De klant heeft het pakket gisteren ontvangen.`

**Accepted:** `De klant heeft het pakket gisteren ontvangen.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** ontvangd is not the standard participle of ontvangen. Existing ontvangen repair is valid and retains perfect aspect and gisteren.

**Pipeline:** No rejection; accepted edits: 1.

## nl-15

**Input:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Reference-first assessment:** Modal Kun je requires coordinated infinitives lezen and laten; en me laten produces grammatical Dutch and retains the recipient me. Existing v2 Grammar repair is valid.

**Existing annotations (UTF-16 start/length):**

- Grammar, 24/10: `en laat me` → `en me laten`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** en mij laten is possible with stronger pronoun emphasis, and Kun je ...? Laat me ... is a different request structure; do not add these automatically.

**Regional conventions:** No relevant NL/BE difference; weak me is valid.

**Ambiguity / priority human review:** The source could blend a question with an imperative; assume coordinated requests under Kun je.

### ministral-3b · B026

**Raw:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Extracted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Accepted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Modal Kun je requires coordinated infinitives lezen and laten; en me laten produces grammatical Dutch and retains the recipient me. Existing v2 Grammar repair is valid. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B083

**Raw:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Extracted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Accepted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Modal Kun je requires coordinated infinitives lezen and laten; en me laten produces grammatical Dutch and retains the recipient me. Existing v2 Grammar repair is valid. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B103

**Raw:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Extracted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Accepted:** `Kun je de bijlage lezen en laat me weten wat je ervan vindt?`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** Modal Kun je requires coordinated infinitives lezen and laten; en me laten produces grammatical Dutch and retains the recipient me. Existing v2 Grammar repair is valid. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

## nl-16

**Input:** `De server is gisteren succesvol opnieuw opgestart.`

**Reference-first assessment:** Correct prose. succesvol functions adverbially; server, restart and yesterday are clear. No correction is needed.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Met succes would be stylistic; retain wording.

**Regional conventions:** No relevant NL/BE difference.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B031

**Raw:** `De server is gisteren succesvol opnieuw opgestart.`

**Extracted:** `De server is gisteren succesvol opnieuw opgestart.`

**Accepted:** `De server is gisteren succesvol opnieuw opgestart.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B099

**Raw:** `De server is gisteren succesvol opnieuw opgestart.`

**Extracted:** `De server is gisteren succesvol opnieuw opgestart.`

**Accepted:** `De server is gisteren succesvol opnieuw opgestart.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B128

**Raw:** `De server is gisteren succesvol opnieuw opgestart.`

**Extracted:** `De server is gisteren succesvol opnieuw opgestart.`

**Accepted:** `De server is gisteren succesvol opnieuw opgestart.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## nl-17

**Input:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Reference-first assessment:** Correct scheduled-event present tense. Protect 19 oktober 2026, 09:30, ApiGateway-v2 and node-04 exactly; no future-tense rewrite is required.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; date-format or tense changes are unsolicited.

**Regional conventions:** Dutch date ordering and lowercase oktober are appropriate.

**Ambiguity / priority human review:** No factual schedule verification is possible or required.

### ministral-3b · B039

**Raw:** `Op 19 oktober 2026 startt ApiGateway-v2 om 09:30 op node-04.`

**Extracted:** `Op 19 oktober 2026 startt ApiGateway-v2 om 09:30 op node-04.`

**Accepted:** `Op 19 oktober 2026 startt ApiGateway-v2 om 09:30 op node-04.`

**Judgments:** raw — Unnecessary change to already-correct text; extracted — Unnecessary change to already-correct text; accepted — Unnecessary change to already-correct text (see reason).

**Reason:** start was correct; startt introduces a third-person verb spelling error. Date, time and identifiers are retained.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B009

**Raw:** `Op 19 oktober 2026 start **ApiGateway-v2** om 09:30 op **node-04**.`

**Extracted:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Accepted:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B025

**Raw:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Extracted:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Accepted:** `Op 19 oktober 2026 start ApiGateway-v2 om 09:30 op node-04.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## nl-18

**Input:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Reference-first assessment:** Correct sentence with a surname and dossier identifier. Preserve De Vries and NL-2026-041; do not infer spelling corrections in names.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required.

**Regional conventions:** Surname-prefix conventions vary by region and registration; preserve supplied name.

**Ambiguity / priority human review:** Actual registered surname capitalization is unknown; source spelling is authoritative.

### ministral-3b · B086

**Raw:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Extracted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Accepted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B018

**Raw:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Extracted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Accepted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B076

**Raw:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Extracted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Accepted:** `Mevrouw De Vries controleert dossier NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## nl-19

**Input:** `De API retourneert HTTP 404 wanneer de cache leeg is.`

**Reference-first assessment:** Correct conditional technical statement. HTTP 404, API and cache are valid terminology; do not judge the application's behavior as a grammar error.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; synonym or API behavior rewrites are inappropriate.

**Regional conventions:** English technical loanwords are acceptable in Dutch.

**Ambiguity / priority human review:** Technical truth is outside proofreading scope.

### ministral-3b · B005

**Raw:** `De API retourneert HTTP 404 wanneer de cache leeg is.`

**Extracted:** `De API retourneert HTTP 404 wanneer de cache leeg is.`

**Accepted:** `De API retourneert HTTP 404 wanneer de cache leeg is.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B002

**Raw:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Extracted:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Accepted:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Judgments:** raw — Unnecessary change to already-correct text; extracted — Unnecessary change to already-correct text; accepted — Unnecessary change to already-correct text (see reason).

**Reason:** HTTP 404 can be returned as a status without an article. een is grammatically defensible through an understood status/error noun but is unnecessary in this correct technical sentence.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B122

**Raw:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Extracted:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Accepted:** `De API retourneert een HTTP 404 wanneer de cache leeg is.`

**Judgments:** raw — Unnecessary change to already-correct text; extracted — Unnecessary change to already-correct text; accepted — Unnecessary change to already-correct text (see reason).

**Reason:** Same article addition as B002: no linguistic error required repair, even if the output can be read as grammatical.

**Pipeline:** No rejection; accepted edits: 1.

## nl-20

**Input:** `De back-up van klant Acme BV staat op https://example.test/rapport?id=42.`

**Reference-first assessment:** Correct back-up spelling and ordinary locative construction. Retain customer Acme BV and the exact URL including query and identifier.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; bedrijfsvorm styling or URL typography is not a correction request.

**Regional conventions:** Retain the supplied corporate abbreviation and standard Dutch back-up spelling.

**Ambiguity / priority human review:** Company registered spelling is not independently known; preserve it.

### ministral-3b · B082

**Raw:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Extracted:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Accepted:** `De back-up van klant Acme BV staat op https://example.test/rapport?id=42.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Plain-text source URL is turned into Markdown with a duplicated URL and delimiters. The destination is unchanged, but literal protected-content/format preservation fails. Rejected by validator; accepted text is unchanged and safe. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a symbol or emoji from the source..

### ministral-3-8b · B124

**Raw:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Extracted:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Accepted:** `De back-up van klant Acme BV staat op https://example.test/rapport?id=42.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Same duplicated Markdown URL transformation as B082. Rejected; accepted text is unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a symbol or emoji from the source..

### ministral-3-14b · B107

**Raw:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Extracted:** `De back-up van klant Acme BV staat op [https://example.test/rapport?id=42](https://example.test/rapport?id=42).`

**Accepted:** `De back-up van klant Acme BV staat op https://example.test/rapport?id=42.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Same duplicated Markdown URL transformation as B082. This is a raw surface violation, not evidence of a changed destination. Rejected; accepted text is unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a symbol or emoji from the source..

## nl-21

**Input:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Reference-first assessment:** Correct Dutch with product names OAuth2 and PostgreSQL. No grammar or spelling repair is needed.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; expanding OAuth2 or respelling brands is unsafe.

**Regional conventions:** Technical terminology should remain invariant.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B013

**Raw:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Extracted:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Accepted:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B085

**Raw:** `Voor deze toepassing gebruiken we OAuth 2.0 en PostgreSQL.`

**Extracted:** `Voor deze toepassing gebruiken we OAuth 2.0 en PostgreSQL.`

**Accepted:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** OAuth2 becomes OAuth 2.0, changing protected identifier and numeric form. Could denote related technology, but such normalization is unauthorized. Rejected; accepted text unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a number, URL, or protected identifier..

### ministral-3-14b · B104

**Raw:** `Voor deze toepassing gebruiken we OAuth 2 en PostgreSQL.`

**Extracted:** `Voor deze toepassing gebruiken we OAuth 2 en PostgreSQL.`

**Accepted:** `Voor deze toepassing gebruiken we OAuth2 en PostgreSQL.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Raw/extracted OAuth2 becomes OAuth 2, changing the literal identifier. The whitespace-only early return emits zero edits, so accepted text actually preserves OAuth2. This is a raw-proposal and trace-observability issue, not an accepted identifier violation. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## nl-22

**Input:** `De wijziging is op 3 mei 2025 door team Atlas goedgekeurd.`

**Reference-first assessment:** Correct passive perfect construction. Preserve approval, team Atlas and 3 mei 2025; no tense or voice change needed.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required.

**Regional conventions:** Dutch date convention is appropriate.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B126

**Raw:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Extracted:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Accepted:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Judgments:** raw — Unnecessary change to already-correct text; extracted — Unnecessary change to already-correct text; accepted — Unnecessary change to already-correct text (see reason).

**Reason:** Same optional het insertion as B119; original is correct and meaning is substantially retained.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B119

**Raw:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Extracted:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Accepted:** `De wijziging is op 3 mei 2025 door het team Atlas goedgekeurd.`

**Judgments:** raw — Unnecessary change to already-correct text; extracted — Unnecessary change to already-correct text; accepted — Unnecessary change to already-correct text (see reason).

**Reason:** team Atlas is already valid Dutch. Adding het is optional phrasing and should not be counted as a necessary proofreading repair.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B079

**Raw:** `De wijziging is op 3 mei 2025 door team Atlas goedgekeurd.`

**Extracted:** `De wijziging is op 3 mei 2025 door team Atlas goedgekeurd.`

**Accepted:** `De wijziging is op 3 mei 2025 door team Atlas goedgekeurd.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-01

**Input:** `She have send the documents yesterday.`

**Reference-first assessment:** have send is malformed. The past-simple sent is the least assumptive correction with yesterday and is a valid existing Grammar target. The span bundles agreement, participle/auxiliary and tense issues into one annotated error.

**Existing annotations (UTF-16 start/length):**

- Grammar, 4/9: `have send` → `sent`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** She had sent the documents yesterday is grammatical only with an understood past reference point; it is not automatically equivalent. She has sent ... yesterday is not the neutral edited-standard solution.

**Regional conventions:** Both standard British and American edited English favor simple past for this isolated completed event; colloquial present-perfect variation does not settle intent.

**Ambiguity / priority human review:** Original tense intent is underdetermined; independent human review required before accepting past perfect or claiming semantic equivalence.

### ministral-3b · B015

**Raw:** `She has sent the documents yesterday.`

**Extracted:** `She has sent the documents yesterday.`

**Accepted:** `She has sent the documents yesterday.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (see reason).

**Reason:** has sent repairs agreement and participle but leaves present perfect with definite finished-time yesterday in this isolated standard-prose context.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B020

**Raw:** `She **had** sent the documents yesterday.`

**Extracted:** `She had sent the documents yesterday.`

**Accepted:** `She had sent the documents yesterday.`

**Judgments:** raw — Grammatically acceptable but semantically ambiguous; extracted — Grammatically acceptable but semantically ambiguous; accepted — Grammatically acceptable but semantically ambiguous (see reason).

**Reason:** had sent is grammatical with an implicit past reference point but introduces a temporal relation not established by the source. Do not equate it automatically with sent; independent human context review required.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B050

**Raw:** `She had sent the documents yesterday.`

**Extracted:** `She had sent the documents yesterday.`

**Accepted:** `She had sent the documents yesterday.`

**Judgments:** raw — Grammatically acceptable but semantically ambiguous; extracted — Grammatically acceptable but semantically ambiguous; accepted — Grammatically acceptable but semantically ambiguous (see reason).

**Reason:** Same past-perfect ambiguity as B020. No automatic full repair credit in the strict proposal; no assertion that the sentence is ungrammatical.

**Pipeline:** No rejection; accepted edits: 2.

## en-02

**Input:** `Please recieve the signed form.`

**Reference-first assessment:** recieve is misspelled; receive is the valid minimal Spelling repair.

**Existing annotations (UTF-16 start/length):**

- Spelling, 7/7: `recieve` → `receive`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Same spelling in British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B046

**Raw:** `Please receive the signed form.`

**Extracted:** `Please receive the signed form.`

**Accepted:** `Please receive the signed form.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieve is misspelled; receive is the valid minimal Spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B109

**Raw:** `Please receive the signed form.`

**Extracted:** `Please receive the signed form.`

**Accepted:** `Please receive the signed form.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieve is misspelled; receive is the valid minimal Spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B006

**Raw:** `Please receive the signed form.`

**Extracted:** `Please receive the signed form.`

**Accepted:** `Please receive the signed form.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieve is misspelled; receive is the valid minimal Spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

## en-03

**Input:** `The informations are included in the report.`

**Reference-first assessment:** In the ordinary informational reading, information is a mass noun and takes is. Both existing Grammar repairs are valid, but they are dependent: fixing the noun alone leaves agreement wrong. This is two annotations versus the bundled treatment in en-01.

**Existing annotations (UTF-16 start/length):**

- Grammar, 4/12: `informations` → `information`
- Grammar, 17/3: `are` → `is`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Do not replace information with details merely to preserve plural agreement.

**Regional conventions:** Ordinary mass-noun usage is shared by British and American English.

**Ambiguity / priority human review:** Specialized legal informations can be countable; context should explicitly exclude that reading in a future held-out case.

### ministral-3b · B040

**Raw:** `The information is included in the report.`

**Extracted:** `The information is included in the report.`

**Accepted:** `The information is included in the report.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In the ordinary informational reading, information is a mass noun and takes is. Both existing Grammar repairs are valid, but they are dependent: fixing the noun alone leaves agreement wrong. This is two annotations versus the bundled treatment in en-01.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B028

**Raw:** `The information is included in the report.`

**Extracted:** `The information is included in the report.`

**Accepted:** `The information is included in the report.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In the ordinary informational reading, information is a mass noun and takes is. Both existing Grammar repairs are valid, but they are dependent: fixing the noun alone leaves agreement wrong. This is two annotations versus the bundled treatment in en-01.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B071

**Raw:** `The information is included in the report.`

**Extracted:** `The information is included in the report.`

**Accepted:** `The information is included in the report.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In the ordinary informational reading, information is a mass noun and takes is. Both existing Grammar repairs are valid, but they are dependent: fixing the noun alone leaves agreement wrong. This is two annotations versus the bundled treatment in en-01.

**Pipeline:** No rejection; accepted edits: 2.

## en-04

**Input:** `Tomorrow I will the package send.`

**Reference-first assessment:** English verb-object order requires send the package. Existing I will send the package tomorrow is valid, but shifts the time adjunct. Keeping Tomorrow initially is equally valid and closer to the input. The short introductory adjunct allows a comma or none.

**Existing annotations (UTF-16 start/length):**

- SentenceStructure, 0/33: `Tomorrow I will the package send.` → `I will send the package tomorrow.`

**Proposal:** Add `Tomorrow I will send the package.`, `Tomorrow, I will send the package.`; retain existing alternatives.

**Other alternatives and limits:** Add Tomorrow I will send the package. and Tomorrow, I will send the package. Both retain agent, futurity, object and time.

**Regional conventions:** Introductory-comma preferences vary by style, not a categorical UK/US correctness split.

**Ambiguity / priority human review:** Temporal focus is discourse-dependent, favor preservation of the original ordering.

### ministral-3b · B092

**Raw:** `Tomorrow I will send the package.`

**Extracted:** `Tomorrow I will send the package.`

**Accepted:** `Tomorrow I will send the package.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** English verb-object order requires send the package. Existing I will send the package tomorrow is valid, but shifts the time adjunct. Keeping Tomorrow initially is equally valid and closer to the input. The short introductory adjunct allows a comma or none.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B054

**Raw:** `Tomorrow, I will send the package.`

**Extracted:** `Tomorrow, I will send the package.`

**Accepted:** `Tomorrow, I will send the package.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** English verb-object order requires send the package. Existing I will send the package tomorrow is valid, but shifts the time adjunct. Keeping Tomorrow initially is equally valid and closer to the input. The short introductory adjunct allows a comma or none.

**Pipeline:** No rejection; accepted edits: 3.

### ministral-3-14b · B059

**Raw:** `Tomorrow, I will send the package.`

**Extracted:** `Tomorrow, I will send the package.`

**Accepted:** `Tomorrow, I will send the package.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** English verb-object order requires send the package. Existing I will send the package tomorrow is valid, but shifts the time adjunct. Keeping Tomorrow initially is equally valid and closer to the input. The short introductory adjunct allows a comma or none.

**Pipeline:** No rejection; accepted edits: 3.

## en-05

**Input:** `Wait lets review the result.`

**Reference-first assessment:** The intended invitation needs let's (let us), and Wait needs separation. Existing Wait, let's is valid; both straight and typographic apostrophes express the same contraction. Two punctuation defects are bundled into one annotation.

**Existing annotations (UTF-16 start/length):**

- Punctuation, 0/9: `Wait lets` → `Wait, let's`

**Proposal:** Add `Wait, let’s`; retain existing alternatives.

**Other alternatives and limits:** Add Wait, let’s. Wait! Let's and Wait; let's are plausible punctuation alternatives but change utterance segmentation/emphasis; defer to human policy rather than enumerate all variants.

**Regional conventions:** ASCII U+0027 and typographic U+2019 are both acceptable English apostrophe representations here.

**Ambiguity / priority human review:** Punctuation prosody and register are unspecified.

### ministral-3b · B007

**Raw:** `Wait, let’s review the result.`

**Extracted:** `Wait, let’s review the result.`

**Accepted:** `Wait, let’s review the result.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The intended invitation needs let's (let us), and Wait needs separation. Existing Wait, let's is valid; both straight and typographic apostrophes express the same contraction. Two punctuation defects are bundled into one annotation.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B068

**Raw:** `Wait, let’s review the result.`

**Extracted:** `Wait, let’s review the result.`

**Accepted:** `Wait, let’s review the result.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The intended invitation needs let's (let us), and Wait needs separation. Existing Wait, let's is valid; both straight and typographic apostrophes express the same contraction. Two punctuation defects are bundled into one annotation.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B118

**Raw:** `Wait, let’s review the result.`

**Extracted:** `Wait, let’s review the result.`

**Accepted:** `Wait, let’s review the result.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** The intended invitation needs let's (let us), and Wait needs separation. Existing Wait, let's is valid; both straight and typographic apostrophes express the same contraction. Two punctuation defects are bundled into one annotation.

**Pipeline:** No rejection; accepted edits: 2.

## en-06

**Input:** `The user succesfully logged in.`

**Reference-first assessment:** succesfully lacks one s; successfully is the minimal valid Spelling correction.

**Existing annotations (UTF-16 start/length):**

- Spelling, 9/11: `succesfully` → `successfully`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Same spelling in British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B014

**Raw:** `The user successfully logged in.`

**Extracted:** `The user successfully logged in.`

**Accepted:** `The user successfully logged in.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** succesfully lacks one s; successfully is the minimal valid Spelling correction.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B113

**Raw:** `The user successfully logged in.`

**Extracted:** `The user successfully logged in.`

**Accepted:** `The user successfully logged in.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** succesfully lacks one s; successfully is the minimal valid Spelling correction.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B108

**Raw:** `The user successfully logged in.`

**Extracted:** `The user successfully logged in.`

**Accepted:** `The user successfully logged in.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** succesfully lacks one s; successfully is the minimal valid Spelling correction.

**Pipeline:** No rejection; accepted edits: 1.

## en-07

**Input:** `They was waiting outside the office.`

**Reference-first assessment:** In standard edited English, plural They requires were. Existing Grammar repair is valid; do not change the subject or progressive aspect.

**Existing annotations (UTF-16 start/length):**

- Grammar, 5/3: `was` → `were`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal edited-standard repair.

**Regional conventions:** Dialect legitimacy is distinct from this edited-standard test.

**Ambiguity / priority human review:** They was occurs in dialects; the benchmark must state its standard-prose target.

### ministral-3b · B062

**Raw:** `They were waiting outside the office.`

**Extracted:** `They were waiting outside the office.`

**Accepted:** `They were waiting outside the office.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In standard edited English, plural They requires were. Existing Grammar repair is valid; do not change the subject or progressive aspect.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B077

**Raw:** `They were waiting outside the office.`

**Extracted:** `They were waiting outside the office.`

**Accepted:** `They were waiting outside the office.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In standard edited English, plural They requires were. Existing Grammar repair is valid; do not change the subject or progressive aspect.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B112

**Raw:** `They were waiting outside the office.`

**Extracted:** `They were waiting outside the office.`

**Accepted:** `They were waiting outside the office.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In standard edited English, plural They requires were. Existing Grammar repair is valid; do not change the subject or progressive aspect.

**Pipeline:** No rejection; accepted edits: 1.

## en-08

**Input:** `I look forward to hear from you.`

**Reference-first assessment:** look forward to takes a nominal/gerund complement: to hearing. Existing Grammar repair is valid and preserves the expressed anticipation.

**Existing annotations (UTF-16 start/length):**

- Grammar, 15/7: `to hear` → `to hearing`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair; hope to hear changes the predicate.

**Regional conventions:** Shared standard British/American construction.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B132

**Raw:** `I look forward to hearing from you.`

**Extracted:** `I look forward to hearing from you.`

**Accepted:** `I look forward to hearing from you.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** look forward to takes a nominal/gerund complement: to hearing. Existing Grammar repair is valid and preserves the expressed anticipation.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B027

**Raw:** `I look forward to hearing from you.`

**Extracted:** `I look forward to hearing from you.`

**Accepted:** `I look forward to hearing from you.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** look forward to takes a nominal/gerund complement: to hearing. Existing Grammar repair is valid and preserves the expressed anticipation.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B049

**Raw:** `I look forward to hearing from you.`

**Extracted:** `I look forward to hearing from you.`

**Accepted:** `I look forward to hearing from you.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** look forward to takes a nominal/gerund complement: to hearing. Existing Grammar repair is valid and preserves the expressed anticipation.

**Pipeline:** No rejection; accepted edits: 1.

## en-09

**Input:** `Wait lets proceed with the review.`

**Reference-first assessment:** Same contraction and separation errors as en-05; the existing correction is valid but omits the typographic apostrophe equivalent. This is a near-duplicate, not independent coverage of another construction.

**Existing annotations (UTF-16 start/length):**

- Punctuation, 0/9: `Wait lets` → `Wait, let's`

**Proposal:** Add `Wait, let’s`; retain existing alternatives.

**Other alternatives and limits:** Add Wait, let’s under the same policy as en-05; apply punctuation alternatives symmetrically.

**Regional conventions:** Both apostrophe representations acceptable here.

**Ambiguity / priority human review:** Punctuation prosody and register unspecified; near-duplicate of en-05.

### ministral-3b · B064

**Raw:** `Wait, let’s proceed with the review.`

**Extracted:** `Wait, let’s proceed with the review.`

**Accepted:** `Wait, let’s proceed with the review.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Same contraction and separation errors as en-05; the existing correction is valid but omits the typographic apostrophe equivalent. This is a near-duplicate, not independent coverage of another construction.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B088

**Raw:** `Wait, let’s proceed with the review.`

**Extracted:** `Wait, let’s proceed with the review.`

**Accepted:** `Wait, let’s proceed with the review.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Same contraction and separation errors as en-05; the existing correction is valid but omits the typographic apostrophe equivalent. This is a near-duplicate, not independent coverage of another construction.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B051

**Raw:** `Wait, let’s proceed with the review.`

**Extracted:** `Wait, let’s proceed with the review.`

**Accepted:** `Wait, let’s proceed with the review.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Same contraction and separation errors as en-05; the existing correction is valid but omits the typographic apostrophe equivalent. This is a near-duplicate, not independent coverage of another construction.

**Pipeline:** No rejection; accepted edits: 2.

## en-10

**Input:** `We recieved your request this morning.`

**Reference-first assessment:** recieved is misspelled; received is a valid minimal Spelling repair. Preserve this morning and the subject.

**Existing annotations (UTF-16 start/length):**

- Spelling, 3/8: `recieved` → `received`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Same spelling in British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B080

**Raw:** `We received your request this morning.`

**Extracted:** `We received your request this morning.`

**Accepted:** `We received your request this morning.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieved is misspelled; received is a valid minimal Spelling repair. Preserve this morning and the subject.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B017

**Raw:** `We received your request this morning.`

**Extracted:** `We received your request this morning.`

**Accepted:** `We received your request this morning.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieved is misspelled; received is a valid minimal Spelling repair. Preserve this morning and the subject.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B116

**Raw:** `We received your request this morning.`

**Extracted:** `We received your request this morning.`

**Accepted:** `We received your request this morning.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** recieved is misspelled; received is a valid minimal Spelling repair. Preserve this morning and the subject.

**Pipeline:** No rejection; accepted edits: 1.

## en-11

**Input:** `After the update completed. The service restarted.`

**Reference-first assessment:** After introduces a dependent clause, stranded by the full stop under continuous-prose interpretation. Joining with a comma and lowercasing the is a valid minimal SentenceStructure repair. Intransitive completed is acceptable for a process; do not mandate was completed.

**Existing annotations (UTF-16 start/length):**

- SentenceStructure, 0/50: `After the update completed. The service restarted.` → `After the update completed, the service restarted.`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** The service restarted after the update completed is grammatical but changes information structure; no need to favor it.

**Regional conventions:** No material UK/US difference in this repair.

**Ambiguity / priority human review:** A deliberate fragment is possible in dialogue or narrative; dataset assumes continuous prose.

### ministral-3b · B105

**Raw:** `After the update completed, the service restarted.`

**Extracted:** `After the update completed, the service restarted.`

**Accepted:** `After the update completed, the service restarted.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** After introduces a dependent clause, stranded by the full stop under continuous-prose interpretation. Joining with a comma and lowercasing the is a valid minimal SentenceStructure repair. Intransitive completed is acceptable for a process; do not mandate was completed.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-8b · B030

**Raw:** `After the update completed, the service restarted.`

**Extracted:** `After the update completed, the service restarted.`

**Accepted:** `After the update completed, the service restarted.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** After introduces a dependent clause, stranded by the full stop under continuous-prose interpretation. Joining with a comma and lowercasing the is a valid minimal SentenceStructure repair. Intransitive completed is acceptable for a process; do not mandate was completed.

**Pipeline:** No rejection; accepted edits: 2.

### ministral-3-14b · B011

**Raw:** `After the update completed, the service restarted.`

**Extracted:** `After the update completed, the service restarted.`

**Accepted:** `After the update completed, the service restarted.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** After introduces a dependent clause, stranded by the full stop under continuous-prose interpretation. Joining with a comma and lowercasing the is a valid minimal SentenceStructure repair. Intransitive completed is acceptable for a process; do not mandate was completed.

**Pipeline:** No rejection; accepted edits: 2.

## en-12

**Input:** `Please confirm wether the backup finished.`

**Reference-first assessment:** wether is a real word for a castrated male sheep but is wrong in this complement position. whether is the valid contextual spelling repair.

**Existing annotations (UTF-16 start/length):**

- Spelling, 15/6: `wether` → `whether`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** if is possible syntactically but changes wording and can have conditional readings; retain whether.

**Regional conventions:** Same spelling in British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B053

**Raw:** `Please confirm whether the backup finished.`

**Extracted:** `Please confirm whether the backup finished.`

**Accepted:** `Please confirm whether the backup finished.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** wether is a real word for a castrated male sheep but is wrong in this complement position. whether is the valid contextual spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B063

**Raw:** `"Please confirm whether the backup finished."`

**Extracted:** `Please confirm whether the backup finished.`

**Accepted:** `Please confirm whether the backup finished.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** wether is a real word for a castrated male sheep but is wrong in this complement position. whether is the valid contextual spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B041

**Raw:** `Please confirm whether the backup finished.`

**Extracted:** `Please confirm whether the backup finished.`

**Accepted:** `Please confirm whether the backup finished.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** wether is a real word for a castrated male sheep but is wrong in this complement position. whether is the valid contextual spelling repair.

**Pipeline:** No rejection; accepted edits: 1.

## en-13

**Input:** `There is several reasons for the delay.`

**Reference-first assessment:** In standard edited prose, existential agreement with several reasons requires are. Existing Grammar repair is valid; preserve number and several.

**Existing annotations (UTF-16 start/length):**

- Grammar, 6/2: `is` → `are`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Changing to a single reason would alter quantity and meaning.

**Regional conventions:** Variation in spoken agreement does not invalidate the edited-standard reference.

**Ambiguity / priority human review:** Singular existential agreement occurs colloquially; target register needs to be declared.

### ministral-3b · B060

**Raw:** `There is several reasons for the delay.`

**Extracted:** `There is several reasons for the delay.`

**Accepted:** `There is several reasons for the delay.`

**Judgments:** raw — Incorrect or incomplete correction; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (missed repair).

**Reason:** In standard edited prose, existential agreement with several reasons requires are. Existing Grammar repair is valid; preserve number and several. Output leaves the error unchanged.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B008

**Raw:** `There is several reasons for the delay.\n\n*(Corrected to: "There are several reasons for the delay.")*`

**Extracted:** `There is several reasons for the delay.`

**Accepted:** `There is several reasons for the delay.`

**Judgments:** raw — Requires independent human review; extracted — Incorrect or incomplete correction; accepted — Incorrect or incomplete correction (see reason).

**Reason:** Raw response first repeats the incorrect sentence, then supplies the correct There are sentence in a note. It contains a correct answer but does not unambiguously deliver the requested single corrected passage. The parser selects the incorrect first sentence. This is a response-format/extraction issue, not a missing gold alternative.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B100

**Raw:** `There are several reasons for the delay.`

**Extracted:** `There are several reasons for the delay.`

**Accepted:** `There are several reasons for the delay.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** In standard edited prose, existential agreement with several reasons requires are. Existing Grammar repair is valid; preserve number and several.

**Pipeline:** No rejection; accepted edits: 1.

## en-14

**Input:** `The report was send to the wrong address.`

**Reference-first assessment:** Passive was requires the past participle sent. Existing Grammar repair is valid; retain wrong address and passive meaning.

**Existing annotations (UTF-16 start/length):**

- Grammar, 15/4: `send` → `sent`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Shared irregular participle.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B129

**Raw:** `The report was sent to the wrong address.`

**Extracted:** `The report was sent to the wrong address.`

**Accepted:** `The report was sent to the wrong address.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Passive was requires the past participle sent. Existing Grammar repair is valid; retain wrong address and passive meaning.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B055

**Raw:** `The report was sent to the wrong address.`

**Extracted:** `The report was sent to the wrong address.`

**Accepted:** `The report was sent to the wrong address.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Passive was requires the past participle sent. Existing Grammar repair is valid; retain wrong address and passive meaning.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B097

**Raw:** `The report was sent to the wrong address.`

**Extracted:** `The report was sent to the wrong address.`

**Accepted:** `The report was sent to the wrong address.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** Passive was requires the past participle sent. Existing Grammar repair is valid; retain wrong address and passive meaning.

**Pipeline:** No rejection; accepted edits: 1.

## en-15

**Input:** `We need to seperate the two configuration files.`

**Reference-first assessment:** seperate is misspelled; separate is the valid minimal repair. Preserve two configuration files and obligation need to.

**Existing annotations (UTF-16 start/length):**

- Spelling, 11/8: `seperate` → `separate`

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** No additional minimal repair.

**Regional conventions:** Same spelling in British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B101

**Raw:** `We need to separate the two configuration files.`

**Extracted:** `We need to separate the two configuration files.`

**Accepted:** `We need to separate the two configuration files.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** seperate is misspelled; separate is the valid minimal repair. Preserve two configuration files and obligation need to.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-8b · B131

**Raw:** `We need to separate the two configuration files.`

**Extracted:** `We need to separate the two configuration files.`

**Accepted:** `We need to separate the two configuration files.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** seperate is misspelled; separate is the valid minimal repair. Preserve two configuration files and obligation need to.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B021

**Raw:** `We need to **separate** the two configuration files.`

**Extracted:** `We need to separate the two configuration files.`

**Accepted:** `We need to separate the two configuration files.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (minimal repair).

**Reason:** seperate is misspelled; separate is the valid minimal repair. Preserve two configuration files and obligation need to.

**Pipeline:** No rejection; accepted edits: 1.

## en-16

**Input:** `The server restarted successfully yesterday.`

**Reference-first assessment:** Correct past-tense sentence. successfully is a legitimate adverb; no change is needed.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required.

**Regional conventions:** Valid British and American English.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B010

**Raw:** `The server restarted successfully yesterday.`

**Extracted:** `The server restarted successfully yesterday.`

**Accepted:** `The server restarted successfully yesterday.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B029

**Raw:** `The server restarted successfully yesterday.`

**Extracted:** `The server restarted successfully yesterday.`

**Accepted:** `The server restarted successfully yesterday.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B012

**Raw:** `The server restarted successfully yesterday.`

**Extracted:** `The server restarted successfully yesterday.`

**Accepted:** `The server restarted successfully yesterday.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-17

**Input:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Reference-first assessment:** Correct scheduled-event present tense and date commas. Preserve October 19, 2026, 09:30, ApiGateway-v2 and node-04; starts does not require will start.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Future tense or date reformatting is not needed.

**Regional conventions:** American month-day date format is valid even if another region prefers day-month.

**Ambiguity / priority human review:** Schedule truth cannot be checked from this sentence and is outside scope.

### ministral-3b · B048

**Raw:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Extracted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Accepted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B058

**Raw:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node 04.`

**Extracted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node 04.`

**Accepted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node 04.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Protected-content violation (see reason).

**Reason:** node-04 becomes node 04, altering an exact technical identifier. Removing the hyphen can change a host/resource name. Validator accepts the change: demonstrated safety-coverage gap.

**Pipeline:** No rejection; accepted edits: 1.

### ministral-3-14b · B001

**Raw:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Extracted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Accepted:** `On October 19, 2026, ApiGateway-v2 starts at 09:30 on node-04.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-18

**Input:** `Ms. De Vries reviewed case NL-2026-041.`

**Reference-first assessment:** Correct title, surname and past-tense verb. Protect De Vries and NL-2026-041.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; changing Ms. to Ms is optional regional house style, not a necessary correction.

**Regional conventions:** Ms. is valid US style; British Ms without a full stop is also conventional. Do not normalize unsolicited.

**Ambiguity / priority human review:** Actual name spelling unknown; source is authoritative.

### ministral-3b · B045

**Raw:** `Ms. De Vries reviewed case NL-2026-041.`

**Extracted:** `Ms. De Vries reviewed case NL-2026-041.`

**Accepted:** `Ms. De Vries reviewed case NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B127

**Raw:** `Ms. De Vries reviewed case NL-2026-041.`

**Extracted:** `Ms. De Vries reviewed case NL-2026-041.`

**Accepted:** `Ms. De Vries reviewed case NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B078

**Raw:** `Ms. De Vries reviewed case NL-2026-041.`

**Extracted:** `Ms. De Vries reviewed case NL-2026-041.`

**Accepted:** `Ms. De Vries reviewed case NL-2026-041.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-19

**Input:** `The API returns HTTP 404 when the cache is empty.`

**Reference-first assessment:** Correct technical statement. API, HTTP 404 and cache are legitimate; technical truth is not a spelling or grammar label.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required.

**Regional conventions:** Valid British and American prose.

**Ambiguity / priority human review:** Application behavior is outside the linguistic assessment.

### ministral-3b · B052

**Raw:** `The API returns HTTP 404 when the cache is empty.`

**Extracted:** `The API returns HTTP 404 when the cache is empty.`

**Accepted:** `The API returns HTTP 404 when the cache is empty.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B065

**Raw:** `The API returns HTTP 404 when the cache is empty.`

**Extracted:** `The API returns HTTP 404 when the cache is empty.`

**Accepted:** `The API returns HTTP 404 when the cache is empty.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B091

**Raw:** `The API returns HTTP 404 when the cache is empty.`

**Extracted:** `The API returns HTTP 404 when the cache is empty.`

**Accepted:** `The API returns HTTP 404 when the cache is empty.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-20

**Input:** `The customer backup is at https://example.test/report?id=42.`

**Reference-first assessment:** Correct noun adjunct customer backup and locative URL. A possessive customer's is not required: the compound can label a backup type.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** Do not add possessive morphology or alter the URL without context.

**Regional conventions:** backup is a valid noun spelling; no regional normalization required.

**Ambiguity / priority human review:** Customer could denote ownership or a category, both compatible with this correct noun compound.

### ministral-3b · B115

**Raw:** `The customer backup is at [https://example.test/report?id=42](https://example.test/report?id=42).`

**Extracted:** `The customer backup is at [https://example.test/report?id=42](https://example.test/report?id=42).`

**Accepted:** `The customer backup is at https://example.test/report?id=42.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Same duplicated Markdown URL transformation as B110. Rejected; accepted text unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a symbol or emoji from the source..

### ministral-3-8b · B110

**Raw:** `The customer backup is at [https://example.test/report?id=42](https://example.test/report?id=42).`

**Extracted:** `The customer backup is at [https://example.test/report?id=42](https://example.test/report?id=42).`

**Accepted:** `The customer backup is at https://example.test/report?id=42.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** Plain-text URL is duplicated into Markdown, as in the Dutch URL cases. Rejected by the validator; accepted text unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a symbol or emoji from the source..

### ministral-3-14b · B120

**Raw:** `The customer backup is at https://example.test/report?id=42.`

**Extracted:** `The customer backup is at https://example.test/report?id=42.`

**Accepted:** `The customer backup is at https://example.test/report?id=42.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-21

**Input:** `This service uses OAuth2 and PostgreSQL.`

**Reference-first assessment:** Correct English with OAuth2 and PostgreSQL. Preserve technical identifiers exactly.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required.

**Regional conventions:** Technical terminology remains invariant.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B111

**Raw:** `This service uses OAuth2 and PostgreSQL.`

**Extracted:** `This service uses OAuth2 and PostgreSQL.`

**Accepted:** `This service uses OAuth2 and PostgreSQL.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B070

**Raw:** `This service uses OAuth 2.0 and PostgreSQL.`

**Extracted:** `This service uses OAuth 2.0 and PostgreSQL.`

**Accepted:** `This service uses OAuth2 and PostgreSQL.`

**Judgments:** raw — Protected-content violation; extracted — Protected-content violation; accepted — Correct minimal correction (correct preservation).

**Reason:** OAuth2 becomes OAuth 2.0, changing a protected technical identifier and numeric form. Rejected; accepted text unchanged. Accepted layer: No edit needed; original clean text preserved.

**Pipeline:** Rejected at validation: Correction output changes a number, URL, or protected identifier..

### ministral-3-14b · B042

**Raw:** `This service uses OAuth2 and PostgreSQL.`

**Extracted:** `This service uses OAuth2 and PostgreSQL.`

**Accepted:** `This service uses OAuth2 and PostgreSQL.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

## en-22

**Input:** `Team Atlas approved the change on May 3, 2025.`

**Reference-first assessment:** Correct simple past with a definite date and team name. Preserve Team Atlas, approval and May 3, 2025.

**Existing annotations (UTF-16 start/length):**

- No errors; preserve the input.

**Proposal:** Retain existing annotation.

**Other alternatives and limits:** None required; changing tense, voice or date format is unsolicited.

**Regional conventions:** US date format is valid; preserve it.

**Ambiguity / priority human review:** No special context ambiguity identified; independent review still pending.

### ministral-3b · B096

**Raw:** `Team Atlas approved the change on May 3, 2025.`

**Extracted:** `Team Atlas approved the change on May 3, 2025.`

**Accepted:** `Team Atlas approved the change on May 3, 2025.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-8b · B095

**Raw:** `Team Atlas approved the change on May 3, 2025.`

**Extracted:** `Team Atlas approved the change on May 3, 2025.`

**Accepted:** `Team Atlas approved the change on May 3, 2025.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

### ministral-3-14b · B061

**Raw:** `Team Atlas approved the change on May 3, 2025.`

**Extracted:** `Team Atlas approved the change on May 3, 2025.`

**Accepted:** `Team Atlas approved the change on May 3, 2025.`

**Judgments:** raw — Correct minimal correction; extracted — Correct minimal correction; accepted — Correct minimal correction (correct preservation).

**Reason:** No edit needed; original clean text preserved.

**Pipeline:** No rejection; accepted edits: 0.

