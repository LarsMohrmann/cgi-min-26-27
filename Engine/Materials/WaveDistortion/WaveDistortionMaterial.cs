using Engine.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Materials.WaveDistortion;

// Displaces the vertices along a sine wave in the vertex shader and displays a texture without lighting.
// Wobble is the phase of the wave: if it is increased in Update, the wave travels across the mesh.
// The wave only becomes visible if the mesh has enough vertices along the x axis.
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

    // Multiplied by the texture color; white leaves the texture unchanged.
    public Vector3 Tint { get; set; } = Vector3.One;

    // Phase of the sine wave in radians.
    public float Wobble { get; set; }

    protected internal override ShaderSource ShaderSource => Source;

    protected internal override void ApplyUniforms(ShaderProgram shader)
    {
        shader.SetTexture("uTexture", Texture, 0);
        shader.SetVector3("uTint", Tint);
        shader.SetFloat("uWobble", Wobble);
    }
}
