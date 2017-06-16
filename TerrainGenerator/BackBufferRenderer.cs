using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CottonRenderer;
using SharpDX;

namespace TerrainGenerator
{
    public class BackBufferRenderer: Renderer
    {
        public BackBufferRenderer(int w, int h)
        {
            Width = w;
            Height = h;
            BackBuffer = new System.Drawing.Color[w * h];
        }
        int Width;
        int Height;
        public System.Drawing.Color[] BackBuffer;
        public override void Clear(Color4 color)
        {
            System.Drawing.Color col = ConvertColor(color);
            for (int index = 0; index < BackBuffer.Length; index ++)
            {
                BackBuffer[index] = col;
            }
        }
        public override void Display()
        {
            base.Display();
        }
        public override void DrawPixel(Vector2 pos, Color4 color)
        {
            int index = ((int)pos.X + (int)pos.Y * Width);

            BackBuffer[index] = ConvertColor(color);
        }
        public override Color4 GetTexturePixel(Vector2 pos)
        {
            return Color4.White;
        }
        public override void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color1, Color4 color2)
        {
            base.DrawLine(pos1, pos2, color1, color2);
        }
        private System.Drawing.Color ConvertColor(Color4 color)
        {
            return System.Drawing.Color.FromArgb((int)(color.Alpha * 255), (int)(color.Red * 255), (int)(color.Green * 255), (int)(color.Blue * 255));
        }
    }
}
