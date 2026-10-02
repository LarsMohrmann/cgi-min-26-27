#version 330 core

// input from the VAO (attribute locations, see Mesh.cs)
layout(location = 0) in vec3 localPosition;
layout(location = 2) in vec2 texCoord;

// the matrices are set by the renderer for each draw call
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

// custom parameter to steer the sine wave animation
uniform float uWobble;

// "uv" is passed to the fragment shader, declared as "out" parameter
out vec2 uv;

void main()
{
    // the texture coordinate is passed directly to the fragment shader
    uv = texCoord;

    // manipulating the z-coordinate of the vertex position
    vec4 v = vec4(localPosition, 1.0);
    v.z = v.z + sin(5.0 * v.x + uWobble) * 0.5;

    gl_Position = v * uModel * uView * uProjection;
}
