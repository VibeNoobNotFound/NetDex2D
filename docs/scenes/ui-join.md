# JoinScreen (`scenes/ui/JoinScreen.tscn`)

## Scene purpose

This scene drives `JoinScreen` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/main/JoinScreen.cs`
- `res://scripts/ui/common/MobileScrollContainer.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `JoinScreen` | `Control` | `.` |
| `ScrollContainer` | `ScrollContainer` | `.` |
| `MainPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/CenterContainer` |
| `VBoxContainer` | `VBoxContainer` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel` |
| `Title` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `PlayerNameInput` | `LineEdit` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `RoomsLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `DiscoveredRooms` | `ItemList` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `RefreshRoomsButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `JoinButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `SpectateButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `DirectIpInput` | `LineEdit` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `DirectJoinButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `DirectSpectateButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `BackButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `StatusLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnBackPressed`
  - `OnDirectJoinPressed`
  - `OnDirectSpectatePressed`
  - `OnJoinAsPlayerPressed`
  - `OnJoinAsSpectatorPressed`
  - `OnRefreshPressed`

## Scene transitions

- OnBackPressed -> GameManager.LoadMainMenu()

## Backend dependencies

- `GameManager`
- `NetworkManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
- Network startup/join failures surface through status/error labels and signals.
- Main risks are node path mismatches and invalid transition assumptions.
