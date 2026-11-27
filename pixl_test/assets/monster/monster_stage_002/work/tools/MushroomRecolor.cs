using System;
using System.Drawing;

public static class MushroomRecolor
{
    // Shift hue of orange cap pixels only (h 17.5~32, s>=0.45, l 0.3~0.66).
    // Leaf (mask + small islands below), cream stem/spots/spores (l>0.66) and dark outline (s<0.45) stay.
    public static void Run(string inPath, string outPath, double delta, double satMul, double lightAdd)
    {
        using (var src = new Bitmap(inPath))
        using (var dst = new Bitmap(src.Width, src.Height))
        {
            int w = src.Width, hgt = src.Height;
            // Leaf mask: h<17.5 seeds, then grow into saturated pixels with >=2 leaf neighbours (h<=20)
            // or >=3 leaf neighbours (any hue, fills leaf veins). Leaf shading shares cap shadow hues.
            var leaf = new bool[w, hgt];
            for (int y = 0; y < hgt; y++)
                for (int x = 0; x < w; x++)
                {
                    Color c = src.GetPixel(x, y);
                    leaf[x, y] = c.A > 0 && c.GetHue() < 17.5 && c.GetSaturation() >= 0.45;
                }
            for (int pass = 0; pass < 3; pass++)
            {
                var grown = (bool[,])leaf.Clone();
                for (int y = 1; y < hgt - 1; y++)
                    for (int x = 1; x < w - 1; x++)
                    {
                        if (leaf[x, y]) continue;
                        int n = (leaf[x - 1, y] ? 1 : 0) + (leaf[x + 1, y] ? 1 : 0) + (leaf[x, y - 1] ? 1 : 0) + (leaf[x, y + 1] ? 1 : 0);
                        Color c = src.GetPixel(x, y);
                        if (c.A == 0 || c.GetSaturation() < 0.45) continue;
                        grown[x, y] = n >= 3 || (n >= 2 && c.GetHue() <= 20);
                    }
                leaf = grown;
            }
            var cand = new bool[w, hgt];
            for (int y = 0; y < hgt; y++)
                for (int x = 0; x < w; x++)
                {
                    Color c = src.GetPixel(x, y);
                    double h = c.GetHue(), s = c.GetSaturation(), l = c.GetBrightness();
                    cand[x, y] = c.A > 0 && !leaf[x, y] && h >= 17.5 && h <= 32 && s >= 0.45 && l >= 0.3 && l <= 0.66;
                }
            // Only big connected pieces are cap; small islands are the leaf when it lightens in hop frames.
            var comp = new int[w, hgt];
            var sizes = new System.Collections.Generic.List<int> { 0 };
            for (int y = 0; y < hgt; y++)
                for (int x = 0; x < w; x++)
                {
                    if (!cand[x, y] || comp[x, y] != 0) continue;
                    int id = sizes.Count, n = 0;
                    var stack = new System.Collections.Generic.Stack<Point>();
                    stack.Push(new Point(x, y)); comp[x, y] = id;
                    while (stack.Count > 0)
                    {
                        Point p = stack.Pop(); n++;
                        foreach (var q in new[] { new Point(p.X + 1, p.Y), new Point(p.X - 1, p.Y), new Point(p.X, p.Y + 1), new Point(p.X, p.Y - 1) })
                            if (q.X >= 0 && q.Y >= 0 && q.X < w && q.Y < hgt && cand[q.X, q.Y] && comp[q.X, q.Y] == 0) { comp[q.X, q.Y] = id; stack.Push(q); }
                    }
                    sizes.Add(n);
                }
            int largest = 0; foreach (int n in sizes) largest = Math.Max(largest, n);
            int minSize = Math.Max(80, largest * 3 / 10);
            for (int y = 0; y < hgt; y++)
                for (int x = 0; x < w; x++)
                {
                    Color c = src.GetPixel(x, y);
                    double h = c.GetHue(), s = c.GetSaturation(), l = c.GetBrightness();
                    if (!cand[x, y] || sizes[comp[x, y]] < minSize) { dst.SetPixel(x, y, c); continue; }
                    double nh = (h + delta) % 360; if (nh < 0) nh += 360;
                    dst.SetPixel(x, y, FromHsl(c.A, nh, Math.Min(1, s * satMul), Math.Max(0, Math.Min(1, l + lightAdd))));
                }
            dst.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    static Color FromHsl(int a, double h, double s, double l)
    {
        double q = l < 0.5 ? l * (1 + s) : l + s - l * s, p = 2 * l - q, hk = h / 360.0;
        return Color.FromArgb(a, C(Hue(p, q, hk + 1.0 / 3)), C(Hue(p, q, hk)), C(Hue(p, q, hk - 1.0 / 3)));
    }
    static int C(double v) { return (int)Math.Round(Math.Max(0, Math.Min(1, v)) * 255); }
    static double Hue(double p, double q, double t)
    {
        if (t < 0) t += 1; if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 0.5) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }
}
