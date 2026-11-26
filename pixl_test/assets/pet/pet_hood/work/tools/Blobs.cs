using System;
using System.Collections.Generic;
using System.Drawing;

public static class Blobs
{
    // mode 0: purple (h 235-290, l>=0.42); mode 1: white-ish (l>=0.80)
    static bool Match(Color c, int mode)
    {
        if (c.A == 0) return false;
        double h = c.GetHue(), l = c.GetBrightness();
        if (mode == 0) return h >= 235 && h <= 290 && l >= 0.42;
        return l >= 0.80;
    }

    // Lists components: "x0,y0,x1,y1,size"
    public static List<string> List(string path, int mode)
    {
        var res = new List<string>();
        using (var b = new Bitmap(path))
            foreach (var comp in Comps(b, mode))
                res.Add(Desc(comp));
        return res;
    }

    // Recolor components with size>=minSize, w>=minW, h>=minH and top y >= minTop to 'to'.
    public static int Fill(string path, int mode, int minSize, int minW, int minH, int minTop, string toHex)
    {
        Color to = ColorTranslator.FromHtml(toHex);
        int n = 0;
        Bitmap outB;
        using (var b = new Bitmap(path))
        {
            outB = new Bitmap(b);
            foreach (var comp in Comps(b, mode))
            {
                int x0 = 999, y0 = 999, x1 = -1, y1 = -1;
                foreach (var p in comp) { x0 = Math.Min(x0, p.X); y0 = Math.Min(y0, p.Y); x1 = Math.Max(x1, p.X); y1 = Math.Max(y1, p.Y); }
                if (comp.Count >= minSize && x1 - x0 + 1 >= minW && y1 - y0 + 1 >= minH && y0 >= minTop)
                    foreach (var p in comp) { outB.SetPixel(p.X, p.Y, to); n++; }
            }
        }
        outB.Save(path); outB.Dispose();
        return n;
    }

    static string Desc(List<Point> comp)
    {
        int x0 = 999, y0 = 999, x1 = -1, y1 = -1;
        foreach (var p in comp) { x0 = Math.Min(x0, p.X); y0 = Math.Min(y0, p.Y); x1 = Math.Max(x1, p.X); y1 = Math.Max(y1, p.Y); }
        return x0 + "," + y0 + "," + x1 + "," + y1 + "," + comp.Count;
    }

    static List<List<Point>> Comps(Bitmap b, int mode)
    {
        int W = b.Width, H = b.Height;
        var seen = new bool[W, H];
        var all = new List<List<Point>>();
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (seen[x, y] || !Match(b.GetPixel(x, y), mode)) continue;
                var comp = new List<Point>(); var st = new Stack<Point>(); st.Push(new Point(x, y)); seen[x, y] = true;
                while (st.Count > 0)
                {
                    var p = st.Pop(); comp.Add(p);
                    int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
                    for (int k = 0; k < 4; k++)
                    {
                        int nx = p.X + dx[k], ny = p.Y + dy[k];
                        if (nx < 0 || ny < 0 || nx >= W || ny >= H || seen[nx, ny]) continue;
                        if (!Match(b.GetPixel(nx, ny), mode)) continue;
                        seen[nx, ny] = true; st.Push(new Point(nx, ny));
                    }
                }
                all.Add(comp);
            }
        return all;
    }
}
