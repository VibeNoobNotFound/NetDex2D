#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
cd "$ROOT_DIR"

python3 - <<'PY'
from pathlib import Path
import re
import sys

root = Path('.').resolve()

method_re = re.compile(
    r'(?P<attrs>(?:\s*\[[^\n\]]+\]\s*)*)'
    r'(?P<access>public|private|protected|internal)\s+'
    r'(?P<mods>(?:(?:static|async|override|virtual|sealed|partial|readonly|new|extern)\s+)*)'
    r'(?:(?P<ret>[A-Za-z_][\w<>\.\?,\s\[\]]*)\s+)?'
    r'(?P<name>[A-Za-z_]\w*)\s*\((?P<params>[^\)]*)\)\s*'
    r'(?P<trailer>\{|=>)',
    re.M,
)
class_re = re.compile(r'\b(?:class|struct|record)\s+(\w+)')
heading_re = re.compile(r'^### `', re.M)

scene_map = {
    'src/main.tscn': 'docs/scenes/main.md',
    'src/scenes/ui/MainMenu.tscn': 'docs/scenes/ui-mainmenu.md',
    'src/scenes/ui/HostScreen.tscn': 'docs/scenes/ui-host.md',
    'src/scenes/ui/JoinScreen.tscn': 'docs/scenes/ui-join.md',
    'src/scenes/ui/LobbyScreen.tscn': 'docs/scenes/ui-lobby.md',
    'src/scenes/ui/SettingsMenu.tscn': 'docs/scenes/ui-settings.md',
    'src/scenes/ui/AboutScreen.tscn': 'docs/scenes/ui-about.md',
    'src/scenes/ui/PauseMenu.tscn': 'docs/scenes/ui-pause.md',
    'src/scenes/game/GameScreen.tscn': 'docs/scenes/game-screen.md',
    'src/scenes/game/Card.tscn': 'docs/scenes/game-card.md',
}

missing = []
method_warnings = []

print('Checking script documentation coverage...')
for script in sorted((root / 'src' / 'scripts').rglob('*.cs')):
    rel = script.relative_to(root).as_posix()
    rel_sub = script.relative_to(root / 'src' / 'scripts').as_posix()
    doc = root / 'docs' / 'scripts' / rel_sub
    doc = doc.with_suffix('.md')
    if not doc.exists():
        missing.append(f'MISSING DOC: {rel} -> {doc.relative_to(root).as_posix()}')
        continue

    text = script.read_text(encoding='utf-8')
    class_names = set(class_re.findall(text))
    method_count = 0
    for m in method_re.finditer(text):
        ret = (m.group('ret') or '').strip()
        name = m.group('name')
        if ret == 'delegate':
            continue
        if not ret and name not in class_names:
            continue
        method_count += 1

    doc_text = doc.read_text(encoding='utf-8')
    doc_method_count = len(heading_re.findall(doc_text))
    if doc_method_count < method_count:
        method_warnings.append(
            f'METHOD COVERAGE WARNING: {doc.relative_to(root).as_posix()} has {doc_method_count} headings but {rel} has {method_count} methods'
        )

print('Checking scene documentation coverage...')
for scene, doc in scene_map.items():
    if not (root / scene).exists():
        missing.append(f'SCENE MISSING IN SRC: {scene}')
        continue
    if not (root / doc).exists():
        missing.append(f'MISSING SCENE DOC: {scene} -> {doc}')

for line in missing + method_warnings:
    print(line)

if missing or method_warnings:
    print('Documentation coverage check failed.')
    sys.exit(1)

print('Documentation coverage check passed.')
PY
