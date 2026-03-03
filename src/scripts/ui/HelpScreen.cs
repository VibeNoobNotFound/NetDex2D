using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NetDex.Managers;
using NetDex.UI.Game;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class HelpScreen : Control
{
    private static readonly PackedScene CardScene = GD.Load<PackedScene>("res://scenes/game/Card.tscn");

    private static readonly HelpStep[] AppSteps =
    {
        new(
            "welcome",
            "Welcome to NetDex",
            "[b]NetDex[/b] is a host-authoritative multiplayer Omi app. Start with flow, then focus on tactics.",
            "Learn flow first, then optimize decisions.",
            "In NetDex, the host process is the authoritative server for match state."
        ),
        new(
            "host",
            "Host a Room",
            "From main menu choose [b]Host[/b], set your name + room name, then create lobby. Host controls seat setup and match start.",
            "Host owns room setup and start authority.",
            "This is enforced in lobby/server validation."
        ),
        new(
            "join-lan",
            "Join via LAN Discovery",
            "Open [b]Join[/b], keep your name, choose role, pick a room from [b]LAN Rooms[/b], then join.",
            "Discovery is fastest when devices are on same subnet.",
            "Use refresh before assuming room is unavailable."
        ),
        new(
            "direct-ip",
            "Use Direct IP Fallback",
            "If discovery is blocked, type host IP into direct join and connect directly.",
            "Direct IP is the reliable fallback path.",
            "Useful on restrictive routers/mobile hotspots."
        ),
        new(
            "lobby",
            "Lobby Setup",
            "Lobby shows players/spectators, seat assignments, and host setup controls (AI autofill + difficulty).",
            "Seat map defines teams and in-game perspective.",
            "Non-host users cannot apply global seat/AI changes."
        ),
        new(
            "action-panel",
            "Match Action Panel",
            "Actions are phase-based: Shuffle/Cut/Trump/Kapothi. If you cannot act, your role/seat is not active for that phase.",
            "The UI only enables legal actions for the acting side.",
            "Snapshots from host drive what is enabled."
        ),
        new(
            "legal-play",
            "Legal Card Play",
            "In TrickPlay only current-turn seat can play. Follow suit is mandatory when possible.",
            "Turn order + follow-suit are strict.",
            "Server rejects illegal plays to keep all clients synchronized."
        ),
        new(
            "pause-reconnect",
            "Pause, Leave, Reconnect",
            "Pause menu supports resume/settings/leave. If a seated player disconnects, match can enter reconnect pause flow.",
            "Reconnect logic protects consistency.",
            "Spectators stay read-only through all match phases."
        )
    };

    private static readonly HelpStep[] OmiSteps =
    {
        new(
            "deck-teams",
            "Deck and Teams",
            "NetDex Omi uses a [b]32-card deck[/b] (7..A) across 4 suits. Opposite seats are teammates.",
            "Ace is highest strength in trick comparison.",
            "Classic references can vary by table, but NetDex uses this fixed model."
        ),
        new(
            "round-sequence",
            "Round Sequence",
            "Authoritative order: [b]Shuffle -> Cut -> FirstDeal -> TrumpSelect -> SecondDeal -> TrickPlay[/b].",
            "Phase order is deterministic.",
            "Timed holds are coordinated by server-side deadlines."
        ),
        new(
            "trump",
            "Trump Selection",
            "After first 4 cards each, trump selector chooses trump suit. Then second deal distributes final 4 cards.",
            "Trump can override lead-suit winner logic.",
            "NetDex applies trump as strict phase transition in rules engine."
        ),
        new(
            "winner",
            "Trick Winner Logic",
            "Winner is highest trump if any trump was played; otherwise highest lead-suit card.",
            "Lead suit matters unless trump appears.",
            "Use simulator cards below to test outcomes."
        ),
        new(
            "follow-suit",
            "Follow-Suit Rule",
            "If your hand contains lead suit, you must play that suit. Only void hands can deviate.",
            "Legal command generation enforces this.",
            "Try violating it in simulator to see real rule feedback."
        ),
        new(
            "kapothi",
            "Kapothi Window",
            "Kapothi opens once at first 4-0 checkpoint after trick resolve. Eligible team proposes/skips; target team accepts/rejects.",
            "Kapothi is team-gated and phase-gated.",
            "Timeout fallback: proposal auto-skip, response auto-reject."
        ),
        new(
            "scoring",
            "Round Scoring",
            "Base loss is trump-sensitive (2 if credit loser is trump team, else 1) + draw bonus (+0/+1). Accepted Kapothi adds +2.",
            "Loss is formula-based, not visual-only.",
            "This is NetDex implementation behavior."
        ),
        new(
            "match-end",
            "Match End",
            "Match ends when any team reaches 0 credits (or forfeit path). Sweep by tricks alone does not auto-end match.",
            "Credits decide match outcome.",
            "Classic Omi references may differ by table/region."
        )
    };

    private static readonly OmiScenario[] OmiScenarios =
    {
        new(
            "Trump Over Lead",
            "Trump suit is [b]Hearts[/b]. Pick the best card from your hand.",
            Card.SuitType.Hearts,
            new[]
            {
                new SimCard(Card.SuitType.Clubs, Card.RankType.King),
                new SimCard(Card.SuitType.Clubs, Card.RankType.Nine),
                new SimCard(Card.SuitType.Spades, Card.RankType.Ace)
            },
            new[] { "Left", "Top", "Right" },
            new[]
            {
                new SimCard(Card.SuitType.Hearts, Card.RankType.Eight),
                new SimCard(Card.SuitType.Hearts, Card.RankType.Queen),
                new SimCard(Card.SuitType.Spades, Card.RankType.Ten)
            },
            1,
            "Correct. A heart trump wins this trick.",
            "You can do better here. Use a stronger trump for clearer advantage."
        ),
        new(
            "Follow Suit Required",
            "Lead suit is [b]Spades[/b], and you have spades. You must follow suit.",
            null,
            new[]
            {
                new SimCard(Card.SuitType.Spades, Card.RankType.Ten),
                new SimCard(Card.SuitType.Spades, Card.RankType.Queen),
                new SimCard(Card.SuitType.Diamonds, Card.RankType.Ace)
            },
            new[] { "Left", "Top", "Right" },
            new[]
            {
                new SimCard(Card.SuitType.Spades, Card.RankType.Ace),
                new SimCard(Card.SuitType.Hearts, Card.RankType.King),
                new SimCard(Card.SuitType.Clubs, Card.RankType.Jack)
            },
            0,
            "Correct. You followed suit and took the trick with Ace of Spades.",
            "NetDex rule: if you hold lead suit, you must play it."
        ),
        new(
            "No Trump Played",
            "No trump this trick. Winner must come from lead suit only.",
            null,
            new[]
            {
                new SimCard(Card.SuitType.Diamonds, Card.RankType.Eight),
                new SimCard(Card.SuitType.Diamonds, Card.RankType.King),
                new SimCard(Card.SuitType.Hearts, Card.RankType.Ace)
            },
            new[] { "Left", "Top", "Right" },
            new[]
            {
                new SimCard(Card.SuitType.Diamonds, Card.RankType.Ace),
                new SimCard(Card.SuitType.Spades, Card.RankType.Ace),
                new SimCard(Card.SuitType.Clubs, Card.RankType.Seven)
            },
            0,
            "Correct. Lead suit applies; Ace of Diamonds wins.",
            "Without trump, non-lead suits cannot win this trick."
        )
    };

    private MarginContainer _outerMargin = null!;
    private PanelContainer _mainPanel = null!;

    private Button _appSectionButton = null!;
    private Button _omiSectionButton = null!;
    private VBoxContainer _appSection = null!;
    private VBoxContainer _omiSection = null!;

    private OptionButton _appStepOption = null!;
    private Label _appStepTitleLabel = null!;
    private RichTextLabel _appBodyRichText = null!;
    private Label _appTakeawayLabel = null!;
    private Label _appNoteLabel = null!;
    private ProgressBar _appProgressBar = null!;
    private Label _appProgressLabel = null!;
    private Button _appPrevButton = null!;
    private Button _appNextButton = null!;

    private OptionButton _omiStepOption = null!;
    private Label _omiStepTitleLabel = null!;
    private RichTextLabel _omiBodyRichText = null!;
    private Label _omiTakeawayLabel = null!;
    private Label _omiNoteLabel = null!;
    private ProgressBar _omiProgressBar = null!;
    private Label _omiProgressLabel = null!;
    private Button _omiPrevButton = null!;
    private Button _omiNextButton = null!;

    private PanelContainer _omiSimulatorPanel = null!;
    private Label _simulatorTitleLabel = null!;
    private Label _simulatorHintLabel = null!;
    private Label _simulatorResultLabel = null!;
    private HBoxContainer _currentTrickRow = null!;
    private HBoxContainer _yourHandRow = null!;

    private Button _backButton = null!;

    private readonly bool[][] _completed =
    {
        new bool[AppSteps.Length],
        new bool[OmiSteps.Length]
    };

    private readonly int[] _selectedStep = new int[2];
    private readonly List<Card> _activeHandCards = new();
    private readonly Dictionary<ulong, int> _handCardIndexById = new();

    private int _currentSection;
    private int _activeScenarioIndex = -1;
    private bool _isUpdating;
    private ulong _lastEvaluatedCardId;
    private double _lastEvaluationTime;

    public override void _Ready()
    {
        UiSettings.EnsureLoaded();
        _outerMargin = GetNode<MarginContainer>("ScrollContainer/MarginContainer");
        _mainPanel = GetNode<PanelContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel");

        _appSectionButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/SectionSelectorPanel/SectionSelectorMargin/SectionSelectorRow/AppSectionButton");
        _omiSectionButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/SectionSelectorPanel/SectionSelectorMargin/SectionSelectorRow/OmiSectionButton");
        _appSection = GetNode<VBoxContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection");
        _omiSection = GetNode<VBoxContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection");

        _appStepOption = GetNode<OptionButton>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppStepSelectorRow/AppStepOption");
        _appStepTitleLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppStepTitleLabel");
        _appBodyRichText = GetNode<RichTextLabel>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppContentPanel/AppContentMargin/AppContentVBox/AppBodyRichText");
        _appTakeawayLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppContentPanel/AppContentMargin/AppContentVBox/AppTakeawayLabel");
        _appNoteLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppContentPanel/AppContentMargin/AppContentVBox/AppNoteLabel");
        _appProgressBar = GetNode<ProgressBar>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppProgressRow/AppProgressBar");
        _appProgressLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppProgressRow/AppProgressLabel");
        _appPrevButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppNavigationRow/AppPrevButton");
        _appNextButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/AppSection/AppNavigationRow/AppNextButton");

        _omiStepOption = GetNode<OptionButton>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiStepSelectorRow/OmiStepOption");
        _omiStepTitleLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiStepTitleLabel");
        _omiBodyRichText = GetNode<RichTextLabel>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiContentPanel/OmiContentMargin/OmiContentVBox/OmiBodyRichText");
        _omiTakeawayLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiContentPanel/OmiContentMargin/OmiContentVBox/OmiTakeawayLabel");
        _omiNoteLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiContentPanel/OmiContentMargin/OmiContentVBox/OmiNoteLabel");
        _omiProgressBar = GetNode<ProgressBar>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiProgressRow/OmiProgressBar");
        _omiProgressLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiProgressRow/OmiProgressLabel");
        _omiPrevButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiNavigationRow/OmiPrevButton");
        _omiNextButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiNavigationRow/OmiNextButton");

        _omiSimulatorPanel = GetNode<PanelContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel");
        _simulatorTitleLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel/OmiSimulatorMargin/OmiSimulatorVBox/SimulatorTitleLabel");
        _simulatorHintLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel/OmiSimulatorMargin/OmiSimulatorVBox/SimulatorHintLabel");
        _simulatorResultLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel/OmiSimulatorMargin/OmiSimulatorVBox/SimulatorResultLabel");
        _currentTrickRow = GetNode<HBoxContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel/OmiSimulatorMargin/OmiSimulatorVBox/CurrentTrickRow");
        _yourHandRow = GetNode<HBoxContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/OmiSection/OmiSimulatorPanel/OmiSimulatorMargin/OmiSimulatorVBox/YourHandRow");

        _backButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/FooterRow/BackButton");

        _appSectionButton.Pressed += () => SetSection(0, true);
        _omiSectionButton.Pressed += () => SetSection(1, true);

        _appStepOption.ItemSelected += index => SelectStep(0, (int)index, true);
        _omiStepOption.ItemSelected += index => SelectStep(1, (int)index, true);

        _appPrevButton.Pressed += () => SelectStep(0, _selectedStep[0] - 1, true);
        _appNextButton.Pressed += () => SelectStep(0, _selectedStep[0] + 1, true);
        _omiPrevButton.Pressed += () => SelectStep(1, _selectedStep[1] - 1, true);
        _omiNextButton.Pressed += () => SelectStep(1, _selectedStep[1] + 1, true);

        _backButton.Pressed += OnBackPressed;

        VisibilityChanged += OnVisibilityChanged;
        GetViewport().SizeChanged += OnViewportSizeChanged;

        PopulateStepSelectors();
        ResetTutorialState();
        OnViewportSizeChanged();
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;
        if (GetViewport() != null)
        {
            GetViewport().SizeChanged -= OnViewportSizeChanged;
        }

        ClearCardRows();
    }

    private void PopulateStepSelectors()
    {
        _isUpdating = true;

        _appStepOption.Clear();
        for (var i = 0; i < AppSteps.Length; i++)
        {
            _appStepOption.AddItem($"{i + 1}. {AppSteps[i].Title}", i);
        }

        _omiStepOption.Clear();
        for (var i = 0; i < OmiSteps.Length; i++)
        {
            _omiStepOption.AddItem($"{i + 1}. {OmiSteps[i].Title}", i);
        }

        _isUpdating = false;
    }

    private void SetSection(int section, bool playCue)
    {
        _currentSection = Mathf.Clamp(section, 0, 1);

        _appSection.Visible = _currentSection == 0;
        _omiSection.Visible = _currentSection == 1;

        _appSectionButton.Disabled = _currentSection == 0;
        _omiSectionButton.Disabled = _currentSection == 1;

        if (playCue)
        {
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.62f, 0.01f);
            AnimateSection(_currentSection == 0 ? _appSection : _omiSection);
        }
    }

    private void SelectStep(int section, int stepIndex, bool animate)
    {
        if (_isUpdating)
        {
            return;
        }

        if (section == 0)
        {
            var clamped = Mathf.Clamp(stepIndex, 0, AppSteps.Length - 1);
            _selectedStep[0] = clamped;
            RenderAppStep(clamped, animate);
        }
        else
        {
            var clamped = Mathf.Clamp(stepIndex, 0, OmiSteps.Length - 1);
            _selectedStep[1] = clamped;
            RenderOmiStep(clamped, animate);
        }
    }

    private void RenderAppStep(int index, bool animate)
    {
        var step = AppSteps[index];

        _isUpdating = true;
        _appStepOption.Select(index);
        _isUpdating = false;

        _completed[0][index] = true;

        _appStepTitleLabel.Text = $"{index + 1}. {step.Title}";
        SetRichText(_appBodyRichText, step.BodyBbcode);
        _appTakeawayLabel.Text = $"Key takeaway: {step.KeyTakeaway}";
        _appNoteLabel.Text = $"Note: {step.Note}";

        _appPrevButton.Disabled = index == 0;
        _appNextButton.Disabled = index >= AppSteps.Length - 1;

        UpdateProgress(0);

        if (animate)
        {
            AnimateSection(_appSection);
        }
    }

    private void RenderOmiStep(int index, bool animate)
    {
        var step = OmiSteps[index];

        _isUpdating = true;
        _omiStepOption.Select(index);
        _isUpdating = false;

        _omiStepTitleLabel.Text = $"{index + 1}. {step.Title}";
        SetRichText(_omiBodyRichText, step.BodyBbcode);
        _omiTakeawayLabel.Text = $"Key takeaway: {step.KeyTakeaway}";
        _omiNoteLabel.Text = $"Note: {step.Note}";

        _omiPrevButton.Disabled = index == 0;
        _omiNextButton.Disabled = index >= OmiSteps.Length - 1;

        RenderScenarioForStep(index);

        if (_activeScenarioIndex < 0)
        {
            _completed[1][index] = true;
        }

        UpdateProgress(1);

        if (animate)
        {
            AnimateSection(_omiSection);
        }
    }

    private void RenderScenarioForStep(int stepIndex)
    {
        var scenarioIndex = stepIndex % OmiScenarios.Length;
        var scenario = OmiScenarios[scenarioIndex];
        _activeScenarioIndex = scenarioIndex;

        _omiSimulatorPanel.Visible = true;
        _simulatorTitleLabel.Text = scenario.Title;

        var trumpText = scenario.TrumpSuit.HasValue ? $"Trump: {scenario.TrumpSuit.Value}" : "Trump: None";
        _simulatorHintLabel.Text = $"{scenario.Hint}  {trumpText}";

        SetSimulatorResult("Pick a card from your hand to evaluate the trick.", UiSeverity.Info);

        BuildScenarioCards(scenario);
    }

    private void BuildScenarioCards(OmiScenario scenario)
    {
        ClearCardRows();

        for (var i = 0; i < scenario.TableCards.Length; i++)
        {
            var wrapper = new VBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center,
                CustomMinimumSize = new Vector2(86f, 132f)
            };

            var card = CreateTutorialCard(scenario.TableCards[i], false);
            wrapper.AddChild(card);

            var seatLabel = new Label
            {
                Text = i < scenario.TableSeatLabels.Length ? scenario.TableSeatLabels[i] : $"Seat {i + 1}",
                HorizontalAlignment = HorizontalAlignment.Center,
                ThemeTypeVariation = "Label"
            };
            wrapper.AddChild(seatLabel);

            _currentTrickRow.AddChild(wrapper);
        }

        for (var i = 0; i < scenario.HandCards.Length; i++)
        {
            var wrapper = new VBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center,
                CustomMinimumSize = new Vector2(86f, 120f)
            };

            var card = CreateTutorialCard(scenario.HandCards[i], true);
            _activeHandCards.Add(card);
            _handCardIndexById[card.GetInstanceId()] = i;

            card.CardClicked += OnTutorialCardChosen;
            card.CardSelected += OnTutorialCardChosen;

            wrapper.AddChild(card);
            _yourHandRow.AddChild(wrapper);
        }
    }

    private Card CreateTutorialCard(SimCard simCard, bool interactable)
    {
        var card = CardScene.Instantiate<Card>();
        card.CustomMinimumSize = new Vector2(78, 106);
        card.Scale = new Vector2(0.9f, 0.9f);
        card.Setup(simCard.Suit, simCard.Rank, true, $"tutorial-{simCard.Suit}-{simCard.Rank}");
        card.SetInteractable(interactable);
        return card;
    }

    private void OnTutorialCardChosen(Card card)
    {
        if (_activeScenarioIndex < 0)
        {
            return;
        }

        var instanceId = card.GetInstanceId();
        if (!_handCardIndexById.TryGetValue(instanceId, out var selectedIndex))
        {
            return;
        }

        var now = Time.GetUnixTimeFromSystem();
        if (_lastEvaluatedCardId == instanceId && now - _lastEvaluationTime < 0.1)
        {
            return;
        }

        _lastEvaluatedCardId = instanceId;
        _lastEvaluationTime = now;

        var scenario = OmiScenarios[_activeScenarioIndex];

        foreach (var active in _activeHandCards)
        {
            active.SetSelected(active == card);
        }

        var selectedCard = scenario.HandCards[selectedIndex];
        var leadSuit = scenario.TableCards[0].Suit;
        var hasLeadSuit = scenario.HandCards.Any(c => c.Suit == leadSuit);
        var isLegal = !hasLeadSuit || selectedCard.Suit == leadSuit;

        if (!isLegal)
        {
            SetSimulatorResult("Illegal play in NetDex: you must follow lead suit when possible.", UiSeverity.Error);
            _completed[1][_selectedStep[1]] = false;
            UpdateProgress(1);
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Error, 0.7f, 0.01f);
            return;
        }

        var trick = new List<SimCard>(scenario.TableCards) { selectedCard };
        var winnerIndex = DetermineTrickWinnerIndex(trick, scenario.TrumpSuit);
        var winnerName = winnerIndex == trick.Count - 1
            ? "You"
            : winnerIndex < scenario.TableSeatLabels.Length
                ? scenario.TableSeatLabels[winnerIndex]
                : $"Seat {winnerIndex + 1}";

        if (selectedIndex == scenario.RecommendedIndex)
        {
            SetSimulatorResult($"{scenario.SuccessText} Winner: {winnerName}.", UiSeverity.Success);
            _completed[1][_selectedStep[1]] = true;
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Success, 0.8f, 0.02f);
        }
        else
        {
            SetSimulatorResult($"{scenario.FailureText} Winner now: {winnerName}.", UiSeverity.Warning);
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Cancel, 0.74f, 0.02f);
        }

        UpdateProgress(1);
    }

    private static int DetermineTrickWinnerIndex(IReadOnlyList<SimCard> trickCards, Card.SuitType? trumpSuit)
    {
        var leadSuit = trickCards[0].Suit;
        var candidateIndexes = new List<int>();

        if (trumpSuit.HasValue && trickCards.Any(card => card.Suit == trumpSuit.Value))
        {
            for (var i = 0; i < trickCards.Count; i++)
            {
                if (trickCards[i].Suit == trumpSuit.Value)
                {
                    candidateIndexes.Add(i);
                }
            }
        }
        else
        {
            for (var i = 0; i < trickCards.Count; i++)
            {
                if (trickCards[i].Suit == leadSuit)
                {
                    candidateIndexes.Add(i);
                }
            }
        }

        var bestIndex = candidateIndexes[0];
        var bestRank = trickCards[bestIndex].Rank;

        for (var i = 1; i < candidateIndexes.Count; i++)
        {
            var index = candidateIndexes[i];
            if ((int)trickCards[index].Rank > (int)bestRank)
            {
                bestRank = trickCards[index].Rank;
                bestIndex = index;
            }
        }

        return bestIndex;
    }

    private void SetSimulatorResult(string text, UiSeverity severity)
    {
        _simulatorResultLabel.Text = text;
        UiFeedbackService.Instance?.ApplyStatusLabelStyle(_simulatorResultLabel, severity);
    }

    private void UpdateProgress(int section)
    {
        var total = section == 0 ? AppSteps.Length : OmiSteps.Length;
        var completed = _completed[section].Count(done => done);

        if (section == 0)
        {
            _appProgressBar.MaxValue = total;
            _appProgressBar.Value = completed;
            _appProgressLabel.Text = $"{completed}/{total} completed";
            return;
        }

        _omiProgressBar.MaxValue = total;
        _omiProgressBar.Value = completed;
        _omiProgressLabel.Text = $"{completed}/{total} completed";
    }

    private void OnBackPressed()
    {
        GameManager.Instance?.ReturnFromHelp();
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        ResetTutorialState();

        if (UiSettings.ReduceMotion)
        {
            _mainPanel.Modulate = Colors.White;
            _mainPanel.Scale = Vector2.One;
            return;
        }

        _mainPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _mainPanel.Scale = new Vector2(0.985f, 0.985f);

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_mainPanel, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_mainPanel, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void ResetTutorialState()
    {
        Array.Fill(_completed[0], false);
        Array.Fill(_completed[1], false);

        _currentSection = 0;
        _selectedStep[0] = 0;
        _selectedStep[1] = 0;

        SetSection(0, false);
        SelectStep(0, 0, false);
        SelectStep(1, 0, false);

        _appStepOption.GrabFocus();
    }

    private void OnViewportSizeChanged()
    {
        var width = GetViewportRect().Size.X;
        var compact = width < 980f;

        _outerMargin.AddThemeConstantOverride("margin_left", compact ? 12 : 20);
        _outerMargin.AddThemeConstantOverride("margin_top", compact ? 12 : 20);
        _outerMargin.AddThemeConstantOverride("margin_right", compact ? 12 : 20);
        _outerMargin.AddThemeConstantOverride("margin_bottom", compact ? 12 : 20);

        _mainPanel.CustomMinimumSize = compact
            ? new Vector2(0f, 1040f)
            : new Vector2(0f, 940f);

        _appSection.AddThemeConstantOverride("separation", compact ? 8 : 10);
        _omiSection.AddThemeConstantOverride("separation", compact ? 8 : 10);

        _currentTrickRow.AddThemeConstantOverride("separation", compact ? 8 : 12);
        _yourHandRow.AddThemeConstantOverride("separation", compact ? 8 : 12);
    }

    private static void AnimateSection(Control section)
    {
        if (UiSettings.ReduceMotion)
        {
            section.Modulate = Colors.White;
            return;
        }

        section.Modulate = new Color(1f, 1f, 1f, 0.88f);
        var tween = section.CreateTween();
        tween.TweenProperty(section, "modulate", Colors.White, (float)UiMotionProfile.MicroDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private static void SetRichText(RichTextLabel label, string bbcode)
    {
        label.BbcodeEnabled = true;
        label.Clear();
        label.AppendText(bbcode);
    }

    private void ClearCardRows()
    {
        _activeHandCards.Clear();
        _handCardIndexById.Clear();

        foreach (Node child in _currentTrickRow.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Node child in _yourHandRow.GetChildren())
        {
            child.QueueFree();
        }
    }

    private readonly record struct HelpStep(
        string Id,
        string Title,
        string BodyBbcode,
        string KeyTakeaway,
        string Note
    );

    private readonly record struct SimCard(
        Card.SuitType Suit,
        Card.RankType Rank
    );

    private readonly record struct OmiScenario(
        string Title,
        string Hint,
        Card.SuitType? TrumpSuit,
        SimCard[] TableCards,
        string[] TableSeatLabels,
        SimCard[] HandCards,
        int RecommendedIndex,
        string SuccessText,
        string FailureText
    );
}
