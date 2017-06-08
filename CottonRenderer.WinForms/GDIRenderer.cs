using System;
using System.Windows.Forms;
using CottonRenderer;
using SharpDX;
using System.Drawing;

namespace CottonRenderer.WinForms
{
    public class GDIRenderer: Renderer
    {
        public GDIRenderer(PictureBox box): base()
        {
            target = box;
            RenderTarget = new Bitmap(target.Width, target.Height);
            target.Image = RenderTarget;
            g = Graphics.FromImage(RenderTarget);
        }
        private PictureBox target;
        private Bitmap RenderTarget;
        private Graphics g;
        public override void Clear(Color4 color)
        {
            g.Clear(ConvertColor(color));
            g.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
        }
        public override void DrawPixel(Vector2 pos, Color4 color)
        {
            if (pos.X < RenderTarget.Width && pos.Y < RenderTarget.Height && pos.X > 0 && pos.Y > 0)
            {
                RenderTarget.SetPixel((int)pos.X, (int)pos.Y, ConvertColor(color));
            }
            base.DrawPixel(pos, color);
        }
        public override void Display()
        {
            g.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
            target.Refresh();
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
        private System.Drawing.Color ConvertColor(Color4 color)
        {
            return System.Drawing.Color.FromArgb((int)(color.Alpha * 255), (int)(color.Red * 255), (int)(color.Green * 255), (int)(color.Blue * 255));
        }
    }
}
