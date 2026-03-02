# Autoloads

Autoloads are defined in `src/project.godot` and are globally available singletons.

| Name | Script | Responsibility |
|---|---|---|
| `GameManager` | `*res://scripts/managers/GameManager.cs` | Global manager (scene/audio/navigation) responsibilities. |
| `LobbyManager` | `*res://scripts/lobby/LobbyManager.cs` | Lobby room state and participant flow. |
| `MatchCoordinator` | `*res://scripts/game/MatchCoordinator.cs` | Match orchestration/runtime gameplay control. |
| `NetworkRpc` | `*res://scripts/network/NetworkRpc.cs` | Network transport/RPC/discovery management. |
| `NetworkManager` | `*res://scripts/network/NetworkManager.cs` | Network transport/RPC/discovery management. |
| `AiCoordinator` | `*res://scripts/ai/AiCoordinator.cs` | Host-side AI decision scheduling and policy execution. |
| `AudioManager` | `*res://scripts/managers/AudioManager.cs` | Global manager (scene/audio/navigation) responsibilities. |
| `UpdateManager` | `*res://scripts/updates/UpdateManager.cs` | GitHub-based updater checks and install actions. |
