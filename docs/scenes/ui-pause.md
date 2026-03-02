# PauseMenu (`scenes/ui/PauseMenu.tscn`)

## Scene purpose

This scene drives `PauseMenu` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/PauseMenu.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `PauseMenu` | `Control` | `.` |
| `PanelContainer` | `PanelContainer` | `.` |
| `VBoxContainer` | `VBoxContainer` | `PanelContainer` |
| `Label` | `Label` | `PanelContainer/VBoxContainer` |
| `ResumeButton` | `Button` | `PanelContainer/VBoxContainer` |
| `SettingsButton` | `Button` | `PanelContainer/VBoxContainer` |
| `LeaveButton` | `Button` | `PanelContainer/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnLeavePressed`
  - `OnResumePressed`
  - `OnSettingsPressed`

## Scene transitions

- OnLeavePressed -> GameManager.LoadMainMenu()
- OnSettingsPressed -> GameManager.LoadSettingsMenu()

## Backend dependencies

- `GameManager`
- `NetworkManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
- Network startup/join failures surface through status/error labels and signals.
