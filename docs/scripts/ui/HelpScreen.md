# HelpScreen (ui/HelpScreen.cs)

- Source: `scripts/ui/HelpScreen.cs`
- Namespace: `NetDex.UI.Main`
- Purpose: Interactive learning screen with step cards for app controls and NetDex Omi rules.

## Dependencies

- Key imports:
  - `Godot`
  - `NetDex.Managers`
  - `NetDex.UI.Polish`
- Autoload/manager dependencies detected:
  - `GameManager`
  - `AudioManager`
  - `UiFeedbackService`
  - `UiSettings`
- Persistence:
  - `user://settings.cfg` (`help/last_tab`, `help/last_step_app`, `help/last_step_omi`)

## Signals

No custom `[Signal]` declarations in this file.

## Public API

| Method | Returns | Notes |
|---|---|---|
| `_Ready()` | `void` | Public entry point |
| `_ExitTree()` | `void` | Public lifecycle cleanup |

## Internal Methods

| Method | Access | Returns |
|---|---|---|
| `BindTab(string basePath)` | `private` | `TabUi` |
| `WireTabEvents(int tabIndex)` | `private` | `void` |
| `PopulateStepSelectors()` | `private` | `void` |
| `PopulateStepSelector(int tabIndex, HelpStep[] steps)` | `private` | `void` |
| `LoadPersistentState()` | `private` | `void` |
| `SavePersistentState()` | `private` | `void` |
| `OnTabChanged(long tab)` | `private` | `void` |
| `OnStepSelected(int tabIndex, int selectedIndex)` | `private` | `void` |
| `SelectStep(int tabIndex, int selectedIndex, bool animate, bool playCue)` | `private` | `void` |
| `RenderStep(int tabIndex, int stepIndex, bool animate)` | `private` | `void` |
| `OnChoiceSelected(int tabIndex, int choiceIndex)` | `private` | `void` |
| `GoPrev(int tabIndex)` | `private` | `void` |
| `GoNext(int tabIndex)` | `private` | `void` |
| `ResetCurrentTabProgress()` | `private` | `void` |
| `OnMetaClicked(int tabIndex, Variant meta)` | `private` | `void` |
| `ApplyTab(int tabIndex, bool animate)` | `private` | `void` |
| `StepsForTab(int tabIndex)` | `private static` | `HelpStep[]` |
| `SetFeedback(TabUi ui, string text, UiSeverity severity)` | `private` | `void` |
| `UpdateProgress(int tabIndex)` | `private` | `void` |
| `OnBackPressed()` | `private` | `void` |
| `OnVisibilityChanged()` | `private` | `void` |
| `OnViewportSizeChanged()` | `private` | `void` |
| `AnimateTabContent(Control root)` | `private static` | `void` |

## Function-by-Function

### `_Ready()`
Initializes node references, binds events, loads persisted tab/step state, and renders current tab.

### `_ExitTree()`
Unhooks lifecycle listeners (`VisibilityChanged`, viewport size changes).

### `BindTab(string basePath)`
Builds one tab UI binding from scene node paths.

### `WireTabEvents(int tabIndex)`
Connects selector, quick-check buttons, navigation buttons, and rich-text meta links for a tab.

### `PopulateStepSelectors()`
Fills lesson dropdowns for both tabs.

### `PopulateStepSelector(int tabIndex, HelpStep[] steps)`
Loads step titles into one `OptionButton`.

### `LoadPersistentState()`
Reads `user://settings.cfg` and restores last tab and step indices.

### `SavePersistentState()`
Stores last tab and selected step indices in `help` section.

### `OnTabChanged(long tab)`
Updates active tab content and plays a focus cue.

### `OnStepSelected(int tabIndex, int selectedIndex)`
Handles dropdown selection and delegates to step rendering.

### `SelectStep(int tabIndex, int selectedIndex, bool animate, bool playCue)`
Sets selected index, renders active tab state, persists indices, and optionally plays cue.

### `RenderStep(int tabIndex, int stepIndex, bool animate)`
Applies step title/body/question/choices/note, updates nav buttons and progress.

### `OnChoiceSelected(int tabIndex, int choiceIndex)`
Evaluates quick-check answer, marks completion on success, updates feedback/progress cues.

### `GoPrev(int tabIndex)`
Moves to previous step in tab.

### `GoNext(int tabIndex)`
Moves to next step in tab.

### `ResetCurrentTabProgress()`
Clears completion state for active tab and refreshes progress/feedback.

### `OnMetaClicked(int tabIndex, Variant meta)`
Supports deep links from rich text (`step:<n>` and `id:<stepId>`).

### `ApplyTab(int tabIndex, bool animate)`
Renders selected step for active tab and focuses step selector.

### `StepsForTab(int tabIndex)`
Returns step data array for selected tab (`AppSteps` or `OmiSteps`).

### `SetFeedback(TabUi ui, string text, UiSeverity severity)`
Updates feedback text and applies severity style.

### `UpdateProgress(int tabIndex)`
Updates progress bar and completion label for tab.

### `OnBackPressed()`
Returns to previous menu via `GameManager.ReturnFromHelp()`.

### `OnVisibilityChanged()`
Re-renders active tab and runs panel entry animation (unless reduce motion).

### `OnViewportSizeChanged()`
Applies compact layout overrides for small widths.

### `AnimateTabContent(Control root)`
Runs lightweight content fade for step transitions unless reduce motion.
