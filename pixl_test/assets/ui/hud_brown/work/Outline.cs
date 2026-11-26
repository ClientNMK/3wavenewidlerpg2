using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public static class Outline
{
    // Adds rings around the opaque shape, one per colour in `rings` (inner first). 8-neighbour = rounder corners off; use 4-neighbour for pixel-art look.
    public static void Add(string src, string dst, string[] rings)
    {
        using (var b0 = new Bitmap(src))
        {
            int pad = rings.Length, w = b0.Width + pad * 2, h = b0.Height + pad * 2;
            var o = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var solid = new bool[w, h];
            for (int y = 0; y < b0.Height; y++)
                for (int x = 0; x < b0.Width; x++)
                {
                    var c = b0.GetPixel(x, y);
                    if (c.A < 128) continue;
                    o.SetPixel(x + pad, y + pad, Color.FromArgb(255, c.R, c.G, c.B));
                    solid[x + pad, y + pad] = true;
                }
            foreach (var hex in rings)
            {
                var col = ColorTranslator.FromHtml(hex);
                var next = (bool[,])solid.Clone();
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        if (solid[x, y]) continue;
                        bool e = (x > 0 && solid[x - 1, y]) || (x < w - 1 && solid[x + 1, y]) || (y > 0 && solid[x, y - 1]) || (y < h - 1 && solid[x, y + 1]);
                        if (e) { o.SetPixel(x, y, col); next[x, y] = true; }
                    }
                solid = next;
            }
            o.Save(dst, ImageFormat.Png);
            o.Dispose();
        }
    }

    // Preview: each image at `scale` on two background crops, stacked.
    public static void Preview(string[] imgs, string bgA, string bgB, string outPath, int scale, string coin, string girl)
    {
        int cellW = 620, cellH = 230;
        using (var pv = new Bitmap(cellW * 2, cellH * imgs.Length, PixelFormat.Format32bppArgb))
        using (var g = Graphics.FromImage(pv))
        {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            string[] bgs = { bgA, bgB };
            int[][] crop = { new[] { 700, 900 }, new[] { 800, 700 } };
            for (int k = 0; k < 2; k++)
                using (var bg = new Bitmap(bgs[k]))
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        g.DrawImage(bg, new Rectangle(k * cellW, i * cellH, cellW, cellH), new Rectangle(crop[k][0], crop[k][1] + i * 60, cellW / 2, cellH / 2), GraphicsUnit.Pixel);
                        using (var im = new Bitmap(imgs[i]))
                            g.DrawImage(im, new Rectangle(k * cellW + 20, i * cellH + (cellH - im.Height * scale) / 2, im.Width * scale, im.Height * scale), 0, 0, im.Width, im.Height, GraphicsUnit.Pixel);
                        using (var c = new Bitmap(coin)) g.DrawImage(c, new Rectangle(k * cellW + 440, i * cellH + 20, 96, 96), 0, 0, 32, 32, GraphicsUnit.Pixel);
                        using (var gi = new Bitmap(girl)) g.DrawImage(gi, new Rectangle(k * cellW + 470, i * cellH + 60, 156, 156), 30, 22, 52, 52, GraphicsUnit.Pixel);
                    }
            pv.Save(outPath, ImageFormat.Png);
        }
    }
}
