using System.Collections.Generic;
using System.Linq;
using Godot;
using NetDex.AI;
using NetDex.Core.Enums;
using NetDex.Lobby;
using NetDex.Managers;
using NetDex.Networking;
using NetDex.UI.Polish;

namespace NetDex.UI.Lobby;

public partial class LobbyScreen : Control
{
    private Label _roomLabel = null!;
    private Label _statusLabel = null!;
    private ItemList _playersList = null!;
    private ItemList _spectatorsList = null!;

    private PanelContainer _seatsPanel = null!;
    private Button _startMatchButton = null!;
    private Button _returnToGameButton = null!;
    private CheckButton _aiAutoFillCheck = null!;
    private OptionButton _aiDifficultyOption = null!;
    private Button _applyAiSettingsButton = null!;
    private VBoxContainer _mainContainer = null!;

    private readonly Dictionary<SeatPosition, OptionButton> _seatOptions = new();
    private readonly Dictionary<int, AiDifficulty> _difficultyByOptionId = new();
    private RoomMatchLifecycle? _lastLifecycle;
    private int _lastPlayerCount = -1;
    private int _lastSpectatorCount = -1;

    public override void _Ready()
    {
        _mainContainer = GetNode<VBoxContainer>("ScrollContainer/MarginContainer/VBoxContainer");
        _roomLabel = GetNode<Label>("ScrollContainer/MarginContainer/VBoxContainer/RoomLabel");
        _statusLabel = GetNode<Label>("ScrollContainer/MarginContainer/VBoxContainer/StatusLabel");
        _playersList = GetNode<ItemList>("ScrollContainer/MarginContainer/VBoxContainer/HBox/PlayersPanel/PlayersVBox/PlayersList");
        _spectatorsList = GetNode<ItemList>("ScrollContainer/MarginContainer/VBoxContainer/HBox/SpectatorsPanel/SpectatorsVBox/SpectatorsList");

        _seatsPanel = GetNode<PanelContainer>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel");

        _seatOptions[SeatPosition.Bottom] = GetNode<OptionButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid/BottomSeatOption");
        _seatOptions[SeatPosition.Right] = GetNode<OptionButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid/RightSeatOption");
        _seatOptions[SeatPosition.Top] = GetNode<OptionButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid/TopSeatOption");
        _seatOptions[SeatPosition.Left] = GetNode<OptionButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsGrid/LeftSeatOption");

        var applySeatsButton = GetNode<Button>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/SeatsActions/ApplySeatsButton");
        applySeatsButton.Pressed += OnApplySeatsPressed;

        _startMatchButton = GetNode<Button>("ScrollContainer/MarginContainer/VBoxContainer/Actions/StartMatchButton");
        _startMatchButton.Pressed += OnStartMatchPressed;

        _returnToGameButton = GetNode<Button>("ScrollContainer/MarginContainer/VBoxContainer/Actions/ReturnToGameButton");
        _returnToGameButton.Pressed += OnReturnToGamePressed;

        _aiAutoFillCheck = GetNode<CheckButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions/AiAutoFillCheck");
        _aiDifficultyOption = GetNode<OptionButton>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiOptions/AiDifficultyRow/AiDifficultyOption");
        _applyAiSettingsButton = GetNode<Button>("ScrollContainer/MarginContainer/VBoxContainer/SeatsPanel/SeatsVBox/AiActions/ApplyAiSettingsButton");
        _applyAiSettingsButton.Pressed += OnApplyAiSettingsPressed;
        PopulateAiDifficultyOptions();

        var leaveButton = GetNode<Button>("ScrollContainer/MarginContainer/VBoxContainer/Actions/LeaveRoomButton");
        leaveButton.Pressed += OnLeavePressed;

        VisibilityChanged += OnVisibilityChanged;

        LobbyManager.Instance.RoomStateChanged += RefreshLobbyView;
        NetworkManager.Instance.NetworkMessage += OnNetworkMessage;
        NetworkRpc.Instance.ServerMessage += OnServerMessage;
        NetworkRpc.Instance.ServerEventReceived += OnServerEventReceived;

        RefreshLobbyView();
    }

