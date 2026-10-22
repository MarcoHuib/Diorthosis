# External GPT-6 versus preparer AI



Both are non-independent AI evidence. No judgment is changed. Null-to-known differences are coverage changes, not known contradictions. Source/candidate identity and both literal rationales follow. Full matched records (including agreements) are in preparer-comparison.jsonl.



## confirmation-bound / case-001 / rejected

Subject: `review-063ba87c4c3ff9fb8b751ba368c848574bf464593bfb038ad7f26bcee6c551e7`

Source: "Hi folks,\nThere's fresh coffee in the kitchen. Help yourselves."

Candidate: "Hi folks,\n\nThere’s fresh coffee in the kitchen. Help yourselves."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "noNewErrors": {"preparer": false, "external": true}}; per-error: {}

Preparer rationale: Unnecessary paragraph and apostrophe changes are withheld; source text is the correct fallback.

External rationale: There is no error in the source. Inserting a blank line after “Hi folks,” and replacing ASCII apostrophe in “There's” with typographic U+2019 “There’s” are permissible email-style variants, not needed linguistic repairs. No proposition changes and the contraction remains correct; precise line-break and character preservation is lost. The candidate was rejected and was NOT delivered; this correct rejection avoids unnecessary changes.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-001 / parsed

Subject: `review-d223511a146c7d66fc2f9c3167e6e807e96c7c5ba778f6ce9f1859247e6547a5`

Source: "Hi folks,\nThere's fresh coffee in the kitchen. Help yourselves."

Candidate: "Hi folks,\n\nThere’s fresh coffee in the kitchen. Help yourselves."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "noNewErrors": {"preparer": false, "external": true}}; per-error: {}

Preparer rationale: Unnecessary paragraph and apostrophe changes are withheld; source text is the correct fallback.

External rationale: There is no error in the source. Inserting a blank line after “Hi folks,” and replacing ASCII apostrophe in “There's” with typographic U+2019 “There’s” are permissible email-style variants, not needed linguistic repairs. No proposition changes and the contraction remains correct; precise line-break and character preservation is lost. This is extracted/parsed output, not proof of delivery.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-001 / delivered

Subject: `review-f9125610eef53f1b7b00bb4d6113fc49306b9b35250eed0c425708ab56e028e4`

Source: "Hi folks,\nThere's fresh coffee in the kitchen. Help yourselves."

Candidate: "Hi folks,\nThere's fresh coffee in the kitchen. Help yourselves."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: The clean original greeting is carried forward exactly only because the proposed variant was rejected. It has no linguistic error, but a source fallback must not be scored as a successful correction or eligible clean preservation. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-011 / annotations

Subject: `review-8da1cfff81b6589687a37cc6b4a72e17bb5ca1aac65d64cf378314b170485ac6`

Source: "Neem een regen jas mee voor de wandeling over de dijk."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: The original Dutch expression “regen jas” incorrectly splits the compound noun “regenjas”. Dutch compounds with “regen-” are written together; the remainder of the sentence is correct. This subject evaluates the ORIGINAL only, before candidate comparison. Provisional source annotations are not assumed authoritative.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-015 / annotations

Subject: `review-b88c9fcb6caa6584842c368d90a600a0a04923c3bc27d89443a2096eb4c239ae`

Source: "We queued outside the chemist's while the rain soaked our trousers."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: The English original is idiomatic British English: “queued”, “chemist’s” (pharmacy) and “trousers” are normal forms, and the past tense “soaked” is correct. No spelling, grammar or regional-language error is present. This subject evaluates the ORIGINAL only, before candidate comparison. Provisional source annotations are not assumed authoritative.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-015 / parsed

Subject: `review-eaa8171e9d415aa153323024c1ac36ddef846a46299b413bfacd8f00d14215e4`

Source: "We queued outside the chemist's while the rain soaked our trousers."

Candidate: "We queued outside the chemist’s while the rain soaked our trousers."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": false, "external": null}, "justified": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: A clean British sentence receives only an unnecessary curly-apostrophe style change. No new grammar error or semantic change is claimed.

External rationale: The English original is idiomatic British English: “queued”, “chemist’s” (pharmacy) and “trousers” are normal forms, and the past tense “soaked” is correct. No spelling, grammar or regional-language error is present. The only change is the straight ASCII apostrophe in “chemist's” becoming U+2019 RIGHT SINGLE QUOTATION MARK in “chemist’s”. The British English vocabulary, possessive form, tense and meaning stay correct; there is no actual source defect to fix. Textual/code-point fidelity changes. Status: parsed candidate; do not count this as an independent model observation or as delivered output.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-015 / delivered

Subject: `review-a7983c408163e634dfd407e46013f91e0bfda736371cdd7a6a3fe1ebf6725b43`

Source: "We queued outside the chemist's while the rain soaked our trousers."

Candidate: "We queued outside the chemist’s while the rain soaked our trousers."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": false, "external": null}, "justified": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: A clean British sentence receives only an unnecessary curly-apostrophe style change. No new grammar error or semantic change is claimed.

External rationale: The English original is idiomatic British English: “queued”, “chemist’s” (pharmacy) and “trousers” are normal forms, and the past tense “soaked” is correct. No spelling, grammar or regional-language error is present. The only change is the straight ASCII apostrophe in “chemist's” becoming U+2019 RIGHT SINGLE QUOTATION MARK in “chemist’s”. The British English vocabulary, possessive form, tense and meaning stay correct; there is no actual source defect to fix. Textual/code-point fidelity changes. Status: delivered candidate, eligible for separate delivery-level assessment.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-017 / annotations

Subject: `review-75937da7a49e510e1f856e3c9b8785b1ec35373200d9bb4847fd4ef4e14e0055`

Source: "De postbode vermoed dat de bel kapot is."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: The Dutch source has a finite-verb agreement/spelling error: third-person singular present tense with subject “De postbode” requires “vermoedt”, not “vermoed” (stem “vermoed” + -t). The remaining words are correct. This subject evaluates the ORIGINAL only, before candidate comparison. Provisional source annotations are not assumed authoritative.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-024 / annotations

Subject: `review-1cffaab93604e5e4180721b5dc7cc4e7f4250f911da40ec7c2f673fe3153f3e0`

Source: "'s Ochtends is het op dit perron meestal rustig."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: OORSPRONKELIJKE TEKST, beoordeeld zonder kandidaat: De formulering ‘'s Ochtends is het op dit perron meestal rustig.’ is correct Nederlands: de afgekorte woordvorm begint met apostrof en kleine s, en ‘Ochtends’ krijgt aan het zinsbegin de hoofdletter. Geen correctie nodig. Dit onderwerp bevat zelf geen kandidaattekst; correctiescores zijn niet van toepassing en blijven null.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-024 / delivered

Subject: `review-1455d37e331f61ca55ced884f31fae42bf7a205abdba6b070654bd33527012aa`

Source: "'s Ochtends is het op dit perron meestal rustig."

Candidate: "'s Ochtends is het op dit perron meestal rustig."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": true, "external": false}, "justified": {"preparer": false, "external": true}, "rejectionAppropriate": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: De bron is correct behouden: de vaste tijdsaanduiding wordt geschreven als ‘'s Ochtends’, zonder spatie tussen apostrof en s en met hoofdletter O aan het zinsbegin. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende schadelijke kandidaatswijziging mocht terecht geweigerd worden.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-025 / annotations

Subject: `review-c2bef27b683e047e6d9bc05b02427aafd17d2ad0d58264e11a02bd9792fe703f`

Source: "Het bericht met `trace_id` is verzondenne, maar nog niet bevestigd."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: OORSPRONKELIJKE TEKST, beoordeeld zonder kandidaat: De bron bevat ‘verzondenne’, een niet-bestaande vervoegingsvorm; correct voltooid deelwoord is ‘verzonden’. De inline-codeaanduiding met backticks beschermt de technische identificatie `trace_id` en moet intact blijven. De bijzin ‘maar nog niet bevestigd’ is correct. Dit onderwerp bevat zelf geen kandidaattekst; correctiescores zijn niet van toepassing en blijven null.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-025 / parsed

Subject: `review-467dde559d8b44173994ca1284db2c8abca64dcdf46c7378f850aac900f610cd`

Source: "Het bericht met `trace_id` is verzondenne, maar nog niet bevestigd."

Candidate: "Het bericht met *trace_id* is verzonden, maar nog niet bevestigd."

Conditions: diorthosis-correction-v3

Differences: {"meaningPreserved": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: Local verzondenne typo is repaired, but backticked technical formatting is replaced by emphasis. Withholding the whole candidate is appropriate.

External rationale: ‘verzondenne’ wordt terecht ‘verzonden’; die lokale spellingreparatie is geslaagd. Maar backticks rond `trace_id` zijn vervangen door asterisken (*trace_id*): Markdown-inline-code verandert in nadruk en de presentatie/semantiek van de technische identifier gaat verloren. Niet geschikt als exacte documentcorrectie; lokale reparatie is niet gelijk aan volledige kwaliteit. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Interpretation: Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-025 / delivered

Subject: `review-6d9e3e0af46a00d811c5005a57dcf8bcacc4f3deab0a5d297592be91b3003189`

Source: "Het bericht met `trace_id` is verzondenne, maar nog niet bevestigd."

Candidate: "Het bericht met `trace_id` is verzondenne, maar nog niet bevestigd."

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}, "rejectionAppropriate": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: De fout ‘verzondenne’ staat nog letterlijk in de uitvoer. De technische identifier `trace_id` is wel intact gebleven. Geen nieuwe fout, maar ook geen spellingreparatie; als dit een terugval na afwijzing is, mag dit niet als succesvolle verbetering tellen. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende schadelijke kandidaatswijziging mocht terecht geweigerd worden.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-025 / rejected

