using System.Reflection;

namespace Engine.OpenGL;

// Beschreibt, woher die GLSL-Quelltexte eines Shaders kommen. Kompiliert wird erst im Renderer,
// weil dafür ein OpenGL-Kontext nötig ist. Eine Instanz pro Materialtyp genügt.
public sealed class ShaderSource
{
    private readonly Assembly _assembly;
    private readonly string _vertexResource;
    private readonly string _fragmentResource;

    public string Name { get; }

    private ShaderSource(Assembly assembly, string vertexResource, string fragmentResource, string name)
    {
        _assembly = assembly;
        _vertexResource = vertexResource;
        _fragmentResource = fragmentResource;
        Name = name;
    }

    // Die GLSL-Dateien liegen im Ordner, der dem Namespace von 'anchor' entspricht,
    // und werden als EmbeddedResource in dessen Assembly eingebettet.
    public static ShaderSource FromEmbeddedResources(Type anchor, string vertexFile, string fragmentFile)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        string prefix = anchor.Namespace is null ? "" : anchor.Namespace + ".";
        return new ShaderSource(anchor.Assembly, prefix + vertexFile, prefix + fragmentFile, anchor.Name);
    }

    internal ShaderProgram Compile() =>
        ShaderProgram.FromSources(ReadResource(_vertexResource), ReadResource(_fragmentResource), Name);

    private string ReadResource(string resourceName)
    {
        using Stream? stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new FileNotFoundException(
                $"Eingebetteter Shader '{resourceName}' fehlt. Vorhanden: {string.Join(", ", _assembly.GetManifestResourceNames())}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
