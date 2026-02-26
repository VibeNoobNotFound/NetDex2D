using Godot;

namespace NetDex.UI.Game;

public partial class Card : Control
{
    public enum SuitType { Hearts, Diamonds, Clubs, Spades }
    public enum RankType { Seven = 7, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public SuitType Suit { get; private set; }
    public RankType Rank { get; private set; }
    public bool IsFaceUp { get; private set; }
    public string CardId { get; private set; } = string.Empty;
    public bool IsInteractable { get; private set; } = true;

    [Signal]
    public delegate void CardClickedEventHandler(Card card);

    private TextureRect _textureRect = null!;

    private static Texture2D _cardSheet = null!;
    private static Texture2D _backSheet = null!;

    private const int CardWidth = 140;
    private const int CardHeight = 190;

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("TextureRect");

        _cardSheet ??= GD.Load<Texture2D>("res://assets/spritesheets/playingCards.png");
        _backSheet ??= GD.Load<Texture2D>("res://assets/spritesheets/playingCardBacks.png");

        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        UpdateVisuals();
    }

    public void Setup(SuitType suit, RankType rank, bool isFaceUp = false, string cardId = "")
    {
        Suit = suit;
        Rank = rank;
        IsFaceUp = isFaceUp;
        CardId = cardId;

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

    public void SetInteractable(bool interactable)
    {
        IsInteractable = interactable;
    }

    private void UpdateVisuals()
    {
        if (_textureRect == null)
        {
            return;
        }

        if (IsFaceUp)
        {
            var atlas = new AtlasTexture
            {
                Atlas = _cardSheet,
                Region = GetCardRegion(Suit, Rank)
            };

            _textureRect.Texture = atlas;
        }
        else
        {
            var atlas = new AtlasTexture
            {
                Atlas = _backSheet,
                Region = new Rect2(140, 380, CardWidth, CardHeight)
            };

            _textureRect.Texture = atlas;
        }
    }

    private static Rect2 GetCardRegion(SuitType suit, RankType rank)
    {
        string suitName = suit switch
        {
            SuitType.Hearts => "Hearts",
            SuitType.Diamonds => "Diamonds",
            SuitType.Clubs => "Clubs",
            SuitType.Spades => "Spades",
            _ => "Spades"
        };

        string rankName = rank switch
        {
            RankType.Ace => "A",
            RankType.Jack => "J",
            RankType.Queen => "Q",
            RankType.King => "K",
            _ => ((int)rank).ToString()
        };

        return $"card{suitName}{rankName}" switch
        {
            "cardClubs7" => new Rect2(560, 1330, CardWidth, CardHeight),
            "cardClubs8" => new Rect2(560, 1140, CardWidth, CardHeight),
            "cardClubs9" => new Rect2(560, 950, CardWidth, CardHeight),
            "cardClubs10" => new Rect2(560, 760, CardWidth, CardHeight),
            "cardClubsJ" => new Rect2(560, 380, CardWidth, CardHeight),
            "cardClubsQ" => new Rect2(560, 0, CardWidth, CardHeight),
            "cardClubsK" => new Rect2(560, 190, CardWidth, CardHeight),
            "cardClubsA" => new Rect2(560, 570, CardWidth, CardHeight),

            "cardDiamonds7" => new Rect2(420, 760, CardWidth, CardHeight),
            "cardDiamonds8" => new Rect2(420, 570, CardWidth, CardHeight),
            "cardDiamonds9" => new Rect2(420, 380, CardWidth, CardHeight),
            "cardDiamonds10" => new Rect2(420, 190, CardWidth, CardHeight),
            "cardDiamondsJ" => new Rect2(280, 1710, CardWidth, CardHeight),
            "cardDiamondsQ" => new Rect2(280, 1330, CardWidth, CardHeight),
            "cardDiamondsK" => new Rect2(280, 1520, CardWidth, CardHeight),
            "cardDiamondsA" => new Rect2(420, 0, CardWidth, CardHeight),

            "cardHearts7" => new Rect2(280, 190, CardWidth, CardHeight),
            "cardHearts8" => new Rect2(280, 0, CardWidth, CardHeight),
            "cardHearts9" => new Rect2(140, 1710, CardWidth, CardHeight),
            "cardHearts10" => new Rect2(140, 1520, CardWidth, CardHeight),
            "cardHeartsJ" => new Rect2(140, 1140, CardWidth, CardHeight),
            "cardHeartsQ" => new Rect2(140, 760, CardWidth, CardHeight),
            "cardHeartsK" => new Rect2(140, 950, CardWidth, CardHeight),
            "cardHeartsA" => new Rect2(140, 1330, CardWidth, CardHeight),

            "cardSpades7" => new Rect2(0, 1330, CardWidth, CardHeight),
            "cardSpades8" => new Rect2(0, 1140, CardWidth, CardHeight),
            "cardSpades9" => new Rect2(0, 950, CardWidth, CardHeight),
            "cardSpades10" => new Rect2(0, 760, CardWidth, CardHeight),
            "cardSpadesJ" => new Rect2(0, 380, CardWidth, CardHeight),
            "cardSpadesQ" => new Rect2(0, 0, CardWidth, CardHeight),
            "cardSpadesK" => new Rect2(0, 190, CardWidth, CardHeight),
            "cardSpadesA" => new Rect2(0, 570, CardWidth, CardHeight),

            _ => new Rect2(0, 0, CardWidth, CardHeight)
        };
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!IsInteractable)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.CardClicked, this);
        }
    }

    private void OnMouseEntered()
    {
        if (!IsInteractable)
        {
            return;
        }

        Position -= new Vector2(0, 10);
    }

    private void OnMouseExited()
    {
        if (!IsInteractable)
        {
            return;
        }

        Position += new Vector2(0, 10);
    }
}
