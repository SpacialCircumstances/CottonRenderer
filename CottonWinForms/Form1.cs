using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using CottonRenderer;
using System.Diagnostics;
using CottonRenderer.ModelLoader;

namespace CottonWinForms
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }
        private GDIRenderer renderer;
        private Device device;
        private Camera camera;
        private Model[] meshes;
        private void RenderButton_Click(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            device.Clear(Color4.Black);
            foreach(var mesh in meshes)
            {
                mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y + 0.1f, mesh.Rotation.Z);
                device.Render(camera, mesh);
            }
            device.Display();
            watch.Stop();
            MessageBox.Show(Convert.ToString(watch.ElapsedMilliseconds) + "ms", "Renderdauer");
        }

        private async void Form_Load(object sender, EventArgs e)
        {
            renderer = new GDIRenderer(target);
            device = new Device(renderer, target.Image.Size.Width, target.Image.Size.Height);
            camera = new Camera();
            BabylonJSLoader loader = new BabylonJSLoader();
            meshes = await loader.LoadModelFileAsync("landscape.babylon");

            camera.Position = new Vector3(0, 0, 15.0f);
            camera.Target = Vector3.Zero;
            meshes[0].Rotation = new Vector3(0f, 0f, (float)(Math.PI / 2));
        }
    }
}
