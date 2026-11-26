using System;
using System.Drawing;
using System.Drawing.Imaging;

public static class Align
{
    // "x0,y0,x1,y1" of opaque pixels (alpha>0)
    public static string BBox(string path)
    {
        using (var b = new Bitmap(path))
        {
            int x0 = 9999, y0 = 9999, x1 = -1, y1 = -1;
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                    if (b.GetPixel(x, y).A > 0) { x0 = Math.Min(x0, x); y0 = Math.Min(y0, y); x1 = Math.Max(x1, x); y1 = Math.Max(y1, y); }
            return x0 + "," + y0 + "," + x1 + "," + y1;
        }
    }

    // Place src onto a W×H transparent canvas at (dx,dy), pixel exact.
    public static void Place(string src, string dst, int W, int H, int dx, int dy)
    {
        using (var b = new Bitmap(src))
        using (var o = new Bitmap(W, H, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || ny < 0 || nx >= W || ny >= H) continue;
                    var c = b.GetPixel(x, y);
                    if (c.A > 0) o.SetPixel(nx, ny, c);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }

    // Horizontal strip from frames.
    public static void Strip(string[] frames, string dst)
    {
        Bitmap first = new Bitmap(frames[0]);
        int w = first.Width, h = first.Height; first.Dispose();
        using (var o = new Bitmap(w * frames.Length, h, PixelFormat.Format32bppArgb))
        {
            using (var g = Graphics.FromImage(o))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                for (int i = 0; i < frames.Length; i++)
                    using (var f = new Bitmap(frames[i])) g.DrawImageUnscaled(f, i * w, 0);
            }
            o.Save(dst, ImageFormat.Png);
        }
    }
}
