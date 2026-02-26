using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NetDex.Core.Enums;
using NetDex.Core.Models;
using NetDex.Core.Serialization;
using NetDex.Lobby;
using NetDex.Managers;
using NetDex.Networking;

namespace NetDex.UI.Game;

public partial class GameScreen : Control
{
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
    private PanelContainer _actionPanel = null!;
    private Label _actionLabel = null!;
    private OptionButton _trumpOption = null!;
    private Button _actionButton = null!;
    private Button _secondaryActionButton = null!;

    private PackedScene _cardScene = null!;

    private AudioStreamOggVorbis[] _placeSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamOggVorbis[] _slideSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamPlayer _sfxPlayer = null!;

    private SeatPosition? _localSeat;
    private ParticipantRole _localRole = ParticipantRole.Spectator;

    private readonly Dictionary<SeatPosition, Card> _deskCards = new();
    private readonly List<Card> _friendlyPileCards = new();
    private readonly List<Card> _enemyPileCards = new();

    private int _lastRoundNumber = -1;
    private int _lastCompletedTricksCount = -1;
    private readonly int[] _lastTeamTricks = new int[2];

    private int _pendingFriendlyTricks;
    private int _pendingEnemyTricks;
    private bool _isCollectingTrick;

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
        _actionPanel = GetNode<PanelContainer>("ActionPanel");
        _actionLabel = GetNode<Label>("ActionPanel/VBoxContainer/ActionLabel");
        _trumpOption = GetNode<OptionButton>("ActionPanel/VBoxContainer/TrumpOption");
        _actionButton = GetNode<Button>("ActionPanel/VBoxContainer/ActionButton");
        _secondaryActionButton = GetNode<Button>("ActionPanel/VBoxContainer/SecondaryActionButton");

        GetNode<Button>("BackLobbyButton").Pressed += OnBackLobbyPressed;
        _actionButton.Pressed += OnActionButtonPressed;
        _secondaryActionButton.Pressed += OnSecondaryActionButtonPressed;

        _trumpOption.Clear();
        _trumpOption.AddItem("Hearts", (int)CardSuit.Hearts);
        _trumpOption.AddItem("Diamonds", (int)CardSuit.Diamonds);
        _trumpOption.AddItem("Clubs", (int)CardSuit.Clubs);
        _trumpOption.AddItem("Spades", (int)CardSuit.Spades);

        _cardScene = GD.Load<PackedScene>("res://scenes/game/Card.tscn");

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

        _sfxPlayer = new AudioStreamPlayer();
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
        if (@event.IsActionPressed("ui_cancel") && !HasNode("PauseMenu"))
        {
            var pauseMenuScene = GD.Load<PackedScene>("res://scenes/ui/PauseMenu.tscn");
            var pauseMenu = pauseMenuScene.Instantiate<Control>();
            pauseMenu.Name = "PauseMenu";
            AddChild(pauseMenu);
            pauseMenu.Show();
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
        ClearHands();
        RenderSeatNames();

        if (snapshot.Count == 0)
        {
            _statusLabel.Text = "Waiting for match state...";
            _actionPanel.Visible = false;
            ResetBoardVisualState();
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
        var creditsText = $"{teamCredits[0]} - {teamCredits[1]}";
        var tricksText = $"{teamTricks[0]} - {teamTricks[1]}";

        var trumpText = trumpSuit >= 0 ? ((CardSuit)trumpSuit).ToString() : "Not selected";
        _statusLabel.Text = $"Round {roundNumber} | Phase: {phase} | Turn: {BuildSeatTitle(currentTurnSeat)} | Trump: {trumpText} | Credits: {creditsText} | Tricks: {tricksText}";

        RenderHands(snapshot, phase, currentTurnSeat);
        RenderTrickState(snapshot, roundNumber, currentTurnSeat, teamTricks);
        RenderActionPanel(snapshot, phase);
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

    private void RenderHands(Godot.Collections.Dictionary snapshot, OmiPhase phase, SeatPosition currentTurnSeat)
    {
        var handCounts = snapshot.TryGetValue("handCounts", out var handCountsVariant) && handCountsVariant.VariantType == Variant.Type.Dictionary
            ? handCountsVariant.AsGodotDictionary()
            : new Godot.Collections.Dictionary();

        var visibleHands = snapshot.TryGetValue("visibleHands", out var visibleHandsVariant) && visibleHandsVariant.VariantType == Variant.Type.Dictionary
            ? visibleHandsVariant.AsGodotDictionary()
            : new Godot.Collections.Dictionary();

        foreach (SeatPosition seat in Enum.GetValues(typeof(SeatPosition)))
        {
            var container = GetHandContainer(seat);
            var visibleCards = new Godot.Collections.Array();

            if (visibleHands.TryGetValue(seat.ToString(), out var cardsVariant) && cardsVariant.VariantType == Variant.Type.Array)
            {
                visibleCards = cardsVariant.AsGodotArray();
            }

            var totalCount = handCounts.TryGetValue(seat.ToString(), out var countVariant)
                ? countVariant.AsInt32()
                : visibleCards.Count;

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
    }

    private void RenderTrickState(Godot.Collections.Dictionary snapshot, int roundNumber, SeatPosition currentTurnSeat, int[] teamTricks)
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
            tween.TweenProperty(card, "position", GetDeskCardPosition(seat, cardSize), 0.22f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            _deskCards[seat] = card;
            PlaySound(_placeSounds);
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

        NetworkRpc.Instance.SendPlayCardRequest(card.CardId);
        PlaySound(_placeSounds);
    }

    private static void OnBackLobbyPressed()
    {
        GameManager.Instance?.LoadLobby();
    }

    private Card CreateCard(CardModel model, bool faceUp, bool interactable, bool playRevealSound = false)
    {
        var card = _cardScene.Instantiate<Card>();
        card.Setup(ToViewSuit(model.Suit), ToViewRank(model.Rank), faceUp, model.Id);
        card.SetInteractable(interactable);

        if (interactable)
        {
            card.CardClicked += OnCardClicked;
        }

        if (faceUp && playRevealSound)
        {
            PlaySound(_slideSounds);
        }

        return card;
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
        ClearTrickAndPileVisuals();
        _lastRoundNumber = -1;
        _lastCompletedTricksCount = -1;
        _lastTeamTricks[0] = 0;
        _lastTeamTricks[1] = 0;
        _pendingFriendlyTricks = 0;
        _pendingEnemyTricks = 0;
        _leftPileLabel.Text = "Your Tricks";
        _rightPileLabel.Text = "Enemy Tricks";
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

        _bottomPlayerName.Text = seatNames[ToSeatForVisualSlot(VisualSlot.Bottom)];
        _rightPlayerName.Text = seatNames[ToSeatForVisualSlot(VisualSlot.Right)];
        _topPlayerName.Text = seatNames[ToSeatForVisualSlot(VisualSlot.Top)];
        _leftPlayerName.Text = seatNames[ToSeatForVisualSlot(VisualSlot.Left)];
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

        for (var i = 0; i < cards.Count; i++)
        {
            var target = baseTarget + new Vector2((i % 2) * 12, (i / 2) * 8);
            tween.TweenProperty(cards[i], "global_position", pile.GlobalPosition + target, 0.35f)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.InOut);
        }

        PlaySound(_slideSounds);

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
