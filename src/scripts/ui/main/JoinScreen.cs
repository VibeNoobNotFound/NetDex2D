using System.Collections.Generic;
using Godot;
using NetDex.Core.Enums;
using NetDex.Lobby;
using NetDex.Managers;
using NetDex.Networking;
using NetDex.UI.Polish;

namespace NetDex.UI.Main;

public partial class JoinScreen : Control
{
    private LineEdit _playerNameInput = null!;
    private LineEdit _directIpInput = null!;
    private ItemList _roomsList = null!;
    private Label _statusLabel = null!;
    private Control _mainPanel = null!;

    private readonly List<RoomAdvertisement> _roomCache = new();
    private string _selectedRoomKey = string.Empty;
    private int _lastRoomCount = -1;

    public override void _Ready()
    {
        _mainPanel = GetNode<Control>("ScrollContainer/MarginContainer/CenterContainer/MainPanel");
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

        VisibilityChanged += OnVisibilityChanged;

        NetworkManager.Instance.DiscoveryUpdated += RefreshRooms;
        NetworkManager.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkManager.Instance.NetworkIssueRaised += OnNetworkIssueRaised;

        NetworkManager.Instance.StartDiscovery();
        RefreshRooms();
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;

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
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Focus, 0.72f, 0.02f);
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
        PulseRoomsList();
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
        }

        if (!string.IsNullOrWhiteSpace(_selectedRoomKey))
        {
            for (var i = 0; i < _roomCache.Count; i++)
            {
                if (_roomCache[i].StableKey == _selectedRoomKey)
                {
                    _roomsList.Select(i);
                    break;
                }
            }
        }

        if (_lastRoomCount >= 0 && _lastRoomCount != _roomCache.Count)
        {
            PulseRoomsList();
            if (_roomCache.Count > _lastRoomCount)
            {
                AudioManager.Instance?.PlayUiCue(UiSfxCue.LobbyJoin, 0.76f, 0.02f);
            }
            else
            {
                AudioManager.Instance?.PlayUiCue(UiSfxCue.LobbyLeave, 0.76f, 0.02f);
            }
        }

        _lastRoomCount = _roomCache.Count;
    }

    private void JoinSelectedRoom(ParticipantRole role)
    {
        if (_roomsList.GetSelectedItems().Length == 0)
        {
            SetStatus("Select a room first.", notify: true);
            ShakeControl(_roomsList);
            return;
        }

        var selectedIndex = _roomsList.GetSelectedItems()[0];
        if (selectedIndex < 0 || selectedIndex >= _roomCache.Count)
        {
            SetStatus("Invalid room selection.", notify: true);
            ShakeControl(_roomsList);
            return;
        }

        var room = _roomCache[selectedIndex];
        _selectedRoomKey = room.StableKey;

        UiFeedbackService.Instance?.SetLoading(true, "Connecting to room...");
        var result = NetworkManager.Instance.JoinRoomByIp(room.HostAddress, _playerNameInput.Text, role);
        if (result != Error.Ok)
        {
            UiFeedbackService.Instance?.SetLoading(false);
            SetStatus($"Join failed: {result}", notify: true);
            ShakeControl(_mainPanel);
            return;
        }

        SetStatus($"Connecting to {room.RoomName}...", notify: false);
    }

    private void JoinDirectIp(ParticipantRole role)
    {
        var ip = _directIpInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(ip))
        {
            SetStatus("Enter a host IP first.", notify: true);
            ShakeControl(_directIpInput);
            return;
        }

        UiFeedbackService.Instance?.SetLoading(true, "Connecting via direct IP...");
        var result = NetworkManager.Instance.JoinRoomByIp(ip, _playerNameInput.Text, role);
        if (result != Error.Ok)
        {
            UiFeedbackService.Instance?.SetLoading(false);
            SetStatus($"Join failed: {result}", notify: true);
            ShakeControl(_directIpInput);
            return;
        }

        SetStatus($"Connecting to {ip}...", notify: false);
    }

    private void OnConnectionStatusChanged(string status, string message)
    {
        if (status == "Connected")
        {
            UiFeedbackService.Instance?.SetLoading(false);
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Success, 0.85f, 0.02f);
        }
        else if (status == "Failed" || status == "Disconnected")
        {
            UiFeedbackService.Instance?.SetLoading(false);
        }

        SetStatus($"[{status}] {message}", notify: false);
    }

    private void OnNetworkMessage(string message)
    {
        if (message.Contains("Auto discovery unavailable"))
        {
            SetStatus($"{message} If no rooms appear, use Direct Host IP below.", notify: true);
            return;
        }

        SetStatus(message, notify: false);
    }

    private void OnNetworkIssueRaised(int issueCode, string message)
    {
        UiFeedbackService.Instance?.SetLoading(false);

        var code = (NetworkIssueCode)issueCode;
        if (code == NetworkIssueCode.DiscoveryBindFailed || code == NetworkIssueCode.MissingInternetPermission)
        {
            SetStatus($"{message} If no rooms appear, use Direct Host IP below.", notify: true);
            return;
        }

        SetStatus(message, notify: true);
    }

    private void SetStatus(string text, bool notify)
    {
        _statusLabel.Text = text;
        var severity = UiFeedbackService.InferSeverity(text);
        UiFeedbackService.Instance?.ApplyStatusLabelStyle(_statusLabel, severity);

        if (notify)
        {
            UiFeedbackService.Instance?.ShowToast(text, severity, 2.2);
        }
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

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

    private void PulseRoomsList()
    {
        var tween = CreateTween();
        tween.TweenProperty(_roomsList, "modulate", new Color(1.08f, 1.08f, 1.12f, 1f), 0.1f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_roomsList, "modulate", Colors.White, 0.18f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private static void ShakeControl(Control control)
    {
        if (UiSettings.ReduceMotion)
        {
            return;
        }

        var tween = control.CreateTween();
        tween.TweenProperty(control, "rotation_degrees", -2.0f, 0.05f);
        tween.TweenProperty(control, "rotation_degrees", 2.0f, 0.05f);
        tween.TweenProperty(control, "rotation_degrees", 0.0f, 0.05f);
    }
}
