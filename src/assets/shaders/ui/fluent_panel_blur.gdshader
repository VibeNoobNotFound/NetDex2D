shader_type canvas_item;

uniform sampler2D screen_tex : hint_screen_texture, repeat_disable, filter_linear_mipmap;
uniform float blur_lod : hint_range(0.0, 28.0, 0.05) = 6.0;
uniform float blur_radius_px : hint_range(0.0, 64.0, 0.25) = 8.0;
uniform float blur_amount : hint_range(0.0, 1.0, 0.01) = 0.9;
uniform vec4 tint_color : source_color = vec4(0.08, 0.09, 0.11, 0.34);
uniform float corner_radius_px : hint_range(0.0, 128.0, 0.5) = 24.0;
uniform float darkness : hint_range(0.5, 3.0, 0.01) = 1.25;
uniform float gloss_strength : hint_range(0.0, 2.0, 0.01) = 1.0;
uniform vec2 panel_size_px = vec2(320.0, 180.0);
uniform bool enabled = true;

varying vec2 local_pos_px;

void vertex() {
    local_pos_px = VERTEX;
}

float rounded_rect_mask(vec2 uv, vec2 size, float radius_px) {
    vec2 half_size = size * 0.5;
    float radius = clamp(radius_px, 0.0, min(half_size.x, half_size.y));
    vec2 p = uv * size - half_size;
    vec2 q = abs(p) - (half_size - vec2(radius));
    float dist = length(max(q, vec2(0.0))) + min(max(q.x, q.y), 0.0) - radius;
    return 1.0 - smoothstep(0.0, 1.5, dist);
}

vec3 sample_box_blur(vec2 screen_uv, vec2 pixel_step) {
    vec3 sum = texture(screen_tex, screen_uv).rgb * 0.20;
    sum += texture(screen_tex, screen_uv + vec2(pixel_step.x, 0.0)).rgb * 0.14;
    sum += texture(screen_tex, screen_uv - vec2(pixel_step.x, 0.0)).rgb * 0.14;
    sum += texture(screen_tex, screen_uv + vec2(0.0, pixel_step.y)).rgb * 0.14;
    sum += texture(screen_tex, screen_uv - vec2(0.0, pixel_step.y)).rgb * 0.14;
    sum += texture(screen_tex, screen_uv + pixel_step).rgb * 0.06;
    sum += texture(screen_tex, screen_uv + vec2(-pixel_step.x, pixel_step.y)).rgb * 0.06;
    sum += texture(screen_tex, screen_uv + vec2(pixel_step.x, -pixel_step.y)).rgb * 0.06;
    sum += texture(screen_tex, screen_uv - pixel_step).rgb * 0.06;
    return sum;
}

void fragment() {
    vec4 out_color = vec4(0.0);
    if (enabled) {
        vec2 safe_size = max(panel_size_px, vec2(1.0));
        vec2 local_uv = clamp(local_pos_px / safe_size, vec2(0.0), vec2(1.0));
        float mask = rounded_rect_mask(local_uv, safe_size, corner_radius_px);

        vec2 blur_step = SCREEN_PIXEL_SIZE * max(0.0, blur_radius_px);
        vec3 sharp_rgb = textureLod(screen_tex, SCREEN_UV, 0.0).rgb;
        vec3 tap_blur_rgb = sample_box_blur(SCREEN_UV, blur_step);
        vec3 mip_blur_rgb = textureLod(screen_tex, SCREEN_UV, blur_lod).rgb;
        vec3 blurred_rgb = mix(tap_blur_rgb, mip_blur_rgb, 0.35);
        vec3 glass_rgb = mix(sharp_rgb, blurred_rgb, blur_amount);

        // Darken and tint for heavier frosted glass.
        float darkness_t = clamp((darkness - 0.5) / 2.5, 0.0, 1.0);
        vec3 base_rgb = mix(glass_rgb * 1.04, glass_rgb * 0.56, darkness_t * 0.72);
        float tint_mix = clamp(0.46 + darkness_t * 0.26, 0.32, 0.78);
        vec3 final_rgb = mix(base_rgb, tint_color.rgb, tint_mix);

        // Reflective glass border + top sheen.
        float edge_dist = min(min(local_uv.x, 1.0 - local_uv.x), min(local_uv.y, 1.0 - local_uv.y));
        float border_edge = 1.0 - smoothstep(0.0, 0.06, edge_dist);
        float top_sheen = pow(clamp(1.0 - local_uv.y, 0.0, 1.0), 2.6) * (0.82 + (1.0 - local_uv.x) * 0.18);
        vec3 gloss_color = vec3(0.95, 0.98, 1.0);
        final_rgb += gloss_color * ((border_edge * 0.10) + (top_sheen * 0.08)) * gloss_strength;
        final_rgb = clamp(final_rgb, vec3(0.0), vec3(1.0));
        float final_a = tint_color.a * mask;
        out_color = vec4(final_rgb, final_a);
    }

    COLOR = out_color;
}
