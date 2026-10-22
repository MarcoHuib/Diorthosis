# Ranked delivered defects and lost valid repairs



Mechanical ordering: delivered critical semantic label, then meaning failure, new errors, unnecessary change, withheld otherwise-valid necessary candidate. Labels and rationales are the submitted AI evidence; severity is not human adjudicated. Repeated conditions on the same source are paired occurrences, not independent observations.



## selection / p4-en-selection-055 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "If the test fails, leave the original file where it is."

Extracted: "If the test fails, leave the original file as it is."

Delivered: "If the test fails, leave the original file as it is."

Delivered assessment: ‘Where it is’ verlangt behoud van de fysieke/logische bestandslocatie; ‘as it is’ verlangt doorgaans behoud van de huidige staat of inhoud. Een bestand kan onveranderd maar verplaatst zijn. De testvoorwaarde blijft behouden, maar de uitvoeringsinstructie verandert wezenlijk. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: ‘Where it is’ verlangt behoud van de fysieke/logische bestandslocatie; ‘as it is’ verlangt doorgaans behoud van de huidige staat of inhoud. Een bestand kan onveranderd maar verplaatst zijn. De testvoorwaarde blijft behouden, maar de uitvoeringsinstructie verandert wezenlijk. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-en-selection-055 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "If the test fails, leave the original file where it is."

Extracted: "If the test fails, leave the original file as it is."

Delivered: "If the test fails, leave the original file as it is."

Delivered assessment: ‘Where it is’ verlangt behoud van de fysieke/logische bestandslocatie; ‘as it is’ verlangt doorgaans behoud van de huidige staat of inhoud. Een bestand kan onveranderd maar verplaatst zijn. De testvoorwaarde blijft behouden, maar de uitvoeringsinstructie verandert wezenlijk. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: ‘Where it is’ verlangt behoud van de fysieke/logische bestandslocatie; ‘as it is’ verlangt doorgaans behoud van de huidige staat of inhoud. Een bestand kan onveranderd maar verplaatst zijn. De testvoorwaarde blijft behouden, maar de uitvoeringsinstructie verandert wezenlijk. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-en-selection-067 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "The variables `naïve_mode` and `café_id` are intentionally distinct."

Extracted: "The variables `nãve_mode` and `café_id` are intentionally distinct."

Delivered: "The variables `nãve_mode` and `café_id` are intentionally distinct."

Delivered assessment: De oorspronkelijke identifier `naïve_mode` (i met trema, U+00EF) is veranderd in `nãve_mode` (a gevolgd door combining tilde U+0303). Dit is geen canoniek equivalente Unicode-normalisatie en verwijst naar een andere variabelenaam. De bron was correct; de tekst introduceert een kritieke inhoudelijke fout. Deze tekst heeft status geleverd, zodat eventuele gebreken daadwerkelijk in de opgeleverde passage staan.

Parsed assessment: De oorspronkelijke identifier `naïve_mode` (i met trema, U+00EF) is veranderd in `nãve_mode` (a gevolgd door combining tilde U+0303). Dit is geen canoniek equivalente Unicode-normalisatie en verwijst naar een andere variabelenaam. De bron was correct; de tekst introduceert een kritieke inhoudelijke fout. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Rejection: None



## confirmation / p4-nl-confirmation-013 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Extracted: "De patiënt vroeg de diëtist of huitkase op het menu mocht blijven."

Delivered: "De patiënt vroeg de diëtist of huitkase op het menu mocht blijven."

Delivered assessment: De correcte voedingsmiddelnaam «hüttenkäse» is vervangen door het niet-bestaande «huitkase». Hierdoor gaat de precieze referentie verloren in een gesprek tussen patiënt en diëtist. De bron was foutloos: een onnodige en materieel onjuiste wijziging. Deze tekst heeft status geleverd, zodat eventuele gebreken daadwerkelijk in de opgeleverde passage staan.

Parsed assessment: De correcte voedingsmiddelnaam «hüttenkäse» is vervangen door het niet-bestaande «huitkase». Hierdoor gaat de precieze referentie verloren in een gesprek tussen patiënt en diëtist. De bron was foutloos: een onnodige en materieel onjuiste wijziging. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Rejection: None



## confirmation / p4-nl-confirmation-013 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De patiënt vroeg de diëtist of hüttenkäse op het menu mocht blijven."

Extracted: "De patiënt vroeg de diëtist of hijttenkäse op het menu mocht blijven."

Delivered: "De patiënt vroeg de diëtist of hijttenkäse op het menu mocht blijven."

Delivered assessment: «hüttenkäse» is onterecht vervangen door «hijttenkäse», geen correcte benaming voor het voedingsmiddel. Het benoemde product wordt vervormd en er ontstaat een nieuwe spelfout in een verder correcte zin. Dit is geen acceptabele Nederlandse spellingvariant. Deze tekst heeft status geleverd, zodat eventuele gebreken daadwerkelijk in de opgeleverde passage staan.

Parsed assessment: «hüttenkäse» is onterecht vervangen door «hijttenkäse», geen correcte benaming voor het voedingsmiddel. Het benoemde product wordt vervormd en er ontstaat een nieuwe spelfout in een verder correcte zin. Dit is geen acceptabele Nederlandse spellingvariant. Deze parsed-kandidaat is afzonderlijk beoordeeld; dit is geen bewijs dat hij is afgeleverd.

Rejection: None



