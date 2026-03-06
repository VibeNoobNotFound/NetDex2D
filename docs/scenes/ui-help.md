# HelpScreen (`scenes/ui/HelpScreen.tscn`)

## Scene purpose

Interactive tutorial screen with two learning sections:
- `App & Controls`
- `How Omi Works`

Uses step cards, quick checks, and per-section progress.

## Attached scripts

- `res://scripts/ui/HelpScreen.cs`

## Node tree summary (important nodes)

| Node | Type | Parent |
|---|---|---|
| `HelpScreen` | `Control` | `.` |
| `ScrollContainer` | `ScrollContainer` | `.` |
| `MainPanel` | `PanelContainer` | `ScrollContainer/MarginContainer/CenterContainer` |
| `HelpTabs` | `TabContainer` | `.../MainPanel/VBoxContainer` |
| `AppControlsTab` | `VBoxContainer` | `.../HelpTabs` |
| `OmiWorksTab` | `VBoxContainer` | `.../HelpTabs` |
| `StepOption` | `OptionButton` | `.../StepSelectorRow` |
| `BodyRichText` | `RichTextLabel` | `.../ContentPanel/ContentMargin/ContentVBox` |
| `QuickCheckPanel` | `PanelContainer` | `.../AppControlsTab` and `.../OmiWorksTab` |
| `ChoiceAButton` | `Button` | `.../QuickCheckVBox` |
| `ChoiceBButton` | `Button` | `.../QuickCheckVBox` |
| `ProgressBar` | `ProgressBar` | `.../ProgressRow` |
| `PrevButton` | `Button` | `.../NavigationRow` |
| `NextButton` | `Button` | `.../NavigationRow` |
| `RestartSectionButton` | `Button` | `.../FooterRow` |
| `BackButton` | `Button` | `.../FooterRow` |

## User interactions

- Button/input handlers discovered in attached scripts:
  - `OnStepSelected`
  - `OnChoiceSelected`
  - `GoPrev`
  - `GoNext`
  - `ResetCurrentTabProgress`
  - `OnBackPressed`
  - `OnMetaClicked`

## Scene transitions

- `BackButton` -> `GameManager.ReturnFromHelp()` (returns to caller screen)

## Backend dependencies

- `GameManager`
- `AudioManager`
- `UiFeedbackService`
- `UiSettings`

## Common failure points and debugging tips

- Keep tab child node names/paths stable; script bindings are explicit.
- If quick checks fail to update, verify `OptionButton`/button signal wiring for both tabs.
- If return navigation fails, verify `GameManager` includes `HelpScreen` in known screen list.
