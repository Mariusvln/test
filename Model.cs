using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace My3DEngine
{
    public class Model
    {
        public int Vao, Vbo;
        public int VertexCount;
        public int Texture;

        public Model(string objPath, string texturePath = "")
        {
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var texCoords = new List<Vector2>();
            var vertices = new List<float>();

            // Parse OBJ file
            foreach (var line in File.ReadLines(objPath))
            {
                string trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

                string[] parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case "v":
                        positions.Add(new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])));
                        break;
                    case "vn":
                        normals.Add(new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])));
                        break;
                    case "vt":
                        texCoords.Add(new Vector2(
                            float.Parse(parts[1]),
                            1 - float.Parse(parts[2]))); // flip V
                        break;
                    case "f":
                        for (int i = 1; i <= 3; i++)
                        {
                            string[] indices = parts[i].Split('/');
                            int vi = int.Parse(indices[0]) - 1;
                            int vti = indices.Length > 1 && !string.IsNullOrEmpty(indices[1]) ? int.Parse(indices[1]) - 1 : -1;
                            int vni = indices.Length > 2 ? int.Parse(indices[2]) - 1 : -1;

                            Vector3 pos = positions[vi];
                            Vector3 norm = vni >= 0 ? normals[vni] : Vector3.UnitY;
                            Vector2 tex = vti >= 0 ? texCoords[vti] : Vector2.Zero;

                            vertices.Add(pos.X); vertices.Add(pos.Y); vertices.Add(pos.Z);
                            vertices.Add(norm.X); vertices.Add(norm.Y); vertices.Add(norm.Z);
                            vertices.Add(tex.X); vertices.Add(tex.Y);
                        }
                        break;
                }
            }

            VertexCount = vertices.Count / 8;

            Vao = GL.GenVertexArray();
            Vbo = GL.GenBuffer();

            GL.BindVertexArray(Vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            // Load texture
            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            if (File.Exists(texturePath))
            {
                using var stream = File.OpenRead(texturePath);
                var image = StbImageSharp.ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
            else
            {
                byte[] redPixel = { 255, 0, 0, 255 };
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, redPixel);
                Console.WriteLine($"Texture not found: {texturePath}, using red fallback.");
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Draw()
        {
            GL.BindVertexArray(Vao);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
        }
    }

    public class ModelInstance
    {
        public Model model;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public ModelInstance(Model m, Vector3 pos, Vector3 rot, Vector3 sc)
        {
            model = m;
            position = pos;
            rotation = rot;
            scale = sc;
        }
    }
}
