# NetworkRpc (network/NetworkRpc.cs)

- Source: `scripts/network/NetworkRpc.cs`
- Namespace: `NetDex.Networking`
- Purpose: NetworkRpc in `network/NetworkRpc.cs`. Transport/session control, discovery, RPC bridge, and networking stores.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.AI`
  - `NetDex.Core.Enums`
  - `NetDex.Lobby`
- Autoload/manager dependencies detected:
  - `LobbyManager`
  - `MatchCoordinator`
- Scene node paths used: none

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `ServerEventReceivedEventHandler` | `string eventType, string payloadJson` | No |
| `ServerMessageEventHandler` | `string message` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `SendJoinRequest(string playerName, ParticipantRole role, string reconnectToken)` | `void` | Public entry point |
| `SendSeatChangeRequest(SeatPosition targetSeat, int targetPeerId)` | `void` | Public entry point |
| `SendStartMatchRequest()` | `void` | Public entry point |
| `SendSetAiOptionsRequest(bool autoFill, AiDifficulty difficulty)` | `void` | Public entry point |
| `SendCutDeckRequest(int cutIndex)` | `void` | Public entry point |
| `SendShuffleAgainRequest(int seed)` | `void` | Public entry point |
| `SendFinishShuffleRequest()` | `void` | Public entry point |
| `SendSelectTrumpRequest(CardSuit suit)` | `void` | Public entry point |
| `SendPlayCardRequest(string cardId)` | `void` | Public entry point |
| `SendLeaveRoomRequest()` | `void` | Public entry point |
| `RequestJoinRoom(string playerName, int requestedRole, string reconnectToken)` | `void` | RPC endpoint |
| `RequestSeatChange(int targetSeat, int targetPeerId = -1)` | `void` | RPC endpoint |
| `RequestStartMatch()` | `void` | RPC endpoint |
| `RequestSetAiOptions(bool autoFill, int difficulty)` | `void` | RPC endpoint |
| `RequestCutDeck(int cutIndex)` | `void` | RPC endpoint |
| `RequestShuffleAgain(int seed)` | `void` | RPC endpoint |
| `RequestFinishShuffle()` | `void` | RPC endpoint |
| `RequestSelectTrump(int suit)` | `void` | RPC endpoint |
| `RequestPlayCard(string cardId)` | `void` | RPC endpoint |
| `RequestLeaveRoom()` | `void` | RPC endpoint |
| `PushRoomSnapshot(string snapshotJson)` | `void` | RPC endpoint |
| `PushSeatMap(string seatSnapshotJson)` | `void` | RPC endpoint |
| `PushMatchStarted(string payloadJson)` | `void` | RPC endpoint |
| `PushPrivateHand(string payloadJson)` | `void` | RPC endpoint |
| `PushSpectatorHands(string payloadJson)` | `void` | RPC endpoint |
| `PushCardPlayed(string payloadJson)` | `void` | RPC endpoint |
| `PushTrickResolved(string payloadJson)` | `void` | RPC endpoint |
| `PushRoundResolved(string payloadJson)` | `void` | RPC endpoint |
| `PushCreditsUpdated(string payloadJson)` | `void` | RPC endpoint |
| `PushPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)` | `void` | RPC endpoint |
| `PushMatchEnded(string payloadJson)` | `void` | RPC endpoint |
| `PushMatchSnapshot(string snapshotJson)` | `void` | RPC endpoint |
| `PushServerMessage(string message)` | `void` | RPC endpoint |
| `BroadcastRoomSnapshot(string snapshotJson)` | `void` | Public entry point |
| `SendRoomSnapshot(int peerId, string snapshotJson)` | `void` | Public entry point |
| `SendMatchSnapshot(int peerId, string snapshotJson)` | `void` | Public entry point |
| `SendPrivateHand(int peerId, string payloadJson)` | `void` | Public entry point |
| `SendSpectatorHands(int peerId, string payloadJson)` | `void` | Public entry point |
| `BroadcastMatchStarted(string payloadJson)` | `void` | Public entry point |
| `BroadcastCardPlayed(string payloadJson)` | `void` | Public entry point |
| `BroadcastTrickResolved(string payloadJson)` | `void` | Public entry point |
| `BroadcastRoundResolved(string payloadJson)` | `void` | Public entry point |
| `BroadcastCreditsUpdated(string payloadJson)` | `void` | Public entry point |
| `BroadcastPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)` | `void` | Public entry point |
| `BroadcastMatchEnded(string payloadJson)` | `void` | Public entry point |
| `BroadcastServerMessage(string message)` | `void` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/network/NetworkRpc.cs:18`
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

### `SendJoinRequest(string playerName, ParticipantRole role, string reconnectToken)`

