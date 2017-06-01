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
        public Vector2 Project(Vector3 coord, Matrix transMat)
        {
            var point = Vector3.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * Width + Width / 2.0f;
            var y = -point.Y * Height + Height / 2.0f;
            return (new Vector2(x, y));
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
                foreach (var face in model.Faces)
                {
                    var vertexA = model.Vertices[face.A];
                    var vertexB = model.Vertices[face.B];
                    var vertexC = model.Vertices[face.C];

                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    renderer.DrawLine(pixelA, pixelB, col, col);
                    renderer.DrawLine(pixelB, pixelC, col, col);
                    renderer.DrawLine(pixelC, pixelA, col, col);
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
                var mesh = new Model(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount);

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
    }
}
