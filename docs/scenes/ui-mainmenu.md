# MainMenu (`scenes/ui/MainMenu.tscn`)

## Scene purpose

This scene drives `MainMenu` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/MainMenu.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `MainMenu` | `Control` | `.` |
| `MainPanel` | `PanelContainer` | `CenterContainer` |
| `VBoxContainer` | `VBoxContainer` | `CenterContainer/MainPanel` |
| `Title` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `Subtitle` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `HostButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `JoinButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `HowToPlayButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `SettingsButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `AboutButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `ExitButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnAboutPressed`
  - `OnExitPressed`
  - `OnHostPressed`
  - `OnHowToPlayPressed`
  - `OnJoinPressed`
  - `OnSettingsPressed`

## Scene transitions

- OnAboutPressed -> GameManager.LoadAboutScreen()
- OnExitPressed -> application quit
- OnHostPressed -> GameManager.LoadHostScreen()
- OnHowToPlayPressed -> GameManager.LoadHelpScreen()
- OnJoinPressed -> GameManager.LoadJoinScreen()
- OnSettingsPressed -> GameManager.LoadSettingsMenu()

## Backend dependencies

- `GameManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