Subject: `review-7980090d7f9fe268ac971bb7ddcc7a2d735393fff43064694c861d64a1b2aaea`

Source: "Het bericht met `trace_id` is verzondenne, maar nog niet bevestigd."

Candidate: "Het bericht met *trace_id* is verzonden, maar nog niet bevestigd."

Conditions: diorthosis-correction-v3

Differences: {"meaningPreserved": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: Local verzondenne typo is repaired, but backticked technical formatting is replaced by emphasis. Withholding the whole candidate is appropriate.

External rationale: ‘verzondenne’ wordt terecht ‘verzonden’; die lokale spellingreparatie is geslaagd. Maar backticks rond `trace_id` zijn vervangen door asterisken (*trace_id*): Markdown-inline-code verandert in nadruk en de presentatie/semantiek van de technische identifier gaat verloren. Niet geschikt als exacte documentcorrectie; lokale reparatie is niet gelijk aan volledige kwaliteit. Deze kandidaat is afgewezen en NIET geleverd. De afwijzing is taalkundig passend.

Interpretation: Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-042 / annotations

Subject: `review-55c236f201b50323943655978cbbe23bd49d2ea4a5395d40a36eb11cee736ab1`

Source: "Het is naief om te denken dat iedereen dezelfde voorkeur heeft."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Beoordeling van de oorspronkelijke tekst, zonder kandidaat. Het Nederlandse bijvoeglijk naamwoord is «naïef», met trema op de i om de klinkers afzonderlijk uit te spreken. «naief» is een spelfout. Voorlopige annotaties zijn hiermee onafhankelijk getoetst; deze rij vertegenwoordigt geen geleverde reparatie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-043 / annotations

Subject: `review-eff0b6fb533077597ecbbdfb46e6a7aaf88c3d549eaa5c93509aee9639517c00`

Source: "We haven't saw that warning before."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Beoordeling van de oorspronkelijke tekst, zonder kandidaat. Na «have not / haven’t» vereist de present perfect het voltooid deelwoord «seen», niet de verleden tijd «saw». Een rechte en typografische apostrof zijn allebei bruikbaar. Voorlopige annotaties zijn hiermee onafhankelijk getoetst; deze rij vertegenwoordigt geen geleverde reparatie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-048 / annotations

Subject: `review-3b399e437ad09f7bee102e6f3cf120f4453a525eac0c92d94179e8570b38d45d`

Source: "There was a unusual smell near the boiler."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Beoordeling van de oorspronkelijke tekst, zonder kandidaat. Het Engelse onbepaalde lidwoord is «an» vóór de klinkerklank van «unusual» (/ʌ/). «a unusual» in de bron is fout. Voorlopige annotaties zijn hiermee onafhankelijk getoetst; deze rij vertegenwoordigt geen geleverde reparatie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-050 / annotations

Subject: `review-d747220e5ac596606055a244c864b645ea849d82f2963ef446d4f13488ab4e6b`

Source: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Beoordeling van de oorspronkelijke tekst, zonder kandidaat. De oorspronkelijke Nederlandse zin is correct. «hüttenkäse» is de correcte naam van een voedingsmiddel; de umlaut en overige letters moeten behouden blijven, temeer in de vraag van een patiënt aan een diëtist. Voorlopige annotaties zijn hiermee onafhankelijk getoetst; deze rij vertegenwoordigt geen geleverde reparatie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-050 / parsed

Subject: `review-99e19c9fa602b80a38cc258d0de3a8d5d80fccbc5b6009bf03c11828d4aff24e`

Source: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Candidate: "De patiënt vroeg de diëtist of hijttenkäse op het menu mocht blijven."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct hüttenkäse in a dietary sentence is replaced by malformed text and accepted. This is a critical lexical/meaning failure in the preparer AI judgment; not a human-confirmed event rate.

External rationale: «hüttenkäse» is onterecht vervangen door «hijttenkäse», geen correcte benaming voor het voedingsmiddel. Het benoemde product wordt vervormd en er ontstaat een nieuwe spelfout in een verder correcte zin. Dit is geen acceptabele Nederlandse spellingvariant. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-050 / parsed

Subject: `review-71974bb30d008547e39538549a5bebc22cc2a2ab4d18903d585e2ad99da103f3`

Source: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Candidate: "De patiënt vroeg de diëtist of huitkase op het menu mocht blijven."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct hüttenkäse in a dietary sentence is replaced by malformed text and accepted. This is a critical lexical/meaning failure in the preparer AI judgment; not a human-confirmed event rate.

External rationale: De correcte voedingsmiddelnaam «hüttenkäse» is vervangen door het niet-bestaande «huitkase». Hierdoor gaat de precieze referentie verloren in een gesprek tussen patiënt en diëtist. De bron was foutloos: een onnodige en materieel onjuiste wijziging. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-050 / delivered

Subject: `review-a55e77b15c1b7ed78c37e862f8b8e71c4629ff25d553014783a13b212889ac60`

Source: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Candidate: "De patiënt vroeg de diëtist of hijttenkäse op het menu mocht blijven."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct hüttenkäse in a dietary sentence is replaced by malformed text and accepted. This is a critical lexical/meaning failure in the preparer AI judgment; not a human-confirmed event rate.

External rationale: «hüttenkäse» is onterecht vervangen door «hijttenkäse», geen correcte benaming voor het voedingsmiddel. Het benoemde product wordt vervormd en er ontstaat een nieuwe spelfout in een verder correcte zin. Dit is geen acceptabele Nederlandse spellingvariant. Deze tekst heeft status geleverd, zodat eventuele gebreken daadwerkelijk in de opgeleverde passage staan.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-050 / delivered

Subject: `review-48018dfd8c76f6fd6bf293019fa6e0ecb7b3ae6130d83e375b56770c6674d55e`

Source: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Candidate: "De patiënt vroeg de diëtist of huitkase op het menu mocht blijven."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct hüttenkäse in a dietary sentence is replaced by malformed text and accepted. This is a critical lexical/meaning failure in the preparer AI judgment; not a human-confirmed event rate.

External rationale: De correcte voedingsmiddelnaam «hüttenkäse» is vervangen door het niet-bestaande «huitkase». Hierdoor gaat de precieze referentie verloren in een gesprek tussen patiënt en diëtist. De bron was foutloos: een onnodige en materieel onjuiste wijziging. Deze tekst heeft status geleverd, zodat eventuele gebreken daadwerkelijk in de opgeleverde passage staan.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-051 / annotations

Subject: `review-02ff9ea04b59363b216b29e3f753cdd577c6882695b884c2e572b10f67931af9`

Source: "Please add the collection date to the shared calnedar."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: De oorspronkelijke Engelse gebiedende zin is verder idiomatisch, maar “calnedar” is een letterverwisseling voor “calendar” (spelling). “Collection date” en “shared” passen grammaticaal bij de bedoelde agenda; er is geen extra bronfout vastgesteld. Dit annotations-subject bevat geen modelkandidaat; kandidaatoordelen, reparatie, behoud en afwijzing zijn niet van toepassing en blijven null.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-051 / parsed

Subject: `review-9303da3e39d106f5ee6570f5feb0106534b7097edb936b45cbb0083f3d8e5625`

Source: "Please add the collection date to the shared calnedar."

Candidate: "Please add the collection date to the shared calendar."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: “calnedar” → “calendar” herstelt de verwisselde letters in het Engelse zelfstandig naamwoord. De opdracht, “collection date”, gedeelde agenda en leestekens zijn behouden. Dit is een afzonderlijk beoordeelde parsed-kandidaat, niet op zichzelf een geleverde reparatie.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-051 / delivered

Subject: `review-0f0a2deaca0daf1d56b68074980e94e382ca6698dbf920fe9fd3993a61de5dd4`

Source: "Please add the collection date to the shared calnedar."

Candidate: "Please add the collection date to the shared calendar."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: “calnedar” → “calendar” herstelt de verwisselde letters in het Engelse zelfstandig naamwoord. De opdracht, “collection date”, gedeelde agenda en leestekens zijn behouden. Deze kandidaat is daadwerkelijk afgeleverd.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-058 / annotations

Subject: `review-d7f6479ca67db5da45e5901c03975d89faa86ebe45e4e0a9643bfd15f2859d49`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: De Engelse bron is correct. “Résumé” is een gangbare spelling; de toponiemen “Curaçao” (met ç) en “São Paulo” (met ã) zijn juist. Namen en diakritische tekens zijn betekenisdragend en mogen niet stilzwijgend worden aangepast. Geen bronfout. Dit annotations-subject bevat geen modelkandidaat; kandidaatoordelen, reparatie, behoud en afwijzing zijn niet van toepassing en blijven null.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-058 / delivered

Subject: `review-5212502934fef4adb78bc60538cd4d5fe8921bddf3dcdd944c18ec14af295630`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: "The résumé mentions work in Curaçao and São Paulo."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": false}, "justified": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: De originele zin met “résumé”, “Curaçao” en “São Paulo” is correct en onveranderd als source fallback na afwijzing van een andere kandidaat teruggegeven. Dit is een abstention, geen succesvolle verbetering en geen eligible clean preservation. De afgewezen naamvervorming is apart beoordeeld.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-058 / delivered

Subject: `review-742d025d74c1924586d183b260203e7a3b856ba5148276b6aa971a8b6bc475f0`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: "The résumé mentions work in Curaçao and São Paulo."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De correcte Engelse zin is exact behouden, inclusief de spelling en accenten van “résumé”, “Curaçao” en “São Paulo”. Bij een foutloze bron is dit de gepaste uitkomst; er zijn geen fouten te repareren. Deze ongewijzigde correcte versie was daadwerkelijk delivery-eligible en afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-058 / rejected

Subject: `review-30faceee19fc8f1a89f9d35a753ae7919a7e68f6959447349ae7479edd5e85c7`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: "The résumé mentions work in Curauçao and São Paulo."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct Curaçao is corrupted to Curauçao. The proper-name error is correctly rejected.

External rationale: De correcte plaatsnaam “Curaçao” is ongeoorloofd veranderd in “Curauçao” met een extra u vóór ç. Dit creëert een onjuiste geografische eigennaam en schaadt de referentie aan de bedoelde plaats; “résumé” en “São Paulo” zijn niet fout. Het is geen alternatieve spelling en geen bronreparatie. Deze kandidaat is terecht geweigerd; weigering is geen afgeleverde verbetering.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-058 / parsed

Subject: `review-341e9d8bd1265a200356b87f66b2c7626f518bab4bf7397cea591281b5776d28`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: "The résumé mentions work in Curaçao and São Paulo."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De correcte Engelse zin is exact behouden, inclusief de spelling en accenten van “résumé”, “Curaçao” en “São Paulo”. Bij een foutloze bron is dit de gepaste uitkomst; er zijn geen fouten te repareren. Dit is een afzonderlijke parsed-observatie, geen bewijs van eigen delivery.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-058 / parsed

Subject: `review-839ea0c0b037ce8e0b6da9252c84864f19c29968d83758ceedee994a6fd1bc5b`

Source: "The résumé mentions work in Curaçao and São Paulo."

Candidate: "The résumé mentions work in Curauçao and São Paulo."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Correct Curaçao is corrupted to Curauçao. The proper-name error is correctly rejected.

External rationale: De correcte plaatsnaam “Curaçao” is ongeoorloofd veranderd in “Curauçao” met een extra u vóór ç. Dit creëert een onjuiste geografische eigennaam en schaadt de referentie aan de bedoelde plaats; “résumé” en “São Paulo” zijn niet fout. Het is geen alternatieve spelling en geen bronreparatie. Deze parsed-kandidaat mag niet worden afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-063 / annotations

Subject: `review-138feca9af9f4a19a78d11d2260596bc61d892084cc3759a3c33918870b0c4f1`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Onafhankelijke beoordeling van het origineel: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. Annotaties zijn voorlopige labels; deze regel beoordeelt alleen de bron, niet een modelcorrectie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-063 / delivered

Subject: `review-39ae988b77609b2ea7f1edd0186f6dd352cc9b7cd4f84faf450ad41ca9304105`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": false}, "justified": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. Dit item levert exact de bron terug NA abstentie (status „source fallback after abstention”, dispositie rejected). Niets is verbeterd, en dit mag niet als geslaagde of voor clean-preservation meetellende modelcorrectie worden gewaardeerd. Geen nieuwe tekstfout in de identieke fallback.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-063 / delivered

Subject: `review-51b5acf67757f888688a8e01db40f777a7afb941822703de16609cff9825c875`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. Alle backticks, identifiers, puntkomma, ontkenning en instructie blijven exact behouden. Terecht geen correctie aangebracht.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-063 / rejected

Subject: `review-e49b43b35f382fc26a98d3eb1fc5727c00db9cdb75bc97db20bc1e6b5a24aebe`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: "client_secret blijft geheim; deel alleen public_key met de ontvanger."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Backticks are removed from two technical identifiers in a clean source. Existing guard correctly withholds formatting damage.

External rationale: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. De kandidaat verwijdert ongevraagd de backticks om zowel `client_secret` als `public_key`. Deze code-span-markering onderscheidt technische identifiers van de lopende tekst; verlies is een opmaak-/fidelity-regressie, zonder wijziging van de letterlijke identifiers of de instructie om de secret geheim te houden. De expliciete afwijzing is daarom passend; de afgewezen kandidaat is geen succesvolle verbetering.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-063 / parsed

Subject: `review-30766c5152c67414ab3931cb76f3f83ac3f1d53b797f69d3b564c2adc2041704`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. Alle backticks, identifiers, puntkomma, ontkenning en instructie blijven exact behouden. Terecht geen correctie aangebracht.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-063 / parsed

Subject: `review-961599d934a68c3c18c790568fec15e4d0853ae7797e25df7e0b2d169014f3cf`

Source: "`client_secret` blijft geheim; deel alleen `public_key` met de ontvanger."

Candidate: "client_secret blijft geheim; deel alleen public_key met de ontvanger."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Backticks are removed from two technical identifiers in a clean source. Existing guard correctly withholds formatting damage.

External rationale: De brontekst is taalkundig correct. De puntkomma scheidt twee nauw verbonden hoofdzinnen, de imperatief „deel” is correct en de backticks markeren de technische identifiers `client_secret` en `public_key`. De waarschuwing over geheimhouding is helder. De kandidaat verwijdert ongevraagd de backticks om zowel `client_secret` als `public_key`. Deze code-span-markering onderscheidt technische identifiers van de lopende tekst; verlies is een opmaak-/fidelity-regressie, zonder wijziging van de letterlijke identifiers of de instructie om de secret geheim te houden. Deze raw/parsed-kandidaat is op zichzelf niet gerechtvaardigd, ongeacht of hij later wordt afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-065 / annotations

Subject: `review-c6293011eb0366f91536e8ec39f55e936893baf8bdbd6b0889f8c0eee9a75a75`

Source: "Ik kreeg een berciht over de opening van de nieuwe winkel."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Onafhankelijke beoordeling van het origineel: „berciht” bevat een letterverwisseling en moet „bericht” zijn. De overige Nederlandse zin is correct; „een bericht over de opening” is gangbaar. Annotaties zijn voorlopige labels; deze regel beoordeelt alleen de bron, niet een modelcorrectie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-071 / annotations

Subject: `review-40f41c7a5685b9c514f2ce2ef880b34f08743e8d985ae0be7eb9e541270963cd`

Source: "Zonder jouw toestemming verstuur ik de brief niet, ook niet als de termijn verstrijkt."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original source assessment only; no candidate exists in this subject. The original is correct Dutch. "Zonder jouw toestemming ... niet" is a negative condition, and "ook niet als de termijn verstrijkt" preserves the refusal even after expiry. No source error or awkward double negation. No correction, rejection or delivery is inferred from source annotation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-071 / parsed

Subject: `review-8f8030062edae65be7c98d14bbdce8d60c7458488ab1c8c6bf4db0afac0960e8`

Source: "Zonder jouw toestemming verstuur ik de brief niet, ook niet als de termijn verstrijkt."

Candidate: "Zonder jouw toestemming verstuur ik de brief niet, ook niet als de termijn verstrijkt."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: The source is already correct Dutch; not changing "Zonder jouw toestemming ... niet, ook niet als de termijn verstrijkt" preserves the precise no-consent and deadline conditions. There was no grammatical or spelling defect to repair. This parsed output is an unchanged preservation, not an error correction.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-071 / delivered

Subject: `review-4c4974b3d62f39cade212aaae55fa48dd043381125613749f04acb820dfcd5e2`

Source: "Zonder jouw toestemming verstuur ik de brief niet, ook niet als de termijn verstrijkt."

Candidate: "Zonder jouw toestemming verstuur ik de brief niet, ook niet als de termijn verstrijkt."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: The source is already correct Dutch; not changing "Zonder jouw toestemming ... niet, ook niet als de termijn verstrijkt" preserves the precise no-consent and deadline conditions. There was no grammatical or spelling defect to repair. This delivered output is an unchanged preservation, not an error correction.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-072 / annotations

Subject: `review-2aab0bfea1046e5c933a95a4e030c47a815e0a50c7dd4dba847516e167c20aa3`

Source: "The price does not includes delivery to the island."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original source assessment only; no candidate exists in this subject. The original has one subject–auxiliary verb error: after "does not", English requires the bare infinitive "include", not "includes". The negative meaning (delivery to the island is not included) is otherwise clear. No correction, rejection or delivery is inferred from source annotation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-076 / annotations

Subject: `review-70e5efbbeeba4197c6a3a324f9af2872e3ebeebaddeabb714c03e27a66c6484a`

Source: "They've said they won't need another copy, haven't they?"

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original source assessment only; no candidate exists in this subject. The original is grammatical English: "They've said" supports the auxiliary-based tag "haven't they?"; the embedded "won't" does not force a different tag. Straight apostrophes are valid. No source error. No correction, rejection or delivery is inferred from source annotation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-076 / delivered

Subject: `review-0f697bfe916053738a9305317d75940ca14fd6a44e0887fb7c7f25b79de98652`

Source: "They've said they won't need another copy, haven't they?"

Candidate: "They've said they won't need another copy, haven't they?"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": null}, "justified": {"preparer": false, "external": null}, "rejectionAppropriate": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: The delivered-layer text exactly repeats the already-correct source because of source fallback after an abstention, not a successful correction or a qualifying clean-preservation model output. The original tag question "haven't they?" correctly agrees with matrix "They've said". English contractions written with straight apostrophes are valid. The rejected typography-only candidate was unnecessary; abstention was appropriate.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-076 / parsed

Subject: `review-2fd10f9e2ae3277cb80df42d73365ece9b40b52fed25aba067637d48e5c7fc2f`

Source: "They've said they won't need another copy, haven't they?"

Candidate: "They’ve said they won’t need another copy, haven’t they?"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": false, "external": true}}; per-error: {}

Preparer rationale: Only correct contractions are restyled. Withholding is appropriate because source needed no correction; no valid necessary repair is lost.

External rationale: The original tag question "haven't they?" correctly agrees with matrix "They've said". English contractions written with straight apostrophes are valid. The candidate only changes three straight ASCII apostrophes to typographic right single quotation marks (U+2019). This glyph normalization is typographically acceptable and preserves meaning, but fixes no source error. This parsed output is not a substantive linguistic improvement or a delivered repair.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-076 / rejected

Subject: `review-483e8731921fde039db18e76681ba91793f8130eb59796321057ab9aaa44e48d`

Source: "They've said they won't need another copy, haven't they?"

Candidate: "They’ve said they won’t need another copy, haven’t they?"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Only correct contractions are restyled. Withholding is appropriate because source needed no correction; no valid necessary repair is lost.

External rationale: The original tag question "haven't they?" correctly agrees with matrix "They've said". English contractions written with straight apostrophes are valid. The candidate only changes three straight ASCII apostrophes to typographic right single quotation marks (U+2019). This glyph normalization is typographically acceptable and preserves meaning, but fixes no source error. This rejected candidate should stay rejected; no improvement was delivered.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-077 / annotations

Subject: `review-e74645d708bcc4f60c292cbc3eab06a8f03c08c39e9c49978e498198d87615de`

Source: "Dat plan is tijdens de lunch voorgestelt."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original source assessment only; no candidate exists in this subject. The original contains a past-participle spelling error: Dutch "voorstellen" has the participle "voorgesteld" (final d), not "voorgestelt". The rest is correct. No correction, rejection or delivery is inferred from source annotation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-081 / annotations

Subject: `review-dd0f28893561117b6f8509836ec2d52e2412aecaab9b17e893f1ce0f178ed2cc`

Source: "De brandweer heeft de kat uit de boom geredden."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: ORIGINAL SOURCE ASSESSMENT. The source has an incorrectly inflected Dutch past participle: after "heeft", the rescue verb is "gered", not "geredden". The singular/plural status of "brandweer" and the remaining words are correct. The frozen annotation list describes 1 source error(s); this source-only subject has no candidate and cannot be scored as a repair.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-082 / annotations

Subject: `review-46f7ea6eb9a197d96c1312add2be8714f1a79910e0546811438e6fd877327042`

Source: "She insisted to paying for the repairs herself."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: ORIGINAL SOURCE ASSESSMENT. After English "insist" followed by a gerund, use "on paying", not "to paying". The agent "she", the self-payment and the repairs must remain. The frozen annotation list describes 1 source error(s); this source-only subject has no candidate and cannot be scored as a repair.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-085 / annotations

Subject: `review-8c4fbd39c0a0353464fd06c2e1e7480715066c3d173a5209f29b8448c673bdf4`

Source: "The `session_token` contain no personal details."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: ORIGINAL SOURCE ASSESSMENT. English third-person singular "The `session_token`" requires "contains" rather than "contain". The backticked literal `session_token`, including the underscore, is a technical identifier and must not be rewritten as the ordinary words "session token". The frozen annotation list describes 1 source error(s); this source-only subject has no candidate and cannot be scored as a repair.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-085 / delivered

Subject: `review-0329c70287eae8a35c715fc6e6478eaa661e853cb7739eefb76167fd655b0da0`

Source: "The `session_token` contain no personal details."

Candidate: "The `session_token` contain no personal details."

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}, "justified": {"preparer": false, "external": true}, "rejectionAppropriate": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: Source fallback is an ABSTENTION, not successful repair. It preserves exact identifier `session_token` but also leaves third-person singular "contain" uncorrected; no clean preservation credit. Rejecting the separately observed unsafe "session token" candidate is appropriate, although this fallback still fails to fix the grammar.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-087 / annotations

