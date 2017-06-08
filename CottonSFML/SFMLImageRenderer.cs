using System;
using CottonRenderer;
using SharpDX;
using SFML.Graphics;

namespace CottonRenderer.SFMLNET
{
    public class SFMLImageRenderer: Renderer
    {
        public SFMLImageRenderer(Image t, int w, int h)
        {
            target = t;
            Height = h;
            Width = w;
        }
        public Image target;
        int Height;
        int Width;
        public override void Clear(Color4 color)
        {
            for(uint x = 0; x < Width; x++)
            {
                for(uint y = 0; y < Height; y++)
                {
                    target.SetPixel(x, y, SFML.Graphics.Color.Black);
                } 
            }
        }
        public override void Display()
        {
            base.Display();
        }
        public override void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color1, Color4 color2)
        {
            int x0 = (int)pos1.X;
            int y0 = (int)pos1.Y;
            int x1 = (int)pos2.X;
            int y1 = (int)pos2.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                DrawPixel(new Vector2(x0, y0), color1);

                if ((x0 == x1) && (y0 == y1)) break;
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }
        public override void DrawPixel(Vector2 pos, Color4 color)
        {
            target.SetPixel((uint)pos.X, (uint)pos.Y, ConvertColor(color));
        }
        public override Color4 GetTexturePixel(Vector2 pos)
        {
            return Color4.White;
        }
        private SFML.Graphics.Color ConvertColor(Color4 col)
        {
            SFML.Graphics.Color c = new SFML.Graphics.Color()
            {
                R = (byte)(col.Red * 255),
                B = (byte)(col.Blue * 255),
                G = (byte)(col.Green * 255),
                A = (byte)(col.Alpha * 255)
            };
            return c;
        }
    }
}
