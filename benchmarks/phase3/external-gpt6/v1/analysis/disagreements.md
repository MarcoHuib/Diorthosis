# External GPT-6 / preparer AI reconciliation

Both are non-independent AI diagnostic assessments. No reviewer or reference is automatically authoritative. Full records, including taxonomy/alternative differences, are in comparison.jsonl.



## case-001 / annotations / review-b827feda8d17d40b6d9a8d77fe5fbbc9bbc0c99e3f7e07d1e0d71d45fd637216

Source: "She is a experienced engineer."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. An is required before the vowel sound in experienced. Literal single-star emphasis is an unnecessary added alteration in a delivered sample; double-star emphasis is stripped from another response. Correct local article repair and acceptable whole passage are different judgments.

External rationale: Source evidence: 'She is a experienced engineer.'. Proposed annotation data was checked but was not treated as ground truth. In ‘a experienced’ the article must be ‘an’ because ‘experienced’ starts with a vowel sound. The suggested ‘an experienced’ is correct and the only apparent error.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-002 / annotations / review-1ff58714b72f78972a29c7351965f43d892ee54fc3cb8bc5d97b83bbfff357f9

Source: "Zij heeft de uitnodiging gisteren verstuurt."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. After heeft, verstuurd is the participle. Uitgezonden is a different lexical action and is not a justified repair of an invitation being sent.

External rationale: Source evidence: 'Zij heeft de uitnodiging gisteren verstuurt.'. Proposed annotation data was checked but was not treated as ground truth. In ‘heeft ... verstuurt’, ‘heeft’ requires the past participle ‘verstuurd’, ending in -d. The annotation correctly identifies the error.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-002 / delivered / review-ce886078193e5164fbc038ac30d5d7c6aaf4a7c4a7fb550c6c8b22cd7bdcb57a

Source: "Zij heeft de uitnodiging gisteren verstuurt."

Candidate: "Zij heeft de uitnodiging gisteren uitgezonden."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "noNewErrors": {"preparer": true, "external": false}}

Preparer rationale: After heeft, verstuurd is the participle. Uitgezonden is a different lexical action and is not a justified repair of an invitation being sent. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Zij heeft de uitnodiging gisteren verstuurt.'. Candidate: 'Zij heeft de uitnodiging gisteren uitgezonden.'. ‘Verstuurt’ is replaced with grammatical ‘uitgezonden’, but broadcasting/transmitting an invitation is not the same as sending it to recipients; the lexical meaning and idiomatic collocation change unnecessarily.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-002 / raw / review-a3b9f9d92e3a3e0d56b4b96f853c047971612b9689f1ab0afea1105e6b2c2d89

Source: "Zij heeft de uitnodiging gisteren verstuurt."

Candidate: "Zij heeft de uitnodiging gisteren uitgezonden."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "noNewErrors": {"preparer": true, "external": false}}

Preparer rationale: After heeft, verstuurd is the participle. Uitgezonden is a different lexical action and is not a justified repair of an invitation being sent. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Zij heeft de uitnodiging gisteren verstuurt.'. Candidate: 'Zij heeft de uitnodiging gisteren uitgezonden.'. ‘Verstuurt’ is replaced with grammatical ‘uitgezonden’, but broadcasting/transmitting an invitation is not the same as sending it to recipients; the lexical meaning and idiomatic collocation change unnecessarily.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / annotations / review-579f82728a62cc85a005defaf32299866f87924b1e58a7c2dc53e4c134e0398b

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain.

External rationale: Source evidence: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Proposed annotation data was checked but was not treated as ground truth. The directions are well-formed. `retry_count`, `OAuth2`, the number 3, and ‘do not retry ... automatically’ are intentional technical and negation-sensitive content; no edit is called for.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-003 / rejected / review-331b1194bafd809d36fa22cd8be4556609d3c3e134018475129f34d20a3c0777

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set `retry_count` to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-202

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set `retry_count` to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically.'. Only the backticks around `OAuth2` disappear. The literal token is legible and instructions otherwise remain, but code formatting of a protected identifier is lost for no linguistic reason. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / rejected / review-c6c4c9821fbd4d2b90a2e1c21d4e9e04f604ed89a8f83660709da48ddb9190da

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set retry\\_count to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-101

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set retry\\_count to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically.'. The code literal `retry_count` becomes the escaped plain-text ‘retry\_count’, and `OAuth2` loses code delimiters. Literal identifier representation is corrupted even though the number and negative instruction remain. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / rejected / review-2e479d2cbb7705131b75b20bfa65e8ec59e09ef5e100da98455f4eac6241aa3b

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set `retry_count` to 3. If *OAuth2* returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-303

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set `retry_count` to 3. If *OAuth2* returns an error, retain the original status code and do not retry the request automatically.'. The code-formatted `OAuth2` is recast as emphasized ‘*OAuth2*’, an unnecessary technical formatting change. The failure-handling meaning remains otherwise intact. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / rejected / review-bea9be7aef87f1f3250b85ffc0f10d6c0ab9af10c6a5a279f3aab80d0fb1c723

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: null

Conditions: voting-3

Different fields: {"rejectionAppropriate": {"preparer": null, "external": true}}

Preparer rationale: No eligible voted candidate exists. Absence of a candidate is not clean preservation or semantic success. The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain.

