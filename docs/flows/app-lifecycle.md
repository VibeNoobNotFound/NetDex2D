# App Lifecycle

## Startup path
1. Godot opens `src/project.godot`.
2. `run/main_scene` loads `src/main.tscn`.
3. Autoload singletons initialize (`GameManager`, `NetworkManager`, `LobbyManager`, `MatchCoordinator`, `AudioManager`, `UpdateManager`, etc.).
4. `Main` root keeps a global background and toggles child screens via `GameManager`.

## Screen navigation model
- Navigation is centralized in `GameManager` methods (`LoadMainMenu`, `LoadHostScreen`, `LoadJoinScreen`, `LoadLobby`, `LoadGameScene`, `LoadSettingsMenu`, `LoadAboutScreen`).
- Individual UI scripts only request transitions by calling those methods.

## Session lifecycle highlights
- Host path: Main -> Host -> Lobby -> Game -> (optional) back to Lobby.
- Client path: Main -> Join -> Lobby -> Game.
- Leave path tears down multiplayer peer and returns to Main menu.
