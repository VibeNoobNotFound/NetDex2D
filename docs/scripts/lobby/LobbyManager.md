# LobbyManager (lobby/LobbyManager.cs)

- Source: `scripts/lobby/LobbyManager.cs`
- Namespace: `NetDex.Lobby`
- Purpose: LobbyManager in `lobby/LobbyManager.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.AI`
  - `NetDex.Core.Enums`
  - `NetDex.Core.Serialization`
  - `NetDex.Lobby.Stores`
  - `NetDex.Networking`
  - `System`
  - `System.Linq`
- Autoload/manager dependencies detected:
  - `MatchCoordinator`
  - `NetworkRpc`
- Scene node paths used: none

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `RoomStateChangedEventHandler` | `none` | No |
| `MatchSnapshotChangedEventHandler` | `none` | No |
| `InfoMessageEventHandler` | `string message` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `GetLocalRole()` | `ParticipantRole` | Public entry point |
| `GetLocalSeat()` | `SeatPosition?` | Public entry point |
| `CreateHostedRoom(string roomName, string hostName, string reconnectToken)` | `void` | Public entry point |
| `LeaveRoomLocally(string reason = "Left room")` | `void` | Public entry point |
| `ServerHandleJoinRequest(int peerId, string playerName, ParticipantRole requestedRole, string reconnectToken, out string error)` | `bool` | Public entry point |
| `ServerHandleLeaveRequest(int peerId)` | `void` | Public entry point |
| `ServerHandlePeerDisconnected(int peerId)` | `void` | Public entry point |
| `ServerHandleSeatChange(int requesterPeerId, SeatPosition targetSeat, int targetPeerId, out string error)` | `bool` | Public entry point |
| `ServerSetAiOptions(int requesterPeerId, bool autoFill, AiDifficulty difficulty, out string error)` | `bool` | Public entry point |
| `ServerPreparePlayersForMatch(out string error)` | `bool` | Public entry point |
| `ServerCanStartMatch(out string error)` | `bool` | Public entry point |
| `SetMatchLifecycle(RoomMatchLifecycle lifecycle)` | `void` | Public entry point |
| `ApplyRemoteRoomSnapshot(string snapshotJson)` | `void` | Public entry point |
| `ApplyRemoteMatchSnapshot(string snapshotJson)` | `void` | Public entry point |
| `BuildRoomSnapshotJson(bool includeReconnectToken = false)` | `string` | Public entry point |
| `BuildAdvertisement(string hostAddress)` | `RoomAdvertisement` | Public entry point |
| `BroadcastRoomState()` | `void` | Public entry point |
| `BroadcastMatchSnapshotToAll()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `ServerRemoveOrDisconnectParticipant(int peerId, bool removeDuringLobby)` | `private` | `void` |
| `ServerAutoFillEmptySeatsWithBots()` | `private` | `void` |
| `AllocateNextBotPeerId()` | `private` | `int` |
| `CreateBotParticipantForSeat(int peerId, SeatPosition seat, AiDifficulty difficulty)` | `private` | `ParticipantInfo` |
| `RemoveUnseatedLobbyBots()` | `private` | `void` |
| `SanitizePlayerName(string name)` | `private` | `string` |
| `SanitizeRoomName(string name)` | `private` | `string` |
| `ParseSnapshotJson(string json)` | `private` | `Godot.Collections.Dictionary` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/lobby/LobbyManager.cs:33`
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

### `GetLocalRole()`

- Signature: `public ParticipantRole GetLocalRole()`
- Source range: `scripts/lobby/LobbyManager.cs:59`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `GetLocalSeat()`

- Signature: `public SeatPosition? GetLocalSeat()`
- Source range: `scripts/lobby/LobbyManager.cs:72`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `CreateHostedRoom(string roomName, string hostName, string reconnectToken)`

