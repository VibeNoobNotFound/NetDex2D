using Godot;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var playBtn = GetNode<Button>("VBoxContainer/PlayButton");
        playBtn.Pressed += OnPlayPressed;

        var hostBtn = GetNode<Button>("VBoxContainer/HostButton");
        hostBtn.Pressed += OnHostPressed;

        var settingsBtn = GetNode<Button>("VBoxContainer/SettingsButton");
        settingsBtn.Pressed += OnSettingsPressed;

        var exitBtn = GetNode<Button>("VBoxContainer/ExitButton");
        exitBtn.Pressed += OnExitPressed;
    }

    private void OnPlayPressed()
    {
        GameManager.Instance?.LoadGameScene();
    }

    private void OnHostPressed()
    {
        // For now, host options play game too
        GameManager.Instance?.LoadGameScene();
    }

    private void OnSettingsPressed()
    {
        // Show Settings
        GameManager.Instance?.LoadSettingsMenu();
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
