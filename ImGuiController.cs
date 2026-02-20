using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using ImGuiNET;

public class ImGuiController
{
    private int _vertexArray;
    private int _fontAtlasID;
    private int _windowWidth;
    private int _windowHeight;

    public ImGuiController(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
        ImGui.CreateContext();
        ImGui.GetIO().Fonts.AddFontDefault();
        ImGui.StyleColorsDark();
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);
        _fontAtlasID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontAtlasID);
    }

    public void Update(GameWindow window, float deltaTime)
    {
        ImGui.NewFrame();
        ImGui.Begin("Scene Models");
        ImGui.Text("Press H to hide/show this panel");
        ImGui.Text($"Window size: {_windowWidth} x {_windowHeight}");
        ImGui.End();
        ImGui.Render();
        RenderDrawData(ImGui.GetDrawData());
    }

    public void RenderDrawData(ImDrawDataPtr drawData)
    {
        GL.Viewport(0, 0, _windowWidth, _windowHeight);
    }

    public void WindowResized(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
    }
}
