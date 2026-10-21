import importlib
import json
from pathlib import Path
import sys
import tempfile
import unittest

ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'scripts'))
analysis=importlib.import_module('analyze-external-linguistic-review')
impact=importlib.import_module('summarize-linguistic-impact')
EVIDENCE=ROOT/'benchmarks/phase3/external-gpt6/v1'

class ExternalReviewAnalysisTests(unittest.TestCase):
    def test_all_comparisons_and_layer_denominators_reproduce(self):
        with tempfile.TemporaryDirectory() as temp:
            out=Path(temp)/'analysis';analysis.analyze(EVIDENCE,out)
            for name in ['comparison.jsonl','disagreements.md','metrics.json','summary.json']:
                self.assertEqual((EVIDENCE/'analysis'/name).read_bytes(),(out/name).read_bytes(),name)
        summary=json.loads((EVIDENCE/'analysis/summary.json').read_text())
        self.assertEqual(119,summary['totalSubjects'])
        self.assertEqual([26,48,18],[summary['failures'][f]['subjects'] for f in ['meaningPreserved','noNewErrors','criticalSemanticFailure']])
        self.assertEqual(3,summary['failures']['criticalSemanticFailure']['deliveredUniqueCandidates'])
        self.assertEqual(0,summary['humanApprovals'])

    def test_paired_projection_retains_original_bindings_and_no_extra_gains(self):
        with tempfile.TemporaryDirectory() as temp:
            out=Path(temp)/'paired';impact.summarize(EVIDENCE,out)
            for name in ['case-ledger.jsonl','summary.json']:
                self.assertEqual((EVIDENCE/'paired-analysis'/name).read_bytes(),(out/name).read_bytes(),name)
        rows=analysis.c.lines(EVIDENCE/'paired-analysis/case-ledger.jsonl')
        changed=[r for r in rows if r['deliveredChanged']]
        self.assertEqual(1,len(changed));self.assertEqual('p2-en-article-error',changed[0]['caseId'])
        self.assertNotEqual(changed[0]['before']['originalDecisionSubjectId'],changed[0]['after']['originalDecisionSubjectId'])
        self.assertFalse(any(r['dispositionChanged'] for r in rows))
        self.assertEqual(120,len(rows))
        self.assertTrue(all(r['after']['judgment'] is not None for r in rows if r['after']['deliveryEligible']))

    def test_unknown_judgments_are_not_successes_and_rejections_not_delivered(self):
        self.assertEqual(dict(numerator=1,denominator=2,unknown=1,total=3,estimate=.5),analysis.measure([True,False,None]))
        self.assertIsNone(analysis.conjunction(dict(justified=True,meaningPreserved=None,allDefectsRepaired=True,noNewErrors=True)))
        for row in analysis.c.lines(EVIDENCE/'paired-analysis/case-ledger.jsonl'):
            if not row['after']['deliveryEligible']:
                self.assertIsNone(row['after']['judgment'])
                self.assertEqual(row['source'],row['afterDelivered'])
