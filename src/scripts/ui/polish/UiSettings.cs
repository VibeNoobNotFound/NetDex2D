using System;
using Godot;

namespace NetDex.UI.Polish;

public static class UiSettings
{
    private const string SettingsPath = "user://settings.cfg";

    private static bool _loaded;

    public static event Action? SettingsChanged;

    public static bool ReduceMotion { get; private set; }
    public static bool UiSfxEnabled { get; private set; } = true;
    public static UiEffectsIntensity EffectsIntensity { get; private set; } = UiEffectsIntensity.Max;
    public static bool FluentBlurEnabled { get; private set; } = true;
    public static UiBlurQuality FluentBlurQuality { get; private set; } = UiBlurQuality.High;
    public static float FluentBlurStrength { get; private set; } = 1.35f;
    public static float FluentBlurDarkness { get; private set; } = 1.25f;
    public static float FluentBlurReflection { get; private set; } = 1.0f;

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
            FluentBlurEnabled = true;
            FluentBlurQuality = UiBlurQuality.High;
            FluentBlurStrength = 1.35f;
            FluentBlurDarkness = 1.25f;
            FluentBlurReflection = 1.0f;
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

        FluentBlurEnabled = (bool)config.GetValue("ui", "fluent_blur_enabled", true);
        var rawBlurQuality = config.GetValue("ui", "fluent_blur_quality", (int)UiBlurQuality.High).AsInt32();
        FluentBlurQuality = rawBlurQuality switch
        {
            (int)UiBlurQuality.Low => UiBlurQuality.Low,
            (int)UiBlurQuality.Normal => UiBlurQuality.Normal,
            _ => UiBlurQuality.High
        };
        FluentBlurStrength = Mathf.Clamp((float)config.GetValue("ui", "fluent_blur_strength", 1.35f).AsDouble(), 0.5f, 5.0f);
        FluentBlurDarkness = Mathf.Clamp((float)config.GetValue("ui", "fluent_blur_darkness", 1.25f).AsDouble(), 0.5f, 3.0f);
        FluentBlurReflection = Mathf.Clamp((float)config.GetValue("ui", "fluent_blur_reflection", 1.0f).AsDouble(), 0.0f, 3.0f);

        _loaded = true;
    }

    public static void SetReduceMotion(bool value)
    {
        EnsureLoaded();
        if (ReduceMotion == value)
        {
            return;
        }

        ReduceMotion = value;
        SettingsChanged?.Invoke();
    }

    public static void SetUiSfxEnabled(bool value)
    {
        EnsureLoaded();
        if (UiSfxEnabled == value)
        {
            return;
        }

        UiSfxEnabled = value;
        SettingsChanged?.Invoke();
    }

    public static void SetEffectsIntensity(UiEffectsIntensity intensity)
    {
        EnsureLoaded();
        if (EffectsIntensity == intensity)
        {
            return;
        }

        EffectsIntensity = intensity;
        SettingsChanged?.Invoke();
    }

    public static void SetFluentBlurEnabled(bool enabled)
    {
        EnsureLoaded();
        if (FluentBlurEnabled == enabled)
        {
            return;
        }

        FluentBlurEnabled = enabled;
        SettingsChanged?.Invoke();
    }

    public static void SetFluentBlurQuality(UiBlurQuality quality)
    {
        EnsureLoaded();
        if (FluentBlurQuality == quality)
        {
            return;
        }

        FluentBlurQuality = quality;
        SettingsChanged?.Invoke();
    }

    public static void SetFluentBlurStrength(float strength)
    {
        EnsureLoaded();
        var clamped = Mathf.Clamp(strength, 0.5f, 5.0f);
        if (Mathf.IsEqualApprox(FluentBlurStrength, clamped))
        {
            return;
        }

        FluentBlurStrength = clamped;
        SettingsChanged?.Invoke();
    }

    public static void SetFluentBlurDarkness(float darkness)
    {
        EnsureLoaded();
        var clamped = Mathf.Clamp(darkness, 0.5f, 3.0f);
        if (Mathf.IsEqualApprox(FluentBlurDarkness, clamped))
        {
            return;
        }

        FluentBlurDarkness = clamped;
        SettingsChanged?.Invoke();
    }

    public static void SetFluentBlurReflection(float reflection)
    {
        EnsureLoaded();
        var clamped = Mathf.Clamp(reflection, 0.0f, 3.0f);
        if (Mathf.IsEqualApprox(FluentBlurReflection, clamped))
        {
            return;
        }

        FluentBlurReflection = clamped;
        SettingsChanged?.Invoke();
    }

    public static void Save()
    {
        EnsureLoaded();

        var config = new ConfigFile();
        config.Load(SettingsPath);

        config.SetValue("ui", "reduce_motion", ReduceMotion);
        config.SetValue("ui", "sfx_enabled", UiSfxEnabled);
        config.SetValue("ui", "effects_intensity", (int)EffectsIntensity);
        config.SetValue("ui", "fluent_blur_enabled", FluentBlurEnabled);
        config.SetValue("ui", "fluent_blur_quality", (int)FluentBlurQuality);
        config.SetValue("ui", "fluent_blur_strength", FluentBlurStrength);
        config.SetValue("ui", "fluent_blur_darkness", FluentBlurDarkness);
        config.SetValue("ui", "fluent_blur_reflection", FluentBlurReflection);

        config.Save(SettingsPath);
    }
}
