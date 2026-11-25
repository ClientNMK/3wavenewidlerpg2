using System;
using System.Drawing;
using System.Collections.Generic;

public static class HoodTool
{
    static int D(Color a, Color b) { return Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B); }
    static string Hex(Color c) { return string.Format("{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B); }

    // Flood fill from seed over fill colors; then add outline-colored pixels adjacent to the fill.
    public static string Extract(string src, int seedX, int seedY, string[] fillHex, string[] outlineHex, int maxY, string outPath)
    {
        var b = new Bitmap(src);
        var fill = new HashSet<string>(fillHex);
        var outl = new HashSet<string>(outlineHex);
        int w = b.Width, h = b.Height;
        var m = new bool[w, h];
        var q = new Queue<Point>();
        q.Enqueue(new Point(seedX, seedY)); m[seedX, seedY] = true;
        int n = 0;
        while (q.Count > 0)
        {
            var p = q.Dequeue(); n++;
            foreach (var d in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
            {
                int x = p.X + d.X, y = p.Y + d.Y;
                if (x < 0 || y < 0 || x >= w || y >= h || y > maxY || m[x, y]) continue;
                var c = b.GetPixel(x, y);
                if (c.A < 20) continue;
                if (fill.Contains(Hex(c))) { m[x, y] = true; q.Enqueue(new Point(x, y)); }
            }
        }
        // outline ring (one pass, 8-neighbour)
        var add = new List<Point>();
        for (int y = 0; y < h && y <= maxY; y++)
            for (int x = 0; x < w; x++)
            {
                if (m[x, y]) continue;
                var c = b.GetPixel(x, y);
                if (c.A < 20 || !outl.Contains(Hex(c))) continue;
                bool adj = false;
                for (int dy = -1; dy <= 1 && !adj; dy++)
                    for (int dx = -1; dx <= 1 && !adj; dx++)
                    {
                        int xx = x + dx, yy = y + dy;
                        if (xx >= 0 && yy >= 0 && xx < w && yy < h && m[xx, yy]) adj = true;
                    }
                if (adj) add.Add(new Point(x, y));
            }
        foreach (var p in add) m[p.X, p.Y] = true;
        // fill small holes enclosed horizontally (eye highlights etc. are already fill colors)
        var o = new Bitmap(w, h);
        int cnt = 0;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (m[x, y]) { o.SetPixel(x, y, b.GetPixel(x, y)); cnt++; }
        o.Save(outPath); o.Dispose(); b.Dispose();
        return "hood px: " + cnt;
    }

    // Add dark outline pixels adjacent to the layer and fill holes (pixels with >=3 masked 4-neighbours).
    public static string Grow(string src, string layerPath, int maxY, string outPath)
    {
        var b = new Bitmap(src); var l = new Bitmap(layerPath);
        int w = b.Width, h = b.Height;
        var m = new bool[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) m[x, y] = l.GetPixel(x, y).A > 20;
        l.Dispose();
        int added = 0;
        for (int pass = 0; pass < 2; pass++)
        {
            var add = new List<Point>();
            for (int y = 1; y < h - 1 && y <= maxY; y++)
                for (int x = 1; x < w - 1; x++)
                {
                    if (m[x, y]) continue;
                    var c = b.GetPixel(x, y);
                    if (c.A < 20) continue;
                    int n4 = (m[x - 1, y] ? 1 : 0) + (m[x + 1, y] ? 1 : 0) + (m[x, y - 1] ? 1 : 0) + (m[x, y + 1] ? 1 : 0);
                    bool dark = c.R + c.G + c.B < 200;
                    if (n4 >= 3 || (dark && n4 >= 1)) add.Add(new Point(x, y));
                }
            foreach (var p in add) m[p.X, p.Y] = true;
            added += add.Count;
        }
        var o = new Bitmap(w, h);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (m[x, y]) o.SetPixel(x, y, b.GetPixel(x, y));
        o.Save(outPath); o.Dispose(); b.Dispose();
        return "added: " + added;
    }

    // Find shift (sx,sy) so template region [rx0..rx1, ry0..ry1] best matches frame.
    public static int[] Register(string tplPath, int rx0, int ry0, int rx1, int ry1, string framePath, int sx0, int sx1, int sy0, int sy1)
    {
        var t = new Bitmap(tplPath); var f = new Bitmap(framePath);
        int best = int.MinValue, bx = 0, by = 0;
        for (int sy = sy0; sy <= sy1; sy++)
            for (int sx = sx0; sx <= sx1; sx++)
            {
                int s = 0;
                for (int y = ry0; y <= ry1; y++)
                    for (int x = rx0; x <= rx1; x++)
                    {
                        var p = t.GetPixel(x, y);
                        int X = x + sx, Y = y + sy;
                        bool inF = X >= 0 && Y >= 0 && X < f.Width && Y < f.Height;
                        var c = inF ? f.GetPixel(X, Y) : Color.Transparent;
                        bool pa = p.A > 20, ca = c.A > 20;
                        if (pa && ca) s += D(p, c) < 40 ? 2 : 0;
                        else if (pa != ca) s -= 1;
                    }
                if (s > best) { best = s; bx = sx; by = sy; }
            }
        t.Dispose(); f.Dispose();
        return new[] { bx, by, best };
    }

    public static void Trim(string layerPath, int maxY, string outPath)
    {
        var l = new Bitmap(layerPath);
        var o = new Bitmap(l.Width, l.Height);
        for (int y = 0; y <= maxY && y < l.Height; y++) for (int x = 0; x < l.Width; x++) o.SetPixel(x, y, l.GetPixel(x, y));
        l.Dispose(); o.Save(outPath); o.Dispose();
    }

    // Erase base pixels outside the design's head silhouette (dilated), only for design rows <= cutoff, then draw hood.
    public static int CompositeClean(string basePath, string hoodPath, string designPath, int ox, int oy, int cutoff, int dilate, string outPath)
    {
        var b = new Bitmap(basePath); var h = new Bitmap(hoodPath); var d = new Bitmap(designPath);
        int dw = d.Width, dh = d.Height;
        var sil = new bool[dw, dh];
        for (int y = 0; y < dh; y++) for (int x = 0; x < dw; x++) sil[x, y] = d.GetPixel(x, y).A > 20;
        var dil = new bool[dw, dh];
        for (int y = 0; y < dh; y++) for (int x = 0; x < dw; x++)
        {
            bool v = false;
            for (int yy = y - dilate; yy <= y + dilate && !v; yy++)
                for (int xx = x - dilate; xx <= x + dilate && !v; xx++)
                    if (xx >= 0 && yy >= 0 && xx < dw && yy < dh && sil[xx, yy]) v = true;
            dil[x, y] = v;
        }
        d.Dispose();
        var c = new Bitmap(b.Width, b.Height);
        int erased = 0;
        for (int y = 0; y < b.Height; y++) for (int x = 0; x < b.Width; x++)
        {
            var p = b.GetPixel(x, y);
            if (p.A > 0)
            {
                int dx = x - ox, dy = y - oy;
                if (dy <= cutoff)
                {
                    bool inside = dx >= 0 && dy >= 0 && dx < dw && dy < dh && dil[dx, dy];
                    if (!inside) { erased++; continue; }
                }
            }
            c.SetPixel(x, y, p);
        }
        b.Dispose();
        using (var g = Graphics.FromImage(c)) g.DrawImageUnscaled(h, ox, oy);
        h.Dispose();
        c.Save(outPath); c.Dispose();
        return erased;
    }

    // Below cutoff (cutoff+1..cutoff+extra design rows): erase hair-colored pixels outside the design silhouette,
    // then erase dark outline pixels there that no longer touch any kept non-dark pixel outside the hair set.
    public static string HoodPath = null;
    public static int HairBelow(string inPath, string designPath, int ox, int oy, int cutoff, int extra, string[] hairHex, string outPath)
    {
        var b = new Bitmap(inPath); var d = new Bitmap(designPath);
        int w = b.Width, h = b.Height;
        var px = new Color[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) px[x, y] = b.GetPixel(x, y);
        b.Dispose();
        var hair = new HashSet<string>(hairHex);
        Func<int, int, bool> outside = (x, y) => {
            int dx = x - ox, dy = y - oy;
            if (dy <= cutoff || dy > cutoff + extra) return false;
            if (dx < 0 || dy < 0 || dx >= d.Width || dy >= d.Height) return true;
            return d.GetPixel(dx, dy).A <= 20;
        };
        int erased = 0;
        var gone = new bool[w, h];
        var cand = new bool[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            var c = px[x, y];
            if (c.A <= 20 || !outside(x, y)) continue;
            bool hairish = c.R >= c.G && c.G >= c.B && c.R - c.B >= 25 && c.R - c.B <= 60 && c.R <= 235;
            if (c.R + c.G + c.B > 250 && (hair.Contains(Hex(c)) || hairish)) cand[x, y] = true;
        }
        // hood footprint
        var hood = new bool[w, h];
        if (HoodPath != null)
        {
            var hb = new Bitmap(HoodPath);
            for (int y = 0; y < hb.Height; y++) for (int x = 0; x < hb.Width; x++)
            {
                int X = x + ox, Y = y + oy;
                if (X >= 0 && Y >= 0 && X < w && Y < h && hb.GetPixel(x, y).A > 20) hood[X, Y] = true;
            }
            hb.Dispose();
        }
        // keep only candidate components (8-conn, may pass through dark pixels outside silhouette) touching the hood
        var seen = new bool[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (!cand[x, y] || seen[x, y]) continue;
            var list = new List<Point>(); var q = new Queue<Point>();
            q.Enqueue(new Point(x, y)); seen[x, y] = true; bool touch = false;
            while (q.Count > 0)
            {
                var p = q.Dequeue(); list.Add(p);
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    int xx = p.X + dx, yy = p.Y + dy;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h) continue;
                    if (hood[xx, yy]) touch = true;
                    if (seen[xx, yy] || !cand[xx, yy]) continue;
                    seen[xx, yy] = true; q.Enqueue(new Point(xx, yy));
                }
            }
            if (touch || HoodPath == null) foreach (var p in list) { gone[p.X, p.Y] = true; erased++; }
        }
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (gone[x, y]) px[x, y] = Color.Transparent;
        for (int pass = 0; pass < 2; pass++)
        {
            var kill = new List<Point>();
            for (int y = 1; y < h - 1; y++) for (int x = 1; x < w - 1; x++)
            {
                var c = px[x, y];
                if (c.A <= 20 || c.R + c.G + c.B > 250 || !outside(x, y)) continue;
                bool nearGone = false, nearSolid = false;
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (gone[x + dx, y + dy]) nearGone = true;
                    var n = px[x + dx, y + dy];
                    bool nh = n.R >= n.G && n.G >= n.B && n.R - n.B >= 25 && n.R - n.B <= 60 && n.R <= 235;
                    if (n.A > 20 && n.R + n.G + n.B > 250 && !hair.Contains(Hex(n)) && !nh) nearSolid = true;
                }
                if (nearGone && !nearSolid) kill.Add(new Point(x, y));
            }
            foreach (var p in kill) { px[p.X, p.Y] = Color.Transparent; gone[p.X, p.Y] = true; erased++; }
        }
        d.Dispose();
        var o = new Bitmap(w, h);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (px[x, y].A > 0) o.SetPixel(x, y, px[x, y]);
        o.Save(outPath); o.Dispose();
        return erased;
    }

    // Erase thin dark strands: 4-connected groups of dark pixels outside the design silhouette (design rows cutoff+1..cutoff+extra),
    // excluding hood pixels, that 4-touch no opaque pixel other than hood or other such dark-outside pixels.
    public static int DarkStrands(string inPath, string designPath, string hoodPath, int ox, int oy, int cutoff, int extra, int maxSize, string outPath)
    {
        var b = new Bitmap(inPath); var d = new Bitmap(designPath); var hb = new Bitmap(hoodPath);
        int w = b.Width, h = b.Height;
        var px = new Color[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) px[x, y] = b.GetPixel(x, y);
        b.Dispose();
        var hood = new bool[w, h]; var cand = new bool[w, h];
        for (int y = 0; y < hb.Height; y++) for (int x = 0; x < hb.Width; x++)
        {
            int X = x + ox, Y = y + oy;
            if (X >= 0 && Y >= 0 && X < w && Y < h && hb.GetPixel(x, y).A > 20) hood[X, Y] = true;
        }
        hb.Dispose();
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            var c = px[x, y];
            bool hairish = c.R >= c.G && c.G >= c.B && c.R - c.B >= 25 && c.R - c.B <= 60 && c.R <= 235;
            if (c.A <= 20 || hood[x, y] || (c.R + c.G + c.B >= 250 && !hairish)) continue;
            int dx = x - ox, dy = y - oy;
            if (dy <= cutoff - 2 || dy > cutoff + extra) continue;
            bool outside = dx < 0 || dy < 0 || dx >= d.Width || dy >= d.Height || d.GetPixel(dx, dy).A <= 20;
            if (outside) cand[x, y] = true;
        }
        d.Dispose();
        var seen = new bool[w, h]; int erased = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (!cand[x, y] || seen[x, y]) continue;
            var list = new List<Point>(); var q = new Queue<Point>(); bool anchored = false;
            q.Enqueue(new Point(x, y)); seen[x, y] = true;
            while (q.Count > 0)
            {
                var p = q.Dequeue(); list.Add(p);
                foreach (var o in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    int xx = p.X + o.X, yy = p.Y + o.Y;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h) continue;
                    if (cand[xx, yy]) { if (!seen[xx, yy]) { seen[xx, yy] = true; q.Enqueue(new Point(xx, yy)); } continue; }
                    if (px[xx, yy].A > 20 && !hood[xx, yy]) anchored = true;
                }
            }
            if (!anchored && list.Count <= maxSize) { foreach (var p in list) px[p.X, p.Y] = Color.Transparent; erased += list.Count; }
        }
        var outB = new Bitmap(w, h);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (px[x, y].A > 0) outB.SetPixel(x, y, px[x, y]);
        outB.Save(outPath); outB.Dispose();
        return erased;
    }

    // Restore base pixels lying to the right of the design silhouette's right edge (front of the face: dagger, hand),
    // for design rows y0..y1. Returns restored count.
    public static int RestoreFront(string basePath, string inPath, string designPath, int ox, int oy, int y0, int y1, string outPath)
    {
        var b = new Bitmap(basePath); var c = new Bitmap(inPath); var d = new Bitmap(designPath);
        int w = c.Width, h = c.Height;
        var o = new Bitmap(w, h);
        using (var g = Graphics.FromImage(o)) g.DrawImageUnscaled(c, 0, 0);
        c.Dispose();
        int n = 0;
        for (int dy = y0; dy <= y1 && dy < d.Height; dy++)
        {
            int right = -1;
            for (int dx = d.Width - 1; dx >= 0; dx--) if (d.GetPixel(dx, dy).A > 20) { right = dx; break; }
            if (right < 0) continue;
            int Y = dy + oy; if (Y < 0 || Y >= h) continue;
            for (int X = right + ox + 1; X < w; X++)
            {
                if (X < 0) continue;
                var p = b.GetPixel(X, Y);
                if (p.A > 20 && o.GetPixel(X, Y).A <= 20) { o.SetPixel(X, Y, p); n++; }
            }
        }
        b.Dispose(); d.Dispose();
        o.Save(outPath); o.Dispose();
        return n;
    }

    // Rotated version of CompositeClean: base pixel -> design-rotated coords -> inverse-rotate around pivot -> design coords.
    public static int CompositeCleanRot(string basePath, string hoodRotPath, string designPath, int ox, int oy, double deg, double pcx, double pcy, int cutoff, string outPath)
    {
        var b = new Bitmap(basePath); var h = new Bitmap(hoodRotPath); var d = new Bitmap(designPath);
        double r = -deg * Math.PI / 180.0, cs = Math.Cos(r), sn = Math.Sin(r);
        var c = new Bitmap(b.Width, b.Height); int erased = 0;
        for (int y = 0; y < b.Height; y++) for (int x = 0; x < b.Width; x++)
        {
            var p = b.GetPixel(x, y); if (p.A <= 0) continue;
            double dx = (x - ox) - pcx, dy = (y - oy) - pcy;
            int u = (int)Math.Round(pcx + dx * cs - dy * sn), v = (int)Math.Round(pcy + dx * sn + dy * cs);
            if (v <= cutoff)
            {
                bool inside = u >= 0 && v >= 0 && u < d.Width && v < d.Height && d.GetPixel(u, v).A > 20;
                if (!inside) { erased++; continue; }
            }
            c.SetPixel(x, y, p);
        }
        b.Dispose(); d.Dispose();
        using (var g = Graphics.FromImage(c)) g.DrawImageUnscaled(h, ox, oy);
        h.Dispose(); c.Save(outPath); c.Dispose();
        return erased;
    }

    // Nearest-neighbour rotate a WxH image by deg (clockwise positive) around (cx,cy); same size output.
    public static void Rotate(string inPath, double deg, double cx, double cy, string outPath)
    {
        var b = new Bitmap(inPath); int w = b.Width, h = b.Height;
        var o = new Bitmap(w, h);
        double r = -deg * Math.PI / 180.0, cs = Math.Cos(r), sn = Math.Sin(r);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            double dx = x - cx, dy = y - cy;
            int sx = (int)Math.Round(cx + dx * cs - dy * sn), sy = (int)Math.Round(cy + dx * sn + dy * cs);
            if (sx >= 0 && sy >= 0 && sx < w && sy < h) { var p = b.GetPixel(sx, sy); if (p.A > 20) o.SetPixel(x, y, p); }
        }
        b.Dispose(); o.Save(outPath); o.Dispose();
    }

    public static int Step = 1;
    // Masked registration: score only over template-opaque pixels. Returns sx, sy, score.
    public static int[] RegisterMasked(string tplPath, string framePath, int sx0, int sx1, int sy0, int sy1)
    {
        var t = new Bitmap(tplPath); var f = new Bitmap(framePath);
        var pts = new List<Tuple<int, int, Color>>();
        for (int y = 0; y < t.Height; y++) for (int x = 0; x < t.Width; x++) { var p = t.GetPixel(x, y); if (p.A > 20) pts.Add(Tuple.Create(x, y, p)); }
        int best = int.MinValue, bx = 0, by = 0;
        for (int sy = sy0; sy <= sy1; sy += Step) for (int sx = sx0; sx <= sx1; sx += Step)
        {
            int s = 0;
            foreach (var q in pts)
            {
                int X = q.Item1 + sx, Y = q.Item2 + sy;
                if (X < 0 || Y < 0 || X >= f.Width || Y >= f.Height) { s -= 1; continue; }
                var c = f.GetPixel(X, Y);
                if (c.A <= 20) s -= 1; else if (D(q.Item3, c) < 40) s += 2;
            }
            if (s > best) { best = s; bx = sx; by = sy; }
        }
        t.Dispose(); f.Dispose();
        return new[] { bx, by, best, pts.Count };
    }

    // Copy rows [y0..y1] of src into a new canvas of size cw x ch at offset (ox,oy).
    public static void CropRowsTo(string src, int y0, int y1, int cw, int ch, int ox, int oy, string outPath)
    {
        var b = new Bitmap(src); var o = new Bitmap(cw, ch);
        for (int y = y0; y <= y1 && y < b.Height; y++) for (int x = 0; x < b.Width; x++)
        {
            var p = b.GetPixel(x, y); int X = x + ox, Y = y + oy;
            if (p.A > 20 && X >= 0 && Y >= 0 && X < cw && Y < ch) o.SetPixel(X, Y, p);
        }
        b.Dispose(); o.Save(outPath); o.Dispose();
    }

    // Remove 8-connected opaque components smaller than minSize.
    public static string RemoveSpecks(string inPath, int minSize, string outPath)
    {
        var b = new Bitmap(inPath);
        int w = b.Width, h = b.Height;
        var px = new Color[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) px[x, y] = b.GetPixel(x, y);
        b.Dispose();
        var lab = new int[w, h];
        int id = 0; int removed = 0, comps = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (lab[x, y] != 0 || px[x, y].A <= 20) continue;
            id++; var list = new List<Point>(); var q = new Queue<Point>();
            q.Enqueue(new Point(x, y)); lab[x, y] = id;
            while (q.Count > 0)
            {
                var p = q.Dequeue(); list.Add(p);
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx != 0 && dy != 0) continue; // 4-connectivity
                    int xx = p.X + dx, yy = p.Y + dy;
                    if (xx < 0 || yy < 0 || xx >= w || yy >= h || lab[xx, yy] != 0 || px[xx, yy].A <= 20) continue;
                    lab[xx, yy] = id; q.Enqueue(new Point(xx, yy));
                }
            }
            comps++;
            if (list.Count < minSize) { foreach (var p in list) px[p.X, p.Y] = Color.Transparent; removed += list.Count; }
        }
        var o = new Bitmap(w, h);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) if (px[x, y].A > 0) o.SetPixel(x, y, px[x, y]);
        o.Save(outPath); o.Dispose();
        return "comps " + comps + " removed px " + removed;
    }

    public static void Composite(string basePath, string hoodPath, int ox, int oy, string outPath)
    {
        var b = new Bitmap(basePath); var h = new Bitmap(hoodPath);
        var c = new Bitmap(b.Width, b.Height);
        using (var g = Graphics.FromImage(c)) { g.DrawImageUnscaled(b, 0, 0); g.DrawImageUnscaled(h, ox, oy); }
        b.Dispose(); h.Dispose();
        c.Save(outPath); c.Dispose();
    }
}

