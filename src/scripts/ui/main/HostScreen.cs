using Godot;
using NetDex.Managers;
using NetDex.Networking;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class HostScreen : Control
{
    private LineEdit _playerNameInput = null!;
    private LineEdit _roomNameInput = null!;
    private Label _statusLabel = null!;
    private Label? _hostIpLabel;
    private Control _mainPanel = null!;
    private Button _createHostButton = null!;

    public override void _Ready()
    {
        _mainPanel = GetNode<Control>("ScrollContainer/MarginContainer/CenterContainer/MainPanel");
        _playerNameInput = GetNode<LineEdit>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/PlayerNameInput");
        _roomNameInput = GetNode<LineEdit>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/RoomNameInput");
        _statusLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/StatusLabel");
        _hostIpLabel = GetNodeOrNull<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/HostIpLabel");
        _createHostButton = GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/CreateHostButton");

        _playerNameInput.Text = NetworkManager.Instance.GetSavedPlayerName();
        if (_hostIpLabel != null)
        {
            _hostIpLabel.Text = $"LAN IP: {NetworkManager.Instance.GetLocalLanAddress()}:{NetworkManager.GamePort}";
        }

        _createHostButton.Pressed += OnCreateHostPressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/BackButton").Pressed += OnBackPressed;

        VisibilityChanged += OnVisibilityChanged;

        NetworkManager.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised += OnNetworkIssueRaised;
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;

        if (NetworkManager.Instance == null)
        {
            return;
        }

        NetworkManager.Instance.ConnectionStatusChanged -= OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage -= OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised -= OnNetworkIssueRaised;
    }

    private void OnCreateHostPressed()
    {
        _createHostButton.Disabled = true;
        UiFeedbackService.Instance?.SetLoading(true, "Creating room...");

        SetStatus("Creating room...", notify: false);
        var result = NetworkManager.Instance.StartHostSession(_roomNameInput.Text, _playerNameInput.Text);
        if (result != Error.Ok)
        {
            UiFeedbackService.Instance?.SetLoading(false);
            _createHostButton.Disabled = false;
            SetStatus($"Host failed: {result}", notify: true);
            ShakePanel();
            return;
        }

        UiFeedbackService.Instance?.SetLoading(false);
        SetStatus("Room created. Opening lobby...", notify: true);
        GameManager.Instance?.LoadLobby();
    }

    private static void OnBackPressed()
    {
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnConnectionStatusChanged(string status, string message)
    {
        _createHostButton.Disabled = status is "creating" or "connecting" or "hosting";
        if (status is "offline" or "error")
        {
            _createHostButton.Disabled = false;
        }

        SetStatus($"[{status}] {message}", notify: false);
    }

    private void OnNetworkMessage(string message)
    {
        SetStatus(message, notify: false);
    }

    private void OnNetworkIssueRaised(int issueCode, string message)
    {
        SetStatus(message, notify: true);
        ShakePanel();
    }

    private void SetStatus(string status, bool notify)
    {
        _statusLabel.Text = status;
        var severity = UiFeedbackService.InferSeverity(status);
        UiFeedbackService.Instance?.ApplyStatusLabelStyle(_statusLabel, severity);

        if (notify)
        {
            UiFeedbackService.Instance?.ShowToast(status, severity, 2.2);
        }
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        _createHostButton.Disabled = false;

        _mainPanel.Modulate = new Color(1f, 1f, 1f, 0f);
        _mainPanel.Scale = new Vector2(0.97f, 0.97f);

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_mainPanel, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_mainPanel, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void ShakePanel()
    {
        if (UiSettings.ReduceMotion)
        {
            return;
        }

        var tween = CreateTween();
        tween.TweenProperty(_mainPanel, "rotation_degrees", -2.0f, 0.05f);
        tween.TweenProperty(_mainPanel, "rotation_degrees", 2.0f, 0.05f);
        tween.TweenProperty(_mainPanel, "rotation_degrees", 0.0f, 0.05f);
    }
}
