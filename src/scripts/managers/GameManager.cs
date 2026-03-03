using System.Threading;
using System.Threading.Tasks;
using Godot;
using NetDex.UI.Polish;

namespace NetDex.Managers;

public partial class GameManager : Node
{
    private const string BackgroundTexturePath = "res://assets/images/background-red.jpg";
    private const int MaxSafeBackgroundDimension = 4096;

    private static readonly string[] ScreenNodeNames =
    {
        "MainMenu",
        "HostScreen",
        "JoinScreen",
        "LobbyScreen",
        "GameScreen",
        "SettingsMenu",
        "AboutScreen"
    };

    public static GameManager Instance { get; private set; } = null!;
    private string _settingsReturnScreen = "MainMenu";
    private string _aboutReturnScreen = "MainMenu";
    private string _activeScreen = "MainMenu";
    private readonly SemaphoreSlim _screenSwitchGate = new(1, 1);

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        EnsureMobileFullscreen();
        _ = InitializeUiStateAsync();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationResumed)
        {
            EnsureMobileFullscreen();
        }
    }

    public void LoadMainMenu() => _ = SetMenuStateAsync("MainMenu");
    public void LoadHostScreen() => _ = SetMenuStateAsync("HostScreen");
    public void LoadJoinScreen() => _ = SetMenuStateAsync("JoinScreen");
    public void LoadLobby() => _ = SetMenuStateAsync("LobbyScreen");
    public void LoadGameScene() => _ = SetMenuStateAsync("GameScreen");
    public void LoadAboutScreen(string returnScreen = "MainMenu")
    {
        _aboutReturnScreen = string.IsNullOrWhiteSpace(returnScreen) ? "MainMenu" : returnScreen;
        _ = SetMenuStateAsync("AboutScreen", ScreenTransitionStyle.ZoomFade);
    }

    public void ReturnFromAbout()
    {
        _ = SetMenuStateAsync(_aboutReturnScreen, ScreenTransitionStyle.Fade);
    }

    public void LoadSettingsMenu(string returnScreen = "MainMenu")
    {
        _settingsReturnScreen = string.IsNullOrWhiteSpace(returnScreen) ? "MainMenu" : returnScreen;
        _ = SetMenuStateAsync("SettingsMenu", ScreenTransitionStyle.ZoomFade);
    }

    public void ReturnFromSettings()
    {
        _ = SetMenuStateAsync(_settingsReturnScreen, ScreenTransitionStyle.Fade);
    }

    private async Task InitializeUiStateAsync()
    {
        for (var attempt = 0; attempt < 120; attempt++)
        {
            var mainRoot = GetTree().Root.GetNodeOrNull<Control>("Main");
            if (mainRoot != null)
            {
                EnsureBackgroundReady(mainRoot);
                await SetMenuStateAsync("MainMenu", ScreenTransitionStyle.Instant);
                return;
            }

            await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    public async Task SetMenuStateAsync(string menuName, ScreenTransitionStyle style = ScreenTransitionStyle.Auto)
    {
        await _screenSwitchGate.WaitAsync();
        try
        {
            await SetMenuStateInternalAsync(menuName, style);
        }
        finally
        {
            _screenSwitchGate.Release();
        }
    }

    private async Task SetMenuStateInternalAsync(string menuName, ScreenTransitionStyle style)
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

        var current = ResolveCurrentVisibleScreen(mainRoot);
        var target = mainRoot.GetNodeOrNull<CanvasItem>(menuName);
        if (target == null)
        {
            target = mainRoot.GetNodeOrNull<CanvasItem>("MainMenu");
            menuName = "MainMenu";
        }

        if (target == null)
        {
            return;
        }

        if (EnsureBackgroundReady(mainRoot) is { } background)
        {
            AnimateBackground(background, menuName);
        }

        var transitionController = mainRoot.GetNodeOrNull<ScreenTransitionController>("ScreenTransitionController");
        if (transitionController != null)
        {
            await transitionController.TransitionAsync(current, target, style);
        }
        else
        {
            if (current != null)
            {
                current.Visible = false;
            }

            target.Visible = true;
        }

        foreach (var screenName in ScreenNodeNames)
        {
            var screen = mainRoot.GetNodeOrNull<CanvasItem>(screenName);
            if (screen == null || screen == target)
            {
                continue;
            }

            screen.Visible = false;
        }

        target.Visible = true;
        _activeScreen = menuName;
    }

    private static TextureRect? EnsureBackgroundReady(Control mainRoot)
    {
        var background = mainRoot.GetNodeOrNull<TextureRect>("GlobalBackground");
        if (background == null)
        {
            return null;
        }

        background.Visible = true;
        background.Modulate = Colors.White;
        background.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        background.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;

        if (background.Texture == null)
        {
            background.Texture = GD.Load<Texture2D>(BackgroundTexturePath);
        }

        if (background.Texture != null &&
            (background.Texture.GetWidth() > MaxSafeBackgroundDimension || background.Texture.GetHeight() > MaxSafeBackgroundDimension))
        {
            GD.PushWarning($"Background texture is too large for many export targets ({background.Texture.GetWidth()}x{background.Texture.GetHeight()}). Using fallback.");
            background.Texture = CreateFallbackBackgroundTexture();
        }

        if (background.Texture == null)
        {
            background.Texture = CreateFallbackBackgroundTexture();
        }

        return background;
    }

    private static Texture2D CreateFallbackBackgroundTexture()
    {
        var image = Image.CreateEmpty(64, 64, false, Image.Format.Rgba8);
        for (var y = 0; y < 64; y++)
        {
            var t = y / 63f;
            var rowColor = new Color(
                Mathf.Lerp(0.62f, 0.32f, t),
                Mathf.Lerp(0.14f, 0.05f, t),
                Mathf.Lerp(0.11f, 0.04f, t),
                1f);

            for (var x = 0; x < 64; x++)
            {
                image.SetPixel(x, y, rowColor);
            }
        }

        return ImageTexture.CreateFromImage(image);
    }

    private CanvasItem? ResolveCurrentVisibleScreen(Control mainRoot)
    {
        if (!string.IsNullOrWhiteSpace(_activeScreen))
        {
            var known = mainRoot.GetNodeOrNull<CanvasItem>(_activeScreen);
            if (known != null && known.Visible)
            {
                return known;
            }
        }

        foreach (var screenName in ScreenNodeNames)
        {
            var screen = mainRoot.GetNodeOrNull<CanvasItem>(screenName);
            if (screen?.Visible == true)
            {
                return screen;
            }
        }

        return null;
    }

    private static void AnimateBackground(TextureRect background, string targetScreen)
    {
        var targetPosition = Vector2.Zero;
        var targetModulate = Colors.White;

        var tween = background.CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(background, "position", targetPosition, (float)UiMotionProfile.ScreenTransitionDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(background, "modulate", targetModulate, (float)UiMotionProfile.ScreenTransitionDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);
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

    private static void EnsureMobileFullscreen()
    {
        if (!OS.HasFeature("android") && !OS.HasFeature("ios"))
        {
            return;
        }

        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
    }
}