- Signature: `public void CreateHostedRoom(string roomName, string hostName, string reconnectToken)`
- Source range: `scripts/lobby/LobbyManager.cs:85`
- Inputs:
  - `string roomName, string hostName, string reconnectToken`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SanitizeRoomName(...)`
  - Calls `SanitizePlayerName(...)`
  - Calls `GetUniqueId(...)`
  - Calls `NewGuid(...)`
  - Calls `ToString(...)`
  - Calls `SetSeat(...)`
  - Calls `Set(...)`
  - Calls `Clear(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `LeaveRoomLocally(string reason = "Left room")`

- Signature: `public void LeaveRoomLocally(string reason = "Left room")`
- Source range: `scripts/lobby/LobbyManager.cs:124`
- Inputs:
  - `string reason = "Left room"`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Clear(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleJoinRequest(int peerId, string playerName, ParticipantRole requestedRole, string reconnectToken, out string error)`

- Signature: `public bool ServerHandleJoinRequest(int peerId, string playerName, ParticipantRole requestedRole, string reconnectToken, out string error)`
- Source range: `scripts/lobby/LobbyManager.cs:133`
- Inputs:
  - `int peerId, string playerName, ParticipantRole requestedRole, string reconnectToken, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `SanitizePlayerName(...)`
  - Calls `FirstOrDefault(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `Remove(...)`
  - Calls `ToList(...)`
  - Calls `ServerHandlePlayerReconnected(...)`
  - Calls `BuildRoomSnapshotJson(...)`
  - Calls `SendRoomSnapshot(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `ContainsKey(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleLeaveRequest(int peerId)`

- Signature: `public void ServerHandleLeaveRequest(int peerId)`
- Source range: `scripts/lobby/LobbyManager.cs:232`
- Inputs:
  - `int peerId`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ServerRemoveOrDisconnectParticipant(...)`
  - Calls `BroadcastRoomState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandlePeerDisconnected(int peerId)`

- Signature: `public void ServerHandlePeerDisconnected(int peerId)`
- Source range: `scripts/lobby/LobbyManager.cs:243`
- Inputs:
  - `int peerId`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `LeaveRoomLocally(...)`
  - Calls `ServerPauseForReconnect(...)`
  - Calls `ServerRemoveOrDisconnectParticipant(...)`
  - Calls `BroadcastRoomState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerHandleSeatChange(int requesterPeerId, SeatPosition targetSeat, int targetPeerId, out string error)`

- Signature: `public bool ServerHandleSeatChange(int requesterPeerId, SeatPosition targetSeat, int targetPeerId, out string error)`
- Source range: `scripts/lobby/LobbyManager.cs:276`
- Inputs:
  - `int requesterPeerId, SeatPosition targetSeat, int targetPeerId, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `SetSeat(...)`
  - Calls `RemoveUnseatedLobbyBots(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerSetAiOptions(int requesterPeerId, bool autoFill, AiDifficulty difficulty, out string error)`

- Signature: `public bool ServerSetAiOptions(int requesterPeerId, bool autoFill, AiDifficulty difficulty, out string error)`
- Source range: `scripts/lobby/LobbyManager.cs:320`
- Inputs:
  - `int requesterPeerId, bool autoFill, AiDifficulty difficulty, out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `BroadcastRoomState(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerPreparePlayersForMatch(out string error)`

- Signature: `public bool ServerPreparePlayersForMatch(out string error)`
- Source range: `scripts/lobby/LobbyManager.cs:354`
- Inputs:
  - `out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ServerAutoFillEmptySeatsWithBots(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerCanStartMatch(out string error)`

- Signature: `public bool ServerCanStartMatch(out string error)`
- Source range: `scripts/lobby/LobbyManager.cs:377`
- Inputs:
  - `out string error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `TryGetValue(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetMatchLifecycle(RoomMatchLifecycle lifecycle)`

- Signature: `public void SetMatchLifecycle(RoomMatchLifecycle lifecycle)`
- Source range: `scripts/lobby/LobbyManager.cs:411`
- Inputs:
  - `RoomMatchLifecycle lifecycle`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ApplyRemoteRoomSnapshot(string snapshotJson)`

- Signature: `public void ApplyRemoteRoomSnapshot(string snapshotJson)`
- Source range: `scripts/lobby/LobbyManager.cs:422`
- Inputs:
  - `string snapshotJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ParseSnapshotJson(...)`
  - Calls `Set(...)`
  - Calls `FromSnapshotDictionary(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ApplyRemoteMatchSnapshot(string snapshotJson)`

- Signature: `public void ApplyRemoteMatchSnapshot(string snapshotJson)`
- Source range: `scripts/lobby/LobbyManager.cs:429`
- Inputs:
  - `string snapshotJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Set(...)`
  - Calls `ParseJson(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildRoomSnapshotJson(bool includeReconnectToken = false)`

- Signature: `public string BuildRoomSnapshotJson(bool includeReconnectToken = false)`
- Source range: `scripts/lobby/LobbyManager.cs:435`
- Inputs:
  - `bool includeReconnectToken = false`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Stringify(...)`
  - Calls `ToSnapshotDictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildAdvertisement(string hostAddress)`

- Signature: `public RoomAdvertisement BuildAdvertisement(string hostAddress)`
- Source range: `scripts/lobby/LobbyManager.cs:442`
- Inputs:
  - `string hostAddress`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `RoomAdvertisement(...)`
  - Calls `ToString(...)`
  - Calls `GetUnixTimeFromSystem(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BroadcastRoomState()`

- Signature: `public void BroadcastRoomState()`
- Source range: `scripts/lobby/LobbyManager.cs:474`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `BuildRoomSnapshotJson(...)`
  - Calls `BroadcastRoomSnapshot(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BroadcastMatchSnapshotToAll()`

- Signature: `public void BroadcastMatchSnapshotToAll()`
- Source range: `scripts/lobby/LobbyManager.cs:486`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ToList(...)`
  - Calls `BuildSnapshotForPeer(...)`
  - Calls `SendMatchSnapshot(...)`
  - Calls `SendSpectatorHands(...)`
  - Calls `SendPrivateHand(...)`
  - Calls `GetUniqueId(...)`
  - Calls `TryGetValue(...)`
  - Calls `Set(...)`
  - Calls `ParseJson(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `ServerRemoveOrDisconnectParticipant(int peerId, bool removeDuringLobby)`

- Signature: `private void ServerRemoveOrDisconnectParticipant(int peerId, bool removeDuringLobby)`
- Source range: `scripts/lobby/LobbyManager.cs:522`
- Inputs:
  - `int peerId, bool removeDuringLobby`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `SetSeat(...)`
  - Calls `Remove(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ServerAutoFillEmptySeatsWithBots()`

- Signature: `private void ServerAutoFillEmptySeatsWithBots()`
- Source range: `scripts/lobby/LobbyManager.cs:559`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetValues(...)`
  - Calls `IsSeatTaken(...)`
  - Calls `AllocateNextBotPeerId(...)`
  - Calls `CreateBotParticipantForSeat(...)`
  - Calls `SetSeat(...)`
  - Calls `RemoveUnseatedLobbyBots(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `AllocateNextBotPeerId()`

- Signature: `private int AllocateNextBotPeerId()`
- Source range: `scripts/lobby/LobbyManager.cs:582`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ContainsKey(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `CreateBotParticipantForSeat(int peerId, SeatPosition seat, AiDifficulty difficulty)`

- Signature: `private ParticipantInfo CreateBotParticipantForSeat(int peerId, SeatPosition seat, AiDifficulty difficulty)`
- Source range: `scripts/lobby/LobbyManager.cs:598`
- Inputs:
  - `int peerId, SeatPosition seat, AiDifficulty difficulty`
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

### `RemoveUnseatedLobbyBots()`

- Signature: `private void RemoveUnseatedLobbyBots()`
- Source range: `scripts/lobby/LobbyManager.cs:614`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Where(...)`
  - Calls `Select(...)`
  - Calls `ToList(...)`
  - Calls `Remove(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SanitizePlayerName(string name)`

- Signature: `private static string SanitizePlayerName(string name)`
- Source range: `scripts/lobby/LobbyManager.cs:632`
- Inputs:
  - `string name`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `Trim(...)`
  - Calls `string(...)`
  - Calls `Where(...)`
  - Calls `IsLetterOrDigit(...)`
  - Calls `ToArray(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SanitizeRoomName(string name)`

- Signature: `private static string SanitizeRoomName(string name)`
- Source range: `scripts/lobby/LobbyManager.cs:644`
- Inputs:
  - `string name`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `Trim(...)`
  - Calls `string(...)`
  - Calls `Where(...)`
  - Calls `IsLetterOrDigit(...)`
  - Calls `ToArray(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ParseSnapshotJson(string json)`

- Signature: `private static Godot.Collections.Dictionary ParseSnapshotJson(string json)`
- Source range: `scripts/lobby/LobbyManager.cs:656`
- Inputs:
  - `string json`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ParseString(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `Dictionary(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
