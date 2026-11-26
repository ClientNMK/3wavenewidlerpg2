using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

public static class TongueFix
{
    // Optional custom tongue range (used when Custom = true), e.g. for the shell costume whose cap stripes are coral.
    public static bool Custom = false;
    public static double CHMax = 12, CSMin = 0.6, CLMin = 0.48, CLMax = 0.62;

    static bool IsTongue(Color c)
    {
        if (c.A == 0) return false;
        float h = c.GetHue(), s = c.GetSaturation(), l = c.GetBrightness();
        if (Custom) return (h <= CHMax || h >= 350) && s >= CSMin && l >= CLMin && l <= CLMax;
        bool tongue = (h <= 16 || h >= 350) && s >= 0.70 && l >= 0.52;  // bright red / pink tongue
        bool crumbs = h >= 19 && h <= 28 && s >= 0.70 && l >= 0.42 && l <= 0.56; // orange crumbs
        return tongue || crumbs;
    }

    // Most common dark colour (face shadow) in a frame.
    public static string FaceColor(string path)
    {
        var cnt = new Dictionary<int, int>();
        using (var b = new Bitmap(path))
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    var c = b.GetPixel(x, y);
                    if (c.A < 200 || c.GetBrightness() > 0.25) continue;
                    int k = c.ToArgb(); int v; cnt.TryGetValue(k, out v); cnt[k] = v + 1;
                }
        int best = 0, bk = 0; foreach (var kv in cnt) if (kv.Value > best) { best = kv.Value; bk = kv.Key; }
        return ColorTranslator.ToHtml(Color.FromArgb(bk));
    }

    // Clear tongue/orange pixels; cleared pixels enclosed by the body (not reachable from the border
    // through transparency) are filled with the face colour instead. Returns cleared count.
    public static int Fix(string src, string dst, string faceHex)
    {
        Color face = ColorTranslator.FromHtml(faceHex);
        using (var b = new Bitmap(src))
        using (var o = new Bitmap(b))
        {
            int W = b.Width, H = b.Height, n = 0;
            var cleared = new bool[W, H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    if (IsTongue(b.GetPixel(x, y))) { o.SetPixel(x, y, Color.Transparent); cleared[x, y] = true; n++; }
            var outside = new bool[W, H]; var st = new Stack<Point>();
            for (int x = 0; x < W; x++) { st.Push(new Point(x, 0)); st.Push(new Point(x, H - 1)); }
            for (int y = 0; y < H; y++) { st.Push(new Point(0, y)); st.Push(new Point(W - 1, y)); }
            while (st.Count > 0)
            {
                var p = st.Pop();
                if (p.X < 0 || p.Y < 0 || p.X >= W || p.Y >= H || outside[p.X, p.Y]) continue;
                if (o.GetPixel(p.X, p.Y).A != 0) continue;
                outside[p.X, p.Y] = true;
                st.Push(new Point(p.X + 1, p.Y)); st.Push(new Point(p.X - 1, p.Y)); st.Push(new Point(p.X, p.Y + 1)); st.Push(new Point(p.X, p.Y - 1));
            }
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    if (cleared[x, y] && !outside[x, y]) o.SetPixel(x, y, face);
            // remove leftover dark tongue outline: dark pixels next to the cleared-outside area with >=2 transparent neighbours
            for (int pass = 0; pass < 3; pass++)
            {
                var kill = new List<Point>();
                for (int y = 1; y < H - 1; y++)
                    for (int x = 1; x < W - 1; x++)
                    {
                        var c = o.GetPixel(x, y);
                        if (c.A == 0 || c.GetBrightness() > 0.32) continue;
                        bool nearCleared = false; int tr = 0;
                        for (int dy = -2; dy <= 2; dy++)
                            for (int dx = -2; dx <= 2; dx++)
                            {
                                int nx = x + dx, ny = y + dy;
                                if (nx < 0 || ny < 0 || nx >= W || ny >= H) continue;
                                if (cleared[nx, ny] && outside[nx, ny]) nearCleared = true;
                            }
                        if (!nearCleared) continue;
                        if (o.GetPixel(x - 1, y).A == 0) tr++; if (o.GetPixel(x + 1, y).A == 0) tr++;
                        if (o.GetPixel(x, y - 1).A == 0) tr++; if (o.GetPixel(x, y + 1).A == 0) tr++;
                        if (tr >= 2) kill.Add(new Point(x, y));
                    }
                foreach (var p in kill) o.SetPixel(p.X, p.Y, Color.Transparent);
            }
            o.Save(dst, ImageFormat.Png);
            return n;
        }
    }
}
