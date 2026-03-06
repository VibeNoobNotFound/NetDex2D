# Main Root Scene (`main.tscn`)

## Scene purpose

This scene drives `Main` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- No script resources attached.

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `Main` | `Control` | `.` |
| `GlobalBackground` | `TextureRect` | `.` |
| `MainMenu` | `MainMenu` scene instance | `.` |
| `HostScreen` | `HostScreen` scene instance | `.` |
| `JoinScreen` | `JoinScreen` scene instance | `.` |
| `LobbyScreen` | `LobbyScreen` scene instance | `.` |
| `GameScreen` | `GameScreen` scene instance | `.` |
| `SettingsMenu` | `SettingsMenu` scene instance | `.` |
| `AboutScreen` | `AboutScreen` scene instance | `.` |
| `HelpScreen` | `HelpScreen` scene instance | `.` |

## User interactions

- No explicit button handler wiring detected (may be passive/render-only scene).

## Scene transitions

- No direct scene transition call detected in attached script(s).

## Backend dependencies

- No backend singletons/managers referenced directly.

## Common failure points and debugging tips

- Validate scene loads, node paths, and singleton availability in `_Ready()`.
