# NetworkManager (network/NetworkManager.cs)

- Source: `scripts/network/NetworkManager.cs`
- Namespace: `NetDex.Networking`
- Purpose: NetworkManager in `network/NetworkManager.cs`. Transport/session control, discovery, RPC bridge, and networking stores.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Enums`
  - `NetDex.Lobby`
  - `NetDex.Managers`
  - `NetDex.Networking.Stores`
  - `System`
  - `System.Collections.Generic`
  - `System.Text`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `LobbyManager`
  - `NetworkRpc`
- Scene node paths used: `1`

## Signals

| Signal | Parameters | Emitted In File |
|---|---|---|
| `DiscoveryUpdatedEventHandler` | `none` | No |
| `ConnectionStatusChangedEventHandler` | `string status, string message` | No |
| `NetworkMessageEventHandler` | `string message` | No |
| `NetworkIssueRaisedEventHandler` | `int issueCode, string message` | No |

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public entry point |
| `GetDiscoveredRooms()` | `IReadOnlyList<RoomAdvertisement>` | Public entry point |
| `ForceDiscoveryRefreshSignal()` | `void` | Public entry point |
| `GetLocalLanAddress()` | `string` | Public entry point |
| `GetSavedPlayerName()` | `string` | Public entry point |
| `GetOrCreateReconnectToken()` | `string` | Public entry point |
| `SavePlayerName(string playerName)` | `void` | Public entry point |
| `StartHostSession(string roomName, string playerName)` | `Error` | Public entry point |
| `JoinRoomByIp(string hostAddress, string playerName, ParticipantRole role)` | `Error` | Public entry point |
| `DisconnectSession(string reason = "Disconnected", bool notifyServer = true)` | `void` | Public entry point |
| `StartDiscovery()` | `void` | Public entry point |
| `StopDiscovery()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `StartAdvertising()` | `private` | `void` |
| `StopAdvertising()` | `private` | `void` |
| `PollDiscoveryPackets()` | `private` | `void` |
| `PublishDiscoveryIfNeeded(bool force = false)` | `private` | `void` |
| `OnAdvertiseTimerTimeout()` | `private` | `void` |
| `EmitNetworkIssue(NetworkIssueCode issueCode, string message)` | `private` | `void` |
| `IsAndroidRuntime()` | `private` | `bool` |
| `MapIssueForSocketError(Error error, bool discoveryBind)` | `private` | `NetworkIssueCode` |
| `AndroidPermissionHint()` | `private` | `string` |
| `BuildSessionCreateFailureMessage(string operation, Error error)` | `private` | `string` |
| `BuildDiscoveryBindFailureMessage(Error error)` | `private` | `string` |
| `GetPreferredLocalAddress()` | `private` | `string` |
| `OnPeerConnected(long peerId)` | `private` | `void` |
| `OnPeerDisconnected(long peerId)` | `private` | `void` |
| `OnConnectedToServer()` | `private` | `void` |
| `OnConnectionFailed()` | `private` | `void` |
| `OnServerDisconnected()` | `private` | `void` |
| `OnServerMessage(string message)` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/network/NetworkManager.cs:55`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
  - Changes node lifecycle/application flow.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `QueueFree(...)`
  - Calls `AddChild(...)`
  - Calls `StartDiscovery(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/network/NetworkManager.cs:98`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `StopAdvertising(...)`
  - Calls `StopDiscovery(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetDiscoveredRooms()`

- Signature: `public IReadOnlyList<RoomAdvertisement> GetDiscoveredRooms()`
- Source range: `scripts/network/NetworkManager.cs:104`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetAll(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ForceDiscoveryRefreshSignal()`

- Signature: `public void ForceDiscoveryRefreshSignal()`
- Source range: `scripts/network/NetworkManager.cs:109`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetLocalLanAddress()`

- Signature: `public string GetLocalLanAddress()`
- Source range: `scripts/network/NetworkManager.cs:114`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetPreferredLocalAddress(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetSavedPlayerName()`

- Signature: `public string GetSavedPlayerName()`
- Source range: `scripts/network/NetworkManager.cs:119`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `GetValue(...)`
  - Calls `AsString(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetOrCreateReconnectToken()`

- Signature: `public string GetOrCreateReconnectToken()`
- Source range: `scripts/network/NetworkManager.cs:130`
- Inputs:
  - `none`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `GetValue(...)`
  - Calls `AsString(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `NewGuid(...)`
  - Calls `ToString(...)`
  - Calls `SetValue(...)`
  - Calls `Save(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SavePlayerName(string playerName)`

- Signature: `public void SavePlayerName(string playerName)`
- Source range: `scripts/network/NetworkManager.cs:146`
- Inputs:
  - `string playerName`
- Output / side effects:
  - Reads/writes local filesystem or persistent config state.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `Trim(...)`
  - Calls `ConfigFile(...)`
  - Calls `Load(...)`
  - Calls `SetValue(...)`
  - Calls `GetValue(...)`
  - Calls `AsString(...)`
  - Calls `NewGuid(...)`
  - Calls `ToString(...)`
  - Calls `Save(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StartHostSession(string roomName, string playerName)`

- Signature: `public Error StartHostSession(string roomName, string playerName)`
- Source range: `scripts/network/NetworkManager.cs:162`
- Inputs:
  - `string roomName, string playerName`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `DisconnectSession(...)`
  - Calls `StopDiscovery(...)`
  - Calls `SavePlayerName(...)`
  - Calls `ENetMultiplayerPeer(...)`
  - Calls `CreateServer(...)`
  - Calls `MapIssueForSocketError(...)`
  - Calls `BuildSessionCreateFailureMessage(...)`
  - Calls `EmitSignal(...)`
  - Calls `EmitNetworkIssue(...)`
  - Calls `StartDiscovery(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `JoinRoomByIp(string hostAddress, string playerName, ParticipantRole role)`

- Signature: `public Error JoinRoomByIp(string hostAddress, string playerName, ParticipantRole role)`
- Source range: `scripts/network/NetworkManager.cs:194`
- Inputs:
  - `string hostAddress, string playerName, ParticipantRole role`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `DisconnectSession(...)`
  - Calls `SavePlayerName(...)`
  - Calls `ENetMultiplayerPeer(...)`
  - Calls `CreateClient(...)`
  - Calls `MapIssueForSocketError(...)`
  - Calls `BuildSessionCreateFailureMessage(...)`
  - Calls `EmitSignal(...)`
  - Calls `EmitNetworkIssue(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `DisconnectSession(string reason = "Disconnected", bool notifyServer = true)`

- Signature: `public void DisconnectSession(string reason = "Disconnected", bool notifyServer = true)`
- Source range: `scripts/network/NetworkManager.cs:218`
- Inputs:
  - `string reason = "Disconnected", bool notifyServer = true`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `BroadcastServerMessage(...)`
  - Calls `SendLeaveRoomRequest(...)`
  - Calls `Close(...)`
  - Calls `StopAdvertising(...)`
  - Calls `StartDiscovery(...)`
  - Calls `LeaveRoomLocally(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `StartDiscovery()`

- Signature: `public void StartDiscovery()`
- Source range: `scripts/network/NetworkManager.cs:246`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `PacketPeerUdp(...)`
  - Calls `SetBroadcastEnabled(...)`
  - Calls `Bind(...)`
  - Calls `MapIssueForSocketError(...)`
  - Calls `BuildDiscoveryBindFailureMessage(...)`
  - Calls `EmitNetworkIssue(...)`
  - Calls `Close(...)`
  - Calls `Start(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StopDiscovery()`

- Signature: `public void StopDiscovery()`
- Source range: `scripts/network/NetworkManager.cs:289`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Stop(...)`
  - Calls `Close(...)`
  - Calls `Clear(...)`
  - Calls `PublishDiscoveryIfNeeded(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StartAdvertising()`

- Signature: `private void StartAdvertising()`
- Source range: `scripts/network/NetworkManager.cs:304`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `PacketPeerUdp(...)`
  - Calls `SetBroadcastEnabled(...)`
  - Calls `Start(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `StopAdvertising()`

- Signature: `private void StopAdvertising()`
- Source range: `scripts/network/NetworkManager.cs:315`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Stop(...)`
  - Calls `Close(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PollDiscoveryPackets()`

- Signature: `private void PollDiscoveryPackets()`
- Source range: `scripts/network/NetworkManager.cs:326`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetAvailablePacketCount(...)`
  - Calls `GetPacket(...)`
  - Calls `GetPacketIP(...)`
  - Calls `GetString(...)`
  - Calls `ParseString(...)`
  - Calls `AsGodotDictionary(...)`
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `FromDictionary(...)`
  - Calls `Upsert(...)`
  - Calls `RemoveExpired(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PublishDiscoveryIfNeeded(bool force = false)`

- Signature: `private void PublishDiscoveryIfNeeded(bool force = false)`
- Source range: `scripts/network/NetworkManager.cs:371`
- Inputs:
  - `bool force = false`
- Output / side effects:
  - Emits signals to notify other systems.
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetUnixTimeFromSystem(...)`
  - Calls `ComputeFingerprint(...)`
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnAdvertiseTimerTimeout()`

- Signature: `private void OnAdvertiseTimerTimeout()`
- Source range: `scripts/network/NetworkManager.cs:395`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `IsServer(...)`
  - Calls `GetPreferredLocalAddress(...)`
  - Calls `BuildAdvertisement(...)`
  - Calls `ToDictionary(...)`
  - Calls `Stringify(...)`
  - Calls `GetBytes(...)`
  - Calls `SetDestAddress(...)`
  - Calls `PutPacket(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `EmitNetworkIssue(NetworkIssueCode issueCode, string message)`

- Signature: `private void EmitNetworkIssue(NetworkIssueCode issueCode, string message)`
- Source range: `scripts/network/NetworkManager.cs:420`
- Inputs:
  - `NetworkIssueCode issueCode, string message`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `IsAndroidRuntime()`

- Signature: `private static bool IsAndroidRuntime()`
- Source range: `scripts/network/NetworkManager.cs:426`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `HasFeature(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `MapIssueForSocketError(Error error, bool discoveryBind)`

- Signature: `private static NetworkIssueCode MapIssueForSocketError(Error error, bool discoveryBind)`
- Source range: `scripts/network/NetworkManager.cs:431`
- Inputs:
  - `Error error, bool discoveryBind`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsAndroidRuntime(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `AndroidPermissionHint()`

- Signature: `private static string AndroidPermissionHint()`
- Source range: `scripts/network/NetworkManager.cs:441`
- Inputs:
  - `none`
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

### `BuildSessionCreateFailureMessage(string operation, Error error)`

- Signature: `private static string BuildSessionCreateFailureMessage(string operation, Error error)`
- Source range: `scripts/network/NetworkManager.cs:446`
- Inputs:
  - `string operation, Error error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsAndroidRuntime(...)`
  - Calls `AndroidPermissionHint(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `BuildDiscoveryBindFailureMessage(Error error)`

- Signature: `private static string BuildDiscoveryBindFailureMessage(Error error)`
- Source range: `scripts/network/NetworkManager.cs:456`
- Inputs:
  - `Error error`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `IsAndroidRuntime(...)`
  - Calls `AndroidPermissionHint(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `GetPreferredLocalAddress()`

- Signature: `private static string GetPreferredLocalAddress()`
- Source range: `scripts/network/NetworkManager.cs:472`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetLocalAddresses(...)`
  - Calls `StartsWith(...)`
  - Calls `Contains(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnPeerConnected(long peerId)`

- Signature: `private void OnPeerConnected(long peerId)`
- Source range: `scripts/network/NetworkManager.cs:491`
- Inputs:
  - `long peerId`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnPeerDisconnected(long peerId)`

- Signature: `private void OnPeerDisconnected(long peerId)`
- Source range: `scripts/network/NetworkManager.cs:496`
- Inputs:
  - `long peerId`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Includes multiplayer authority/role validation guards.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `IsServer(...)`
  - Calls `ServerHandlePeerDisconnected(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Multiplayer-sensitive: behavior depends on authority and peer context.

### `OnConnectedToServer()`

- Signature: `private void OnConnectedToServer()`
- Source range: `scripts/network/NetworkManager.cs:506`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `SendJoinRequest(...)`
  - Calls `GetOrCreateReconnectToken(...)`
  - Calls `LoadLobby(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnConnectionFailed()`

- Signature: `private void OnConnectionFailed()`
- Source range: `scripts/network/NetworkManager.cs:513`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `DisconnectSession(...)`
  - Calls `LoadMainMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnServerDisconnected()`

- Signature: `private void OnServerDisconnected()`
- Source range: `scripts/network/NetworkManager.cs:520`
- Inputs:
  - `none`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
  - Calls `DisconnectSession(...)`
  - Calls `LoadMainMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnServerMessage(string message)`

- Signature: `private void OnServerMessage(string message)`
- Source range: `scripts/network/NetworkManager.cs:527`
- Inputs:
  - `string message`
- Output / side effects:
  - Emits signals to notify other systems.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `EmitSignal(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