Subject: `review-d0c0bd8cdca135927c181d30d6db44554b54f3c53c4adddd90af51ab400b5253`

Source: "We'll meet outside the café tommorrow morning."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: ORIGINAL SOURCE ASSESSMENT. "tommorrow" is misspelled; standard English requires "tomorrow". The source contraction "We’ll" written with a straight apostrophe is acceptable; "café" uses a decomposed combining accent, a valid Unicode representation of "café" and not itself a spelling defect. The frozen annotation list describes 1 source error(s); this source-only subject has no candidate and cannot be scored as a repair.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-087 / delivered

Subject: `review-588dab3c2aab9c717b13303e7f4e151b62a9b0dba926e050f5248dd14356eac5`

Source: "We'll meet outside the café tommorrow morning."

Candidate: "We'll meet outside the café tommorrow morning."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}, "rejectionAppropriate": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: This delivered record is SOURCE FALLBACK after abstention, so the misspelled "tommorrow" remains. It cannot be credited as a repaired output or eligible clean preservation. The separately rejected alternatives correctly spell "tomorrow" and preserve meaning, making their rejection linguistically unwarranted on the supplied evidence.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-102 / annotations

Subject: `review-0b6e05974dd84e09a717247de40925556cf2fe73d62eb42607168d3cf643f785`

