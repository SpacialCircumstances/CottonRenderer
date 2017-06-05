using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using SharpDX;

namespace CottonRenderer
{
    public class Device
    {
        public Device(Renderer r, int width, int height)
        {
            renderer = r;
            Width = width;
            Height = height;
        }
        Renderer renderer;
        public int Height;
        public int Width;
        public Vector3 Project(Vector3 coord, Matrix transMat)
        {
            var point = Vector3.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * Width + Width / 2.0f;
            var y = -point.Y * Height + Height / 2.0f;
            return (new Vector3(x, y, point.Z));
        }
        public void Render(Camera camera, params Model[] models)
        {
            var viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            var projectionMatrix = Matrix.PerspectiveFovRH(0.78f, (float)Width / Height, 0.01f, 1.0f);
            var col = new Color4(0.0f, 0.5f, 1.0f, 1.0f);
            foreach (Model model in models)
            {
                var worldMatrix = Matrix.RotationYawPitchRoll(model.Rotation.Y, model.Rotation.X, model.Rotation.Z) * Matrix.Translation(model.Position);
                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;
                int faceIndex = 0;
                foreach (var face in model.Faces)
                {
                    var vertexA = model.Vertices[face.A];
                    var vertexB = model.Vertices[face.B];
                    var vertexC = model.Vertices[face.C];

                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    var color = 0.25f + (faceIndex % model.Faces.Length) * 0.75f / model.Faces.Length;
                    DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1));
                    faceIndex++;
                }
            }
        }
        public void Clear(Color4 color)
        {
            renderer.Clear(color);
        }
        public void Display()
        {
            renderer.Display();
        }
        public async Task<Model[]> LoadJSONFileAsync(string fileName)
        {
            var meshes = new List<Model>();
            StreamReader r = new StreamReader(fileName);
            var data = await r.ReadToEndAsync();
            r.Close();
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            for (var meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                var verticesArray = jsonObject.meshes[meshIndex].vertices;
                // Faces
                var indicesArray = jsonObject.meshes[meshIndex].indices;

                var uvCount = jsonObject.meshes[meshIndex].uvCount.Value;
                var verticesStep = 1;

                // Depending of the number of texture's coordinates per vertex
                // we're jumping in the vertices array  by 6, 8 & 10 windows frame
                switch ((int)uvCount)
                {
                    case 0:
                        verticesStep = 6;
                        break;
                    case 1:
                        verticesStep = 8;
                        break;
                    case 2:
                        verticesStep = 10;
                        break;
                }

                // the number of interesting vertices information for us
                var verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                var facesCount = indicesArray.Count / 3;
                var mesh = new Model(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount, RenderMode.Colored);

                // Filling the Vertices array of our mesh first
                for (var index = 0; index < verticesCount; index++)
                {
                    var x = (float)verticesArray[index * verticesStep].Value;
                    var y = (float)verticesArray[index * verticesStep + 1].Value;
                    var z = (float)verticesArray[index * verticesStep + 2].Value;
                    mesh.Vertices[index] = new Vector3(x, y, z);
                }

                // Then filling the Faces array
                for (var index = 0; index < facesCount; index++)
                {
                    var a = (int)indicesArray[index * 3].Value;
                    var b = (int)indicesArray[index * 3 + 1].Value;
                    var c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender
                var position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        void DrawLineLeftRight(int y, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pd, Color4 color)
        {
            var gradient1 = pa.Y != pb.Y ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            for (var x = sx; x < ex; x++)
            {
                renderer.DrawPixel(new Vector2(x, y), color);
            }
        }

        public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color4 color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            float dP1P2;
            float dP1P3;

            if (p2.Y - p1.Y > 0)
            {
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            }
            else
            {
                dP1P2 = 0;
            }

            if (p3.Y - p1.Y > 0)
            {
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            }
            else
            {
                dP1P3 = 0;
            }

            if (dP1P2 > dP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        DrawLineLeftRight(y, p1, p3, p1, p2, color);
                    }
                    else
                    {
                        DrawLineLeftRight(y, p1, p3, p2, p3, color);
                    }
                }
            }
            else
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        DrawLineLeftRight(y, p1, p2, p1, p3, color);
                    }
                    else
                    {
                        DrawLineLeftRight(y, p2, p3, p1, p3, color);
                    }
                }
            }
        }
    }
}
