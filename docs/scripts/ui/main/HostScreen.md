# HostScreen (ui/main/HostScreen.cs)

- Source: `scripts/ui/main/HostScreen.cs`
- Namespace: `NetDex.UI.Main`
- Purpose: HostScreen in `ui/main/HostScreen.cs`. Menu/lobby/settings/about/pause user interface scripts and interactions.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Managers`
  - `NetDex.Networking`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `NetworkManager`
- Scene node paths used: `6`

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
| `OnCreateHostPressed()` | `private` | `void` |
| `OnBackPressed()` | `private` | `void` |
| `OnConnectionStatusChanged(string status, string message)` | `private` | `void` |
| `OnNetworkMessage(string message)` | `private` | `void` |
| `OnNetworkIssueRaised(int issueCode, string message)` | `private` | `void` |
| `SetStatus(string status)` | `private` | `void` |

## Function-by-Function

### `_Ready()`

- Signature: `public override void _Ready()`
- Source range: `scripts/ui/main/HostScreen.cs:14`
- Inputs:
  - `none`
- Output / side effects:
  - Mutates local object state/fields.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `GetSavedPlayerName(...)`
  - Calls `GetLocalLanAddress(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `_ExitTree()`

- Signature: `public override void _ExitTree()`
- Source range: `scripts/ui/main/HostScreen.cs:35`
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

### `OnCreateHostPressed()`

- Signature: `private void OnCreateHostPressed()`
- Source range: `scripts/ui/main/HostScreen.cs:47`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `SetStatus(...)`
  - Calls `StartHostSession(...)`
  - Calls `LoadLobby(...)`
- Failure paths:
  - Handles Godot `Error` status outcomes from engine/network APIs.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `OnBackPressed()`

- Signature: `private static void OnBackPressed()`
- Source range: `scripts/ui/main/HostScreen.cs:61`
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

### `OnConnectionStatusChanged(string status, string message)`

- Signature: `private void OnConnectionStatusChanged(string status, string message)`
- Source range: `scripts/ui/main/HostScreen.cs:66`
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
- Source range: `scripts/ui/main/HostScreen.cs:71`
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

### `OnNetworkIssueRaised(int issueCode, string message)`

- Signature: `private void OnNetworkIssueRaised(int issueCode, string message)`
- Source range: `scripts/ui/main/HostScreen.cs:76`
- Inputs:
  - `int issueCode, string message`
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

### `SetStatus(string status)`

- Signature: `private void SetStatus(string status)`
- Source range: `scripts/ui/main/HostScreen.cs:81`
- Inputs:
  - `string status`
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
