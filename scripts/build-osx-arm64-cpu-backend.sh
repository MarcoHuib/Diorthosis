#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 <published-app-directory>" >&2
  exit 2
fi

if [[ "$(uname -s)" != Darwin || "$(uname -m)" != arm64 ]]; then
  echo "This script must run on macOS ARM64." >&2
  exit 2
fi

publish_dir="$(cd "$1" && pwd)"
native_dir="$publish_dir/runtimes/osx-arm64/native"
if [[ ! -d "$native_dir" ]]; then
  echo "Published osx-arm64 native library directory was not found." >&2
  exit 2
fi

command -v git >/dev/null || { echo "Git is required to build the pinned native backend." >&2; exit 2; }
command -v cmake >/dev/null || { echo "CMake 3.31.6 or compatible is required to build the pinned native backend." >&2; exit 2; }
command -v ninja >/dev/null || { echo "Ninja 1.11.1.4 or compatible is required to build the pinned native backend." >&2; exit 2; }
command -v xcrun >/dev/null || { echo "Xcode Command Line Tools are required to build the pinned native backend." >&2; exit 2; }

llama_commit=3f7c29d318e317b63f54c558bc69803963d7d88c
work_root="${TMPDIR:-/tmp}/diorthosis-llama-cpu-${llama_commit}"
source_dir="$work_root/source"
build_dir="$work_root/build"

if [[ ! -d "$source_dir/.git" ]]; then
  mkdir -p "$work_root"
  git clone --filter=blob:none --no-checkout https://github.com/ggml-org/llama.cpp.git "$source_dir"
fi
git -C "$source_dir" fetch --depth 1 origin "$llama_commit"
git -C "$source_dir" checkout --detach "$llama_commit"

cmake -S "$source_dir" -B "$build_dir" -G Ninja \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DGGML_NATIVE=OFF \
  -DGGML_CPU_ARM_ARCH=armv8.2-a \
  -DGGML_METAL=OFF \
  -DGGML_BLAS=ON \
  -DGGML_BLAS_VENDOR=Apple \
  -DGGML_OPENMP=OFF \
  -DLLAMA_BUILD_TESTS=OFF \
  -DLLAMA_BUILD_EXAMPLES=OFF \
  -DLLAMA_BUILD_SERVER=OFF \
  -DLLAMA_BUILD_TOOLS=OFF
cmake --build "$build_dir" --config Release --parallel "${BUILD_JOBS:-4}"

# The pinned LLamaSharp binding expects the 0.27 llama.cpp ABI and these canonical names.
rm -f "$native_dir"/libggml*.dylib "$native_dir"/libllama*.dylib "$native_dir"/libmtmd.dylib
cp -a "$build_dir"/bin/libggml*.dylib "$native_dir/"
cp -a "$build_dir"/bin/libllama*.dylib "$native_dir/"

if find "$native_dir" -maxdepth 1 -iname '*metal*' | grep -q .; then
  echo "Metal library unexpectedly remains in the CPU-only backend." >&2
  exit 1
fi
if otool -L "$native_dir/libllama.dylib" | grep -qi metal; then
  echo "CPU-only llama library unexpectedly links Metal." >&2
  exit 1
fi

echo "Built and installed llama.cpp $llama_commit for portable CPU-only macOS ARM64 (Apple Accelerate; Metal disabled)."
