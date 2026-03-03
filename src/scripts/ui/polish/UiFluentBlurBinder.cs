using System;
using System.Collections.Generic;
using Godot;

namespace NetDex.UI.Polish;

public partial class UiFluentBlurBinder : Node
{
    private const string BackdropNodeName = "__FluentBackdrop";
    private const string ShaderPath = "res://assets/shaders/ui/fluent_panel_blur.gdshader";
    private const float DefaultCornerRadius = 24.0f;

    private readonly HashSet<ulong> _boundPanels = new();
    private readonly Dictionary<ulong, ShaderMaterial> _panelMaterials = new();

    private Shader _fluentShader = null!;

    public override void _Ready()
    {
        UiSettings.EnsureLoaded();
        _fluentShader = GD.Load<Shader>(ShaderPath);
        if (_fluentShader == null)
        {
            GD.PushWarning($"Fluent blur shader missing at {ShaderPath}. Blur binder disabled.");
            return;
        }

        GetTree().NodeAdded += OnNodeAdded;
        UiSettings.SettingsChanged += OnSettingsChanged;

        BindRecursive(GetTree().Root);
        ApplySettingsToBoundPanels();
    }

    public override void _ExitTree()
    {
        if (GetTree() != null)
        {
            GetTree().NodeAdded -= OnNodeAdded;
        }

        UiSettings.SettingsChanged -= OnSettingsChanged;
        _panelMaterials.Clear();
        _boundPanels.Clear();
    }

    private void OnNodeAdded(Node node)
    {
        BindRecursive(node);
    }

    private void OnSettingsChanged()
    {
        ApplySettingsToBoundPanels();
    }

    private void BindRecursive(Node node)
    {
        if (node is PanelContainer panel)
        {
            BindPanel(panel);
        }

        foreach (Node child in node.GetChildren())
        {
            BindRecursive(child);
        }
    }

    private void BindPanel(PanelContainer panel)
    {
        if (!GodotObject.IsInstanceValid(panel) || IsExempt(panel))
        {
            return;
        }

        var panelId = panel.GetInstanceId();
        if (_boundPanels.Contains(panelId))
        {
            RefreshPanelMaterial(panel);
            return;
        }

        _boundPanels.Add(panelId);

        var backdrop = EnsureBackdrop(panel);
        var material = new ShaderMaterial { Shader = _fluentShader };
        backdrop.Material = material;
        _panelMaterials[panelId] = material;

        panel.Resized += () =>
        {
            if (GodotObject.IsInstanceValid(panel))
            {
                RefreshPanelMaterial(panel);
            }
        };

        panel.TreeExiting += () =>
        {
            _boundPanels.Remove(panelId);
            _panelMaterials.Remove(panelId);
        };

        RefreshPanelMaterial(panel);
    }

    private static bool IsExempt(PanelContainer panel)
    {
        if (panel.IsInGroup("fluent_blur_exempt"))
        {
            return true;
        }

        if (!panel.HasMeta("fluent_blur_exempt"))
        {
            return false;
        }

        var meta = panel.GetMeta("fluent_blur_exempt");
        return meta.VariantType == Variant.Type.Bool && meta.AsBool();
    }

    private static Polygon2D EnsureBackdrop(PanelContainer panel)
    {
        var backdrop = panel.GetNodeOrNull<Polygon2D>(BackdropNodeName);
        if (backdrop == null)
        {
            backdrop = new Polygon2D
            {
                Name = BackdropNodeName,
                Color = Colors.White
            };

            panel.AddChild(backdrop);
        }

        panel.MoveChild(backdrop, 0);
        return backdrop;
    }

    private void ApplySettingsToBoundPanels()
    {
        foreach (var panelId in _boundPanels)
        {
            var instance = GodotObject.InstanceFromId(panelId) as PanelContainer;
            if (!GodotObject.IsInstanceValid(instance))
            {
                continue;
            }

            RefreshPanelMaterial(instance);
        }
    }

