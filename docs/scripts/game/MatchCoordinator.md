# MatchCoordinator (game/MatchCoordinator.cs)

- Source: `scripts/game/MatchCoordinator.cs`
- Namespace: `NetDex.Lobby`
- Purpose: MatchCoordinator in `game/MatchCoordinator.cs`. Runtime match orchestration and in-game UI rendering/animation logic.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Commands`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Models`
  - `NetDex.Core.Rules`
  - `NetDex.Networking`
- Autoload/manager dependencies detected:
  - `LobbyManager`
  - `NetworkRpc`
- Scene node paths used: none

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `MatchStateAdvancedEventHandler` | `none` | No |
| `MatchInfoEventHandler` | `string message` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_Process(double delta)` | `void` | Public entry point |
| `ServerStartMatch(int requesterPeerId, out string error)` | `bool` | Public entry point |
| `GetAuthoritativeState()` | `OmiMatchState?` | Public entry point |
| `ServerHandleCutDeck(int peerId, int cutIndex, out string error)` | `bool` | Public entry point |
| `ServerHandleShuffleAgain(int peerId, int seed, out string error)` | `bool` | Public entry point |
| `ServerHandleFinishShuffle(int peerId, out string error)` | `bool` | Public entry point |
| `ServerHandleSelectTrump(int peerId, CardSuit suit, out string error)` | `bool` | Public entry point |
| `ServerHandlePlayCard(int peerId, string cardId, out string error)` | `bool` | Public entry point |
| `ServerHandleAiCommand(MatchCommand command, out string error)` | `bool` | Public entry point |
| `ServerPauseForReconnect(int peerId, double timeoutSeconds)` | `void` | Public entry point |
| `ServerHandlePlayerReconnected(int peerId)` | `void` | Public entry point |
| `BuildSnapshotForPeer(int peerId, ParticipantRole role)` | `string` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `ApplyServerCommand(MatchCommand command)` | `private` | `bool` |
| `ApplyServerCommand(MatchCommand command, out string error)` | `private` | `bool` |
| `PushEventNotifications(MatchCommandResult result)` | `private` | `void` |
| `TryResolveSeat(int peerId, out SeatPosition seat, out string error)` | `private` | `bool` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/game/MatchCoordinator.cs:27`
- Inputs:
  - `none`
- Output / side effects:
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `QueueFree(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_Process(double delta)`

