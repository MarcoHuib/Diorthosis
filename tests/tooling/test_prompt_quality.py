import hashlib
import json
from pathlib import Path
import unittest
ROOT=Path(__file__).resolve().parents[2]
BASE=ROOT/'benchmarks/phase4/v1'
class PanelTests(unittest.TestCase):
    def test_frozen_identity(self):
        manifest=json.loads((BASE/'freeze.json').read_text())
        for name,digest in manifest['files'].items():
            self.assertEqual(digest,hashlib.sha256((BASE/name).read_bytes()).hexdigest(),name)
    def test_disjoint_splits_and_annotations(self):
        seen=set()
        for split,count,clean in [('development',10,4),('selection',70,28),('confirmation',20,8)]:
            rows=[json.loads(s) for s in (BASE/(split+'.jsonl')).read_text().splitlines()]
            for language in ('dutch','english'):
                selected=[r for r in rows if r['language']==language]
                self.assertEqual(count,len(selected));self.assertEqual(clean,sum(not r['errors'] for r in selected))
            for r in rows:
                self.assertNotIn(r['inputText'],seen);seen.add(r['inputText'])
                source=r['inputText'].encode('utf-16-le')
                for e in r['errors']:
                    self.assertEqual(e['originalText'],source[e['start']*2:(e['start']+e['length'])*2].decode('utf-16-le'))
                self.assertIn('AI-generated',r['provenance']['labelStatus'])
        self.assertEqual(200,len(seen))
    def test_prompt_examples_are_development_only(self):
        source=(ROOT/'src/Diorthosis.Experiments/PromptQuality.cs').read_text()
        development=[json.loads(s) for s in (BASE/'development.jsonl').read_text().splitlines()]
        examples=[r['inputText'] for r in development if r['id'].endswith(('-001','-002','-007','-008'))]
        self.assertEqual(8,len(examples))
        for example in examples:self.assertIn(example,source)
if __name__=='__main__':unittest.main()
