#version 330 core

layout(location = 0) in vec3 localPosition;
layout(location = 1) in vec3 localNormal;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 worldPosition;
out vec3 worldNormal;

void main()
{
    // OpenTK verwendet Zeilenvektoren; die Matrizen werden entsprechend geladen.
    worldPosition = (vec4(localPosition, 1.0) * uModel).xyz;
    worldNormal = normalize(localNormal * transpose(inverse(mat3(uModel))));
    gl_Position = vec4(localPosition, 1.0) * uModel * uView * uProjection;
}
