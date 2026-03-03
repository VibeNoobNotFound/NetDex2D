using Godot;
using NetDex.Managers;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class MainMenu : Control
{
    private VBoxContainer _buttonStack = null!;
    private Label _title = null!;
    private Label _subtitle = null!;

    public override void _Ready()
    {
        _buttonStack = GetNode<VBoxContainer>("CenterContainer/MainPanel/VBoxContainer");
        _title = GetNode<Label>("CenterContainer/MainPanel/VBoxContainer/Title");
        _subtitle = GetNode<Label>("CenterContainer/MainPanel/VBoxContainer/Subtitle");

        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/HostButton").Pressed += OnHostPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/JoinButton").Pressed += OnJoinPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/SettingsButton").Pressed += OnSettingsPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/AboutButton").Pressed += OnAboutPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/ExitButton").Pressed += OnExitPressed;

        VisibilityChanged += OnVisibilityChanged;
        OnVisibilityChanged();
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;
    }

    private void OnHostPressed()
    {
        GameManager.Instance?.LoadHostScreen();
    }

    private void OnJoinPressed()
    {
        GameManager.Instance?.LoadJoinScreen();
    }

    private void OnSettingsPressed()
    {
        GameManager.Instance?.LoadSettingsMenu("MainMenu");
    }

    private void OnAboutPressed()
    {
        GameManager.Instance?.LoadAboutScreen("MainMenu");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        _title.Modulate = new Color(1f, 1f, 1f, 0f);
        _title.Scale = new Vector2(0.98f, 0.98f);
        _subtitle.Modulate = new Color(1f, 1f, 1f, 0f);
        _subtitle.Scale = new Vector2(0.98f, 0.98f);

        var titleTween = CreateTween();
        titleTween.SetParallel(true);
        titleTween.TweenProperty(_title, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        titleTween.TweenProperty(_title, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        var subtitleTween = CreateTween();
        subtitleTween.SetParallel(true);
        subtitleTween.TweenProperty(_subtitle, "modulate:a", 1f, 0.24f)
            .SetDelay(0.1f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        subtitleTween.TweenProperty(_subtitle, "scale", Vector2.One, 0.24f)
            .SetDelay(0.1f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        var buttonIndex = 0;
        foreach (Node child in _buttonStack.GetChildren())
        {
            if (child is not Button button)
            {
                continue;
            }

            button.Modulate = new Color(1f, 1f, 1f, 0f);
            button.Scale = new Vector2(0.98f, 0.98f);

            var tween = CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(button, "modulate:a", 1f, 0.2f)
                .SetDelay(0.14f + buttonIndex * 0.05f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            tween.TweenProperty(button, "scale", Vector2.One, 0.2f)
                .SetDelay(0.14f + buttonIndex * 0.05f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);

            buttonIndex++;
        }

        AudioManager.Instance?.PlayUiCue(UiSfxCue.MatchPhase, 0.72f, 0.01f);
    }
}
