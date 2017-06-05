using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.IO;
using CottonRenderer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace CottonWPF
{
    public class WPFRenderer : Renderer
    {
        //Info: It is necessary to draw the stuff pixel-wise instead of using a byte array.
        //When the resolution is high, we would have to deal with Overflows.
        public WPFRenderer(System.Windows.Controls.Image img) : base()
        {
            Width = (int)img.Width;
            Height = (int)img.Height;
            var dpiX = 96d;
            var dpiY = 96d;
            Source = new WriteableBitmap(Width, Height, dpiX, dpiY, PixelFormats.Bgra32, null);

        }
        int Height;
        int Width;
        public WriteableBitmap Source;
        public override void Clear(Color4 color)
        {
            System.Drawing.Color c = ConvertColor(color);
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    DrawPoint(x, y, c);
                }
            }
        }
        public override void Display()
        {
            //Source = ByteArrayToImage(backBuffer);
        }
        public override void DrawPixel(Vector2 pos, Color4 color)
        {
            DrawPoint((int)pos.X, (int)pos.Y, ConvertColor(color));
        }
        private void DrawPoint(int x, int y, System.Drawing.Color c)
        {
            if (x < Width && y < Height && x > 0 && y > 0)
            {
                int index = (int)(x + y * Width) * 4;
                byte[] b = { c.B, c.R, c.G, c.A };
                Source.WritePixels(new Int32Rect(x, y, 1, 1), b, 4, 0);
            }
        }
        public override Color4 GetTexturePixel(Vector2 pos)
        {
            return Color4.White;
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
        private System.Drawing.Color ConvertColor(Color4 color)
        {
            return System.Drawing.Color.FromArgb((int)(color.Alpha * 255), (int)(color.Red * 255), (int)(color.Green * 255), (int)(color.Blue * 255));
        }
    }
}
