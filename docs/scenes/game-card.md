# Card (`scenes/game/Card.tscn`)

## Scene purpose

This scene drives `Card` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/game/Card.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `Card` | `Control` | `.` |
| `TextureRect` | `TextureRect` | `.` |

## User interactions

- No explicit button handler wiring detected (may be passive/render-only scene).

## Scene transitions

- No direct scene transition call detected in attached script(s).

## Backend dependencies

- No backend singletons/managers referenced directly.

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
