# CGI-MIN 26/27

Kleiner Startpunkt für Computergrafik mit C# und OpenTK: Ein rotierender,
texturierter Würfel ohne Beleuchtung. Ein Phong-Material mit Ambient-, Diffuse-
und Specular-Beleuchtung liegt ebenfalls in der Engine.

## Aufbau

- `Engine/Application.cs` ist die Basisklasse für ein Programm. Sie trennt Logik
  und Zeichnen ähnlich wie Unity (siehe unten).
- `Engine/Camera.cs` beschreibt eine Ansicht über Position, Blickpunkt und
  Projektion. Mehrere Kamerainstanzen können unabhängig voneinander existieren.
- `Engine/Materials/` enthält die abstrakte Basis `Material` und pro Materialtyp
  einen Unterordner mit Klasse und GLSL-Dateien: `UnlitTexture/` (Textur ohne
  Licht), `WaveDistortion/` (Textur ohne Licht, Vertices per Sinuswelle verschoben;
  die Phase `Wobble` wird in `Update` erhöht) und `Phong/` (einfarbig mit Beleuchtung). Die Shader werden als `EmbeddedResource`
  in die `Engine.dll` eingebettet.
- `Engine/Geometry/` enthält Geometrie im Hauptspeicher ohne OpenGL: `Vertex`
  (Position, Normale, Texturkoordinate), `MeshData` (Vertices und Dreiecksindizes)
  und `Primitives` (z. B. `CreateCube()`). `Loaders/ObjLoader` liest die Geometrie
  aus Wavefront-.obj-Dateien (`v`, `vt`, `vn`, `f`, auch Polygone und Faces ohne UVs
  oder Normalen) und liefert ebenfalls `MeshData`; `.mtl`-Dateien werden ignoriert.
- `Engine/OpenGL/` enthält `Mesh` (Vertex- und Indexbuffer auf der GPU),
  `Texture` (Bild als RGBA-Textur, geladen mit StbImageSharp),
  `ShaderSource` (woher ein Shader kommt), `ShaderProgram` (GLSL-Kompilierung und Uniforms) und `OpenGlRenderer`
  (OpenGL-Zustand, Shader-Verwaltung und Draw-Aufruf).
- `Example/` ist die ausführbare OpenTK-Anwendung und referenziert `Engine`.
  Bilder und andere Dateien in `Example/Assets/` werden beim Build neben die
  Anwendung kopiert; die Textur wird aus `Assets/wood_box.png` geladen.
- `CgiMin.sln` lädt beide Projekte zusammen.

Das Example lädt den Stern aus `Assets/star.obj` mit `ObjLoader` (alternativ, auskommentiert,
den Würfel aus `Primitives.CreateCube()`) und zeichnet ihn mit
`renderer.Draw(mesh, material, modelMatrix, camera)`. Die Animation ändert nur
die Modellmatrix; die Kamera ist keine globale statische Klasse. Material und
Kamera zeichnen nicht selbst. Ein Szenenmodell kommt erst in einem späteren Schritt.

Ein Material hält nur Oberflächenwerte und verweist auf seine `ShaderSource`.
Texturen referenziert es nur; freigeben muss sie, wer sie erzeugt hat.
Der Renderer kompiliert jeden Material-Shader beim ersten `Draw` einmal und gibt
ihn in `Dispose` frei. Jeder Material-Shader muss `uModel`, `uView` und
`uProjection` verwenden; `uCameraPosition` und `uLightPosition` sind optional.

## Ablauf eines Frames

Ein Programm erbt von `Engine.Application` und überschreibt nur diese Methoden:

| Methode | Wann | Wofür |
|---|---|---|
| `Initialize()` | einmal beim Start | Renderer, Meshes, Materialien anlegen |
| `FramebufferResized(width, height)` | nach `Initialize` und bei Größenänderung | Viewport anpassen |
| `FixedUpdate(fixedDeltaTime)` | 0…n-mal pro Frame, feste Schrittweite `FixedDeltaTime` (Standard 1/50 s) | Physik, Simulation |
| `Update(deltaTime)` | einmal pro Frame | Animation, Eingabe, Spiellogik |
| `Render()` | einmal pro Frame, nach `Update` | nur zeichnen |
| `Shutdown()` | einmal beim Beenden | GPU-Ressourcen freigeben |

`Render()` bekommt bewusst keine Zeit: Alles, was sich bewegt, wird in `Update`
oder `FixedUpdate` berechnet und in `Render` nur noch gezeichnet. Die
OpenTK-Callbacks (`OnUpdateFrame`, `OnRenderFrame` usw.) sind versiegelt.
Holt `FixedUpdate` nach einem Ruckler mehr als fünf Schritte nach, wird die
restliche Zeit verworfen.

## Starten

Voraussetzungen: .NET 10 SDK, ein OpenGL-3.3-fähiger Grafiktreiber und beim
ersten Build Zugriff auf NuGet.

```powershell
dotnet build CgiMin.sln
dotnet run --project Example/Example.csproj
```

In Visual Studio Code den **Repo-Ordner** öffnen und als vertrauenswürdig
bestätigen. `Ctrl+Shift+B` baut beide Projekte; mit `F5` und der Konfiguration
„Example (OpenTK)“ startet der Würfel. `Engine` ist eine Library und wird nicht
eigenständig ausgeführt.
