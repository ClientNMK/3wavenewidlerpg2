using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

// Removes a flat background by flood-filling from the image border over pixels close to the border colour.
public static class BgKey
{
    public static void Run(string path, string outPath, int tol)
    {
        using (var src = new Bitmap(path))
        {
            var b = src.Clone(new Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format32bppArgb);
            int w = b.Width, h = b.Height;
            var key = b.GetPixel(0, 0);
            var seen = new bool[w, h];
            var q = new Queue<Point>();
            for (int x = 0; x < w; x++) { q.Enqueue(new Point(x, 0)); q.Enqueue(new Point(x, h - 1)); }
            for (int y = 0; y < h; y++) { q.Enqueue(new Point(0, y)); q.Enqueue(new Point(w - 1, y)); }
            while (q.Count > 0)
            {
                var p = q.Dequeue();
                if (p.X < 0 || p.Y < 0 || p.X >= w || p.Y >= h || seen[p.X, p.Y]) continue;
                seen[p.X, p.Y] = true;
                var c = b.GetPixel(p.X, p.Y);
                if (Math.Abs(c.R - key.R) + Math.Abs(c.G - key.G) + Math.Abs(c.B - key.B) > tol) continue;
                b.SetPixel(p.X, p.Y, Color.Transparent);
                q.Enqueue(new Point(p.X + 1, p.Y)); q.Enqueue(new Point(p.X - 1, p.Y));
                q.Enqueue(new Point(p.X, p.Y + 1)); q.Enqueue(new Point(p.X, p.Y - 1));
            }
            b.Save(outPath, ImageFormat.Png);
            b.Dispose();
        }
    }
}
