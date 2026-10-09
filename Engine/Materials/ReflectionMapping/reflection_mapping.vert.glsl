#version 330 core

// input from the VAO (attribute locations, see Mesh.cs)
layout(location = 0) in vec3 localPosition;
layout(location = 1) in vec3 localNormal;

// the matrices are set by the renderer for each draw call
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

// "uv" is passed to the fragment shader, declared as "out" parameter
out vec2 uv;

void main()
{
    // modelview matrix: model first, then camera (OpenTK uses row vectors: vector * matrix)
    mat4 modelView = uModel * uView;

    // normal in view space; w = 0 ignores the translation
    vec3 viewNormal = normalize((vec4(localNormal, 0.0) * modelView).xyz);

    // x and y of the view space normal range from -1 to 1, the texture from 0 to 1
    uv = viewNormal.xy * 0.5 + 0.5;

    gl_Position = vec4(localPosition, 1.0) * modelView * uProjection;
}
