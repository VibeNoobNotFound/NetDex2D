# NetDex

NetDex is a multiplayer card game project built with **Godot 4.6 + C#**.
The current implemented game mode is **Omi** (Sri Lankan trick-taking card game), with architecture prepared for additional game types later.

## Core features

- Host-authoritative multiplayer backend (no external dedicated server)
- LAN room discovery + direct IP join fallback
- 4 player seats + spectator support
- Reconnect pause/timeout handling
- Omi rules engine with deterministic state machine
- Host-side AI bots (Easy / Normal / Strong)
- In-game animations for deal, trump announcement, and trick resolution pacing
- Cross-platform updater flow using GitHub Releases

## Feature matrix

| Area | Status | Notes |
|---|---|---|
| LAN host/join | Implemented | ENet + UDP discovery |
| Direct IP join | Implemented | Fallback when discovery blocked |
| Spectators | Implemented | Spectator snapshots supported |
| AI seats | Implemented | Host controls auto-fill + difficulty |
| Reconnect timeout | Implemented | Pause + forfeit path |
| Updater macOS | Implemented | Download + digest + helper relaunch |
| Updater Windows | Implemented | Download + digest + helper relaunch |
| Updater Linux | Implemented | Download + digest + helper relaunch |
| Updater Android | Implemented | Browser handoff to APK |
| Updater iOS | Manual | Release page handoff only |

## Tech stack

- Godot Engine `4.6`
- C# (`net9.0`, `Godot.NET.Sdk/4.6.1`)
- ENet high-level multiplayer
- UDP LAN discovery (`PacketPeerUdp`)
- GitHub Releases API for update metadata

## Repository layout

- `src/` - Godot project root (`project.godot`, scenes, scripts, export presets)
- `docs/` - architecture, flow, and generated reference docs
- `src/tools/` - utility scripts (release and docs checks)

## Local setup

### Build

```bash
dotnet build src/NetDex.sln
```

### Open in Godot

Open this project file:

```
src/project.godot
```

## Local multiplayer testing

### Same machine multi-instance

1. Run one instance as host.
2. Run additional instances as clients/spectators.
3. If discovery binding fails, ensure only expected number of instances are active and use direct IP fallback.

### LAN testing

1. Keep all devices on same subnet/Wi-Fi.
2. Host room from one device.
3. Join from others via discovery list or direct host IP.

## Platform/export notes

- Android networking requires export permissions (`internet`, `access_network_state`, `access_wifi_state`, `change_wifi_multicast_state`).
- Updater behavior is platform-specific and documented in `docs/flows/updater-lifecycle.md`.

## Release process (GitHub Releases)

Expected release assets:

- `netdex-macos-universal.zip`
- `netdex-windows-x64.zip`
- `netdex-linux-x64.zip`
- `netdex-android-arm64.apk`

Useful scripts:

```bash
bash src/tools/release/package_release.sh
bash src/tools/release/verify_release_assets.sh <owner> <repo>
```

## Documentation

Start here:

- `docs/README.md`

Important references:

- `docs/reference/autoloads.md`
- `docs/reference/rpc-map.md`
- `docs/reference/signals-and-events.md`
- `docs/_manifest.md`

## Contributing docs

When changing code in `src/scripts` or scenes in `src/scenes`, update docs and run coverage checks:

```bash
bash src/tools/docs/check_docs_coverage.sh
bash src/tools/docs/check_docs_links.sh
```
