using System;
using System.Windows;
using System.Diagnostics;
using SharpDX;
using CottonRenderer;
using CottonRenderer.ModelLoader;

namespace CottonWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private WPFRenderer renderer;
        private Device device;
        private Camera camera;
        private Model[] meshes;

        private void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            device.Clear(Color4.Black);
            foreach (var mesh in meshes)
            {
                mesh.Rotation = new Vector3(mesh.Rotation.X + 0.1f, mesh.Rotation.Y + 0.1f, mesh.Rotation.Z);
                device.Render(camera, mesh);
            }
            device.Display();
            watch.Stop();
            MessageBox.Show(Convert.ToString(watch.ElapsedMilliseconds) + "ms", "Renderdauer");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            renderer = new WPFRenderer(RenderTarget);
            RenderTarget.Source = renderer.Source;
            device = new Device(renderer, (int)RenderTarget.Width, (int)RenderTarget.Height);
            camera = new Camera();
            BabylonJSLoader loader = new BabylonJSLoader();
            meshes = await loader.LoadModelFileAsync("landscape.babylon");

            camera.Position = new Vector3(0, 0, 5.0f);
            camera.Target = Vector3.Zero;
        }
    }
}
