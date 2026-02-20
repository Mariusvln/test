using OpenTK.Mathematics;

class Camera
{
    public Vector3 Position;
    public Vector3 Front = -Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Right = Vector3.UnitX;

    private float _pitch;
    public float Pitch
    {
        get => _pitch;
        set { _pitch = MathHelper.Clamp(value, -89f, 89f); UpdateVectors(); }
    }

    public float Yaw { get; set; } = -90f;
    private float aspect;

    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        aspect = aspectRatio;
        UpdateVectors();
    }

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + Front, Up);
    public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), aspect, 0.1f, 100f);

    private void UpdateVectors()
    {
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        Front = Vector3.Normalize(front);
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}
