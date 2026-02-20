using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;

class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        int vertex = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertex, vertexSource);
        GL.CompileShader(vertex);
        GL.GetShader(vertex, ShaderParameter.CompileStatus, out int success);
        if (success == 0) throw new Exception(GL.GetShaderInfoLog(vertex));

        int fragment = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragment, fragmentSource);
        GL.CompileShader(fragment);
        GL.GetShader(fragment, ShaderParameter.CompileStatus, out success);
        if (success == 0) throw new Exception(GL.GetShaderInfoLog(fragment));

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertex);
        GL.AttachShader(Handle, fragment);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0) throw new Exception(GL.GetProgramInfoLog(Handle));

        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 mat)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(loc, false, ref mat);
    }

    public void SetVector3(string name, Vector3 vec)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(loc, vec);
    }
}
