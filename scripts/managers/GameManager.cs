using Godot;

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

    public void LoadGameScene()
    {
        SetMenuState("GameScreen");
    }

    public void LoadLobby()
    {
        SetMenuState("LobbyScreen");
    }

    public void LoadMainMenu()
    {
        SetMenuState("MainMenu");
    }

    public void LoadSettingsMenu()
    {
        SetMenuState("SettingsMenu");
    }

    private void SetMenuState(string menuName)
    {
        var mainRoot = GetTree().Root.GetNodeOrNull<Control>("Main");
        if (mainRoot != null)
        {
            foreach (Node child in mainRoot.GetChildren())
            {
                if (child is Control controlChild)
                {
                    controlChild.Visible = (controlChild.Name == menuName);
                }
            }
        }
    }
}
