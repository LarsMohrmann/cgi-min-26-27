using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.Phong;

// Ambient, diffuse and specular lighting according to Phong with one point light.
public sealed class PhongMaterial : Material
{
    private static readonly ShaderSource Source =
        ShaderSource.FromEmbeddedResources(typeof(PhongMaterial), "phong.vert.glsl", "phong.frag.glsl");

    public Vector3 BaseColor { get; set; } = new(0.18f, 0.55f, 0.95f);
    public float AmbientStrength { get; set; } = 0.14f;
    public float SpecularStrength { get; set; } = 0.75f;
    public float Shininess { get; set; } = 48.0f;

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetVector3("uBaseColor", BaseColor);
        shader.SetFloat("uAmbientStrength", AmbientStrength);
        shader.SetFloat("uSpecularStrength", SpecularStrength);
        shader.SetFloat("uShininess", Shininess);
    }
}
