# Signals and Events Map

This reference lists custom Godot signals and key event names observed from the source code.

## Custom `[Signal]` declarations by file

### `scripts/game/Card.cs`
- `CardClickedEventHandler`

### `scripts/game/MatchCoordinator.cs`
- `MatchInfoEventHandler`
- `MatchStateAdvancedEventHandler`

### `scripts/lobby/LobbyManager.cs`
- `InfoMessageEventHandler`
- `MatchSnapshotChangedEventHandler`
- `RoomStateChangedEventHandler`

### `scripts/network/NetworkManager.cs`
- `ConnectionStatusChangedEventHandler`
- `DiscoveryUpdatedEventHandler`
- `NetworkIssueRaisedEventHandler`
- `NetworkMessageEventHandler`

### `scripts/network/NetworkRpc.cs`
- `ServerEventReceivedEventHandler`
- `ServerMessageEventHandler`

### `scripts/updates/UpdateManager.cs`
- `UpdateAvailableEventHandler`
- `UpdateIssueRaisedEventHandler`
- `UpdateStatusChangedEventHandler`

## Match event names emitted by rules engine

- No `MatchEvent` names discovered.
