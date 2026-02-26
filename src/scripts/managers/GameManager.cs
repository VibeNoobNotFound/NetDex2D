using Godot;

namespace NetDex.Managers;

public partial class GameManager : Node
{
    private static readonly string[] ScreenNodeNames =
    {
        "MainMenu",
        "HostScreen",
        "JoinScreen",
        "LobbyScreen",
        "GameScreen",
        "SettingsMenu"
    };

    public static GameManager Instance { get; private set; } = null!;
    private string _settingsReturnScreen = "MainMenu";

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
    public void LoadSettingsMenu(string returnScreen = "MainMenu")
    {
        _settingsReturnScreen = string.IsNullOrWhiteSpace(returnScreen) ? "MainMenu" : returnScreen;
        SetMenuState("SettingsMenu");
    }

    public void ReturnFromSettings()
    {
        SetMenuState(_settingsReturnScreen);
    }

    private void SetMenuState(string menuName)
    {
        var mainRoot = GetTree().Root.GetNodeOrNull<Control>("Main");
        if (mainRoot == null)
        {
            return;
        }

        if (!IsKnownScreen(menuName))
        {
            menuName = "MainMenu";
        }

        // Background should stay visible across all screens.
        var globalBackground = mainRoot.GetNodeOrNull<CanvasItem>("GlobalBackground");
        if (globalBackground != null)
        {
            globalBackground.Visible = true;
        }

        foreach (var screenName in ScreenNodeNames)
        {
            var screen = mainRoot.GetNodeOrNull<CanvasItem>(screenName);
            if (screen == null)
            {
                continue;
            }

            screen.Visible = screenName == menuName;
        }
    }

    private static bool IsKnownScreen(string menuName)
    {
        foreach (var screenName in ScreenNodeNames)
        {
            if (screenName == menuName)
            {
                return true;
            }
        }

        return false;
    }
}