Source: "In de gang moeten nog twee lamp worden vervangen."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original Dutch contains “twee lamp”: the cardinal “twee” requires the plural “lampen”. One source agreement/inflection defect. The annotation points to a genuine source issue, but it is provisional AI evidence rather than a human gold label; no candidate correction is proposed in this annotations-layer subject.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-102 / parsed

Subject: `review-3087896b2f44094d530b597bb626ecfdbd6cf32b53b260385c107b1628c677b7`

Source: "In de gang moeten nog twee lamp worden vervangen."

Candidate: "In de gang moeten nog twee lampen worden vervangen."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: “twee lamp” was an incorrect singular after numeral two; “twee lampen” supplies the required Dutch plural while retaining location, number, obligation, and timing. Assessed separately as parsed (output-1); deliveryEligible=false.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-102 / delivered

Subject: `review-b9a898e92735a7c39428ee283b208600c0b824a0e9a7143940eca47f57dc73c3`

Source: "In de gang moeten nog twee lamp worden vervangen."

Candidate: "In de gang moeten nog twee lampen worden vervangen."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: “twee lamp” was an incorrect singular after numeral two; “twee lampen” supplies the required Dutch plural while retaining location, number, obligation, and timing. Assessed separately as delivered (output-3); deliveryEligible=true.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-112 / annotations

Subject: `review-e532a4cea75e3def91444146a368d19d6e773062d9672059330908becd571b35`

Source: "In het café wachten we totdatd de regen ophoudt. ☔"

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Source-only assessment for case-112; no candidate exists in this subject. “totdatd” is misspelled; the conjunction is “totdat”. The decomposed e + U+0301 in “café” is canonically equivalent to “café”, not an independent spelling mistake. The rain emoji ☔ belongs to the original message and should not be replaced. Correction, rejection, and preservation outcome fields remain null rather than inventing an evaluation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-112 / delivered

Subject: `review-ad6acaa3b5a0cef9e8132eebb3a5b828a6049a4f255cf6b7f8ce51366c1e58a6`

Source: "In het café wachten we totdatd de regen ophoudt. ☔"

Candidate: "In het café wachten we totdatd de regen ophoudt. ☔"

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: delivered source fallback after abstention: “totdatd” remains, instead of “totdat”; decomposed “café” is not itself defective. A source fallback after abstention is not a successful repair or clean preservation.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-115 / annotations

Subject: `review-d7bff829e66a7432a7a2daa6352564de5ed96277a3bd85469efd0d492d3052d4`

Source: "Ik hoop dat je de instructies hebt kunnen lezen."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Source-only assessment for case-115; no candidate exists in this subject. Correct Dutch: “Ik hoop dat je de instructies hebt kunnen lezen.” has a natural subordinate-clause verb cluster (“hebt kunnen lezen”), sound grammar and punctuation. Leave unchanged. Correction, rejection, and preservation outcome fields remain null rather than inventing an evaluation.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-115 / parsed

Subject: `review-5cb1130b44d485bbc302eba65b769af96062ba893a8ef931aea34ee4d25cfd10`

Source: "Ik hoop dat je de instructies hebt kunnen lezen."

Candidate: "Ik hoop dat je de instructies hebt kunnen lezen."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: parsed/passage: no changes to an already correct text. The Dutch verb cluster “hebt kunnen lezen” is grammatical and conveys the hoped-for completed ability to read. No accidental meaning change or introduced error; no correction was needed. “allDefectsRepaired” is not applicable because the source has no identifiable defects.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-115 / delivered

Subject: `review-a59fdaf6b0344e33498fcf9c03ccc5e35e7018685fed1f7ed84447c7dd18aecf`

