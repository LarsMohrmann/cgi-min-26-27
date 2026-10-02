namespace Engine.Geometry;

// Indexed triangle geometry in main memory, without OpenGL state.
// Primitives and model loaders create MeshData; only Mesh uploads it to the GPU.
public sealed class MeshData
{
    public Vertex[] Vertices { get; }
    public uint[] Indices { get; }

    public MeshData(Vertex[] vertices, uint[] indices)
    {
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(indices);
        if (vertices.Length == 0)
            throw new ArgumentException("At least one vertex is required.", nameof(vertices));
        if (indices.Length == 0 || indices.Length % 3 != 0)
            throw new ArgumentException("Each triangle requires three indices.", nameof(indices));
        foreach (uint index in indices)
        {
            if (index >= vertices.Length)
                throw new ArgumentOutOfRangeException(nameof(indices), $"Index {index} does not refer to any of the {vertices.Length} vertices.");
        }

        Vertices = vertices;
        Indices = indices;
    }
}