    public override void _ExitTree()
    {
        VisibilityChanged -= OnVisibilityChanged;

        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.RoomStateChanged -= RefreshLobbyView;
        }

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.NetworkMessage -= OnNetworkMessage;
        }

        if (NetworkRpc.Instance != null)
        {
            NetworkRpc.Instance.ServerMessage -= OnServerMessage;
            NetworkRpc.Instance.ServerEventReceived -= OnServerEventReceived;
        }
    }

    private void RefreshLobbyView()
    {
        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            _lastLifecycle = null;
            _roomLabel.Text = "No active room";
            SetStatus("Join or host a room from the main menu.", notify: false);
            return;
        }

        var previousLifecycle = _lastLifecycle;
        _lastLifecycle = room.MatchLifecycle;

        _roomLabel.Text = $"Room: {room.RoomName} | Host: {room.HostName} | State: {room.MatchLifecycle}";
        SetStatus($"Players: {room.PlayerCount} | Spectators: {room.SpectatorCount} | AI: {(room.AiAutoFillEnabled ? "Auto" : "Manual")} ({room.SelectedAiDifficulty})", notify: false);

        _playersList.Clear();
        _spectatorsList.Clear();

        foreach (var participant in room.Participants.Values.OrderBy(p => p.PeerId))
        {
            var tag = participant.IsConnected ? "online" : "offline";
            var seatText = participant.Seat?.ToString() ?? "No seat";
            var botTag = participant.IsBot
                ? $"[AI {participant.BotDifficulty ?? room.SelectedAiDifficulty}] "
                : string.Empty;
            var text = $"{botTag}{participant.Name} ({tag}) - {seatText}";
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
        _startMatchButton.Visible = isHost;
        _startMatchButton.Disabled = !isHost;
        _seatsPanel.Visible = isHost;
        _aiAutoFillCheck.Disabled = !isHost;
        _aiDifficultyOption.Disabled = !isHost;
        _applyAiSettingsButton.Disabled = !isHost;

        _returnToGameButton.Visible = room.MatchLifecycle != RoomMatchLifecycle.Lobby;
        _returnToGameButton.Disabled = room.MatchLifecycle == RoomMatchLifecycle.Lobby;

        foreach (var option in _seatOptions.Values)
        {
            option.Disabled = !isHost;
        }

        _aiAutoFillCheck.ButtonPressed = room.AiAutoFillEnabled;
        SelectDifficulty(room.SelectedAiDifficulty);

        if (previousLifecycle != null && previousLifecycle != room.MatchLifecycle)
        {
            UiFeedbackService.Instance?.ShowBanner($"Room State: {room.MatchLifecycle}", UiSeverity.Info, 2.0);
            AudioManager.Instance?.PlayUiCue(UiSfxCue.MatchPhase, 0.82f, 0.02f);
        }

        if (_lastPlayerCount >= 0 && room.PlayerCount != _lastPlayerCount)
        {
            PulseList(_playersList);
            AudioManager.Instance?.PlayUiCue(room.PlayerCount > _lastPlayerCount ? UiSfxCue.LobbyJoin : UiSfxCue.LobbyLeave, 0.82f, 0.02f);
        }

        if (_lastSpectatorCount >= 0 && room.SpectatorCount != _lastSpectatorCount)
        {
            PulseList(_spectatorsList);
            AudioManager.Instance?.PlayUiCue(room.SpectatorCount > _lastSpectatorCount ? UiSfxCue.LobbyJoin : UiSfxCue.LobbyLeave, 0.82f, 0.02f);
        }

        _lastPlayerCount = room.PlayerCount;
        _lastSpectatorCount = room.SpectatorCount;

        var shouldAutoOpenGame = room.MatchLifecycle != RoomMatchLifecycle.Lobby &&
                                 (previousLifecycle == null || previousLifecycle == RoomMatchLifecycle.Lobby);

        if (shouldAutoOpenGame)
        {
            GameManager.Instance?.LoadGameScene();
        }

        EmphasizeStartIfReady(room);
    }

    private void PopulateSeatOptions(RoomState room)
    {
        foreach (var pair in _seatOptions)
        {
            var seat = pair.Key;
            var option = pair.Value;
            option.Clear();
            option.AddItem("Empty", -1);

            foreach (var participant in room.Participants.Values.Where(p => p.IsConnected).OrderBy(p => p.IsBot ? 1 : 0).ThenBy(p => p.Name))
            {
                var name = participant.IsBot
                    ? $"[AI] {participant.Name}"
                    : participant.Name;
                option.AddItem(name, participant.PeerId);
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
            SetStatus("Only host can change seats.", notify: true);
            return;
        }

        var requestedSeats = new Dictionary<SeatPosition, int>();
        foreach (var pair in _seatOptions)
        {
            requestedSeats[pair.Key] = pair.Value.GetSelectedId();
        }

        var duplicatePeer = requestedSeats.Values
            .Where(peerId => peerId != -1)
            .GroupBy(peerId => peerId)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicatePeer != null)
        {
            var room = LobbyManager.Instance.CurrentRoom;
            var duplicateName = room != null && room.Participants.TryGetValue(duplicatePeer.Key, out var participant)
                ? participant.Name
                : $"Peer {duplicatePeer.Key}";
            SetStatus($"Seat assignment invalid: '{duplicateName}' is selected more than once.", notify: true);
            ShakeSeatPanel();
            return;
        }

        foreach (SeatPosition seat in System.Enum.GetValues(typeof(SeatPosition)))
        {
            var peerId = requestedSeats.TryGetValue(seat, out var selectedPeer) ? selectedPeer : -1;
            NetworkRpc.Instance.SendSeatChangeRequest(seat, peerId);
        }

        SetStatus("Seat update sent.", notify: true);
    }

    private void PopulateAiDifficultyOptions()
    {
        _aiDifficultyOption.Clear();
        _difficultyByOptionId.Clear();
        foreach (AiDifficulty difficulty in System.Enum.GetValues(typeof(AiDifficulty)))
        {
            var id = (int)difficulty;
            _aiDifficultyOption.AddItem(difficulty.ToString(), id);
            _difficultyByOptionId[id] = difficulty;
        }
    }

    private void SelectDifficulty(AiDifficulty difficulty)
    {
        var targetId = (int)difficulty;
        for (var i = 0; i < _aiDifficultyOption.ItemCount; i++)
        {
            if (_aiDifficultyOption.GetItemId(i) != targetId)
            {
                continue;
            }

            _aiDifficultyOption.Select(i);
            return;
        }
    }

    private void OnApplyAiSettingsPressed()
    {
        if (!LobbyManager.Instance.IsHostAuthority)
        {
            SetStatus("Only host can change AI settings.", notify: true);
            return;
        }

        var selectedId = _aiDifficultyOption.GetSelectedId();
        var difficulty = _difficultyByOptionId.TryGetValue(selectedId, out var mapped)
            ? mapped
            : AiDifficulty.Strong;
        NetworkRpc.Instance.SendSetAiOptionsRequest(_aiAutoFillCheck.ButtonPressed, difficulty);
        SetStatus("AI settings updated.", notify: true);
    }

    private void OnStartMatchPressed()
    {
        NetworkRpc.Instance.SendStartMatchRequest();
        SetStatus("Start match requested.", notify: true);
    }

    private static void OnLeavePressed()
    {
        NetworkManager.Instance.DisconnectSession("Left room");
        UiFeedbackService.Instance?.ShowToast("Left room.", UiSeverity.Info, 1.6);
        GameManager.Instance?.LoadMainMenu();
    }

    private static void OnReturnToGamePressed()
    {
        GameManager.Instance?.LoadGameScene();
    }

    private void OnServerEventReceived(string eventType, string payloadJson)
    {
        if (eventType == "PushMatchStarted")
        {
            UiFeedbackService.Instance?.ShowBanner("Match started", UiSeverity.Success, 1.8);
            GameManager.Instance?.LoadGameScene();
        }
    }

    private void OnNetworkMessage(string message)
    {
        SetStatus(message, notify: false);
    }

    private void OnServerMessage(string message)
    {
        SetStatus(message, notify: false);
    }

    private void SetStatus(string text, bool notify)
    {
        _statusLabel.Text = text;
        var severity = UiFeedbackService.InferSeverity(text);
        UiFeedbackService.Instance?.ApplyStatusLabelStyle(_statusLabel, severity);

        if (notify)
        {
            UiFeedbackService.Instance?.ShowToast(text, severity, 2.1);
        }
    }

    private void OnVisibilityChanged()
    {
        if (!Visible)
        {
            return;
        }

        _mainContainer.Modulate = new Color(1f, 1f, 1f, 0f);
        _mainContainer.Scale = new Vector2(0.98f, 0.98f);

        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_mainContainer, "modulate:a", 1f, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(_mainContainer, "scale", Vector2.One, (float)UiMotionProfile.PanelEnterDurationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void EmphasizeStartIfReady(RoomState room)
    {
        var allSeatsFilled = room.SeatAssignments.Values.All(v => v.HasValue);
        var allSeatedConnected = room.SeatAssignments.Values
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .All(peerId => room.Participants.TryGetValue(peerId, out var p) && p.IsConnected);

        var ready = room.MatchLifecycle == RoomMatchLifecycle.Lobby && allSeatsFilled && allSeatedConnected;
        if (!ready || !_startMatchButton.Visible)
        {
            _startMatchButton.Modulate = Colors.White;
            _startMatchButton.Scale = Vector2.One;
            return;
        }

        var pulse = CreateTween();
        pulse.SetLoops(2);
        pulse.TweenProperty(_startMatchButton, "scale", new Vector2(1.03f, 1.03f), 0.16f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
        pulse.TweenProperty(_startMatchButton, "scale", Vector2.One, 0.16f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        _startMatchButton.Modulate = new Color(1f, 1f, 0.92f, 1f);
    }

    private static void PulseList(Control list)
    {
        var tween = list.CreateTween();
        tween.TweenProperty(list, "modulate", new Color(1.08f, 1.08f, 1.12f, 1f), 0.1f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(list, "modulate", Colors.White, 0.2f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void ShakeSeatPanel()
    {
        if (UiSettings.ReduceMotion)
        {
            return;
        }

        var tween = CreateTween();
        tween.TweenProperty(_seatsPanel, "rotation_degrees", -2.0f, 0.05f);
        tween.TweenProperty(_seatsPanel, "rotation_degrees", 2.0f, 0.05f);
        tween.TweenProperty(_seatsPanel, "rotation_degrees", 0.0f, 0.05f);
    }
}
