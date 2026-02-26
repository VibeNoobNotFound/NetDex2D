# Code Map (What each folder does)

This is a quick "where should I look?" guide.

## `src/scripts/core/`

Pure game domain and rules (no scene UI dependencies):

- `OmiRulesEngine.cs` -> rules validation and state transitions
- `OmiMatchState.cs` -> full authoritative match state
- `DeckService.cs` -> build/shuffle/cut deck
- `commands/` -> match command + event contracts
- `enums/` -> game enums
- `models/` -> data models (card, played card, visible state)
- `serialization/` -> dictionary/json conversion used for networking
- `rules/IGameRulesEngine.cs` -> abstraction for future game types

## `src/scripts/network/`

Low-level networking:

- `NetworkManager.cs` -> ENet session + discovery + connection lifecycle
- `NetworkRpc.cs` -> RPC methods and event pushers
- `stores/DiscoveryStore.cs` -> discovered room cache

## `src/scripts/lobby/`

Room and participant management:

- `LobbyManager.cs` -> host-side room logic + snapshot broadcast
- `RoomState.cs` -> room data model and seat mapping
- `ParticipantInfo.cs` -> participant data model
- `RoomAdvertisement.cs` -> LAN discovery payload model
- `RoomMatchLifecycle.cs` -> room state enum
- `stores/` -> local room/match state stores

## `src/scripts/game/`

Game-specific runtime:

- `MatchCoordinator.cs` -> server command application and match lifecycle
- `GameScreen.cs` -> UI rendering from authoritative snapshot
- `Card.cs` -> card widget behavior

## `src/scripts/ui/`

Menu and lobby screens:

- `MainMenu.cs`
- `main/HostScreen.cs`
- `main/JoinScreen.cs`
- `LobbyScreen.cs`
- `PauseMenu.cs`
- `SettingsMenu.cs`

## `src/scenes/`

Godot scenes:

- `ui/` -> all menu and lobby screens
- `game/` -> game table and card scene

## `docs/`

Documentation for architecture and behavior (this folder).
