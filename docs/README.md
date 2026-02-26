# NetDex Docs (Beginner Friendly)

The `docs/` folder explains how the game works, especially the backend (multiplayer + rules engine).

The project code is in `src/`.

If you are new, read in this order:

1. `backend-overview.md`
2. `code-map.md`
3. `network-discovery.md`
4. `lobby-and-room-management.md`
5. `omi-rules-engine.md`
6. `match-runtime-flow.md`
7. `data-contracts.md`
8. `troubleshooting.md`

## Quick project map

- Main project file: `src/project.godot`
- Main scene: `src/main.tscn`
- Core game rules: `src/scripts/core/`
- Lobby/session backend: `src/scripts/lobby/`
- Networking backend: `src/scripts/network/`
- Match orchestration: `src/scripts/game/MatchCoordinator.cs`
- UI scripts: `src/scripts/ui/` and `src/scripts/game/GameScreen.cs`

## Build and open

- Build:
  - `cd src`
  - `dotnet build NetDex.sln`
- Open in Godot:
  - Open `src/project.godot`