External rationale: Source evidence: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: null. No extracted candidate was provided. No source-to-candidate linguistic judgment is possible; withholding a non-existent output is appropriate.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. No candidate exists: external approval of abstention is distinct from a linguistic judgment about an output. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-003 / raw / review-68306ba9d58c23848080fc974da17d344a0d3b886cb44e8399d4d6d750f66a43

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set `retry_count` to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-202

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set `retry_count` to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically.'. Only the backticks around `OAuth2` disappear. The literal token is legible and instructions otherwise remain, but code formatting of a protected identifier is lost for no linguistic reason.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / raw / review-74892b15ab5b8df67d8d3e283c81660d0a2d14d2ad4f719d04944f61375a8a4d

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set retry\\_count to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-101

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set retry\\_count to 3. If OAuth2 returns an error, retain the original status code and do not retry the request automatically.'. The code literal `retry_count` becomes the escaped plain-text ‘retry\_count’, and `OAuth2` loses code delimiters. Literal identifier representation is corrupted even though the number and negative instruction remain.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-003 / raw / review-5cc64ca2df6dc70a284b3ec7b39467742dcefc0515d60b1604862013671f7349

Source: "Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically."

Candidate: "Set `retry_count` to 3. If *OAuth2* returns an error, retain the original status code and do not retry the request automatically."

Conditions: sample-303

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable. Deleting code delimiters or adding escapes/emphasis violates protected source notation; the rejection is appropriate even when the identifier letters remain. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Set `retry_count` to 3. If `OAuth2` returns an error, retain the original status code and do not retry the request automatically.'. Candidate: 'Set `retry_count` to 3. If *OAuth2* returns an error, retain the original status code and do not retry the request automatically.'. The code-formatted `OAuth2` is recast as emphasized ‘*OAuth2*’, an unnecessary technical formatting change. The failure-handling meaning remains otherwise intact.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-004 / annotations / review-8f5faf2ae437dd1615595dacd230b81dbd8b537a3b6972f1036205271984b8f2

Source: "Kom je straks ook? Ik zet alvast koffie; neem gerust iemand mee."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The informal source is grammatical and should remain literal. Appending a self-assessment note violates the output contract; the extractor removes it before delivery.

External rationale: Source evidence: 'Kom je straks ook? Ik zet alvast koffie; neem gerust iemand mee.'. Proposed annotation data was checked but was not treated as ground truth. ‘Kom je straks ook?’ and ‘Ik zet alvast koffie; neem gerust iemand mee’ are idiomatic Dutch. The semicolon legitimately joins closely related independent clauses; no spelling or grammar error.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-005 / annotations / review-a2b425849159bd773f4450b8e36de85fa649451c27ce43cfbf2dc922574d1c33

Source: "The organisation will analyse the colour samples at its new centre."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The British spellings are acceptable. Inserting also invents an additional relation not supported by the source and is unjustified. Voting removes that sampled insertion.

External rationale: Source evidence: 'The organisation will analyse the colour samples at its new centre.'. Proposed annotation data was checked but was not treated as ground truth. ‘Organisation’, ‘analyse’, ‘colour’ and ‘centre’ are consistent standard British English; they are not American-English spelling mistakes. No content change is necessary.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-005 / delivered / review-5f82eed73b9912965e322b0f2002230d97e27656fc8bea6cc7d370886792a5a3

Source: "The organisation will analyse the colour samples at its new centre."

Candidate: "The organisation will also analyse the colour samples at its new centre."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "noNewErrors": {"preparer": true, "external": false}}

Preparer rationale: The British spellings are acceptable. Inserting also invents an additional relation not supported by the source and is unjustified. Voting removes that sampled insertion. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'The organisation will analyse the colour samples at its new centre.'. Candidate: 'The organisation will also analyse the colour samples at its new centre.'. Inserts ‘also’, which asserts an additional or parallel activity absent from the original. The British spellings were correct and provide no license for a semantic addition.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-005 / raw / review-543c79509f20176ba325ddc797bbb3552ad60ee3a4c8ee79f176a807e4305f16

Source: "The organisation will analyse the colour samples at its new centre."

Candidate: "The organisation will also analyse the colour samples at its new centre."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "noNewErrors": {"preparer": true, "external": false}}

Preparer rationale: The British spellings are acceptable. Inserting also invents an additional relation not supported by the source and is unjustified. Voting removes that sampled insertion. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'The organisation will analyse the colour samples at its new centre.'. Candidate: 'The organisation will also analyse the colour samples at its new centre.'. Inserts ‘also’, which asserts an additional or parallel activity absent from the original. The British spellings were correct and provide no license for a semantic addition.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-006 / annotations / review-fada914b14d9a989e206cf30354eefd94b80aa96cd493636adf93a97c68ed89d

Source: "The list of approved items are on the desk."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The head noun list is singular, so is is the repair. Raw bold markup is unnecessary, though the extracted delivered passage removes it.

External rationale: Source evidence: 'The list of approved items are on the desk.'. Proposed annotation data was checked but was not treated as ground truth. In ‘The list of approved items are’, the head of the subject is singular ‘list’; the finite verb must be ‘is’. ‘Items’ is within a prepositional modifier, not the subject head.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-007 / annotations / review-6e8de102b577e3d75a5e084931d4b3ccf7d0f8220a40ff70f9f4f3ac60d9c432

