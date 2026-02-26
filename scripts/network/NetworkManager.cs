using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public partial class NetworkManager : Node
{
    public const int GamePort = 7777;
    public const int DiscoveryPort = 7778;
    public const double ReconnectTimeoutSeconds = 90.0;
    public const int MaxSpectators = 32;
    public const int MaxPlayers = 4;

    public static NetworkManager Instance { get; private set; } = null!;

    private PacketPeerUdp? _discoverySender;
    private PacketPeerUdp? _discoveryReceiver;

    private Timer? _advertiseTimer;
    private Timer? _discoveryPollTimer;

    private readonly Dictionary<string, RoomAdvertisement> _discoveredRooms = new();

    private string _pendingPlayerName = "Player";
    private ParticipantRole _pendingRole = ParticipantRole.Player;

    [Signal]
    public delegate void DiscoveryUpdatedEventHandler();

    [Signal]
    public delegate void ConnectionStatusChangedEventHandler(string status, string message);

    [Signal]
    public delegate void NetworkMessageEventHandler(string message);

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;

        _advertiseTimer = new Timer
        {
            WaitTime = 1.0,
            OneShot = false,
            Autostart = false
        };
        AddChild(_advertiseTimer);
        _advertiseTimer.Timeout += OnAdvertiseTimerTimeout;

        _discoveryPollTimer = new Timer
        {
            WaitTime = 0.2,
            OneShot = false,
            Autostart = false
        };
        AddChild(_discoveryPollTimer);
        _discoveryPollTimer.Timeout += PollDiscoveryPackets;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;

        var rpc = GetNodeOrNull<NetworkRpc>("/root/NetworkRpc");
        if (rpc != null)
        {
            rpc.ServerMessage += OnServerMessage;
        }

        StartDiscovery();
    }

    public override void _ExitTree()
    {
        StopAdvertising();
        StopDiscovery();
    }

    public IReadOnlyList<RoomAdvertisement> GetDiscoveredRooms()
    {
        return _discoveredRooms.Values
            .OrderByDescending(room => room.LastSeenUnixSeconds)
            .ToList();
    }

    public string GetSavedPlayerName()
    {
        var config = new ConfigFile();
        if (config.Load("user://player_profile.cfg") != Error.Ok)
        {
            return "Player";
        }

        return config.GetValue("profile", "name", "Player").AsString();
    }

    public string GetOrCreateReconnectToken()
    {
        var config = new ConfigFile();
        config.Load("user://player_profile.cfg");

        var token = config.GetValue("profile", "reconnect_token", string.Empty).AsString();
        if (string.IsNullOrWhiteSpace(token))
        {
            token = Guid.NewGuid().ToString("N");
            config.SetValue("profile", "reconnect_token", token);
            config.Save("user://player_profile.cfg");
        }

        return token;
    }

    public void SavePlayerName(string playerName)
    {
        var sanitized = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        var config = new ConfigFile();
        config.Load("user://player_profile.cfg");
        config.SetValue("profile", "name", sanitized);

        var existingToken = config.GetValue("profile", "reconnect_token", string.Empty).AsString();
        if (string.IsNullOrWhiteSpace(existingToken))
        {
            config.SetValue("profile", "reconnect_token", Guid.NewGuid().ToString("N"));
        }

        config.Save("user://player_profile.cfg");
    }

    public Error StartHostSession(string roomName, string playerName)
    {
        DisconnectSession("Starting host session", notifyServer: false);
        StopDiscovery();

        SavePlayerName(playerName);
        _pendingPlayerName = playerName;
        _pendingRole = ParticipantRole.Player;

        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateServer(GamePort, MaxPlayers + MaxSpectators + 1);
        if (result != Error.Ok)
        {
            EmitSignal(SignalName.ConnectionStatusChanged, "error", $"Failed to host: {result}");
            return result;
        }

        Multiplayer.MultiplayerPeer = peer;

        var reconnectToken = GetOrCreateReconnectToken();
        LobbyManager.Instance.CreateHostedRoom(roomName, playerName, reconnectToken);

        StartAdvertising();
        EmitSignal(SignalName.ConnectionStatusChanged, "hosting", $"Hosting room '{roomName}' on port {GamePort}");
        return Error.Ok;
    }

    public Error JoinRoomByIp(string hostAddress, string playerName, ParticipantRole role)
    {
        DisconnectSession("Starting join", notifyServer: false);

        SavePlayerName(playerName);
        _pendingPlayerName = playerName;
        _pendingRole = role;

        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateClient(hostAddress, GamePort);
        if (result != Error.Ok)
        {
            EmitSignal(SignalName.ConnectionStatusChanged, "error", $"Failed to connect: {result}");
            return result;
        }

        Multiplayer.MultiplayerPeer = peer;
        EmitSignal(SignalName.ConnectionStatusChanged, "connecting", $"Connecting to {hostAddress}:{GamePort}");
        return Error.Ok;
    }

    public void DisconnectSession(string reason = "Disconnected", bool notifyServer = true)
    {
        if (notifyServer && Multiplayer.MultiplayerPeer != null && !Multiplayer.IsServer())
        {
            NetworkRpc.Instance.SendLeaveRoomRequest();
        }

        if (Multiplayer.MultiplayerPeer is ENetMultiplayerPeer enetPeer)
        {
            enetPeer.Close();
        }

        Multiplayer.MultiplayerPeer = null;

        StopAdvertising();
        StartDiscovery();

        LobbyManager.Instance.LeaveRoomLocally(reason);
        EmitSignal(SignalName.ConnectionStatusChanged, "offline", reason);
    }

    public void StartDiscovery()
    {
        if (_discoveryReceiver != null)
        {
            return;
        }

        _discoveryReceiver = new PacketPeerUdp();
        _discoveryReceiver.SetBroadcastEnabled(true);

        var bindResult = _discoveryReceiver.Bind(DiscoveryPort, "*");
        if (bindResult != Error.Ok)
        {
            EmitSignal(SignalName.NetworkMessage, $"Discovery bind failed: {bindResult}");
            _discoveryReceiver.Close();
            _discoveryReceiver = null;
            return;
        }

        _discoveryPollTimer?.Start();
    }

    public void StopDiscovery()
    {
        _discoveryPollTimer?.Stop();
        if (_discoveryReceiver != null)
        {
            _discoveryReceiver.Close();
            _discoveryReceiver = null;
        }
    }

    private void StartAdvertising()
    {
        if (_discoverySender == null)
        {
            _discoverySender = new PacketPeerUdp();
            _discoverySender.SetBroadcastEnabled(true);
        }

        _advertiseTimer?.Start();
    }

    private void StopAdvertising()
    {
        _advertiseTimer?.Stop();

        if (_discoverySender != null)
        {
            _discoverySender.Close();
            _discoverySender = null;
        }
    }

    private void PollDiscoveryPackets()
    {
        if (_discoveryReceiver == null)
        {
            return;
        }

        var changed = false;

        while (_discoveryReceiver.GetAvailablePacketCount() > 0)
        {
            var packet = _discoveryReceiver.GetPacket();
            var hostIp = _discoveryReceiver.GetPacketIP();
            var json = Encoding.UTF8.GetString(packet);

            var parsed = Json.ParseString(json);
            if (parsed.VariantType != Variant.Type.Dictionary)
            {
                continue;
            }

            var dict = parsed.AsGodotDictionary();
            dict["hostAddress"] = hostIp;
            dict["lastSeen"] = Time.GetUnixTimeFromSystem();

            var advertisement = RoomAdvertisement.FromDictionary(dict);
            var key = $"{advertisement.HostAddress}:{advertisement.Port}:{advertisement.RoomName}";
            _discoveredRooms[key] = advertisement;
            changed = true;
        }

        var now = Time.GetUnixTimeFromSystem();
        var expiredKeys = _discoveredRooms
            .Where(pair => now - pair.Value.LastSeenUnixSeconds > 3.2)
            .Select(pair => pair.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _discoveredRooms.Remove(key);
            changed = true;
        }

        if (changed)
        {
            EmitSignal(SignalName.DiscoveryUpdated);
        }
    }

    private void OnAdvertiseTimerTimeout()
    {
        if (_discoverySender == null || !Multiplayer.IsServer())
        {
            return;
        }

        var room = LobbyManager.Instance.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var localIp = GetPreferredLocalAddress();
        var advertisement = LobbyManager.Instance.BuildAdvertisement(localIp).ToDictionary();
        var payload = Json.Stringify(advertisement);

        _discoverySender.SetDestAddress("255.255.255.255", DiscoveryPort);
        _discoverySender.PutPacket(Encoding.UTF8.GetBytes(payload));
    }

    private static string GetPreferredLocalAddress()
    {
        var addresses = IP.GetLocalAddresses();
        foreach (var address in addresses)
        {
            if (address.StartsWith("127."))
            {
                continue;
            }

            if (address.Contains('.'))
            {
                return address;
            }
        }

        return addresses.Length > 0 ? addresses[0] : "127.0.0.1";
    }

    private void OnPeerConnected(long peerId)
    {
        EmitSignal(SignalName.NetworkMessage, $"Peer connected: {peerId}");
    }

    private void OnPeerDisconnected(long peerId)
    {
        EmitSignal(SignalName.NetworkMessage, $"Peer disconnected: {peerId}");

        if (Multiplayer.IsServer())
        {
            LobbyManager.Instance.ServerHandlePeerDisconnected((int)peerId);
        }
    }

    private void OnConnectedToServer()
    {
        EmitSignal(SignalName.ConnectionStatusChanged, "connected", "Connected to host");
        NetworkRpc.Instance.SendJoinRequest(_pendingPlayerName, _pendingRole, GetOrCreateReconnectToken());
        GameManager.Instance?.LoadLobby();
    }

    private void OnConnectionFailed()
    {
        EmitSignal(SignalName.ConnectionStatusChanged, "error", "Connection failed");
        DisconnectSession("Connection failed", notifyServer: false);
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnServerDisconnected()
    {
        EmitSignal(SignalName.ConnectionStatusChanged, "error", "Server disconnected");
        DisconnectSession("Server disconnected", notifyServer: false);
        GameManager.Instance?.LoadMainMenu();
    }

    private void OnServerMessage(string message)
    {
        EmitSignal(SignalName.NetworkMessage, message);
    }
}
