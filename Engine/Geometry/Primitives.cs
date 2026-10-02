using OpenTK.Mathematics;

namespace Engine.Geometry;

// Erzeugt einfache Grundkörper als MeshData. Dreiecke sind gegen den Uhrzeigersinn orientiert.
public static class Primitives
{
    // Würfel um den Ursprung; jede Seite hat eigene Vertices, damit die Normalen hart bleiben.
    public static MeshData CreateCube(float size = 2.0f)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

        float h = size * 0.5f;
        Vector3[] corners =
        [
            new(-h, -h, -h), new(h, -h, -h), new(h, h, -h), new(-h, h, -h),
            new(-h, -h, h), new(h, -h, h), new(h, h, h), new(-h, h, h)
        ];
        // Pro Seite vier Ecken gegen den Uhrzeigersinn, von außen betrachtet.
        (int[] Corners, Vector3 Normal)[] faces =
        [
            ([4, 5, 6, 7], Vector3.UnitZ),
            ([1, 0, 3, 2], -Vector3.UnitZ),
            ([5, 1, 2, 6], Vector3.UnitX),
            ([0, 4, 7, 3], -Vector3.UnitX),
            ([7, 6, 2, 3], Vector3.UnitY),
            ([0, 1, 5, 4], -Vector3.UnitY)
        ];
        Vector2[] texCoords = [new(0, 0), new(1, 0), new(1, 1), new(0, 1)];

        var vertices = new Vertex[faces.Length * 4];
        var indices = new uint[faces.Length * 6];
        for (int f = 0; f < faces.Length; f++)
        {
            for (int c = 0; c < 4; c++)
                vertices[f * 4 + c] = new Vertex(corners[faces[f].Corners[c]], faces[f].Normal, texCoords[c]);

            uint first = (uint)(f * 4);
            int offset = f * 6;
            indices[offset] = first;
            indices[offset + 1] = first + 1;
            indices[offset + 2] = first + 2;
            indices[offset + 3] = first;
            indices[offset + 4] = first + 2;
            indices[offset + 5] = first + 3;
        }

        return new MeshData(vertices, indices);
    }
}
