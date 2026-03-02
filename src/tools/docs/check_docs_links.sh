#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
cd "$ROOT_DIR"

python3 - <<'PY'
from pathlib import Path
import re
import sys

root = Path('.').resolve()
files = sorted(list((root / 'docs').rglob('*.md')) + [root / 'README.md'])
link_re = re.compile(r'\[[^\]]*\]\(([^)]+)\)')

bad = []
for f in files:
    text = f.read_text(encoding='utf-8')
    for m in link_re.finditer(text):
        target = m.group(1).strip()
        if not target:
            continue
        if target.startswith(('http://', 'https://', 'mailto:')):
            continue
        if target.startswith('#'):
            continue

        path_part = target.split('#', 1)[0]
        if not path_part:
            continue

        ref = (f.parent / path_part).resolve()
        if not ref.exists():
            bad.append((f.relative_to(root).as_posix(), target))

if bad:
    print('Broken local markdown links found:')
    for src, target in bad:
        print(f'  {src} -> {target}')
    sys.exit(1)

print('Markdown link check passed.')
PY
