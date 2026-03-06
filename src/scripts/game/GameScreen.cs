using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Serialization;
using NetDex.Lobby;
using NetDex.Managers;
using NetDex.Networking;
using NetDex.UI.Polish;

namespace NetDex.UI.Game;

public partial class GameScreen : Control
{
    private const int DealSourceDeckLayers = 8;

    private enum VisualSlot
    {
        Bottom = 0,
        Right = 1,
        Top = 2,
        Left = 3
    }

    private HBoxContainer _bottomHand = null!;
    private HBoxContainer _topHand = null!;
    private VBoxContainer _leftHand = null!;
    private VBoxContainer _rightHand = null!;
    private Control _desk = null!;
    private Control _leftTrickPile = null!;
    private Control _rightTrickPile = null!;
    private Label _leftPileLabel = null!;
    private Label _rightPileLabel = null!;

    private Label _bottomPlayerName = null!;
    private Label _topPlayerName = null!;
    private Label _leftPlayerName = null!;
    private Label _rightPlayerName = null!;

    private Label _statusLabel = null!;
    private Button _mobilePauseButton = null!;
    private PanelContainer _actionPanel = null!;
    private Label _actionLabel = null!;
    private OptionButton _trumpOption = null!;
    private Button _actionButton = null!;
    private Button _secondaryActionButton = null!;
    private Control _animationLayer = null!;
    private Control _dealSourceAnchor = null!;
    private Control _dealSourceStack = null!;
    private PanelContainer _trumpAnnouncementPanel = null!;
    private Label _trumpAnnouncementLabel = null!;
    private PanelContainer _roundResultPanel = null!;
    private Label _roundResultLabel = null!;
    private PanelContainer _kapothiBannerPanel = null!;
    private Label _kapothiBannerLabel = null!;
    private ColorRect _focusVignette = null!;

    private PackedScene _cardScene = null!;

    private AudioStreamOggVorbis[] _placeSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamOggVorbis[] _slideSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamOggVorbis _dealSlideSound = null!;
    private AudioStreamPlayer _sfxPlayer = null!;

    private SeatPosition? _localSeat;
    private ParticipantRole _localRole = ParticipantRole.Spectator;

    private readonly Dictionary<SeatPosition, Card> _deskCards = new();
    private readonly List<Card> _friendlyPileCards = new();
    private readonly List<Card> _enemyPileCards = new();
    private readonly List<Card> _dealSourceDeckCards = new();

    private int _lastRoundNumber = -1;
    private int _lastCompletedTricksCount = -1;
    private readonly int[] _lastTeamTricks = new int[2];

    private int _pendingFriendlyTricks;
    private int _pendingEnemyTricks;
    private bool _isCollectingTrick;
    private bool _isDealAnimationRunning;
    private OmiPhase? _lastPhase;
    private OmiPhase? _lastPhaseCue;
    private int _lastTrumpSuit = -1;
    private int _lastRoundBannerShown = -1;
    private SeatPosition? _lastTurnSeat;
    private int _dealAnimationRunId;
    private bool _suppressInitialDeskEntryAudio = true;
    private Godot.Collections.Dictionary _pendingSnapshot = new();

    public override void _Ready()
    {
        _bottomHand = GetNode<HBoxContainer>("BottomHand");
        _topHand = GetNode<HBoxContainer>("TopHand");
        _leftHand = GetNode<VBoxContainer>("LeftHand");
        _rightHand = GetNode<VBoxContainer>("RightHand");
        _desk = GetNode<Control>("Desk");
        _leftTrickPile = GetNode<Control>("LeftTrickPile");
        _rightTrickPile = GetNode<Control>("RightTrickPile");
        _leftPileLabel = GetNode<Label>("LeftPileLabel");
        _rightPileLabel = GetNode<Label>("RightPileLabel");

        _bottomPlayerName = GetNode<Label>("BottomPlayerName");
        _topPlayerName = GetNode<Label>("TopPlayerName");
        _leftPlayerName = GetNode<Label>("LeftPlayerName");
        _rightPlayerName = GetNode<Label>("RightPlayerName");

        _statusLabel = GetNode<Label>("TopStatus");
        _mobilePauseButton = GetNode<Button>("MobilePauseButton");
        _actionPanel = GetNode<PanelContainer>("ActionPanel");
        _actionLabel = GetNode<Label>("ActionPanel/VBoxContainer/ActionLabel");
        _trumpOption = GetNode<OptionButton>("ActionPanel/VBoxContainer/TrumpOption");
        _actionButton = GetNode<Button>("ActionPanel/VBoxContainer/ActionButton");
        _secondaryActionButton = GetNode<Button>("ActionPanel/VBoxContainer/SecondaryActionButton");
        _animationLayer = GetNode<Control>("AnimationLayer");
        _dealSourceAnchor = GetNode<Control>("AnimationLayer/DealSourceAnchor");
        _dealSourceStack = GetNode<Control>("AnimationLayer/DealSourceAnchor/DealSourceStack");
        _trumpAnnouncementPanel = GetNode<PanelContainer>("AnimationLayer/TrumpAnnouncementPanel");
        _trumpAnnouncementLabel = GetNode<Label>("AnimationLayer/TrumpAnnouncementPanel/AnnouncementMargin/AnnouncementLabel");
        _roundResultPanel = GetNode<PanelContainer>("AnimationLayer/RoundResultPanel");
        _roundResultLabel = GetNode<Label>("AnimationLayer/RoundResultPanel/RoundResultMargin/RoundResultLabel");
        _kapothiBannerPanel = GetNode<PanelContainer>("AnimationLayer/KapothiBannerPanel");
        _kapothiBannerLabel = GetNode<Label>("AnimationLayer/KapothiBannerPanel/KapothiBannerMargin/KapothiBannerLabel");
        _focusVignette = GetNode<ColorRect>("FocusVignette");
        _dealSourceAnchor.Visible = false;
        _roundResultPanel.Visible = false;
        _kapothiBannerPanel.Visible = false;
        ConfigureSeatLabelVisuals();

        GetNode<Button>("BackLobbyButton").Pressed += OnBackLobbyPressed;
        _mobilePauseButton.Visible = OS.HasFeature("android") || OS.HasFeature("ios");
        _mobilePauseButton.Pressed += OnMobilePausePressed;
        _actionButton.Pressed += OnActionButtonPressed;
        _secondaryActionButton.Pressed += OnSecondaryActionButtonPressed;

        _trumpOption.Clear();
        _trumpOption.AddItem("Hearts", (int)CardSuit.Hearts);
        _trumpOption.AddItem("Diamonds", (int)CardSuit.Diamonds);
        _trumpOption.AddItem("Clubs", (int)CardSuit.Clubs);
        _trumpOption.AddItem("Spades", (int)CardSuit.Spades);

        _cardScene = GD.Load<PackedScene>("res://scenes/game/Card.tscn");
        BuildDealSourceDeckVisual();

        _placeSounds = new[]
        {
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace1.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace2.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace3.ogg")
        };
        _slideSounds = new[]
        {
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide1.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide2.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide3.ogg")
        };
        _dealSlideSound = GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide2.ogg");

        _sfxPlayer = new AudioStreamPlayer();
        _sfxPlayer.Bus = AudioManager.GetSfxBusName();
        AddChild(_sfxPlayer);

        LobbyManager.Instance.MatchSnapshotChanged += OnMatchSnapshotChanged;
        LobbyManager.Instance.RoomStateChanged += OnRoomStateChanged;

        RefreshLocalIdentity();
        RenderSnapshot(LobbyManager.Instance.LocalMatchSnapshot);
    }

