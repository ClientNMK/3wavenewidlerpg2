using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// Turns a smooth illustrated UI part into pixel art at the game's pixel density:
// high-quality downscale -> hard alpha -> snap to a fixed palette -> 1px dark outline.
public static class Pixelize
{
    static Color[] pal;

    public static void SetPalette(string[] hex)
    {
        pal = new Color[hex.Length];
        for (int i = 0; i < hex.Length; i++) pal[i] = ColorTranslator.FromHtml(hex[i]);
    }

    static Color Nearest(Color c)
    {
        int best = 0; double bd = double.MaxValue;
        for (int i = 0; i < pal.Length; i++)
        {
            double dr = (c.R - pal[i].R) * 0.30, dg = (c.G - pal[i].G) * 0.59, db = (c.B - pal[i].B) * 0.11;
            double d = dr * dr + dg * dg + db * db;
            if (d < bd) { bd = d; best = i; }
        }
        return pal[best];
    }

    // targetH: height in game pixels. alphaCut: below = transparent. keepSemi: keep translucent plates (HUD) as one alpha level.
    public static Bitmap Run(string path, int targetH, int alphaCut, bool keepSemi, string outline)
    {
        using (var src = new Bitmap(path))
        {
            int tw = Math.Max(1, (int)Math.Round(src.Width * (double)targetH / src.Height));
            var small = new Bitmap(tw, targetH, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(small))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(src, 0, 0, tw, targetH);
            }
            var outC = ColorTranslator.FromHtml(outline);
            int W = tw + 2, H = targetH + 2;
            var res = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            var solid = new bool[W, H];
            for (int y = 0; y < targetH; y++)
                for (int x = 0; x < tw; x++)
                {
                    var c = small.GetPixel(x, y);
                    if (c.A < alphaCut) continue;
                    var n = Nearest(Color.FromArgb(255, c.R, c.G, c.B));
                    int a = 255;
                    if (keepSemi && c.A < 235) a = 190;
                    res.SetPixel(x + 1, y + 1, Color.FromArgb(a, n));
                    solid[x + 1, y + 1] = true;
                }
            tw = W; targetH = H;
            // 1px outline around opaque shape (outside pixels touching it)
            if (!keepSemi)
            {
                var add = new System.Collections.Generic.List<Point>();
                for (int y = 0; y < targetH; y++)
                    for (int x = 0; x < tw; x++)
                    {
                        if (solid[x, y]) continue;
                        bool edge = (x > 0 && solid[x - 1, y]) || (x < tw - 1 && solid[x + 1, y]) || (y > 0 && solid[x, y - 1]) || (y < targetH - 1 && solid[x, y + 1]);
                        if (edge) add.Add(new Point(x, y));
                    }
                // canvas may clip outline; acceptable for a test
                foreach (var p in add) res.SetPixel(p.X, p.Y, outC);
            }
            small.Dispose();
            return res;
        }
    }
}