## selection / p4-nl-selection-007 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Extracted: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-007 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Extracted: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-007 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Extracted: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-007 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Extracted: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-007 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik vraag me af waarom wil ik steeds alles zelf doen."

Extracted: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered: "Ik vraag me af waarom ik steeds alles zelf doe."

Delivered assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: De bijzinvolgorde ‘waarom ik ...’ is hersteld, maar ‘wil doen’ (wens/zelfonderzoek over motivatie) wordt ‘doe’ (feitelijk gedrag). De werkwoordelijke modaliteit ‘wil’ verdwijnt. Ondanks correcte grammatica is de bedoelde boodschap niet bewaard. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een heksje hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een heksje hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘heksje’ (een kleine heks). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘heksje’ (een kleine heks). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een hemd hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een hemd hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemd’ (een overhemd of hemd). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemd’ (een overhemd of hemd). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-019 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De coordinator controleert of alle vrijwilligers een hesje hebben."

Extracted: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered: "De coordinator controleert of alle vrijwilligers een hemdje hebben."

Delivered assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: delivered; status: delivered.

Parsed assessment: Het oorspronkelijke ‘hesje’ is een geldig woord voor een vest en wordt onterecht vervangen door ‘hemdje’ (een onderhemdje of shirtje). Dit wijzigt het kleding-/veiligheidsvoorwerp wezenlijk. De echte spelfout ‘coordinator’ blijft bovendien staan in plaats van ‘coördinator’. Geen volledige reparatie en wel een nieuwe inhoudelijke fout. Individueel beoordeelde laag: parsed; status: parsed.

Rejection: None



## selection / p4-nl-selection-033 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de uitnoding staat zowel bienvenue als welkom."

Extracted: "Op de uitnoding staat zowel welkom als welkom."

Delivered: "Op de uitnoding staat zowel welkom als welkom."

Delivered assessment: “Uitnoding” remains misspelled, and replacing the French “bienvenue” with a second Dutch “welkom” erases the bilingual message and creates a nonsensical repetition. The meaning is changed even if the spelling mistake was independently fixed.

Parsed assessment: “Uitnoding” remains misspelled, and replacing the French “bienvenue” with a second Dutch “welkom” erases the bilingual message and creates a nonsensical repetition. The meaning is changed even if the spelling mistake was independently fixed.

Rejection: None



## selection / p4-nl-selection-033 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de uitnoding staat zowel bienvenue als welkom."

Extracted: "Op de uitnodiging staat zowel welkom als welkom."

Delivered: "Op de uitnodiging staat zowel welkom als welkom."

Delivered assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Parsed assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Rejection: None



## selection / p4-nl-selection-033 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de uitnoding staat zowel bienvenue als welkom."

Extracted: "Op de uitnodiging staat zowel welkom als welkom."

Delivered: "Op de uitnodiging staat zowel welkom als welkom."

Delivered assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Parsed assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Rejection: None



## selection / p4-nl-selection-033 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de uitnoding staat zowel bienvenue als welkom."

Extracted: "Op de uitnoding staat zowel welkom als welkom."

Delivered: "Op de uitnoding staat zowel welkom als welkom."

Delivered assessment: “Uitnoding” remains misspelled, and replacing the French “bienvenue” with a second Dutch “welkom” erases the bilingual message and creates a nonsensical repetition. The meaning is changed even if the spelling mistake was independently fixed.

Parsed assessment: “Uitnoding” remains misspelled, and replacing the French “bienvenue” with a second Dutch “welkom” erases the bilingual message and creates a nonsensical repetition. The meaning is changed even if the spelling mistake was independently fixed.

Rejection: None



## selection / p4-nl-selection-033 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de uitnoding staat zowel bienvenue als welkom."

Extracted: "Op de uitnodiging staat zowel welkom als welkom."

Delivered: "Op de uitnodiging staat zowel welkom als welkom."

Delivered assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Parsed assessment: “Uitnoding” is repaired as “uitnodiging”, but “bienvenue” is incorrectly replaced by a second “welkom”, erasing the explicit French/Dutch contrast and creating a redundant “zowel welkom als welkom”. The meaning is changed even if the spelling mistake was independently fixed.

Rejection: None



## selection / p4-nl-selection-050 / diorthosis-correction-v3

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb geest in frieten, maar de frituur is nog dicht."

Delivered: "Ik heb geest in frieten, maar de frituur is nog dicht."

Delivered assessment: The correct Belgian-Dutch noun "goesting" (appetite/craving) was replaced with "geest" (spirit/mind). "Ik heb geest in frieten" is an incoherent collocation, fundamentally losing the speaker's desire for fries; the second clause remains intact. This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: The correct Belgian-Dutch noun "goesting" (appetite/craving) was replaced with "geest" (spirit/mind). "Ik heb geest in frieten" is an incoherent collocation, fundamentally losing the speaker's desire for fries; the second clause remains intact. This parsed candidate must not be counted as a delivered repair.

Rejection: None



## selection / p4-nl-selection-050 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb een gevoel van misselijkheid bij frieten, maar de frituur is nog gesloten."

Delivered: "Ik heb een gevoel van misselijkheid bij frieten, maar de frituur is nog gesloten."