Source: "Ik weet dat hij heeft het bestand opgeslagen."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Dutch subordinate-clause order allows both het bestand heeft opgeslagen and het bestand opgeslagen heeft. The latter is missing from the frozen reference. A comma directly before dat is unnecessary here and merits punctuation review, not automatic equivalence with a required repair.

External rationale: Source evidence: 'Ik weet dat hij heeft het bestand opgeslagen.'. Proposed annotation data was checked but was not treated as ground truth. In the Dutch subordinate clause after ‘dat’, ‘heeft het bestand opgeslagen’ is ordered as a main clause. Place the object before the verbal cluster: ‘dat hij het bestand heeft opgeslagen’ or ‘dat hij het bestand opgeslagen heeft’.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-007 / delivered / review-f7a4bc20cd5eb7b7d439c714d34f2101a9f0740923d4fa600a9d216375f5321c

Source: "Ik weet dat hij heeft het bestand opgeslagen."

Candidate: "Ik weet, dat hij het bestand heeft opgeslagen."

Conditions: sample-101

Different fields: {"noNewErrors": {"preparer": null, "external": false}}

Preparer rationale: Dutch subordinate-clause order allows both het bestand heeft opgeslagen and het bestand opgeslagen heeft. The latter is missing from the frozen reference. A comma directly before dat is unnecessary here and merits punctuation review, not automatic equivalence with a required repair. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Ik weet dat hij heeft het bestand opgeslagen.'. Candidate: 'Ik weet, dat hij het bestand heeft opgeslagen.'. The subordinate-clause word order is now correct, but the added comma after ‘weet’ is not normally used before its closely integrated ‘dat’-complement; it is an unnecessary and questionable punctuation intrusion.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-007 / raw / review-f3cbe822d0b26453f7c7221247b782be9910ef9214b028076562e86014873756

Source: "Ik weet dat hij heeft het bestand opgeslagen."

Candidate: "Ik weet, dat hij het bestand heeft opgeslagen."

Conditions: sample-101

Different fields: {"noNewErrors": {"preparer": null, "external": false}}

Preparer rationale: Dutch subordinate-clause order allows both het bestand heeft opgeslagen and het bestand opgeslagen heeft. The latter is missing from the frozen reference. A comma directly before dat is unnecessary here and merits punctuation review, not automatic equivalence with a required repair. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Ik weet dat hij heeft het bestand opgeslagen.'. Candidate: 'Ik weet, dat hij het bestand heeft opgeslagen.'. The subordinate-clause word order is now correct, but the added comma after ‘weet’ is not normally used before its closely integrated ‘dat’-complement; it is an unnecessary and questionable punctuation intrusion.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-008 / annotations / review-a48ddd82b790de98e2314f7e3f85152a599aef2edd07776d5a6e9f298bfe6dec

Source: "The neighborhood theater canceled the evening performance while staff repaired the elevator."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The American spellings and source sentence are acceptable. Replacing an actual cancellation with plans to cancel changes event status and time; the sampled rewrite is appropriately rejected.

External rationale: Source evidence: 'The neighborhood theater canceled the evening performance while staff repaired the elevator.'. Proposed annotation data was checked but was not treated as ground truth. The original is idiomatic American English (‘neighborhood’, ‘theater’, ‘canceled’, ‘elevator’). It states an actual cancellation while repairs were taking place; no error requires rewriting.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-008 / rejected / review-00133f3dec97e0e12488085fd5ddf45c9fee01caf633957cbb69e0a1468b4432

Source: "The neighborhood theater canceled the evening performance while staff repaired the elevator."

Candidate: "The neighborhood theater had plans to cancel the evening performance, however, staff were busy repairing the elevator before the scheduled showtime."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: The American spellings and source sentence are acceptable. Replacing an actual cancellation with plans to cancel changes event status and time; the sampled rewrite is appropriately rejected. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'The neighborhood theater canceled the evening performance while staff repaired the elevator.'. Candidate: 'The neighborhood theater had plans to cancel the evening performance, however, staff were busy repairing the elevator before the scheduled showtime.'. ‘Canceled’ (completed action) becomes ‘had plans to cancel’ (intention, not necessarily realized), ‘while’ becomes a contrastive ‘however’, and ‘before the scheduled showtime’ is invented. It also creates a comma splice with ‘, however,’. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-008 / raw / review-141e3b32974060d4e622e6c6964e05832ac62498d4edb9f5bb09efdecb769b50

Source: "The neighborhood theater canceled the evening performance while staff repaired the elevator."

Candidate: "The neighborhood theater had plans to cancel the evening performance, however, staff were busy repairing the elevator before the scheduled showtime."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: The American spellings and source sentence are acceptable. Replacing an actual cancellation with plans to cancel changes event status and time; the sampled rewrite is appropriately rejected. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'The neighborhood theater canceled the evening performance while staff repaired the elevator.'. Candidate: 'The neighborhood theater had plans to cancel the evening performance, however, staff were busy repairing the elevator before the scheduled showtime.'. ‘Canceled’ (completed action) becomes ‘had plans to cancel’ (intention, not necessarily realized), ‘while’ becomes a contrastive ‘however’, and ‘before the scheduled showtime’ is invented. It also creates a comma splice with ‘, however,’.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / annotations / review-17ef58d95b7003ce1484c27f11080227695cea33eb3e335560ae4b614af5751c

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong.

