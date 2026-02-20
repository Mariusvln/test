using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using My3DEngine;

class Program : GameWindow
{
    private Shader shader = null!;
    private Camera camera;
    private Vector2 lastMousePos;
    private bool firstMouse = true;
    private float deltaTime;

    private List<ModelInstance> sceneModels = new();
    private int currentModelIndex = 0;

    public Program()
        : base(GameWindowSettings.Default,
               new NativeWindowSettings() { ClientSize = new Vector2i(800, 600), Title = "3D Scene" })
    {
        camera = new Camera(new Vector3(0, 0, 5));
        CursorState = CursorState.Grabbed;
    }

    protected override void OnLoad()
    {
        GL.Enable(EnableCap.DepthTest);

        shader = new Shader("shader.vert", "shader.frag");

        // Load models from Assets folder
        string assetsPath = "Assets";
        if (!Directory.Exists(assetsPath))
        {
            Console.WriteLine("Assets folder not found! Create 'Assets' folder with subfolders for each model.");
            Close();
            return;
        }

        var modelFolders = Directory.GetDirectories(assetsPath);
        if (modelFolders.Length == 0)
        {
            Console.WriteLine("No model folders found in Assets!");
            Close();
            return;
        }

        int gridX = 0;
        int gridZ = 0;
        int spacing = 3;

        foreach (var folder in modelFolders)
        {
            string[] objFiles = Directory.GetFiles(folder, "*.obj");
            if (objFiles.Length == 0) continue;

            string objFile = objFiles[0];
            string texFile = Directory.GetFiles(folder, "*.jpg").Length > 0 ?
                             Directory.GetFiles(folder, "*.jpg")[0] :
                             Directory.GetFiles(folder, "*.png").Length > 0 ?
                             Directory.GetFiles(folder, "*.png")[0] : "";

            Model m = new Model(objFile, texFile);
            Vector3 pos = new Vector3(gridX * spacing, 0, gridZ * spacing);
            sceneModels.Add(new ModelInstance(m, pos, Vector3.Zero, Vector3.One));

            gridX++;
            if (gridX > 4) { gridX = 0; gridZ++; }
        }

        if (sceneModels.Count == 0)
        {
            Console.WriteLine("No valid models found in Assets!");
            Close();
            return;
        }

        currentModelIndex = 0;

        // Drag & drop support
        this.FileDrop += (e) =>
        {
            foreach (var file in e.FileNames)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".obj")
                {
                    string folder = Path.GetDirectoryName(file)!;
                    string texFile = Directory.GetFiles(folder, "*.jpg").Length > 0 ?
                                     Directory.GetFiles(folder, "*.jpg")[0] :
                                     Directory.GetFiles(folder, "*.png").Length > 0 ?
                                     Directory.GetFiles(folder, "*.png")[0] : "";

                    Model m = new Model(file, texFile);
                    sceneModels.Add(new ModelInstance(m,
                        new Vector3(sceneModels.Count * spacing, 0, 0), Vector3.Zero, Vector3.One));
                }
            }
        };
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        deltaTime = (float)args.Time;

        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

        camera.ProcessKeyboard(KeyboardState, deltaTime);

        Vector2 mousePos = MouseState.Position;
        if (firstMouse)
        {
            lastMousePos = mousePos;
            firstMouse = false;
        }
        float xoffset = mousePos.X - lastMousePos.X;
        float yoffset = mousePos.Y - lastMousePos.Y;
        lastMousePos = mousePos;
        camera.ProcessMouse(xoffset, yoffset);

        if (sceneModels.Count > 0)
        {
            // Rotate current model
            if (KeyboardState.IsKeyPressed(Keys.Tab))
                currentModelIndex = (currentModelIndex + 1) % sceneModels.Count;

            sceneModels[currentModelIndex].rotation.Y += 30f * deltaTime;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use();

        Matrix4 view = camera.GetViewMatrix();
        Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y, 0.1f, 100f);

        GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "view"), false, ref view);
        GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "projection"), false, ref proj);
        GL.Uniform3(GL.GetUniformLocation(shader.Handle, "lightPos"), new Vector3(2, 5, 2));
        GL.Uniform3(GL.GetUniformLocation(shader.Handle, "viewPos"), camera.Position);

        foreach (var instance in sceneModels)
        {
            Matrix4 modelMat = Matrix4.CreateScale(instance.scale) *
                               Matrix4.CreateRotationX(MathHelper.DegreesToRadians(instance.rotation.X)) *
                               Matrix4.CreateRotationY(MathHelper.DegreesToRadians(instance.rotation.Y)) *
                               Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(instance.rotation.Z)) *
                               Matrix4.CreateTranslation(instance.position);

            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "model"), false, ref modelMat);
            instance.model.Draw();
        }

        SwapBuffers();
    }

    static void Main()
    {
        using var game = new Program();
        game.Run();
    }
}
