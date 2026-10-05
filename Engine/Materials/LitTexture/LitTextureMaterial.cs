using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.LitTexture;

// Displays a texture with lighting, optionally multiplied by a color.
public sealed class LitTextureMaterial : Material
{
    private static readonly ShaderSource Source =
        ShaderSource.FromEmbeddedResources(typeof(LitTextureMaterial), "lit_texture.vert.glsl", "lit_texture.frag.glsl");

    private Texture _texture;

    public LitTextureMaterial(Texture texture)
    {
        _texture = texture ?? throw new ArgumentNullException(nameof(texture));
    }

    public Texture Texture
    {
        get => _texture;
        set => _texture = value ?? throw new ArgumentNullException(nameof(value));
    }

    // Multiplied by the texture color; white leaves the texture unchanged.
    public Vector3 Tint { get; set; } = Vector3.One;

    public float AmbientStrength { get; set; } = 0.14f;
    public float SpecularStrength { get; set; } = 0.75f;
    public float Shininess { get; set; } = 48.0f;

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", Texture, 0);
        shader.SetVector3("uTint", Tint);
        shader.SetFloat("uAmbientStrength", AmbientStrength);
        shader.SetFloat("uSpecularStrength", SpecularStrength);
        shader.SetFloat("uShininess", Shininess);
    }
}
