using Godot;
using NetDex.Managers;

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
    private bool _isMobile;

    public override void _Ready()
    {
        var vbox = GetNode<VBoxContainer>("CenterContainer/MainPanel/VBoxContainer");

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

        string platform = OS.GetName();
        _isMobile = platform == "Android" || platform == "iOS";

        var displayLabel = vbox.GetNode<Label>("DisplayLabel");
        displayLabel.Visible = !_isMobile;
        _fullscreenToggle.Visible = !_isMobile;
        vbox.GetNode<HSeparator>("HSeparator2").Visible = !_isMobile;
        vbox.GetNode<HSeparator>("HSeparator3").Visible = !_isMobile;

        _qualityLabel.Visible = true;
        _resolutionSlider.Visible = true;

        LoadSettings();

        _fullscreenToggle.Toggled += OnFullscreenToggled;
        _resolutionSlider.ValueChanged += OnQualitySliderChanged;
        _musicSlider.ValueChanged += OnMusicVolumeChanged;
        _sfxSlider.ValueChanged += OnSfxVolumeChanged;
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
        SaveSettings();
    }

    private void OnMusicVolumeChanged(double value)
    {
        _musicValueLabel.Text = $"Music Volume: {(int)value}%";
        AudioManager.Instance?.SetMusicVolumePercent(value);
        SaveSettings();
    }

    private void OnSfxVolumeChanged(double value)
    {
        _sfxValueLabel.Text = $"SFX Volume: {(int)value}%";
        AudioManager.Instance?.SetSfxVolumePercent(value);
        SaveSettings();
    }

    private void SaveSettings()
    {
        var config = new ConfigFile();
        config.SetValue("display", "fullscreen", _fullscreenToggle.ButtonPressed);
        config.SetValue("display", "quality", _resolutionSlider.Value);
        config.SetValue("audio", "music_volume", _musicSlider.Value);
        config.SetValue("audio", "sfx_volume", _sfxSlider.Value);
        config.Save("user://settings.cfg");
    }

    private void LoadSettings()
    {
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

        Error err = config.Load("user://settings.cfg");
        if (err == Error.Ok)
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
            // On Android/iOS, keep fullscreen to preserve immersive behavior.
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
