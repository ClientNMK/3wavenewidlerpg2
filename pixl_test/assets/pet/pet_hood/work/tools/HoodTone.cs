using System;
using System.Drawing;
using System.Drawing.Imaging;

// Re-tones the hood spirit: dark body (indigo/navy) and glow (cyan) are remapped separately.
public static class HoodTone
{
    static void ToHsl(Color c, out double h, out double s, out double l)
    {
        double r = c.R / 255.0, g = c.G / 255.0, b = c.B / 255.0;
        double mx = Math.Max(r, Math.Max(g, b)), mn = Math.Min(r, Math.Min(g, b));
        l = (mx + mn) / 2; double d = mx - mn;
        if (d < 1e-6) { h = 0; s = 0; return; }
        s = l > 0.5 ? d / (2 - mx - mn) : d / (mx + mn);
        if (mx == r) h = ((g - b) / d + (g < b ? 6 : 0)) * 60;
        else if (mx == g) h = ((b - r) / d + 2) * 60;
        else h = ((r - g) / d + 4) * 60;
    }
    static double Hue2(double p, double q, double t)
    {
        if (t < 0) t += 1; if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 0.5) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }
    static Color FromHsl(int a, double h, double s, double l)
    {
        h = ((h % 360) + 360) % 360 / 360.0; s = Math.Max(0, Math.Min(1, s)); l = Math.Max(0, Math.Min(1, l));
        double r, g, b;
        if (s < 1e-6) r = g = b = l;
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s, p = 2 * l - q;
            r = Hue2(p, q, h + 1.0 / 3); g = Hue2(p, q, h); b = Hue2(p, q, h - 1.0 / 3);
        }
        return Color.FromArgb(a, (int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
    }

    // body: pixels with l < glowMinL that are not cyan -> hue bodyH, l = l*bodyLMul + bodyLAdd, s = s*bodySMul
    // glow: cyan-ish (hue 160-215, l >= 0.35) -> hue glowH, s *= glowSMul, l += glowLAdd
    // whites (s<0.12, l>0.8) untouched
    public static void Apply(string src, string dst, double bodyH, double bodyLMul, double bodyLAdd, double bodySMul,
                             double glowH, double glowSMul, double glowLAdd)
    {
        using (var b = new Bitmap(src))
        using (var o = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    var c = b.GetPixel(x, y);
                    if (c.A == 0) { o.SetPixel(x, y, c); continue; }
                    double h, s, l; ToHsl(c, out h, out s, out l);
                    Color n = c;
                    if (s < 0.12 && l > 0.8) n = c;
                    else if (h >= 160 && h <= 215 && l >= 0.35) n = FromHsl(c.A, glowH + (h - 190) * 0.3, s * glowSMul, l + glowLAdd);
                    else n = FromHsl(c.A, bodyH + (h - 240) * 0.3, s * bodySMul, l * bodyLMul + bodyLAdd);
                    o.SetPixel(x, y, n);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }
}
