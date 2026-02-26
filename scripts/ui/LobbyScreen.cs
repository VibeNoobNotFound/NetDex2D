using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LobbyScreen : Control
{
    private Label _roomLabel = null!;
    private Label _statusLabel = null!;
    private ItemList _playersList = null!;
    private ItemList _spectatorsList = null!;

    private readonly Dictionary<SeatPosition, OptionButton> _seatOptions = new();

    public override void _Ready()
    {
        _roomLabel = GetNode<Label>("MarginContainer/VBoxContainer/RoomLabel");
        _statusLabel = GetNode<Label>("MarginContainer/VBoxContainer/StatusLabel");
        _playersList = GetNode<ItemList>("MarginContainer/VBoxContainer/HBox/PlayersPanel/PlayersList");
        _spectatorsList = GetNode<ItemList>("MarginContainer/VBoxContainer/HBox/SpectatorsPanel/SpectatorsList");

        _seatOptions[SeatPosition.Bottom] = GetNode<OptionButton>("MarginContainer/VBoxContainer/SeatsPanel/SeatsGrid/BottomSeatOption");
        _seatOptions[SeatPosition.Right] = GetNode<OptionButton>("MarginContainer/VBoxContainer/SeatsPanel/SeatsGrid/RightSeatOption");
        _seatOptions[SeatPosition.Top] = GetNode<OptionButton>("MarginContainer/VBoxContainer/SeatsPanel/SeatsGrid/TopSeatOption");
        _seatOptions[SeatPosition.Left] = GetNode<OptionButton>("MarginContainer/VBoxContainer/SeatsPanel/SeatsGrid/LeftSeatOption");

        var applySeatsButton = GetNode<Button>("MarginContainer/VBoxContainer/SeatsPanel/SeatsActions/ApplySeatsButton");
        applySeatsButton.Pressed += OnApplySeatsPressed;

        var startMatchButton = GetNode<Button>("MarginContainer/VBoxContainer/Actions/StartMatchButton");
        startMatchButton.Pressed += OnStartMatchPressed;

        var openGameButton = GetNode<Button>("MarginContainer/VBoxContainer/Actions/OpenGameButton");
        openGameButton.Pressed += OnOpenGamePressed;

        var leaveButton = GetNode<Button>("MarginContainer/VBoxContainer/Actions/LeaveRoomButton");
        leaveButton.Pressed += OnLeavePressed;

        LobbyManager.Instance.RoomStateChanged += RefreshLobbyView;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkRpc.Instance.ServerMessage += OnServerMessage;

        RefreshLobbyView();
    }

    private void RefreshLobbyView()
    {
        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            _roomLabel.Text = "No active room";
            _statusLabel.Text = "Join or host a room from the main menu.";
            return;
        }

        _roomLabel.Text = $"Room: {room.RoomName} | Host: {room.HostName} | State: {room.MatchLifecycle}";
        _statusLabel.Text = $"Players: {room.PlayerCount} | Spectators: {room.SpectatorCount}";

        _playersList.Clear();
        _spectatorsList.Clear();

        foreach (var participant in room.Participants.Values.OrderBy(p => p.PeerId))
        {
            var tag = participant.IsConnected ? "online" : "offline";
            var seatText = participant.Seat?.ToString() ?? "No seat";
            var text = $"{participant.Name} ({tag}) - {seatText}";
            if (participant.Role == ParticipantRole.Player)
            {
                _playersList.AddItem(text);
            }
            else
            {
                _spectatorsList.AddItem(text);
            }
        }

        PopulateSeatOptions(room);

        var isHost = LobbyManager.Instance.IsHostAuthority;
        GetNode<Button>("MarginContainer/VBoxContainer/Actions/StartMatchButton").Visible = isHost;
        GetNode<Control>("MarginContainer/VBoxContainer/SeatsPanel").Visible = isHost;

        var canOpenGame = room.MatchLifecycle != RoomMatchLifecycle.Lobby;
        GetNode<Button>("MarginContainer/VBoxContainer/Actions/OpenGameButton").Visible = canOpenGame;

        if (canOpenGame)
        {
            GameManager.Instance?.LoadGameScene();
        }
    }

    private void PopulateSeatOptions(RoomState room)
    {
        foreach (var pair in _seatOptions)
        {
            var seat = pair.Key;
            var option = pair.Value;
            option.Clear();
            option.AddItem("Empty", -1);

            foreach (var participant in room.Participants.Values.Where(p => p.IsConnected))
            {
                option.AddItem(participant.Name, participant.PeerId);
            }

            var occupant = room.SeatAssignments[seat];
            var selectedIndex = 0;
            for (var i = 0; i < option.ItemCount; i++)
            {
                if (option.GetItemId(i) == (occupant ?? -1))
                {
                    selectedIndex = i;
                    break;
                }
            }

            option.Select(selectedIndex);
        }
    }

    private void OnApplySeatsPressed()
    {
        if (!LobbyManager.Instance.IsHostAuthority)
        {
            SetStatus("Only host can change seats.");
            return;
        }

        foreach (var pair in _seatOptions)
        {
            var seat = pair.Key;
            var option = pair.Value;
            var peerId = option.GetSelectedId();
            NetworkRpc.Instance.SendSeatChangeRequest(seat, peerId);
        }

        SetStatus("Seat update sent.");
    }

    private void OnStartMatchPressed()
    {
        NetworkRpc.Instance.SendStartMatchRequest();
        SetStatus("Start match requested.");
    }

    private void OnOpenGamePressed()
    {
        GameManager.Instance?.LoadGameScene();
    }

    private void OnLeavePressed()
    {
        NetworkManager.Instance.DisconnectSession("Left room");
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnNetworkMessage(string message)
    {
        SetStatus(message);
    }

    private void OnServerMessage(string message)
    {
        SetStatus(message);
    }

    private void SetStatus(string text)
    {
        _statusLabel.Text = text;
    }
}
