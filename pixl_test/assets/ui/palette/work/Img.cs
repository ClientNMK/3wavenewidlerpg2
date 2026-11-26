using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

public static class Img
{
    public static string Thumb(string path, string outPath, int max, bool nearest)
    {
        using (var b = new Bitmap(path))
        {
            double m = Math.Min(1.0, (double)max / Math.Max(b.Width, b.Height));
            int w = Math.Max(1, (int)(b.Width * m)), h = Math.Max(1, (int)(b.Height * m));
            using (var o = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(o))
            {
                g.InterpolationMode = nearest ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
                g.Clear(Color.FromArgb(255, 110, 110, 110));
                g.DrawImage(b, 0, 0, w, h);
                o.Save(outPath, ImageFormat.Png);
            }
            return b.Width + "x" + b.Height;
        }
    }

    static byte[] Bytes(Bitmap b, out int stride)
    {
        var d = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var buf = new byte[d.Stride * d.Height];
        Marshal.Copy(d.Scan0, buf, 0, buf.Length);
        stride = d.Stride;
        b.UnlockBits(d);
        return buf;
    }

    // Top colours of opaque pixels, quantised to `step`, returned as "#RRGGBB pct".
    public static string[] Top(string path, int step, int n)
    {
        using (var src = new Bitmap(path))
        {
            int stride; var buf = Bytes(src, out stride);
            var cnt = new Dictionary<int, long[]>();
            long total = 0;
            for (int y = 0; y < src.Height; y++)
                for (int x = 0; x < src.Width; x++)
                {
                    int i = y * stride + x * 4;
                    if (buf[i + 3] < 200) continue;
                    int r = buf[i + 2], g = buf[i + 1], bl = buf[i];
                    int key = ((r / step) << 16) | ((g / step) << 8) | (bl / step);
                    long[] v;
                    if (!cnt.TryGetValue(key, out v)) { v = new long[4]; cnt[key] = v; }
                    v[0]++; v[1] += r; v[2] += g; v[3] += bl; total++;
                }
            return cnt.Values.OrderByDescending(v => v[0]).Take(n)
                .Select(v => string.Format("#{0:X2}{1:X2}{2:X2} {3:F1}%", v[1] / v[0], v[2] / v[0], v[3] / v[0], 100.0 * v[0] / total)).ToArray();
        }
    }
}
