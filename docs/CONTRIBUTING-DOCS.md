# Documentation Contribution Guide

## Goal
Keep docs synchronized with `src/` implementation.

## Rules
- `src/` is the source of truth.
- Every script in `src/scripts/*.cs` must have a matching page in `docs/scripts/`.
- Every scene in `src/scenes/*.tscn` plus `src/main.tscn` must have a matching page in `docs/scenes/`.
- Prefer beginner-friendly explanations with accurate technical details.

## Before opening a PR

```bash
bash src/tools/docs/check_docs_coverage.sh
bash src/tools/docs/check_docs_links.sh
```

## Recommended update order
1. Update code.
2. Update script/scene docs.
3. Update flow/reference docs if contracts changed.
4. Update `docs/_manifest.md`.
