using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.UnlitTexture;

// Displays a texture without lighting, optionally multiplied by a color.
public sealed class UnlitTextureMaterial : Material
{
    private static readonly ShaderSource Source =
        ShaderSource.FromEmbeddedResources(typeof(UnlitTextureMaterial), "unlit_texture.vert.glsl", "unlit_texture.frag.glsl");

    private Texture _texture;

    public UnlitTextureMaterial(Texture texture)
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

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", Texture, 0);
        shader.SetVector3("uTint", Tint);
    }
}
