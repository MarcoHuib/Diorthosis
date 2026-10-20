#!/usr/bin/env bash
set -euo pipefail
if [[ $# -ne 2 ]]; then echo 'Usage: verify-native-source.sh <source-directory> <pinned-commit>' >&2; exit 2; fi
source_dir="$1"
pinned_commit="$2"
[[ "$(git -C "$source_dir" remote get-url origin)" == 'https://github.com/ggml-org/llama.cpp.git' ]] || { echo 'Unexpected native source origin.' >&2; exit 1; }
[[ "$(git -C "$source_dir" rev-parse HEAD)" == "$pinned_commit" ]] || { echo 'Native source revision does not match the pin.' >&2; exit 1; }
[[ -z "$(git -C "$source_dir" status --porcelain --untracked-files=all --ignored)" ]] || { echo 'Native source contains modified, untracked or ignored files.' >&2; exit 1; }
