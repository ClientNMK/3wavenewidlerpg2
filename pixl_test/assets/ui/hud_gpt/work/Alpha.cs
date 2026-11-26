using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class Alpha
{
    // Returns "w h | bbox(a>th) x0 y0 x1 y1 | transparentPct semiPct opaquePct | magentaPct"
    public static string Info(string path, int th)
    {
        using (var b = new Bitmap(path))
        {
            var r = new Rectangle(0, 0, b.Width, b.Height);
            var d = b.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var buf = new byte[d.Stride * d.Height];
            Marshal.Copy(d.Scan0, buf, 0, buf.Length);
            b.UnlockBits(d);
            int x0 = int.MaxValue, y0 = int.MaxValue, x1 = -1, y1 = -1;
            long tr = 0, semi = 0, op = 0, mag = 0, n = (long)b.Width * b.Height;
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    int i = y * d.Stride + x * 4;
                    byte bl = buf[i], g = buf[i + 1], rd = buf[i + 2], a = buf[i + 3];
                    if (a == 0) tr++; else if (a < 250) semi++; else op++;
                    if (a > 200 && rd > 235 && g < 30 && bl > 235) mag++;
                    if (a > th) { if (x < x0) x0 = x; if (x > x1) x1 = x; if (y < y0) y0 = y; if (y > y1) y1 = y; }
                }
            return string.Format("{0}x{1} | bbox {2},{3},{4},{5} | tr {6:F1}% semi {7:F1}% op {8:F1}% | magenta {9:F2}%",
                b.Width, b.Height, x0, y0, x1, y1, 100.0 * tr / n, 100.0 * semi / n, 100.0 * op / n, 100.0 * mag / n);
        }
    }

    public static void Crop(string path, int x, int y, int w, int h, string outPath)
    {
        using (var b = new Bitmap(path))
        using (var c = b.Clone(new Rectangle(x, y, w, h), PixelFormat.Format32bppArgb)) c.Save(outPath, ImageFormat.Png);
    }
}
