# SettingsMenu (`scenes/ui/SettingsMenu.tscn`)

## Scene purpose

This scene drives `SettingsMenu` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/SettingsMenu.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `SettingsMenu` | `Control` | `.` |
| `MainPanel` | `PanelContainer` | `CenterContainer` |
| `VBoxContainer` | `VBoxContainer` | `CenterContainer/MainPanel` |
| `Label` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `MusicValueLabel` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `MusicVolumeSlider` | `HSlider` | `CenterContainer/MainPanel/VBoxContainer` |
| `SfxValueLabel` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `SfxVolumeSlider` | `HSlider` | `CenterContainer/MainPanel/VBoxContainer` |
| `DisplayLabel` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `FullscreenToggle` | `CheckButton` | `CenterContainer/MainPanel/VBoxContainer` |
| `QualityLabel` | `Label` | `CenterContainer/MainPanel/VBoxContainer` |
| `ResolutionSlider` | `HSlider` | `CenterContainer/MainPanel/VBoxContainer` |
| `BackButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |
| `AboutButton` | `Button` | `CenterContainer/MainPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnAboutPressed`
  - `OnBackPressed`

## Scene transitions

- OnAboutPressed -> GameManager.LoadAboutScreen()
- OnBackPressed -> GameManager.ReturnFromSettings()

## Backend dependencies

- `AudioManager`
- `GameManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
