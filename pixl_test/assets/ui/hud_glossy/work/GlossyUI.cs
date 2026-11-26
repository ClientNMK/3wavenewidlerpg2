using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// Procedural glossy UI parts (smooth, non-pixel). All sizes are final pixel sizes (2x of logical HUD size).
public static class GlossyUI
{
    static Random rng = new Random(7);

    public static Color C(string hex, int a = 255)
    {
        hex = hex.TrimStart('#');
        return Color.FromArgb(a, Convert.ToInt32(hex.Substring(0, 2), 16), Convert.ToInt32(hex.Substring(2, 2), 16), Convert.ToInt32(hex.Substring(4, 2), 16));
    }

    static Bitmap NewBmp(int w, int h, out Graphics g)
    {
        var b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        g = Graphics.FromImage(b);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.Clear(Color.Transparent);
        return b;
    }

    public static GraphicsPath RoundRect(RectangleF r, float rad)
    {
        var p = new GraphicsPath();
        float d = Math.Min(rad * 2, Math.Min(r.Width, r.Height));
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    static GraphicsPath Star4(float cx, float cy, float r, float inner)
    {
        var p = new GraphicsPath();
        var pts = new PointF[8];
        for (int i = 0; i < 8; i++)
        {
            double ang = -Math.PI / 2 + i * Math.PI / 4;
            float rr = (i % 2 == 0) ? r : r * inner;
            pts[i] = new PointF(cx + (float)(Math.Cos(ang) * rr), cy + (float)(Math.Sin(ang) * rr));
        }
        p.AddPolygon(pts);
        return p;
    }

    static void Glow(Graphics g, GraphicsPath path, Color col, float maxW, int steps, int maxA)
    {
        for (int i = steps; i >= 1; i--)
        {
            float w = maxW * i / steps;
            int a = (int)(maxA * (1.0 - (double)i / (steps + 1)) / 2.2);
            using (var pen = new Pen(Color.FromArgb(Math.Max(0, Math.Min(255, a)), col), w) { LineJoin = LineJoin.Round })
                g.DrawPath(pen, path);
        }
    }

    static void Sparkle(Graphics g, float cx, float cy, float r, Color col)
    {
        using (var glow = new GraphicsPath())
        {
            glow.AddEllipse(cx - r * 1.2f, cy - r * 1.2f, r * 2.4f, r * 2.4f);
            using (var pb = new PathGradientBrush(glow) { CenterColor = Color.FromArgb(150, col), SurroundColors = new[] { Color.FromArgb(0, col) } })
                g.FillPath(pb, glow);
        }
        using (var s = Star4(cx, cy, r, 0.18f)) using (var br = new SolidBrush(Color.FromArgb(245, 255, 255, 255))) g.FillPath(br, s);
    }

    static void Bokeh(Graphics g, RectangleF area, int n, Color col, float minR, float maxR, bool fadeLeft)
    {
        for (int i = 0; i < n; i++)
        {
            float x = area.X + (float)rng.NextDouble() * area.Width;
            float y = area.Y + (float)rng.NextDouble() * area.Height;
            float t = fadeLeft ? (x - area.X) / area.Width : 1f;
            float r = minR + (float)rng.NextDouble() * (maxR - minR);
            int a = (int)(20 + 70 * t * rng.NextDouble());
            using (var br = new SolidBrush(Color.FromArgb(a, col))) g.FillEllipse(br, x - r, y - r, r * 2, r * 2);
        }
    }

    // Gold metallic rim: dark outline + gold gradient band + thin highlight.
    static void GoldRim(Graphics g, GraphicsPath path, RectangleF bounds, float w)
    {
        using (var pen = new Pen(C("2a1606"), w + 3) { LineJoin = LineJoin.Round }) g.DrawPath(pen, path);
        using (var lg = new LinearGradientBrush(new PointF(0, bounds.Top - 2), new PointF(0, bounds.Bottom + 2), C("fff2b0"), C("b8740f")))
        {
            var cb = new ColorBlend(4);
            cb.Colors = new[] { C("fff6c8"), C("f2c14e"), C("c98a1b"), C("ffd770") };
            cb.Positions = new[] { 0f, 0.35f, 0.7f, 1f };
            lg.InterpolationColors = cb;
            using (var pen = new Pen(lg, w) { LineJoin = LineJoin.Round }) g.DrawPath(pen, path);
        }
    }

    // Capsule bar backplate: gradient fill, gloss, bokeh, sparkles, gold rim, outer glow.
    public static void Bar(string outPath, int w, int h, string c1, string c2, string glowHex, bool bokeh)
    {
        Graphics g; var b = NewBmp(w, h, out g);
        float pad = 14;
        var r = new RectangleF(pad, pad, w - pad * 2, h - pad * 2);
        using (var path = RoundRect(r, r.Height / 2))
        {
            Glow(g, path, C(glowHex), 22, 8, 160);
            using (var lg = new LinearGradientBrush(r, C(c1), C(c2), LinearGradientMode.Horizontal)) g.FillPath(lg, path);
            g.SetClip(path);
            // depth: darker bottom
            using (var lg = new LinearGradientBrush(r, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(110, 0, 0, 20), LinearGradientMode.Vertical)) g.FillRectangle(lg, r);
            if (bokeh) Bokeh(g, r, 70, Color.White, 2, 9, true);
            // gloss
            var gl = new RectangleF(r.X + r.Height * 0.3f, r.Y + 3, r.Width - r.Height * 0.6f, r.Height * 0.42f);
            using (var gp = RoundRect(gl, gl.Height / 2))
            using (var lg = new LinearGradientBrush(gl, Color.FromArgb(120, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical)) g.FillPath(lg, gp);
            g.ResetClip();
            GoldRim(g, path, r, 5);
        }
        if (bokeh)
        {
            Sparkle(g, w - pad - r.Height * 0.9f, pad + 4, 9, C("fff3c0"));
            Sparkle(g, w - pad - r.Height * 2.2f, h - pad - 2, 6, C("bfe8ff"));
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }

    // Round icon socket ring (left end of bars) with glow and star accents.
    public static void Socket(string outPath, int size, string innerHex, string glowHex, bool stars)
    {
        Graphics g; var b = NewBmp(size, size, out g);
        float pad = size * 0.14f;
        var r = new RectangleF(pad, pad, size - pad * 2, size - pad * 2);
        using (var path = new GraphicsPath())
        {
            path.AddEllipse(r);
            Glow(g, path, C(glowHex), size * 0.12f, 8, 170);
            using (var pb = new PathGradientBrush(path) { CenterColor = C("2b2f63"), SurroundColors = new[] { C(innerHex) } }) g.FillPath(pb, path);
            // inner gloss arc
            var gl = new RectangleF(r.X + r.Width * 0.18f, r.Y + r.Height * 0.06f, r.Width * 0.64f, r.Height * 0.38f);
            using (var gp = new GraphicsPath()) { gp.AddEllipse(gl); using (var lg = new LinearGradientBrush(gl, Color.FromArgb(70, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical)) g.FillPath(lg, gp); }
            GoldRim(g, path, r, size * 0.055f);
        }
        if (stars)
        {
            Sparkle(g, size * 0.82f, size * 0.2f, size * 0.09f, C("fff3c0"));
            Sparkle(g, size * 0.16f, size * 0.78f, size * 0.06f, C("bfe8ff"));
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }

    // Rectangular panel with ornamental star pieces at corners and top center.
    public static void Panel(string outPath, int w, int h, string c1, string c2, string glowHex, bool ornaments)
    {
        Graphics g; var b = NewBmp(w, h, out g);
        float pad = 26;
        var r = new RectangleF(pad, pad, w - pad * 2, h - pad * 2);
        using (var path = RoundRect(r, 14))
        {
            Glow(g, path, C(glowHex), 20, 8, 140);
            using (var lg = new LinearGradientBrush(r, C(c1), C(c2), LinearGradientMode.Vertical)) g.FillPath(lg, path);
            g.SetClip(path);
            Bokeh(g, r, 40, Color.White, 1, 3, false);
            // shooting star streak
            using (var pen = new Pen(Color.FromArgb(90, 255, 255, 255), 2)) g.DrawLine(pen, r.Right - r.Width * 0.22f, r.Y + r.Height * 0.25f, r.Right - r.Width * 0.12f, r.Y + r.Height * 0.12f);
            var gl = new RectangleF(r.X, r.Y, r.Width, r.Height * 0.45f);
            using (var lg = new LinearGradientBrush(gl, Color.FromArgb(55, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical)) g.FillRectangle(lg, gl);
            g.ResetClip();
            GoldRim(g, path, r, 5);
            // inner thin line
            var ri = RectangleF.Inflate(r, -8, -8);
            using (var ip = RoundRect(ri, 9)) using (var pen = new Pen(Color.FromArgb(110, 255, 226, 150), 1.5f)) g.DrawPath(pen, ip);
        }
        if (ornaments)
        {
            OrnamentStar(g, r.X, r.Y, 20);
            OrnamentStar(g, r.Right, r.Y, 20);
            OrnamentStar(g, r.X, r.Bottom, 20);
            OrnamentStar(g, r.Right, r.Bottom, 20);
            OrnamentStar(g, r.X + r.Width / 2, r.Y, 26);
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }

    static void OrnamentStar(Graphics g, float cx, float cy, float rad)
    {
        using (var gp = Star4(cx, cy, rad * 1.5f, 0.3f)) Glow(g, gp, C("ffd36b"), 10, 5, 120);
        using (var s = Star4(cx, cy, rad, 0.32f))
        {
            using (var pen = new Pen(C("2a1606"), 3) { LineJoin = LineJoin.Round }) g.DrawPath(pen, s);
            using (var lg = new LinearGradientBrush(new RectangleF(cx - rad, cy - rad, rad * 2, rad * 2), C("fff6c8"), C("c98a1b"), LinearGradientMode.ForwardDiagonal)) g.FillPath(lg, s);
        }
        using (var s2 = Star4(cx, cy, rad * 0.45f, 0.35f)) using (var br = new SolidBrush(C("8fd8ff"))) g.FillPath(br, s2);
    }

    // Rounded square button / slot.
    public static void Square(string outPath, int size, string c1, string c2, string glowHex, float radius)
    {
        Graphics g; var b = NewBmp(size, size, out g);
        float pad = size * 0.12f;
        var r = new RectangleF(pad, pad, size - pad * 2, size - pad * 2);
        using (var path = RoundRect(r, radius))
        {
            Glow(g, path, C(glowHex), size * 0.1f, 7, 150);
            using (var lg = new LinearGradientBrush(r, C(c1), C(c2), LinearGradientMode.Vertical)) g.FillPath(lg, path);
            g.SetClip(path);
            var gl = new RectangleF(r.X + 4, r.Y + 3, r.Width - 8, r.Height * 0.42f);
            using (var gp = RoundRect(gl, radius * 0.7f)) using (var lg = new LinearGradientBrush(gl, Color.FromArgb(95, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical)) g.FillPath(lg, gp);
            g.ResetClip();
            GoldRim(g, path, r, size * 0.045f);
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }

    // Plus button: glossy green with white plus.
    public static void Plus(string outPath, int size)
    {
        Square(outPath + ".tmp.png", size, "5fe08a", "138a4a", "7dffb0", size * 0.2f);
        var b = new Bitmap(outPath + ".tmp.png");
        var g = Graphics.FromImage(b); g.SmoothingMode = SmoothingMode.AntiAlias;
        float c = size / 2f, L = size * 0.2f, T = size * 0.075f;
        using (var p = new GraphicsPath())
        {
            p.AddRectangle(new RectangleF(c - L, c - T, L * 2, T * 2));
            p.AddRectangle(new RectangleF(c - T, c - L, T * 2, L * 2));
            p.FillMode = FillMode.Winding;
            using (var pen = new Pen(C("0b4a26"), 4) { LineJoin = LineJoin.Round }) g.DrawPath(pen, p);
            using (var br = new SolidBrush(Color.White)) g.FillPath(br, p);
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
        System.IO.File.Delete(outPath + ".tmp.png");
    }

    // Progress bar trough (empty) and fill (gold gradient with sparkles).
    public static void BarFill(string outPath, int w, int h)
    {
        Graphics g; var b = NewBmp(w, h, out g);
        var r = new RectangleF(2, 2, w - 4, h - 4);
        using (var path = RoundRect(r, r.Height / 2))
        {
            var cb = new ColorBlend(3) { Colors = new[] { C("ff9b2f"), C("ffd257"), C("fff1a8") }, Positions = new[] { 0f, 0.7f, 1f } };
            using (var lg = new LinearGradientBrush(r, C("ff9b2f"), C("fff1a8"), LinearGradientMode.Horizontal) { InterpolationColors = cb }) g.FillPath(lg, path);
            g.SetClip(path);
            var gl = new RectangleF(r.X, r.Y, r.Width, r.Height * 0.45f);
            using (var lg = new LinearGradientBrush(gl, Color.FromArgb(150, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical)) g.FillRectangle(lg, gl);
            Bokeh(g, r, 30, Color.White, 1, 3, true);
            g.ResetClip();
        }
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }

    public static void Sparkles(string outPath, int size)
    {
        Graphics g; var b = NewBmp(size, size, out g);
        Sparkle(g, size / 2f, size / 2f, size * 0.32f, C("fff3c0"));
        g.Dispose(); b.Save(outPath, ImageFormat.Png); b.Dispose();
    }
}
