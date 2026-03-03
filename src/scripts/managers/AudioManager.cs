using System;
using System.Collections.Generic;
using Godot;
using NetDex.UI.Polish;

namespace NetDex.Managers;

public partial class AudioManager : Node
{
    private const string MusicBusName = "Music";
    private const string SfxBusName = "Sfx";
    private const string SettingsPath = "user://settings.cfg";
    private const string MusicAssetPath = "res://assets/sounds/background-music.mp3";
    private const float MinDb = -80f;
    private const int UiSfxPoolSize = 10;

    public static AudioManager Instance { get; private set; } = null!;

    private AudioStreamPlayer _musicPlayer = null!;
    private readonly List<AudioStreamPlayer> _uiSfxPool = new();
    private readonly Dictionary<UiSfxCue, AudioStream[]> _uiCueStreams = new();
    private readonly Dictionary<UiSfxCue, double> _lastCuePlayTimes = new();
    private readonly Dictionary<UiSfxCue, double> _cueCooldownSeconds = new()
    {
        [UiSfxCue.Hover] = 0.05,
        [UiSfxCue.Focus] = 0.08,
        [UiSfxCue.Click] = 0.05,
        [UiSfxCue.Confirm] = 0.04,
        [UiSfxCue.Cancel] = 0.08,
        [UiSfxCue.Error] = 0.2,
        [UiSfxCue.Success] = 0.12,
        [UiSfxCue.Toast] = 0.1,
        [UiSfxCue.Banner] = 0.1,
        [UiSfxCue.ScreenTransition] = 0.12,
        [UiSfxCue.MatchPhase] = 0.18,
        [UiSfxCue.TrickWin] = 0.18,
        [UiSfxCue.RoundResolved] = 0.25,
        [UiSfxCue.KapothiDecision] = 0.25,
        [UiSfxCue.LobbyJoin] = 0.12,
        [UiSfxCue.LobbyLeave] = 0.12
    };

    public double MusicVolumePercent { get; private set; } = 80;
    public double SfxVolumePercent { get; private set; } = 80;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
        UiSettings.EnsureLoaded();

        EnsureBuses();
        SetupMusicPlayer();
        SetupUiSfxPool();
        SetupUiCueMap();