Delivered assessment: "Goesting in frieten" expresses desire for fries. Replacing it with "een gevoel van misselijkheid bij frieten" asserts nausea at the thought of fries, the opposite attitude. This is a severe introduced semantic failure; "nog gesloten" is grammatical but needlessly formalizes "nog dicht". This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: "Goesting in frieten" expresses desire for fries. Replacing it with "een gevoel van misselijkheid bij frieten" asserts nausea at the thought of fries, the opposite attitude. This is a severe introduced semantic failure; "nog gesloten" is grammatical but needlessly formalizes "nog dicht". This parsed candidate must not be counted as a delivered repair.

Rejection: None



## selection / p4-nl-selection-050 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb geest in frieten, maar de frituur is nog dicht."

Delivered: "Ik heb geest in frieten, maar de frituur is nog dicht."

Delivered assessment: The correct Belgian-Dutch noun "goesting" (appetite/craving) was replaced with "geest" (spirit/mind). "Ik heb geest in frieten" is an incoherent collocation, fundamentally losing the speaker's desire for fries; the second clause remains intact. This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: The correct Belgian-Dutch noun "goesting" (appetite/craving) was replaced with "geest" (spirit/mind). "Ik heb geest in frieten" is an incoherent collocation, fundamentally losing the speaker's desire for fries; the second clause remains intact. This parsed candidate must not be counted as a delivered repair.

Rejection: None



## selection / p4-nl-selection-059 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de kaart staan déjà vu, Straße en καλημέρα naast elkaar."

Extracted: "Op de kaart staan déjà vu, Straße en kaalméra naast elkaar."

Delivered: "Op de kaart staan déjà vu, Straße en kaalméra naast elkaar."

Delivered assessment: The Greek greeting "καλημέρα" is replaced by fabricated Latin-script "kaalméra", which is neither the same string nor a reliable transliteration. A Dutch spellchecker must preserve the source’s foreign-language example; this was a clean sentence. This is marked as delivered, so the candidate’s actual correctness or error matters for user-facing output.

Parsed assessment: The Greek greeting "καλημέρα" is replaced by fabricated Latin-script "kaalméra", which is neither the same string nor a reliable transliteration. A Dutch spellchecker must preserve the source’s foreign-language example; this was a clean sentence. This is a parsed candidate, not a claim that the text was delivered.

Rejection: None



## selection / p4-nl-selection-065 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=True, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Op 14 mei staat versie 2.4 op https://example.org/uitleg klaar."

Extracted: "Op 14 mei staat versie 2.4 van https://example.org/uitleg klaar."

Delivered: "Op 14 mei staat versie 2.4 van https://example.org/uitleg klaar."

Delivered assessment: Replacing “op https://example.org/uitleg” with “van https://example.org/uitleg” changes a location/availability relation into “version of a URL”. That is semantically different and unnatural; the original date, number and link text remain otherwise intact.

Parsed assessment: Replacing “op https://example.org/uitleg” with “van https://example.org/uitleg” changes a location/availability relation into “version of a URL”. That is semantically different and unnatural; the original date, number and link text remain otherwise intact.

Rejection: None



## confirmation / p4-nl-confirmation-018 / diorthosis-correction-v3

Disposition: accepted; delivered critical=None, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Het opschrift luidt hola, bonjour en γεια σας."

Extracted: "Het opschrift luidt hola, bonjour en γειά σας."

Delivered: "Het opschrift luidt hola, bonjour en γειά σας."

Delivered assessment: Fase delivered, status delivered; De bron meldt letterlijk welk opschrift er staat: “hola, bonjour en γεια σας”. De kandidaat verandert zonder bewijs het Griekse woord van “γεια” in “γειά”; de begroeting blijft inhoudelijk herkenbaar, maar de letterlijke transcriptie klopt niet meer met de opgegeven bron. Geen Nederlands brondefect vraagt om de wijziging. Kritieke ernst is zonder zicht op het opschrift onzeker.

Parsed assessment: Fase parsed, status parsed; De bron meldt letterlijk welk opschrift er staat: “hola, bonjour en γεια σας”. De kandidaat verandert zonder bewijs het Griekse woord van “γεια” in “γειά”; de begroeting blijft inhoudelijk herkenbaar, maar de letterlijke transcriptie klopt niet meer met de opgegeven bron. Geen Nederlands brondefect vraagt om de wijziging. Kritieke ernst is zonder zicht op het opschrift onzeker.

Rejection: None



## selection / p4-nl-selection-018 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=None, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "We hebben je aanvraag onvangen en sturen binnenkort een bevestiging."

Extracted: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-nl-selection-018 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=None, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "We hebben je aanvraag onvangen en sturen binnenkort een bevestiging."

Extracted: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-nl-selection-018 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=None, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "We hebben je aanvraag onvangen en sturen binnenkort een bevestiging."

Extracted: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered: "We hebben je aanvraag onontvangen en sturen binnenkort een bevestiging."

Delivered assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: In plaats van “onvangen” naar “ontvangen” te corrigeren, maakt de kandidaat er “onontvangen” van. Dit is een niet-bestaande/onbruikbare vorm en geen geslaagde spellingreparatie; het negatieve voorvoegsel kan bovendien de ontvangststatus omkeren. De overige zinsdelen zijn niet hersteld of gewijzigd. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-nl-selection-047 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Laat `build_id`, OAuth2 en UTF-8 precies zo staan."

Extracted: "Laat `build_id`, OAuth2 en UTF-8 **zo** staan."

Delivered: "Laat `build_id`, OAuth2 en UTF-8 **zo** staan."