- Signature: `public override void _Process(double delta)`
- Source range: `scripts/game/MatchCoordinator.cs:38`
- Inputs:
  - `double delta`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `TryGetValue(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `ForfeitTeam(...)`
  - Calls `TeamIndex(...)`
  - Calls `CompleteFirstDeal(...)`
  - Calls `CompleteSecondDeal(...)`
  - Calls `ResolveCurrentTrick(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `ServerStartMatch(int requesterPeerId, out string error)`

- Signature: `public bool ServerStartMatch(int requesterPeerId, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:91`
- Inputs:
  - `int requesterPeerId, out string error`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerPreparePlayersForMatch(...)`
  - Calls `ServerCanStartMatch(...)`
  - Calls `TryGetSeatForPeer(...)`
  - Calls `CreateInitialMatchState(...)`
  - Calls `ApplyCommand(...)`
  - Calls `StartRound(...)`
  - Calls `Randi(...)`
  - Calls `PushEventNotifications(...)`
  - Calls `BroadcastRoomState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `GetAuthoritativeState()`

- Signature: `public OmiMatchState? GetAuthoritativeState()`
- Source range: `scripts/game/MatchCoordinator.cs:150`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `ServerHandleCutDeck(int peerId, int cutIndex, out string error)`

- Signature: `public bool ServerHandleCutDeck(int peerId, int cutIndex, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:160`
- Inputs:
  - `int peerId, int cutIndex, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryResolveSeat(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `CutDeck(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleShuffleAgain(int peerId, int seed, out string error)`

- Signature: `public bool ServerHandleShuffleAgain(int peerId, int seed, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:170`
- Inputs:
  - `int peerId, int seed, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryResolveSeat(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `ShuffleAgain(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleFinishShuffle(int peerId, out string error)`

- Signature: `public bool ServerHandleFinishShuffle(int peerId, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:180`
- Inputs:
  - `int peerId, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryResolveSeat(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `FinishShuffle(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleSelectTrump(int peerId, CardSuit suit, out string error)`

- Signature: `public bool ServerHandleSelectTrump(int peerId, CardSuit suit, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:190`
- Inputs:
  - `int peerId, CardSuit suit, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryResolveSeat(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `SelectTrump(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandlePlayCard(int peerId, string cardId, out string error)`

- Signature: `public bool ServerHandlePlayCard(int peerId, string cardId, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:200`
- Inputs:
  - `int peerId, string cardId, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryResolveSeat(...)`
  - Calls `ApplyServerCommand(...)`
  - Calls `PlayCard(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleAiCommand(MatchCommand command, out string error)`

- Signature: `public bool ServerHandleAiCommand(MatchCommand command, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:210`
- Inputs:
  - `MatchCommand command, out string error`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `TryGetValue(...)`
  - Calls `ApplyServerCommand(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `ServerPauseForReconnect(int peerId, double timeoutSeconds)`

- Signature: `public void ServerPauseForReconnect(int peerId, double timeoutSeconds)`
- Source range: `scripts/game/MatchCoordinator.cs:254`
- Inputs:
  - `int peerId, double timeoutSeconds`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `BroadcastPausedForReconnect(...)`
  - Calls `SetMatchLifecycle(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `BroadcastMatchSnapshotToAll(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandlePlayerReconnected(int peerId)`

- Signature: `public void ServerHandlePlayerReconnected(int peerId)`
- Source range: `scripts/game/MatchCoordinator.cs:277`
- Inputs:
  - `int peerId`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `SetMatchLifecycle(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `BroadcastMatchSnapshotToAll(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildSnapshotForPeer(int peerId, ParticipantRole role)`

- Signature: `public string BuildSnapshotForPeer(int peerId, ParticipantRole role)`
- Source range: `scripts/game/MatchCoordinator.cs:303`
- Inputs:
  - `int peerId, ParticipantRole role`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryGetSeatForPeer(...)`
  - Calls `SerializeJson(...)`
  - Calls `GetVisibleStateForPeer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ApplyServerCommand(MatchCommand command)`

- Signature: `private bool ApplyServerCommand(MatchCommand command)`
- Source range: `scripts/game/MatchCoordinator.cs:321`
- Inputs:
  - `MatchCommand command`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ApplyServerCommand(MatchCommand command, out string error)`

- Signature: `private bool ApplyServerCommand(MatchCommand command, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:326`
- Inputs:
  - `MatchCommand command, out string error`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ApplyCommand(...)`
  - Calls `PushEventNotifications(...)`
  - Calls `StartNextRound(...)`
  - Calls `Randi(...)`
  - Calls `SetMatchLifecycle(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `BroadcastMatchSnapshotToAll(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PushEventNotifications(MatchCommandResult result)`

- Signature: `private static void PushEventNotifications(MatchCommandResult result)`
- Source range: `scripts/game/MatchCoordinator.cs:376`
- Inputs:
  - `MatchCommandResult result`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Stringify(...)`
  - Calls `BroadcastMatchStarted(...)`
  - Calls `BroadcastCardPlayed(...)`
  - Calls `BroadcastTrickResolved(...)`
  - Calls `BroadcastRoundResolved(...)`
  - Calls `BroadcastCreditsUpdated(...)`
  - Calls `BroadcastMatchEnded(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `TryResolveSeat(int peerId, out SeatPosition seat, out string error)`

- Signature: `private bool TryResolveSeat(int peerId, out SeatPosition seat, out string error)`
- Source range: `scripts/game/MatchCoordinator.cs:405`
- Inputs:
  - `int peerId, out SeatPosition seat, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `TryGetSeatForPeer(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
