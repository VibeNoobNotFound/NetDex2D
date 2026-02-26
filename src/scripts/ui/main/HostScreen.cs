using Godot;
using NetDex.Managers;
using NetDex.Networking;

namespace NetDex.UI.Main;

public partial class HostScreen : Control
{
    private LineEdit _playerNameInput = null!;
    private LineEdit _roomNameInput = null!;
    private Label _statusLabel = null!;
    private Label? _hostIpLabel;

    public override void _Ready()
    {
        _playerNameInput = GetNode<LineEdit>("CenterContainer/MainPanel/VBoxContainer/PlayerNameInput");
        _roomNameInput = GetNode<LineEdit>("CenterContainer/MainPanel/VBoxContainer/RoomNameInput");
        _statusLabel = GetNode<Label>("CenterContainer/MainPanel/VBoxContainer/StatusLabel");
        _hostIpLabel = GetNodeOrNull<Label>("CenterContainer/MainPanel/VBoxContainer/HostIpLabel");

        _playerNameInput.Text = NetworkManager.Instance.GetSavedPlayerName();
        if (_hostIpLabel != null)
        {
            _hostIpLabel.Text = $"LAN IP: {NetworkManager.Instance.GetLocalLanAddress()}:{NetworkManager.GamePort}";
        }

        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/CreateHostButton").Pressed += OnCreateHostPressed;
        GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/BackButton").Pressed += OnBackPressed;

        NetworkManager.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised += OnNetworkIssueRaised;
    }

    public override void _ExitTree()
    {
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
        SetStatus("Creating Room...");
        var result = NetworkManager.Instance.StartHostSession(_roomNameInput.Text, _playerNameInput.Text);
        if (result != Error.Ok)
        {
            SetStatus($"Host failed: {result}");
            return;
        }

        SetStatus("Room created. Opening lobby...");
        GameManager.Instance?.LoadLobby();
    }

    private static void OnBackPressed()
    {
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnConnectionStatusChanged(string status, string message)
    {
        SetStatus($"[{status}] {message}");
    }

    private void OnNetworkMessage(string message)
    {
        SetStatus(message);
    }

    private void OnNetworkIssueRaised(int issueCode, string message)
    {
        SetStatus(message);
    }

    private void SetStatus(string status)
    {
        _statusLabel.Text = status;
    }
}
