using Engine.OpenGL;

namespace Engine.Materials;

// Ein Material legt fest, mit welchem Shader gezeichnet wird und welche Oberflächenwerte er bekommt.
// Es besitzt keine OpenGL-Ressourcen: den kompilierten Shader verwaltet der Renderer,
// und Texturen werden nur referenziert, freigeben muss sie, wer sie erzeugt hat.
//
// Jeder Material-Shader muss uModel, uView und uProjection verwenden.
// uCameraPosition und uLightPosition setzt der Renderer nur, wenn der Shader sie verwendet.
public abstract class Material
{
    protected internal abstract ShaderSource ShaderSource { get; }

    // Wird aufgerufen, während der Shader bereits aktiv ist.
    protected internal abstract void ApplyUniforms(ShaderProgram shader);
}
