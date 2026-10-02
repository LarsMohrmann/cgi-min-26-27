using System.Reflection;

namespace Engine.OpenGL;

// Describes where the GLSL sources of a shader come from. Compilation happens in the renderer,
// because it requires an OpenGL context. One instance per material type is enough.
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

    // The GLSL files are located in the folder matching the namespace of 'anchor'
    // and are embedded into its assembly as EmbeddedResource.
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
                $"Embedded shader '{resourceName}' is missing. Available: {string.Join(", ", _assembly.GetManifestResourceNames())}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