External rationale: Source evidence: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Proposed annotation data was checked but was not treated as ground truth. The Dutch instruction is correct. The backtick-delimited identifiers `retry_count` and `OAuth2`, value 3, ‘oorspronkelijke statuscode’, and the negative ‘niet automatisch opnieuw’ must remain intact.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-009 / rejected / review-19822d49aa4328c4f808d9938fbe488b3bb3e5efc4856475975fab9d619ce161

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel **retry_count** in op 3. Als **OAuth2** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-202

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel **retry_count** in op 3. Als **OAuth2** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Both original code spans `retry_count` and `OAuth2` become bold Markdown **retry_count** and **OAuth2**; identifiers are no longer represented as code despite no language defect. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / rejected / review-d277375891be461d26afa8a1365e8ef75554c00196c645dd00a8c7222017143c

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel `__retry_count__` in op 3. Als `__OAuth2__` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-101

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel `__retry_count__` in op 3. Als `__OAuth2__` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Code-span contents change from the exact identifiers `retry_count` and `OAuth2` to `__retry_count__` and `__OAuth2__`. Unlike mere styling outside code, inserted underscores are inside backticks and alter the actual technical names. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / rejected / review-d48aebfd037ec54f73a891813784bf9a63022d8095acad97e2baae6c1b695f73

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel **\\u0060retry_count\\u0060** in op 3. Als **\\u0060OAuth2\\u0060** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-303

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel **\\u0060retry_count\\u0060** in op 3. Als **\\u0060OAuth2\\u0060** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Literal backslash-u escape strings ‘\u0060’ replace Markdown backticks, wrapped in bold markers. This is not the original code-formatted identifier representation and leaks escape syntax into text. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / rejected / review-596a611559f6d8b9ea5909b7ba497cd9eee7addf61c6ddfbad3a2048d7d991c9

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: null

Conditions: voting-3

Different fields: {"rejectionAppropriate": {"preparer": null, "external": true}}

Preparer rationale: No eligible voted candidate exists. Absence of a candidate is not clean preservation or semantic success. The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong.

External rationale: Source evidence: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: null. No extracted candidate was provided. No source-to-candidate linguistic judgment is possible; withholding a non-existent output is appropriate.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. No candidate exists: external approval of abstention is distinct from a linguistic judgment about an output. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-009 / rejected / review-a882181a9fb5a06496c83eb43c643e05b7707b4a76a34cde9ac434d988fbfcda

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel *retry_count* in op 3. Als *OAuth2* een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: baseline, hints

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel *retry_count* in op 3. Als *OAuth2* een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Both technical code spans become italic Markdown rather than backtick-delimited identifiers; the instruction and negation are otherwise unchanged. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / raw / review-0e2d42f4debe34726399061b197756ebfd9a478732361db07a39dde21b671154

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel `__retry_count__` in op 3. Als `__OAuth2__` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-101

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel `__retry_count__` in op 3. Als `__OAuth2__` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Code-span contents change from the exact identifiers `retry_count` and `OAuth2` to `__retry_count__` and `__OAuth2__`. Unlike mere styling outside code, inserted underscores are inside backticks and alter the actual technical names.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / raw / review-b9be318818f8d28ce2b843f6351fcd0910de3e22dc69f50e61a6ecadcce45582

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel **retry_count** in op 3. Als **OAuth2** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-202

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel **retry_count** in op 3. Als **OAuth2** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Both original code spans `retry_count` and `OAuth2` become bold Markdown **retry_count** and **OAuth2**; identifiers are no longer represented as code despite no language defect.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / raw / review-3fe19e78f3a7b651ac130ec58c3c091f4235cc64911f4861abf1fd96e1de2c03

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel **\\u0060retry_count\\u0060** in op 3. Als **\\u0060OAuth2\\u0060** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: sample-303

