using System;
using System.Drawing;
using System.Drawing.Imaging;

// Renders a flat pixel glyph from an ASCII map onto a W×H canvas with a 1px outline.
// '#' fill, 'x' dark (inside), 'c' accent, 'a' eye, '.' empty.
public static class Glyph
{
    public static void Render(string[] map, string dst, int W, int H, string fillHex, string darkHex, string accentHex, string eyeHex, string outlineHex)
    {
        Color fill = ColorTranslator.FromHtml(fillHex), dark = ColorTranslator.FromHtml(darkHex),
              acc = ColorTranslator.FromHtml(accentHex), eye = ColorTranslator.FromHtml(eyeHex), ol = ColorTranslator.FromHtml(outlineHex);
        int mh = map.Length, mw = 0; foreach (var r in map) mw = Math.Max(mw, r.Length);
        int ox = (W - mw) / 2, oy = (H - mh) / 2;
        using (var o = new Bitmap(W, H, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < mh; y++)
                for (int x = 0; x < map[y].Length; x++)
                {
                    char ch = map[y][x]; Color c;
                    if (ch == '#') c = fill; else if (ch == 'x') c = dark; else if (ch == 'c') c = acc; else if (ch == 'a') c = eye; else continue;
                    o.SetPixel(x + ox, y + oy, c);
                }
            var add = new System.Collections.Generic.List<Point>();
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    if (o.GetPixel(x, y).A != 0) continue;
                    bool near = (x > 0 && o.GetPixel(x - 1, y).A != 0) || (x < W - 1 && o.GetPixel(x + 1, y).A != 0) ||
                                (y > 0 && o.GetPixel(x, y - 1).A != 0) || (y < H - 1 && o.GetPixel(x, y + 1).A != 0);
                    if (near) add.Add(new Point(x, y));
                }
            foreach (var p in add) o.SetPixel(p.X, p.Y, ol);
            o.Save(dst, ImageFormat.Png);
        }
    }
}
