using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace CottonRenderer
{
    public class Model
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public RenderMode Mode { get; set; }

        public Model(string name, int verticesCount, int facesCount, RenderMode mode)
        {
            Vertices = new Vector3[verticesCount];
            Faces = new Face[facesCount];
            Name = name;
            Mode = mode;
        }
    }
}
