using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SharpDX;

namespace CottonRenderer.ModelLoader
{
    public class BabylonJSLoader: ModelLoader
    {
        public BabylonJSLoader(): base()
        {

        }
        public override Model[] LoadModelFile(string fileName)
        {
            List<Model> meshes = new List<Model>();
            StreamReader r = new StreamReader(fileName);
            string data = r.ReadToEnd();
            r.Close();
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            for (int meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                dynamic verticesArray = jsonObject.meshes[meshIndex].vertices;
                // Faces
                dynamic indicesArray = jsonObject.meshes[meshIndex].indices;

                dynamic uvCount = jsonObject.meshes[meshIndex].uvCount.Value;
                int verticesStep = 1;

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
                dynamic verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                dynamic facesCount = indicesArray.Count / 3;
                Model mesh = new Model(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount, RenderMode.Colored);

                // Filling the Vertices array of our mesh first
                for (int index = 0; index < verticesCount; index++)
                {
                    float x = (float)verticesArray[index * verticesStep].Value;
                    float y = (float)verticesArray[index * verticesStep + 1].Value;
                    float z = (float)verticesArray[index * verticesStep + 2].Value;
                    // Loading the vertex normal exported by Blender
                    float nx = (float)verticesArray[index * verticesStep + 3].Value;
                    float ny = (float)verticesArray[index * verticesStep + 4].Value;
                    float nz = (float)verticesArray[index * verticesStep + 5].Value;
                    mesh.Vertices[index] = new Vertex { Coordinates = new Vector3(x, y, z), Normal = new Vector3(nx, ny, nz) };
                }
                Color4 color = new Color4(1f, 0.5f, 0.5f, 1f);
                // Then filling the Faces array
                for (int index = 0; index < facesCount; index++)
                {
                    int a = (int)indicesArray[index * 3].Value;
                    int b = (int)indicesArray[index * 3 + 1].Value;
                    int c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c, Color = color };
                }

                // Getting the position you've set in Blender
                dynamic position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
        public override async Task<Model[]> LoadModelFileAsync(string fileName)
        {
            List<Model> meshes = new List<Model>();
            StreamReader r = new StreamReader(fileName);
            string data = await r.ReadToEndAsync();
            r.Close();
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            for (int meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                dynamic verticesArray = jsonObject.meshes[meshIndex].vertices;
                // Faces
                dynamic indicesArray = jsonObject.meshes[meshIndex].indices;

                dynamic uvCount = jsonObject.meshes[meshIndex].uvCount.Value;
                int verticesStep = 1;

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
                dynamic verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                dynamic facesCount = indicesArray.Count / 3;
                Model mesh = new Model(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount, RenderMode.Colored);

                // Filling the Vertices array of our mesh first
                for (int index = 0; index < verticesCount; index++)
                {
                    float x = (float)verticesArray[index * verticesStep].Value;
                    float y = (float)verticesArray[index * verticesStep + 1].Value;
                    float z = (float)verticesArray[index * verticesStep + 2].Value;
                    // Loading the vertex normal exported by Blender
                    float nx = (float)verticesArray[index * verticesStep + 3].Value;
                    float ny = (float)verticesArray[index * verticesStep + 4].Value;
                    float nz = (float)verticesArray[index * verticesStep + 5].Value;
                    mesh.Vertices[index] = new Vertex { Coordinates = new Vector3(x, y, z), Normal = new Vector3(nx, ny, nz) };
                }
                Color4 color = new Color4(1f, 0.5f, 0.5f, 1f);
                // Then filling the Faces array
                for (int index = 0; index < facesCount; index++)
                {
                    int a = (int)indicesArray[index * 3].Value;
                    int b = (int)indicesArray[index * 3 + 1].Value;
                    int c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c, Color = color };
                }

                // Getting the position you've set in Blender
                dynamic position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
    }
}
