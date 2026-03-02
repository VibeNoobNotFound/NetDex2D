#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
OUT_DIR="${1:-$ROOT_DIR/ReleaseArtifacts}"
mkdir -p "$OUT_DIR"

echo "Packaging release artifacts into: $OUT_DIR"

echo "Expected files to upload to GitHub Release:"
echo "  - netdex-macos-universal.zip"
echo "  - netdex-windows-x86_64.zip"
echo "  - netdex-windows-arm64.zip"
echo "  - netdex-linux-x64.zip"
echo "  - netdex-android-arm64.apk"

echo
echo "Example commands:"
echo "  cp /path/to/exported/NetDex.app \"$OUT_DIR/NetDex.app\""
echo "  (cd \"$OUT_DIR\" && zip -r netdex-macos-universal.zip NetDex.app)"
echo "  cp /path/to/exported/NetDex-Windows-x86_64.zip \"$OUT_DIR/netdex-windows-x86_64.zip\""
echo "  cp /path/to/exported/NetDex-Windows-arm64.zip \"$OUT_DIR/netdex-windows-arm64.zip\""
echo "  cp /path/to/exported/NetDex-Linux.zip \"$OUT_DIR/netdex-linux-x64.zip\""
echo "  cp /path/to/exported/NetDex.apk \"$OUT_DIR/netdex-android-arm64.apk\""

echo
echo "Done. Ensure release tag format is vMAJOR.MINOR.PATCH before publishing."
