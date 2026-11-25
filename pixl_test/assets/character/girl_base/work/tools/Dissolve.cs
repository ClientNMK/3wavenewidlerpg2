using System;
using System.Drawing;
using System.Collections.Generic;

// Pixel dissolve: the sprite breaks apart into dots that drift up and fade (by shrinking count), head first.
public static class Dissolve
{
    class P { public double x, y, vx, vy; public int born, life; public Color c; }

    static Color Lighten(Color c, double t)
    {
        return Color.FromArgb(255, (int)(c.R + (255 - c.R) * t), (int)(c.G + (255 - c.G) * t), (int)(c.B + (248 - c.B) * t));
    }

    // Materialize over the first n frames of an existing animation: pixels appear bottom->top (with jitter),
    // pixels just appearing glow, and dots converge in from above toward their spots. Frames >= n are copied unchanged.
    public static string MaterializeInto(string[] framePaths, int n, string outDir, int seed)
    {
        var rnd = new Random(seed);
        var first = new Bitmap(framePaths[0]); int w = first.Width, h = first.Height; first.Dispose();
        var thr = new double[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) thr[x, y] = (1.0 - (double)y / h) * 0.75 + rnd.NextDouble() * 0.35; // bottom first
        int dots = 0;
        for (int f = 0; f < framePaths.Length; f++)
        {
            var src = new Bitmap(framePaths[f]);
            var o = new Bitmap(w, h);
            double prog = f >= n ? 2.0 : (double)(f + 1) / n * 1.1; // reveal level
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
            {
                var c = src.GetPixel(x, y); if (c.A <= 20) continue;
                if (f >= n) { o.SetPixel(x, y, c); continue; }
                double t = thr[x, y];
                if (t <= prog - 0.18) o.SetPixel(x, y, c);
                else if (t <= prog) o.SetPixel(x, y, Lighten(c, 0.55));
                else if (rnd.NextDouble() < 0.10)
                {
                    // converging dot: drawn above its target, closer as progress grows
                    double k = Math.Max(0, t - prog);
                    int Y = y - (int)Math.Round(6 + k * 40), X = x + (int)Math.Round((rnd.NextDouble() - 0.5) * k * 16);
                    if (X >= 0 && Y >= 0 && X < w && Y < h) { o.SetPixel(X, Y, Lighten(c, 0.6)); dots++; }
                }
            }
            src.Dispose();
            o.Save(System.IO.Path.Combine(outDir, string.Format("frame_{0:D3}.png", f)));
            o.Dispose();
        }
        return "frames " + framePaths.Length + " dots " + dots;
    }

    // frames: number of output frames. breakStart/breakEnd: frame range over which pixels break (top->bottom sweep).
    public static string Make(string srcPath, string outDir, int frames, int breakStart, int breakEnd, int seed)
    {
        var src = new Bitmap(srcPath); int w = src.Width, h = src.Height;
        var rnd = new Random(seed);
        int top = h, bot = 0;
        var pix = new List<Tuple<int, int, Color>>();
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            var c = src.GetPixel(x, y);
            if (c.A > 20) { pix.Add(Tuple.Create(x, y, c)); top = Math.Min(top, y); bot = Math.Max(bot, y); }
        }
        src.Dispose();
        // break time per pixel: top-to-bottom sweep with jitter
        var tBreak = new Dictionary<int, double>();
        var parts = new List<P>();
        for (int i = 0; i < pix.Count; i++)
        {
            var p = pix[i];
            double f = (double)(p.Item2 - top) / Math.Max(1, bot - top);
            double t = breakStart + f * (breakEnd - breakStart) + (rnd.NextDouble() - 0.5) * 2.2;
            tBreak[i] = t;
            if (rnd.NextDouble() < 0.26)
            {
                parts.Add(new P {
                    x = p.Item1, y = p.Item2,
                    vx = (rnd.NextDouble() - 0.3) * 2.4,          // drift right/back... forward
                    vy = -(1.4 + rnd.NextDouble() * 2.4),       // up
                    born = (int)Math.Ceiling(t), life = 3 + rnd.Next(4),
                    c = Lighten(p.Item3, 0.35 + rnd.NextDouble() * 0.4)
                });
            }
        }
        for (int fr = 0; fr < frames; fr++)
        {
            var o = new Bitmap(w, h);
            // body: pixels not yet broken; pixels about to break (within 1.2 frames) glow
            for (int i = 0; i < pix.Count; i++)
            {
                var p = pix[i]; double t = tBreak[i];
                if (fr >= t) continue;
                var c = p.Item3;
                if (t - fr < 1.2) c = Lighten(c, 0.55);
                else if (fr >= 1 && fr <= 2 && breakStart >= 2) c = Lighten(c, fr == 1 ? 0.15 : 0.3); // charge-up flash
                o.SetPixel(p.Item1, p.Item2, c);
            }
            // particles
            foreach (var q in parts)
            {
                int age = fr - q.born;
                if (age < 0 || age >= q.life) continue;
                int X = (int)Math.Round(q.x + q.vx * age), Y = (int)Math.Round(q.y + q.vy * age - 0.25 * age * age);
                if (X < 0 || Y < 0 || X >= w || Y >= h) continue;
                var c = age >= q.life - 1 ? Lighten(q.c, 0.5) : q.c;
                o.SetPixel(X, Y, c);
            }
            o.Save(System.IO.Path.Combine(outDir, string.Format("frame_{0:D3}.png", fr)));
            o.Dispose();
        }
        return "pixels " + pix.Count + " particles " + parts.Count;
    }
}

