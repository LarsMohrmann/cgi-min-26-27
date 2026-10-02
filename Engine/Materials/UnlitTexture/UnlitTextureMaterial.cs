using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.UnlitTexture;

// Zeigt eine Textur ohne Beleuchtung an, optional mit einer Farbe multipliziert.
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

    // Wird mit der Texturfarbe multipliziert; Weiß lässt die Textur unverändert.
    public Vector3 Tint { get; set; } = Vector3.One;

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", Texture, 0);
        shader.SetVector3("uTint", Tint);
    }
}
