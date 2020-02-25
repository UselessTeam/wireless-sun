
shader_type canvas_item;
uniform float thickness = 2.0;
uniform vec4 color: hint_color;

void fragment(){
    vec4 col = texture(TEXTURE, UV);
    if(col.a == 1.0) {
        COLOR = col;
    } else {
        bool outlined = false;

        for(float y = -thickness; y <= thickness; y++) {
            for(float x = -thickness; x <= thickness; x++) {
                if(vec2(x,y) == vec2(0.0)) {
                    continue;
                }
                if(texture(TEXTURE, UV + TEXTURE_PIXEL_SIZE * vec2(x,y)).a > 0.0) {
                    outlined = true;
                    break;
                }
            }
        }
        if(outlined) {
            COLOR = color;
        } else {
            COLOR = col;
        }
    }
}