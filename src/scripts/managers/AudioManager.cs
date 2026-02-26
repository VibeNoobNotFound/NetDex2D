using Godot;

namespace NetDex.Managers;

public partial class AudioManager : Node
{
    private const string MusicBusName = "Music";
    private const string SfxBusName = "Sfx";
    private const string SettingsPath = "user://settings.cfg";
    private const string MusicAssetPath = "res://assets/sounds/background-music.mp3";
    private const float MinDb = -80f;

    public static AudioManager Instance { get; private set; } = null!;

    private AudioStreamPlayer _musicPlayer = null!;

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
        EnsureBuses();
        SetupMusicPlayer();
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
