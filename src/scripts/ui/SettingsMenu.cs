using System.Collections.Generic;
using Godot;
using NetDex.Managers;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class SettingsMenu : Control
{
    private readonly Dictionary<Control, SettingHelp> _settingHelp = new();

    private CheckButton _fullscreenToggle = null!;
    private ScrollContainer _scrollContainer = null!;
    private HSlider _resolutionSlider = null!;
    private Label _qualityLabel = null!;
    private Label _musicValueLabel = null!;
    private Label _sfxValueLabel = null!;
    private HSlider _musicSlider = null!;
    private HSlider _sfxSlider = null!;
    private CheckButton _reduceMotionToggle = null!;
    private CheckButton _uiSfxToggle = null!;
    private OptionButton _effectsIntensityOption = null!;
    private CheckButton _fluentBlurToggle = null!;
    private OptionButton _fluentBlurQualityOption = null!;
    private Button _advancedBlurToggleButton = null!;
    private VBoxContainer _advancedBlurSection = null!;
    private Label _fluentBlurStrengthLabel = null!;
    private HSlider _fluentBlurStrengthSlider = null!;
    private Label _fluentBlurDarknessLabel = null!;
    private HSlider _fluentBlurDarknessSlider = null!;
    private Label _fluentBlurReflectionLabel = null!;
    private HSlider _fluentBlurReflectionSlider = null!;
    private PanelContainer _infoPanel = null!;
    private Label _infoNameLabel = null!;
    private Label _infoDescriptionLabel = null!;
    private Label _infoHintLabel = null!;

    private bool _isMobile;
    private bool _isLoadingValues;
    private bool _advancedBlurExpanded;

    public override void _Ready()
    {
        VisibilityChanged += OnVisibilityChanged;
        GetViewport().SizeChanged += OnViewportSizeChanged;

        _scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
        _scrollContainer.Set("vertical_scroll_mode", 2);
        _scrollContainer.Set("horizontal_scroll_mode", 0);
        _scrollContainer.FollowFocus = true;

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
        _fluentBlurToggle = vbox.GetNode<CheckButton>("FluentBlurToggle");
        _fluentBlurQualityOption = vbox.GetNode<OptionButton>("FluentBlurQualityOption");
        _advancedBlurToggleButton = vbox.GetNode<Button>("AdvancedBlurToggleButton");
        _advancedBlurSection = vbox.GetNode<VBoxContainer>("AdvancedBlurSection");
        _fluentBlurStrengthLabel = vbox.GetNode<Label>("AdvancedBlurSection/FluentBlurStrengthLabel");
        _fluentBlurStrengthSlider = vbox.GetNode<HSlider>("AdvancedBlurSection/FluentBlurStrengthSlider");
        _fluentBlurDarknessLabel = vbox.GetNode<Label>("AdvancedBlurSection/FluentBlurDarknessLabel");
        _fluentBlurDarknessSlider = vbox.GetNode<HSlider>("AdvancedBlurSection/FluentBlurDarknessSlider");
        _fluentBlurReflectionLabel = vbox.GetNode<Label>("AdvancedBlurSection/FluentBlurReflectionLabel");
        _fluentBlurReflectionSlider = vbox.GetNode<HSlider>("AdvancedBlurSection/FluentBlurReflectionSlider");
        _infoPanel = GetNode<PanelContainer>("InfoPanel");
        _infoNameLabel = GetNode<Label>("InfoPanel/InfoVBox/InfoNameLabel");
        _infoDescriptionLabel = GetNode<Label>("InfoPanel/InfoVBox/InfoDescriptionLabel");
        _infoHintLabel = GetNode<Label>("InfoPanel/InfoVBox/InfoHintLabel");

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
        PopulateFluentBlurQuality();
        LoadSettings();

        _fullscreenToggle.Toggled += OnFullscreenToggled;
        _resolutionSlider.ValueChanged += OnQualitySliderChanged;
        _musicSlider.ValueChanged += OnMusicVolumeChanged;
        _sfxSlider.ValueChanged += OnSfxVolumeChanged;
        _reduceMotionToggle.Toggled += OnReduceMotionToggled;
        _uiSfxToggle.Toggled += OnUiSfxToggled;
        _effectsIntensityOption.ItemSelected += OnEffectsIntensitySelected;
        _fluentBlurToggle.Toggled += OnFluentBlurToggled;
        _fluentBlurQualityOption.ItemSelected += OnFluentBlurQualitySelected;
        _advancedBlurToggleButton.Pressed += OnAdvancedBlurTogglePressed;
        _fluentBlurStrengthSlider.ValueChanged += OnFluentBlurStrengthChanged;
        _fluentBlurDarknessSlider.ValueChanged += OnFluentBlurDarknessChanged;
        _fluentBlurReflectionSlider.ValueChanged += OnFluentBlurReflectionChanged;

        SetAdvancedBlurExpanded(false);
        InitializeSettingHelp();
        ShowSettingHelp(_resolutionSlider);
        SetInfoPanelVisibility();
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;
        GetViewport().SizeChanged -= OnViewportSizeChanged;
    }

    private void PopulateEffectsIntensity()
    {
        _effectsIntensityOption.Clear();
        _effectsIntensityOption.AddItem("Low", (int)UiEffectsIntensity.Low);
        _effectsIntensityOption.AddItem("Normal", (int)UiEffectsIntensity.Normal);
        _effectsIntensityOption.AddItem("Max", (int)UiEffectsIntensity.Max);
    }

    private void PopulateFluentBlurQuality()
    {
        _fluentBlurQualityOption.Clear();
        _fluentBlurQualityOption.AddItem("Low", (int)UiBlurQuality.Low);
        _fluentBlurQualityOption.AddItem("Normal", (int)UiBlurQuality.Normal);
        _fluentBlurQualityOption.AddItem("High", (int)UiBlurQuality.High);
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
        ShowSettingHelp(_resolutionSlider);
        _qualityLabel.Text = $"Render Scale: {(int)value}%";
        GetWindow().ContentScaleFactor = (float)(value / 100.0);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.35f, 0.01f);
        SaveSettings();
    }

    private void OnMusicVolumeChanged(double value)
    {
        ShowSettingHelp(_musicSlider);
        _musicValueLabel.Text = $"Music Volume: {(int)value}%";
        AudioManager.Instance?.SetMusicVolumePercent(value);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.28f, 0.01f);
        SaveSettings();
    }

    private void OnSfxVolumeChanged(double value)
    {
        ShowSettingHelp(_sfxSlider);
        _sfxValueLabel.Text = $"SFX Volume: {(int)value}%";
        AudioManager.Instance?.SetSfxVolumePercent(value);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.4f, 0.01f);
        SaveSettings();
    }

    private void OnReduceMotionToggled(bool toggled)
    {
        ShowSettingHelp(_reduceMotionToggle);
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
        ShowSettingHelp(_uiSfxToggle);
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
        ShowSettingHelp(_effectsIntensityOption);
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

    private void OnFluentBlurToggled(bool enabled)
    {
        ShowSettingHelp(_fluentBlurToggle);
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetFluentBlurEnabled(enabled);
        UiSettings.Save();
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Confirm, 0.72f, 0.02f);
        UiFeedbackService.Instance?.ShowToast($"Fluent blur {(enabled ? "enabled" : "disabled")}", UiSeverity.Info, 1.8);
        SaveSettings();
    }

    private void OnFluentBlurQualitySelected(long index)
    {
        ShowSettingHelp(_fluentBlurQualityOption);
        if (_isLoadingValues)
        {
            return;
        }

        var selected = _fluentBlurQualityOption.GetItemId((int)index);
        var quality = selected switch
        {
            (int)UiBlurQuality.Low => UiBlurQuality.Low,
            (int)UiBlurQuality.Normal => UiBlurQuality.Normal,
            _ => UiBlurQuality.High
        };

        UiSettings.SetFluentBlurQuality(quality);
        UiSettings.Save();
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Confirm, 0.72f, 0.02f);
        UiFeedbackService.Instance?.ShowToast($"Fluent blur quality: {quality}", UiSeverity.Info, 1.8);
        SaveSettings();
    }

    private void OnFluentBlurStrengthChanged(double value)
    {
        ShowSettingHelp(_fluentBlurStrengthSlider);
        _fluentBlurStrengthLabel.Text = $"Fluent Blur Strength: {(int)value}%";
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetFluentBlurStrength((float)(value / 100.0));
        UiSettings.Save();
        SaveSettings();
    }

    private void OnFluentBlurDarknessChanged(double value)
    {
        ShowSettingHelp(_fluentBlurDarknessSlider);
        _fluentBlurDarknessLabel.Text = $"Fluent Blur Darkness: {(int)value}%";
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetFluentBlurDarkness((float)(value / 100.0));
        UiSettings.Save();
        SaveSettings();
    }

    private void OnFluentBlurReflectionChanged(double value)
    {
        ShowSettingHelp(_fluentBlurReflectionSlider);
        _fluentBlurReflectionLabel.Text = $"Glass Reflection: {(int)value}%";
        if (_isLoadingValues)
        {
            return;
        }

        UiSettings.SetFluentBlurReflection((float)(value / 100.0));
        UiSettings.Save();
        SaveSettings();
    }

    private void OnAdvancedBlurTogglePressed()
    {
        ShowSettingHelp(_advancedBlurToggleButton);
        SetAdvancedBlurExpanded(!_advancedBlurExpanded);
    }

    private void SetAdvancedBlurExpanded(bool expanded)
    {
        _advancedBlurExpanded = expanded;
        _advancedBlurSection.Visible = expanded;
        _advancedBlurToggleButton.Text = expanded
            ? "Hide Advanced Blur Tuning"
            : "Show Advanced Blur Tuning";
    }

    private void InitializeSettingHelp()
    {
        RegisterSettingHelp(_fullscreenToggle, "Fullscreen", "Switches between exclusive fullscreen and windowed mode.", "Enable for immersive play on desktop.");
        RegisterSettingHelp(_resolutionSlider, "Render Scale", "Adjusts rendering resolution scale for sharpness vs performance.", "Lower values improve FPS on weaker devices.");
        RegisterSettingHelp(_musicSlider, "Music Volume", "Controls background music volume.", "Set to 0% to mute music.");
        RegisterSettingHelp(_sfxSlider, "SFX Volume", "Controls gameplay and interface sound effect volume.", "Set to 0% to mute sound effects.");
        RegisterSettingHelp(_reduceMotionToggle, "Reduce Motion", "Reduces motion-heavy UI effects and shakes.", "Recommended if you prefer calmer visuals.");
        RegisterSettingHelp(_uiSfxToggle, "UI SFX", "Enables or disables user interface sound cues.", "Disable if you prefer silent menu navigation.");
        RegisterSettingHelp(_effectsIntensityOption, "Effects Intensity", "Controls global visual effect intensity.", "Use Low for battery/performance, Max for full visuals.");
        RegisterSettingHelp(_fluentBlurToggle, "Fluent Blur", "Turns frosted blur panel effects on or off.", "Disable for maximum performance.");
        RegisterSettingHelp(_fluentBlurQualityOption, "Fluent Blur Quality", "Sets blur quality preset.", "High looks best, Low is lighter.");
        RegisterSettingHelp(_advancedBlurToggleButton, "Advanced Blur Tuning", "Shows detailed blur controls.", "Adjust only if you want custom visual style.");
        RegisterSettingHelp(_fluentBlurStrengthSlider, "Blur Strength", "Controls how strongly backgrounds are blurred.", "Higher values produce heavier glass blur.");
        RegisterSettingHelp(_fluentBlurDarknessSlider, "Blur Darkness", "Darkens frosted panels for better contrast.", "Increase if text needs stronger separation.");
        RegisterSettingHelp(_fluentBlurReflectionSlider, "Glass Reflection", "Adjusts highlight intensity on glass edges.", "Higher values create stronger reflective gradient.");
    }

    private void RegisterSettingHelp(Control control, string name, string description, string hint)
    {
        _settingHelp[control] = new SettingHelp(name, description, hint);
        control.FocusEntered += () => ShowSettingHelp(control);
        control.MouseEntered += () => ShowSettingHelp(control);
    }

    private void ShowSettingHelp(Control control)
    {
        if (!_settingHelp.TryGetValue(control, out var help))
        {
            return;
        }

        _infoNameLabel.Text = help.Name;
        _infoDescriptionLabel.Text = help.Description;
        _infoHintLabel.Text = $"Hint: {help.Hint}";
    }

    private void OnViewportSizeChanged()
    {
        SetInfoPanelVisibility();
    }

    private void SetInfoPanelVisibility()
    {
        var width = GetViewportRect().Size.X;
        var showInfoPanel = !_isMobile && width >= 1040f;
        _infoPanel.Visible = showInfoPanel;
        _scrollContainer.SetOffset(Side.Right, showInfoPanel ? -360f : 0f);
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
        config.SetValue("ui", "fluent_blur_enabled", _fluentBlurToggle.ButtonPressed);
        config.SetValue("ui", "fluent_blur_quality", _fluentBlurQualityOption.GetSelectedId());
        config.SetValue("ui", "fluent_blur_strength", _fluentBlurStrengthSlider.Value / 100.0);
        config.SetValue("ui", "fluent_blur_darkness", _fluentBlurDarknessSlider.Value / 100.0);
        config.SetValue("ui", "fluent_blur_reflection", _fluentBlurReflectionSlider.Value / 100.0);

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
        _fluentBlurToggle.SetPressedNoSignal(UiSettings.FluentBlurEnabled);
        SelectFluentBlurQuality(UiSettings.FluentBlurQuality);
        var blurStrengthPercent = Mathf.Clamp(Mathf.RoundToInt(UiSettings.FluentBlurStrength * 100.0f), 50, 500);
        _fluentBlurStrengthSlider.SetValueNoSignal(blurStrengthPercent);
        _fluentBlurStrengthLabel.Text = $"Fluent Blur Strength: {blurStrengthPercent}%";
        var blurDarknessPercent = Mathf.Clamp(Mathf.RoundToInt(UiSettings.FluentBlurDarkness * 100.0f), 50, 300);
        _fluentBlurDarknessSlider.SetValueNoSignal(blurDarknessPercent);
        _fluentBlurDarknessLabel.Text = $"Fluent Blur Darkness: {blurDarknessPercent}%";
        var blurReflectionPercent = Mathf.Clamp(Mathf.RoundToInt(UiSettings.FluentBlurReflection * 100.0f), 0, 300);
        _fluentBlurReflectionSlider.SetValueNoSignal(blurReflectionPercent);
        _fluentBlurReflectionLabel.Text = $"Glass Reflection: {blurReflectionPercent}%";

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

    private void SelectFluentBlurQuality(UiBlurQuality quality)
    {
        var targetId = (int)quality;
        for (var i = 0; i < _fluentBlurQualityOption.ItemCount; i++)
        {
            if (_fluentBlurQualityOption.GetItemId(i) == targetId)
            {
                _fluentBlurQualityOption.Select(i);
                return;
            }
        }

        _fluentBlurQualityOption.Select(2);
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        SetInfoPanelVisibility();

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

    private readonly struct SettingHelp
    {
        public SettingHelp(string name, string description, string hint)
        {
            Name = name;
            Description = description;
            Hint = hint;
        }

        public string Name { get; }
        public string Description { get; }
        public string Hint { get; }
    }
}
