using Godot;
using NetDex.Managers;

namespace NetDex.UI.Main;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/HostButton").Pressed += OnHostPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/JoinButton").Pressed += OnJoinPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/SettingsButton").Pressed += OnSettingsPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/ExitButton").Pressed += OnExitPressed;
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

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