Different fields: {"criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel **\\u0060retry_count\\u0060** in op 3. Als **\\u0060OAuth2\\u0060** een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Literal backslash-u escape strings ‘\u0060’ replace Markdown backticks, wrapped in bold markers. This is not the original code-formatted identifier representation and leaks escape syntax into text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-009 / raw / review-1bb28ecfa1676629304956bcb3ebcf331254ed76622f13108ab771763d017f85

Source: "Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Candidate: "Stel *retry_count* in op 3. Als *OAuth2* een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw."

Conditions: baseline, hints

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The source is acceptable technical Dutch. Replacing code backticks with emphasis, adding underscores or escaped code-point literals changes protected source notation; rejection is appropriate. Dictionary absence for identifiers is not evidence that they are wrong. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Stel `retry_count` in op 3. Als `OAuth2` een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Candidate: 'Stel *retry_count* in op 3. Als *OAuth2* een fout meldt, bewaar dan de oorspronkelijke statuscode en probeer het verzoek niet automatisch opnieuw.'. Both technical code spans become italic Markdown rather than backtick-delimited identifiers; the instruction and negation are otherwise unchanged.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-010 / annotations / review-5e1f7ef82d33c9d25ce64c7b366c44319a8d8f286ede099f5cc45adee67d7edb

Source: "De lange termijnplanning staat op papier."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The proposed compound reading calls for langetermijnplanning. No delivered arm repairs it. The malformed lange-termijnerplanning is not an acceptable alternative. The intended compound interpretation still requires independent review.

External rationale: Source evidence: 'De lange termijnplanning staat op papier.'. Proposed annotation data was checked but was not treated as ground truth. For planning for the long term, Dutch compounds are written together: ‘langetermijnplanning’ (or idiomatically ‘planning voor de lange termijn’). ‘lange termijnplanning’ splits that compound in the intended reading.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-010 / rejected / review-7f7a2bcd9044582b2569e9e3ca241b84fae8a9a3314b983ef9d43b021129b43b

Source: "De lange termijnplanning staat op papier."

Candidate: "De lange-termijnerplanning staat op papier."

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The proposed compound reading calls for langetermijnplanning. No delivered arm repairs it. The malformed lange-termijnerplanning is not an acceptable alternative. The intended compound interpretation still requires independent review. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De lange termijnplanning staat op papier.'. Candidate: 'De lange-termijnerplanning staat op papier.'. ‘Lange-termijnerplanning’ is not a correct Dutch compound; the added ‘-er’ and hyphen create a new misspelling rather than repairing ‘langetermijnplanning’. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-010 / raw / review-b53df06824c77f13453cf732028e2f3179deec2ddc22f079a2b0a6de87831c96

Source: "De lange termijnplanning staat op papier."

Candidate: "De lange-termijnerplanning staat op papier."

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: The proposed compound reading calls for langetermijnplanning. No delivered arm repairs it. The malformed lange-termijnerplanning is not an acceptable alternative. The intended compound interpretation still requires independent review. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De lange termijnplanning staat op papier.'. Candidate: 'De lange-termijnerplanning staat op papier.'. ‘Lange-termijnerplanning’ is not a correct Dutch compound; the added ‘-er’ and hyphen create a new misspelling rather than repairing ‘langetermijnplanning’.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / annotations / review-4db6637f9c9c388e60c6cf220991c86b1cef94c9786c6d58ee87539dba77371c

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required.

External rationale: Source evidence: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Proposed annotation data was checked but was not treated as ground truth. ‘Coördinator’, ‘reünie’, ‘café’ and ‘geïmproviseerde’ all have correct diacritics and spelling; the past tense ‘besprak’ and ‘klonk’ is appropriate. No intervention is needed.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-011 / delivered / review-23057a3e6d1a0cbc92d5d2c4e67955db195bd3acc838df6336bec8c210cdb24a

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinator besprak deոց een café waar ook improviserende muziek klonk."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinator besprak deոց een café waar ook improviserende muziek klonk.'. ‘Coördinator’ loses a diaeresis, ‘reünie in’ is replaced with the nonsensical non-Dutch ‘deոց’, and ‘geïmproviseerde muziek’ becomes ‘improviserende muziek’ (different meaning). Original clean text is damaged.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / delivered / review-babf3de067641bbd3ee2a4215ec44eceeeea91ccb70f3e48a8df0e8221030688

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinateur besprak de fuewrnie in een café waar ook geïmproviseerde muziek klonk."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinateur besprak de fuewrnie in een café waar ook geïmproviseerde muziek klonk.'. ‘Coördinator’ is changed to nonstandard ‘coordinateur’ and ‘reünie’ to meaningless ‘fuewrnie’; both introduce errors into a correct sentence.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / delivered / review-ed4d466fc0dcc1c5e31c8347a685399cf1a244ed69a090e3915dbbd886a629c6

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinaat besprak de relië in een café waar ook gimproviseerde muziek klonk."

Conditions: sample-303

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinaat besprak de relië in een café waar ook gimproviseerde muziek klonk.'. ‘Coordinaat’ changes a human coordinator into a coordinate, ‘relië’ is not ‘reünie’, and ‘gimproviseerde’ is misspelled; fundamental referents and spelling break.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / raw / review-1962f69853f29fdd81c750ef3d2aeb2454ce2a0e4e988c09768eeff4d7266a64

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinateur besprak de fuewrnie in een café waar ook geïmproviseerde muziek klonk."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinateur besprak de fuewrnie in een café waar ook geïmproviseerde muziek klonk.'. ‘Coördinator’ is changed to nonstandard ‘coordinateur’ and ‘reünie’ to meaningless ‘fuewrnie’; both introduce errors into a correct sentence.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / raw / review-982afe7aa8e4f1520b92713f07822009ce6b25aa0fd18f3203dafc755f37275e

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinaat besprak de relië in een café waar ook gimproviseerde muziek klonk."

Conditions: sample-303

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinaat besprak de relië in een café waar ook gimproviseerde muziek klonk.'. ‘Coordinaat’ changes a human coordinator into a coordinate, ‘relië’ is not ‘reünie’, and ‘gimproviseerde’ is misspelled; fundamental referents and spelling break.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-011 / raw / review-b01c8f79643e97b7932b67e0be3cc2fe469c42d46e1dbfdeefdf10ee22df45df

Source: "De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk."

Candidate: "De coordinator besprak deոց een café waar ook improviserende muziek klonk.\n\n*(Always returned exactly as corrected, including capitalization and punctuation. If original had correct spelling, the input above contains no spelling/grammar/punctuation errors.)*"

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: The source is grammatical Dutch with correct diacritics. Corruptions such as deոց, fuewrnie, relië and gimproviseerde introduce errors; the Armenian-script fragment and loss of reünie in are a substantial meaning failure. No rewrite is required. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.'. Candidate: 'De coordinator besprak deոց een café waar ook improviserende muziek klonk.\n\n*(Always returned exactly as corrected, including capitalization and punctuation. If original had correct spelling, the input above contains no spelling/grammar/punctuation errors.)*'. As well as destroying ‘coördinator’, ‘reünie in’ and ‘geïmproviseerde’, the raw response appends a false English self-explanation. The intact source needed no repairs.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-012 / annotations / review-437433a87719c284df132c87180abd27e51468f9a3fceeff31092e8d470c07c0

Source: "De controleur controleerd de bon voordat hij vertrekt."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The singular present form is controleert; controleerd is erroneous. The controller/controleer output changes the occupational term and leaves agreement wrong; controlert is another misspelling.

External rationale: Source evidence: 'De controleur controleerd de bon voordat hij vertrekt.'. Proposed annotation data was checked but was not treated as ground truth. ‘De controleur controleerd’ uses a mistaken -d ending; the third-person present tense of ‘controleren’ is ‘controleert’. The subject ‘controleur’ is correct and must not be silently replaced with ‘controller’.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-013 / annotations / review-762173e411717334b98a02404ad4854d24815b511eb5a41b4e3aae20b567a5e1

Source: "Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. The original regional Dutch is acceptable. Bezorgt u een ontvangstbewijs is idiomatic; verzorgt u een ontvangstbewijs is an unjustified lexical substitution with a valency/collocation problem.

External rationale: Source evidence: 'Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs.'. Proposed annotation data was checked but was not treated as ground truth. ‘Gelieve ... af te geven’ is formal Dutch, and ‘De medewerker bezorgt u een ontvangstbewijs’ correctly uses ‘bezorgen’ with an indirect object ‘u’. No correction is needed; register is not an error.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-013 / delivered / review-690802d79f7f880e10e2a36ba4019480e0ccb136e4b51d02f762bd36005d7d0d

Source: "Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs."

Candidate: "Gelieve uw aanvraag aan het loket af te geven. De medewerker verzorgt u een ontvangstbewijs."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": false, "external": true}}

Preparer rationale: The original regional Dutch is acceptable. Bezorgt u een ontvangstbewijs is idiomatic; verzorgt u een ontvangstbewijs is an unjustified lexical substitution with a valency/collocation problem. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs.'. Candidate: 'Gelieve uw aanvraag aan het loket af te geven. De medewerker verzorgt u een ontvangstbewijs.'. ‘Bezorgt u’ is a natural ditransitive construction for giving a receipt; ‘verzorgt u een ontvangstbewijs’ is at best an awkward or region-dependent way to express provision, not a necessary correction of any source error.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Regional/collocational uncertainty: both reviewers reject the necessity of the change, but disagree about whether it preserves meaning. Do not implement a word blacklist. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-013 / raw / review-7889f3612525a35ae2fa65f8e197412e6f760273241802bf88d8fbf6f3971313

Source: "Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs."

Candidate: "Gelieve uw aanvraag aan het loket af te geven. De medewerker verzorgt u een ontvangstbewijs."

Conditions: sample-202

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": false, "external": true}}

Preparer rationale: The original regional Dutch is acceptable. Bezorgt u een ontvangstbewijs is idiomatic; verzorgt u een ontvangstbewijs is an unjustified lexical substitution with a valency/collocation problem. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Gelieve uw aanvraag aan het loket af te geven. De medewerker bezorgt u een ontvangstbewijs.'. Candidate: 'Gelieve uw aanvraag aan het loket af te geven. De medewerker verzorgt u een ontvangstbewijs.'. ‘Bezorgt u’ is a natural ditransitive construction for giving a receipt; ‘verzorgt u een ontvangstbewijs’ is at best an awkward or region-dependent way to express provision, not a necessary correction of any source error.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Regional/collocational uncertainty: both reviewers reject the necessity of the change, but disagree about whether it preserves meaning. Do not implement a word blacklist. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-014 / annotations / review-cb2e1d82f4e249c3424134409d234a5f03e1b60ac04642758f6b0625e7b73888

Source: "The museum has a seperate entrance for deliveries."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Separate is the required spelling; the original preposition and plural noun are acceptable.

External rationale: Source evidence: 'The museum has a seperate entrance for deliveries.'. Proposed annotation data was checked but was not treated as ground truth. ‘Seperate’ misspells the adjective ‘separate’; ‘a separate entrance’ is the correct spelling and normal collocation.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-015 / annotations / review-30c3820ee7de5122ea9efc643e942c00e7c30264a687ecd7714f8c1bad611f63

Source: "Please explain me the procedure."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails.

External rationale: Source evidence: 'Please explain me the procedure.'. Proposed annotation data was checked but was not treated as ground truth. In standard English, ‘explain’ takes the topic as its direct object and the listener through ‘to me’. ‘Explain me the procedure’ is nonstandard; ‘Please explain the procedure to me’ preserves the addressee.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-015 / rejected / review-9ee529cd296e78cc6e58a754c69dd0638609892b1d3f857074a769a0f0c3eb32

Source: "Please explain me the procedure."

Candidate: "Please explain me\n\nthe procedure."

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Please explain me the procedure.'. Candidate: 'Please explain me\n\nthe procedure.'. Adding two line breaks leaves nonstandard ‘explain me the procedure’ unchanged. Standard English requires ‘explain the procedure to me’. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-015 / rejected / review-914828790434e08bfdd0df5325c477289bf537b2d0bb5638f721d87fe87b308b

Source: "Please explain me the procedure."

Candidate: null

Conditions: voting-3

Different fields: {"rejectionAppropriate": {"preparer": null, "external": true}}

Preparer rationale: No eligible voted candidate exists. Absence of a candidate is not clean preservation or semantic success. Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails.

External rationale: Source evidence: 'Please explain me the procedure.'. Candidate: null. No extracted candidate was provided. No source-to-candidate linguistic judgment is possible; withholding a non-existent output is appropriate.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. No candidate exists: external approval of abstention is distinct from a linguistic judgment about an output. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-015 / rejected / review-e7e75abe411cca434b3f18b86f90a4b5ca9d6f33b7a2a18eafa194fda977d7d1

Source: "Please explain me the procedure."

Candidate: "Please explain the procedure."

Conditions: baseline, hints, sample-202, sample-303

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Please explain me the procedure.'. Candidate: 'Please explain the procedure.'. Deleting ‘me’ yields grammatical ‘Please explain the procedure’, but removes the specified addressee ‘to me’. The intended meaning-preserving correction is ‘Please explain the procedure to me’. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-015 / raw / review-de27d0834fa6771c9a3801cbbbcd0ba7fb1cdf4bc10caa8bb6e23e78b117c17c

Source: "Please explain me the procedure."

Candidate: "Please explain me\n\nthe procedure."

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Please explain me the procedure.'. Candidate: 'Please explain me\n\nthe procedure.'. Adding two line breaks leaves nonstandard ‘explain me the procedure’ unchanged. Standard English requires ‘explain the procedure to me’.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-015 / raw / review-2939f0cfaa63ec1f5fdf0276d03effb47819bd71138c6e16ec0c78e0a089e9c1

Source: "Please explain me the procedure."

Candidate: "Please explain the procedure."

Conditions: baseline, hints, sample-202, sample-303

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}}

