using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.WaveDistortion;

// Verschiebt die Vertices im Vertex-Shader entlang einer Sinuswelle und zeigt eine Textur ohne Beleuchtung.
// Wobble ist die Phase der Welle: Wird sie in Update erhöht, läuft die Welle über das Mesh.
// Sichtbar wird die Welle nur, wenn das Mesh entlang der x-Achse genug Vertices hat.
public sealed class WaveDistortionMaterial : Material
{
    private static readonly ShaderSource Source =
        ShaderSource.FromEmbeddedResources(typeof(WaveDistortionMaterial), "wave_distortion.vert.glsl", "wave_distortion.frag.glsl");

    private Texture _texture;

    public WaveDistortionMaterial(Texture texture)
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

    // Phase der Sinuswelle im Bogenmaß.
    public float Wobble { get; set; }

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", Texture, 0);
        shader.SetVector3("uTint", Tint);
        shader.SetFloat("uWobble", Wobble);
    }
}
