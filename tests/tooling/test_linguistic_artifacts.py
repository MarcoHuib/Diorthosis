import importlib.util
import json
from pathlib import Path
import sys
import tempfile
import unittest

ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'scripts'))
spec=importlib.util.spec_from_file_location('phase3_review',ROOT/'scripts/build-phase3-review.py')
review=importlib.util.module_from_spec(spec);spec.loader.exec_module(review)

class LinguisticArtifactTests(unittest.TestCase):
    def fixture(self,root):
        panel=root/'panel.jsonl';run=root/'run';run.mkdir()
        case=dict(id='private-looking-original-id',language='English',inputText='Literal <script> data.',errors=[],
            provenance=dict(kind='original AI-authored fictional passage',rights='No copied external or private text.'))
        panel.write_text(json.dumps(case)+'\n')
        (run/'manifest.json').write_text(json.dumps(dict(state='complete',datasetSha256=review.common.sha(panel))))
        rows=[dict(id=case['id'],arm=arm,analysis=dict(disposition='unchanged',edits=[],extractedCorrection=case['inputText']),
            **({} if arm=='voting-3' else dict(inference=dict(text=case['inputText'])))) for arm in review.ARMS]
        (run/'trace.jsonl').write_text(''.join(json.dumps(r)+'\n' for r in rows))
        return panel,run

    def test_export_is_deterministic_blinded_and_html_escaped(self):
        with tempfile.TemporaryDirectory() as d:
            root=Path(d);panel,run=self.fixture(root)
            review.build(panel,run,root/'a');review.build(panel,run,root/'b')
            for p in (root/'a').iterdir():
                self.assertEqual(p.read_bytes(),(root/'b'/p.name).read_bytes())
            public=(root/'a/subjects.jsonl').read_text()+(root/'a/review.html').read_text()
            self.assertNotIn('private-looking-original-id',public)
            self.assertNotIn('ministral',public)
            self.assertNotIn('<script>',(root/'a/review.html').read_text())
            self.assertIn('&lt;script&gt;',(root/'a/review.html').read_text())
            self.assertIn('private-looking-original-id',(root/'a/lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text())
            with self.assertRaises(FileExistsError):review.build(panel,run,root/'a')

    def test_private_or_stale_source_cannot_be_exported(self):
        with tempfile.TemporaryDirectory() as d:
            root=Path(d);panel,run=self.fixture(root)
            obj=json.loads(panel.read_text());obj['provenance']['kind']='private document'
            panel.write_text(json.dumps(obj)+'\n')
            with self.assertRaises(ValueError):review.build(panel,run,root/'out')
            (run/'manifest.json').write_text(json.dumps(dict(state='complete',datasetSha256=review.common.sha(panel))))
            with self.assertRaises(ValueError):review.build(panel,run,root/'out')
            self.assertFalse((root/'out').exists())

if __name__=='__main__':unittest.main()
