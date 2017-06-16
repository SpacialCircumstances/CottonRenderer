using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace TerrainGenerator
{
    public class TerrainRenderSettings
    {
        public TerrainRenderSettings()
        {

        }
        public int ImageHeight;
        public int ImageWidth;
        public ImageFormat Format;
        public Vector3 CameraPosition;
        public Vector3 CameraTarget;
        public HeightmapGenerator Generator;
        public int HeightmapSizeX;
        public int HeightmapSizeZ;
        public Vector3 ModelPosition;
        public Vector3 ModelRotation;
        public Color4 BackgroundColor;
        public string Filepath;
    }
}