    public override void _ExitTree()
    {
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.MatchSnapshotChanged -= OnMatchSnapshotChanged;
            LobbyManager.Instance.RoomStateChanged -= OnRoomStateChanged;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            TogglePauseMenu();
            GetViewport().SetInputAsHandled();
        }
    }

    private void OnRoomStateChanged()
    {
        RefreshLocalIdentity();
        RenderSnapshot(LobbyManager.Instance.LocalMatchSnapshot);
    }

    private void OnMatchSnapshotChanged()
    {
        RenderSnapshot(LobbyManager.Instance.LocalMatchSnapshot);
    }

    private void RefreshLocalIdentity()
    {
        _localSeat = LobbyManager.Instance.GetLocalSeat();
        _localRole = LobbyManager.Instance.GetLocalRole();
    }

    private void RenderSnapshot(Godot.Collections.Dictionary snapshot)
    {
        RenderSeatNames();

        if (snapshot.Count == 0)
        {
            _statusLabel.Text = "Waiting for match state...";
            _actionPanel.Visible = false;
            ResetBoardVisualState();
            _lastPhase = null;
            _lastTrumpSuit = -1;
            _lastPhaseCue = null;
            _pendingSnapshot = new Godot.Collections.Dictionary();
            _suppressInitialDeskEntryAudio = true;
            return;
        }

        var phase = snapshot.TryGetValue("phase", out var phaseVariant)
            ? (OmiPhase)phaseVariant.AsInt32()
            : OmiPhase.LobbySeating;

        var trumpSuit = snapshot.TryGetValue("trumpSuit", out var trumpVariant) ? trumpVariant.AsInt32() : -1;
        var roundNumber = snapshot.TryGetValue("roundNumber", out var roundVariant) ? roundVariant.AsInt32() : 0;

        var currentTurnSeat = SeatPosition.Bottom;
        if (snapshot.TryGetValue("currentTurnSeat", out var turnVariant))
        {
            currentTurnSeat = NetDex.Core.Enums.SeatPositionExtensions.Parse(turnVariant.AsString()) ?? SeatPosition.Bottom;
        }

        var teamCredits = ExtractPair(snapshot, "teamCredits", 10, 10);
        var teamTricks = ExtractPair(snapshot, "teamTricks", 0, 0);
        UpdateSideHandLayout(snapshot);
        var pendingDrawBonus = snapshot.TryGetValue("pendingDrawBonusCredits", out var drawBonusVariant)
            ? drawBonusVariant.AsInt32()
            : 0;
        _statusLabel.Text = BuildStatusText(phase, roundNumber, currentTurnSeat, trumpSuit, teamCredits, teamTricks, pendingDrawBonus);
        UpdateTurnSpotlight(currentTurnSeat, phase);
        PlayPhaseCueIfNeeded(phase);
        HandlePhaseVisualEvents(phase, roundNumber, teamCredits, teamTricks);

        TryStartPhaseAnimation(snapshot, phase, trumpSuit);
        if (_isDealAnimationRunning)
        {
            _pendingSnapshot = snapshot.Duplicate(true);
            RenderActionPanel(snapshot, phase);
            return;
        }

        ClearHands();
        RenderHands(snapshot, phase, currentTurnSeat);
        RenderTrickState(snapshot, roundNumber, currentTurnSeat, teamTricks, _suppressInitialDeskEntryAudio);
        RenderActionPanel(snapshot, phase);
        if (_lastTurnSeat != currentTurnSeat)
        {
            PulseSeatLabel(currentTurnSeat);
            _lastTurnSeat = currentTurnSeat;
        }
        _lastPhase = phase;
        _lastTrumpSuit = trumpSuit;
        _suppressInitialDeskEntryAudio = false;
    }

    private string BuildStatusText(OmiPhase phase, int roundNumber, SeatPosition currentTurnSeat, int trumpSuit, int[] teamCredits, int[] teamTricks, int pendingDrawBonus)
    {
        var creditsText = $"{teamCredits[0]} - {teamCredits[1]}";
        var tricksText = $"{teamTricks[0]} - {teamTricks[1]}";
        var trumpText = trumpSuit >= 0 ? ((CardSuit)trumpSuit).ToString() : "Not selected";
        var phaseText = phase switch
        {
            OmiPhase.FirstDeal => "Dealing first 4 cards...",
            OmiPhase.SecondDeal => "Trump selected. Dealing remaining cards...",
            OmiPhase.TrickResolveHold => "Resolving trick...",
            OmiPhase.KapothiProposal => "Kapothi opportunity",
            OmiPhase.KapothiResponse => "Kapothi proposed, waiting response",
            _ => $"Phase: {phase}"
        };

        return $"Round {roundNumber} | {phaseText} | Turn: {BuildSeatTitle(currentTurnSeat)} | Trump: {trumpText} | Credits: {creditsText} | Tricks: {tricksText} | Draw Bonus: +{pendingDrawBonus}";
    }

    private void TryStartPhaseAnimation(Godot.Collections.Dictionary snapshot, OmiPhase phase, int trumpSuit)
    {
        if (_isDealAnimationRunning)
        {
            return;
        }

        var shufflerSeat = ParseSeat(snapshot, "shufflerSeat");
        var trumpSelectorSeat = ParseSeat(snapshot, "trumpSelectorSeat");
        if (!shufflerSeat.HasValue || !trumpSelectorSeat.HasValue)
        {
            return;
        }

        if (phase == OmiPhase.FirstDeal && _lastPhase != OmiPhase.FirstDeal)
        {
            _ = RunDealAnimationSequence(shufflerSeat.Value, trumpSelectorSeat.Value, includeTrumpAnnouncement: false, trumpSuit);
            return;
        }

        if (phase == OmiPhase.SecondDeal && (_lastPhase != OmiPhase.SecondDeal || trumpSuit != _lastTrumpSuit))
        {
            _ = RunDealAnimationSequence(shufflerSeat.Value, trumpSelectorSeat.Value, includeTrumpAnnouncement: true, trumpSuit);
        }
    }

    private static int[] ExtractPair(Godot.Collections.Dictionary snapshot, string key, int fallbackLeft, int fallbackRight)
    {
        if (!snapshot.TryGetValue(key, out var pairVariant) || pairVariant.VariantType != Variant.Type.Array)
        {
            return new[] { fallbackLeft, fallbackRight };
        }

        var pair = pairVariant.AsGodotArray();
        return pair.Count >= 2
            ? new[] { pair[0].AsInt32(), pair[1].AsInt32() }
            : new[] { fallbackLeft, fallbackRight };
    }

    private static SeatPosition? ParseSeat(Godot.Collections.Dictionary snapshot, string key)
    {
        return snapshot.TryGetValue(key, out var variant)
            ? NetDex.Core.Enums.SeatPositionExtensions.Parse(variant.AsString())
            : null;
    }

    private void ConfigureSeatLabelVisuals()
    {
        ConfigureSideSeatLabel(_leftPlayerName);
        ConfigureSideSeatLabel(_rightPlayerName);
        _leftPlayerName.ZIndex = 260;
        _rightPlayerName.ZIndex = 260;
    }

    private static void ConfigureSideSeatLabel(Label label)
    {
        label.AutowrapMode = TextServer.AutowrapMode.Off;
        label.Set("clip_text", true);
        label.Set("text_overrun_behavior", 3);
    }

    private void UpdateSideHandLayout(Godot.Collections.Dictionary snapshot)
    {
        var handCounts = ReadHandCounts(snapshot);
        var visibleHands = ReadVisibleHands(snapshot);
        var counts = new Dictionary<SeatPosition, int>();

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            var visibleCards = ReadVisibleCardsForSeat(visibleHands, seat);
            counts[seat] = ResolveSeatHandCount(handCounts, seat, visibleCards.Count);
        }

        UpdateSideHandLayoutFromCounts(counts);
    }

    private void UpdateSideHandLayoutFromCounts(IDictionary<SeatPosition, int> counts)
    {
        var leftSeat = ToSeatForVisualSlot(VisualSlot.Left);
        var rightSeat = ToSeatForVisualSlot(VisualSlot.Right);

        var leftCount = counts.TryGetValue(leftSeat, out var resolvedLeftCount) ? resolvedLeftCount : _leftHand.GetChildCount();
        var rightCount = counts.TryGetValue(rightSeat, out var resolvedRightCount) ? resolvedRightCount : _rightHand.GetChildCount();

        ApplySideHandSeparation(_leftHand, leftCount);
        ApplySideHandSeparation(_rightHand, rightCount);
    }

    private static void ApplySideHandSeparation(VBoxContainer container, int cardCount)
    {
        const float cardHeight = 95f;
        const int defaultSeparation = -44;

        if (cardCount <= 1)
        {
            container.AddThemeConstantOverride("separation", defaultSeparation);
            return;
        }

        var measuredHeight = container.Size.Y > 2f ? container.Size.Y : container.GetGlobalRect().Size.Y;
        if (measuredHeight <= 2f)
        {
            measuredHeight = 376f;
        }

        var availableHeight = Math.Max(cardHeight + 4f, measuredHeight - 6f);
        var step = (availableHeight - cardHeight) / Math.Max(1, cardCount - 1);
        step = (float)Math.Clamp(step, 30f, 52f);
        var separation = (int)Math.Round(step - cardHeight);
        container.AddThemeConstantOverride("separation", separation);
    }

    private static Godot.Collections.Dictionary ReadHandCounts(Godot.Collections.Dictionary snapshot)
    {
        return snapshot.TryGetValue("handCounts", out var handCountsVariant) && handCountsVariant.VariantType == Variant.Type.Dictionary
            ? handCountsVariant.AsGodotDictionary()
            : new Godot.Collections.Dictionary();
    }

    private static Godot.Collections.Dictionary ReadVisibleHands(Godot.Collections.Dictionary snapshot)
    {
        return snapshot.TryGetValue("visibleHands", out var visibleHandsVariant) && visibleHandsVariant.VariantType == Variant.Type.Dictionary
            ? visibleHandsVariant.AsGodotDictionary()
            : new Godot.Collections.Dictionary();
    }

    private static Godot.Collections.Array ReadVisibleCardsForSeat(Godot.Collections.Dictionary visibleHands, SeatPosition seat)
    {
        if (visibleHands.TryGetValue(seat.ToString(), out var cardsVariant) && cardsVariant.VariantType == Variant.Type.Array)
        {
            return cardsVariant.AsGodotArray();
        }

        return new Godot.Collections.Array();
    }

    private static int ResolveSeatHandCount(Godot.Collections.Dictionary handCounts, SeatPosition seat, int visibleCardsCount)
    {
        return handCounts.TryGetValue(seat.ToString(), out var countVariant)
            ? countVariant.AsInt32()
            : visibleCardsCount;
    }

    private Dictionary<SeatPosition, int> BuildCurrentHandCounts()
    {
        var counts = new Dictionary<SeatPosition, int>();
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            counts[seat] = GetHandContainer(seat).GetChildCount();
        }

        return counts;
    }

    private Dictionary<SeatPosition, int> BuildTargetHandCounts(Godot.Collections.Dictionary snapshot)
    {
        var handCounts = ReadHandCounts(snapshot);
        var visibleHands = ReadVisibleHands(snapshot);
        var target = new Dictionary<SeatPosition, int>();

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            var visibleCards = ReadVisibleCardsForSeat(visibleHands, seat);
            target[seat] = ResolveSeatHandCount(handCounts, seat, visibleCards.Count);
        }

        return target;
    }

    private bool TryGetVisibleCardModel(Godot.Collections.Dictionary snapshot, SeatPosition seat, int index, out CardModel model)
    {
        var visibleHands = ReadVisibleHands(snapshot);
        var visibleCards = ReadVisibleCardsForSeat(visibleHands, seat);
        if (index < 0 || index >= visibleCards.Count)
        {
            model = default;
            return false;
        }

        var item = visibleCards[index];
        if (item.VariantType != Variant.Type.Dictionary)
        {
            model = default;
            return false;
        }

        model = CardModelConversions.FromDictionary(item.AsGodotDictionary());
        return true;
    }

    private bool ShouldRevealDealtCardFace(SeatPosition actualSeat)
    {
        return _localRole == ParticipantRole.Player && ToVisualSlot(actualSeat) == VisualSlot.Bottom;
    }

    private void EnsureDealBaselineCards(
        Godot.Collections.Dictionary snapshot,
        IReadOnlyDictionary<SeatPosition, int> targetCounts,
        IDictionary<SeatPosition, int> currentCounts,
        int baselineCount)
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (!targetCounts.TryGetValue(seat, out var targetCount))
            {
                continue;
            }

            var desiredBaseline = Math.Min(targetCount, baselineCount);
            while (currentCounts[seat] < desiredBaseline)
            {
                var index = currentCounts[seat];
                CardModel? visibleModel = null;
                if (ShouldRevealDealtCardFace(seat) && TryGetVisibleCardModel(snapshot, seat, index, out var resolvedVisibleCard))
                {
                    visibleModel = resolvedVisibleCard;
                }

                var model = visibleModel ?? new CardModel($"hidden-{seat}-{index}", CardSuit.Spades, CardRank.Ace);
                var faceUp = visibleModel.HasValue;
                GetHandContainer(seat).AddChild(CreateCard(model, faceUp, false));
                currentCounts[seat] += 1;
            }
        }

        UpdateSideHandLayoutFromCounts(currentCounts);
    }

    private void BuildDealSourceDeckVisual()
    {
        ClearContainer(_dealSourceStack);
        _dealSourceDeckCards.Clear();

        for (var i = 0; i < DealSourceDeckLayers; i++)
        {
            var card = CreateCard(new CardModel($"deal-source-{i}", CardSuit.Spades, CardRank.Ace), false, false);
            card.MouseFilter = MouseFilterEnum.Ignore;
            card.Scale = new Vector2(1.14f, 1.14f);
            _dealSourceStack.AddChild(card);
            _dealSourceDeckCards.Add(card);
        }

        for (var i = 0; i < _dealSourceDeckCards.Count; i++)
        {
            var card = _dealSourceDeckCards[i];
            var depth = _dealSourceDeckCards.Count - 1 - i;
            card.Position = new Vector2(depth * 1.8f, depth * 1.25f);
            card.ZIndex = 20 + i;
        }

        SetDealSourceDeckRemaining(0);
    }

    private void SetDealSourceDeckRemaining(int remainingCards)
    {
        if (_dealSourceDeckCards.Count == 0)
        {
            return;
        }

        var visibleCount = (int)Math.Clamp(Math.Ceiling(remainingCards / 2.0), 0, _dealSourceDeckCards.Count);
        for (var i = 0; i < _dealSourceDeckCards.Count; i++)
        {
            var card = _dealSourceDeckCards[i];
            var visible = i < visibleCount;
            card.Visible = visible;
            if (!visible)
            {
                continue;
            }

            var alpha = 0.55f + 0.45f * (i / (float)Math.Max(1, visibleCount - 1));
            card.Modulate = new Color(1f, 1f, 1f, alpha);
        }
    }

    private void UpdateDealSourcePosition(SeatPosition shufflerSeat)
    {
        var sourceHand = GetHandContainer(shufflerSeat).GetGlobalRect();
        var anchorSize = _dealSourceAnchor.Size;
        if (anchorSize.X <= 2f || anchorSize.Y <= 2f)
        {
            anchorSize = new Vector2(100f, 130f);
        }

        var sourceCenter = sourceHand.Position + sourceHand.Size / 2f;
        const float gap = 18f;
        var targetPosition = sourceCenter - anchorSize / 2f;
        switch (ToVisualSlot(shufflerSeat))
        {
            case VisualSlot.Bottom:
                targetPosition = new Vector2(sourceCenter.X - anchorSize.X / 2f, sourceHand.Position.Y - anchorSize.Y - gap);
                break;
            case VisualSlot.Top:
                targetPosition = new Vector2(sourceCenter.X - anchorSize.X / 2f, sourceHand.End.Y + gap);
                break;
            case VisualSlot.Left:
                targetPosition = new Vector2(sourceHand.End.X + gap, sourceCenter.Y - anchorSize.Y / 2f);
                break;
            case VisualSlot.Right:
                targetPosition = new Vector2(sourceHand.Position.X - anchorSize.X - gap, sourceCenter.Y - anchorSize.Y / 2f);
                break;
        }

        _dealSourceAnchor.GlobalPosition = targetPosition;
    }

    private bool ShouldAbortDealAnimation(int runId, int roundNumber)
    {
        if (!_isDealAnimationRunning || runId != _dealAnimationRunId)
        {
            return true;
        }

        var latestSnapshot = LobbyManager.Instance.LocalMatchSnapshot;
        if (latestSnapshot.Count == 0)
        {
            return true;
        }

        var latestRoundNumber = latestSnapshot.TryGetValue("roundNumber", out var roundVariant)
            ? roundVariant.AsInt32()
            : roundNumber;
        return latestRoundNumber != roundNumber;
    }

    private void UpdateTurnSpotlight(SeatPosition currentTurnSeat, OmiPhase phase)
    {
        var baseIdleAlpha = UiMotionProfile.VignetteIdleAlpha;
        var activeAlpha = UiMotionProfile.VignetteActiveAlpha;
        var overlayAlpha = phase switch
        {
            OmiPhase.KapothiProposal or OmiPhase.KapothiResponse => Mathf.Clamp(activeAlpha + 0.02f, 0f, 0.28f),
            OmiPhase.TrickPlay => activeAlpha,
            _ => baseIdleAlpha
        };
        var duration = (float)Math.Max(0.06, UiMotionProfile.MicroDurationSeconds * 1.4);

        var tween = CreateTween();
        tween.TweenProperty(_focusVignette, "color:a", overlayAlpha, duration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            var label = GetLabelForSeat(seat);
            if (label == null)
            {
                continue;
            }

            var isActive = seat == currentTurnSeat;
            var targetColor = isActive ? new Color(1f, 0.96f, 0.74f, 1f) : new Color(0.88f, 0.87f, 0.83f, 1f);
            var targetScale = isActive ? new Vector2(1.05f, 1.05f) : Vector2.One;
            var labelTween = CreateTween();
            labelTween.SetParallel(true);
            labelTween.TweenProperty(label, "modulate", targetColor, duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            labelTween.TweenProperty(label, "scale", targetScale, duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
        }
    }

    private void PulseSeatLabel(SeatPosition seat)
    {
        var label = GetLabelForSeat(seat);
        if (label == null)
        {
            return;
        }

        var tween = CreateTween();
        tween.SetParallel(true);
        var pulseDuration = (float)Math.Max(0.05, UiMotionProfile.MicroDurationSeconds * 0.75);
        var settleDuration = (float)Math.Max(0.08, UiMotionProfile.MicroDurationSeconds * 1.1);
        tween.TweenProperty(label, "scale", new Vector2(1.09f, 1.09f), pulseDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "modulate", new Color(1f, 0.94f, 0.72f, 1f), pulseDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.Finished += () =>
        {
            if (!GodotObject.IsInstanceValid(label))
            {
                return;
            }

            var settle = CreateTween();
            settle.SetParallel(true);
            settle.TweenProperty(label, "scale", Vector2.One, settleDuration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.InOut);
            settle.TweenProperty(label, "modulate", Colors.White, settleDuration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.InOut);
        };
    }

    private Label? GetLabelForSeat(SeatPosition actualSeat)
    {
        return ToVisualSlot(actualSeat) switch
        {
            VisualSlot.Bottom => _bottomPlayerName,
            VisualSlot.Right => _rightPlayerName,
            VisualSlot.Top => _topPlayerName,
            VisualSlot.Left => _leftPlayerName,
            _ => null
        };
    }

    private void PlayPhaseCueIfNeeded(OmiPhase phase)
    {
        if (_lastPhaseCue == phase)
        {
            return;
        }

        _lastPhaseCue = phase;
        var cue = phase switch
        {
            OmiPhase.KapothiProposal or OmiPhase.KapothiResponse => UiSfxCue.KapothiDecision,
            OmiPhase.RoundScore => UiSfxCue.RoundResolved,
            OmiPhase.TrickResolveHold => UiSfxCue.TrickWin,
            _ => UiSfxCue.MatchPhase
        };
        AudioManager.Instance?.PlayUiCue(cue, 0.84f, 0.02f);
    }

    private void HandlePhaseVisualEvents(OmiPhase phase, int roundNumber, int[] teamCredits, int[] teamTricks)
    {
        if (_lastPhase != phase && (phase == OmiPhase.KapothiProposal || phase == OmiPhase.KapothiResponse))
        {
            var text = phase == OmiPhase.KapothiProposal
                ? "Kapothi opportunity opened."
                : "Kapothi response required.";
            _ = ShowKapothiBannerAsync(text);
        }

        if (_lastPhase != phase && phase == OmiPhase.RoundScore && _lastRoundBannerShown != roundNumber)
        {
            _lastRoundBannerShown = roundNumber;
            _ = ShowRoundResultAsync(roundNumber, teamCredits, teamTricks);
        }
    }

    private async Task ShowKapothiBannerAsync(string text)
    {
        var motionDuration = (float)Math.Max(0.1, UiMotionProfile.PanelEnterDurationSeconds * 0.75);
        var holdDuration = (float)Math.Max(0.8, UiMotionProfile.GameBannerHoldSeconds - 0.4);
        _kapothiBannerLabel.Text = text;
        _kapothiBannerPanel.Visible = true;
        _kapothiBannerPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _kapothiBannerPanel.Scale = new Vector2(0.96f, 0.96f);

        var tweenIn = CreateTween();
        tweenIn.SetParallel(true);
        tweenIn.TweenProperty(_kapothiBannerPanel, "modulate:a", 1f, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tweenIn.TweenProperty(_kapothiBannerPanel, "scale", Vector2.One, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(tweenIn, Tween.SignalName.Finished);

        await ToSignal(GetTree().CreateTimer(holdDuration), SceneTreeTimer.SignalName.Timeout);

        var tweenOut = CreateTween();
        tweenOut.SetParallel(true);
        tweenOut.TweenProperty(_kapothiBannerPanel, "modulate:a", 0f, motionDuration * 0.8f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        tweenOut.TweenProperty(_kapothiBannerPanel, "scale", new Vector2(1.03f, 1.03f), motionDuration * 0.8f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        await ToSignal(tweenOut, Tween.SignalName.Finished);

        _kapothiBannerPanel.Visible = false;
    }

    private async Task ShowRoundResultAsync(int roundNumber, int[] teamCredits, int[] teamTricks)
    {
        var motionDuration = (float)Math.Max(0.12, UiMotionProfile.PanelEnterDurationSeconds * 0.9);
        var holdDuration = (float)Math.Max(1.0, UiMotionProfile.GameBannerHoldSeconds);
        _roundResultLabel.Text = $"Round {roundNumber} resolved | Credits {teamCredits[0]} - {teamCredits[1]} | Tricks {teamTricks[0]} - {teamTricks[1]}";
        _roundResultPanel.Visible = true;
        _roundResultPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _roundResultPanel.Scale = new Vector2(0.95f, 0.95f);

        var tweenIn = CreateTween();
        tweenIn.SetParallel(true);
        tweenIn.TweenProperty(_roundResultPanel, "modulate:a", 1f, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tweenIn.TweenProperty(_roundResultPanel, "scale", Vector2.One, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(tweenIn, Tween.SignalName.Finished);

        await ToSignal(GetTree().CreateTimer(holdDuration), SceneTreeTimer.SignalName.Timeout);

        var tweenOut = CreateTween();
        tweenOut.SetParallel(true);
        tweenOut.TweenProperty(_roundResultPanel, "modulate:a", 0f, motionDuration * 0.84f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        tweenOut.TweenProperty(_roundResultPanel, "scale", new Vector2(1.04f, 1.04f), motionDuration * 0.84f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        await ToSignal(tweenOut, Tween.SignalName.Finished);
        _roundResultPanel.Visible = false;
    }

    private async Task RunDealAnimationSequence(SeatPosition shufflerSeat, SeatPosition dealStartSeat, bool includeTrumpAnnouncement, int trumpSuit)
    {
        if (_isDealAnimationRunning)
        {
            return;
        }

        var initialSnapshot = LobbyManager.Instance.LocalMatchSnapshot;
        var dealPhase = initialSnapshot.TryGetValue("phase", out var phaseVariant)
            ? (OmiPhase)phaseVariant.AsInt32()
            : OmiPhase.LobbySeating;
        var roundNumber = initialSnapshot.TryGetValue("roundNumber", out var roundVariant) ? roundVariant.AsInt32() : 0;
        var targetCounts = BuildTargetHandCounts(initialSnapshot);
        var currentCounts = BuildCurrentHandCounts();
        if (currentCounts.Any(entry => entry.Value > targetCounts[entry.Key]))
        {
            ClearHands();
            currentCounts = BuildCurrentHandCounts();
        }

        if (dealPhase == OmiPhase.SecondDeal)
        {
            EnsureDealBaselineCards(initialSnapshot, targetCounts, currentCounts, 4);
        }

        _isDealAnimationRunning = true;
        _dealAnimationRunId += 1;
        var runId = _dealAnimationRunId;
        _actionPanel.Visible = false;
        _dealSourceAnchor.Visible = false;

        if (includeTrumpAnnouncement && trumpSuit >= 0)
        {
            var trumpSelector = BuildSeatTitle(dealStartSeat);
            await ShowTrumpAnnouncementAsync(trumpSelector, (CardSuit)trumpSuit);
        }

        if (!ShouldAbortDealAnimation(runId, roundNumber))
        {
            await AnimateDealAsync(shufflerSeat, dealStartSeat, targetCounts, currentCounts, roundNumber, runId);
        }

        _isDealAnimationRunning = false;
        _dealSourceAnchor.Visible = false;
        _dealSourceStack.Scale = Vector2.One;
        SetDealSourceDeckRemaining(0);

        var snapshotToRender = _pendingSnapshot.Count > 0
            ? _pendingSnapshot
            : LobbyManager.Instance.LocalMatchSnapshot;
        _pendingSnapshot = new Godot.Collections.Dictionary();

        if (snapshotToRender.Count > 0)
        {
            RenderSnapshot(snapshotToRender);
        }
    }

    private async Task ShowTrumpAnnouncementAsync(string selectorName, CardSuit trumpSuit)
    {
        var motionDuration = (float)Math.Max(0.12, UiMotionProfile.PanelEnterDurationSeconds * 0.85);
        var holdDuration = Math.Max(0.2, UiMotionProfile.GameBannerHoldSeconds - 1.1);
        _trumpAnnouncementLabel.Text = $"{selectorName} selected {trumpSuit} as trump";
        _trumpAnnouncementPanel.Visible = true;
        _trumpAnnouncementPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _trumpAnnouncementPanel.Scale = new Vector2(0.92f, 0.92f);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.MatchPhase, 0.9f, 0.02f);

        var fadeIn = CreateTween();
        fadeIn.TweenProperty(_trumpAnnouncementPanel, "modulate:a", 1f, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        fadeIn.Parallel().TweenProperty(_trumpAnnouncementPanel, "scale", Vector2.One, motionDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(fadeIn, Tween.SignalName.Finished);

        await ToSignal(GetTree().CreateTimer(holdDuration), SceneTreeTimer.SignalName.Timeout);

        var fadeOut = CreateTween();
        fadeOut.TweenProperty(_trumpAnnouncementPanel, "modulate:a", 0f, motionDuration * 0.84f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        fadeOut.Parallel().TweenProperty(_trumpAnnouncementPanel, "scale", new Vector2(1.05f, 1.05f), motionDuration * 0.84f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        await ToSignal(fadeOut, Tween.SignalName.Finished);
        _trumpAnnouncementPanel.Visible = false;
    }

    private async Task AnimateDealAsync(
        SeatPosition shufflerSeat,
        SeatPosition dealStartSeat,
        IReadOnlyDictionary<SeatPosition, int> targetCounts,
        IDictionary<SeatPosition, int> currentCounts,
        int roundNumber,
        int runId)
    {
        var dealOrder = dealStartSeat.OrderedFrom();
        var maxCardsToAdd = targetCounts.Max(entry => Math.Max(0, entry.Value - currentCounts[entry.Key]));
        if (maxCardsToAdd <= 0)
        {
            return;
        }

        UpdateDealSourcePosition(shufflerSeat);
        _dealSourceAnchor.Visible = true;
        _dealSourceStack.Modulate = Colors.White;
        var remainingCards = targetCounts.Sum(entry => Math.Max(0, entry.Value - currentCounts[entry.Key]));
        SetDealSourceDeckRemaining(remainingCards);
        UpdateSideHandLayoutFromCounts(currentCounts);

        for (var pass = 0; pass < maxCardsToAdd; pass++)
        {
            foreach (var targetSeat in dealOrder)
            {
                if (ShouldAbortDealAnimation(runId, roundNumber))
                {
                    return;
                }

                if (currentCounts[targetSeat] >= targetCounts[targetSeat])
                {
                    continue;
                }

                var seatCardIndex = currentCounts[targetSeat];
                CardModel? exposedBottomCard = null;
                if (ShouldRevealDealtCardFace(targetSeat) &&
                    TryGetVisibleCardModel(LobbyManager.Instance.LocalMatchSnapshot, targetSeat, seatCardIndex, out var visibleModel))
                {
                    exposedBottomCard = visibleModel;
                }

                await SpawnDealCardAsync(shufflerSeat, targetSeat, seatCardIndex, exposedBottomCard, roundNumber, runId);
                currentCounts[targetSeat] += 1;
                remainingCards = Math.Max(0, remainingCards - 1);
                SetDealSourceDeckRemaining(remainingCards);
                UpdateSideHandLayoutFromCounts(currentCounts);

                if (ShouldAbortDealAnimation(runId, roundNumber))
                {
                    return;
                }

                await ToSignal(GetTree().CreateTimer(UiMotionProfile.DealIntervalSeconds), SceneTreeTimer.SignalName.Timeout);
            }
        }

        await ToSignal(GetTree().CreateTimer(Math.Max(0.03, UiMotionProfile.DealTravelSeconds * 0.2)), SceneTreeTimer.SignalName.Timeout);
    }

    private async Task SpawnDealCardAsync(
        SeatPosition shufflerSeat,
        SeatPosition targetSeat,
        int seatCardIndex,
        CardModel? exposedBottomCard,
        int roundNumber,
        int runId)
    {
        if (ShouldAbortDealAnimation(runId, roundNumber))
        {
            return;
        }

        var flyModel = exposedBottomCard ?? new CardModel($"deal-fly-{targetSeat}-{seatCardIndex}", CardSuit.Spades, CardRank.Ace);
        var flyCard = CreateCard(flyModel, false, false);
        _animationLayer.AddChild(flyCard);
        flyCard.ZIndex = 900 + seatCardIndex;
        var cardSize = EnsureCardSize(flyCard);

        var source = _dealSourceAnchor.Visible
            ? _dealSourceAnchor.GlobalPosition + (_dealSourceAnchor.Size - cardSize) / 2f
            : GetHandCenterGlobal(shufflerSeat, cardSize);
        flyCard.GlobalPosition = source;

        var target = GetHandCenterGlobal(targetSeat, cardSize) + GetDealSeatOffset(targetSeat, seatCardIndex);
        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(flyCard, "global_position", target, (float)UiMotionProfile.DealTravelSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_dealSourceStack, "scale", new Vector2(0.96f, 0.96f), (float)Math.Max(0.04, UiMotionProfile.MicroDurationSeconds * 0.55))
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_dealSourceStack, "scale", Vector2.One, (float)Math.Max(0.05, UiMotionProfile.MicroDurationSeconds * 0.7))
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        PlaySound(_dealSlideSound);
        await ToSignal(tween, Tween.SignalName.Finished);

        if (GodotObject.IsInstanceValid(flyCard))
        {
            flyCard.QueueFree();
        }

        if (ShouldAbortDealAnimation(runId, roundNumber))
        {
            return;
        }

        var insertedModel = exposedBottomCard ?? new CardModel($"hidden-{targetSeat}-{seatCardIndex}", CardSuit.Spades, CardRank.Ace);
        var revealFace = exposedBottomCard.HasValue;
        var insertedCard = CreateCard(insertedModel, false, false);
        GetHandContainer(targetSeat).AddChild(insertedCard);
        insertedCard.Modulate = new Color(1f, 1f, 1f, UiSettings.ReduceMotion ? 1f : 0f);
        insertedCard.Scale = UiSettings.ReduceMotion ? Vector2.One : new Vector2(0.94f, 0.94f);
        await PlayDealArrivalVisualAsync(insertedCard, revealFace);
    }

    private async Task PlayDealArrivalVisualAsync(Card insertedCard, bool shouldRevealFace)
    {
        var revealDuration = (float)Math.Max(0.06, UiMotionProfile.DealFlipSeconds);
        var useCinematicFlip = shouldRevealFace && !UiSettings.ReduceMotion;

        if (useCinematicFlip)
        {
            insertedCard.SetFaceUp(false);
            insertedCard.Modulate = new Color(1f, 1f, 1f, 1f);
            insertedCard.Scale = Vector2.One;

            var firstHalf = CreateTween();
            firstHalf.TweenProperty(insertedCard, "scale:x", 0.06f, revealDuration * 0.5f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.In);
            await ToSignal(firstHalf, Tween.SignalName.Finished);

            insertedCard.SetFaceUp(true);
            var secondHalf = CreateTween();
            secondHalf.SetParallel(true);
            secondHalf.TweenProperty(insertedCard, "scale:x", 1f, revealDuration * 0.5f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            secondHalf.TweenProperty(insertedCard, "scale:y", 1.03f, revealDuration * 0.25f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            secondHalf.TweenProperty(insertedCard, "scale:y", 1f, revealDuration * 0.25f)
                .SetDelay(revealDuration * 0.25f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.InOut);
            await ToSignal(secondHalf, Tween.SignalName.Finished);
            return;
        }

        if (shouldRevealFace)
        {
            insertedCard.SetFaceUp(true);
        }

        var settle = CreateTween();
        settle.SetParallel(true);
        settle.TweenProperty(insertedCard, "modulate:a", 1f, revealDuration * 0.65f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        settle.TweenProperty(insertedCard, "scale", Vector2.One, revealDuration * 0.65f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(settle, Tween.SignalName.Finished);
    }

    private Vector2 GetHandCenterGlobal(SeatPosition seat, Vector2 cardSize)
    {
        var handRect = GetHandContainer(seat).GetGlobalRect();
        return handRect.Position + handRect.Size / 2f - cardSize / 2f;
    }

    private Vector2 GetDealSeatOffset(SeatPosition seat, int seatCardIndex)
    {
        var indexOffset = seatCardIndex - 1.5f;
        return ToVisualSlot(seat) switch
        {
            VisualSlot.Bottom => new Vector2(indexOffset * 18f, 0),
            VisualSlot.Top => new Vector2(indexOffset * 18f, 0),
            VisualSlot.Left => new Vector2(0, indexOffset * 14f),
            VisualSlot.Right => new Vector2(0, indexOffset * 14f),
            _ => Vector2.Zero
        };
    }

    private void RenderHands(Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat)
    {
        var handCounts = ReadHandCounts(snapshot);
        var visibleHands = ReadVisibleHands(snapshot);
        var renderedCounts = new Dictionary<SeatPosition, int>();

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            var container = GetHandContainer(seat);
            var visibleCards = ReadVisibleCardsForSeat(visibleHands, seat);
            var totalCount = ResolveSeatHandCount(handCounts, seat, visibleCards.Count);
            renderedCounts[seat] = totalCount;

            var isLocalPlayableSeat = _localSeat.HasValue && _localSeat.Value == seat && _localRole == ParticipantRole.Player;
            var canInteract = isLocalPlayableSeat && phase == OmiPhase.TrickPlay && currentTurnSeat == seat;

            if (visibleCards.Count > 0)
            {
                foreach (var item in visibleCards)
                {
                    if (item.VariantType != Variant.Type.Dictionary)
                    {
                        continue;
                    }

                    var cardModel = CardModelConversions.FromDictionary(item.AsGodotDictionary());
                    container.AddChild(CreateCard(cardModel, true, canInteract));
                }
            }
            else
            {
                for (var i = 0; i < totalCount; i++)
                {
                    container.AddChild(CreateCard(new CardModel($"hidden-{seat}-{i}", CardSuit.Spades, CardRank.Ace), false, false));
                }
            }
        }

        UpdateSideHandLayoutFromCounts(renderedCounts);
    }

    private void RenderTrickState(Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks, bool suppressEntryAudio)
    {
        if (_lastRoundNumber != -1 && roundNumber != _lastRoundNumber)
        {
            ClearTrickAndPileVisuals();
        }

        if (!snapshot.TryGetValue("currentTrick", out var currentTrickVariant) || currentTrickVariant.VariantType != Variant.Type.Array)
        {
            UpdatePileCounts(teamTricks);
            _lastRoundNumber = roundNumber;
            _lastTeamTricks[0] = teamTricks[0];
            _lastTeamTricks[1] = teamTricks[1];
            return;
        }

        var completedTricksCount = snapshot.TryGetValue("completedTricksCount", out var completedVariant)
            ? completedVariant.AsInt32()
            : 0;

        if (_lastCompletedTricksCount >= 0 && completedTricksCount > _lastCompletedTricksCount && _deskCards.Count > 0)
        {
            var winnerTeam = ResolveWinnerTeam(teamTricks, currentTurnSeat);
            AnimateTrickCollection(winnerTeam);
        }

        var currentTrick = currentTrickVariant.AsGodotArray();
        var snapshotSeats = new HashSet<SeatPosition>();
        foreach (var item in currentTrick)
        {
            if (item.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            var dict = item.AsGodotDictionary();
            var seat = dict.TryGetValue("seat", out var seatVariant)
                ? NetDex.Core.Enums.SeatPositionExtensions.Parse(seatVariant.AsString()) ?? SeatPosition.Bottom
                : SeatPosition.Bottom;
            snapshotSeats.Add(seat);

            if (!dict.TryGetValue("card", out var cardVariant) || cardVariant.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            if (_deskCards.ContainsKey(seat))
            {
                continue;
            }

            var cardModel = CardModelConversions.FromDictionary(cardVariant.AsGodotDictionary());
            var card = CreateCard(cardModel, true, false);
            _desk.AddChild(card);
            var cardSize = EnsureCardSize(card);
            card.Position = GetDeskSpawnPosition(seat, cardSize);

            var tween = CreateTween();
            tween.TweenProperty(card, "position", GetDeskCardPosition(seat, cardSize), (float)UiMotionProfile.DeskPlayTravelSeconds)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            _deskCards[seat] = card;
            if (!suppressEntryAudio)
            {
                PlaySound(_placeSounds);
            }
        }

        foreach (var seat in _deskCards.Keys.ToList())
        {
            if (snapshotSeats.Contains(seat))
            {
                continue;
            }

            if (_isCollectingTrick)
            {
                continue;
            }

            _deskCards[seat].QueueFree();
            _deskCards.Remove(seat);
        }

        UpdatePileCounts(teamTricks);
        _lastRoundNumber = roundNumber;
        _lastCompletedTricksCount = completedTricksCount;
        _lastTeamTricks[0] = teamTricks[0];
        _lastTeamTricks[1] = teamTricks[1];
    }

    private void RenderActionPanel(Godot.Collections.Dictionary snapshot, OmiPhase phase)
    {
        _secondaryActionButton.Visible = false;
        _secondaryActionButton.Disabled = true;
        _trumpOption.Visible = false;
        _actionButton.Disabled = false;

        if (_isDealAnimationRunning || phase == OmiPhase.TrickResolveHold)
        {
            _actionPanel.Visible = false;
            return;
        }

        if (_localRole != ParticipantRole.Player || !_localSeat.HasValue)
        {
            _actionPanel.Visible = false;
            return;
        }

        var shufflerSeat = snapshot.TryGetValue("shufflerSeat", out var shufflerVariant)
            ? NetDex.Core.Enums.SeatPositionExtensions.Parse(shufflerVariant.AsString())
            : null;

        var cutterSeat = snapshot.TryGetValue("cutterSeat", out var cutterVariant)
            ? NetDex.Core.Enums.SeatPositionExtensions.Parse(cutterVariant.AsString())
            : null;

        var trumpSelectorSeat = snapshot.TryGetValue("trumpSelectorSeat", out var selectorVariant)
            ? NetDex.Core.Enums.SeatPositionExtensions.Parse(selectorVariant.AsString())
            : null;

        if (phase == OmiPhase.Shuffle && shufflerSeat == _localSeat)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Your action: shuffle as many times as you want";
            _actionButton.Text = "Shuffle Again";
            _secondaryActionButton.Visible = true;
            _secondaryActionButton.Disabled = false;
            _secondaryActionButton.Text = "Done Shuffling";
            return;
        }

        if (phase == OmiPhase.Cut && cutterSeat == _localSeat)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Your action: cut the deck";
            _actionButton.Text = "Cut Deck";
            return;
        }

        if (phase == OmiPhase.TrumpSelect && trumpSelectorSeat == _localSeat)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Your action: select trump suit";
            _trumpOption.Visible = true;
            _actionButton.Text = "Select Trump";
            return;
        }

        var localTeam = _localSeat.Value.TeamIndex();
        var kapothiEligibleTeam = snapshot.TryGetValue("kapothiEligibleTeam", out var eligibleVariant)
            ? eligibleVariant.AsInt32()
            : -1;
        var kapothiTargetTeam = snapshot.TryGetValue("kapothiTargetTeam", out var targetVariant)
            ? targetVariant.AsInt32()
            : -1;
        var kapothiOffered = snapshot.TryGetValue("kapothiOfferedThisRound", out var offeredVariant) && offeredVariant.AsBool();

        if (phase == OmiPhase.KapothiProposal && kapothiEligibleTeam == localTeam)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Your team can propose Kapothi.";
            _actionButton.Text = "Propose Kapothi";
            _secondaryActionButton.Visible = true;
            _secondaryActionButton.Disabled = false;
            _secondaryActionButton.Text = "Skip Kapothi";
            return;
        }

        if (phase == OmiPhase.KapothiResponse && kapothiOffered && kapothiTargetTeam == localTeam)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Kapothi proposed. Choose your response.";
            _actionButton.Text = "Accept Kapothi";
            _secondaryActionButton.Visible = true;
            _secondaryActionButton.Disabled = false;
            _secondaryActionButton.Text = "Reject Kapothi";
            return;
        }

        _actionPanel.Visible = false;
    }

    private void OnActionButtonPressed()
    {
        var snapshot = LobbyManager.Instance.LocalMatchSnapshot;
        if (snapshot.Count == 0)
        {
            return;
        }

        var phase = snapshot.TryGetValue("phase", out var phaseVariant)
            ? (OmiPhase)phaseVariant.AsInt32()
            : OmiPhase.LobbySeating;

        if (phase == OmiPhase.Shuffle)
        {
            NetworkRpc.Instance.SendShuffleAgainRequest((int)GD.Randi());
            return;
        }

        if (phase == OmiPhase.Cut)
        {
            NetworkRpc.Instance.SendCutDeckRequest((int)GD.RandRange(1, 31));
            return;
        }

        if (phase == OmiPhase.TrumpSelect)
        {
            NetworkRpc.Instance.SendSelectTrumpRequest((CardSuit)_trumpOption.GetSelectedId());
            return;
        }

        if (phase == OmiPhase.KapothiProposal)
        {
            NetworkRpc.Instance.SendKapothiProposeRequest();
            return;
        }

        if (phase == OmiPhase.KapothiResponse)
        {
            NetworkRpc.Instance.SendKapothiAcceptRequest();
        }
    }

    private void OnSecondaryActionButtonPressed()
    {
        var snapshot = LobbyManager.Instance.LocalMatchSnapshot;
        if (snapshot.Count == 0)
        {
            return;
        }

        var phase = snapshot.TryGetValue("phase", out var phaseVariant)
            ? (OmiPhase)phaseVariant.AsInt32()
            : OmiPhase.LobbySeating;

        if (phase == OmiPhase.Shuffle)
        {
            NetworkRpc.Instance.SendFinishShuffleRequest();
            return;
        }

        if (phase == OmiPhase.KapothiProposal)
        {
            NetworkRpc.Instance.SendKapothiSkipRequest();
            return;
        }

        if (phase == OmiPhase.KapothiResponse)
        {
            NetworkRpc.Instance.SendKapothiRejectRequest();
        }
    }

    private void OnCardClicked(Card card)
    {
        if (_localRole != ParticipantRole.Player)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(card.CardId) || card.CardId.StartsWith("hidden-"))
        {
            return;
        }

        if (!CanPlayCardFromSnapshot(card.CardId))
        {
            return;
        }

        AudioManager.Instance?.PlayUiCue(UiSfxCue.Confirm, 0.72f, 0.02f);
        NetworkRpc.Instance.SendPlayCardRequest(card.CardId);
    }

    private bool CanPlayCardFromSnapshot(string cardId)
    {
        var snapshot = LobbyManager.Instance.LocalMatchSnapshot;
        if (snapshot.Count == 0 || !_localSeat.HasValue)
        {
            return true;
        }

        var phase = snapshot.TryGetValue("phase", out var phaseVariant)
            ? (OmiPhase)phaseVariant.AsInt32()
            : OmiPhase.LobbySeating;
        if (phase != OmiPhase.TrickPlay)
        {
            return false;
        }

        var currentTurnSeat = snapshot.TryGetValue("currentTurnSeat", out var turnVariant)
            ? NetDex.Core.Enums.SeatPositionExtensions.Parse(turnVariant.AsString())
            : null;
        if (!currentTurnSeat.HasValue || currentTurnSeat.Value != _localSeat.Value)
        {
            return false;
        }

        var visibleHands = ReadVisibleHands(snapshot);
        if (!visibleHands.TryGetValue(_localSeat.Value.ToString(), out var localHandVariant) || localHandVariant.VariantType != Variant.Type.Array)
        {
            return true;
        }

        var localCards = localHandVariant.AsGodotArray();
        var localHandModels = new List<CardModel>(localCards.Count);
        CardModel selectedCard = default;
        var foundSelectedCard = false;

        foreach (var item in localCards)
        {
            if (item.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            var model = CardModelConversions.FromDictionary(item.AsGodotDictionary());
            localHandModels.Add(model);
            if (model.Id != cardId)
            {
                continue;
            }

            selectedCard = model;
            foundSelectedCard = true;
        }

        if (!foundSelectedCard)
        {
            return true;
        }

        if (!snapshot.TryGetValue("currentTrick", out var trickVariant) || trickVariant.VariantType != Variant.Type.Array)
        {
            return true;
        }

        var currentTrick = trickVariant.AsGodotArray();
        if (currentTrick.Count == 0)
        {
            return true;
        }

        var firstPlay = currentTrick[0];
        if (firstPlay.VariantType != Variant.Type.Dictionary)
        {
            return true;
        }

        var firstPlayDict = firstPlay.AsGodotDictionary();
        if (!firstPlayDict.TryGetValue("card", out var leadCardVariant) || leadCardVariant.VariantType != Variant.Type.Dictionary)
        {
            return true;
        }

        var leadCard = CardModelConversions.FromDictionary(leadCardVariant.AsGodotDictionary());
        if (selectedCard.Suit == leadCard.Suit)
        {
            return true;
        }

        var hasLeadSuit = localHandModels.Any(localCard => localCard.Suit == leadCard.Suit);
        return !hasLeadSuit;
    }

    private static void OnBackLobbyPressed()
    {
        GameManager.Instance?.LoadLobby();
    }

    private void OnMobilePausePressed()
    {
        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        var pauseMenu = GetNodeOrNull<PauseMenu>("PauseMenu");
        if (pauseMenu == null)
        {
            pauseMenu = EnsurePauseMenu();
            pauseMenu.ShowMenu();
            return;
        }

        if (pauseMenu.Visible)
        {
            pauseMenu.HideMenu();
        }
        else
        {
            pauseMenu.ShowMenu();
        }
    }

    private PauseMenu EnsurePauseMenu()
    {
        var pauseMenuScene = GD.Load<PackedScene>("res://scenes/ui/PauseMenu.tscn");
        var pauseMenu = pauseMenuScene.Instantiate<PauseMenu>();
        pauseMenu.Name = "PauseMenu";
        AddChild(pauseMenu);
        return pauseMenu;
    }

    private Card CreateCard(CardModel model, bool faceUp, bool interactable, bool playRevealSound = false)
    {
        var card = _cardScene.Instantiate<Card>();
        card.Setup(ToViewSuit(model.Suit), ToViewRank(model.Rank), faceUp, model.Id);
        card.SetInteractable(interactable);

        if (interactable)
        {
            card.CardClicked += OnCardClicked;
            card.CardSelected += OnCardSelected;
        }

        if (faceUp && playRevealSound)
        {
            PlaySound(_slideSounds);
        }

        return card;
    }

    private void OnCardSelected(Card selectedCard)
    {
        foreach (Node child in _bottomHand.GetChildren())
        {
            if (child is not Card card || card == selectedCard)
            {
                continue;
            }

            card.SetSelected(false);
        }
    }

    private void ClearHands()
    {
        ClearContainer(_bottomHand);
        ClearContainer(_topHand);
        ClearContainer(_leftHand);
        ClearContainer(_rightHand);
    }

    private void ResetBoardVisualState()
    {
        ClearHands();
        ClearTrickAndPileVisuals();
        _lastRoundNumber = -1;
        _lastCompletedTricksCount = -1;
        _lastTeamTricks[0] = 0;
        _lastTeamTricks[1] = 0;
        _pendingFriendlyTricks = 0;
        _pendingEnemyTricks = 0;
        _leftPileLabel.Text = "Your Tricks";
        _rightPileLabel.Text = "Enemy Tricks";
        _isDealAnimationRunning = false;
        _pendingSnapshot = new Godot.Collections.Dictionary();
        _trumpAnnouncementPanel.Visible = false;
        _roundResultPanel.Visible = false;
        _kapothiBannerPanel.Visible = false;
        _lastPhaseCue = null;
        _lastRoundBannerShown = -1;
        _lastTurnSeat = null;
        _focusVignette.Color = new Color(0f, 0f, 0f, UiMotionProfile.VignetteIdleAlpha);
        _dealSourceAnchor.Visible = false;
        _dealSourceStack.Scale = Vector2.One;
        SetDealSourceDeckRemaining(0);
        _suppressInitialDeskEntryAudio = true;
        ClearAnimationLayerCards();
    }

    private void ClearTrickAndPileVisuals()
    {
        foreach (var card in _deskCards.Values)
        {
            card.QueueFree();
        }

        foreach (var card in _friendlyPileCards)
        {
            card.QueueFree();
        }

        foreach (var card in _enemyPileCards)
        {
            card.QueueFree();
        }

        _deskCards.Clear();
        _friendlyPileCards.Clear();
        _enemyPileCards.Clear();
        _isCollectingTrick = false;

        ClearContainer(_desk);
        ClearContainer(_leftTrickPile);
        ClearContainer(_rightTrickPile);
    }

    private static void ClearContainer(Node container)
    {
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void ClearAnimationLayerCards()
    {
        foreach (Node child in _animationLayer.GetChildren())
        {
            if (child == _trumpAnnouncementPanel || child == _roundResultPanel || child == _kapothiBannerPanel || child == _dealSourceAnchor)
            {
                continue;
            }

            child.QueueFree();
        }
    }

    private Control GetHandContainer(SeatPosition seat)
    {
        return ToVisualSlot(seat) switch
        {
            VisualSlot.Bottom => _bottomHand,
            VisualSlot.Right => _rightHand,
            VisualSlot.Top => _topHand,
            VisualSlot.Left => _leftHand,
            _ => _bottomHand
        };
    }

    private Vector2 GetDeskCardPosition(SeatPosition seat, Vector2 cardSize)
    {
        var center = _desk.Size / 2f - cardSize / 2f;
        return ToVisualSlot(seat) switch
        {
            VisualSlot.Bottom => center + new Vector2(0, 110),
            VisualSlot.Right => center + new Vector2(110, 0),
            VisualSlot.Top => center + new Vector2(0, -110),
            VisualSlot.Left => center + new Vector2(-110, 0),
            _ => center
        };
    }

    private Vector2 GetDeskSpawnPosition(SeatPosition seat, Vector2 cardSize)
    {
        var container = GetHandContainer(seat);
        var globalRect = container.GetGlobalRect();
        var sourceCenterGlobal = globalRect.Position + globalRect.Size / 2f - cardSize / 2f;
        return _desk.GetGlobalTransformWithCanvas().AffineInverse() * sourceCenterGlobal;
    }

    private VisualSlot ToVisualSlot(SeatPosition actualSeat)
    {
        var perspectiveSeat = _localRole == ParticipantRole.Player && _localSeat.HasValue
            ? _localSeat.Value
            : SeatPosition.Bottom;

        var offset = ((int)actualSeat - (int)perspectiveSeat + 4) % 4;
        return (VisualSlot)offset;
    }

    private void RenderSeatNames()
    {
        var seatNames = new Dictionary<SeatPosition, string>();
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            seatNames[seat] = seat.ToString();
        }

        var room = LobbyManager.Instance.CurrentRoom;
        if (room != null)
        {
            foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
            {
                if (!room.SeatAssignments.TryGetValue(seat, out var peerId) || !peerId.HasValue)
                {
                    seatNames[seat] = $"{seat} (empty)";
                    continue;
                }

                if (room.Participants.TryGetValue(peerId.Value, out var participant))
                {
                    var suffix = _localSeat.HasValue && _localSeat.Value == seat ? " (You)" : string.Empty;
                    seatNames[seat] = $"{participant.Name}{suffix}";
                    continue;
                }

                seatNames[seat] = seat.ToString();
            }
        }

        var bottomName = seatNames[ToSeatForVisualSlot(VisualSlot.Bottom)];
        var rightName = seatNames[ToSeatForVisualSlot(VisualSlot.Right)];
        var topName = seatNames[ToSeatForVisualSlot(VisualSlot.Top)];
        var leftName = seatNames[ToSeatForVisualSlot(VisualSlot.Left)];

        _bottomPlayerName.Text = bottomName;
        _topPlayerName.Text = topName;
        _leftPlayerName.Text = leftName;
        _rightPlayerName.Text = rightName;

        _bottomPlayerName.TooltipText = bottomName;
        _topPlayerName.TooltipText = topName;
        _leftPlayerName.TooltipText = leftName;
        _rightPlayerName.TooltipText = rightName;
    }

    private SeatPosition ToSeatForVisualSlot(VisualSlot visualSlot)
    {
        var perspectiveSeat = _localRole == ParticipantRole.Player && _localSeat.HasValue
            ? _localSeat.Value
            : SeatPosition.Bottom;

        return (SeatPosition)(((int)perspectiveSeat + (int)visualSlot) % 4);
    }

    private string BuildSeatTitle(SeatPosition seat)
    {
        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null || !room.SeatAssignments.TryGetValue(seat, out var peerId) || !peerId.HasValue)
        {
            return seat.ToString();
        }

        if (!room.Participants.TryGetValue(peerId.Value, out var participant))
        {
            return seat.ToString();
        }

        var isYou = _localSeat.HasValue && _localSeat.Value == seat;
        return isYou ? $"{participant.Name} (You)" : participant.Name;
    }

    private int ResolveWinnerTeam(int[] teamTricks, SeatPosition currentTurnSeat)
    {
        if (teamTricks[0] > _lastTeamTricks[0])
        {
            return 0;
        }

        if (teamTricks[1] > _lastTeamTricks[1])
        {
            return 1;
        }

        return currentTurnSeat.TeamIndex();
    }

    private void AnimateTrickCollection(int winnerTeam)
    {
        if (_isCollectingTrick || _deskCards.Count == 0)
        {
            return;
        }

        _isCollectingTrick = true;
        var cards = _deskCards.Values.ToList();
        _deskCards.Clear();

        var localTeam = _localSeat?.TeamIndex() ?? 0;
        var isFriendlyWinner = winnerTeam == localTeam;
        var pile = isFriendlyWinner ? _leftTrickPile : _rightTrickPile;
        var pileCount = isFriendlyWinner ? _friendlyPileCards.Count : _enemyPileCards.Count;
        var baseTarget = new Vector2(6 + pileCount * 5, 10 + pileCount * 3);

        var tween = CreateTween();
        tween.SetParallel(true);
        var collectDuration = (float)Math.Max(0.16, UiMotionProfile.DeskPlayTravelSeconds + 0.08);

        for (var i = 0; i < cards.Count; i++)
        {
            var target = baseTarget + new Vector2((i % 2) * 12, (i / 2) * 8);
            tween.TweenProperty(cards[i], "global_position", pile.GlobalPosition + target, collectDuration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.InOut);
        }

        PlaySound(_slideSounds);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.TrickWin, 0.78f, 0.02f);
        FlashWinningTeamLabels(winnerTeam);

        tween.Finished += () =>
        {
            foreach (var card in cards)
            {
                card.QueueFree();
            }

            _isCollectingTrick = false;
            SyncPileMarkers(_pendingFriendlyTricks, _pendingEnemyTricks);
        };
    }

    private void FlashWinningTeamLabels(int winningTeam)
    {
        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            if (seat.TeamIndex() != winningTeam)
            {
                continue;
            }

            var label = GetLabelForSeat(seat);
            if (label == null)
            {
                continue;
            }

            var tween = CreateTween();
            tween.SetParallel(true);
            var flashDuration = (float)Math.Max(0.08, UiMotionProfile.MicroDurationSeconds * 1.1);
            var settleDuration = (float)Math.Max(0.1, UiMotionProfile.MicroDurationSeconds * 1.25);
            tween.TweenProperty(label, "modulate", new Color(1f, 0.98f, 0.68f, 1f), flashDuration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            tween.TweenProperty(label, "scale", new Vector2(1.08f, 1.08f), flashDuration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            tween.Finished += () =>
            {
                if (!GodotObject.IsInstanceValid(label))
                {
                    return;
                }

                var settle = CreateTween();
                settle.SetParallel(true);
                settle.TweenProperty(label, "modulate", Colors.White, settleDuration)
                    .SetTrans(Tween.TransitionType.Cubic)
                    .SetEase(Tween.EaseType.InOut);
                settle.TweenProperty(label, "scale", Vector2.One, settleDuration)
                    .SetTrans(Tween.TransitionType.Cubic)
                    .SetEase(Tween.EaseType.InOut);
            };
        }
    }

    private void UpdatePileCounts(int[] teamTricks)
    {
        var localTeam = _localSeat?.TeamIndex() ?? 0;
        _pendingFriendlyTricks = teamTricks[localTeam];
        _pendingEnemyTricks = teamTricks[1 - localTeam];

        _leftPileLabel.Text = $"Your Tricks: {_pendingFriendlyTricks}";
        _rightPileLabel.Text = $"Enemy Tricks: {_pendingEnemyTricks}";

        if (_isCollectingTrick)
        {
            return;
        }

        SyncPileMarkers(_pendingFriendlyTricks, _pendingEnemyTricks);
    }

    private void SyncPileMarkers(int friendlyCount, int enemyCount)
    {
        SyncSinglePile(_friendlyPileCards, _leftTrickPile, friendlyCount);
        SyncSinglePile(_enemyPileCards, _rightTrickPile, enemyCount);
    }

    private void SyncSinglePile(List<Card> pileCards, Control pileContainer, int targetCount)
    {
        const float stackXStep = 5f;
        const float stackYStep = 3f;
        const float baseX = 6f;
        const float baseY = 10f;

        while (pileCards.Count > targetCount)
        {
            var last = pileCards[^1];
            pileCards.RemoveAt(pileCards.Count - 1);
            last.QueueFree();
        }

        while (pileCards.Count < targetCount)
        {
            var marker = CreateCard(new CardModel($"pile-{pileCards.Count}", CardSuit.Spades, CardRank.Ace), false, false);
            pileContainer.AddChild(marker);
            marker.Position = new Vector2(baseX + pileCards.Count * stackXStep, baseY + pileCards.Count * stackYStep);
            marker.ZIndex = 100 + pileCards.Count;
            pileCards.Add(marker);
        }
    }

    private static Vector2 EnsureCardSize(Card card)
    {
        if (card.Size.X > 0 && card.Size.Y > 0)
        {
            return card.Size;
        }

        card.Size = card.CustomMinimumSize;
        return card.Size;
    }

    private void PlaySound(AudioStreamOggVorbis[] sounds)
    {
        if (sounds.Length == 0)
        {
            return;
        }

        _sfxPlayer.Stream = sounds[GD.RandRange(0, sounds.Length - 1)];
        _sfxPlayer.Play();
    }

    private void PlaySound(AudioStream sound)
    {
        if (sound == null)
        {
            return;
        }

        var player = new AudioStreamPlayer
        {
            Stream = sound,
            Bus = AudioManager.GetSfxBusName()
        };
        AddChild(player);
        player.Finished += () => player.QueueFree();
        player.Play();
    }

    private static Card.SuitType ToViewSuit(CardSuit suit)
    {
        return suit switch
        {
            CardSuit.Hearts => Card.SuitType.Hearts,
            CardSuit.Diamonds => Card.SuitType.Diamonds,
            CardSuit.Clubs => Card.SuitType.Clubs,
            CardSuit.Spades => Card.SuitType.Spades,
            _ => Card.SuitType.Spades
        };
    }

    private static Card.RankType ToViewRank(CardRank rank)
    {
        return rank switch
        {
            CardRank.Seven => Card.RankType.Seven,
            CardRank.Eight => Card.RankType.Eight,
            CardRank.Nine => Card.RankType.Nine,
            CardRank.Ten => Card.RankType.Ten,
            CardRank.Jack => Card.RankType.Jack,
            CardRank.Queen => Card.RankType.Queen,
            CardRank.King => Card.RankType.King,
            CardRank.Ace => Card.RankType.Ace,
            _ => Card.RankType.Seven
        };
    }
}
