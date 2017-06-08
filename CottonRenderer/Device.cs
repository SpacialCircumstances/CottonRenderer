using System;
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
            depthBuffer = new float[Width * Height];
        }
        Renderer renderer;
        public int Height;
        public int Width;
        private float[] depthBuffer;
        public Vertex Project(Vertex vertex, Matrix transMat, Matrix world)
        {
            // transforming the coordinates into 2D space
            Vector3 point2d = Vector3.TransformCoordinate(vertex.Coordinates, transMat);
            // transforming the coordinates & the normal to the vertex in the 3D world
            Vector3 point3dWorld = Vector3.TransformCoordinate(vertex.Coordinates, world);
            Vector3 normal3dWorld = Vector3.TransformCoordinate(vertex.Normal, world);

            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            float x = point2d.X * Width + Width / 2.0f;
            float y = -point2d.Y * Height + Height / 2.0f;
            return new Vertex
            {
                Coordinates = new Vector3(x, y, point2d.Z),
                Normal = normal3dWorld,
                WorldCoordinates = point3dWorld,
                Color = vertex.Color
            };
        }

        public void Render(Camera camera, params Model[] models)
        {
            Matrix viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            Matrix projectionMatrix = Matrix.PerspectiveFovRH(0.78f, (float)Width / Height, 0.01f, 1.0f);
            Color4 col = new Color4(0.0f, 0.5f, 1.0f, 1.0f);
            foreach (Model model in models)
            {
                Matrix worldMatrix = Matrix.RotationYawPitchRoll(model.Rotation.Y, model.Rotation.X, model.Rotation.Z) * Matrix.Translation(model.Position);
                Matrix transformMatrix = worldMatrix * viewMatrix * projectionMatrix;
                int faceIndex = 0;
                foreach (Face face in model.Faces)
                {
                    Vertex vertexA = model.Vertices[face.A];
                    Vertex vertexB = model.Vertices[face.B];
                    Vertex vertexC = model.Vertices[face.C];

                    Vertex pixelA = Project(vertexA, transformMatrix, worldMatrix);
                    Vertex pixelB = Project(vertexB, transformMatrix, worldMatrix);
                    Vertex pixelC = Project(vertexC, transformMatrix, worldMatrix);

                    float color = 0.25f + (faceIndex % model.Faces.Length) * 0.75f / model.Faces.Length;
                    DrawTriangle(pixelA, pixelB, pixelC, face.Color);
                    faceIndex++;
                }
            }
        }
        public void Clear(Color4 color)
        {
            renderer.Clear(color);
            for (int index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = float.MaxValue;
            }
        }
        public void Display()
        {
            renderer.Display();
        }
        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }
        Color4 InterpolateColor(Color4 color1, Color4 color2, float amount)
        {
            if (amount > 1 && amount < 0)
            {
                amount = Clamp(amount);
            }
            Color4 c1 = color1 * amount;
            Color4 c2 = color2 * (1 - amount);
            return c1 + c2;
        }

        void ProcessScanLine(int y, Vertex pa, Vertex pb, Vertex pc, Vertex pd)
        {
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Coordinates.Y != pb.Coordinates.Y ? (y - pa.Coordinates.Y) / (pb.Coordinates.Y - pa.Coordinates.Y) : 1;
            var gradient2 = pc.Coordinates.Y != pd.Coordinates.Y ? (y - pc.Coordinates.Y) / (pd.Coordinates.Y - pc.Coordinates.Y) : 1;

            Color4 color1 = InterpolateColor(pa.Color, pb.Color, gradient1);
            Color4 color2 = InterpolateColor(pc.Color, pd.Color, gradient2);

            int sx = (int)Interpolate(pa.Coordinates.X, pb.Coordinates.X, gradient1);
            int ex = (int)Interpolate(pc.Coordinates.X, pd.Coordinates.X, gradient2);

            // starting Z & ending Z
            float z1 = Interpolate(pa.Coordinates.Z, pb.Coordinates.Z, gradient1);
            float z2 = Interpolate(pc.Coordinates.Z, pd.Coordinates.Z, gradient2);

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                Color4 color = InterpolateColor(color1, color2, gradient);
                DrawPoint(new Vector2(x, y), z, color);
            }
        }

        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        float ComputeNDotL(Vector3 vertex, Vector3 normal, Vector3 lightPosition)
        {
            Vector3 lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }


        public void DrawTriangle(Vertex p1, Vertex p2, Vertex p3, Color4 color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Coordinates.Y > p2.Coordinates.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Coordinates.Y > p3.Coordinates.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Coordinates.Y > p2.Coordinates.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (p2.Coordinates.Y - p1.Coordinates.Y > 0)
                dP1P2 = (p2.Coordinates.X - p1.Coordinates.X) / (p2.Coordinates.Y - p1.Coordinates.Y);
            else
                dP1P2 = 0;

            if (p3.Coordinates.Y - p1.Coordinates.Y > 0)
                dP1P3 = (p3.Coordinates.X - p1.Coordinates.X) / (p3.Coordinates.Y - p1.Coordinates.Y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Coordinates.Y; y <= (int)p3.Coordinates.Y; y++)
                {
                    if (y < p2.Coordinates.Y)
                    {
                        ProcessScanLine(y, p1, p3, p1, p2);
                    }
                    else
                    {
                        ProcessScanLine(y, p1, p3, p2, p3);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (var y = (int)p1.Coordinates.Y; y <= (int)p3.Coordinates.Y; y++)
                {
                    if (y < p2.Coordinates.Y)
                    {
                        ProcessScanLine(y, p1, p2, p1, p3);
                    }
                    else
                    {
                        ProcessScanLine(y, p2, p3, p1, p3);
                    }
                }
            }
        }
        private void DrawPoint(Vector2 pos, float z, Color4 color)
        {
            if (pos.X >= Width || pos.X < 0 || pos.Y >= Height || pos.Y < 0)
            {
                return;
            }
            int index = (int)(pos.X + pos.Y * Width);
            int index4 = index * 4;

            if (depthBuffer[index] < z)
            {
                return;
            }

            depthBuffer[index] = z;
            renderer.DrawPixel(pos, color);
        }
    }
}