Preparer rationale: Explain takes the thing explained as its direct object and to me for the recipient. Dropping me loses the intended addressee; rejection is appropriate, rather than crediting a shorter grammatical sentence. The paragraph-split output also fails. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Please explain me the procedure.'. Candidate: 'Please explain the procedure.'. Deleting ‘me’ yields grammatical ‘Please explain the procedure’, but removes the specified addressee ‘to me’. The intended meaning-preserving correction is ‘Please explain the procedure to me’.

Explanation / expected impact: All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-016 / annotations / review-b2fe724c9f3951c75ca84315384a23c1db09d9953602378ccdd77471e9025486

Source: "The backup has been took off site."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Has been taken is the correct passive perfect construction. Off site is acceptable adverbial wording; no hyphenation change is needed.

External rationale: Source evidence: 'The backup has been took off site.'. Proposed annotation data was checked but was not treated as ground truth. The passive perfect ‘has been ...’ needs the past participle ‘taken’, not the simple past ‘took’; ‘taken off site’ is grammatical.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-017 / annotations / review-0d1796b60d43d79e958fe35402165c7bcaff10abac4eec94acfef1473fcf7956

Source: "I wouldn't have called if I'd known you were still asleep."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Both ASCII and typographic apostrophes form grammatical contractions. The output makes an unnecessary cosmetic change. Blocking it preserves the clean source, but the name-change rejection reason is misleading; this is not a lost necessary linguistic repair.