- Signature: `public void SendJoinRequest(string playerName, ParticipantRole role, string reconnectToken)`
- Source range: `scripts/network/NetworkRpc.cs:29`
- Inputs:
  - `string playerName, ParticipantRole role, string reconnectToken`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetUniqueId(...)`
  - Calls `ServerHandleJoinRequest(...)`
  - Calls `BroadcastRoomState(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendSeatChangeRequest(SeatPosition targetSeat, int targetPeerId)`

- Signature: `public void SendSeatChangeRequest(SeatPosition targetSeat, int targetPeerId)`
- Source range: `scripts/network/NetworkRpc.cs:42`
- Inputs:
  - `SeatPosition targetSeat, int targetPeerId`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleSeatChange(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendStartMatchRequest()`

- Signature: `public void SendStartMatchRequest()`
- Source range: `scripts/network/NetworkRpc.cs:53`
- Inputs:
  - `none`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerStartMatch(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendSetAiOptionsRequest(bool autoFill, AiDifficulty difficulty)`

- Signature: `public void SendSetAiOptionsRequest(bool autoFill, AiDifficulty difficulty)`
- Source range: `scripts/network/NetworkRpc.cs:64`
- Inputs:
  - `bool autoFill, AiDifficulty difficulty`
- Output / side effects:
  - Emits signals to notify other systems.
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerSetAiOptions(...)`
  - Calls `GetUniqueId(...)`
  - Calls `EmitSignal(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendCutDeckRequest(int cutIndex)`

- Signature: `public void SendCutDeckRequest(int cutIndex)`
- Source range: `scripts/network/NetworkRpc.cs:84`
- Inputs:
  - `int cutIndex`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleCutDeck(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendShuffleAgainRequest(int seed)`

- Signature: `public void SendShuffleAgainRequest(int seed)`
- Source range: `scripts/network/NetworkRpc.cs:95`
- Inputs:
  - `int seed`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleShuffleAgain(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendFinishShuffleRequest()`

- Signature: `public void SendFinishShuffleRequest()`
- Source range: `scripts/network/NetworkRpc.cs:106`
- Inputs:
  - `none`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleFinishShuffle(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendSelectTrumpRequest(CardSuit suit)`

- Signature: `public void SendSelectTrumpRequest(CardSuit suit)`
- Source range: `scripts/network/NetworkRpc.cs:117`
- Inputs:
  - `CardSuit suit`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleSelectTrump(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendPlayCardRequest(string cardId)`

- Signature: `public void SendPlayCardRequest(string cardId)`
- Source range: `scripts/network/NetworkRpc.cs:128`
- Inputs:
  - `string cardId`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandlePlayCard(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendLeaveRoomRequest()`

- Signature: `public void SendLeaveRoomRequest()`
- Source range: `scripts/network/NetworkRpc.cs:139`
- Inputs:
  - `none`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `ServerHandleLeaveRequest(...)`
  - Calls `GetUniqueId(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestJoinRoom(string playerName, int requestedRole, string reconnectToken)`

- Signature: `public void RequestJoinRoom(string playerName, int requestedRole, string reconnectToken)`
- Source range: `scripts/network/NetworkRpc.cs:148`
- Inputs:
  - `string playerName, int requestedRole, string reconnectToken`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleJoinRequest(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestSeatChange(int targetSeat, int targetPeerId = -1)`

- Signature: `public void RequestSeatChange(int targetSeat, int targetPeerId = -1)`
- Source range: `scripts/network/NetworkRpc.cs:164`
- Inputs:
  - `int targetSeat, int targetPeerId = -1`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleSeatChange(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestStartMatch()`

- Signature: `public void RequestStartMatch()`
- Source range: `scripts/network/NetworkRpc.cs:180`
- Inputs:
  - `none`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerStartMatch(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestSetAiOptions(bool autoFill, int difficulty)`

- Signature: `public void RequestSetAiOptions(bool autoFill, int difficulty)`
- Source range: `scripts/network/NetworkRpc.cs:196`
- Inputs:
  - `bool autoFill, int difficulty`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `IsDefined(...)`
  - Calls `RpcId(...)`
  - Calls `ServerSetAiOptions(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestCutDeck(int cutIndex)`

- Signature: `public void RequestCutDeck(int cutIndex)`
- Source range: `scripts/network/NetworkRpc.cs:218`
- Inputs:
  - `int cutIndex`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleCutDeck(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestShuffleAgain(int seed)`

- Signature: `public void RequestShuffleAgain(int seed)`
- Source range: `scripts/network/NetworkRpc.cs:234`
- Inputs:
  - `int seed`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleShuffleAgain(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestFinishShuffle()`

- Signature: `public void RequestFinishShuffle()`
- Source range: `scripts/network/NetworkRpc.cs:250`
- Inputs:
  - `none`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleFinishShuffle(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestSelectTrump(int suit)`

- Signature: `public void RequestSelectTrump(int suit)`
- Source range: `scripts/network/NetworkRpc.cs:266`
- Inputs:
  - `int suit`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleSelectTrump(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestPlayCard(string cardId)`

- Signature: `public void RequestPlayCard(string cardId)`
- Source range: `scripts/network/NetworkRpc.cs:282`
- Inputs:
  - `string cardId`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandlePlayCard(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `RequestLeaveRoom()`

- Signature: `public void RequestLeaveRoom()`
- Source range: `scripts/network/NetworkRpc.cs:298`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetRemoteSenderId(...)`
  - Calls `ServerHandleLeaveRequest(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushRoomSnapshot(string snapshotJson)`

- Signature: `public void PushRoomSnapshot(string snapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:310`
- Inputs:
  - `string snapshotJson`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ApplyRemoteRoomSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushSeatMap(string seatSnapshotJson)`

- Signature: `public void PushSeatMap(string seatSnapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:316`
- Inputs:
  - `string seatSnapshotJson`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ApplyRemoteRoomSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushMatchStarted(string payloadJson)`

- Signature: `public void PushMatchStarted(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:322`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushPrivateHand(string payloadJson)`

- Signature: `public void PushPrivateHand(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:328`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushSpectatorHands(string payloadJson)`

- Signature: `public void PushSpectatorHands(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:334`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushCardPlayed(string payloadJson)`

- Signature: `public void PushCardPlayed(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:340`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushTrickResolved(string payloadJson)`

- Signature: `public void PushTrickResolved(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:346`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushRoundResolved(string payloadJson)`

- Signature: `public void PushRoundResolved(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:352`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushCreditsUpdated(string payloadJson)`

- Signature: `public void PushCreditsUpdated(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:358`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)`

- Signature: `public void PushPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)`
- Source range: `scripts/network/NetworkRpc.cs:364`
- Inputs:
  - `int disconnectedPeerId, double reconnectDeadlineUnixSeconds`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `Stringify(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushMatchEnded(string payloadJson)`

- Signature: `public void PushMatchEnded(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:376`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushMatchSnapshot(string snapshotJson)`

- Signature: `public void PushMatchSnapshot(string snapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:382`
- Inputs:
  - `string snapshotJson`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ApplyRemoteMatchSnapshot(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `PushServerMessage(string message)`

- Signature: `public void PushServerMessage(string message)`
- Source range: `scripts/network/NetworkRpc.cs:388`
- Inputs:
  - `string message`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastRoomSnapshot(string snapshotJson)`

- Signature: `public void BroadcastRoomSnapshot(string snapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:396`
- Inputs:
  - `string snapshotJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendRoomSnapshot(int peerId, string snapshotJson)`

- Signature: `public void SendRoomSnapshot(int peerId, string snapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:402`
- Inputs:
  - `int peerId, string snapshotJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `ApplyRemoteRoomSnapshot(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendMatchSnapshot(int peerId, string snapshotJson)`

- Signature: `public void SendMatchSnapshot(int peerId, string snapshotJson)`
- Source range: `scripts/network/NetworkRpc.cs:413`
- Inputs:
  - `int peerId, string snapshotJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `ApplyRemoteMatchSnapshot(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendPrivateHand(int peerId, string payloadJson)`

- Signature: `public void SendPrivateHand(int peerId, string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:424`
- Inputs:
  - `int peerId, string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `EmitSignal(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `SendSpectatorHands(int peerId, string payloadJson)`

- Signature: `public void SendSpectatorHands(int peerId, string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:435`
- Inputs:
  - `int peerId, string payloadJson`
- Output / side effects:
  - Emits signals to notify other systems.
  - Sends RPC traffic across peers.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUniqueId(...)`
  - Calls `EmitSignal(...)`
  - Calls `RpcId(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastMatchStarted(string payloadJson)`

- Signature: `public void BroadcastMatchStarted(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:446`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastCardPlayed(string payloadJson)`

- Signature: `public void BroadcastCardPlayed(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:451`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastTrickResolved(string payloadJson)`

- Signature: `public void BroadcastTrickResolved(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:456`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastRoundResolved(string payloadJson)`

- Signature: `public void BroadcastRoundResolved(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:461`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastCreditsUpdated(string payloadJson)`

- Signature: `public void BroadcastCreditsUpdated(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:466`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)`

- Signature: `public void BroadcastPausedForReconnect(int disconnectedPeerId, double reconnectDeadlineUnixSeconds)`
- Source range: `scripts/network/NetworkRpc.cs:471`
- Inputs:
  - `int disconnectedPeerId, double reconnectDeadlineUnixSeconds`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastMatchEnded(string payloadJson)`

- Signature: `public void BroadcastMatchEnded(string payloadJson)`
- Source range: `scripts/network/NetworkRpc.cs:476`
- Inputs:
  - `string payloadJson`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `BroadcastServerMessage(string message)`

- Signature: `public void BroadcastServerMessage(string message)`
- Source range: `scripts/network/NetworkRpc.cs:481`
- Inputs:
  - `string message`
- Output / side effects:
  - Sends RPC traffic across peers.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Rpc(...)`
- Failure paths:
  - Propagates failure/user-facing diagnostics to clients/UI.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.