Delivered assessment: Fase delivered, status delivered; “precies zo” uit de grammaticaal correcte bron wordt “**zo**”: “precies” is verwijderd, de eis van nauwkeurigheid afgezwakt en ongevraagd vette Markdown toegevoegd. Technische identifiers zijn wel ongewijzigd. Geen verantwoord taalherstel, wel betekenis- en opmaakverlies.

Parsed assessment: Fase parsed, status parsed; “precies zo” uit de grammaticaal correcte bron wordt “**zo**”: “precies” is verwijderd, de eis van nauwkeurigheid afgezwakt en ongevraagd vette Markdown toegevoegd. Technische identifiers zijn wel ongewijzigd. Geen verantwoord taalherstel, wel betekenis- en opmaakverlies.

Rejection: None



## selection / p4-nl-selection-047 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Laat `build_id`, OAuth2 en UTF-8 precies zo staan."

Extracted: "Laat `build_id`, OAuth2 en UTF-8 **zo** staan."

Delivered: "Laat `build_id`, OAuth2 en UTF-8 **zo** staan."

Delivered assessment: Fase delivered, status delivered; “precies zo” uit de grammaticaal correcte bron wordt “**zo**”: “precies” is verwijderd, de eis van nauwkeurigheid afgezwakt en ongevraagd vette Markdown toegevoegd. Technische identifiers zijn wel ongewijzigd. Geen verantwoord taalherstel, wel betekenis- en opmaakverlies.

Parsed assessment: Fase parsed, status parsed; “precies zo” uit de grammaticaal correcte bron wordt “**zo**”: “precies” is verwijderd, de eis van nauwkeurigheid afgezwakt en ongevraagd vette Markdown toegevoegd. Technische identifiers zijn wel ongewijzigd. Geen verantwoord taalherstel, wel betekenis- en opmaakverlies.

Rejection: None



## selection / p4-nl-selection-050 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb een gevoel van honger naar frieten, maar de frituur is nog gesloten."

Delivered: "Ik heb een gevoel van honger naar frieten, maar de frituur is nog gesloten."

Delivered assessment: The source is idiomatic Belgian Dutch; this over-expands "goesting" into the stilted "een gevoel van honger naar frieten". Hunger is not exactly the desire/appetite expressed by "goesting". "Nog gesloten" is valid Dutch but needlessly alters the informal register of "nog dicht"; the shop is still closed. This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: The source is idiomatic Belgian Dutch; this over-expands "goesting" into the stilted "een gevoel van honger naar frieten". Hunger is not exactly the desire/appetite expressed by "goesting". "Nog gesloten" is valid Dutch but needlessly alters the informal register of "nog dicht"; the shop is still closed. This parsed candidate must not be counted as a delivered repair.

Rejection: None



## selection / p4-nl-selection-051 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=True, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Doe de deur maar dicht, hoor; het tocht hier behoorlijk."

Extracted: "Doe de deur maar dicht, hoor; het tocht hier behoort behoorlijk."

Delivered: "Doe de deur maar dicht, hoor; het tocht hier behoort behoorlijk."

Delivered assessment: De bron is foutloos: “het tocht hier behoorlijk” is een correcte mededeling over sterke tocht. De invoeging “behoort” levert “het tocht hier behoort behoorlijk” op, een ongrammaticale en semantisch verstoorde combinatie van werkwoorden/bijwoorden. De juiste zin wordt slechter, niet beter. Het bevel om de deur te sluiten blijft herkenbaar, maar de reden klinkt niet meer als een geldige zin. Deze foutieve wijziging is werkelijk afgeleverd.

Parsed assessment: De bron is foutloos: “het tocht hier behoorlijk” is een correcte mededeling over sterke tocht. De invoeging “behoort” levert “het tocht hier behoort behoorlijk” op, een ongrammaticale en semantisch verstoorde combinatie van werkwoorden/bijwoorden. De juiste zin wordt slechter, niet beter. Het bevel om de deur te sluiten blijft herkenbaar, maar de reden klinkt niet meer als een geldige zin. Dit is een problematische parsed-kandidaat; geen afleverbewijs op zich.

Rejection: None



## selection / p4-nl-selection-043 / diorthosis-correction-v3

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De conciërge bespreekt de reünie met de coördinator in het café."

Extracted: "De concierge bespreekt de reünie met de coördinator in het café."

Delivered: "De concierge bespreekt de reünie met de coördinator in het café."

Delivered assessment: ‘conciërge’ verandert in ‘concierge’: de vereiste Nederlandse trema ontbreekt. De persoon en overige betekenis blijven begrijpelijk, maar de correcte spelling is beschadigd. De oorspronkelijke zin had geen te herstellen fouten. Deze tekst is daadwerkelijk geleverd; alleen deze versie kwalificeert voor gerealiseerde verbeteringen of behoud.

Parsed assessment: ‘conciërge’ verandert in ‘concierge’: de vereiste Nederlandse trema ontbreekt. De persoon en overige betekenis blijven begrijpelijk, maar de correcte spelling is beschadigd. De oorspronkelijke zin had geen te herstellen fouten. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: None



## selection / p4-nl-selection-043 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "De conciërge bespreekt de reünie met de coördinator in het café."

Extracted: "De concierge bespreekt de reünie met de coördinator in het café."

Delivered: "De concierge bespreekt de reünie met de coördinator in het café."

