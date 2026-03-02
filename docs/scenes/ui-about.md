# AboutScreen (`scenes/ui/AboutScreen.tscn`)

## Scene purpose

This scene drives `AboutScreen` (`Control`) in the NetDex UI/game flow.

## Attached scripts

- `res://scripts/ui/AboutScreen.cs`
- `res://scripts/ui/common/MobileScrollContainer.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `AboutScreen` | `Control` | `.` |
| `ScrollContainer` | `ScrollContainer` | `.` |
| `MainPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/CenterContainer` |
| `VBoxContainer` | `VBoxContainer` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel` |
| `TitleLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `GameNameLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `VersionLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `PlatformLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `DeveloperLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `CopyrightLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `RepoUrlLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `ReleasesUrlLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `LinksRow` | `HBoxContainer` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `OpenRepoButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/LinksRow` |
| `OpenReleasesButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/LinksRow` |
| `UpdateSectionLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `UpdateStatusLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `LatestVersionLabel` | `Label` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `CheckUpdatesButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `UpdateActionButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `SkipUpdateButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |
| `BackButton` | `Button` | `ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnBackPressed`
  - `OnCheckUpdatesPressed`
  - `OnOpenReleasesPressed`
  - `OnOpenRepoPressed`
  - `OnSkipUpdatePressed`
  - `OnUpdateActionPressed`

## Scene transitions

- OnBackPressed -> /root/GameManager.ReturnFromAbout()

## Backend dependencies

- `GameManager`
- `UpdateManager`

## Common failure points and debugging tips

- Node path dependencies are strict; renamed nodes can break `_Ready()` bindings.
- Updater UI depends on release fetch/network availability.
- Main risks are node path mismatches and invalid transition assumptions.
