using System.Threading.Tasks;
using Godot;

namespace NetDex.UI.Polish;

public partial class UiOverlay : CanvasLayer
{
    public static UiOverlay Instance { get; private set; } = null!;

    private ColorRect _fadeRect = null!;
    private VBoxContainer _toastContainer = null!;
    private PanelContainer _bannerPanel = null!;
    private Label _bannerLabel = null!;
    private Control _loadingBlocker = null!;
    private Label _loadingLabel = null!;
    private Label _loadingSpinnerLabel = null!;

    private bool _isBusy;
    private double _spinnerClock;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;

        _fadeRect = GetNode<ColorRect>("FadeRect");
        _toastContainer = GetNode<VBoxContainer>("ToastMargin/ToastContainer");
        _bannerPanel = GetNode<PanelContainer>("BannerPanel");
        _bannerLabel = GetNode<Label>("BannerPanel/BannerMargin/BannerLabel");
        _loadingBlocker = GetNode<Control>("LoadingBlocker");
        _loadingLabel = GetNode<Label>("LoadingBlocker/CenterContainer/LoadingPanel/LoadingVBox/LoadingLabel");
        _loadingSpinnerLabel = GetNode<Label>("LoadingBlocker/CenterContainer/LoadingPanel/LoadingVBox/LoadingSpinnerLabel");

        _fadeRect.Visible = false;
        _fadeRect.Modulate = new Color(0f, 0f, 0f, 0f);
        _bannerPanel.Visible = false;
        _loadingBlocker.Visible = false;

        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (!_isBusy || !_loadingBlocker.Visible)
        {
            return;
        }

        _spinnerClock += delta;
        var frame = (int)(_spinnerClock * 4.0) % 4;
        _loadingSpinnerLabel.Text = frame switch
        {
            0 => ".",
            1 => "..",
            2 => "...",
            _ => "...."
        };
    }

    public async Task FadeAsync(float fromAlpha, float toAlpha, double durationSeconds)
    {
        if (!IsInsideTree())
        {
            return;
        }

        _fadeRect.Visible = true;
        _fadeRect.Modulate = new Color(0f, 0f, 0f, Mathf.Clamp(fromAlpha, 0f, 1f));
        var tween = CreateTween();
        tween.TweenProperty(_fadeRect, "modulate:a", Mathf.Clamp(toAlpha, 0f, 1f), (float)durationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
        await ToSignal(tween, Tween.SignalName.Finished);
        if (toAlpha <= 0.01f)
        {
            _fadeRect.Visible = false;
        }
    }

    public void ShowToast(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.4)
    {
        _ = ShowToastAsync(message, severity, durationSeconds);
    }

    public async Task ShowToastAsync(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.4)
    {
        if (!IsInsideTree() || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(280, 0);
        panel.Modulate = new Color(1f, 1f, 1f, 0f);
        panel.Scale = new Vector2(0.95f, 0.95f);

        var style = new StyleBoxFlat
        {
            BgColor = GetSeverityColor(severity, 0.88f),
            BorderColor = GetSeverityColor(severity, 1.0f),
            BorderWidthBottom = 1,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthTop = 1,
            CornerRadiusBottomLeft = 10,
            CornerRadiusBottomRight = 10,
            CornerRadiusTopLeft = 10,
            CornerRadiusTopRight = 10,
            ContentMarginBottom = 10,
            ContentMarginTop = 10,
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ShadowColor = new Color(0, 0, 0, 0.35f),
            ShadowSize = 8
        };
        panel.AddThemeStyleboxOverride("panel", style);

        var label = new Label
        {
            Text = message,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        label.AddThemeColorOverride("font_color", Colors.WhiteSmoke);
        panel.AddChild(label);

        _toastContainer.AddChild(panel);

        var inTween = CreateTween();
        inTween.SetParallel(true);
        inTween.TweenProperty(panel, "modulate:a", 1f, (float)UiMotionProfile.MicroDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        inTween.TweenProperty(panel, "scale", Vector2.One, (float)UiMotionProfile.MicroDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(inTween, Tween.SignalName.Finished);

        await ToSignal(GetTree().CreateTimer(durationSeconds), SceneTreeTimer.SignalName.Timeout);

        var outTween = CreateTween();
        outTween.SetParallel(true);
        outTween.TweenProperty(panel, "modulate:a", 0f, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        outTween.TweenProperty(panel, "scale", new Vector2(0.96f, 0.96f), 0.2f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        await ToSignal(outTween, Tween.SignalName.Finished);

        panel.QueueFree();
    }

    public void ShowBanner(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.2)
    {
        _ = ShowBannerAsync(message, severity, durationSeconds);
    }

    public async Task ShowBannerAsync(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.2)
    {
        if (!IsInsideTree() || string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _bannerLabel.Text = message;
        _bannerPanel.Visible = true;
        _bannerPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _bannerPanel.Position = new Vector2(_bannerPanel.Position.X, 14f);

        var style = new StyleBoxFlat
        {
            BgColor = GetSeverityColor(severity, 0.9f),
            BorderColor = GetSeverityColor(severity, 1f),
            BorderWidthBottom = 1,
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthTop = 1,
            CornerRadiusBottomLeft = 12,
            CornerRadiusBottomRight = 12,
            CornerRadiusTopLeft = 12,
            CornerRadiusTopRight = 12,
            ContentMarginBottom = 10,
            ContentMarginTop = 10,
            ContentMarginLeft = 14,
            ContentMarginRight = 14
        };
        _bannerPanel.AddThemeStyleboxOverride("panel", style);

        var inTween = CreateTween();
        inTween.SetParallel(true);
        inTween.TweenProperty(_bannerPanel, "modulate:a", 1f, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        inTween.TweenProperty(_bannerPanel, "position:y", 22f, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        await ToSignal(inTween, Tween.SignalName.Finished);

        await ToSignal(GetTree().CreateTimer(durationSeconds), SceneTreeTimer.SignalName.Timeout);

        var outTween = CreateTween();
        outTween.SetParallel(true);
        outTween.TweenProperty(_bannerPanel, "modulate:a", 0f, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        outTween.TweenProperty(_bannerPanel, "position:y", 10f, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        await ToSignal(outTween, Tween.SignalName.Finished);

        _bannerPanel.Visible = false;
    }

    public void SetLoading(bool enabled, string message = "Loading...")
    {
        _isBusy = enabled;
        _loadingBlocker.Visible = enabled;
        _loadingLabel.Text = message;
        _spinnerClock = 0.0;
        _loadingSpinnerLabel.Text = enabled ? "." : string.Empty;
    }

    private static Color GetSeverityColor(UiSeverity severity, float alpha)
    {
        return severity switch
        {
            UiSeverity.Success => new Color(0.15f, 0.47f, 0.22f, alpha),
            UiSeverity.Warning => new Color(0.63f, 0.42f, 0.1f, alpha),
            UiSeverity.Error => new Color(0.54f, 0.16f, 0.18f, alpha),
            _ => new Color(0.16f, 0.24f, 0.46f, alpha)
        };
    }
}
