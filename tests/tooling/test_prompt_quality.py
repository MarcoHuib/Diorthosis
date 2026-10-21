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

class ReviewImportTests(unittest.TestCase):
    def setUp(self):
        import importlib
        import sys
        import tempfile
        sys.path.insert(0,str(ROOT/'scripts'))
        self.common=importlib.import_module('phase2-analysis')
        self.importer=importlib.import_module('import-prompt-quality-review')
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);self.root=Path(self.tmp.name)
        case=dict(id='case-001',language='dutch',inputText='Dit is correct.',errors=[],sourceFamily='fictional-1',documentId=None)
        self.subject=self.common.subject(case,'delivered','passage',case['inputText'],[],dict(deliveryEligible=True))
        self.common.write_lines(self.root/'subjects.jsonl',[self.subject])
        self.row=dict(subjectId=self.subject['subjectId'],bindingSha256=self.subject['bindingSha256'],reviewerId='Test AI',reviewerKind='ai',language='dutch',
            qualifications='AI test fixture only',independenceDeclared=False,version=1,
            judgment=dict(justified=True,meaningPreserved=True,allDefectsRepaired=True,noNewErrors=True,cleanPreserved=True,criticalSemanticFailure=False,
                rejectionAppropriate=None,acceptedAlternatives=[],errorCategories=[],ambiguity='none in fixture',rationale='Fictional import-validation fixture.'))
        self.common.write_lines(self.root/'decisions.jsonl',[self.row])
        self.provenance=dict(reviewerId='Test AI',reviewerKind='ai',independenceDeclared=False,modelBuild='test fixture',evaluationDate='2026-10-21',priorExposure='test fixture',
            reviewLimitations='test fixture',reviewRequest='test only',reviewRequestSha256=self.common.digest('test only'),subjectsSha256=self.common.sha(self.root/'subjects.jsonl'),
            decisionsSha256=self.common.sha(self.root/'decisions.jsonl'),subjectCount=1,decisionCount=1,completeCoverage=True)
        self.common.write(self.root/'provenance.json',self.provenance)
    def validate(self):return self.importer.validate(self.root,self.root/'decisions.jsonl',self.root/'provenance.json')
    def rewrite(self,rows):
        self.common.write_lines(self.root/'decisions.jsonl',rows);self.provenance['decisionsSha256']=self.common.sha(self.root/'decisions.jsonl')
        self.common.write(self.root/'provenance.json',self.provenance)
    def test_valid_bound_ai_import(self):
        result=self.validate();self.assertEqual(1,result['decisions']);self.assertEqual(0,result['humanApprovals'])
    def test_duplicate_and_missing_coverage_rejected(self):
        self.rewrite([self.row,self.row])
        with self.assertRaisesRegex(ValueError,'Duplicate'):self.validate()
        self.rewrite([])
        with self.assertRaisesRegex(ValueError,'coverage'):self.validate()
    def test_binding_and_types_rejected(self):
        self.row['bindingSha256']='0'*64;self.rewrite([self.row])
        with self.assertRaisesRegex(ValueError,'Unbound'):self.validate()
        self.row['bindingSha256']=self.subject['bindingSha256'];self.row['judgment']['meaningPreserved']='true';self.rewrite([self.row])
        with self.assertRaisesRegex(ValueError,'booleans'):self.validate()
    def test_human_or_independence_fabrication_rejected(self):
        self.row['reviewerKind']='human-declared';self.rewrite([self.row])
        with self.assertRaisesRegex(ValueError,'Identity'):self.validate()
        self.row['reviewerKind']='ai';self.row['independenceDeclared']=True;self.rewrite([self.row])
        with self.assertRaisesRegex(ValueError,'Identity'):self.validate()
    def test_subject_tamper_rejected(self):
        self.subject['candidateText']='Andere tekst.';self.common.write_lines(self.root/'subjects.jsonl',[self.subject])
        self.provenance['subjectsSha256']=self.common.sha(self.root/'subjects.jsonl');self.common.write(self.root/'provenance.json',self.provenance)
        with self.assertRaisesRegex(ValueError,'Stale'):self.validate()

class PairingAndBlindingTests(unittest.TestCase):
    def setUp(self):
        import importlib
        import sys
        import tempfile
        sys.path.insert(0,str(ROOT/'scripts'))
        self.c=importlib.import_module('phase2-analysis');self.a=importlib.import_module('analyze-prompt-quality');self.b=importlib.import_module('build-prompt-quality-review')
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);self.root=Path(self.tmp.name);self.run=self.root/'run';self.run.mkdir()
        self.case=dict(id='p4-test-only',language='english',inputText='Keep it.',errors=[],sourceFamily='fictional-test',documentId=None,
            provenance=dict(kind='original AI-authored fictional passage',rights='Test only; no copied external or private text'))
        self.c.write_lines(self.root/'panel.jsonl',[self.case])
        self.manifest=dict(state='complete',datasetSha256=self.c.sha(self.root/'panel.jsonl'),provenance=dict(modelSha256='9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8'),
            settings={},runtime='test',nativeHashes={},assemblyHashes={},arms=[dict(promptVersion='condition-secret')],attempts=1)
        self.row=dict(id='p4-test-only',language='english',input='Keep it.',promptVersion='condition-secret',prompt='Never exported secret prompt',promptSha256=self.c.digest('Never exported secret prompt'),
            inference=dict(text='Keep it.'),generationTermination='EndOfGeneration',analysis=dict(disposition='unchanged',edits=[],extractedCorrection='Keep it.'),delivered='Keep it.')
        self.save()
    def save(self):
        self.c.write(self.run/'manifest.json',self.manifest);self.c.write_lines(self.run/'trace.jsonl',[self.row])
    def test_complete_paired_binding(self):
        cases,rows=self.a.load(self.root/'panel.jsonl',[self.run]);self.assertEqual(1,len(rows));self.assertEqual(cases[0],self.case)
    def test_missing_attempt_or_tamper_rejected(self):
        self.manifest['attempts']=2;self.save()
        with self.assertRaisesRegex(ValueError,'coverage'):self.a.load(self.root/'panel.jsonl',[self.run])
        self.manifest['attempts']=1;self.row['delivered']='Changed.';self.save()
        with self.assertRaisesRegex(ValueError,'Delivery'):self.a.load(self.root/'panel.jsonl',[self.run])
    def test_zip_withholds_prompt_condition_and_lineage(self):
        import zipfile
        out=self.root/'review';self.b.build([self.root/'panel.jsonl'],[[self.run]],out)
        with zipfile.ZipFile(out/'chatgpt-review.zip') as z:
            self.assertEqual({'subjects.jsonl','review.html','CHATGPT-REVIEW.md','packet-manifest.json'},set(z.namelist()))
            content=b''.join(z.read(n) for n in z.namelist())
            self.assertNotIn(b'condition-secret',content);self.assertNotIn(b'Never exported secret prompt',content);self.assertNotIn(b'p4-test-only',content)
        self.assertIn('condition-secret',(out/'lineage-DO-NOT-SEND-TO-REVIEWERS.json').read_text())