Source: "Ik hoop dat je de instructies hebt kunnen lezen."

Candidate: "Ik hoop dat je de instructies hebt kunnen lezen."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: delivered/passage: no changes to an already correct text. The Dutch verb cluster “hebt kunnen lezen” is grammatical and conveys the hoped-for completed ability to read. No accidental meaning change or introduced error; no correction was needed. “allDefectsRepaired” is not applicable because the source has no identifiable defects.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / annotations

Subject: `review-715a4aae594217799a6634aa48a571774eaaa72f9d41991150e6d8a7462d45f6`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Source-only assessment. The source misspells 'exhibition' as 'exhibiton'. 'Ömer' and 'Chloë' are proper names and must retain their exact spelling and diacritics; 'Chloé' is not an interchangeable normalization of 'Chloë'. The annotation is provisional AI evidence, not an independently verified truth.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / rejected

Subject: `review-e7cbd3712a04f3f1d3cbc85baac9d5d87f6280e19f65631b86d87ffabea47703`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: "The exhibition includes photographs by Ömer and Chloé."

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: The typo exhibition is repaired but the name Chloë becomes Chloé. Identity change is correctly rejected.

External rationale: The candidate repairs 'exhibiton' to 'exhibition' but improperly changes the person's name from 'Chloë' (ë) to 'Chloé' (é). These Unicode characters are distinct and the names cannot be treated as interchangeable. The alteration risks a different named person; a local spelling success does not make the passage acceptable. Rejection is appropriate because the candidate corrupts a proper name despite repairing the source typo.

Interpretation: Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / delivered

Subject: `review-98f3394140037ffd65510abfa340d08be586cd6e4e1ca8c16a76669946f93378`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: "The exhibiton includes photographs by Ömer and Chloë."

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: Source fallback retains the misspelling 'exhibiton'. Proper names remain intact, but the original spelling error was not repaired. This delivered fallback is an abstention, not a successful correction or rewarded clean preservation.

Interpretation: Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / parsed

Subject: `review-e4d6a7adc32a6d5b3e0d3d4a82dba50fdb9cc684b8a2b8a26d2a9c2372fe3e16`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: "The exhibition includes photographs by Ömer and Chloé."

Conditions: diorthosis-correction-v3

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: The typo exhibition is repaired but the name Chloë becomes Chloé. Identity change is correctly rejected.

External rationale: The candidate repairs 'exhibiton' to 'exhibition' but improperly changes the person's name from 'Chloë' (ë) to 'Chloé' (é). These Unicode characters are distinct and the names cannot be treated as interchangeable. The alteration risks a different named person; a local spelling success does not make the passage acceptable.

Interpretation: Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / parsed

Subject: `review-ef903d59c4e98ff8ac53441e9234139199ac39a1533c90932cf4b3967be81ad2`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: "The exhibition includes photographs by Ömer and Chloë."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Only 'exhibiton' is corrected to 'exhibition'. The names 'Ömer' and 'Chloë' retain their exact diacritics; the exhibition's participants remain unchanged.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-123 / delivered

Subject: `review-229ff678207a2db219daf63417cfcf05df54d1827cae820fee8d0b15554d82b4`

Source: "The exhibiton includes photographs by Ömer and Chloë."

Candidate: "The exhibition includes photographs by Ömer and Chloë."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Only 'exhibiton' is corrected to 'exhibition'. The names 'Ömer' and 'Chloë' retain their exact diacritics; the exhibition's participants remain unchanged.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-134 / annotations

Subject: `review-f420ae02845712de7771af2944016ec1071c032716f6a9ca2cdba4faee3a42c5`

Source: "Hoi allemaal,\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bronbeoordeling, zonder correctiekandidaat. De Nederlandse aanhef, komma, zinnen en afsluiting zijn correct. Een lege regel na de aanhef is een opmaakkeuze, geen verplichte taalcorrectie. Er zijn geen geannoteerde bronfouten; zelfstandig zijn ook geen taalfouten gevonden. Reparatie, betekenisbehoud en afwijzing zijn hier niet van toepassing.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-134 / parsed

Subject: `review-6901a8f75a59f56869541ca8b0b1740e73cb8969d000e53755f8c050e628cc48`

Source: "Hoi allemaal,\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Candidate: "Hoi allemaal,\n\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": false, "external": null}, "noNewErrors": {"preparer": false, "external": true}}; per-error: {}

Preparer rationale: An unrequested extra paragraph break is inserted; the cosmetic output is suppressed and original is returned.

External rationale: Fase parsed, status parsed; De oorspronkelijke Nederlandse zinnen zijn grammaticaal en orthografisch correct. De kandidaat voegt uitsluitend een extra regeleinde na “Hoi allemaal,” toe, waarmee de alinea-indeling verandert maar de boodschap niet. Dit is een acceptabele stijlvariant, niet bewezen noodzakelijk. Deze afwijkende alinea-indeling is geen vereiste correctie.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-134 / delivered

Subject: `review-e839f445eb5ed6b4c6ac3ff263702514262035324af673742bde66c5a0b941bc`

Source: "Hoi allemaal,\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Candidate: "Hoi allemaal,\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": false}, "rejectionAppropriate": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: Fase delivered, status source fallback after abstention; De correcte bron blijft behouden. De onderdrukte kandidaat voegde slechts een facultatieve lege regel toe. Bronfallback is geen afzonderlijke succesvolle correctie of eligible behoudsprestatie, ook al bevat de brontekst geen taalfouten.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-134 / rejected

Subject: `review-18c2db998440e3e16447b2ac08664437f2ee7b7477c8234c1b5fc6b61dd5dc9e`

Source: "Hoi allemaal,\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Candidate: "Hoi allemaal,\n\nDe sleutels liggen bij de buren. Veel plezier dit weekend!"

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": false, "external": null}, "noNewErrors": {"preparer": false, "external": true}}; per-error: {}

Preparer rationale: An unrequested extra paragraph break is inserted; the cosmetic output is suppressed and original is returned.

External rationale: Fase rejected, status rejected; De oorspronkelijke Nederlandse zinnen zijn grammaticaal en orthografisch correct. De kandidaat voegt uitsluitend een extra regeleinde na “Hoi allemaal,” toe, waarmee de alinea-indeling verandert maar de boodschap niet. Dit is een acceptabele stijlvariant, niet bewezen noodzakelijk. Onderdrukking van deze facultatieve wijziging is verdedigbaar; er is geen verbetering afgeleverd.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-136 / annotations

Subject: `review-35906ff7a630834071830305763149fe01c0f5ce0d7af591552d4284fbf18b17`

Source: "I have forgot where the spare batteries are kept."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bronbeoordeling, zonder correctiekandidaat. De present perfect vereist na have het voltooid deelwoord forgotten, niet de verleden tijd forgot. De voorlopige annotatie komt overeen met de zelfstandig vastgestelde fout. Reparatie, betekenisbehoud en afwijzing zijn hier niet van toepassing.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-136 / parsed

Subject: `review-c7806430d0e96f47693707e9b3ea70b40fb44ddc84ea9a71f8cea1daab4755ad`

Source: "I have forgot where the spare batteries are kept."

Candidate: "I have forgotten where the spare batteries are kept."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Fase parsed, status parsed; Bij de present perfect “have” hoort het voltooid deelwoord “forgotten”, niet “forgot”. De reparatie bewaart persoon, tijdsbetekenis en de vraag waar de batterijen worden bewaard; geen andere defecten of nieuwe fouten.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-136 / delivered

Subject: `review-432cf42ff7716b6dd0a238c08a0f9affcb9534bd4c0a6b5742a990397ca602fe`

Source: "I have forgot where the spare batteries are kept."

Candidate: "I have forgotten where the spare batteries are kept."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Fase delivered, status delivered; Bij de present perfect “have” hoort het voltooid deelwoord “forgotten”, niet “forgot”. De reparatie bewaart persoon, tijdsbetekenis en de vraag waar de batterijen worden bewaard; geen andere defecten of nieuwe fouten.

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-140 / annotations

Subject: `review-c461db01f0918ae1944714798c173a66de98bf61d5e4cf19b833dd6470243d4b`

Source: "Het opschrift luidt hola, bonjour en γεια σας."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bronbeoordeling, zonder correctiekandidaat. De Nederlandse verslagzin is correct en de meertalige begroetingen zijn letterlijke opschriftgegevens. Het Griekse γεια σας behoeft geen correctie; accent/diakritiek niet zonder bronbewijs aanpassen. Er zijn geen geannoteerde bronfouten; zelfstandig zijn ook geen taalfouten gevonden. Reparatie, betekenisbehoud en afwijzing zijn hier niet van toepassing.

Interpretation: Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling. Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-140 / delivered

Subject: `review-3b5f56e69841831b827ec650637ca87b7039ada663506a243e3637759c887cc5`

Source: "Het opschrift luidt hola, bonjour en γεια σας."

Candidate: "Het opschrift luidt hola, bonjour en γειά σας."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "meaningPreserved": {"preparer": true, "external": false}, "noNewErrors": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: An accent is added to the Greek greeting in a clean multilingual source. It is unnecessary. Greek orthographic validity is left unassessed rather than calling every Unicode difference corruption.

