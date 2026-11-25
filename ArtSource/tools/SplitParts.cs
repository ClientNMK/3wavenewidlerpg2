using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

public static class SplitParts
{
    // Finds connected opaque regions (8-neighbour), merges boxes closer than `gap`, returns boxes sorted top-to-bottom, left-to-right.
    public static List<Rectangle> Find(string path, int gap, int minArea)
    {
        var bmp = new Bitmap(path);
        int w = bmp.Width, h = bmp.Height;
        var a = new bool[w, h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) a[x, y] = bmp.GetPixel(x, y).A > 0;
        var seen = new bool[w, h];
        var boxes = new List<Rectangle>();
        var st = new Stack<Point>();
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            if (!a[x, y] || seen[x, y]) continue;
            int x0 = x, x1 = x, y0 = y, y1 = y; st.Push(new Point(x, y)); seen[x, y] = true;
            while (st.Count > 0)
            {
                var p = st.Pop();
                if (p.X < x0) x0 = p.X; if (p.X > x1) x1 = p.X; if (p.Y < y0) y0 = p.Y; if (p.Y > y1) y1 = p.Y;
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = p.X + dx, ny = p.Y + dy;
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                    if (a[nx, ny] && !seen[nx, ny]) { seen[nx, ny] = true; st.Push(new Point(nx, ny)); }
                }
            }
            boxes.Add(Rectangle.FromLTRB(x0, y0, x1 + 1, y1 + 1));
        }
        bool merged = true;
        while (merged)
        {
            merged = false;
            for (int i = 0; i < boxes.Count && !merged; i++)
                for (int j = i + 1; j < boxes.Count && !merged; j++)
                {
                    var bi = boxes[i]; bi.Inflate(gap, gap);
                    if (bi.IntersectsWith(boxes[j]))
                    {
                        boxes[i] = Rectangle.Union(boxes[i], boxes[j]); boxes.RemoveAt(j); merged = true;
                    }
                }
        }
        boxes.RemoveAll(b => b.Width * b.Height < minArea);
        boxes.Sort((p, q) => Math.Abs(p.Y - q.Y) > 12 ? p.Y.CompareTo(q.Y) : p.X.CompareTo(q.X));
        bmp.Dispose();
        return boxes;
    }

    public static void Crop(string path, Rectangle r, string outPath)
    {
        using (var bmp = new Bitmap(path))
        using (var c = bmp.Clone(r, PixelFormat.Format32bppArgb)) c.Save(outPath, ImageFormat.Png);
    }
}
