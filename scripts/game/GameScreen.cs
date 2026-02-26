using Godot;
using System.Collections.Generic;

public partial class GameScreen : Control
{
    private HBoxContainer _bottomHand;
    private HBoxContainer _topHand;
    private VBoxContainer _leftHand;
    private VBoxContainer _rightHand;
    private Control _desk;

    private PackedScene _cardScene;

    public override void _Ready()
    {
        _bottomHand = GetNode<HBoxContainer>("BottomHand");
        _topHand = GetNode<HBoxContainer>("TopHand");
        _leftHand = GetNode<VBoxContainer>("LeftHand");
        _rightHand = GetNode<VBoxContainer>("RightHand");
        _desk = GetNode<Control>("Desk");

        _cardScene = GD.Load<PackedScene>("res://scenes/game/Card.tscn");

        // Deal dummy cards for testing
        TestCardDeal();
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
                pauseMenu.Show(); // Make sure it's visible if hidden by default
            }
        }
    }

    public void AddCardToHand(int playerId, Card.SuitType suit, Card.RankType rank, bool faceUp)
    {
        var card = _cardScene.Instantiate<Card>();

        switch (playerId)
        {
            case 0: _bottomHand.AddChild(card); break;
            case 1: _leftHand.AddChild(card); break;
            case 2: _topHand.AddChild(card); break;
            case 3: _rightHand.AddChild(card); break;
        }

        card.Setup(suit, rank, faceUp);
        card.CardClicked += OnCardClicked;
    }

    private void TestCardDeal()
    {
        // Player 0 (Bottom - Visible)
        for (int i = 0; i < 8; i++)
        {
            AddCardToHand(0, (Card.SuitType)(i % 4), (Card.RankType)(7 + i % 8), true);
        }

        // Other players (Hidden)
        for (int p = 1; p < 4; p++)
        {
            for (int i = 0; i < 8; i++)
            {
                AddCardToHand(p, Card.SuitType.Spades, Card.RankType.Ace, false);
            }
        }
    }

    private void OnCardClicked(Card card)
    {
        // For testing, if it's in a hand, play it to the desk
        if (card.GetParent() == _bottomHand || card.GetParent() == _topHand || card.GetParent() == _leftHand || card.GetParent() == _rightHand)
        {
            PlayCardToDesk(card);
        }
    }

    public void PlayCardToDesk(Card card)
    {
        card.CardClicked -= OnCardClicked; // Don't allow clicking again on desk

        var startGlobalPos = card.GlobalPosition;
        card.GetParent().RemoveChild(card);
        _desk.AddChild(card);

        // Put it in center but slightly random offset for desk cards
        Vector2 targetPos = _desk.Size / 2 - card.Size / 2;
        targetPos += new Vector2((float)GD.RandRange(-40, 40), (float)GD.RandRange(-40, 40));

        card.GlobalPosition = startGlobalPos;
        card.SetFaceUp(true); // Always face up on desk

        using var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(card, "position", targetPos, 0.3f);
    }
}
