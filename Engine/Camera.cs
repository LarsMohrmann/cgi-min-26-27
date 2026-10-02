using OpenTK.Mathematics;

namespace Engine;

// Eine Kamera beschreibt nur eine Ansicht; sie besitzt keinen OpenGL-Zustand.
public sealed class Camera
{
    public Vector3 Position { get; private set; }
    public Vector3 Target { get; private set; }
    public Vector3 Up { get; private set; }

    public float FieldOfViewDegrees { get; set; } = 50.0f;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100.0f;

    public Camera(Vector3 position, Vector3 target)
    {
        LookAt(position, target);
    }

    public void LookAt(Vector3 position, Vector3 target, Vector3? up = null)
    {
        Position = position;
        Target = target;
        Up = up ?? Vector3.UnitY;
    }

    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
        if (aspectRatio <= 0)
            throw new ArgumentOutOfRangeException(nameof(aspectRatio));
        if (FieldOfViewDegrees <= 0 || FieldOfViewDegrees >= 180)
            throw new ArgumentOutOfRangeException(nameof(FieldOfViewDegrees));
        if (NearPlane <= 0 || FarPlane <= NearPlane)
            throw new ArgumentOutOfRangeException(nameof(NearPlane));

        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(FieldOfViewDegrees), aspectRatio, NearPlane, FarPlane);
    }
}
