# Updater Lifecycle

## Check path
1. `UpdateManager` auto-checks periodically and supports manual checks.
2. `GitHubReleaseProvider` loads `releases/latest`.
3. Version comparison + required platform asset lookup are applied.
4. On Windows, updater selects architecture-specific asset (`netdex-windows-x86_64.zip` or `netdex-windows-arm64.zip`) based on runtime features.
5. Desktop platforms require valid `sha256` digest metadata.

## Install path
- macOS/Windows/Linux: download package -> verify checksum -> spawn helper script/process -> relaunch.
- Android: browser handoff to APK/release page.
- iOS: manual update page only.

## UI integration
- `AboutScreen` subscribes to update signals and exposes check/update/skip actions.