Delivered assessment: ‘conciërge’ verandert in ‘concierge’: de vereiste Nederlandse trema ontbreekt. De persoon en overige betekenis blijven begrijpelijk, maar de correcte spelling is beschadigd. De oorspronkelijke zin had geen te herstellen fouten. Deze tekst is daadwerkelijk geleverd; alleen deze versie kwalificeert voor gerealiseerde verbeteringen of behoud.

Parsed assessment: ‘conciërge’ verandert in ‘concierge’: de vereiste Nederlandse trema ontbreekt. De persoon en overige betekenis blijven begrijpelijk, maar de correcte spelling is beschadigd. De oorspronkelijke zin had geen te herstellen fouten. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: None



## selection / p4-nl-selection-050 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=True, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb honger op frieten, maar de frituur is nog dicht."

Delivered: "Ik heb honger op frieten, maar de frituur is nog dicht."

Delivered assessment: The original "goesting in frieten" is correct Belgian Dutch. "Honger op frieten" is not an idiomatic standard Dutch collocation ("honger hebben" / "zin in frieten"), and it shifts craving toward hunger. The general intent of wanting fries remains intelligible; the unchanged shop clause remains correct. This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: The original "goesting in frieten" is correct Belgian Dutch. "Honger op frieten" is not an idiomatic standard Dutch collocation ("honger hebben" / "zin in frieten"), and it shifts craving toward hunger. The general intent of wanting fries remains intelligible; the unchanged shop clause remains correct. This parsed candidate must not be counted as a delivered repair.

Rejection: None



## confirmation / p4-en-confirmation-011 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "We'll meet outside the café tommorrow morning."

Extracted: "We’ll meet outside the café tomorrow morning."

Delivered: "We'll meet outside the café tommorrow morning."

Delivered assessment: This delivered record is SOURCE FALLBACK after abstention, so the misspelled "tommorrow" remains. It cannot be credited as a repaired output or eligible clean preservation. The separately rejected alternatives correctly spell "tomorrow" and preserve meaning, making their rejection linguistically unwarranted on the supplied evidence.

Parsed assessment: Corrects "tommorrow" to "tomorrow". NFC-normalizing source "café" to "café" is valid Unicode normalization, not a lexical change. Curly contraction "We’ll" is acceptable English typography, equivalent to source straight-apostrophe "We'll". Morning rendezvous outside the same café is preserved. This parsed candidate is linguistically sound but not credited as delivered.

Rejection: Correction output changes a capitalized name or term.



## confirmation / p4-en-confirmation-011 / phase4-LiteralJson-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "We'll meet outside the café tommorrow morning."

Extracted: "We'll meet outside the café tomorrow morning."

Delivered: "We'll meet outside the café tommorrow morning."

Delivered assessment: This delivered record is SOURCE FALLBACK after abstention, so the misspelled "tommorrow" remains. It cannot be credited as a repaired output or eligible clean preservation. The separately rejected alternatives correctly spell "tomorrow" and preserve meaning, making their rejection linguistically unwarranted on the supplied evidence.

Parsed assessment: Corrects "tommorrow" to "tomorrow". NFC-normalizing source "café" to "café" is valid Unicode normalization, not a lexical change. Retains the valid English contraction "We'll" unchanged. Morning rendezvous outside the same café is preserved. This parsed candidate is linguistically sound but not credited as delivered.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-001 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-001 / phase4-LiteralJson-FewShot-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-001 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-001 / phase4-LiteralJson-Minimal-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-001 / phase4-LiteralJson-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-001 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The owner of the bicycles need a new lock."

Extracted: "The owner of the bicycles needs a new lock."

Delivered: "The owner of the bicycles need a new lock."

Delivered assessment: delivered source fallback after abstention: “The owner ... need” still lacks singular “needs”: plural “bicycles” is not the clause subject. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The singular head noun “owner” now correctly takes “needs”; “of the bicycles” does not control agreement. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-013 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "I'm interested on the history of the old harbour."

Extracted: "I’m interested in the history of the old harbour."

Delivered: "I'm interested on the history of the old harbour."

Delivered assessment: The English source uses the wrong preposition in “interested on”. Standard English requires “interested in” for a topic or subject. “The old harbour” is acceptable British spelling and should not be changed. The delivered source fallback repeats the incorrect “interested on” unchanged. It preserves the intended topic and introduces no new error, but leaves the preposition defect unrepaired. Abstaining on the valid candidate was inappropriate; this does NOT count as a successful or eligible correction. Status: delivered source fallback after abstention; not a model correction.

Parsed assessment: The English source uses the wrong preposition in “interested on”. Standard English requires “interested in” for a topic or subject. “The old harbour” is acceptable British spelling and should not be changed. Repairs the genuine preposition error (“interested on” → “interested in”) while also replacing the ASCII apostrophe in “I'm” with a U+2019 apostrophe. That is typographically optional, not a new language error or a meaning change. The candidate is linguistically fully corrected. Status: parsed candidate; do not count this as an independent model observation or as delivered output.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / phase4-LiteralJson-FewShot-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / phase4-LiteralJson-Minimal-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / phase4-LiteralJson-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-016 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Thanks for your mesasge; I'll answer after lunch."

Extracted: "Thanks for your message; I’ll answer after lunch."

Delivered: "Thanks for your mesasge; I'll answer after lunch."

Delivered assessment: De bron-fallback na afwijzing houdt de spelfout «mesasge» intact. Dit is geen gedane correctie en mag niet meetellen als reparatie, hoewel de bedoelde mededeling verder onveranderd blijft.

