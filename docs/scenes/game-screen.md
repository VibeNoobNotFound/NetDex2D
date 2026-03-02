# GameScreen (`scenes/game/GameScreen.tscn`)

## Scene purpose

This scene drives `GameScreen` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/game/GameScreen.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `GameScreen` | `Control` | `.` |
| `AnimationLayer` | `Control` | `.` |
| `TrumpAnnouncementPanel` | `PanelContainer` | `AnimationLayer` |
| `AnnouncementLabel` | `Label` | `AnimationLayer/TrumpAnnouncementPanel/AnnouncementMargin` |
| `TopStatus` | `Label` | `.` |
| `BackLobbyButton` | `Button` | `.` |
| `MobilePauseButton` | `Button` | `.` |
| `Desk` | `Control` | `.` |
| `LeftTrickPile` | `Control` | `.` |
| `RightTrickPile` | `Control` | `.` |
| `LeftPileLabel` | `Label` | `.` |
| `RightPileLabel` | `Label` | `.` |
| `BottomPlayerName` | `Label` | `.` |
| `TopPlayerName` | `Label` | `.` |
| `LeftPlayerName` | `Label` | `.` |
| `RightPlayerName` | `Label` | `.` |
| `BottomHand` | `HBoxContainer` | `.` |
| `TopHand` | `HBoxContainer` | `.` |
| `LeftHand` | `VBoxContainer` | `.` |
| `RightHand` | `VBoxContainer` | `.` |
| `ActionPanel` | `PanelContainer` | `.` |
| `VBoxContainer` | `VBoxContainer` | `ActionPanel` |
| `ActionLabel` | `Label` | `ActionPanel/VBoxContainer` |
| `TrumpOption` | `OptionButton` | `ActionPanel/VBoxContainer` |
| `ActionButton` | `Button` | `ActionPanel/VBoxContainer` |
| `SecondaryActionButton` | `Button` | `ActionPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnActionButtonPressed`
  - `OnBackLobbyPressed`
  - `OnMobilePausePressed`
  - `OnSecondaryActionButtonPressed`

## Scene transitions

- OnBackLobbyPressed -> GameManager.LoadLobby()

## Backend dependencies

- `GameManager`
- `LobbyManager`
- `NetworkRpc`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
