# Bounded format experiment results

Larger local models did not rescue direct model-generated offsets here. Keep production v3 available; do not promote an experimental protocol or select a model from this panel.

| Model | Protocol | Natural EOG | Valid JSON | Assessable delivery | NL reference repairs | EN reference repairs | Mean ms | Sampled peak GiB |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| ministral-3b | baseline | 8/8 | 0/8 | 8/8 | 2/2 | 2/3 | 1431 | 2.73 |
| ministral-3b | offsets | 8/8 | 8/8 | 0/8 | 0/2 | 0/3 | 2722 | 2.84 |
| ministral-14b | baseline | 8/8 | 0/8 | 7/8 | 2/2 | 2/3 | 5179 | 8.86 |
| ministral-14b | offsets | 8/8 | 8/8 | 6/8 | 0/2 | 0/3 | 6973 | 8.98 |
| eurollm-9b | baseline | 8/8 | 1/8 | 8/8 | 2/2 | 2/3 | 3830 | 6.25 |
| eurollm-9b | offsets | 6/8 | 6/8 | 0/8 | 0/2 | 0/3 | 11806 | 6.39 |

All48 generations were actually run on already local pinned assets, CPU-only under OS network denial. Complete model/native/prompt/dataset/runtime/settings and raw traces are adjacent. JSON is not the baseline output protocol, so its JSON count is not a baseline failure metric.

3B offsets:8/8 rejected despite natural EOG and valid JSON.14B offsets:6 unchanged empty arrays and2 rejected candidates; no reference repairs. EuroLLM9B offsets:8 rejected, including2 maximum-token incomplete outputs;6 valid JSON responses also fail delivery. Bigger models did not establish usable offsets.

Important reference ambiguity: baseline en-04 uses a grammatical initial Tomorrow alternative not accepted by the old whole-sentence reference;4/5 total reference repairs does not prove a remaining linguistic defect. Conversely3B changes correct Dutch start to startt in nl-17 and the validator accepts that typo;14B tries OAuth2→OAuth 2 in nl-21 and is correctly rejected by identifier protection. These are explicit AI/mechanical observations, not human precision measurements.

Memory sampling covers load and inference at100ms and can miss short peaks. OS cache and competing development processes were not isolated; figures are diagnostic, not speed superiority or minimum-device requirements. No quality target, production change or model selection is justified.
