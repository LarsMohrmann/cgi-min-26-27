# Changelog

Each lecture state is tagged in Git. Update your own project with
`git fetch upstream --tags` and `git merge --no-edit lecture-XX`.
Changes that can break your own code are marked with **Breaking**.

## lecture-03

First tagged state of the engine.

- `Application` base class with `Initialize`, `FixedUpdate`, `Update`, `Render`, `Shutdown`
- Geometry: `Vertex`, `MeshData`, `Primitives.CreateCube`, `ObjLoader` (.obj files)
- OpenGL: `Mesh`, `Texture`, `ShaderProgram`, `ShaderSource`, `OpenGlRenderer`
- Materials: `UnlitTexture`, `WaveDistortion`, `ReflectionMapping`, `Phong`
- Example: own material in the application project (`Example/Materials/NormalColor`)
- `ShaderSource` also finds embedded shaders when the project's root namespace
  differs from the code's namespace (e.g. after copying `Example` to `MyGame`)
