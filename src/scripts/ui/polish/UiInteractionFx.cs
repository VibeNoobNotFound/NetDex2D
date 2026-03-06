using System.Collections.Generic;
using Godot;
using NetDex.Managers;

namespace NetDex.UI.Polish;

public partial class UiInteractionFx : Node
{
    private readonly HashSet<ulong> _boundControls = new();

    public override void _Ready()
    {
        UiSettings.EnsureLoaded();
        GetTree().NodeAdded += OnNodeAdded;

        BindRecursive(GetTree().Root);
    }

    public override void _ExitTree()
    {
        if (GetTree() != null)
        {
            GetTree().NodeAdded -= OnNodeAdded;
        }
    }

    private void OnNodeAdded(Node node)
    {
        BindRecursive(node);
    }

    private void BindRecursive(Node node)
    {
        if (node is Control control)
        {
            BindControl(control);
        }

        foreach (Node child in node.GetChildren())
        {
            BindRecursive(child);
        }
    }

    private void BindControl(Control control)
    {
        var key = control.GetInstanceId();
        if (_boundControls.Contains(key))
        {
            return;
        }

        _boundControls.Add(key);

        if (control is OptionButton optionButton)
        {
            BindOptionButton(optionButton);
            BindButton(optionButton);
            return;
        }

        if (control is BaseButton button)
        {
            BindButton(button);
            return;
        }

        if (control is LineEdit lineEdit)
        {
            BindLineEdit(lineEdit);
            return;
        }

        if (control is ItemList itemList)
        {
            BindItemList(itemList);
        }
    }

    private static void BindButton(BaseButton button)
    {
        button.PivotOffset = button.Size * 0.5f;
        button.Resized += () =>
        {
            if (!GodotObject.IsInstanceValid(button))
            {
                return;
            }

            button.PivotOffset = button.Size * 0.5f;
        };

        button.MouseEntered += () =>
        {
            AnimateScale(button, new Vector2(UiMotionProfile.HoverScale, UiMotionProfile.HoverScale), UiMotionProfile.MicroDurationSeconds);
        };

        button.MouseExited += () =>
        {
            AnimateScale(button, Vector2.One, UiMotionProfile.MicroDurationSeconds * 0.9);
        };

        button.FocusEntered += () =>
        {
            AnimateScale(button, new Vector2(UiMotionProfile.HoverScale, UiMotionProfile.HoverScale), UiMotionProfile.MicroDurationSeconds);
        };

        button.FocusExited += () =>
        {
            AnimateScale(button, Vector2.One, UiMotionProfile.MicroDurationSeconds * 0.9);
        };

        button.Pressed += () =>
        {
            var buttonText = button is Button textButton ? textButton.Text : button.Name.ToString();
            var cue = ResolveCueForButton(buttonText);
            AudioManager.Instance?.PlayUiCue(cue, 0.92f, 0.04f);

            var pressScale = UiMotionProfile.PressScale;
            var down = button.CreateTween();
            down.TweenProperty(button, "scale", new Vector2(pressScale, pressScale), (float)(UiMotionProfile.MicroDurationSeconds * 0.55))
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            down.Finished += () =>
            {
                if (!GodotObject.IsInstanceValid(button))
                {
                    return;
                }

                AnimateScale(button, Vector2.One, UiMotionProfile.MicroDurationSeconds * 0.8);
            };
        };
    }

    private static void BindOptionButton(OptionButton optionButton)
    {
        ApplyPopupReadability(optionButton);
        optionButton.GetPopup().AboutToPopup += () =>
        {
            if (!GodotObject.IsInstanceValid(optionButton))
            {
                return;
            }

            ApplyPopupReadability(optionButton);
        };
    }

