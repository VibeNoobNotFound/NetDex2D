# LobbyScreen (ui/LobbyScreen.cs)

- Source: `scripts/ui/LobbyScreen.cs`
- Namespace: `NetDex.UI.Lobby`
- Purpose: LobbyScreen in `ui/LobbyScreen.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.AI`
  - `NetDex.Core.Enums`
  - `NetDex.Lobby`
  - `NetDex.Managers`
  - `NetDex.Networking`
  - `System.Collections.Generic`
  - `System.Linq`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `LobbyManager`
  - `NetworkManager`
  - `NetworkRpc`
- Scene node paths used: `16`

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
| `RefreshLobbyView()` | `private` | `void` |
| `PopulateSeatOptions(RoomState room)` | `private` | `void` |
| `OnApplySeatsPressed()` | `private` | `void` |
| `PopulateAiDifficultyOptions()` | `private` | `void` |
| `SelectDifficulty(AiDifficulty difficulty)` | `private` | `void` |
| `OnApplyAiSettingsPressed()` | `private` | `void` |
| `OnStartMatchPressed()` | `private` | `void` |
| `OnLeavePressed()` | `private` | `void` |
| `OnReturnToGamePressed()` | `private` | `void` |
| `OnServerEventReceived(string eventType, string payloadJson)` | `private` | `void` |
| `OnNetworkMessage(string message)` | `private` | `void` |
| `OnServerMessage(string message)` | `private` | `void` |
| `SetStatus(string text)` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/LobbyScreen.cs:30`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `PopulateAiDifficultyOptions(...)`
  - Calls `RefreshLobbyView(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/ui/LobbyScreen.cs:70`
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

### `RefreshLobbyView()`

- Signature: `private void RefreshLobbyView()`
- Source range: `scripts/ui/LobbyScreen.cs:89`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Clear(...)`
  - Calls `OrderBy(...)`
  - Calls `ToString(...)`
  - Calls `AddItem(...)`
  - Calls `PopulateSeatOptions(...)`
  - Calls `SelectDifficulty(...)`
  - Calls `LoadGameScene(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PopulateSeatOptions(RoomState room)`

- Signature: `private void PopulateSeatOptions(RoomState room)`
- Source range: `scripts/ui/LobbyScreen.cs:157`
- Inputs:
  - `RoomState room`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Clear(...)`
  - Calls `AddItem(...)`
  - Calls `Where(...)`
  - Calls `OrderBy(...)`
  - Calls `ThenBy(...)`
  - Calls `GetItemId(...)`
  - Calls `Select(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnApplySeatsPressed()`

- Signature: `private void OnApplySeatsPressed()`
- Source range: `scripts/ui/LobbyScreen.cs:189`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `SetStatus(...)`
  - Calls `GetSelectedId(...)`
  - Calls `Where(...)`
  - Calls `GroupBy(...)`
  - Calls `FirstOrDefault(...)`
  - Calls `Count(...)`
  - Calls `TryGetValue(...)`
  - Calls `GetValues(...)`
  - Calls `SendSeatChangeRequest(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `PopulateAiDifficultyOptions()`

- Signature: `private void PopulateAiDifficultyOptions()`
- Source range: `scripts/ui/LobbyScreen.cs:228`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Clear(...)`
  - Calls `GetValues(...)`
  - Calls `AddItem(...)`
  - Calls `ToString(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `SelectDifficulty(AiDifficulty difficulty)`

- Signature: `private void SelectDifficulty(AiDifficulty difficulty)`
- Source range: `scripts/ui/LobbyScreen.cs:240`
- Inputs:
  - `AiDifficulty difficulty`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetItemId(...)`
  - Calls `Select(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnApplyAiSettingsPressed()`

- Signature: `private void OnApplyAiSettingsPressed()`
- Source range: `scripts/ui/LobbyScreen.cs:255`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `SetStatus(...)`
  - Calls `GetSelectedId(...)`
  - Calls `TryGetValue(...)`
  - Calls `SendSetAiOptionsRequest(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnStartMatchPressed()`

- Signature: `private void OnStartMatchPressed()`
- Source range: `scripts/ui/LobbyScreen.cs:271`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `SendStartMatchRequest(...)`
  - Calls `SetStatus(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnLeavePressed()`

- Signature: `private static void OnLeavePressed()`
- Source range: `scripts/ui/LobbyScreen.cs:277`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `DisconnectSession(...)`
  - Calls `LoadMainMenu(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnReturnToGamePressed()`

- Signature: `private static void OnReturnToGamePressed()`
- Source range: `scripts/ui/LobbyScreen.cs:283`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `LoadGameScene(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnServerEventReceived(string eventType, string payloadJson)`

- Signature: `private void OnServerEventReceived(string eventType, string payloadJson)`
- Source range: `scripts/ui/LobbyScreen.cs:288`
- Inputs:
  - `string eventType, string payloadJson`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `LoadGameScene(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnNetworkMessage(string message)`

- Signature: `private void OnNetworkMessage(string message)`
- Source range: `scripts/ui/LobbyScreen.cs:296`
- Inputs:
  - `string message`
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

### `OnServerMessage(string message)`

- Signature: `private void OnServerMessage(string message)`
- Source range: `scripts/ui/LobbyScreen.cs:301`
- Inputs:
  - `string message`
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

### `SetStatus(string text)`

- Signature: `private void SetStatus(string text)`
- Source range: `scripts/ui/LobbyScreen.cs:306`
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
