# LobbyScreen (`scenes/ui/LobbyScreen.tscn`)

## Scene purpose

This scene drives `LobbyScreen` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/LobbyScreen.cs`
- `res://scripts/ui/common/MobileScrollContainer.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `LobbyScreen` | `Control` | `.` |
| `ScrollContainer` | `ScrollContainer` | `.` |
| `VBoxContainer` | `VBoxContainer` | `ScrollContainer/MarginContainer` |
| `RoomLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer` |
| `StatusLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer` |
| `HBox` | `HBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer` |
| `PlayersPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/VBoxContainer/HBox` |
| `PlayersVBox` | `VBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/PlayersPanel` |
| `PlayersLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/PlayersPanel/PlayersVBox` |
| `PlayersList` | `ItemList` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/PlayersPanel/PlayersVBox` |
| `SpectatorsPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/VBoxContainer/HBox` |
| `SpectatorsVBox` | `VBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/SpectatorsPanel` |
| `SpectatorsLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/SpectatorsPanel/SpectatorsVBox` |
| `SpectatorsList` | `ItemList` | `ScrollContainer/MarginContainer/VBoxContainer/HBox/SpectatorsPanel/SpectatorsVBox` |
| `SeatsPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/VBoxContainer` |
| `SeatsVBox` | `VBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel` |
| `SeatsTitle` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox` |
| `AiOptions` | `VBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox` |
| `AiAutoFillCheck` | `CheckButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions` |
| `AiDifficultyRow` | `HBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions` |
| `AiDifficultyLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions/AiDifficultyRow` |
| `AiDifficultyOption` | `OptionButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions/AiDifficultyRow` |
| `BottomLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `BottomSeatOption` | `OptionButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `RightLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `RightSeatOption` | `OptionButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `TopLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `TopSeatOption` | `OptionButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `LeftLabel` | `Label` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `LeftSeatOption` | `OptionButton` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid` |
| `SeatsActions` | `HBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox` |
| `ApplySeatsButton` | `Button` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsActions` |
| `AiActions` | `HBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox` |
| `ApplyAiSettingsButton` | `Button` | `ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiActions` |
| `Actions` | `HBoxContainer` | `ScrollContainer/MarginContainer/VBoxContainer` |
| `StartMatchButton` | `Button` | `ScrollContainer/MarginContainer/VBoxContainer/Actions` |
| `ReturnToGameButton` | `Button` | `ScrollContainer/MarginContainer/VBoxContainer/Actions` |
| `LeaveRoomButton` | `Button` | `ScrollContainer/MarginContainer/VBoxContainer/Actions` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnApplyAiSettingsPressed`
  - `OnApplySeatsPressed`
  - `OnLeavePressed`
  - `OnReturnToGamePressed`
  - `OnStartMatchPressed`

## Scene transitions

- OnLeavePressed -> GameManager.LoadMainMenu()
- OnReturnToGamePressed -> GameManager.LoadGameScene()
- OnServerEventReceived -> GameManager.LoadGameScene()
- RefreshLobbyView -> GameManager.LoadGameScene()

## Backend dependencies

- `GameManager`
- `LobbyManager`
- `NetworkManager`
- `NetworkRpc`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
- Network startup/join failures surface through status/error labels and signals.
- Main risks are node path mismatches and invalid transition assumptions.
