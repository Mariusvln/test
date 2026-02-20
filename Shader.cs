using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace My3DEngine
{
    public class Shader
    {
        public int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            // Load shader source
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            // Compile shaders
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);
            CheckShaderCompile(vertex, vertexPath);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);
            CheckShaderCompile(fragment, fragmentPath);

            // Link program
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, fragment);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(Handle);
                throw new Exception($"Error linking shader program: {info}");
            }

            // Cleanup
            GL.DetachShader(Handle, vertex);
            GL.DetachShader(Handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public void Use() => GL.UseProgram(Handle);

        private void CheckShaderCompile(int shader, string path)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader ({path}): {info}");
            }
        }
    }
}