External rationale: Source evidence: "I wouldn't have called if I'd known you were still asleep.". Proposed annotation data was checked but was not treated as ground truth. ‘I wouldn’t have called if I’d known you were still asleep’ uses an appropriate third conditional. Straight apostrophes in ‘wouldn't’ and ‘I'd’ are valid; no grammatical correction is necessary.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Alternatives may differ in granularity (fragment versus complete passage) or wording. Neither set is promoted into frozen references. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-017 / rejected / review-41cb38c725ef757114dccbbf2a361efb69a82ab1fb9bad9a7354c0015d36d89b

Source: "I wouldn't have called if I'd known you were still asleep."

Candidate: null

Conditions: voting-3

Different fields: {"rejectionAppropriate": {"preparer": null, "external": true}}

Preparer rationale: No eligible voted candidate exists. Absence of a candidate is not clean preservation or semantic success. Both ASCII and typographic apostrophes form grammatical contractions. The output makes an unnecessary cosmetic change. Blocking it preserves the clean source, but the name-change rejection reason is misleading; this is not a lost necessary linguistic repair.

External rationale: Source evidence: "I wouldn't have called if I'd known you were still asleep.". Candidate: null. No extracted candidate was provided. No source-to-candidate linguistic judgment is possible; withholding a non-existent output is appropriate.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. No candidate exists: external approval of abstention is distinct from a linguistic judgment about an output. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-018 / annotations / review-a9893206cdbf3e26522b01a06ed42dd3ff5fc7a0ff1999ee00f50668729736a6

Source: "De kinderen loop naar de biblioteek."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Plural kinderen requires lopen; bibliotheek repairs the spelling. Gaan de bibliotheek in changes walking toward the library into entering it; the rejection is appropriate.

External rationale: Source evidence: 'De kinderen loop naar de biblioteek.'. Proposed annotation data was checked but was not treated as ground truth. Plural ‘De kinderen’ requires ‘lopen’, not ‘loop’; the noun is spelled ‘bibliotheek’, not ‘biblioteek’. Both defects are independent.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-018 / rejected / review-7a42b72aa6e1b9a0c8ae9d1124250f28bb8a69ae93f152cf20a1312c7762b1de

Source: "De kinderen loop naar de biblioteek."

