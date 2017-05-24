using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SoftEngine
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
            foreach (Model model in models)
            {
                var worldMatrix = Matrix.RotationYawPitchRoll(model.Rotation.Y, model.Rotation.X, model.Rotation.Z) * Matrix.Translation(model.Position);
                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;
                foreach (var vertex in model.Vertices)
                {
                    Vector2 point = Project(vertex, transformMatrix);
                    renderer.DrawPixel(point, Color4.White);
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
    }
}
