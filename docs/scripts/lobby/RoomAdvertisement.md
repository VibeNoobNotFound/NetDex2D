# struct (lobby/RoomAdvertisement.cs)

- Source: `scripts/lobby/RoomAdvertisement.cs`
- Namespace: `NetDex.Lobby`
- Purpose: struct in `lobby/RoomAdvertisement.cs`. Room membership, seat assignment, match lifecycle state, and room snapshots.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Core.Enums`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `RoomAdvertisement(string RoomName, string HostName, string GameType, int Port, int PlayerCount, int SpectatorCount, string MatchState, string HostAddress, double LastSeenUnixSeconds, string RoomInstanceId, int ProtocolVersion)` | `record struct` | Public entry point |
| `ToDictionary()` | `Godot.Collections.Dictionary` | Public entry point |
| `FromDictionary(Godot.Collections.Dictionary dict)` | `RoomAdvertisement` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `RoomAdvertisement(string RoomName, string HostName, string GameType, int Port, int PlayerCount, int SpectatorCount, string MatchState, string HostAddress, double LastSeenUnixSeconds, string RoomInstanceId, int ProtocolVersion)`

- Signature: `public readonly record struct RoomAdvertisement(string RoomName, string HostName, string GameType, int Port, int PlayerCount, int SpectatorCount, string MatchState, string HostAddress, double LastSeenUnixSeconds, string RoomInstanceId, int ProtocolVersion)`
- Source range: `scripts/lobby/RoomAdvertisement.cs:6`
- Inputs:
  - `string RoomName, string HostName, string GameType, int Port, int PlayerCount, int SpectatorCount, string MatchState, string HostAddress, double LastSeenUnixSeconds, string RoomInstanceId, int ProtocolVersion`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `IsNullOrWhiteSpace(...)`
  - Calls `ToDictionary(...)`
  - Calls `FromDictionary(...)`
  - Calls `TryGetValue(...)`
  - Calls `AsString(...)`
  - Calls `AsInt32(...)`
  - Calls `ToString(...)`
  - Calls `AsDouble(...)`
  - Calls `GetUnixTimeFromSystem(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ToDictionary()`

- Signature: `public Godot.Collections.Dictionary ToDictionary()`
- Source range: `scripts/lobby/RoomAdvertisement.cs:23`
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

### `FromDictionary(Godot.Collections.Dictionary dict)`

- Signature: `public static RoomAdvertisement FromDictionary(Godot.Collections.Dictionary dict)`
- Source range: `scripts/lobby/RoomAdvertisement.cs:41`
- Inputs:
  - `Godot.Collections.Dictionary dict`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `RoomAdvertisement(...)`
  - Calls `TryGetValue(...)`
  - Calls `AsString(...)`
  - Calls `AsInt32(...)`
  - Calls `ToString(...)`
  - Calls `AsDouble(...)`
  - Calls `GetUnixTimeFromSystem(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.
