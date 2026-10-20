import importlib.util
from pathlib import Path
import unittest
ROOT=Path(__file__).resolve().parents[2]
def module(name):
    spec=importlib.util.spec_from_file_location(name,ROOT/'scripts'/f'{name}.py');result=importlib.util.module_from_spec(spec);spec.loader.exec_module(result);return result
candidates=module('phase2-candidates');analysis=module('phase2-analysis')
class Phase2ToolsTests(unittest.TestCase):
    def test_candidates_are_varied_with_exact_utf16_annotations(self):
        rows=candidates.build();self.assertEqual(80,len(rows));self.assertEqual(80,len({r['inputText'] for r in rows}))
        for lang in ['Dutch','English']:
            selected=[r for r in rows if r['language']==lang];self.assertEqual(24,sum(not r['errors'] for r in selected))
        for row in rows:
            raw=row['inputText'].encode('utf-16-le')
            self.assertEqual('development-exposed',row['partition'])
            for span in row['errors']+row['protectedSpans']:
                self.assertEqual(span['originalText'],raw[2*span['start']:2*(span['start']+span['length'])].decode('utf-16-le'))
    def test_utf16_apply_with_emoji_and_combining_prefix(self):
        self.assertEqual('🌷 Jose\u0301: received.',analysis.apply('🌷 Jose\u0301: recieved.',[dict(start=10,length=7,originalText='recieve',replacementText='receive')]))
        with self.assertRaises((ValueError,UnicodeDecodeError)):analysis.apply('🌷 text',[dict(start=1,length=1,originalText='x',replacementText='y')])
    def test_subject_binding_changes_with_contract_and_deduplicates_identical_outputs(self):
        case=dict(id='test',language='Dutch',inputText='Dit werkt.',errors=[])
        first=analysis.subject(case,'parsed','passage','Dit werkt.',[],{})
        self.assertEqual(first,analysis.subject(case,'parsed','passage','Dit werkt.',[],{}))
        second=analysis.subject(case,'delivered','passage','Dit werkt.',[],{})
        self.assertNotEqual(first['subjectId'],second['subjectId'])
        self.assertNotEqual(first['subjectId'],analysis.subject(case,'parsed','passage','Dit helpt.',[],{})['subjectId'])
    def test_parsed_candidate_reconstruction_retains_partial_output_repairs(self):
        case=dict(id='partial',inputText='Please recieve it.')
        prediction=analysis.candidate_prediction(case,'Please receive it')
        self.assertEqual('Please receive it',analysis.apply(case['inputText'],prediction['edits']))
        self.assertEqual([],analysis.candidate_prediction(case,case['inputText'])['edits'])
    def test_delivery_eligibility_and_annotation_conflicts_fail_closed(self):
        for disposition in ['rejected','needsReview','suppressed']:
            self.assertFalse(analysis.delivered_assessable(disposition))
        self.assertTrue(analysis.delivered_assessable('unchanged'))
        with self.assertRaises(ValueError):analysis.annotation_errors(dict(annotationVariants=[dict(errors=[]),dict(errors=[])]))
    def test_rejection_classes_preserve_uncertainty(self):
        self.assertEqual('invalid-generation-termination',analysis.rejection_class('GenerationTermination','truncated'))
        self.assertEqual('protected-content-violation',analysis.rejection_class('validation','protected identifiers'))
        self.assertEqual('unsupported-output-or-unclassified',analysis.rejection_class('unknown','new reason'))
if __name__=='__main__':unittest.main()
