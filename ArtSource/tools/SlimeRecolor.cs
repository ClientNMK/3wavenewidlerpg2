using System;
using System.Drawing;

public static class SlimeRecolor
{
    // Shift hue of slime-body pixels only; keep seaweed (olive), cream specular, mouth red.
    public static void Run(string inPath, string outPath, double delta, double satMul, double lightAdd)
    {
        RunEx(inPath, outPath, delta, satMul, lightAdd, -1, 0, 360);
    }

    // RunEx: pale body pixels (lightness >= paleL) get their new hue clamped into [hueMin, hueMax].
    // Used for gold dead frames, where the pale swollen body otherwise drifts pink. paleL < 0 = off.
    public static void RunEx(string inPath, string outPath, double delta, double satMul, double lightAdd, double paleL, double hueMin, double hueMax)
    {
        using (var src = new Bitmap(inPath))
        using (var dst = new Bitmap(src.Width, src.Height))
        {
            for (int y = 0; y < src.Height; y++)
                for (int x = 0; x < src.Width; x++)
                {
                    Color c = src.GetPixel(x, y);
                    if (c.A == 0) { dst.SetPixel(x, y, c); continue; }
                    double h = c.GetHue(), s = c.GetSaturation(), l = c.GetBrightness();
                    bool body =
                        (h >= 100 && h <= 195 && l >= 0.38) ||            // body tones / shadows
                        (h >= 70 && h < 100 && l >= 0.75 && s < 0.45) ||   // pale body highlights
                        (h >= 140 && h <= 195 && l < 0.38);                // body outline / eyes (very dark)
                    if (!body) { dst.SetPixel(x, y, c); continue; }
                    double nh = (h + delta) % 360; if (nh < 0) nh += 360;
                    if (paleL >= 0 && l >= paleL) nh = Math.Max(hueMin, Math.Min(hueMax, nh));
                    double ns = Math.Min(1.0, s * satMul);
                    double nl = l;
                    if (l >= 0.38) nl = Math.Max(0, Math.Min(1, l + lightAdd));
                    else ns = Math.Min(ns, 0.35);
                    dst.SetPixel(x, y, FromHsl(c.A, nh, ns, nl));
                }
            dst.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    static Color FromHsl(int a, double h, double s, double l)
    {
        double q = l < 0.5 ? l * (1 + s) : l + s - l * s, p = 2 * l - q, hk = h / 360.0;
        double r = Hue(p, q, hk + 1.0 / 3), g = Hue(p, q, hk), b = Hue(p, q, hk - 1.0 / 3);
        return Color.FromArgb(a, (int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
    }

    static double Hue(double p, double q, double t)
    {
        if (t < 0) t += 1; if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 0.5) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }
}