External rationale: Fase delivered, status delivered; De bron meldt letterlijk welk opschrift er staat: “hola, bonjour en γεια σας”. De kandidaat verandert zonder bewijs het Griekse woord van “γεια” in “γειά”; de begroeting blijft inhoudelijk herkenbaar, maar de letterlijke transcriptie klopt niet meer met de opgegeven bron. Geen Nederlands brondefect vraagt om de wijziging. Kritieke ernst is zonder zicht op het opschrift onzeker.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-140 / parsed

Subject: `review-b4ab6ade576f7587f6e4b9a6442665c8f9ae5b7d8848f0c0f8f2a7564f59face`

Source: "Het opschrift luidt hola, bonjour en γεια σας."

Candidate: "Het opschrift luidt hola, bonjour en γειά σας."

Conditions: diorthosis-correction-v3

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "meaningPreserved": {"preparer": true, "external": false}, "noNewErrors": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: An accent is added to the Greek greeting in a clean multilingual source. It is unnecessary. Greek orthographic validity is left unassessed rather than calling every Unicode difference corruption.

External rationale: Fase parsed, status parsed; De bron meldt letterlijk welk opschrift er staat: “hola, bonjour en γεια σας”. De kandidaat verandert zonder bewijs het Griekse woord van “γεια” in “γειά”; de begroeting blijft inhoudelijk herkenbaar, maar de letterlijke transcriptie klopt niet meer met de opgegeven bron. Geen Nederlands brondefect vraagt om de wijziging. Kritieke ernst is zonder zicht op het opschrift onzeker.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-140 / parsed

Subject: `review-55c549dad90544051a4873e7aa1d42058aa83d31442f968ddb8fc7fd6768ea6d`

Source: "Het opschrift luidt hola, bonjour en γεια σας."

Candidate: "Het opschrift luidt hola, bonjour en γεια σας."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Fase parsed, status parsed; De grammaticaal correcte Nederlandse zin en de letterlijke meertalige inhoud “hola, bonjour en γεια σας” zijn volledig en tekengetrouw behouden. Er is geen noodzaak de Griekse accentuering, de namen of Unicode-tekens te wijzigen.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-140 / delivered

Subject: `review-efd675cf51a559a4ea0ecc29eb612d529f8edf0f3afc6d054761693c3171e804`

Source: "Het opschrift luidt hola, bonjour en γεια σας."

Candidate: "Het opschrift luidt hola, bonjour en γεια σας."

Conditions: phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Fase delivered, status delivered; De grammaticaal correcte Nederlandse zin en de letterlijke meertalige inhoud “hola, bonjour en γεια σας” zijn volledig en tekengetrouw behouden. Er is geen noodzaak de Griekse accentuering, de namen of Unicode-tekens te wijzigen.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Literal multilingual quotation fidelity differs from general Greek spelling acceptability; preserve both assessments without a new ruling. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-147 / annotations

Subject: `review-433d1400cc4ded54ed7e1c69838be0cc79537956319ee88d3960afefda049756`

Source: "The park needs several bench near the pond."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: BRONBEOORDELING vóór kandidaatvergelijking: «Several» vereist hier een meervoudig zelfstandig naamwoord: «several benches» in plaats van «several bench». De plaatsbepaling «near the pond» en rest van de zin zijn correct. De voorlopige foutannotatie komt daarmee overeen.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-149 / annotations

Subject: `review-a6eb01be61cfb774cc55b719af4fab49a6e5505b88318c26801ff0ff51d76a4b`

Source: "De inhoud van beide enveloppen zijn zorgvuldig gecontroleerd."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: BRONBEOORDELING vóór kandidaatvergelijking: Het onderwerp is het enkelvoudige kernwoord «de inhoud», niet het meervoud «beide enveloppen» in de voorzetselbepaling. Dus «is zorgvuldig gecontroleerd» en niet «zijn ... gecontroleerd». De voorlopige foutannotatie komt daarmee overeen.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-155 / annotations

Subject: `review-e031fbb67c57b39a03e012ba6e70efb4753242219e61959c29617156add73793`

Source: "Well done! 🎈 The whole team is pleased with the result."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Original-text-only examination, performed without favoring any supplied correction: The source is correct idiomatic English: "Well done!" and "The whole team is pleased with the result." The balloon emoji conveys a celebratory element and is part of the content, not a spelling defect. No candidate exists for this subject; candidate-level judgments are not applicable.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-155 / delivered

Subject: `review-b5bde0d4695f0962609a17a5964b2d5b887937af1f937a347fb5405639f09639`

Source: "Well done! 🎈 The whole team is pleased with the result."

Candidate: "Well done! 🎈 The whole team is pleased with the result."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "cleanPreserved": {"preparer": true, "external": false}, "justified": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Original source returned after abstention. It preserves source content but is not an eligible correction or successful repair.

External rationale: Status explicitly says source fallback after abstention, not an accepted correction. The visible candidate equals the source verbatim. The original congratulatory balloon 🎈 and the two correct English sentences are unchanged. This fallback must not be credited as a model repair or independent clean preservation; no withheld rejected candidate can be linked definitively to this fallback by its subject ID alone.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Source fallback after abstention: differences in cleanPreserved/justified reflect applicability conventions; neither earns delivered success. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## confirmation-bound / case-155 / rejected

Subject: `review-2f896d56bcd136d72684630ba1387012d288c42d4ded94acfcddab01125fd96e`

Source: "Well done! 🎈 The whole team is pleased with the result."

Candidate: "Well done! 😊 The whole team is pleased with the result."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Balloon is replaced by a smile in already-correct text. Existing symbol validation correctly withholds it.

External rationale: The grammatical English sentences need no repair. Substituting a smiling face 😊 for the original celebratory balloon 🎈 changes an explicit symbol and its expressive connotation, albeit without changing the factual claim about the team. This is an unwarranted content alteration, not a spelling improvement. The candidate is explicitly REJECTED; withholding this unwanted change is appropriate, but no correction success is credited.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-155 / parsed

Subject: `review-64a9a286e3e96766815e30775a6726bdf672256d180532a0cb5bb3b5beaf6719`

Source: "Well done! 🎈 The whole team is pleased with the result."

Candidate: "Well done! 😊 The whole team is pleased with the result."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Balloon is replaced by a smile in already-correct text. Existing symbol validation correctly withholds it.

External rationale: The grammatical English sentences need no repair. Substituting a smiling face 😊 for the original celebratory balloon 🎈 changes an explicit symbol and its expressive connotation, albeit without changing the factual claim about the team. This is an unwarranted content alteration, not a spelling improvement. This is a parsed candidate, not a claim that the text was delivered.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=False, external=False. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-161 / annotations

Subject: `review-e2de06970ab328083847eaea0d2faff92a28b1570e3da16d8e2a83dc3e135351`

Source: "If the power go out, do not open the freezer."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bron eerst onafhankelijk beoordeeld. The singular third-person subject “the power” requires “goes” in an open conditional (“If the power goes out”). “Do not open the freezer” is a correctly formed negative imperative. Dit subject bevat uitsluitend de bron en voorlopige foutannotaties, geen te beoordelen kandidaat. Referentiecorrecties worden niet automatisch als bewijs beschouwd.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-161 / delivered

Subject: `review-76a4f1e50b919c1b1dd0cec8e5755647393f098e08385343a18fb246b735ea49`

Source: "If the power go out, do not open the freezer."

Candidate: "If the power goes out, do not open the freezer."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De bronfout “the power go out” is in deze kandidaat terecht verbeterd tot “the power goes out”: “power” is derde persoon enkelvoud. De instructie “do not open the freezer”, de voorwaarde en de interpunctie blijven onveranderd. De wijziging is correct zonder bijkomende fouten. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-161 / parsed

Subject: `review-b2de6e1617bbcebf58ca21434df69ec7a8e74062a1eec65da8ec4e2bcf6d6acb`

Source: "If the power go out, do not open the freezer."

Candidate: "If the power goes out, do not open the freezer."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"cleanPreserved": {"preparer": null, "external": false}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De bronfout “the power go out” is in deze kandidaat terecht verbeterd tot “the power goes out”: “power” is derde persoon enkelvoud. De instructie “do not open the freezer”, de voorwaarde en de interpunctie blijven onveranderd. De wijziging is correct zonder bijkomende fouten. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Interpretation: Strict passage conjunction before/after: preparer=True, external=True. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-162 / annotations

Subject: `review-b0dccfdeb348cf4855b74e31cbbc060aa79ac5ecc3b3dcb5c8cecbc3427e3538`

Source: "The alarm may be faulty; that does not mean the room is safe."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bron eerst onafhankelijk beoordeeld. The source is grammatical: “may” expresses possibility rather than certainty, and “does not mean the room is safe” does not assert safety. The semicolon correctly links independent clauses. Dit subject bevat uitsluitend de bron en voorlopige foutannotaties, geen te beoordelen kandidaat. Referentiecorrecties worden niet automatisch als bewijs beschouwd.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-162 / delivered

Subject: `review-e262218fdc22e0627f9a6e9a6224ac7c9d0f1ccc949b3581c4c0f3380a85b623`

Source: "The alarm may be faulty; that does not mean the room is safe."

Candidate: "The alarm may be faulty; that does not mean the room is safe."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Ongewijzigde, foutloze Engelse brontekst: “may be faulty” behoudt de onzekerheid en “does not mean the room is safe” houdt de ontkenning intact. Geen reparatie nodig, geen nieuwe fout en geen overbodige wijziging. [Beoordelingslaag: delivered; disposition: unchanged; afgeleverd-toegestaan: True].

