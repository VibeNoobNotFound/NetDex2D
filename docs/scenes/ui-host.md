# HostScreen (`scenes/ui/HostScreen.tscn`)

## Scene purpose

This scene drives `HostScreen` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/main/HostScreen.cs`
- `res://scripts/ui/common/MobileScrollContainer.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `HostScreen` | `Control` | `.` |
| `ScrollContainer` | `ScrollContainer` | `.` |
| `MainPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/CenterContainer` |
| `VBoxContainer` | `VBoxContainer` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel` |
| `Title` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `PlayerNameInput` | `LineEdit` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `RoomNameInput` | `LineEdit` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `HostIpLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `CreateHostButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `BackButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `StatusLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnBackPressed`
  - `OnCreateHostPressed`

## Scene transitions

- OnBackPressed -> GameManager.LoadMainMenu()
- OnCreateHostPressed -> GameManager.LoadLobby()

## Backend dependencies

- `GameManager`
- `NetworkManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
- Network startup/join failures surface through status/error labels and signals.
- Main risks are node path mismatches and invalid transition assumptions.
