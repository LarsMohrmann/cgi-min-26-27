using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Engine.Geometry;

// Vertexformat aller Meshes: Position (xyz), Normale (xyz), Texturkoordinate (uv).
// Die Feldreihenfolge bestimmt das Speicherlayout im Vertexbuffer.
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
