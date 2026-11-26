using System;
using System.Drawing;
using System.Drawing.Imaging;

// Recolours only the cream cloak of the hood spirit (face, eyes, ribbon, bell and outline untouched).
public static class CloakTone
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

    public static bool IsCloak(Color c)
    {
        if (c.A == 0) return false;
        double h, s, l; ToHsl(c, out h, out s, out l);
        if (l < 0.38 || h < 13 || h > 27.5) return false;
        return s <= 0.36 || (l >= 0.76 && s <= 0.86);
    }

    // newL = l*lMul + lAdd ; new sat = sat (fixed, scaled a bit by lightness so highlights stay soft)
    public static void Apply(string src, string dst, double hue, double sat, double lMul, double lAdd)
    {
        using (var b = new Bitmap(src))
        using (var o = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    var c = b.GetPixel(x, y);
                    if (IsCloak(c))
                    {
                        double h, s, l; ToHsl(c, out h, out s, out l);
                        double nl = l * lMul + lAdd;
                        double ns = sat * (l >= 0.8 ? 0.85 : 1.0);
                        c = FromHsl(c.A, hue + (h - 22) * 0.5, ns, nl);
                    }
                    o.SetPixel(x, y, c);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }
}
