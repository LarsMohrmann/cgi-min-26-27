#version 330 core

in vec3 worldPosition;
in vec3 worldNormal;
out vec4 outColor;

uniform vec3 uCameraPosition;
uniform vec3 uLightPosition;
uniform vec3 uBaseColor;
uniform float uAmbientStrength;
uniform float uSpecularStrength;
uniform float uShininess;

void main()
{
    vec3 N = normalize(worldNormal);
    vec3 L = normalize(uLightPosition - worldPosition);
    vec3 V = normalize(uCameraPosition - worldPosition);
    vec3 R = reflect(-L, N);

    vec3 ambient = uAmbientStrength * uBaseColor;
    vec3 diffuse = max(dot(N, L), 0.0) * uBaseColor;
    vec3 specular = vec3(uSpecularStrength) * pow(max(dot(R, V), 0.0), uShininess)
                    * step(0.0, dot(N, L));

    // No gamma correction for now: the result is written out directly.
    outColor = vec4(ambient + diffuse + specular, 1.0);
}