Candidate: "De kinderen gaan de bibliotheek in."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}, "noNewErrors": {"preparer": false, "external": true}}

Preparer rationale: Plural kinderen requires lopen; bibliotheek repairs the spelling. Gaan de bibliotheek in changes walking toward the library into entering it; the rejection is appropriate. Assessment binds this literal rejected candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De kinderen loop naar de biblioteek.'. Candidate: 'De kinderen gaan de bibliotheek in.'. Corrects the surface grammar/spelling but substitutes entering the library (‘gaan ... in’) for walking toward it (‘lopen naar’). Arrival/entry is not entailed by the source; change is unjustified. The rejected/review-required candidate was appropriately withheld; do not count it as delivered text.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-018 / raw / review-4fb20be7413cc32cb4373c05ae050faf5b7fcaf720c93e3f6651e99f1971d931

Source: "De kinderen loop naar de biblioteek."

Candidate: "De kinderen gaan de bibliotheek in."

Conditions: sample-101

Different fields: {"allDefectsRepaired": {"preparer": false, "external": true}, "criticalSemanticFailure": {"preparer": false, "external": true}, "meaningPreserved": {"preparer": null, "external": false}, "noNewErrors": {"preparer": false, "external": true}}

Preparer rationale: Plural kinderen requires lopen; bibliotheek repairs the spelling. Gaan de bibliotheek in changes walking toward the library into entering it; the rejection is appropriate. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'De kinderen loop naar de biblioteek.'. Candidate: 'De kinderen gaan de bibliotheek in.'. Corrects the surface grammar/spelling but substitutes entering the library (‘gaan ... in’) for walking toward it (‘lopen naar’). Arrival/entry is not entailed by the source; change is unjustified.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. All-defects-repaired is interpreted differently: the external reviewer separates repair of original defects (vacuously true for clean input) from new errors and meaning preservation. Evaluate the conjunction, not this field alone. Severity threshold differs; retain both AI labels without assuming a human-adjudicated critical-failure rate. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-019 / annotations / review-e659cd9ca102456e7acbc6a6d8f2963fda65f1500bd4b2b9f155b74b4f6f09fc

Source: "Wil je de bijalge bij dit bericht openen?"

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Bijlage repairs bijalge. Bijnalge is an invalid new spelling. The conditional garble changes the request and is appropriately rejected.

External rationale: Source evidence: 'Wil je de bijalge bij dit bericht openen?'. Proposed annotation data was checked but was not treated as ground truth. ‘Bijalge’ is a letter transposition of ‘bijlage’ (attachment); the rest of the informal request ‘Wil je ... openen?’ is natural Dutch.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.



## case-019 / delivered / review-367355c9b0507eb99d1a265fdf8e99f10c631ef908dd1eb568477d100f8161b8

Source: "Wil je de bijalge bij dit bericht openen?"

Candidate: "Wil je de bijnalge bij dit bericht openen?"

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: Bijlage repairs bijalge. Bijnalge is an invalid new spelling. The conditional garble changes the request and is appropriately rejected. Assessment binds this literal delivered candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Wil je de bijalge bij dit bericht openen?'. Candidate: 'Wil je de bijnalge bij dit bericht openen?'. The misspelling ‘bijalge’ is changed to another misspelling ‘bijnalge’, not the noun ‘bijlage’; the original defect remains and a new malformed token appears.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-019 / raw / review-11898424998c491f3705d5333dc4856b26c16926f2adde6c0c22697c97383df7

Source: "Wil je de bijalge bij dit bericht openen?"

Candidate: "Wil je de bijnalge bij dit bericht openen?"

Conditions: sample-101

Different fields: {"meaningPreserved": {"preparer": null, "external": true}}

Preparer rationale: Bijlage repairs bijalge. Bijnalge is an invalid new spelling. The conditional garble changes the request and is appropriately rejected. Assessment binds this literal raw candidate; formatting/comments are not silently credited as source-preserving corrections.

External rationale: Source: 'Wil je de bijalge bij dit bericht openen?'. Candidate: 'Wil je de bijnalge bij dit bericht openen?'. The misspelling ‘bijalge’ is changed to another misspelling ‘bijnalge’, not the noun ‘bijlage’; the original defect remains and a new malformed token appears.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Error taxonomies differ; category-list inequality alone is not an independent language failure. Full-passage diagnostic conjunction: preparer=False, external=False. Human metrics remain unknown; human approvals remain zero.



## case-020 / annotations / review-f44154d1b449b6e44fef402caa22f62fdcb1edc13f73fe0907fe16c826c5721b

Source: "The volunteers was carrying the equiptment upstairs."

Candidate: null

Conditions: annotations

Different fields: {"justified": {"preparer": null, "external": true}}

Preparer rationale: Source/annotation screening only. Plural volunteers requires were, and equipment repairs equiptment. Hints fix only spelling and leave was, losing a repair made by baseline.

External rationale: Source evidence: 'The volunteers was carrying the equiptment upstairs.'. Proposed annotation data was checked but was not treated as ground truth. Plural ‘The volunteers’ requires ‘were’, not ‘was’; ‘equiptment’ is misspelled and must be ‘equipment’. These are two separate errors.

Explanation / expected impact: Some differences fill previously unknown judgments; these change known-judgment coverage, not automatically correctness. Full-passage diagnostic conjunction: preparer=None, external=None. Human metrics remain unknown; human approvals remain zero.
