"""Build-source integrity checks; no downloads or native compilation."""
import pathlib
import subprocess
import tempfile
import unittest

ROOT = pathlib.Path(__file__).resolve().parents[2]

class NativeSourceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.repo = pathlib.Path(self.temp.name)
        self.git('init', '-q')
        self.git('config', 'user.email', 'fixture@example.invalid')
        self.git('config', 'user.name', 'Unit fixture')
        self.git('remote', 'add', 'origin', 'https://github.com/ggml-org/llama.cpp.git')
        (self.repo/'CMakeLists.txt').write_text('project(fixture)\n')
        self.git('add', '.')
        self.git('commit', '-qm', 'fixture')
        self.pin = self.git('rev-parse', 'HEAD').stdout.strip()
    def tearDown(self): self.temp.cleanup()
    def git(self, *args): return subprocess.run(['git', '-C', str(self.repo), *args], check=True, capture_output=True, text=True)
    def verify(self, pin=None): return subprocess.run(['bash', str(ROOT/'scripts/verify-native-source.sh'), str(self.repo), pin or self.pin], capture_output=True, text=True)
    def test_pristine_pin_passes(self): self.assertEqual(0, self.verify().returncode)
    def test_tracked_mutation_is_rejected(self):
        (self.repo/'CMakeLists.txt').write_text('project(modified)\n')
        self.git('checkout', '--detach', self.pin)
        self.assertNotEqual(0, self.verify().returncode)
    def test_untracked_source_is_rejected(self):
        (self.repo/'injected.cmake').write_text('add_definitions(-DUNREVIEWED)\n')
        self.assertNotEqual(0, self.verify().returncode)
    def test_wrong_pin_and_origin_are_rejected(self):
        self.assertNotEqual(0, self.verify('0'*40).returncode)
        self.git('remote', 'set-url', 'origin', 'https://example.invalid/source.git')
        self.assertNotEqual(0, self.verify().returncode)
