using System;
using System.Collections.Generic;
using Godot;

public partial class GameScreen : Control
{
    private HBoxContainer _bottomHand = null!;
    private HBoxContainer _topHand = null!;
    private VBoxContainer _leftHand = null!;
    private VBoxContainer _rightHand = null!;
    private Control _desk = null!;

    private Label _statusLabel = null!;
    private PanelContainer _actionPanel = null!;
    private Label _actionLabel = null!;
    private OptionButton _trumpOption = null!;
    private Button _actionButton = null!;

    private PackedScene _cardScene = null!;

    private AudioStreamOggVorbis[] _placeSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamOggVorbis[] _slideSounds = Array.Empty<AudioStreamOggVorbis>();
    private AudioStreamPlayer _sfxPlayer = null!;

    private SeatPosition? _localSeat;
    private ParticipantRole _localRole = ParticipantRole.Spectator;

    public override void _Ready()
    {
        _bottomHand = GetNode<HBoxContainer>("BottomHand");
        _topHand = GetNode<HBoxContainer>("TopHand");
        _leftHand = GetNode<VBoxContainer>("LeftHand");
        _rightHand = GetNode<VBoxContainer>("RightHand");
        _desk = GetNode<Control>("Desk");

        _statusLabel = GetNode<Label>("TopStatus");
        _actionPanel = GetNode<PanelContainer>("ActionPanel");
        _actionLabel = GetNode<Label>("ActionPanel/VBoxContainer/ActionLabel");
        _trumpOption = GetNode<OptionButton>("ActionPanel/VBoxContainer/TrumpOption");
        _actionButton = GetNode<Button>("ActionPanel/VBoxContainer/ActionButton");

        var backButton = GetNode<Button>("BackLobbyButton");
        backButton.Pressed += OnBackLobbyPressed;

        _actionButton.Pressed += OnActionButtonPressed;

        _trumpOption.Clear();
        _trumpOption.AddItem("Hearts", (int)CardSuit.Hearts);
        _trumpOption.AddItem("Diamonds", (int)CardSuit.Diamonds);
        _trumpOption.AddItem("Clubs", (int)CardSuit.Clubs);
        _trumpOption.AddItem("Spades", (int)CardSuit.Spades);

        _cardScene = GD.Load<PackedScene>("res://scenes/game/Card.tscn");

        _placeSounds = new AudioStreamOggVorbis[]
        {
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace1.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace2.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardPlace3.ogg"),
        };
        _slideSounds = new AudioStreamOggVorbis[]
        {
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide1.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide2.ogg"),
            GD.Load<AudioStreamOggVorbis>("res://assets/sounds/cardSlide3.ogg"),
        };

        _sfxPlayer = new AudioStreamPlayer();
        AddChild(_sfxPlayer);

        LobbyManager.Instance.MatchSnapshotChanged += OnMatchSnapshotChanged;
        LobbyManager.Instance.RoomStateChanged += OnRoomStateChanged;

        RefreshLocalIdentity();
        RenderSnapshot(LobbyManager.Instance.LocalMatchSnapshot);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            if (!HasNode("PauseMenu"))
            {
                var pauseMenuScene = GD.Load<PackedScene>("res://scenes/ui/PauseMenu.tscn");
                var pauseMenu = pauseMenuScene.Instantiate<Control>();
                pauseMenu.Name = "PauseMenu";
                AddChild(pauseMenu);
                pauseMenu.Show();
            }
        }
    }

    private void OnRoomStateChanged()
    {
        RefreshLocalIdentity();
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
        ClearHandsAndDesk();

        if (snapshot.Count == 0)
        {
            _statusLabel.Text = "Waiting for match state...";
            _actionPanel.Visible = false;
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
            currentTurnSeat = SeatPositionExtensions.Parse(turnVariant.AsString()) ?? SeatPosition.Bottom;
        }

        var teamCreditsText = "10 - 10";
        if (snapshot.TryGetValue("teamCredits", out var creditsVariant) && creditsVariant.VariantType == Variant.Type.Array)
        {
            var credits = creditsVariant.AsGodotArray();
            if (credits.Count >= 2)
            {
                teamCreditsText = $"{credits[0].AsInt32()} - {credits[1].AsInt32()}";
            }
        }

        var teamTricksText = "0 - 0";
        if (snapshot.TryGetValue("teamTricks", out var tricksVariant) && tricksVariant.VariantType == Variant.Type.Array)
        {
            var tricks = tricksVariant.AsGodotArray();
            if (tricks.Count >= 2)
            {
                teamTricksText = $"{tricks[0].AsInt32()} - {tricks[1].AsInt32()}";
            }
        }

        var trumpText = trumpSuit >= 0 ? ((CardSuit)trumpSuit).ToString() : "Not selected";
        _statusLabel.Text = $"Round {roundNumber} | Phase: {phase} | Turn: {currentTurnSeat} | Trump: {trumpText} | Credits: {teamCreditsText} | Tricks: {teamTricksText}";

        RenderHands(snapshot, phase, currentTurnSeat);
        RenderCurrentTrick(snapshot);
        RenderActionPanel(snapshot, phase);
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

            var totalCount = handCounts.TryGetValue(seat.ToString(), out var countVariant) ? countVariant.AsInt32() : visibleCards.Count;

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
                    var card = CreateCard(cardModel, true, canInteract);
                    container.AddChild(card);
                }
            }
            else
            {
                for (var i = 0; i < totalCount; i++)
                {
                    var hidden = CreateCard(new CardModel($"hidden-{seat}-{i}", CardSuit.Spades, CardRank.Ace), false, false);
                    container.AddChild(hidden);
                }
            }
        }
    }

    private void RenderCurrentTrick(Godot.Collections.Dictionary snapshot)
    {
        if (!snapshot.TryGetValue("currentTrick", out var currentTrickVariant) || currentTrickVariant.VariantType != Variant.Type.Array)
        {
            return;
        }

        var currentTrick = currentTrickVariant.AsGodotArray();
        foreach (var item in currentTrick)
        {
            if (item.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            var dict = item.AsGodotDictionary();
            var seat = dict.TryGetValue("seat", out var seatVariant)
                ? SeatPositionExtensions.Parse(seatVariant.AsString()) ?? SeatPosition.Bottom
                : SeatPosition.Bottom;

            if (!dict.TryGetValue("card", out var cardVariant) || cardVariant.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            var cardModel = CardModelConversions.FromDictionary(cardVariant.AsGodotDictionary());
            var card = CreateCard(cardModel, true, false);
            _desk.AddChild(card);
            card.Position = GetDeskCardPosition(seat, card.Size);
        }
    }

    private void RenderActionPanel(Godot.Collections.Dictionary snapshot, OmiPhase phase)
    {
        if (_localRole != ParticipantRole.Player || !_localSeat.HasValue)
        {
            _actionPanel.Visible = false;
            return;
        }

        var cutterSeat = snapshot.TryGetValue("cutterSeat", out var cutterVariant)
            ? SeatPositionExtensions.Parse(cutterVariant.AsString())
            : null;

        var trumpSelectorSeat = snapshot.TryGetValue("trumpSelectorSeat", out var selectorVariant)
            ? SeatPositionExtensions.Parse(selectorVariant.AsString())
            : null;

        if (phase == OmiPhase.Cut && cutterSeat == _localSeat)
        {
            _actionPanel.Visible = true;
            _actionLabel.Text = "Your action: cut the deck";
            _trumpOption.Visible = false;
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

        if (phase == OmiPhase.Cut)
        {
            var cutIndex = GD.RandRange(1, 31);
            NetworkRpc.Instance.SendCutDeckRequest(cutIndex);
            return;
        }

        if (phase == OmiPhase.TrumpSelect)
        {
            var suit = (CardSuit)_trumpOption.GetSelectedId();
            NetworkRpc.Instance.SendSelectTrumpRequest(suit);
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

    private void OnBackLobbyPressed()
    {
        GameManager.Instance?.LoadLobby();
    }

    private Card CreateCard(CardModel model, bool faceUp, bool interactable)
    {
        var card = _cardScene.Instantiate<Card>();
        card.Setup(ToViewSuit(model.Suit), ToViewRank(model.Rank), faceUp, model.Id);
        card.SetInteractable(interactable);

        if (interactable)
        {
            card.CardClicked += OnCardClicked;
        }

        if (faceUp)
        {
            PlaySound(_slideSounds);
        }

        return card;
    }

    private void ClearHandsAndDesk()
    {
        ClearContainer(_bottomHand);
        ClearContainer(_topHand);
        ClearContainer(_leftHand);
        ClearContainer(_rightHand);
        ClearContainer(_desk);
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
        return seat switch
        {
            SeatPosition.Bottom => _bottomHand,
            SeatPosition.Right => _rightHand,
            SeatPosition.Top => _topHand,
            SeatPosition.Left => _leftHand,
            _ => _bottomHand
        };
    }

    private Vector2 GetDeskCardPosition(SeatPosition seat, Vector2 cardSize)
    {
        var center = _desk.Size / 2f - cardSize / 2f;

        return seat switch
        {
            SeatPosition.Bottom => center + new Vector2(0, 110),
            SeatPosition.Right => center + new Vector2(110, 0),
            SeatPosition.Top => center + new Vector2(0, -110),
            SeatPosition.Left => center + new Vector2(-110, 0),
            _ => center
        };
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
