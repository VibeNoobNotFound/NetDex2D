using Godot;

namespace NetDex.UI.Polish;

public static class UiSettings
{
    private const string SettingsPath = "user://settings.cfg";

    private static bool _loaded;

    public static bool ReduceMotion { get; private set; }
    public static bool UiSfxEnabled { get; private set; } = true;
    public static UiEffectsIntensity EffectsIntensity { get; private set; } = UiEffectsIntensity.Max;

    public static void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        Load();
    }

    public static void Load()
    {
        var config = new ConfigFile();
        if (config.Load(SettingsPath) != Error.Ok)
        {
            ReduceMotion = false;
            UiSfxEnabled = true;
            EffectsIntensity = UiEffectsIntensity.Max;
            _loaded = true;
            return;
        }

        ReduceMotion = (bool)config.GetValue("ui", "reduce_motion", false);
        UiSfxEnabled = (bool)config.GetValue("ui", "sfx_enabled", true);
        var rawIntensity = config.GetValue("ui", "effects_intensity", (int)UiEffectsIntensity.Max).AsInt32();
        EffectsIntensity = rawIntensity switch
        {
            (int)UiEffectsIntensity.Low => UiEffectsIntensity.Low,
            (int)UiEffectsIntensity.Normal => UiEffectsIntensity.Normal,
            _ => UiEffectsIntensity.Max
        };

        _loaded = true;
    }

    public static void SetReduceMotion(bool value)
    {
        EnsureLoaded();
        ReduceMotion = value;
    }

    public static void SetUiSfxEnabled(bool value)
    {
        EnsureLoaded();
        UiSfxEnabled = value;
    }

    public static void SetEffectsIntensity(UiEffectsIntensity intensity)
    {
        EnsureLoaded();
        EffectsIntensity = intensity;
    }

    public static void Save()
    {
        EnsureLoaded();

        var config = new ConfigFile();
        config.Load(SettingsPath);

        config.SetValue("ui", "reduce_motion", ReduceMotion);
        config.SetValue("ui", "sfx_enabled", UiSfxEnabled);
        config.SetValue("ui", "effects_intensity", (int)EffectsIntensity);

        config.Save(SettingsPath);
    }
}
