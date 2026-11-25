using System;
using System.Drawing;
using System.Collections.Generic;

public static class Fx
{
    static bool White(Color p) { return p.A > 20 && p.R > 225 && p.G > 215 && p.B > 195; }
    static bool Light(Color p) { return p.A > 20 && p.R > 205 && p.G > 195 && p.B > 175 && (Math.Max(p.R, Math.Max(p.G, p.B)) - Math.Min(p.R, Math.Min(p.G, p.B))) < 45; }

    // Remove white streak effects: 8-connected components of white pixels whose bbox height or width >= minSpan
    // and height >= minH. Then remove light fringe pixels adjacent to removed ones (2 passes).
    // Holes (removed pixel with opaque kept pixels on both left&right or up&down) are filled from the nearest kept pixel.
    public static string RemoveStreaks(string inPath, int minH, string outPath)
    {
        var b = new Bitmap(inPath); int w = b.Width, h = b.Height;
        var px = new Color[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) px[x, y] = b.GetPixel(x, y);
        b.Dispose();
        var seen = new bool[w, h]; var gone = new bool[w, h]; int removed = 0, comps = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (seen[x, y] || !White(px[x, y])) continue;
            var list = new List<Point>(); var q = new Queue<Point>(); q.Enqueue(new Point(x, y)); seen[x, y] = true;
            int x0 = x, x1 = x, y0 = y, y1 = y;
            while (q.Count > 0)
            {
                var p = q.Dequeue(); list.Add(p);
                x0 = Math.Min(x0, p.X); x1 = Math.Max(x1, p.X); y0 = Math.Min(y0, p.Y); y1 = Math.Max(y1, p.Y);
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    int xx = p.X + dx, yy = p.Y + dy;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h || seen[xx, yy] || !White(px[xx, yy])) continue;
                    seen[xx, yy] = true; q.Enqueue(new Point(xx, yy));
                }
            }
            int edge = 0;
            foreach (var p in list)
                foreach (var d in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    int xx = p.X + d.X, yy = p.Y + d.Y;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h || px[xx, yy].A <= 20) { edge++; break; }
                }
            if (y1 - y0 + 1 >= minH && edge >= 5) { comps++; foreach (var p in list) { gone[p.X, p.Y] = true; removed++; } }
        }
        for (int pass = 0; pass < 2; pass++)
        {
            var add = new List<Point>();
            for (int y = 1; y < h - 1; y++) for (int x = 1; x < w - 1; x++)
            {
                if (gone[x, y] || !Light(px[x, y])) continue;
                bool near = gone[x - 1, y] || gone[x + 1, y] || gone[x, y - 1] || gone[x, y + 1];
                if (near) add.Add(new Point(x, y));
            }
            foreach (var p in add) { gone[p.X, p.Y] = true; removed++; }
        }
        // fill holes
        int filled = 0;
        var outC = new Color[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) outC[x, y] = gone[x, y] ? Color.Transparent : px[x, y];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (!gone[x, y]) continue;
            int lx = -1, rx = -1, uy = -1, dy2 = -1;
            for (int k = x - 1; k >= 0; k--) { if (!gone[k, y]) { if (px[k, y].A > 20) lx = k; break; } }
            for (int k = x + 1; k < w; k++) { if (!gone[k, y]) { if (px[k, y].A > 20) rx = k; break; } }
            for (int k = y - 1; k >= 0; k--) { if (!gone[x, k]) { if (px[x, k].A > 20) uy = k; break; } }
            for (int k = y + 1; k < h; k++) { if (!gone[x, k]) { if (px[x, k].A > 20) dy2 = k; break; } }
            if (lx >= 0 && rx >= 0 && uy >= 0 && dy2 >= 0)
            {
                // nearest horizontal kept pixel
                outC[x, y] = (x - lx <= rx - x) ? px[lx, y] : px[rx, y]; filled++;
            }
        }
        var o = new Bitmap(w, h);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (outC[x, y].A > 0) o.SetPixel(x, y, outC[x, y]);
        o.Save(outPath); o.Dispose();
        return "streaks " + comps + " removed " + removed + " filled " + filled;
    }
}
