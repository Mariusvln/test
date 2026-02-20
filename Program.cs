using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

class Program
{
    static void Main()
    {
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),
            Title = "Simple 3D Renderer v3"
        };

        using var window = new Game(nativeSettings);
        window.Run();
    }
}

class Game : GameWindow
{
    private int _vao, _vbo, _ebo;
    private Shader _shader = null!;
    private Camera _camera = null!;

    private float _rotation = 0f;
    private Vector2 _lastMouse;
    private bool _firstMove = true;

    private readonly float[] _vertices =
    {
        // positions        // normals
        -0.5f,-0.5f,-0.5f,  0f,0f,-1f,
         0.5f,-0.5f,-0.5f,  0f,0f,-1f,
         0.5f, 0.5f,-0.5f,  0f,0f,-1f,
        -0.5f, 0.5f,-0.5f,  0f,0f,-1f,
        -0.5f,-0.5f, 0.5f,  0f,0f,1f,
         0.5f,-0.5f, 0.5f,  0f,0f,1f,
         0.5f, 0.5f, 0.5f,  0f,0f,1f,
        -0.5f, 0.5f, 0.5f,  0f,0f,1f
    };

    private readonly uint[] _indices =
    {
        0,1,2,2,3,0,
        4,5,6,6,7,4,
        0,1,5,5,4,0,
        2,3,7,7,6,2,
        0,3,7,7,4,0,
        1,2,6,6,5,1
    };

    public Game(NativeWindowSettings settings) : base(GameWindowSettings.Default, settings) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
        GL.Enable(EnableCap.DepthTest);

        // VAO / VBO / EBO
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        // vertex positions
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // vertex normals
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Shader
        _shader = new Shader("shader.vert", "shader.frag");
        _shader.Use();

        // Camera
        _camera = new Camera(new Vector3(0f, 0f, 3f), Size.X / (float)Size.Y);

        CursorState = CursorState.Grabbed;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (!IsFocused) return;

        var input = KeyboardState;
        const float speed = 2.5f;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W)) _camera.Position += _camera.Front * speed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S)) _camera.Position -= _camera.Front * speed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A)) _camera.Position -= _camera.Right * speed * (float)args.Time;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D)) _camera.Position += _camera.Right * speed * (float)args.Time;

        // Mouse movement
        var mouse = MouseState.Position;
        if (_firstMove) { _lastMouse = mouse; _firstMove = false; }
        var delta = mouse - _lastMouse;
        _lastMouse = mouse;

        _camera.Yaw += delta.X * 0.1f;
        _camera.Pitch -= delta.Y * 0.1f;

        _rotation += 50f * (float)args.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation));
        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 proj = _camera.GetProjectionMatrix();

        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", proj);

        _shader.SetVector3("lightPos", new Vector3(1.2f, 1f, 2f));
        _shader.SetVector3("viewPos", _camera.Position);
        _shader.SetVector3("lightColor", new Vector3(1f, 1f, 1f));

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }
}
