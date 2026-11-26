using System;
using System.Drawing;
using System.Drawing.Imaging;

// Recolours the devour mark G: gold rim and cream lips separately. Teeth, mouth inside and drool untouched.
public static class MarkTone
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

    // Like Apply, but lips become dark cocoa (l * lipMul) and cream teeth become near-white.
    // rimOn=false keeps the original gold rim.
    public static void ApplyDark(string src, string dst, bool rimOn, double rimH, double rimS, double rimL, double lipMul)
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
                    if (h >= 20 && h <= 46 && l >= 0.88)
                        c = FromHsl(c.A, 35, 0.6, 0.95);                       // teeth -> white
                    else if (h >= 20 && h <= 46 && s >= 0.55)
                    { if (rimOn) c = FromHsl(c.A, rimH + (h - 38) * 0.4, s * rimS, l + rimL); }
                    else if (h >= 13 && h <= 29 && s < 0.55 && l >= 0.48 && l <= 0.85)
                        c = FromHsl(c.A, 10, 0.38, l * lipMul);                 // lips -> dark cocoa
                    o.SetPixel(x, y, c);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }

    // Teeth white, lips recoloured to (lipH, lipS, l*lipMul). rimMode: 0 = remove rim (keep largest piece + new 1px outline),
    // 1 = recolour rim to rimH, 2 = rim becomes dark cocoa.
    public static void ApplyV2(string src, string dst, int rimMode, double rimH, double rimS, double rimL, double lipH, double lipS, double lipMul, string outlineHex)
    {
        using (var b = new Bitmap(src))
        {
            int W = b.Width, H = b.Height;
            var o = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    var c = b.GetPixel(x, y);
                    if (c.A == 0) { o.SetPixel(x, y, c); continue; }
                    double h, s, l; ToHsl(c, out h, out s, out l);
                    if (h >= 20 && h <= 46 && l >= 0.88) c = FromHsl(c.A, 35, 0.6, 0.96);
                    else if (h >= 20 && h <= 46 && s >= 0.55)
                    {
                        if (rimMode == 0) c = Color.Transparent;
                        else if (rimMode == 1) c = FromHsl(c.A, rimH + (h - 38) * 0.4, s * rimS, l + rimL);
                        else c = FromHsl(c.A, 12, 0.35, l * 0.45);
                    }
                    else if (h >= 13 && h <= 29 && s < 0.55 && l >= 0.48 && l <= 0.85)
                        c = FromHsl(c.A, lipH, lipS, l * lipMul);
                    o.SetPixel(x, y, c);
                }
            if (rimMode == 0)
            {
                // keep largest 4-connected opaque piece
                var lab = new int[W, H]; int best = 0, bestId = 0, id = 0;
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        if (lab[x, y] != 0 || o.GetPixel(x, y).A == 0) continue;
                        id++; int cnt = 0; var st = new System.Collections.Generic.Stack<Point>(); st.Push(new Point(x, y)); lab[x, y] = id;
                        while (st.Count > 0)
                        {
                            var p = st.Pop(); cnt++;
                            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
                            for (int k = 0; k < 4; k++)
                            {
                                int nx = p.X + dx[k], ny = p.Y + dy[k];
                                if (nx < 0 || ny < 0 || nx >= W || ny >= H || lab[nx, ny] != 0 || o.GetPixel(nx, ny).A == 0) continue;
                                lab[nx, ny] = id; st.Push(new Point(nx, ny));
                            }
                        }
                        if (cnt > best) { best = cnt; bestId = id; }
                    }
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                        if (o.GetPixel(x, y).A != 0 && lab[x, y] != bestId) o.SetPixel(x, y, Color.Transparent);
                // 1px outline around the kept piece
                Color ol = ColorTranslator.FromHtml(outlineHex);
                var add = new System.Collections.Generic.List<Point>();
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        if (o.GetPixel(x, y).A != 0) continue;
                        bool near = (x > 0 && o.GetPixel(x - 1, y).A != 0) || (x < W - 1 && o.GetPixel(x + 1, y).A != 0) ||
                                    (y > 0 && o.GetPixel(x, y - 1).A != 0) || (y < H - 1 && o.GetPixel(x, y + 1).A != 0);
                        if (near) add.Add(new Point(x, y));
                    }
                foreach (var p in add) o.SetPixel(p.X, p.Y, ol);
            }
            o.Save(dst, ImageFormat.Png); o.Dispose();
        }
    }

    // rim: hue 20-46, s>=0.55, l<0.88 ; lips: hue 13-29, s<0.55, l 0.48-0.85
    public static void Apply(string src, string dst, double rimH, double rimS, double rimL, double lipH, double lipS, double lipL)
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
                    if (h >= 20 && h <= 46 && s >= 0.55 && l < 0.88)
                        c = FromHsl(c.A, rimH + (h - 38) * 0.4, s * rimS, l + rimL);
                    else if (h >= 13 && h <= 29 && s < 0.55 && l >= 0.48 && l <= 0.85)
                        c = FromHsl(c.A, lipH, Math.Max(s, 0.3) * lipS, l + lipL);
                    o.SetPixel(x, y, c);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }
}
