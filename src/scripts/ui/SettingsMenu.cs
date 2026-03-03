using Godot;
using NetDex.Managers;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class SettingsMenu : Control
{
    private CheckButton _fullscreenToggle = null!;
    private HSlider _resolutionSlider = null!;
    private Label _qualityLabel = null!;
    private Label _musicValueLabel = null!;
    private Label _sfxValueLabel = null!;
    private HSlider _musicSlider = null!;
    private HSlider _sfxSlider = null!;
    private CheckButton _reduceMotionToggle = null!;
    private CheckButton _uiSfxToggle = null!;
    private OptionButton _effectsIntensityOption = null!;

    private bool _isMobile;
    private bool _isLoadingValues;

    public override void _Ready()
    {
        VisibilityChanged += OnVisibilityChanged;

        var scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
        scrollContainer.Set("vertical_scroll_mode", 2);
        scrollContainer.Set("horizontal_scroll_mode", 0);
        scrollContainer.FollowFocus = true;

        var vbox = GetNode<VBoxContainer>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer");

        var backBtn = vbox.GetNode<Button>("BackButton");
        backBtn.Pressed += OnBackPressed;
        vbox.GetNode<Button>("AboutButton").Pressed += OnAboutPressed;

        _fullscreenToggle = vbox.GetNode<CheckButton>("FullscreenToggle");
        _resolutionSlider = vbox.GetNode<HSlider>("ResolutionSlider");
        _qualityLabel = vbox.GetNode<Label>("QualityLabel");
        _musicSlider = vbox.GetNode<HSlider>("MusicVolumeSlider");
        _sfxSlider = vbox.GetNode<HSlider>("SfxVolumeSlider");
        _musicValueLabel = vbox.GetNode<Label>("MusicValueLabel");
        _sfxValueLabel = vbox.GetNode<Label>("SfxValueLabel");
        _reduceMotionToggle = vbox.GetNode<CheckButton>("ReduceMotionToggle");
        _uiSfxToggle = vbox.GetNode<CheckButton>("UiSfxToggle");
        _effectsIntensityOption = vbox.GetNode<OptionButton>("EffectsIntensityOption");

        string platform = OS.GetName();
        _isMobile = platform == "Android" || platform == "iOS";

        var displayLabel = vbox.GetNode<Label>("DisplayLabel");
        displayLabel.Visible = !_isMobile;
        _fullscreenToggle.Visible = !_isMobile;
        vbox.GetNode<HSeparator>("HSeparator2").Visible = !_isMobile;
        vbox.GetNode<HSeparator>("HSeparator3").Visible = !_isMobile;

        _qualityLabel.Visible = true;
        _resolutionSlider.Visible = true;

        PopulateEffectsIntensity();
        LoadSettings();

        _fullscreenToggle.Toggled += OnFullscreenToggled;
        _resolutionSlider.ValueChanged += OnQualitySliderChanged;
        _musicSlider.ValueChanged += OnMusicVolumeChanged;
        _sfxSlider.ValueChanged += OnSfxVolumeChanged;
        _reduceMotionToggle.Toggled += OnReduceMotionToggled;
        _uiSfxToggle.Toggled += OnUiSfxToggled;
        _effectsIntensityOption.ItemSelected += OnEffectsIntensitySelected;
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;
    }

    private void PopulateEffectsIntensity()
    {
        _effectsIntensityOption.Clear();
        _effectsIntensityOption.AddItem("Low", (int)UiEffectsIntensity.Low);
        _effectsIntensityOption.AddItem("Normal", (int)UiEffectsIntensity.Normal);
        _effectsIntensityOption.AddItem("Max", (int)UiEffectsIntensity.Max);
    }

    private void OnFullscreenToggled(bool toggled)
    {
        if (_isMobile)
        {
            return;
        }

        if (toggled)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }

        SaveSettings();
    }

    private void OnQualitySliderChanged(double value)
    {
        _qualityLabel.Text = $"Render Scale: {(int)value}%";
        GetWindow().ContentScaleFactor = (float)(value / 100.0);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.35f, 0.01f);
        SaveSettings();
    }

    private void OnMusicVolumeChanged(double value)
    {
        _musicValueLabel.Text = $"Music Volume: {(int)value}%";
        AudioManager.Instance?.SetMusicVolumePercent(value);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.28f, 0.01f);
        SaveSettings();
    }

    private void OnSfxVolumeChanged(double value)
    {
        _sfxValueLabel.Text = $"SFX Volume: {(int)value}%";
        AudioManager.Instance?.SetSfxVolumePercent(value);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.4f, 0.01f);
        SaveSettings();
    }

    private void OnReduceMotionToggled(bool toggled)
    {
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetReduceMotion(toggled);
        UiSettings.Save();
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Confirm, 0.7f, 0.02f);
        UiFeedbackService.Instance?.ShowToast($"Reduce Motion {(toggled ? "enabled" : "disabled")}", UiSeverity.Info, 1.8);
        SaveSettings();
    }

    private void OnUiSfxToggled(bool toggled)
    {
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetUiSfxEnabled(toggled);
        UiSettings.Save();
        AudioManager.Instance?.PlayUiCue(toggled ? UiSfxCue.Success : UiSfxCue.Cancel, 0.75f, 0.02f);
        UiFeedbackService.Instance?.ShowToast($"UI SFX {(toggled ? "enabled" : "disabled")}", UiSeverity.Info, 1.8);
        SaveSettings();
    }

    private void OnEffectsIntensitySelected(long index)
    {
        if (_isLoadingValues)
        {
            return;
        }

        var selected = _effectsIntensityOption.GetItemId((int)index);
        var intensity = selected switch
        {
            (int)UiEffectsIntensity.Low => UiEffectsIntensity.Low,
            (int)UiEffectsIntensity.Normal => UiEffectsIntensity.Normal,
            _ => UiEffectsIntensity.Max
        };

        UiSettings.SetEffectsIntensity(intensity);
        UiSettings.Save();
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Confirm, 0.7f, 0.02f);
        UiFeedbackService.Instance?.ShowToast($"Effects intensity: {intensity}", UiSeverity.Info, 1.8);
        SaveSettings();
    }

    private void SaveSettings()
    {
        var config = new ConfigFile();
        config.Load("user://settings.cfg");

        config.SetValue("display", "fullscreen", _fullscreenToggle.ButtonPressed);
        config.SetValue("display", "quality", _resolutionSlider.Value);
        config.SetValue("audio", "music_volume", _musicSlider.Value);
        config.SetValue("audio", "sfx_volume", _sfxSlider.Value);

        config.SetValue("ui", "reduce_motion", _reduceMotionToggle.ButtonPressed);
        config.SetValue("ui", "sfx_enabled", _uiSfxToggle.ButtonPressed);
        config.SetValue("ui", "effects_intensity", _effectsIntensityOption.GetSelectedId());

        config.Save("user://settings.cfg");
    }

    private void LoadSettings()
    {
        _isLoadingValues = true;
        UiSettings.Load();

        var config = new ConfigFile();
        _musicSlider.MinValue = 0;
        _musicSlider.MaxValue = 100;
        _musicSlider.Step = 1;
        _sfxSlider.MinValue = 0;
        _sfxSlider.MaxValue = 100;
        _sfxSlider.Step = 1;

        bool fullscreen = !_isMobile;
        double quality = 100.0;
        var defaultMusic = AudioManager.Instance?.MusicVolumePercent ?? 80;
        var defaultSfx = AudioManager.Instance?.SfxVolumePercent ?? 80;
        double music = defaultMusic;
        double sfx = defaultSfx;

        if (config.Load("user://settings.cfg") == Error.Ok)
        {
            if (!_isMobile)
            {
                fullscreen = (bool)config.GetValue("display", "fullscreen", true);
            }

            quality = (double)config.GetValue("display", "quality", 100.0);
            music = (double)config.GetValue("audio", "music_volume", defaultMusic);
            sfx = (double)config.GetValue("audio", "sfx_volume", defaultSfx);
        }

        _fullscreenToggle.SetPressedNoSignal(fullscreen);
        if (_isMobile)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else if (fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }

        _resolutionSlider.SetValueNoSignal(quality);
        _qualityLabel.Text = $"Render Scale: {(int)quality}%";
        GetWindow().ContentScaleFactor = (float)(quality / 100.0);

        _musicSlider.SetValueNoSignal(music);
        _sfxSlider.SetValueNoSignal(sfx);
        _musicValueLabel.Text = $"Music Volume: {(int)music}%";
        _sfxValueLabel.Text = $"SFX Volume: {(int)sfx}%";
        AudioManager.Instance?.SetMusicVolumePercent(music);
        AudioManager.Instance?.SetSfxVolumePercent(sfx);

        _reduceMotionToggle.SetPressedNoSignal(UiSettings.ReduceMotion);
        _uiSfxToggle.SetPressedNoSignal(UiSettings.UiSfxEnabled);
        SelectEffectsIntensity(UiSettings.EffectsIntensity);

        _isLoadingValues = false;
    }

    private void SelectEffectsIntensity(UiEffectsIntensity intensity)
    {
        var targetId = (int)intensity;
        for (var i = 0; i < _effectsIntensityOption.ItemCount; i++)
        {
            if (_effectsIntensityOption.GetItemId(i) == targetId)
            {
                _effectsIntensityOption.Select(i);
                return;
            }
        }

        _effectsIntensityOption.Select(2);
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        var panel = GetNode<Control>("ScrollContainer/MarginContainer/CenterContainer/MainPanel");
        panel.Modulate = new Color(1f, 1f, 1f, 0f);
        panel.Scale = new Vector2(0.97f, 0.97f);

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(panel, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(panel, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private static void OnBackPressed()
    {
        GameManager.Instance?.ReturnFromSettings();
    }

    private static void OnAboutPressed()
    {
        GameManager.Instance?.LoadAboutScreen("SettingsMenu");
    }
}
