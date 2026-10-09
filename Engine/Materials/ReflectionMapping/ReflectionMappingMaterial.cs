using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.ReflectionMapping;

// Simple reflection mapping without lighting: the texture coordinates are not taken from the mesh,
// but calculated from the normal in view space. The environment texture therefore appears to be
// reflected on the surface and stays fixed relative to the camera while the object rotates.
public sealed class ReflectionMappingMaterial : Material
{
    private static readonly ShaderSource Source =
        ShaderSource.FromEmbeddedResources(typeof(ReflectionMappingMaterial), "reflection_mapping.vert.glsl", "reflection_mapping.frag.glsl");

    private Texture _environmentTexture;

    public ReflectionMappingMaterial(Texture environmentTexture)
    {
        _environmentTexture = environmentTexture ?? throw new ArgumentNullException(nameof(environmentTexture));
    }

    public Texture EnvironmentTexture
    {
        get => _environmentTexture;
        set => _environmentTexture = value ?? throw new ArgumentNullException(nameof(value));
    }

    // Multiplied by the texture color; white leaves the texture unchanged.
    public Vector3 Tint { get; set; } = Vector3.One;

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", EnvironmentTexture, 0);
        shader.SetVector3("uTint", Tint);
    }
}
