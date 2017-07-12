using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using SharpDX;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using CottonRenderer;

namespace TerrainGenerator
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

        private void ShowMessage(string error)
        {
            errorOcurred = true;
            MessageBox.Show(error, error, MessageBoxButton.OK, MessageBoxImage.Error);
            LogBox.AppendText(error + "\n");
        }
        bool errorOcurred;
        private async void RenderButton_Click(object sender, RoutedEventArgs e)
        {
            TerrainRenderSettings settings = new TerrainRenderSettings();
            //Validate values
            errorOcurred = false;
            if (ImageSizeX.Text != null && int.TryParse(ImageSizeX.Text, out int ImageHeight))
            {
                if (ImageHeight < 10000 && ImageHeight > 0)
                {
                    settings.ImageHeight = ImageHeight;
                }
                else
                {
                    ShowMessage("Image height too big/small");
                }
            }
            else
            {
                ShowMessage("Image height invalid");
            }
            if (ImageSizeY.Text != null && int.TryParse(ImageSizeY.Text, out int ImageWidth))
            {
                if (ImageWidth < 10000 && ImageWidth > 0)
                {
                    settings.ImageWidth = ImageWidth;
                }
                else
                {
                    ShowMessage("Image width too big/small");
                }
            }
            else
            {
                ShowMessage("Image width invalid");
            }
            int format = ImageFormats.SelectedIndex;
            if (format == 0)
            {
                settings.Format = ImageFormat.JPG;
            }
            else if (format == 1)
            {
                settings.Format = ImageFormat.PNG;
            }
            else
            {
                settings.Format = ImageFormat.BMP;
            }
            #region CAMERA
            float CameraPosition1 = 0f;
            if (CameraPositionX.Text != null && float.TryParse(CameraPositionX.Text, out CameraPosition1))
            {

            }
            else
            {
                ShowMessage("Camera Position X invalid");
            }
            float CameraPosition2 = 0f;
            if (CameraPositionY.Text != null && float.TryParse(CameraPositionY.Text, out CameraPosition2))
            {

            }
            else
            {
                ShowMessage("Camera Position Y invalid");
            }
            float CameraPosition3 = 0f;
            if (CameraPositionZ.Text != null && float.TryParse(CameraPositionZ.Text, out CameraPosition3))
            {

            }
            else
            {
                ShowMessage("Camera Position Z invalid");
            }
            settings.CameraPosition = new Vector3(CameraPosition1, CameraPosition2, CameraPosition3);
            if (CameraTargetX.Text != null && float.TryParse(CameraTargetX.Text, out float CameraTarget1))
            {

            }
            else
            {
                ShowMessage("Camera Target X invalid");
            }
            if (CameraTargetY.Text != null && float.TryParse(CameraTargetY.Text, out float CameraTarget2))
            {

            }
            else
            {
                ShowMessage("Camera Target Y invalid");
            }
            if (CameraTargetZ.Text != null && float.TryParse(CameraTargetZ.Text, out float CameraTarget3))
            {

            }
            else
            {
                ShowMessage("Camera Target Z invalid");
            }
            #endregion CAMERA
            int gen = HeightmapGeneratorType.SelectedIndex;
            if (gen == 0)
            {
                settings.Generator = HeightmapGenerator.Perlin;
            }
            else if (gen == 1)
            {
                settings.Generator = HeightmapGenerator.Simplex;
            }
            else
            {
                ShowMessage("ERROR");
            }
            if (HeightmapSizeX.Text != null && int.TryParse(HeightmapSizeX.Text, out int HeightmapX))
            {
                settings.HeightmapSizeX = HeightmapX;
            }
            else
            {
                ShowMessage("Heightmap size X invalid");
            }
            if (HeightmapSizeZ.Text != null && int.TryParse(HeightmapSizeZ.Text, out int HeightmapZ))
            {
                settings.HeightmapSizeZ = HeightmapZ;
            }
            else
            {
                ShowMessage("Heightmap size Z invalid");
            }
            #region MODEL
            float ModelPosition1 = 0f;
            if (ModelPositionX.Text != null && float.TryParse(ModelPositionX.Text, out ModelPosition1))
            {

            }
            else
            {
                ShowMessage("Model Position X invalid");
            }
            float ModelPosition2 = 0f;
            if (ModelPositionY.Text != null && float.TryParse(ModelPositionY.Text, out ModelPosition2))
            {

            }
            else
            {
                ShowMessage("Model Position Y invalid");
            }
            float ModelPosition3 = 0f;
            if (ModelPositionZ.Text != null && float.TryParse(ModelPositionZ.Text, out ModelPosition3))
            {

            }
            else
            {
                ShowMessage("Model Position Z invalid");
            }
            settings.ModelPosition = new Vector3(ModelPosition1, ModelPosition2, ModelPosition3);
            float ModelRotation1 = 0f;
            if (ModelRotationX.Text != null && float.TryParse(ModelRotationX.Text, out ModelRotation1))
            {

            }
            else
            {
                ShowMessage("Model Rotation X invalid");
            }
            float ModelRotation2 = 0f;
            if (ModelRotationY.Text != null && float.TryParse(ModelRotationY.Text, out ModelRotation2))
            {

            }
            else
            {
                ShowMessage("Model Rotation Y invalid");
            }
            float ModelRotation3 = 0f;
            if (ModelRotationZ.Text != null && float.TryParse(ModelRotationZ.Text, out ModelRotation3))
            {

            }
            else
            {
                ShowMessage("Model Rotation Z invalid");
            }
            settings.ModelRotation = new Vector3(ModelRotation1, ModelRotation2, ModelRotation3);
            #endregion MODEL
            float BGR = 0f;
            if (BackgroundR.Text != null && float.TryParse(BackgroundR.Text, out BGR))
            {

            }
            else
            {
                ShowMessage("Background color R invalid");
            }
            float BGG = 0f;
            if (BackgroundG.Text != null && float.TryParse(BackgroundG.Text, out BGG))
            {

            }
            else
            {
                ShowMessage("Background color G invalid");
            }
            float BGB = 0f;
            if (BackgroundB.Text != null && float.TryParse(BackgroundB.Text, out BGB))
            {

            }
            else
            {
                ShowMessage("Background color B invalid");
            }
            Color4 color = Color4.Black;
            try
            {
                color = new Color4(BGR, BGG, BGB, 1.0f);
            }
            catch(Exception ex)
            {
                ShowMessage(ex.ToString());
            }
            settings.BackgroundColor = color;
            if(FilePathBox.Text != null)
            {
                settings.Filepath = FilePathBox.Text;
            }
            if(errorOcurred)
            {
                MessageBox.Show("Rendering aborted. Cause: Invalid input.", "Rendering canceled", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Deactivate controls
            MainStack.IsEnabled = false;
            RenderButton.IsEnabled = false;
            FilePathBox.IsEnabled = false;

            LogBox.AppendText("Rendering started.\n");

            Render(settings);
            MainStack.IsEnabled = true;
            RenderButton.IsEnabled = true;
            FilePathBox.IsEnabled = true;
        }

        private void Render(TerrainRenderSettings settings)
        {
            //Create renderer objects
            BackBufferRenderer renderer = new BackBufferRenderer(settings.ImageWidth, settings.ImageHeight);
            Device device = new Device(renderer, settings.ImageWidth, settings.ImageHeight);
            Camera camera = new Camera()
            {
                Position = settings.CameraPosition,
                Target = settings.CameraTarget
            };

            //Create terrain
            PlaneNoiseMapBuilder builder = new PlaneNoiseMapBuilder();
            NoiseMap map = new NoiseMap(settings.HeightmapSizeX, settings.HeightmapSizeZ);
            builder.DestNoiseMap = map;
            if(settings.Generator == HeightmapGenerator.Perlin)
            {
                Perlin perlin = new Perlin()
                {
                    Quality = NoiseQuality.Best,
                    Lacunarity = 0.5f,
                    OctaveCount = 8,
                    Persistence = 2f
                };
                builder.SourceModule = perlin;
            }
            else
            {
                Simplex simplex = new Simplex()
                {
                    OctaveCount = 8,
                    Lacunarity = 0.5f,
                    Persistence = 2f
                };
                builder.SourceModule = simplex;
            }
            CancellationToken cancel = new CancellationToken();
            builder.SetDestSize(settings.HeightmapSizeX, settings.HeightmapSizeZ);
            builder.SetBounds(0, 20, 0, 20);
            builder.Build();
            Model terrain = new Model("Terrain", settings.HeightmapSizeZ * settings.HeightmapSizeX, settings.HeightmapSizeX * settings.HeightmapSizeZ * 2, RenderMode.Colored)
            {
                Position = settings.ModelPosition,
                Rotation = settings.ModelRotation
            };
            Random random = new Random();
            for (int x = 0; x < settings.HeightmapSizeX; x++)
            {
                for (int z = 0; z < settings.HeightmapSizeZ; z++)
                {
                    int index = x * settings.HeightmapSizeX + z;
                    float y = map.GetValue(x, z) / 10;
                    Vertex temp = new Vertex()
                    {
                        Color = new Color4(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1), 1),
                        Coordinates = new Vector3(x, y, z),
                        Normal = new Vector3(0, 0, 0)
                    };
                    terrain.Vertices[index] = temp;
                }
            }
            for (int i = 0; i < (settings.HeightmapSizeZ - 1) * settings.HeightmapSizeX - 1; i++)
            {
                Face face1 = new Face();
                face1.A = i;
                face1.B = i + settings.HeightmapSizeX;
                face1.C = i + 1;
                Face face2 = new Face();
                face2.A = i;
                face2.B = i + settings.HeightmapSizeX + 1;
                face2.C = i + 1;
                terrain.Faces[i * 2] = face1;
                terrain.Faces[i * 2 + 1] = face2;
            }
            device.Clear(Color4.Black);
            device.Render(camera, terrain);
            device.Display();
            Bitmap bmp;
            bmp = new Bitmap(settings.ImageWidth, settings.ImageHeight);
            for(int x = 0; x < settings.ImageWidth; x++)
            {
                for (int y = 0; y < settings.ImageHeight; y++)
                {
                    bmp.SetPixel(x, y, renderer.BackBuffer[y * settings.ImageWidth + x]);
                }
            }
            bmp.Save("output.png");

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
