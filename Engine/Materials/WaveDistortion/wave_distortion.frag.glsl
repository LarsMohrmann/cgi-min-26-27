#version 330 core

in vec2 uv;
out vec4 outColor;

uniform sampler2D uTexture;
uniform vec3 uTint;

void main()
{
    // As in the UnlitTexture material: the texture value is written out directly.
    outColor = vec4(texture(uTexture, uv).rgb * uTint, 1.0);
}
