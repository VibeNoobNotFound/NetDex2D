using Godot;

public partial class SettingsMenu : Control
{
    private CheckButton _fullscreenToggle;
    private HSlider _resolutionSlider;
    private Label _qualityLabel;

    public override void _Ready()
    {
        var vbox = GetNode<VBoxContainer>("CenterContainer/VBoxContainer");

        // Back button
        var backBtn = vbox.GetNode<Button>("BackButton");
        backBtn.Pressed += OnBackPressed;

        // Display controls
        _fullscreenToggle = vbox.GetNode<CheckButton>("FullscreenToggle");
        _resolutionSlider = vbox.GetNode<HSlider>("ResolutionSlider");
        _qualityLabel = vbox.GetNode<Label>("QualityLabel");

        // Platform check: hide display controls on mobile
        string platform = OS.GetName();
        bool isMobile = platform == "Android" || platform == "iOS";

        var displayLabel = vbox.GetNode<Label>("DisplayLabel");
        displayLabel.Visible = !isMobile;
        _fullscreenToggle.Visible = !isMobile;
        vbox.GetNode<HSeparator>("HSeparator2").Visible = !isMobile;
        vbox.GetNode<HSeparator>("HSeparator3").Visible = !isMobile;

        // Quality slider is always visible
        _qualityLabel.Visible = true;
        _resolutionSlider.Visible = true;

        // Load saved settings
        LoadSettings();

        // Connect signals
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
        float scale = (float)(value / 100.0);
        GetWindow().ContentScaleFactor = scale;
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

        // Fullscreen
        bool fullscreen = (bool)config.GetValue("display", "fullscreen", false);
        _fullscreenToggle.SetPressedNoSignal(fullscreen);
        if (fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }

        // Quality
        double quality = (double)config.GetValue("display", "quality", 100.0);
        _resolutionSlider.SetValueNoSignal(quality);
        _qualityLabel.Text = $"Render Quality: {(int)quality}%";
        GetWindow().ContentScaleFactor = (float)(quality / 100.0);
    }

    private void OnBackPressed()
    {
        GameManager.Instance?.LoadMainMenu();
    }
}
