using System.Threading.Tasks;
using Godot;
using NetDex.Managers;

namespace NetDex.UI.Polish;

public partial class ScreenTransitionController : Node
{
    public static ScreenTransitionController Instance { get; private set; } = null!;

    private bool _isTransitioning;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }

    public async Task TransitionAsync(CanvasItem? current, CanvasItem target, ScreenTransitionStyle style = ScreenTransitionStyle.Auto)
    {
        if (_isTransitioning || target == null)
        {
            return;
        }

        _isTransitioning = true;
        var resolvedStyle = ResolveStyle(style);

        try
        {
            if (current == target)
            {
                target.Visible = true;
                return;
            }

            if (resolvedStyle == ScreenTransitionStyle.Instant || UiMotionProfile.ScreenTransitionDurationSeconds <= 0.02)
            {
                if (current != null)
                {
                    current.Visible = false;
                }

                target.Visible = true;
                ResetVisualState(target);
                return;
            }

            AudioManager.Instance?.PlayUiCue(UiSfxCue.ScreenTransition, 0.85f, 0.02f);

            var duration = (float)UiMotionProfile.ScreenTransitionDurationSeconds;

            PrepareEnterState(target, resolvedStyle);
            var tween = CreateTween();
            tween.SetParallel(true);

            if (current != null)
            {
                ConfigureExitTween(tween, current, resolvedStyle, duration);
            }

            ConfigureEnterTween(tween, target, resolvedStyle, duration);

            await ToSignal(tween, Tween.SignalName.Finished);

            if (current != null)
            {
                current.Visible = false;
                ResetVisualState(current);
            }

            target.Visible = true;
            ResetVisualState(target);
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    private static ScreenTransitionStyle ResolveStyle(ScreenTransitionStyle style)
    {
        UiSettings.EnsureLoaded();
        return style switch
        {
            ScreenTransitionStyle.Auto => UiSettings.ReduceMotion ? ScreenTransitionStyle.Fade : ScreenTransitionStyle.ZoomFade,
            _ => style
        };
    }

    private static void PrepareEnterState(CanvasItem target, ScreenTransitionStyle style)
    {
        target.Visible = true;
        target.Modulate = new Color(1f, 1f, 1f, 0f);

        if (target is not Control control)
        {
            return;
        }

        control.Position = Vector2.Zero;
        control.Scale = Vector2.One;

        if (style == ScreenTransitionStyle.SlideLeft)
        {
            control.Position = new Vector2(44f, 0f);
        }
        else if (style == ScreenTransitionStyle.ZoomFade)
        {
            control.Scale = new Vector2(0.97f, 0.97f);
        }
    }

    private static void ConfigureExitTween(Tween tween, CanvasItem current, ScreenTransitionStyle style, float duration)
    {
        tween.TweenProperty(current, "modulate:a", 0f, duration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.InOut);

        if (current is not Control control)
        {
            return;
        }

        if (style == ScreenTransitionStyle.SlideLeft)
        {
            tween.TweenProperty(control, "position:x", -44f, duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.InOut);
        }
        else if (style == ScreenTransitionStyle.ZoomFade)
        {
            tween.TweenProperty(control, "scale", new Vector2(1.03f, 1.03f), duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.InOut);
        }
    }

    private static void ConfigureEnterTween(Tween tween, CanvasItem target, ScreenTransitionStyle style, float duration)
    {
        tween.TweenProperty(target, "modulate:a", 1f, duration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        if (target is not Control control)
        {
            return;
        }

        if (style == ScreenTransitionStyle.SlideLeft)
        {
            tween.TweenProperty(control, "position", Vector2.Zero, duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
        }
        else if (style == ScreenTransitionStyle.ZoomFade)
        {
            tween.TweenProperty(control, "scale", Vector2.One, duration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
        }
    }

    private static void ResetVisualState(CanvasItem item)
    {
        if (item == null)
        {
            return;
        }

        item.Modulate = Colors.White;
        if (item is Control control)
        {
            control.Scale = Vector2.One;
            control.Position = Vector2.Zero;
        }
    }
}
