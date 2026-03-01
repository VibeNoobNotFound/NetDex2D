#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 2 ]]; then
  echo "Usage: $0 <repo-owner> <repo-name>"
  echo "Example: $0 NoobNotFound net-dex"
  exit 1
fi

OWNER="$1"
REPO="$2"
API_URL="https://api.github.com/repos/${OWNER}/${REPO}/releases/latest"

json="$(curl -fsSL -H 'Accept: application/vnd.github+json' -H 'X-GitHub-Api-Version: 2022-11-28' "$API_URL")"

tag="$(printf '%s' "$json" | python3 -c 'import json,sys; print(json.load(sys.stdin).get("tag_name",""))')"
if [[ ! "$tag" =~ ^v[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
  echo "ERROR: latest release tag '$tag' does not match vMAJOR.MINOR.PATCH"
  exit 1
fi

required=(
  "netdex-macos-universal.zip"
  "netdex-android-arm64.apk"
)

for name in "${required[@]}"; do
  found="$(printf '%s' "$json" | python3 -c "import json,sys; d=json.load(sys.stdin); print(any(a.get('name')=='$name' for a in d.get('assets',[])))")"
  if [[ "$found" != "True" ]]; then
    echo "ERROR: required asset missing: $name"
    exit 1
  fi
  echo "OK: found $name"
done

echo "Release verification passed for tag $tag"