Parsed assessment: De parser herstelt «mesasge» naar «message». De omzetting van rechte «I'll» naar typografische «I’ll» is correct Engels en geen betekeniswijziging; het leesteken na «message» blijft terecht een puntkomma.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-020 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "He doesn't has access to the shared folder."

Extracted: "He doesn’t have access to the shared folder."

Delivered: "He doesn't has access to the shared folder."

Delivered assessment: The returned source still says 'doesn't has': after 'doesn't', 'have' is required. Unchanged fallback is not repair. This delivered fallback is an abstention, not a successful correction or rewarded clean preservation.

Parsed assessment: 'doesn't has' becomes 'doesn’t have'; the typographic curly apostrophe is acceptable English punctuation and changes neither person nor negation.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-020 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "He doesn't has access to the shared folder."

Extracted: "He doesn’t have access to the shared folder."

Delivered: "He doesn't has access to the shared folder."

Delivered assessment: The returned source still says 'doesn't has': after 'doesn't', 'have' is required. Unchanged fallback is not repair. This delivered fallback is an abstention, not a successful correction or rewarded clean preservation.

Parsed assessment: 'doesn't has' becomes 'doesn’t have'; the typographic curly apostrophe is acceptable English punctuation and changes neither person nor negation.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-020 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "He doesn't has access to the shared folder."

Extracted: "He doesn’t have access to the shared folder."

Delivered: "He doesn't has access to the shared folder."

Delivered assessment: The returned source still says 'doesn't has': after 'doesn't', 'have' is required. Unchanged fallback is not repair. This delivered fallback is an abstention, not a successful correction or rewarded clean preservation.

Parsed assessment: 'doesn't has' becomes 'doesn’t have'; the typographic curly apostrophe is acceptable English punctuation and changes neither person nor negation.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-021 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "Your welcome to borrow my spare helmet."

Extracted: "You’re welcome to borrow my spare helmet."

Delivered: "Your welcome to borrow my spare helmet."

Delivered assessment: delivered source fallback after abstention: “Your welcome” remains incorrect; the predicate requires “You’re/You're”, not possessive “Your”. A source fallback after abstention is not a successful repair or clean preservation.

