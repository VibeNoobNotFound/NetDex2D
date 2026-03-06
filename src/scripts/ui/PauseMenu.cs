using Godot;
using NetDex.Managers;
using NetDex.Networking;
using NetDex.UI.Polish;

namespace NetDex.UI.Game;

public partial class PauseMenu : Control
{
    private PanelContainer _panel = null!;
    private ColorRect _overlay = null!;

    public override void _Ready()
    {
        _overlay = GetNode<ColorRect>("ColorRect");
        _panel = GetNode<PanelContainer>("PanelContainer");

        HideImmediate();

        GetNode<Button>("PanelContainer/VBoxContainer/ResumeButton").Pressed += OnResumePressed;
        GetNode<Button>("PanelContainer/VBoxContainer/SettingsButton").Pressed += OnSettingsPressed;
        GetNode<Button>("PanelContainer/VBoxContainer/LeaveButton").Pressed += OnLeavePressed;
    }

    public void ShowMenu()
    {
        Show();
        _overlay.Modulate = new Color(1f, 1f, 1f, 0f);
        _panel.Modulate = new Color(1f, 1f, 1f, 0f);
        _panel.Scale = new Vector2(0.94f, 0.94f);

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_overlay, "modulate:a", 1f, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_panel, "modulate:a", 1f, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_panel, "scale", Vector2.One, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        AudioManager.Instance?.PlayUiCue(UiSfxCue.Click, 0.7f, 0.02f);
    }

    public void HideMenu()
    {
        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_overlay, "modulate:a", 0f, 0.14f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        tween.TweenProperty(_panel, "modulate:a", 0f, 0.14f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        tween.TweenProperty(_panel, "scale", new Vector2(0.96f, 0.96f), 0.14f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
        tween.Finished += HideImmediate;
    }

    private void OnResumePressed()
    {
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Cancel, 0.72f, 0.02f);
        HideMenu();
    }

    private void OnSettingsPressed()
    {
        HideMenu();
        GameManager.Instance?.LoadSettingsMenu("GameScreen");
    }

    private static void OnLeavePressed()
    {
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Cancel, 0.88f, 0.03f);
        NetworkManager.Instance.DisconnectSession("Left match");
        GameManager.Instance?.LoadMainMenu();
    }

    private void HideImmediate()
    {
        _panel.Scale = Vector2.One;
        _panel.Modulate = Colors.White;
        _overlay.Modulate = Colors.White;
        Hide();
    }
}
