using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace SoftEngine
{
    public class Renderer
    {
        public Renderer()
        {
            
        }
        public virtual void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color1, Color4 color2)
        {

        }
        public virtual void DrawPixel(Vector2 pos, Color4 color)
        {

        }
        public virtual Color4 GetTexturePixel(Vector2 pos)
        {
            return Color4.White;
        }
        public virtual void Clear(Color4 color)
        {

        }
        public virtual void Display()
        {

        }
    }
}
