namespace Engine.Geometry;

// Indizierte Dreiecksgeometrie im Hauptspeicher, ohne OpenGL-Zustand.
// Primitive und Modell-Loader erzeugen MeshData; erst Mesh lädt sie auf die GPU.
public sealed class MeshData
{
    public Vertex[] Vertices { get; }
    public uint[] Indices { get; }

    public MeshData(Vertex[] vertices, uint[] indices)
    {
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(indices);
        if (vertices.Length == 0)
            throw new ArgumentException("Mindestens ein Vertex wird benötigt.", nameof(vertices));
        if (indices.Length == 0 || indices.Length % 3 != 0)
            throw new ArgumentException("Pro Dreieck werden drei Indizes benötigt.", nameof(indices));
        foreach (uint index in indices)
        {
            if (index >= vertices.Length)
                throw new ArgumentOutOfRangeException(nameof(indices), $"Index {index} verweist auf keinen der {vertices.Length} Vertices.");
        }

        Vertices = vertices;
        Indices = indices;
    }
}
