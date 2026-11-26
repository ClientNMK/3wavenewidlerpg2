using System;
using System.Drawing;
using System.Drawing.Imaging;

// Status-effect icon: round coloured badge (with cocoa ring and a soft top highlight) + cream glyph with cocoa outline.
// Glyph map chars: '#' glyph, 'x' dark (cocoa), 'a' accent; '.' = empty (badge shows through).
public static class StatusIcon
{
    static Color Mix(Color a, Color b, double t)
    {
        return Color.FromArgb(255, (int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));
    }

    public static void Render(string[] map, string dst, int S, bool badge, string badgeHex, string glyphHex, string accentHex, string outlineHex)
    {
        Color bc = ColorTranslator.FromHtml(badgeHex), gc = ColorTranslator.FromHtml(glyphHex),
              ac = ColorTranslator.FromHtml(accentHex), ol = ColorTranslator.FromHtml(outlineHex);
        double c = (S - 1) / 2.0, R = S / 2.0 - 0.6;
        using (var o = new Bitmap(S, S, PixelFormat.Format32bppArgb))
        {
            var inDisk = new bool[S, S];
            if (badge)
                for (int y = 0; y < S; y++)
                    for (int x = 0; x < S; x++)
                    {
                        double d = Math.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
                        if (d <= R - 1) { inDisk[x, y] = true; o.SetPixel(x, y, (y < c - R * 0.45 && d <= R - 2.2) ? Mix(bc, Color.White, 0.22) : bc); }
                        else if (d <= R) o.SetPixel(x, y, ol);
                    }
            int mh = map.Length, mw = 0; foreach (var r in map) mw = Math.Max(mw, r.Length);
            int ox = (S - mw) / 2, oy = (S - mh) / 2;
            var g = new bool[S, S];
            for (int y = 0; y < mh; y++)
                for (int x = 0; x < map[y].Length; x++)
                {
                    char ch = map[y][x]; if (ch == '.') continue;
                    int px = x + ox, py = y + oy; g[px, py] = true;
                    o.SetPixel(px, py, ch == '#' ? gc : ch == 'a' ? ac : ol);
                }
            // glyph outline
            for (int y = 0; y < S; y++)
                for (int x = 0; x < S; x++)
                {
                    if (g[x, y]) continue;
                    bool near = (x > 0 && g[x - 1, y]) || (x < S - 1 && g[x + 1, y]) || (y > 0 && g[x, y - 1]) || (y < S - 1 && g[x, y + 1]);
                    if (near && (!badge || inDisk[x, y])) o.SetPixel(x, y, ol);
                }
            o.Save(dst, ImageFormat.Png);
        }
    }
}
