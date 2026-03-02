# JoinScreen (ui/main/JoinScreen.cs)

- Source: `scripts/ui/main/JoinScreen.cs`
- Namespace: `NetDex.UI.Main`
- Purpose: JoinScreen in `ui/main/JoinScreen.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Enums`
  - `NetDex.Lobby`
  - `NetDex.Managers`
  - `NetDex.Networking`
  - `System.Collections.Generic`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `NetworkManager`
- Scene node paths used: `10`

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `OnRefreshPressed()` | `private` | `void` |
| `OnJoinAsPlayerPressed()` | `private` | `void` |
| `OnJoinAsSpectatorPressed()` | `private` | `void` |
| `OnDirectJoinPressed()` | `private` | `void` |
| `OnDirectSpectatePressed()` | `private` | `void` |
| `OnBackPressed()` | `private` | `void` |
| `OnRoomSelected(long index)` | `private` | `void` |
| `RefreshRooms()` | `private` | `void` |
| `JoinSelectedRoom(ParticipantRole role)` | `private` | `void` |
| `JoinDirectIp(ParticipantRole role)` | `private` | `void` |
| `OnConnectionStatusChanged(string status, string message)` | `private` | `void` |
| `OnNetworkMessage(string message)` | `private` | `void` |
| `OnNetworkIssueRaised(int issueCode, string message)` | `private` | `void` |
| `SetStatus(string text)` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/main/JoinScreen.cs:20`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `GetSavedPlayerName(...)`
  - Calls `StartDiscovery(...)`
  - Calls `RefreshRooms(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/ui/main/JoinScreen.cs:47`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnRefreshPressed()`

- Signature: `private void OnRefreshPressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:60`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `ForceDiscoveryRefreshSignal(...)`
  - Calls `RefreshRooms(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnJoinAsPlayerPressed()`

- Signature: `private void OnJoinAsPlayerPressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:66`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `JoinSelectedRoom(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnJoinAsSpectatorPressed()`

- Signature: `private void OnJoinAsSpectatorPressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:71`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `JoinSelectedRoom(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnDirectJoinPressed()`

- Signature: `private void OnDirectJoinPressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:76`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `JoinDirectIp(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnDirectSpectatePressed()`

- Signature: `private void OnDirectSpectatePressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:81`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `JoinDirectIp(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnBackPressed()`

- Signature: `private static void OnBackPressed()`
- Source range: `scripts/ui/main/JoinScreen.cs:86`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `LoadMainMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnRoomSelected(long index)`

- Signature: `private void OnRoomSelected(long index)`
- Source range: `scripts/ui/main/JoinScreen.cs:91`
- Inputs:
  - `long index`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - No nested call path extracted (simple/inline logic).
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RefreshRooms()`

- Signature: `private void RefreshRooms()`
- Source range: `scripts/ui/main/JoinScreen.cs:102`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `Clear(...)`
  - Calls `GetDiscoveredRooms(...)`
  - Calls `AddItem(...)`
  - Calls `SetItemMetadata(...)`
  - Calls `Add(...)`
  - Calls `SetItemDisabled(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `Select(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `JoinSelectedRoom(ParticipantRole role)`

- Signature: `private void JoinSelectedRoom(ParticipantRole role)`
- Source range: `scripts/ui/main/JoinScreen.cs:139`
- Inputs:
  - `ParticipantRole role`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetSelectedItems(...)`
  - Calls `SetStatus(...)`
  - Calls `JoinRoomByIp(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `JoinDirectIp(ParticipantRole role)`

- Signature: `private void JoinDirectIp(ParticipantRole role)`
- Source range: `scripts/ui/main/JoinScreen.cs:167`
- Inputs:
  - `ParticipantRole role`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `Trim(...)`
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `SetStatus(...)`
  - Calls `JoinRoomByIp(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnConnectionStatusChanged(string status, string message)`

- Signature: `private void OnConnectionStatusChanged(string status, string message)`
- Source range: `scripts/ui/main/JoinScreen.cs:186`
- Inputs:
  - `string status, string message`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SetStatus(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnNetworkMessage(string message)`

- Signature: `private void OnNetworkMessage(string message)`
- Source range: `scripts/ui/main/JoinScreen.cs:191`
- Inputs:
  - `string message`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Contains(...)`
  - Calls `SetStatus(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnNetworkIssueRaised(int issueCode, string message)`

- Signature: `private void OnNetworkIssueRaised(int issueCode, string message)`
- Source range: `scripts/ui/main/JoinScreen.cs:202`
- Inputs:
  - `int issueCode, string message`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `SetStatus(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SetStatus(string text)`

- Signature: `private void SetStatus(string text)`
- Source range: `scripts/ui/main/JoinScreen.cs:214`
- Inputs:
  - `string text`
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
