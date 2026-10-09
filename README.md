# CGI-MIN 26/27

A small starting point for computer graphics with C# and OpenTK: a rotating
model with reflection mapping. The engine also contains materials for plain
texturing, wave distortion and Phong lighting (ambient, diffuse and specular).

## Structure

- `Engine/Application.cs` is the base class for a program. It separates logic
  and drawing, similar to Unity (see below).
- `Engine/Camera.cs` describes a view by position, target and projection.
  Multiple camera instances can exist independently of each other.
- `Engine/Materials/` contains the abstract base class `Material` and one
  subfolder per material type with its class and GLSL files: `UnlitTexture/`
  (texture without lighting), `WaveDistortion/` (texture without lighting,
  vertices displaced by a sine wave; the phase `Wobble` is increased in `Update`),
  `ReflectionMapping/` (texture coordinates calculated from the normal in view
  space, so an environment texture appears reflected on the surface) and `Phong/`
  (single color with lighting). The shaders are embedded into
  `Engine.dll` as `EmbeddedResource`.
- `Engine/Geometry/` contains geometry in main memory, without OpenGL: `Vertex`
  (position, normal, texture coordinate), `MeshData` (vertices and triangle
  indices) and `Primitives` (e.g. `CreateCube()`). `Loaders/ObjLoader` reads the
  geometry from Wavefront .obj files (`v`, `vt`, `vn`, `f`, including polygons and
  faces without UVs or normals) and also returns `MeshData`; `.mtl` files are ignored.
- `Engine/OpenGL/` contains `Mesh` (vertex and index buffer on the GPU),
  `Texture` (image as RGBA texture, loaded with StbImageSharp), `ShaderSource`
  (where a shader comes from), `ShaderProgram` (GLSL compilation and uniforms) and
  `OpenGlRenderer` (OpenGL state, shader management and draw call).
- `Example/` is the executable OpenTK application and references `Engine`.
  Images and other files in `Example/Assets/` are copied next to the application
  during the build.
- `CgiMin.sln` loads both projects together.

The example loads the duck from `Assets/duck_smooth.obj` with `ObjLoader` and
draws it with the `ReflectionMappingMaterial` and `Assets/environment.png`. The
other textures, materials and meshes (cube, star, flat-shaded duck) are commented
out in `Initialize` and can be switched quickly. The mesh is drawn with
`renderer.Draw(mesh, material, modelMatrix, camera)`. The animation only changes
the model matrix; the camera is not a global static class. Materials and cameras
do not draw themselves. A scene model will follow in a later step.

A material only holds surface values and refers to its `ShaderSource`. It only
references textures; whoever created a texture has to dispose of it. The renderer
compiles each material shader once on the first `Draw` and releases it in
`Dispose`. Every material shader must use `uModel`, `uView` and `uProjection`;
`uCameraPosition` and `uLightPosition` are optional.

## Frame sequence

A program derives from `Engine.Application` and only overrides these methods:

| Method | When | What for |
|---|---|---|
| `Initialize()` | once at startup | create renderer, meshes, materials |
| `FramebufferResized(width, height)` | after `Initialize` and on resize | adjust the viewport |
| `FixedUpdate(fixedDeltaTime)` | 0…n times per frame, fixed time step `FixedDeltaTime` (default 1/50 s) | physics, simulation |
| `Update(deltaTime)` | once per frame | animation, input, game logic |
| `Render()` | once per frame, after `Update` | drawing only |
| `Shutdown()` | once on exit | release GPU resources |

`Render()` deliberately receives no time: everything that moves is calculated in
`Update` or `FixedUpdate`, and `Render` only draws it. The OpenTK callbacks
(`OnUpdateFrame`, `OnRenderFrame`, etc.) are sealed. If `FixedUpdate` would have
to catch up more than five steps after a stutter, the remaining time is discarded.

## Running

Requirements: .NET 10 SDK, a graphics driver supporting OpenGL 3.3 and NuGet
access for the first build.

```powershell
dotnet build CgiMin.sln
dotnet run --project Example/Example.csproj
```

In Visual Studio Code, open the **repository folder** and confirm that you trust
it. `Ctrl+Shift+B` builds both projects; `F5` with the configuration
"Example (OpenTK)" starts the example. `Engine` is a library and is not run on
its own.
