using System.Collections.Generic;
using Godot;
using NetDex.Core.Enums;
using NetDex.Lobby;
using NetDex.Managers;
using NetDex.Networking;

namespace NetDex.UI.Main;

public partial class JoinScreen : Control
{
    private LineEdit _playerNameInput = null!;
    private LineEdit _directIpInput = null!;
    private ItemList _roomsList = null!;
    private Label _statusLabel = null!;

    private readonly List<RoomAdvertisement> _roomCache = new();
    private string _selectedRoomKey = string.Empty;

    public override void _Ready()
    {
        _playerNameInput = GetNode<LineEdit>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/PlayerNameInput");
        _directIpInput = GetNode<LineEdit>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/DirectIpInput");
        _roomsList = GetNode<ItemList>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/DiscoveredRooms");
        _statusLabel = GetNode<Label>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/StatusLabel");

        _playerNameInput.Text = NetworkManager.Instance.GetSavedPlayerName();

        _roomsList.ItemSelected += OnRoomSelected;

        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/RefreshRoomsButton").Pressed += OnRefreshPressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/JoinButton").Pressed += OnJoinAsPlayerPressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/SpectateButton").Pressed += OnJoinAsSpectatorPressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/DirectJoinButton").Pressed += OnDirectJoinPressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/DirectSpectateButton").Pressed += OnDirectSpectatePressed;
        GetNode<Button>("ScrollContainer/MarginContainer/CenterContainer/MainPanel/VBoxContainer/BackButton").Pressed += OnBackPressed;

        NetworkManager.Instance.DiscoveryUpdated += RefreshRooms;
        NetworkManager.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised += OnNetworkIssueRaised;

        NetworkManager.Instance.StartDiscovery();
        RefreshRooms();
    }

    public override void _ExitTree()
    {
        if (NetworkManager.Instance == null)
        {
            return;
        }

        NetworkManager.Instance.DiscoveryUpdated -= RefreshRooms;
        NetworkManager.Instance.ConnectionStatusChanged -= OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage -= OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised -= OnNetworkIssueRaised;
    }

    private void OnRefreshPressed()
    {
        NetworkManager.Instance.ForceDiscoveryRefreshSignal();
        RefreshRooms();
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

    private static void OnBackPressed()
    {
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnRoomSelected(long index)
    {
        if (index < 0 || index >= _roomCache.Count)
        {
            _selectedRoomKey = string.Empty;
            return;
        }

        _selectedRoomKey = _roomCache[(int)index].StableKey;
    }

    private void RefreshRooms()
    {
        _roomsList.Clear();
        _roomCache.Clear();

        foreach (var room in NetworkManager.Instance.GetDiscoveredRooms())
        {
            var index = _roomsList.AddItem($"{room.RoomName} | Host: {room.HostName} | Players: {room.PlayerCount}/4 | Specs: {room.SpectatorCount} | {room.MatchState}");
            _roomsList.SetItemMetadata(index, room.StableKey);
            _roomCache.Add(room);
        }

        if (_roomCache.Count == 0)
        {
            _selectedRoomKey = string.Empty;
            var index = _roomsList.AddItem("No rooms found on LAN");
            _roomsList.SetItemDisabled(index, true);
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedRoomKey))
        {
            return;
        }

        for (var i = 0; i < _roomCache.Count; i++)
        {
            if (_roomCache[i].StableKey == _selectedRoomKey)
            {
                _roomsList.Select(i);
                return;
            }
        }

        _selectedRoomKey = string.Empty;
    }

    private void JoinSelectedRoom(ParticipantRole role)
    {
        if (_roomsList.GetSelectedItems().Length == 0)
        {
            SetStatus("Select a room first.");
            return;
        }

        var selectedIndex = _roomsList.GetSelectedItems()[0];
        if (selectedIndex < 0 || selectedIndex >= _roomCache.Count)
        {
            SetStatus("Invalid room selection.");
            return;
        }

        var room = _roomCache[selectedIndex];
        _selectedRoomKey = room.StableKey;

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

    private void OnConnectionStatusChanged(string status, string message)
    {
        SetStatus($"[{status}] {message}");
    }

    private void OnNetworkMessage(string message)
    {
        if (message.Contains("Auto discovery unavailable"))
        {
            SetStatus($"{message} If no rooms appear, use Direct Host IP below.");
            return;
        }

        SetStatus(message);
    }

    private void OnNetworkIssueRaised(int issueCode, string message)
    {
        var code = (NetworkIssueCode)issueCode;
        if (code == NetworkIssueCode.DiscoveryBindFailed || code == NetworkIssueCode.MissingInternetPermission)
        {
            SetStatus($"{message} If no rooms appear, use Direct Host IP below.");
            return;
        }

        SetStatus(message);
    }

    private void SetStatus(string text)
    {
        _statusLabel.Text = text;
    }
}
