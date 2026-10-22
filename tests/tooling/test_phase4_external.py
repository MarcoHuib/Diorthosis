"""Adversarial packaging and denominator checks, using synthetic evidence only."""
import importlib
import json
from pathlib import Path
import stat
import sys
import tempfile
import unittest
import zipfile
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'scripts'))
i=importlib.import_module('integrate-phase4-reviews')
a=importlib.import_module('analyze-phase4-external-reviews')
c=importlib.import_module('phase2-analysis')

class ArchiveTests(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);self.root=Path(self.tmp.name)
    def archive(self,entries):
        path=self.root/'input.zip'
        with zipfile.ZipFile(path,'w') as z:
            for name,value in entries:z.writestr(name,value)
        return path
    def test_traversal_and_duplicate_members_rejected(self):
        for names in [('decisions.jsonl','error-decisions.jsonl','../provenance.json'),('decisions.jsonl','decisions.jsonl','provenance.json')]:
            with self.assertRaisesRegex(ValueError,'three unique'):
                i.archive_files(self.archive([(n,b'fixture') for n in names]))
    def test_symlink_and_oversize_rejected(self):
        link=zipfile.ZipInfo('provenance.json');link.create_system=3;link.external_attr=(stat.S_IFLNK|0o777)<<16
        with self.assertRaisesRegex(ValueError,'nonregular'):
            i.archive_files(self.archive([('decisions.jsonl',b''),('error-decisions.jsonl',b''),(link,b'/etc/passwd')]))
        with self.assertRaisesRegex(ValueError,'oversized'):
            i.archive_files(self.archive([('decisions.jsonl',b'x'*2_000_001),('error-decisions.jsonl',b''),('provenance.json',b'{}')]))
    def test_exact_member_bytes_preserved(self):
        entries=[('decisions.jsonl','é\r\n'.encode()),('error-decisions.jsonl',b''),('provenance.json',b'{"x":1}\n')]
        self.assertEqual(dict(entries),i.archive_files(self.archive(entries)))
    def test_source_only_extra_quarantined_without_rebinding(self):
        case=dict(id='test',language='dutch',inputText='Een fout.',errors=[{}],sourceFamily='fictional',documentId=None)
        source=c.subject(case,'annotations','case',None,[{'originalText':'fout'}],{})
        cand=c.subject(case,'parsed','passage','Een herstel.',[{'originalText':'fout'}],{})
        c.write_lines(self.root/'subjects.jsonl',[source,cand])
        rows=[dict(subjectId=s['subjectId'],bindingSha256=s['bindingSha256'],annotationIndex=0,repaired=s==cand,rationale='Synthetic fixture, no linguistic judgment.') for s in (source,cand)]
        raw=[c.jsontext(r).encode()+b'\r\n' for r in rows]
        original=self.root/'error-decisions.jsonl';original.write_bytes(b''.join(raw))
        selected,valid,quarantined,missing=i.prepare_errors(self.root,original,self.root/'derived',{s['subjectId'] for s in (source,cand)})
        self.assertEqual(b''.join(raw),original.read_bytes());self.assertEqual(raw[1],selected.read_bytes())
        self.assertEqual(rows[1],valid[0]);self.assertEqual(rows[0],quarantined[0]['decision']);self.assertEqual([],missing)
        rows[0]['bindingSha256']='0'*64;c.write_lines(original,rows)
        with self.assertRaisesRegex(ValueError,'Invalid supplementary source'):i.prepare_errors(self.root,original,self.root/'other',{s['subjectId'] for s in (source,cand)})
    def test_duplicate_source_only_extra_rejected(self):
        case=dict(id='test',language='dutch',inputText='Een fout.',errors=[{}])
        s=c.subject(case,'annotations','case',None,[{}],{})
        c.write_lines(self.root/'subjects.jsonl',[s])
        r=dict(subjectId=s['subjectId'],bindingSha256=s['bindingSha256'],annotationIndex=0,repaired=False,rationale='Test only.')
        c.write_lines(self.root/'errors.jsonl',[r,r])
        with self.assertRaisesRegex(ValueError,'Duplicate supplementary'):i.prepare_errors(self.root,self.root/'errors.jsonl',self.root/'derived',{s['subjectId']})

class AggregationTests(unittest.TestCase):
    def test_parent_overlap_or_mutation_cannot_inflate_coverage(self):
        parent=[dict(subjectId='a',bindingSha256='original'),dict(subjectId='b',bindingSha256='second')]
        self.assertEqual(['b'],i.reconcile(parent,[[parent[0]]]))
        with self.assertRaisesRegex(ValueError,'Duplicate subject across'):i.reconcile(parent,[[parent[0]],[parent[0]]])
        with self.assertRaisesRegex(ValueError,'altered parent'):i.reconcile(parent,[[dict(subjectId='a',bindingSha256='changed')]])
    def test_contradictory_passage_and_bound_local_repair_rejected(self):
        main=[dict(subjectId='test',judgment=dict(allDefectsRepaired=True))]
        error=[dict(subjectId='test',annotationIndex=0,repaired=False)]
        with self.assertRaisesRegex(ValueError,'Contradictory passage/per-error'):i.validate_error_consistency(main,error)
        main[0]['judgment']['allDefectsRepaired']=False
        error[0]['repaired']=True
        # A local repair can coexist with another unannotated source error; do not reject it.
        i.validate_error_consistency(main,error)
    def test_unknowns_and_abstentions_keep_all_attempt_denominators(self):
        measured=a.measure([True,False,None,False]);self.assertEqual(dict(target=True,count=1,contrary=2,unknown=1,attempted=4),measured)
        pair=a.paired_records({'case':{'ok':None}},{'case':{'ok':True}},'ok')
        self.assertEqual(0,pair['knownPairs']);self.assertEqual([],pair['gains']);self.assertEqual(1,len(pair['unknown']))
    def test_clean_inapplicable_repair_flag_is_not_missing_candidate_judgment(self):
        j=dict(justified=True,meaningPreserved=True,noNewErrors=True,cleanPreserved=True,allDefectsRepaired=None,criticalSemanticFailure=False)
        self.assertIs(True,a.passage(j,True));self.assertIsNone(a.passage(j,False))
        j['meaningPreserved']=False;self.assertIs(False,a.passage(j,False))
    def test_critical_semantics_cannot_be_outweighed_by_repair(self):
        j=dict(justified=True,meaningPreserved=None,noNewErrors=True,allDefectsRepaired=True,criticalSemanticFailure=True)
        self.assertIs(False,a.passage(j,False))
    def test_duplicate_lineage_layer_rejected(self):
        subjects={'a':dict(layer='raw'),'b':dict(layer='raw')}
        link=dict(originalCaseId='case',arm='prompt')
        with self.assertRaisesRegex(ValueError,'Repeated generation/layer'):a.occurrence_index(subjects,{'a':[link],'b':[link]})

if __name__=='__main__':unittest.main()
