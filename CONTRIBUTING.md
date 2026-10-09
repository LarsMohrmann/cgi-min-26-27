# Contributing to the engine

You want to add something to the engine (a new material, a primitive, a loader,
a bug fix)? Great! Changes to the course repository are only accepted through
pull requests. Nobody pushes directly to `main`.

Your own game stays in your own repository (see "Your own project" in the
[README](README.md)). A contribution to the engine is a separate, small change.

## How it works

1. **Fork** this repository on GitHub (button "Fork", top right).
2. Clone your fork and create a branch for your change:

   ```bash
   git clone https://github.com/<your-name>/cgi-min-26-27.git
   cd cgi-min-26-27
   git switch -c add-toon-material
   ```

3. Make your change, then check that everything still builds and the example runs:

   ```bash
   dotnet build CgiMin.sln
   dotnet run --project Example/Example.csproj
   ```

4. Commit and push the branch to your fork:

   ```bash
   git add -A
   git commit -m "Add toon material"
   git push -u origin add-toon-material
   ```

5. On GitHub, open a **pull request** from your branch to `main` of the course
   repository. Describe what you changed and why, and add a screenshot if it is
   something visual.

The lecturer reviews the pull request, may ask for changes, and merges it. Merged
changes are part of the next lecture tag (e.g. `lecture-06`), so everybody gets
them with `git merge --no-edit lecture-06`.

## Rules

- **One topic per pull request.** A new material and a loader fix are two pull requests.
- **Engine only.** Change files in `Engine/` (and, if needed, `README.md` or
  `Example/` to show your feature). Do not include your own game.
- **Follow the existing structure.** A new material gets its own folder in
  `Engine/Materials/<Name>/` with its class and `.glsl` files, like the existing ones.
- **Do not break existing code.** Renaming or removing public classes, methods or
  shader uniforms breaks everybody's projects. If it is really necessary, explain
  it in the pull request.
- **It must build without errors and warnings.**
- **English** for code, comments and the pull request description.
- **Only add files you are allowed to share.** Textures and models must be your
  own or have a license that allows sharing them.
