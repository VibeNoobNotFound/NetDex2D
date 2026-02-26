using Godot;
using System;

public partial class Card : Control
{
    public enum SuitType { Hearts, Diamonds, Clubs, Spades }
    public enum RankType { Seven = 7, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public SuitType Suit { get; private set; }
    public RankType Rank { get; private set; }
    public bool IsFaceUp { get; private set; }

    [Signal]
    public delegate void CardClickedEventHandler(Card card);

    private Label _label;
    private ColorRect _bg;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _bg = GetNode<ColorRect>("Background");
        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        UpdateVisuals();
    }

    public void Setup(SuitType suit, RankType rank, bool isFaceUp = false)
    {
        Suit = suit;
        Rank = rank;
        IsFaceUp = isFaceUp;
        if (IsInsideTree())
        {
            UpdateVisuals();
        }
    }

    public void SetFaceUp(bool faceUp)
    {
        IsFaceUp = faceUp;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_label == null || _bg == null) return;

        if (IsFaceUp)
        {
            _bg.Color = Colors.White;
            string suitStr = Suit switch
            {
                SuitType.Hearts => "♥",
                SuitType.Diamonds => "♦",
                SuitType.Clubs => "♣",
                SuitType.Spades => "♠",
                _ => "?"
            };
            Color fontColor = (Suit == SuitType.Hearts || Suit == SuitType.Diamonds) ? Colors.Red : Colors.Black;
            _label.AddThemeColorOverride("font_color", fontColor);

            string rankStr = Rank switch
            {
                RankType.Jack => "J",
                RankType.Queen => "Q",
                RankType.King => "K",
                RankType.Ace => "A",
                _ => ((int)Rank).ToString()
            };

            _label.Text = $"{rankStr}\n{suitStr}";
        }
        else
        {
            _bg.Color = Colors.DarkSlateBlue;
            _label.Text = "OMI";
            _label.AddThemeColorOverride("font_color", Colors.White);
        }
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.CardClicked, this);
        }
    }

    private void OnMouseEntered()
    {
        // Simple hover effect
        Position -= new Vector2(0, 10);
    }

    private void OnMouseExited()
    {
        Position += new Vector2(0, 10);
    }
}