        LoadAudioSettings();
        StartBackgroundMusic();
    }

    public void SetMusicVolumePercent(double value)
    {
        MusicVolumePercent = Mathf.Clamp((float)value, 0f, 100f);
        var busIndex = AudioServer.GetBusIndex(MusicBusName);
        if (busIndex >= 0)
        {
            AudioServer.SetBusVolumeDb(busIndex, PercentToDb(MusicVolumePercent));
        }
    }

    public void SetSfxVolumePercent(double value)
    {
        SfxVolumePercent = Mathf.Clamp((float)value, 0f, 100f);
        var busIndex = AudioServer.GetBusIndex(SfxBusName);
        if (busIndex >= 0)
        {
            AudioServer.SetBusVolumeDb(busIndex, PercentToDb(SfxVolumePercent));
        }
    }

    public void PlayUiCue(UiSfxCue cue)
    {
        PlayUiCue(cue, 1f, 0.03f);
    }

    public void PlayUiCue(UiSfxCue cue, float volumeScale, float pitchJitter)
    {
        UiSettings.EnsureLoaded();
        if (!UiSettings.UiSfxEnabled)
        {
            return;
        }

        if (!_uiCueStreams.TryGetValue(cue, out var streams) || streams.Length == 0)
        {
            return;
        }

        var now = Time.GetUnixTimeFromSystem();
        if (_lastCuePlayTimes.TryGetValue(cue, out var lastPlayed))
        {
            var cooldown = _cueCooldownSeconds.TryGetValue(cue, out var value) ? value : 0.05;
            if (now - lastPlayed < cooldown)
            {
                return;
            }
        }

        var player = AcquireUiPlayer();
        var selected = streams[GD.RandRange(0, streams.Length - 1)];
        if (selected == null)
        {
            return;
        }

        player.Stop();
        player.Stream = selected;

        var clampedVolume = Mathf.Clamp(volumeScale, 0.05f, 2.0f);
        player.VolumeDb = Mathf.LinearToDb(clampedVolume);

        var jitter = Mathf.Clamp(pitchJitter, 0f, 0.25f);
        player.PitchScale = 1f + (jitter <= 0f ? 0f : (float)GD.RandRange(-jitter, jitter));

        player.Play();
        _lastCuePlayTimes[cue] = now;
    }

    public static string GetSfxBusName() => SfxBusName;
    public static string GetMusicBusName() => MusicBusName;

    private static float PercentToDb(double percent)
    {
        if (percent <= 0.01)
        {
            return MinDb;
        }

        return Mathf.LinearToDb((float)(percent / 100.0));
    }

    private static void EnsureBuses()
    {
        EnsureBus(MusicBusName);
        EnsureBus(SfxBusName);
    }

    private static void EnsureBus(string busName)
    {
        var index = AudioServer.GetBusIndex(busName);
        if (index < 0)
        {
            AudioServer.AddBus(AudioServer.BusCount);
            index = AudioServer.BusCount - 1;
            AudioServer.SetBusName(index, busName);
        }

        AudioServer.SetBusSend(index, "Master");
    }

    private void SetupMusicPlayer()
    {
        var stream = GD.Load<AudioStream>(MusicAssetPath);
        if (stream is AudioStreamMP3 mp3Stream)
        {
            mp3Stream.Loop = true;
        }

        _musicPlayer = new AudioStreamPlayer
        {
            Bus = MusicBusName,
            Stream = stream,
            Autoplay = false
        };
        AddChild(_musicPlayer);
    }

    private void SetupUiSfxPool()
    {
        for (var i = 0; i < UiSfxPoolSize; i++)
        {
            var player = new AudioStreamPlayer
            {
                Name = $"UiSfxPlayer{i}",
                Bus = SfxBusName,
                Autoplay = false
            };
            _uiSfxPool.Add(player);
            AddChild(player);
        }
    }

    private void SetupUiCueMap()
    {
        var place1 = GD.Load<AudioStream>("res://assets/sounds/cardPlace1.ogg");
        var place2 = GD.Load<AudioStream>("res://assets/sounds/cardPlace2.ogg");
        var place3 = GD.Load<AudioStream>("res://assets/sounds/cardPlace3.ogg");
        var slide1 = GD.Load<AudioStream>("res://assets/sounds/cardSlide1.ogg");
        var slide2 = GD.Load<AudioStream>("res://assets/sounds/cardSlide2.ogg");
        var slide3 = GD.Load<AudioStream>("res://assets/sounds/cardSlide3.ogg");

        _uiCueStreams[UiSfxCue.Hover] = new[] { slide1, slide2 };
        _uiCueStreams[UiSfxCue.Click] = new[] { place1 };
        _uiCueStreams[UiSfxCue.Confirm] = new[] { place2, place3 };
        _uiCueStreams[UiSfxCue.Cancel] = new[] { slide3 };
        _uiCueStreams[UiSfxCue.Error] = new[] { slide1 };
        _uiCueStreams[UiSfxCue.Success] = new[] { place3 };
        _uiCueStreams[UiSfxCue.Focus] = new[] { slide2 };
        _uiCueStreams[UiSfxCue.ScreenTransition] = new[] { slide2, slide3 };
        _uiCueStreams[UiSfxCue.Banner] = new[] { slide2 };
        _uiCueStreams[UiSfxCue.Toast] = new[] { place1 };
        _uiCueStreams[UiSfxCue.LobbyJoin] = new[] { place2 };
        _uiCueStreams[UiSfxCue.LobbyLeave] = new[] { slide3 };
        _uiCueStreams[UiSfxCue.MatchPhase] = new[] { slide2 };
        _uiCueStreams[UiSfxCue.TrickWin] = new[] { place3 };
        _uiCueStreams[UiSfxCue.RoundResolved] = new[] { place2, place3 };
        _uiCueStreams[UiSfxCue.KapothiDecision] = new[] { slide1, place3 };
    }

    private AudioStreamPlayer AcquireUiPlayer()
    {
        foreach (var player in _uiSfxPool)
        {
            if (!player.Playing)
            {
                return player;
            }
        }

        return _uiSfxPool.Count > 0 ? _uiSfxPool[0] : null!;
    }

    private void StartBackgroundMusic()
    {
        if (_musicPlayer.Stream == null || _musicPlayer.Playing)
        {
            return;
        }

        _musicPlayer.Play();
    }

    private void LoadAudioSettings()
    {
        var config = new ConfigFile();
        if (config.Load(SettingsPath) != Error.Ok)
        {
            SetMusicVolumePercent(MusicVolumePercent);
            SetSfxVolumePercent(SfxVolumePercent);
            return;
        }

        var music = (double)config.GetValue("audio", "music_volume", MusicVolumePercent);
        var sfx = (double)config.GetValue("audio", "sfx_volume", SfxVolumePercent);
        SetMusicVolumePercent(music);
        SetSfxVolumePercent(sfx);
    }
}
