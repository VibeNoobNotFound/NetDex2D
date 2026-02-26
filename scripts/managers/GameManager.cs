using Godot;

namespace NetDex.Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; } = null!;

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public void LoadMainMenu() => SetMenuState("MainMenu");
    public void LoadHostScreen() => SetMenuState("HostScreen");
    public void LoadJoinScreen() => SetMenuState("JoinScreen");
    public void LoadLobby() => SetMenuState("LobbyScreen");
    public void LoadGameScene() => SetMenuState("GameScreen");
    public void LoadSettingsMenu() => SetMenuState("SettingsMenu");

    private void SetMenuState(string menuName)
    {
        var mainRoot = GetTree().Root.GetNodeOrNull<Control>("Main");
        if (mainRoot == null)
        {
            return;
        }

        foreach (Node child in mainRoot.GetChildren())
        {
            if (child is Control controlChild)
            {
                controlChild.Visible = controlChild.Name == menuName;
            }
        }
    }
}