Parsed assessment: parsed/passage: The possessive “Your” becomes the grammatically required “You’re” (a valid curly apostrophe variant of “You're”). Permission to borrow the helmet is unchanged. No remaining errors, new errors, or relevant meaning shift.

Rejection: Correction output changes a capitalized name or term.



## selection / p4-en-selection-029 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-029 / phase4-LiteralJson-FewShot-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-029 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-029 / phase4-LiteralJson-Minimal-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-029 / phase4-LiteralJson-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-029 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The backup don't contain any customer records."

Extracted: "The backup does not contain any customer records."

Delivered: "The backup don't contain any customer records."

Delivered assessment: The subject–auxiliary disagreement remains: singular ‘The backup’ cannot take ‘don't’ in standard English here. Valid corrections include ‘does not contain’ and ‘doesn't contain’. The negative meaning is preserved, but the source defect was not corrected. Deze geleverde tekst is uitsluitend bronterugval na afwijzing/onderdrukking; dit is abstentie en telt NIET als geslaagde correctie of als in aanmerking komend schoon behoud. De bijbehorende geldige grammaticaverbetering had niet geweigerd hoeven worden.

Parsed assessment: ‘The backup’ is third-person singular; ‘does not contain’ repairs the incorrect ‘don't contain’. The explicit negative remains, ‘any customer records’ is preserved, and the expanded form ‘does not’ is fully valid even if an annotation proposes ‘doesn't’. Dit is een geparseerde kandidaat; dit feit alleen bewijst geen levering.

Rejection: Correction output changes negation or modality and requires review.



## selection / p4-en-selection-035 / diorthosis-correction-v3

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The draft is attached.\nWe can discuss the costs tomorow."

Extracted: "The draft is attached.\nWe can discuss the costs tomorrow."

Delivered: "The draft is attached.\nWe can discuss the costs tomorow."

Delivered assessment: The misspelling “tomorow” persists. This is a source fallback after a rejected candidate, not a successful repair, regardless of any corrected raw or parsed variant. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Parsed assessment: The correct English spelling is “tomorrow”; the source has “tomorow”. The candidate repairs the sole detected source defect, introduces no new error, and retains agents, tense, conditions, wording and layout. This is extracted/parsed output, not proof of delivery.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-035 / phase4-LiteralJson-FewShot-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The draft is attached.\nWe can discuss the costs tomorow."

Extracted: "The draft is attached.\nWe can discuss the costs tomorrow."

Delivered: "The draft is attached.\nWe can discuss the costs tomorow."

Delivered assessment: The misspelling “tomorow” persists. This is a source fallback after a rejected candidate, not a successful repair, regardless of any corrected raw or parsed variant. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Parsed assessment: The correct English spelling is “tomorrow”; the source has “tomorow”. The candidate repairs the sole detected source defect, introduces no new error, and retains agents, tense, conditions, wording and layout. This is extracted/parsed output, not proof of delivery.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-035 / phase4-LiteralJson-Minimal-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The draft is attached.\nWe can discuss the costs tomorow."

Extracted: "The draft is attached.\nWe can discuss the costs tomorrow."

Delivered: "The draft is attached.\nWe can discuss the costs tomorow."

Delivered assessment: The misspelling “tomorow” persists. This is a source fallback after a rejected candidate, not a successful repair, regardless of any corrected raw or parsed variant. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Parsed assessment: The correct English spelling is “tomorrow”; the source has “tomorow”. The candidate repairs the sole detected source defect, introduces no new error, and retains agents, tense, conditions, wording and layout. This is extracted/parsed output, not proof of delivery.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-035 / phase4-LiteralJson-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The draft is attached.\nWe can discuss the costs tomorow."

Extracted: "The draft is attached.\nWe can discuss the costs tomorrow."

Delivered: "The draft is attached.\nWe can discuss the costs tomorow."

Delivered assessment: The misspelling “tomorow” persists. This is a source fallback after a rejected candidate, not a successful repair, regardless of any corrected raw or parsed variant. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Parsed assessment: The correct English spelling is “tomorrow”; the source has “tomorow”. The candidate repairs the sole detected source defect, introduces no new error, and retains agents, tense, conditions, wording and layout. This is extracted/parsed output, not proof of delivery.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-035 / phase4-PlainText-Original-1.0.0

Disposition: rejected; delivered critical=False, meaning failure=False, new errors=False, unnecessary=False, valid necessary repair withheld=True.

Original: "The draft is attached.\nWe can discuss the costs tomorow."

Extracted: "The draft is attached.\nWe can discuss the costs tomorrow."

Delivered: "The draft is attached.\nWe can discuss the costs tomorow."

Delivered assessment: The misspelling “tomorow” persists. This is a source fallback after a rejected candidate, not a successful repair, regardless of any corrected raw or parsed variant. Output metadata identifies source fallback after abstention; it must not count as an eligible successful edit or clean preservation.

Parsed assessment: The correct English spelling is “tomorrow”; the source has “tomorow”. The candidate repairs the sole detected source defect, introduces no new error, and retains agents, tense, conditions, wording and layout. This is extracted/parsed output, not proof of delivery.

Rejection: Correction output changes protected temporal, relational, or personal-agent anchors.



## selection / p4-en-selection-048 / diorthosis-correction-v3

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Keep OAuth2, UTF-8 and `user_ID` unchanged in the documentation."

Extracted: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Parsed assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Rejection: None



## selection / p4-en-selection-048 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Keep OAuth2, UTF-8 and `user_ID` unchanged in the documentation."

Extracted: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Parsed assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Rejection: None



## selection / p4-en-selection-048 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Keep OAuth2, UTF-8 and `user_ID` unchanged in the documentation."

Extracted: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Parsed assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Rejection: None



## selection / p4-en-selection-048 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Keep OAuth2, UTF-8 and `user_ID` unchanged in the documentation."

Extracted: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Parsed assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Rejection: None



## selection / p4-en-selection-048 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Keep OAuth2, UTF-8 and `user_ID` unchanged in the documentation."

Extracted: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered: "Keep OAuth2, UTF-8, and `user_ID` unchanged in the documentation."

Delivered assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Parsed assessment: The extra serial/Oxford comma after UTF-8 is an acceptable American-English punctuation variant, but the source list already was correct. OAuth2, UTF-8, `user_ID`, and the imperative meaning remain exact. This optional edit was not necessary.

Rejection: None



## selection / p4-en-selection-053 / diorthosis-correction-v3

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "I didn't say that the supplier had agreed to replace the unit."

Extracted: "I didn’t say that the supplier had agreed to replace the unit."

Delivered: "I didn’t say that the supplier had agreed to replace the unit."

Delivered assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-en-selection-053 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "I didn't say that the supplier had agreed to replace the unit."

Extracted: "I didn’t say that the supplier had agreed to replace the unit."

Delivered: "I didn’t say that the supplier had agreed to replace the unit."

Delivered assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-en-selection-053 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "I didn't say that the supplier had agreed to replace the unit."

Extracted: "I didn’t say that the supplier had agreed to replace the unit."

Delivered: "I didn’t say that the supplier had agreed to replace the unit."

Delivered assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-en-selection-053 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "I didn't say that the supplier had agreed to replace the unit."

Extracted: "I didn’t say that the supplier had agreed to replace the unit."

Delivered: "I didn’t say that the supplier had agreed to replace the unit."

Delivered assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: delivered; disposition: accepted; afgeleverd-toegestaan: True].

Parsed assessment: De bron is al juist. Alleen de typografische apostrof is gewijzigd: “didn't” → “didn’t”. Beide samentrekkingen zijn geldig Engels, en de ontkenning blijft op “say” gericht; de tijdsvorm “had agreed” verandert niet. Dit is een geldige typografische variant, geen noodzakelijke taalkundige reparatie. [Beoordelingslaag: parsed; disposition: geen; afgeleverd-toegestaan: False].

Rejection: None



## selection / p4-nl-selection-045 / diorthosis-correction-v3

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-045 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-045 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-045 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-045 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-045 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "De expositie in het café is deze week gesloten."

Extracted: "De expositie in het café is deze week gesloten."

Delivered: "De expositie in het café is deze week gesloten."

Delivered assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Deze onschadelijke normalisatie is afgeleverd, zonder dat dit als taalreparatie geldt.

