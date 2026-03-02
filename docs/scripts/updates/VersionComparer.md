# VersionComparer (updates/VersionComparer.cs)

- Source: `scripts/updates/VersionComparer.cs`
- Namespace: `NetDex.Updates`
- Purpose: VersionComparer in `updates/VersionComparer.cs`. GitHub release updater state machine, checksum verification, and installers.

## Dependencies

- Key imports:
  - `System`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `IsNewer(string candidateVersion, string currentVersion)` | `bool` | Public entry point |
| `Compare(string leftVersion, string rightVersion)` | `int` | Public entry point |
| `NormalizeTagToVersion(string tag)` | `string` | Public entry point |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `ParsePart(string[] parts, int index)` | `private` | `int` |

## Function-by-Function

### `IsNewer(string candidateVersion, string currentVersion)`

- Signature: `public static bool IsNewer(string candidateVersion, string currentVersion)`
- Source range: `scripts/updates/VersionComparer.cs:7`
- Inputs:
  - `string candidateVersion, string currentVersion`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Compare(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Compare(string leftVersion, string rightVersion)`

- Signature: `public static int Compare(string leftVersion, string rightVersion)`
- Source range: `scripts/updates/VersionComparer.cs:12`
- Inputs:
  - `string leftVersion, string rightVersion`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
- Main call path (observed):
  - Calls `Parse(...)`
  - Calls `CompareTo(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `NormalizeTagToVersion(string tag)`

- Signature: `public static string NormalizeTagToVersion(string tag)`
- Source range: `scripts/updates/VersionComparer.cs:30`
- Inputs:
  - `string tag`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Parse(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ParsePart(string[] parts, int index)`

- Signature: `private static int ParsePart(string[] parts, int index)`
- Source range: `scripts/updates/VersionComparer.cs:70`
- Inputs:
  - `string[] parts, int index`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryParse(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
