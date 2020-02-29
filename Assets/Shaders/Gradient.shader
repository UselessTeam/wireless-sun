shader_type canvas_item;
render_mode unshaded;

uniform sampler2D gradient: hint_black;
uniform float mix_value: 1.0;

void fragment() {
	vec4 base_color = texture(TEXTURE, UV);
	float greyscale = dot(base_color.rgb, vec3(0.3,0.4,0.3));
	vec3 shaded_color = texture(gradient, vec2(greyscale, 0.0)).rgb;
	COLOR.rgb = mix(base_color.rgb, shaded_color, mix_value);
	COLOR.a = base_color.a;
}