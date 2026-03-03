namespace NetDex.UI.Polish;

public static class UiMotionProfile
{
    public static double MicroDurationSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.06;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.08,
                UiEffectsIntensity.Normal => 0.11,
                _ => 0.14
            };
        }
    }

    public static double PanelEnterDurationSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.12;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.22,
                UiEffectsIntensity.Normal => 0.27,
                _ => 0.32
            };
        }
    }

    public static double ScreenTransitionDurationSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.14;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.28,
                UiEffectsIntensity.Normal => 0.35,
                _ => 0.42
            };
        }
    }

    public static float HoverScale
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 1.0f;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 1.01f,
                UiEffectsIntensity.Normal => 1.018f,
                _ => 1.03f
            };
        }
    }

    public static float PressScale
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.995f;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.98f,
                UiEffectsIntensity.Normal => 0.965f,
                _ => 0.95f
            };
        }
    }

    public static double DealIntervalSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.045;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.055,
                UiEffectsIntensity.Normal => 0.07,
                _ => 0.085
            };
        }
    }

    public static double DealTravelSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.14;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.2,
                UiEffectsIntensity.Normal => 0.26,
                _ => 0.34
            };
        }
    }

    public static double DealFlipSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.08;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.12,
                UiEffectsIntensity.Normal => 0.16,
                _ => 0.2
            };
        }
    }

    public static double DeskPlayTravelSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.14;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.18,
                UiEffectsIntensity.Normal => 0.22,
                _ => 0.28
            };
        }
    }

    public static double GameBannerHoldSeconds
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 1.2;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 1.45,
                UiEffectsIntensity.Normal => 1.85,
                _ => 2.3
            };
        }
    }

    public static float VignetteIdleAlpha
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.06f;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.07f,
                UiEffectsIntensity.Normal => 0.09f,
                _ => 0.12f
            };
        }
    }

    public static float VignetteActiveAlpha
    {
        get
        {
            UiSettings.EnsureLoaded();
            if (UiSettings.ReduceMotion)
            {
                return 0.1f;
            }

            return UiSettings.EffectsIntensity switch
            {
                UiEffectsIntensity.Low => 0.12f,
                UiEffectsIntensity.Normal => 0.16f,
                _ => 0.2f
            };
        }
    }
}