Interpretation: Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-162 / parsed

Subject: `review-018e74d9a9d605151a21db8e3834bddb2d27e40049360b2d994e2ef5f5ce9ae4`

Source: "The alarm may be faulty; that does not mean the room is safe."

Candidate: "The alarm may be faulty; that does not mean the room is safe."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: Ongewijzigde, foutloze Engelse brontekst: “may be faulty” behoudt de onzekerheid en “does not mean the room is safe” houdt de ontkenning intact. Geen reparatie nodig, geen nieuwe fout en geen overbodige wijziging. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Interpretation: Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-164 / annotations

Subject: `review-2fb4d0e873f364dd851bbbd44b15bfd0c5d2c31565d89e96f1710584c284da82`

Source: "The elevator in our apartment building was repaired on Tuesday."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Bron eerst onafhankelijk beoordeeld. The source is idiomatic, grammatical US English; “elevator” and “apartment building” are appropriate and the past-tense passive “was repaired on Tuesday” is sound. Dit subject bevat uitsluitend de bron en voorlopige foutannotaties, geen te beoordelen kandidaat. Referentiecorrecties worden niet automatisch als bewijs beschouwd.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-164 / parsed

Subject: `review-ce96ddb7dc44575c9f25cec264bcb815baf46c02b7b7adf68ac57a58ce8f5fa6`

Source: "The elevator in our apartment building was repaired on Tuesday."

Candidate: "The elevator in our apartment building was repaired on Tuesday."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De correct gespelde Amerikaanse Engelse bron “The elevator in our apartment building was repaired on Tuesday.” bleef volledig ongewijzigd. De tijdsaanduiding, het passief en de bewonersrelatie blijven intact; een ingreep was niet nodig. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Interpretation: Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-164 / delivered

Subject: `review-30959689cc067ca814410f0a176cf29f4ecb0597a8c5810cddfa6fb340002b0e`

Source: "The elevator in our apartment building was repaired on Tuesday."

Candidate: "The elevator in our apartment building was repaired on Tuesday."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De correct gespelde Amerikaanse Engelse bron “The elevator in our apartment building was repaired on Tuesday.” bleef volledig ongewijzigd. De tijdsaanduiding, het passief en de bewonersrelatie blijven intact; een ingreep was niet nodig. [Beoordelingslaag: delivered; disposition: unchanged; afgeleverd-toegestaan: True].

Interpretation: Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-175 / annotations

Subject: `review-7d46ca0d810f60f1b55ee5795cdac25de358b5a66387d5b48ae5d342b6ab5ca4`

Source: "In de namiddag zetten we ons nog even op het terras."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Onafhankelijke bronbeoordeling, zonder modelkandidaat: De oorspronkelijke zin is correct in Belgisch-Nederlands: wederkerend ‘zich zetten’ betekent hier gaan zitten. ‘In de namiddag’ en ‘op het terras’ zijn aanvaardbaar; vervanging omwille van Nederlandse regionale voorkeur is niet nodig. Deze annotatieregel vertegenwoordigt geen geslaagde modelcorrectie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-175 / parsed

Subject: `review-a40c3b5e4723f76f322de41b7175327e1537855caa7adc9e3a725041de4e06e3`

Source: "In de namiddag zetten we ons nog even op het terras."

Candidate: "In de namiddag zetten we ons nog even op het terras."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De volledige, in Belgisch-Nederlands grammaticaal correcte bron is ongewijzigd. De constructie ‘ons zetten’ mag blijven staan, zodat een geldige regionale variant behouden blijft. Individueel beoordeelde laag: parsed; status: parsed.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-175 / delivered

Subject: `review-b0946c9368b52bd28589e3adbb684d3b22ed1255eb971a419e3030f00d5ee458`

Source: "In de namiddag zetten we ons nog even op het terras."

Candidate: "In de namiddag zetten we ons nog even op het terras."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De volledige, in Belgisch-Nederlands grammaticaal correcte bron is ongewijzigd. De constructie ‘ons zetten’ mag blijven staan, zodat een geldige regionale variant behouden blijft. Individueel beoordeelde laag: delivered; status: delivered.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-176 / annotations

Subject: `review-ce314a3542645e283d9190e3d052715b4ac6e620ac8579bc678af7d0e6d1d3ac`

Source: "Leave `invoice_no` and JSON null unchanged in this example."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Onafhankelijke bronbeoordeling, zonder modelkandidaat: De Engelse imperatief is correct. Zowel de letterlijk gespelde identifier `invoice_no` als de aanduiding JSON null en de backticks zijn betekenisdragend; de instructie zegt expliciet deze ongewijzigd te laten. Deze annotatieregel vertegenwoordigt geen geslaagde modelcorrectie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-176 / parsed

Subject: `review-fc1663735d6812775f652aa438795f4df3c7ef55ca2f077f73faac975a5598fb`

Source: "Leave `invoice_no` and JSON null unchanged in this example."

Candidate: "Leave `invoice_no` and JSON null unchanged in this example."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De al correcte Engelse zin is letterlijk behouden, inclusief `invoice_no`, ‘JSON null’ en de code-backticks. De technische instructie is intact. Individueel beoordeelde laag: parsed; status: parsed.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-176 / delivered

Subject: `review-e0f8cc84867b2d2f91c43a36e1bbf4a7be11025011ed5bb0f7ac5cb35617fc0b`

Source: "Leave `invoice_no` and JSON null unchanged in this example."

Candidate: "Leave `invoice_no` and JSON null unchanged in this example."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"allDefectsRepaired": {"preparer": true, "external": null}, "justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Manual preparer reading: necessary local repair is correct and preserves meaning/identifiers, or correct source is preserved exactly.

External rationale: De al correcte Engelse zin is letterlijk behouden, inclusief `invoice_no`, ‘JSON null’ en de code-backticks. De technische instructie is intact. Individueel beoordeelde laag: delivered; status: delivered.

Interpretation: Clean-source allDefectsRepaired is inapplicable/vacuous; complete-passage analysis uses cleanPreserved instead. Strict passage conjunction before/after: preparer=True, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## confirmation-bound / case-178 / annotations

Subject: `review-d3e67dd2fc30c6d30741408f578830844273437dfadb5f1f8f5d6aebff9ada9c`

Source: "De factuur is niet betaalt; stuur dus nog geen bevestiging."

Candidate: null

Conditions: source-annotations

