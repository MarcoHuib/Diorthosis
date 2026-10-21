import importlib.util
import json
from pathlib import Path
import stat
import sys
import tempfile
import unittest
import zipfile

ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'scripts'))
spec=importlib.util.spec_from_file_location('external_import',ROOT/'scripts/import-external-linguistic-review.py')
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
ORIGINAL=ROOT/'benchmarks/phase3/external-gpt6/v1/original/diorthosis-ai-review-package.zip'

class ExternalReviewImportTests(unittest.TestCase):
    def mutate(self,change):
        with zipfile.ZipFile(ORIGINAL) as z:files={n:z.read(n) for n in z.namelist()}
        change(files)
        if 'diorthosis-ai-review.jsonl' in files:
            p=json.loads(files['diorthosis-ai-review-provenance.json'])
            p['outputSha256']=module.digest(files['diorthosis-ai-review.jsonl'])
            files['diorthosis-ai-review-provenance.json']=json.dumps(p).encode()
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/'mutated.zip'
            with zipfile.ZipFile(path,'w') as z:
                for name,data in files.items():z.writestr(name,data)
            with self.assertRaises((ValueError,KeyError)):module.validate(path)

    def test_original_is_complete_bound_and_nonhuman(self):
        files,result=module.validate(ORIGINAL)
        self.assertEqual(119,result['decisions']);self.assertEqual(0,result['humanApprovals'])
        self.assertEqual(b'',files['empty-adjudications.jsonl'])

    def test_missing_duplicate_unbound_language_and_version_fail(self):
        def change(index,field=None,value=None):
            def mutate(files):
                rows=files['diorthosis-ai-review.jsonl'].decode().splitlines()
                if index=='missing':rows.pop()
                elif index=='duplicate':rows[-1]=rows[0]
                else:
                    r=json.loads(rows[0]);r[field]=value;rows[0]=json.dumps(r)
                files['diorthosis-ai-review.jsonl']=('\n'.join(rows)+'\n').encode()
            return mutate
        for args in [('missing',),('duplicate',),(0,'subjectId','review-unbound'),(0,'language','dutch'),(0,'version',True),(0,'version',0),(0,'reviewerKind','human-declared'),(0,'independenceDeclared',True)]:
            with self.subTest(args=args):self.mutate(change(*args))

    def test_schema_types_and_duplicate_json_properties_fail(self):
        for mode in ['extra','missing','boolean-number','duplicate-property','surrogate']:
            def change(files):
                rows=files['diorthosis-ai-review.jsonl'].decode().splitlines();r=json.loads(rows[0])
                if mode=='extra':r['extra']='unexpected'
                if mode=='missing':del r['judgment']['meaningPreserved']
                if mode=='boolean-number':r['judgment']['justified']=1
                if mode=='surrogate':r['judgment']['rationale']='\ud800'
                rows[0]=json.dumps(r)
                if mode=='duplicate-property':rows[0]=rows[0][:-1]+',"version":1}'
                files['diorthosis-ai-review.jsonl']=('\n'.join(rows)+'\n').encode()
            with self.subTest(mode=mode):self.mutate(change)

    def test_archive_traversal_extra_entries_and_human_adjudications_fail(self):
        self.mutate(lambda files:files.update({'../escape.txt':b'x'}))
        self.mutate(lambda files:files.update({'empty-adjudications.jsonl':b'{}\n'}))
        with tempfile.TemporaryDirectory() as d:
            path=Path(d)/'links.zip'
            with zipfile.ZipFile(ORIGINAL) as source,zipfile.ZipFile(path,'w') as out:
                for name in source.namelist():
                    info=zipfile.ZipInfo(name);info.external_attr=((stat.S_IFLNK|0o777)<<16)
                    out.writestr(info,source.read(name))
            with self.assertRaises(ValueError):module.validate(path)

    def test_wrong_provenance_hash_fails_before_extraction(self):
        def change(files):
            p=json.loads(files['diorthosis-ai-review-provenance.json']);p['subjectsJsonlSha256']='0'*64
            files['diorthosis-ai-review-provenance.json']=json.dumps(p).encode()
        self.mutate(change)

if __name__=='__main__':unittest.main()
