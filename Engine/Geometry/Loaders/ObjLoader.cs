using System.Globalization;
using OpenTK.Mathematics;

namespace Engine.Geometry.Loaders;

// Liest die Geometrie aus Wavefront-.obj-Dateien und liefert indizierte MeshData.
//
// Unterstützt: v, vt, vn und f in allen vier Formen (v, v/vt, v//vn, v/vt/vn),
// Polygone mit mehr als drei Ecken (werden als Fächer trianguliert) und negative Indizes.
// Ignoriert: Materialien (mtllib, usemtl), Objekte und Gruppen (o, g), Smoothing Groups (s),
// Linien und Punkte. Alle Objekte einer Datei landen in einem gemeinsamen Mesh.
public static class ObjLoader
{
    private const int Missing = -1;

    public static MeshData Load(string path)
    {
        using var reader = new StreamReader(path);
        return Parse(reader, Path.GetFileName(path));
    }

    private static MeshData Parse(TextReader reader, string fileName)
    {
        var positions = new List<Vector3>();
        var texCoords = new List<Vector2>();
        var normals = new List<Vector3>();

        var vertices = new List<Vertex>();
        var indices = new List<uint>();
        // Gleiche Kombinationen aus Position, UV und Normale werden nur einmal als Vertex angelegt.
        var vertexLookup = new Dictionary<(int Position, int TexCoord, int Normal), uint>();

        int lineNumber = 0;
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            string[] parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0 || parts[0].StartsWith('#')) continue;

            try
            {
                switch (parts[0])
                {
                    case "v":
                        positions.Add(ParseVector3(parts));
                        break;
                    case "vt":
                        // V ist optional; der Ursprung liegt wie in OpenGL unten links.
                        texCoords.Add(new Vector2(ParseFloat(parts, 1), parts.Length > 2 ? ParseFloat(parts, 2) : 0));
                        break;
                    case "vn":
                        normals.Add(SafeNormalize(ParseVector3(parts)));
                        break;
                    case "f":
                        AddFace(parts);
                        break;
                }
            }
            catch (FormatException e)
            {
                throw new FormatException($"{fileName}:{lineNumber}: {e.Message}", e);
            }
        }

        if (indices.Count == 0)
            throw new FormatException($"{fileName}: Die Datei enthält keine Flächen (f-Zeilen).");

        return new MeshData(vertices.ToArray(), indices.ToArray());

        void AddFace(string[] parts)
        {
            if (parts.Length < 4)
                throw new FormatException("Eine Fläche braucht mindestens drei Ecken.");

            var corners = new (int Position, int TexCoord, int Normal)[parts.Length - 1];
            for (int i = 0; i < corners.Length; i++)
                corners[i] = ParseCorner(parts[i + 1]);

            // Fächer-Triangulierung: (0, 1, 2), (0, 2, 3), (0, 3, 4) …
            for (int i = 1; i < corners.Length - 1; i++)
            {
                var a = corners[0];
                var b = corners[i];
                var c = corners[i + 1];
                Vector3 faceNormal = FaceNormal(positions[a.Position], positions[b.Position], positions[c.Position]);
                indices.Add(GetVertex(a, faceNormal));
                indices.Add(GetVertex(b, faceNormal));
                indices.Add(GetVertex(c, faceNormal));
            }
        }

        (int, int, int) ParseCorner(string corner)
        {
            // "1", "1/2", "1//3" oder "1/2/3" – leere Felder bedeuten: nicht vorhanden.
            string[] fields = corner.Split('/');
            if (fields.Length > 3)
                throw new FormatException($"Ungültige Ecke '{corner}'.");

            int position = ResolveIndex(fields[0], positions.Count, "Positions");
            int texCoord = fields.Length > 1 && fields[1] != "" ? ResolveIndex(fields[1], texCoords.Count, "UV") : Missing;
            int normal = fields.Length > 2 && fields[2] != "" ? ResolveIndex(fields[2], normals.Count, "Normalen") : Missing;
            return (position, texCoord, normal);
        }

        uint GetVertex((int Position, int TexCoord, int Normal) corner, Vector3 faceNormal)
        {
            // Ohne Normale aus der Datei bekommt jedes Dreieck eine eigene, flache Normale.
            // Solche Ecken dürfen deshalb nicht mit Ecken anderer Dreiecke zusammengelegt werden.
            bool shareable = corner.Normal != Missing;
            if (shareable && vertexLookup.TryGetValue(corner, out uint existing)) return existing;

            var vertex = new Vertex(
                positions[corner.Position],
                corner.Normal != Missing ? normals[corner.Normal] : faceNormal,
                corner.TexCoord != Missing ? texCoords[corner.TexCoord] : Vector2.Zero);
            uint index = (uint)vertices.Count;
            vertices.Add(vertex);
            if (shareable) vertexLookup.Add(corner, index);
            return index;
        }
    }

    // OBJ-Indizes beginnen bei 1; negative Werte zählen vom zuletzt gelesenen Eintrag rückwärts.
    private static int ResolveIndex(string field, int count, string kind)
    {
        if (!int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index) || index == 0)
            throw new FormatException($"Ungültiger {kind}-Index '{field}'.");

        int resolved = index > 0 ? index - 1 : count + index;
        if (resolved < 0 || resolved >= count)
            throw new FormatException($"{kind}-Index {index} verweist auf keinen der {count} bisher gelesenen Einträge.");
        return resolved;
    }

    private static Vector3 ParseVector3(string[] parts) =>
        new(ParseFloat(parts, 1), ParseFloat(parts, 2), ParseFloat(parts, 3));

    private static float ParseFloat(string[] parts, int index)
    {
        if (index >= parts.Length)
            throw new FormatException($"'{parts[0]}' braucht mindestens {index} Zahlenwerte.");
        if (!float.TryParse(parts[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            throw new FormatException($"'{parts[index]}' ist keine gültige Zahl.");
        return value;
    }

    private static Vector3 FaceNormal(Vector3 a, Vector3 b, Vector3 c) =>
        SafeNormalize(Vector3.Cross(b - a, c - a));

    // Ein Nullvektor (z. B. von einem entarteten Dreieck) hat keine Richtung und bleibt Null statt NaN.
    private static Vector3 SafeNormalize(Vector3 v) =>
        v.LengthSquared > 0 ? v.Normalized() : Vector3.Zero;
}