Differences: {"justified": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Preparer AI inspected this original and its frozen annotation(s). They are plausible for this fictional passage; this is not independent label validation.

External rationale: Onafhankelijke bronbeoordeling, zonder modelkandidaat: De bron bevat een voltooid-deelwoordfout: na ‘is’ hoort ‘betaald’, niet de persoonsvorm ‘betaalt’. De ontkenning ‘niet’ en instructie ‘nog geen bevestiging’ zijn essentieel. Deze annotatieregel vertegenwoordigt geen geslaagde modelcorrectie.

Interpretation: Strict passage conjunction before/after: preparer=None, external=None. Delivery eligibility is applied separately.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-075 / parsed

Subject: `review-afbcbcf5ec98a34e72a55cff740fa1dbbe9870e469e34157040d872965e75cfd`

Source: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Candidate: "Ik heb geest in frieten, maar de frituur is nog dicht."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-Original-1.0.0

Differences: {"criticalSemanticFailure": {"preparer": null, "external": true}}; per-error: {}

Preparer rationale: Informal Belgian appetite word goesting is replaced by geest, damaging the sentence. Severity classification needs external review.

External rationale: The correct Belgian-Dutch noun "goesting" (appetite/craving) was replaced with "geest" (spirit/mind). "Ik heb geest in frieten" is an incoherent collocation, fundamentally losing the speaker's desire for fries; the second clause remains intact. This parsed candidate must not be counted as a delivered repair.

Interpretation: No automatic adjudication. Both original rationales remain the linguistic reasoning; sensitivity uses only their explicit fields.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-075 / parsed

Subject: `review-34a699405fbb539d39744c0260fe39f76631476b44239100952962867422c190`

Source: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Candidate: "Ik heb een gevoel van honger naar frieten, maar de frituur is nog gesloten."

Conditions: phase4-LiteralJson-LanguageSpecific-1.0.0

Differences: {"meaningPreserved": {"preparer": true, "external": false}, "noNewErrors": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: Unnecessary paraphrase of a valid informal regional sentence. Broad appetite meaning remains; the original register/wording does not.

External rationale: The source is idiomatic Belgian Dutch; this over-expands "goesting" into the stilted "een gevoel van honger naar frieten". Hunger is not exactly the desire/appetite expressed by "goesting". "Nog gesloten" is valid Dutch but needlessly alters the informal register of "nog dicht"; the shop is still closed. This parsed candidate must not be counted as a delivered repair.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-075 / parsed

Subject: `review-bc76c9d78e7e7b40c06f51963c34a41b1a0d96ee5faee003cc8554449f9ea1e8`

Source: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Candidate: "Ik heb zin op frieten, maar de frituur is nog dicht."

Conditions: phase4-LiteralJson-FewShot-1.0.0

Differences: {"noNewErrors": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Unnecessary replacement of goesting, with incorrect collocation zin op instead of zin in. Appetite meaning broadly survives but a new preposition error is introduced.

External rationale: The original "goesting in frieten" is already correct and regionally appropriate. Standard Dutch usually uses "zin in frieten", not "zin op frieten". The meaning remains a desire for fries, but the status of "zin op" as an acceptable informal Belgian-Dutch variant is uncertain, so noNewErrors is left null rather than automatically classifying it as wrong. This parsed candidate must not be counted as a delivered repair.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-165 / parsed

Subject: `review-4080e6ff32c7cd1df53da507a347890ebe8cf3e0c58324ca0f4905d8dc56ab46`

Source: "We hebben je aanvraag onvangen en sturen binnenkort een bevestiging."

Candidate: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Conditions: phase4-LiteralJson-FewShot-1.0.0, phase4-LiteralJson-LanguageSpecific-1.0.0, phase4-LiteralJson-Minimal-1.0.0

Differences: {"criticalSemanticFailure": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: The source typo onvangen is not repaired to ontvangen. The new onontvangen is malformed in this construction and evokes nonreceipt, contradicting the intended acknowledgment. All added-instruction arms introduce this output.

External rationale: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Interpretation: No automatic adjudication. Both original rationales remain the linguistic reasoning; sensitivity uses only their explicit fields.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-180 / parsed

Subject: `review-0508f23e75589e9deaaf9ec0122545234d8e45abf1a1170461028bb6ba931ef6`

Source: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Candidate: "Ik vraag me af waarom ik steeds alles zelf doe."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-FewShot-1.0.0, phase4-LiteralJson-LanguageSpecific-1.0.0, phase4-LiteralJson-Minimal-1.0.0, phase4-LiteralJson-Original-1.0.0

Differences: {"noNewErrors": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: The source expresses wanting to do something; the candidate asserts doing it. Volition is deleted. Local syntax is grammatical, but the complete repair is semantically unacceptable.

External rationale: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-180 / parsed

Subject: `review-70bb239d67ea55202b236ef87380390a197c4494a2d3203dcf9d2d556a359187`

Source: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Candidate: "Ik vraag me af waarom ik steeds alles zelf moet doen."

Conditions: phase4-PlainText-Original-1.0.0

Differences: {"noNewErrors": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: Wanting is changed into obligation. Existing validator appropriately withholds it.

External rationale: De indirecte-vraagvolgorde ‘waarom ik ...’ is grammaticaal gerepareerd, maar ‘wil doen’ (eigen wens) is vervangen door ‘moet doen’ (noodzaak/verplichting). Deze modaliteitswijziging verandert de strekking. Lokale syntaxisreparatie bewijst dus geen geslaagde passagecorrectie. Individueel beoordeelde laag: parsed; status: parsed.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-041 / parsed

Subject: `review-c9063b0fab82adc30ca4d2c70cef3f395e02016532fa49ee05abb082f9b25f39`

Source: "Niemand weet wanneer moet de trein vertrekken."

Candidate: "Niemand weet wanneer de trein vertrekt."

Conditions: phase4-LiteralJson-FewShot-1.0.0

Differences: {"noNewErrors": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: The source modal moet is removed. The otherwise grammatical word-order repair loses obligation/scheduled necessity and is correctly withheld.

External rationale: De inversie is grammaticaal gerepareerd, maar «moet vertrekken» is veranderd in «vertrekt». Daarmee verdwijnt de noodzaak/verplichting en ontstaat een andere vraag: verplicht vertrekmoment versus feitelijk vertrek. Niet gelijkwaardig ondanks grammaticale correctheid. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-154 / parsed

Subject: `review-f9aa54988ac8fde1b1f8b2cf5cc2fdaf758aafc3f3646818b535adff6dc90bed`

Source: "Het koffie apparaat lekt zodra ik het aanzet."

Candidate: "Het koffie-apparaat lekt zodra ik het aanzet."

Conditions: diorthosis-correction-v3, phase4-LiteralJson-FewShot-1.0.0, phase4-LiteralJson-Minimal-1.0.0, phase4-LiteralJson-Original-1.0.0, phase4-PlainText-Original-1.0.0

Differences: {"noNewErrors": {"preparer": true, "external": null}}; per-error: {"0": {"preparer": true, "external": null}}

Preparer rationale: Plausible valid compound repair using a facultative readability hyphen, withheld by the identifier guard. Qualified review remains appropriate; no frozen alternative is added.

External rationale: Replacing the incorrect space in "koffie apparaat" with a hyphen joins the compound and preserves the leaking-machine meaning. The usual unhyphenated standard is "koffieapparaat"; a readability hyphen can sometimes be facultative in Dutch, but its necessity/acceptability here is debatable. Do not silently treat this nonpreferred variant as certainly wrong or certainly a complete repair. This is a parsed candidate, not a claim that the text was delivered.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate. Preparer credits facultative hyphenation; external review retains orthographic uncertainty. Do not count an unknown repair as a verified validator false rejection.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-152 / parsed

Subject: `review-71eebb816510292fff62080d0b3243bc8dcebeccd1682867ee0ab22b925b789a`

Source: "Op de kaart staan déjà vu, Straße en καλημέρα naast elkaar."

Candidate: "Op de kaart staan *déjà vu*, Straße en καλημέρα naast elkaar."

Conditions: phase4-LiteralJson-LanguageSpecific-1.0.0

Differences: {"noNewErrors": {"preparer": false, "external": null}}; per-error: {}

Preparer rationale: Language-specific prompt adds unrequested phrase emphasis to clean multilingual text. The words/meaning remain, but formatting is damaged.

External rationale: Only literal Markdown asterisks around "déjà vu" are added. Original French, German and Greek text is retained exactly and meaning is unchanged. The clean source did not ask for emphasis; whether this becomes visible extraneous stars or legitimate italics depends on the output renderer. This is a parsed candidate, not a claim that the text was delivered.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-170 / parsed

Subject: `review-1380a77e18dbbdbd38f0f2f3fd656f6d7c8c63c066ed6dbdfbc46a17a7f68355`

Source: "The translaton preserves bonjour and grüß dich as greetings."

Candidate: "The translation preserves bonjour and *grüß dich* as greetings."

Conditions: phase4-LiteralJson-FewShot-1.0.0, phase4-LiteralJson-LanguageSpecific-1.0.0, phase4-LiteralJson-Minimal-1.0.0, phase4-LiteralJson-Original-1.0.0, phase4-PlainText-Original-1.0.0

Differences: {"noNewErrors": {"preparer": false, "external": null}, "unnecessaryChanges": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: The spelling repair is valid, but phrase-level Markdown emphasis is unrequested and accepted. Whole-passage compliance is not established by the local repair.

External rationale: De bronfout “translaton” is gecorrigeerd tot “translation”. De Franse en Duitse begroetingen blijven exact gespeld, inclusief ü en ß. De kandidaat voegt Markdown-cursivering toe rond “grüß dich”. Dat is een mogelijke typografische conventie, maar geen noodzakelijke taalcorrectie; de acceptatie hangt af van het uitvoerformaat. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.



## selection-literal-parsed / case-045 / parsed

Subject: `review-43f3ec91b31c233aa856cb14fdd7cfd0051b4d293364ca5bc9a9468194a777ff`

Source: "Does the pumps run during a power cut?"

Candidate: "Does the pump run during a power cut?"

Conditions: phase4-LiteralJson-LanguageSpecific-1.0.0, phase4-PlainText-Original-1.0.0

Differences: {"meaningPreserved": {"preparer": false, "external": null}, "criticalSemanticFailure": {"preparer": true, "external": null}, "noNewErrors": {"preparer": false, "external": null}, "unnecessaryChanges": {"preparer": true, "external": null}}; per-error: {}

Preparer rationale: Original plural pumps becomes singular pump. Agreement is superficially grammatical but the number of technical objects changes; the proper minimal repair preserves pumps and changes Does to Do.

External rationale: Grammaticale congruentie is hersteld door «pumps» naar «pump» te veranderen, maar het expliciet genoemde aantal objecten gaat van meervoud naar enkelvoud. Zonder context kan de betekenis niet veilig als behouden worden aangemerkt. Behoud van meervoud door «Does» → «Do» is de conservatieve correctie. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Different assessment coverage; retain unknowns.



## selection-literal-parsed / case-177 / parsed

Subject: `review-76e869fae08c047e70aab6cbb492904000a72d12caec02831dba3cf44c490e39`

Source: "If the test fails, leave the original file where it is."

Candidate: "If the test fails, leave the original file as it is."

Conditions: phase4-LiteralJson-LanguageSpecific-1.0.0, phase4-LiteralJson-Minimal-1.0.0

Differences: {"noNewErrors": {"preparer": true, "external": false}}; per-error: {}

Preparer rationale: A location constraint (where the file is) is changed to a state/content constraint (as it is). This clean conditional sentence needed no rewrite; the minimal and language-specific arms accept it.

External rationale: ‘Where it is’ verlangt behoud van de fysieke/logische bestandslocatie; ‘as it is’ verlangt doorgaans behoud van de huidige staat of inhoud. Een bestand kan onveranderd maar verplaatst zijn. De testvoorwaarde blijft behouden, maar de uitvoeringsinstructie verandert wezenlijk. Individueel beoordeelde laag: parsed; status: parsed.

Interpretation: Inspect both rationales: one reviewer may separate semantic damage from newly introduced linguistic errors; necessity, meaning and new errors remain separate.

Impact: Known contradiction: further qualified review warranted; do not resolve by authority or change frozen labels.
