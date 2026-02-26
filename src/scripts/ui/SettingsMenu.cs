using Godot;
using NetDex.Managers;

namespace NetDex.UI.Main;

public partial class SettingsMenu : Control
{
    private CheckButton _fullscreenToggle = null!;
    private HSlider _resolutionSlider = null!;
    private Label _qualityLabel = null!;

    public override void _Ready()
    {
        var vbox = GetNode<VBoxContainer>("CenterContainer/VBoxContainer");

        var backBtn = vbox.GetNode<Button>("BackButton");
        backBtn.Pressed += OnBackPressed;

        _fullscreenToggle = vbox.GetNode<CheckButton>("FullscreenToggle");
        _resolutionSlider = vbox.GetNode<HSlider>("ResolutionSlider");
        _qualityLabel = vbox.GetNode<Label>("QualityLabel");

        string platform = OS.GetName();
        bool isMobile = platform == "Android" || platform == "iOS";

        var displayLabel = vbox.GetNode<Label>("DisplayLabel");
        displayLabel.Visible = !isMobile;
        _fullscreenToggle.Visible = !isMobile;
        vbox.GetNode<HSeparator>("HSeparator2").Visible = !isMobile;
        vbox.GetNode<HSeparator>("HSeparator3").Visible = !isMobile;

        _qualityLabel.Visible = true;
        _resolutionSlider.Visible = true;

        LoadSettings();

        _fullscreenToggle.Toggled += OnFullscreenToggled;
        _resolutionSlider.ValueChanged += OnQualitySliderChanged;
    }

    private void OnFullscreenToggled(bool toggled)
    {
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
        _qualityLabel.Text = $"Render Quality: {(int)value}%";
        GetWindow().ContentScaleFactor = (float)(value / 100.0);
        SaveSettings();
    }

    private void SaveSettings()
    {
        var config = new ConfigFile();
        config.SetValue("display", "fullscreen", _fullscreenToggle.ButtonPressed);
        config.SetValue("display", "quality", _resolutionSlider.Value);
        config.Save("user://settings.cfg");
    }

    private void LoadSettings()
    {
        var config = new ConfigFile();
        Error err = config.Load("user://settings.cfg");
        if (err != Error.Ok)
            return;

        bool fullscreen = (bool)config.GetValue("display", "fullscreen", false);
        _fullscreenToggle.SetPressedNoSignal(fullscreen);
        if (fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }

        double quality = (double)config.GetValue("display", "quality", 100.0);
        _resolutionSlider.SetValueNoSignal(quality);
        _qualityLabel.Text = $"Render Quality: {(int)quality}%";
        GetWindow().ContentScaleFactor = (float)(quality / 100.0);
    }

    private static void OnBackPressed()
    {
        GameManager.Instance?.LoadMainMenu();
    }
}
