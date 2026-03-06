using Godot;
using NetDex.Managers;

namespace NetDex.UI.Polish;

public partial class UiFeedbackService : Node
{
    public static UiFeedbackService Instance { get; private set; } = null!;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
        UiSettings.EnsureLoaded();
    }

    public void ShowToast(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.4)
    {
        UiOverlay.Instance?.ShowToast(message, severity, durationSeconds);
        PlaySeverityCue(severity);
    }

    public void ShowBanner(string message, UiSeverity severity = UiSeverity.Info, double durationSeconds = 2.2)
    {
        UiOverlay.Instance?.ShowBanner(message, severity, durationSeconds);
        AudioManager.Instance?.PlayUiCue(UiSfxCue.Banner, 0.9f, 0.02f);
    }

    public void SetLoading(bool enabled, string message = "Loading...")
    {
        UiOverlay.Instance?.SetLoading(enabled, message);
    }

    public void ApplyStatusLabelStyle(Label label, UiSeverity severity)
    {
        if (label == null)
        {
            return;
        }

        label.AddThemeColorOverride("font_color", severity switch
        {
            UiSeverity.Success => new Color(0.68f, 0.93f, 0.72f),
            UiSeverity.Warning => new Color(0.95f, 0.85f, 0.62f),
            UiSeverity.Error => new Color(0.96f, 0.67f, 0.68f),
            _ => new Color(0.9f, 0.92f, 0.98f)
        });
    }

    public static UiSeverity InferSeverity(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return UiSeverity.Info;
        }

        var normalized = message.ToLowerInvariant();
        if (normalized.Contains("failed") || normalized.Contains("error") || normalized.Contains("invalid") || normalized.Contains("unable"))
        {
            return UiSeverity.Error;
        }

        if (normalized.Contains("warning") || normalized.Contains("timeout") || normalized.Contains("blocked"))
        {
            return UiSeverity.Warning;
        }

        if (normalized.Contains("connected") || normalized.Contains("created") || normalized.Contains("updated") || normalized.Contains("success") || normalized.Contains("ready"))
        {
            return UiSeverity.Success;
        }

        return UiSeverity.Info;
    }

    private static void PlaySeverityCue(UiSeverity severity)
    {
        var cue = severity switch
        {
            UiSeverity.Success => UiSfxCue.Success,
            UiSeverity.Warning => UiSfxCue.Focus,
            UiSeverity.Error => UiSfxCue.Error,
            _ => UiSfxCue.Toast
        };

        AudioManager.Instance?.PlayUiCue(cue, 0.92f, 0.02f);
    }
}
