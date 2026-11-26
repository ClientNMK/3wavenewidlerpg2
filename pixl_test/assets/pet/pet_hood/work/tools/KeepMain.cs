using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

public static class KeepMain
{
    // Remove opaque islands smaller than minKeep pixels (8-connectivity). Returns removed pixel count.
    public static int Run(string path, int minKeep)
    {
        Bitmap o;
        int removed = 0;
        using (var b = new Bitmap(path))
        {
            o = new Bitmap(b);
            int W = b.Width, H = b.Height;
            var seen = new bool[W, H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    if (seen[x, y] || b.GetPixel(x, y).A == 0) continue;
                    var comp = new List<Point>(); var st = new Stack<Point>(); st.Push(new Point(x, y)); seen[x, y] = true;
                    while (st.Count > 0)
                    {
                        var p = st.Pop(); comp.Add(p);
                        for (int dy = -1; dy <= 1; dy++)
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int nx = p.X + dx, ny = p.Y + dy;
                                if (nx < 0 || ny < 0 || nx >= W || ny >= H || seen[nx, ny]) continue;
                                if (b.GetPixel(nx, ny).A == 0) continue;
                                seen[nx, ny] = true; st.Push(new Point(nx, ny));
                            }
                    }
                    if (comp.Count < minKeep) foreach (var p in comp) { o.SetPixel(p.X, p.Y, Color.Transparent); removed++; }
                }
        }
        o.Save(path, ImageFormat.Png); o.Dispose();
        return removed;
    }
}
