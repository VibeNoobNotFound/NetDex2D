# DiscoveryStore (network/stores/DiscoveryStore.cs)

- Source: `scripts/network/stores/DiscoveryStore.cs`
- Namespace: `NetDex.Networking.Stores`
- Purpose: DiscoveryStore in `network/stores/DiscoveryStore.cs`. Transport/session control, discovery, RPC bridge, and networking stores.

## Dependencies

- Key imports:
  - `NetDex.Lobby`
  - `System.Collections.Generic`
  - `System.Linq`
- Autoload/manager dependencies detected: none
- Scene node paths used: none

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `Upsert(RoomAdvertisement advertisement)` | `bool` | Public entry point |
| `RemoveExpired(double nowUnixSeconds, double expirySeconds)` | `bool` | Public entry point |
| `GetAll()` | `IReadOnlyList<RoomAdvertisement>` | Public entry point |
| `ComputeFingerprint()` | `string` | Public entry point |
| `Clear()` | `void` | Public entry point |

## Internal Methods

No internal/private methods.

## Function-by-Function

### `Upsert(RoomAdvertisement advertisement)`

- Signature: `public bool Upsert(RoomAdvertisement advertisement)`
- Source range: `scripts/network/stores/DiscoveryStore.cs:11`
- Inputs:
  - `RoomAdvertisement advertisement`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - Uses guard/branch checks before progressing logic.
  - Validates input/state with parse/look-up safety checks.
- Main call path (observed):
  - Calls `TryGetValue(...)`
  - Calls `Equals(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `RemoveExpired(double nowUnixSeconds, double expirySeconds)`

- Signature: `public bool RemoveExpired(double nowUnixSeconds, double expirySeconds)`
- Source range: `scripts/network/stores/DiscoveryStore.cs:23`
- Inputs:
  - `double nowUnixSeconds, double expirySeconds`
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

### `GetAll()`

- Signature: `public IReadOnlyList<RoomAdvertisement> GetAll()`
- Source range: `scripts/network/stores/DiscoveryStore.cs:43`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `OrderByDescending(...)`
  - Calls `ToList(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `ComputeFingerprint()`

- Signature: `public string ComputeFingerprint()`
- Source range: `scripts/network/stores/DiscoveryStore.cs:50`
- Inputs:
  - `none`
- Output / side effects:
  - Primarily computes/returns values with limited external side effects.
- Validation rules:
  - No dedicated validation block; relies on surrounding call flow and type safety.
- Main call path (observed):
  - Calls `Join(...)`
  - Calls `OrderBy(...)`
  - Calls `Select(...)`
- Failure paths:
  - No explicit hard-failure branch beyond early returns/flow checks.
- Multiplayer / threading notes:
  - Runs in standard Godot frame/event loop context unless caller schedules otherwise.

### `Clear()`

- Signature: `public void Clear()`
- Source range: `scripts/network/stores/DiscoveryStore.cs:57`
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