    private static void ApplyPopupReadability(OptionButton optionButton)
    {
        var popup = optionButton.GetPopup();
        if (popup == null)
        {
            return;
        }

        var darkness = Mathf.Clamp(0.74f * UiSettings.FluentBlurDarkness, 0.55f, 0.95f);
        var borderAlpha = Mathf.Clamp(0.62f + 0.16f * UiSettings.FluentBlurReflection, 0.6f, 0.96f);

        var panel = new StyleBoxFlat
        {
            ContentMarginLeft = 10,
            ContentMarginTop = 10,
            ContentMarginRight = 10,
            ContentMarginBottom = 10,
            BgColor = new Color(0.03f, 0.03f, 0.03f, darkness),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(0.78f, 0.79f, 0.74f, borderAlpha),
            CornerRadiusTopLeft = 10,
            CornerRadiusTopRight = 10,
            CornerRadiusBottomRight = 10,
            CornerRadiusBottomLeft = 10
        };

        var hover = new StyleBoxFlat
        {
            ContentMarginLeft = 8,
            ContentMarginTop = 6,
            ContentMarginRight = 8,
            ContentMarginBottom = 6,
            BgColor = new Color(0.82f, 0.88f, 1f, Mathf.Clamp(0.34f + 0.12f * UiSettings.FluentBlurReflection, 0.3f, 0.64f)),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(0.93f, 0.97f, 1f, Mathf.Clamp(0.82f + 0.08f * UiSettings.FluentBlurReflection, 0.82f, 0.98f)),
            CornerRadiusTopLeft = 8,
            CornerRadiusTopRight = 8,
            CornerRadiusBottomRight = 8,
            CornerRadiusBottomLeft = 8
        };

        popup.AddThemeStyleboxOverride("panel", panel);
        popup.AddThemeStyleboxOverride("hover", hover);
        popup.AddThemeColorOverride("font_color", new Color(0.96f, 0.95f, 0.92f, 1f));
        popup.AddThemeColorOverride("font_hover_color", Colors.White);
        popup.AddThemeColorOverride("font_pressed_color", new Color(0.99f, 0.98f, 0.95f, 1f));
    }

    private static void BindLineEdit(LineEdit lineEdit)
    {
        lineEdit.PivotOffset = lineEdit.Size * 0.5f;
        lineEdit.Resized += () =>
        {
            if (!GodotObject.IsInstanceValid(lineEdit))
            {
                return;
            }

            lineEdit.PivotOffset = lineEdit.Size * 0.5f;
        };

        var idleColor = new Color(1f, 1f, 1f, 1f);
        var activeColor = new Color(1.05f, 1.05f, 1.08f, 1f);

        lineEdit.FocusEntered += () =>
        {
            AnimateModulate(lineEdit, activeColor, UiMotionProfile.MicroDurationSeconds);
        };

        lineEdit.FocusExited += () =>
        {
            AnimateModulate(lineEdit, idleColor, UiMotionProfile.MicroDurationSeconds);
        };
    }

    private static void BindItemList(ItemList itemList)
    {
        itemList.PivotOffset = itemList.Size * 0.5f;
        itemList.Resized += () =>
        {
            if (!GodotObject.IsInstanceValid(itemList))
            {
                return;
            }

            itemList.PivotOffset = itemList.Size * 0.5f;
        };

        itemList.ItemSelected += _ =>
        {
            AudioManager.Instance?.PlayUiCue(UiSfxCue.Click, 0.72f, 0.02f);
            var scaleUp = itemList.CreateTween();
            scaleUp.TweenProperty(itemList, "scale", new Vector2(1.01f, 1.01f), (float)(UiMotionProfile.MicroDurationSeconds * 0.5))
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);
            scaleUp.Finished += () =>
            {
                if (!GodotObject.IsInstanceValid(itemList))
                {
                    return;
                }

                AnimateScale(itemList, Vector2.One, UiMotionProfile.MicroDurationSeconds * 0.7);
            };
        };
    }

    private static UiSfxCue ResolveCueForButton(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return UiSfxCue.Click;
        }

        var normalized = text.ToLowerInvariant();
        if (normalized.Contains("back") || normalized.Contains("cancel") || normalized.Contains("leave") || normalized.Contains("exit"))
        {
            return UiSfxCue.Cancel;
        }

        if (normalized.Contains("start") || normalized.Contains("create") || normalized.Contains("join") || normalized.Contains("apply") || normalized.Contains("update") || normalized.Contains("confirm") || normalized.Contains("resume"))
        {
            return UiSfxCue.Confirm;
        }

        return UiSfxCue.Click;
    }

    private static void AnimateScale(Control control, Vector2 targetScale, double durationSeconds)
    {
        if (!GodotObject.IsInstanceValid(control))
        {
            return;
        }

        if (UiSettings.ReduceMotion)
        {
            control.Scale = Vector2.One;
            return;
        }

        var tween = control.CreateTween();
        tween.TweenProperty(control, "scale", targetScale, (float)durationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private static void AnimateModulate(Control control, Color target, double durationSeconds)
    {
        if (!GodotObject.IsInstanceValid(control))
        {
            return;
        }

        var tween = control.CreateTween();
        tween.TweenProperty(control, "modulate", target, (float)durationSeconds)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }
}
