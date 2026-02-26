using System.Collections.Generic;
using Godot;

public partial class MainMenu : Control
{
    private LineEdit _playerNameInput = null!;
    private LineEdit _roomNameInput = null!;
    private LineEdit _directIpInput = null!;
    private ItemList _roomsList = null!;
    private Label _statusLabel = null!;

    private readonly List<RoomAdvertisement> _roomCache = new();

    public override void _Ready()
    {
        _playerNameInput = GetNode<LineEdit>("CenterContainer/MainPanel/VBoxContainer/PlayerNameInput");
        _roomNameInput = GetNode<LineEdit>("CenterContainer/MainPanel/VBoxContainer/RoomNameInput");
        _directIpInput = GetNode<LineEdit>("CenterContainer/MainPanel/VBoxContainer/DirectIpInput");
        _roomsList = GetNode<ItemList>("CenterContainer/MainPanel/VBoxContainer/DiscoveredRooms");
        _statusLabel = GetNode<Label>("CenterContainer/MainPanel/VBoxContainer/StatusLabel");

        var hostButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/HostButton");
        hostButton.Pressed += OnHostPressed;

        var playButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/PlayButton");
        playButton.Pressed += OnJoinAsPlayerPressed;

        var spectateButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/SpectateButton");
        spectateButton.Pressed += OnJoinAsSpectatorPressed;

        var refreshButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/RefreshRoomsButton");
        refreshButton.Pressed += RefreshRooms;

        var directJoinButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/DirectJoinButton");
        directJoinButton.Pressed += OnDirectJoinPressed;

        var directSpectateButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/DirectSpectateButton");
        directSpectateButton.Pressed += OnDirectSpectatePressed;

        var settingsButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/SettingsButton");
        settingsButton.Pressed += OnSettingsPressed;

        var exitButton = GetNode<Button>("CenterContainer/MainPanel/VBoxContainer/ExitButton");
        exitButton.Pressed += OnExitPressed;

        _playerNameInput.Text = NetworkManager.Instance.GetSavedPlayerName();

        NetworkManager.Instance.DiscoveryUpdated += RefreshRooms;
        NetworkManager.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;

        NetworkManager.Instance.StartDiscovery();
        RefreshRooms();
    }

    private void OnHostPressed()
    {
        var playerName = _playerNameInput.Text;
        var roomName = _roomNameInput.Text;

        var result = NetworkManager.Instance.StartHostSession(roomName, playerName);
        if (result != Error.Ok)
        {
            SetStatus($"Host failed: {result}");
            return;
        }

        SetStatus("Room hosted. Waiting for players...");
        GameManager.Instance?.LoadLobby();
    }

    private void OnJoinAsPlayerPressed()
    {
        JoinSelectedRoom(ParticipantRole.Player);
    }

    private void OnJoinAsSpectatorPressed()
    {
        JoinSelectedRoom(ParticipantRole.Spectator);
    }

    private void OnDirectJoinPressed()
    {
        JoinDirectIp(ParticipantRole.Player);
    }

    private void OnDirectSpectatePressed()
    {
        JoinDirectIp(ParticipantRole.Spectator);
    }

    private void JoinSelectedRoom(ParticipantRole role)
    {
        var selected = _roomsList.GetSelectedItems();
        if (selected.Length == 0)
        {
            SetStatus("Select a room first.");
            return;
        }

        var index = selected[0];
        if (index < 0 || index >= _roomCache.Count)
        {
            SetStatus("Invalid room selection.");
            return;
        }

        var room = _roomCache[index];
        var result = NetworkManager.Instance.JoinRoomByIp(room.HostAddress, _playerNameInput.Text, role);
        if (result != Error.Ok)
        {
            SetStatus($"Join failed: {result}");
            return;
        }

        SetStatus($"Connecting to {room.RoomName}...");
    }

    private void JoinDirectIp(ParticipantRole role)
    {
        var ip = _directIpInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(ip))
        {
            SetStatus("Enter a host IP first.");
            return;
        }

        var result = NetworkManager.Instance.JoinRoomByIp(ip, _playerNameInput.Text, role);
        if (result != Error.Ok)
        {
            SetStatus($"Join failed: {result}");
            return;
        }

        SetStatus($"Connecting to {ip}...");
    }

    private void OnSettingsPressed()
    {
        GameManager.Instance?.LoadSettingsMenu();
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void RefreshRooms()
    {
        _roomsList.Clear();
        _roomCache.Clear();

        foreach (var room in NetworkManager.Instance.GetDiscoveredRooms())
        {
            var text = $"{room.RoomName} | Host: {room.HostName} | Players: {room.PlayerCount}/4 | Specs: {room.SpectatorCount} | {room.MatchState}";
            _roomsList.AddItem(text);
            _roomCache.Add(room);
        }

        if (_roomCache.Count == 0)
        {
            _roomsList.AddItem("No rooms found on LAN");
            _roomsList.SetItemDisabled(0, true);
        }
    }

    private void OnConnectionStatusChanged(string status, string message)
    {
        SetStatus($"[{status}] {message}");
    }

    private void OnNetworkMessage(string message)
    {
        SetStatus(message);
    }

    private void SetStatus(string text)
    {
        _statusLabel.Text = text;
    }
}
