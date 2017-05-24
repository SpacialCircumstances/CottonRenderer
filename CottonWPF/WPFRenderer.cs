using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CottonRenderer;

namespace SoftEngineWPF
{
    public class WPFRenderer: Renderer
    {
        public override void Clear(Color4 color)
        {
            base.Clear(color);
        }
        public override void Display()
        {
            base.Display();
        }
        public override void DrawPixel(Vector2 pos, Color4 color)
        {
            base.DrawPixel(pos, color);
        }
        public override Color4 GetTexturePixel(Vector2 pos)
        {
            return Color4.White;
        }
        public override void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color1, Color4 color2)
        {
            base.DrawLine(pos1, pos2, color1, color2);
        }
    }
}