    private void RefreshPanelMaterial(PanelContainer panel)
    {
        if (!GodotObject.IsInstanceValid(panel))
        {
            return;
        }

        var panelId = panel.GetInstanceId();
        if (!_panelMaterials.TryGetValue(panelId, out var material))
        {
            return;
        }

        var backdrop = panel.GetNodeOrNull<Polygon2D>(BackdropNodeName);
        if (backdrop == null)
        {
            return;
        }

        var panelSize = panel.Size;
        if (panelSize.X < 1f || panelSize.Y < 1f)
        {
            panelSize = panel.GetCombinedMinimumSize();
        }

        SyncBackdropGeometry(backdrop, panelSize);

        var enabled = UiSettings.FluentBlurEnabled;
        var (baseLod, baseRadiusPx, baseAmount, tintColor) = ResolveQualityVisuals(UiSettings.FluentBlurQuality);
        var strength = Mathf.Clamp(UiSettings.FluentBlurStrength, 0.5f, 5.0f);
        var darkness = Mathf.Clamp(UiSettings.FluentBlurDarkness, 0.5f, 3.0f);
        var reflection = Mathf.Clamp(UiSettings.FluentBlurReflection, 0.0f, 3.0f);
        var blurLod = Mathf.Clamp(baseLod * strength, 0.0f, 28.0f);
        var blurRadiusPx = Mathf.Clamp(baseRadiusPx * strength, 0.0f, 64.0f);
        var blurAmount = Mathf.Clamp(baseAmount + ((strength - 1.0f) * 0.18f), 0.0f, 1.0f);
        var cornerRadius = ResolvePanelCornerRadius(panel);
        var alphaScale = Mathf.Lerp(0.8f, 1.75f, Mathf.Clamp((strength - 0.5f) / 4.5f, 0f, 1f));
        var darknessScale = Mathf.Lerp(0.82f, 1.38f, Mathf.Clamp((darkness - 0.5f) / 2.5f, 0f, 1f));
        var effectiveTint = new Color(tintColor.R, tintColor.G, tintColor.B, Mathf.Clamp(tintColor.A * alphaScale * darknessScale, 0.08f, 0.93f));
        var baseGloss = Mathf.Clamp(0.95f + (darkness - 1.0f) * 0.25f, 0.65f, 1.6f);
        var glossStrength = Mathf.Clamp(baseGloss * Mathf.Lerp(0.35f, 2.2f, reflection / 3.0f), 0.0f, 3.0f);

        material.SetShaderParameter("enabled", enabled);
        material.SetShaderParameter("blur_lod", blurLod);
        material.SetShaderParameter("blur_radius_px", blurRadiusPx);
        material.SetShaderParameter("blur_amount", blurAmount);
        material.SetShaderParameter("darkness", darkness);
        material.SetShaderParameter("gloss_strength", glossStrength);
        material.SetShaderParameter("tint_color", effectiveTint);
        material.SetShaderParameter("panel_size_px", panelSize);
        material.SetShaderParameter("corner_radius_px", cornerRadius);
        backdrop.Visible = enabled;
    }

    private static void SyncBackdropGeometry(Polygon2D backdrop, Vector2 panelSize)
    {
        var width = Mathf.Max(1f, panelSize.X);
        var height = Mathf.Max(1f, panelSize.Y);
        backdrop.Position = Vector2.Zero;
        backdrop.Polygon = new[]
        {
            Vector2.Zero,
            new Vector2(width, 0f),
            new Vector2(width, height),
            new Vector2(0f, height)
        };
    }

    private static (float blurLod, float blurRadiusPx, float blurAmount, Color tintColor) ResolveQualityVisuals(UiBlurQuality quality)
    {
        return quality switch
        {
            UiBlurQuality.Low => (4.0f, 4.0f, 0.82f, new Color(0.07f, 0.08f, 0.10f, 0.26f)),
            UiBlurQuality.Normal => (7.0f, 7.0f, 0.93f, new Color(0.07f, 0.08f, 0.10f, 0.34f)),
            _ => (10.0f, 10.0f, 1.0f, new Color(0.07f, 0.08f, 0.10f, 0.46f))
        };
    }

    private static float ResolvePanelCornerRadius(PanelContainer panel)
    {
        var styleBox = panel.GetThemeStylebox("panel");
        if (styleBox is not StyleBoxFlat styleFlat)
        {
            return DefaultCornerRadius;
        }

        var top = Mathf.Max(styleFlat.CornerRadiusTopLeft, styleFlat.CornerRadiusTopRight);
        var bottom = Mathf.Max(styleFlat.CornerRadiusBottomLeft, styleFlat.CornerRadiusBottomRight);
        return Mathf.Max(top, bottom);
    }
}
