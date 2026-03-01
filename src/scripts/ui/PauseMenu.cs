using Godot;
using NetDex.Managers;
using NetDex.Networking;

namespace NetDex.UI.Game;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        Hide();

        GetNode<Button>("PanelContainer/VBoxContainer/ResumeButton").Pressed += OnResumePressed;
        GetNode<Button>("PanelContainer/VBoxContainer/SettingsButton").Pressed += OnSettingsPressed;
        GetNode<Button>("PanelContainer/VBoxContainer/LeaveButton").Pressed += OnLeavePressed;
    }

    private void OnResumePressed()
    {
        Hide();
    }

    private void OnSettingsPressed()
    {
        Hide();
        GameManager.Instance?.LoadSettingsMenu("GameScreen");
    }

    private static void OnLeavePressed()
    {
        NetworkManager.Instance.DisconnectSession("Left match");
        GameManager.Instance?.LoadMainMenu();
    }
}
