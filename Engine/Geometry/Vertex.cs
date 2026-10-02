using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Engine.Geometry;

// Vertex format of all meshes: position (xyz), normal (xyz), texture coordinate (uv).
// The field order defines the memory layout in the vertex buffer.
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vertex
{
    public readonly Vector3 Position;
    public readonly Vector3 Normal;
    public readonly Vector2 TexCoord;

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        Position = position;
        Normal = normal;
        TexCoord = texCoord;
    }
}
