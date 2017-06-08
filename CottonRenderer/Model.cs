using SharpDX;

namespace CottonRenderer
{
    public class Model
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public RenderMode Mode { get; set; }

        public Model(string name, int verticesCount, int facesCount, RenderMode mode)
        {
            Vertices = new Vertex[verticesCount];
            Faces = new Face[facesCount];
            Name = name;
            Mode = mode;
        }
    }
}
