using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using CottonRenderer;
using CottonRenderer.ModelLoader;
using SharpDX;
using System.Diagnostics;

namespace CottonSFML
{
    public class Game
    {
        public Game()
        {
            
        }
        RenderWindow window;
        Image img;
        Texture tex;
        Sprite s;
        Device device;
        Camera camera;
        private Model[] meshes;
        private SFMLImageRenderer renderer;
        public void Run()
        {
            window = new RenderWindow(new VideoMode(800, 600), "CottonSFML", Styles.Close | Styles.Titlebar);
            window.Closed += Window_Closed;
            window.MouseButtonPressed += Window_MouseButtonPressed;
            img = new Image(800, 600);
            tex = new Texture(img);
            s = new Sprite(tex);
            s.Position = new Vector2f(0, 0);
            renderer = new SFMLImageRenderer(img, 800, 600);
            device = new Device(renderer, 800, 600);
            camera = new Camera();
            BabylonJSLoader loader = new BabylonJSLoader();
            meshes = loader.LoadModelFile("monkey.babylon");
            meshes[0].Rotation = new Vector3(3.14f, 0f, 0f);
            
            camera.Position = new Vector3(0, 0, 15.0f);
            camera.Target = Vector3.Zero;
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(SFML.Graphics.Color.Black);
                Draw();
                window.Display();
            }
        }

        private void Window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            device.Clear(Color4.Black);
            foreach (var mesh in meshes)
            {
                mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y + 0.1f, mesh.Rotation.Z);
                device.Render(camera, mesh);
            }
            device.Display();
            tex = new Texture(renderer.target);
            s.Texture = tex;
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
        private void Draw()
        {
            window.Draw(s);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
