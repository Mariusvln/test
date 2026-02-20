using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace My3DEngine
{
    public class Camera
    {
        public Vector3 Position;
        private float pitch, yaw;
        private Vector3 front = -Vector3.UnitZ;
        public Vector3 Front => front;
        public Vector3 Up = Vector3.UnitY;
        public Vector3 Right => Vector3.Normalize(Vector3.Cross(front, Up));

        public float Speed = 2.5f;
        public float Sensitivity = 0.1f;

        public Camera(Vector3 startPosition)
        {
            Position = startPosition;
            yaw = -90f;
            pitch = 0f;
        }

        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + front, Up);

        public void ProcessKeyboard(KeyboardState keys, float deltaTime)
        {
            float velocity = Speed * deltaTime;
            if (keys.IsKeyDown(Keys.W)) Position += front * velocity;
            if (keys.IsKeyDown(Keys.S)) Position -= front * velocity;
            if (keys.IsKeyDown(Keys.A)) Position -= Right * velocity;
            if (keys.IsKeyDown(Keys.D)) Position += Right * velocity;
        }

        public void ProcessMouse(float xoffset, float yoffset)
        {
            xoffset *= Sensitivity;
            yoffset *= Sensitivity;

            yaw += xoffset;
            pitch -= yoffset;

            pitch = Math.Clamp(pitch, -89f, 89f);

            Vector3 f;
            f.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            f.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            f.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
            front = Vector3.Normalize(f);
        }
    }
}
