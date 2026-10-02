#version 330 core

in vec2 uv;
out vec4 outColor;

uniform sampler2D uTexture;
uniform vec3 uTint;

void main()
{
    // Vorerst ohne Farbraumumrechnung: der Texturwert wird direkt ausgegeben.
    outColor = vec4(texture(uTexture, uv).rgb * uTint, 1.0);
}
