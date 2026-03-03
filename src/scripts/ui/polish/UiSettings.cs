using Godot;

namespace NetDex.UI.Polish;

public static class UiSettings
{
    private const string SettingsPath = "user://settings.cfg";

    private static bool _loaded;

    public static bool ReduceMotion { get; private set; }
    public static bool UiSfxEnabled { get; private set; } = true;
    public static UiEffectsIntensity EffectsIntensity { get; private set; } = UiEffectsIntensity.Max;
    public static DealAnimationStyle DealAnimationStyle { get; private set; } = DealAnimationStyle.CinematicFlip;

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
            DealAnimationStyle = DealAnimationStyle.CinematicFlip;
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

        var rawDealStyle = config.GetValue("ui", "deal_animation_style", (int)DealAnimationStyle.CinematicFlip).AsInt32();
        DealAnimationStyle = rawDealStyle switch
        {
            (int)DealAnimationStyle.Smooth => DealAnimationStyle.Smooth,
            _ => DealAnimationStyle.CinematicFlip
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

    public static void SetDealAnimationStyle(DealAnimationStyle style)
    {
        EnsureLoaded();
        DealAnimationStyle = style;
    }

    public static void Save()
    {
        EnsureLoaded();

        var config = new ConfigFile();
        config.Load(SettingsPath);

        config.SetValue("ui", "reduce_motion", ReduceMotion);
        config.SetValue("ui", "sfx_enabled", UiSfxEnabled);
        config.SetValue("ui", "effects_intensity", (int)EffectsIntensity);
        config.SetValue("ui", "deal_animation_style", (int)DealAnimationStyle);

        config.Save(SettingsPath);
    }
}