Parsed assessment: De tekst verandert uitsluitend van decomposed “cafe” + U+0301 naar NFC-“café” (U+00E9). Beide zijn canoniek gelijkwaardig en orthografisch correct: geen spelfout is verholpen. Er is dus geen taalkundige noodzaak voor een wijziging, hoewel Unicode-normalisatie als technische conventie aanvaardbaar en onschadelijk is. Locatie, week en sluitingsmededeling blijven gelijk. Dit is een afzonderlijke parsed-kandidaat, niet zelfstandig een afgeleverde correctie.

Rejection: None



## selection / p4-nl-selection-046 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=None, unnecessary=True, valid necessary repair withheld=False.

Original: "De kinderen riepen: \"Hoera! 🎉\" Daarna ging iedereen naar huis."

Extracted: "De kinderen riepen: *\"Hoera! 🎉\"* Daarna ging iedereen naar huis."

Delivered: "De kinderen riepen: *\"Hoera! 🎉\"* Daarna ging iedereen naar huis."

Delivered assessment: The text, original double quotes, celebratory emoji 🎉, and Dutch sentence meaning are otherwise identical. Adding * around the direct quotation introduces an unrequested italic/Markdown emphasis, not a grammar correction. It preserves content but fails exact clean-text preservation; a new formatting defect is uncertain without knowing the rendering target. Output metadata marks this candidate delivered; assess its actual linguistic quality rather than delivery metadata.

Parsed assessment: The text, original double quotes, celebratory emoji 🎉, and Dutch sentence meaning are otherwise identical. Adding * around the direct quotation introduces an unrequested italic/Markdown emphasis, not a grammar correction. It preserves content but fails exact clean-text preservation; a new formatting defect is uncertain without knowing the rendering target. This is extracted/parsed output, not proof of delivery.

Rejection: None



## selection / p4-nl-selection-050 / phase4-LiteralJson-FewShot-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=None, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik heb goesting in frieten, maar de frituur is nog dicht."

Extracted: "Ik heb zin op frieten, maar de frituur is nog dicht."

Delivered: "Ik heb zin op frieten, maar de frituur is nog dicht."

Delivered assessment: The original "goesting in frieten" is already correct and regionally appropriate. Standard Dutch usually uses "zin in frieten", not "zin op frieten". The meaning remains a desire for fries, but the status of "zin op" as an acceptable informal Belgian-Dutch variant is uncertain, so noNewErrors is left null rather than automatically classifying it as wrong. This was delivered as an accepted candidate despite the identified defect or needless change.

Parsed assessment: The original "goesting in frieten" is already correct and regionally appropriate. Standard Dutch usually uses "zin in frieten", not "zin op frieten". The meaning remains a desire for fries, but the status of "zin op" as an acceptable informal Belgian-Dutch variant is uncertain, so noNewErrors is left null rather than automatically classifying it as wrong. This parsed candidate must not be counted as a delivered repair.

Rejection: None



## selection / p4-nl-selection-052 / diorthosis-correction-v3

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik denk dat zij de rekening gisteren betaald heeft."

Extracted: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Parsed assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Rejection: None



## selection / p4-nl-selection-052 / phase4-LiteralJson-Minimal-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik denk dat zij de rekening gisteren betaald heeft."

Extracted: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Parsed assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Rejection: None



## selection / p4-nl-selection-052 / phase4-LiteralJson-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik denk dat zij de rekening gisteren betaald heeft."

Extracted: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Parsed assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Rejection: None



## selection / p4-nl-selection-052 / phase4-PlainText-Original-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=False, unnecessary=True, valid necessary repair withheld=False.

Original: "Ik denk dat zij de rekening gisteren betaald heeft."

Extracted: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered: "Ik denk dat zij de rekening gisteren heeft betaald."

Delivered assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Parsed assessment: De brontekst is grammaticaal correct. In de Nederlandse bijzin is de eindgroep „betaald heeft” geldig; „heeft betaald” is eveneens mogelijk. „Gisteren” is goed geplaatst. Geen spel- of betekenisfouten. De kandidaat verwisselt uitsluitend de toegelaten werkwoordsvolgorde „betaald heeft” naar „heeft betaald”. Beide zijn correct Nederlands met dezelfde voltooide tijd en betekenis. Geen nieuwe taalfout, maar de oorspronkelijke zin behoefde geen correctie; daarom is de verandering overbodig.

Rejection: None



## selection / p4-nl-selection-059 / phase4-LiteralJson-LanguageSpecific-1.0.0

Disposition: accepted; delivered critical=False, meaning failure=False, new errors=None, unnecessary=True, valid necessary repair withheld=False.

Original: "Op de kaart staan déjà vu, Straße en καλημέρα naast elkaar."

Extracted: "Op de kaart staan *déjà vu*, Straße en καλημέρα naast elkaar."

Delivered: "Op de kaart staan *déjà vu*, Straße en καλημέρα naast elkaar."

Delivered assessment: Only literal Markdown asterisks around "déjà vu" are added. Original French, German and Greek text is retained exactly and meaning is unchanged. The clean source did not ask for emphasis; whether this becomes visible extraneous stars or legitimate italics depends on the output renderer. This is marked as delivered, so the candidate’s actual correctness or error matters for user-facing output.

Parsed assessment: Only literal Markdown asterisks around "déjà vu" are added. Original French, German and Greek text is retained exactly and meaning is unchanged. The clean source did not ask for emphasis; whether this becomes visible extraneous stars or legitimate italics depends on the output renderer. This is a parsed candidate, not a claim that the text was delivered.

Rejection: None
