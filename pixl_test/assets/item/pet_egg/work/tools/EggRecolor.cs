using System;
using System.Drawing;

public static class EggRecolor
{
    // Shift hue of pixels whose hue is in [hMin,hMax] (wraps if hMin>hMax) and lightness in [lMin,lMax].
    public static void Apply(Bitmap bmp, double hMin, double hMax, double lMin, double lMax, double sMin,
                             double delta, double satMul, double satAdd, double lightAdd)
    {
        for (int y = 0; y < bmp.Height; y++)
            for (int x = 0; x < bmp.Width; x++)
            {
                Color c = bmp.GetPixel(x, y);
                if (c.A == 0) continue;
                double h = c.GetHue(), s = c.GetSaturation(), l = c.GetBrightness();
                bool inHue = hMin <= hMax ? (h >= hMin && h <= hMax) : (h >= hMin || h <= hMax);
                if (!inHue || l < lMin || l > lMax || s < sMin) continue;
                double nh = (h + delta) % 360; if (nh < 0) nh += 360;
                double ns = Math.Max(0, Math.Min(1, s * satMul + satAdd));
                double nl = Math.Max(0, Math.Min(1, l + lightAdd));
                bmp.SetPixel(x, y, FromHsl(c.A, nh, ns, nl));
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
