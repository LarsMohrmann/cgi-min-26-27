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
        return new ShaderSource(anchor.Assembly,
            ResolveResourceName(anchor, vertexFile), ResolveResourceName(anchor, fragmentFile), anchor.Name);
    }

    private static string ResolveResourceName(Type anchor, string file)
    {
        string expected = anchor.Namespace is null ? file : anchor.Namespace + "." + file;
        string[] names = anchor.Assembly.GetManifestResourceNames();
        if (names.Contains(expected)) return expected;

        // The resource name is built from the project's root namespace and the folder. If the code's
        // namespace differs (e.g. after copying and renaming a project), fall back to a unique file name.
        string[] matches = names.Where(n => n.EndsWith("." + file, StringComparison.Ordinal)).ToArray();
        return matches.Length == 1 ? matches[0] : expected;
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
