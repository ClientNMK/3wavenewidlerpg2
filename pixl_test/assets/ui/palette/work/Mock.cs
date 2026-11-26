using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

public static class Mock
{
    // Exact-colour remap. map: "#RRGGBB>#RRGGBB" entries.
    public static void Recolor(string src, string dst, string[] map)
    {
        var d = new Dictionary<int, Color>();
        foreach (var m in map) { var p = m.Split('>'); d[ColorTranslator.FromHtml(p[0]).ToArgb() & 0xFFFFFF] = ColorTranslator.FromHtml(p[1]); }
        using (var b0 = new Bitmap(src))
        using (var b = b0.Clone(new Rectangle(0, 0, b0.Width, b0.Height), PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    var c = b.GetPixel(x, y);
                    if (c.A == 0) continue;
                    Color n;
                    if (d.TryGetValue(c.ToArgb() & 0xFFFFFF, out n)) b.SetPixel(x, y, Color.FromArgb(c.A, n));
                }
            b.Save(dst, ImageFormat.Png);
        }
    }

    static Graphics G;
    static string P;
    static Bitmap Part(string n) { return new Bitmap(Path.Combine(P, n + ".png")); }

    static void Put(Bitmap img, int x, int y, int s)
    {
        G.DrawImage(img, new Rectangle(x, y, img.Width * s, img.Height * s), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
    }

    static void Nine(Bitmap img, int x, int y, int w, int h, int l, int t, int r, int b)
    {
        int[] sx = { 0, l, img.Width - r }, sw = { l, img.Width - l - r, r };
        int[] sy = { 0, t, img.Height - b }, sh = { t, img.Height - t - b, b };
        int[] dx = { x, x + l, x + w - r }, dw = { l, w - l - r, r };
        int[] dy = { y, y + t, y + h - b }, dh = { t, h - t - b, b };
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (sw[j] <= 0 || sh[i] <= 0 || dw[j] <= 0 || dh[i] <= 0) continue;
                G.DrawImage(img, new Rectangle(dx[j], dy[i], dw[j], dh[i]), sx[j], sy[i], sw[j], sh[i], GraphicsUnit.Pixel);
            }
    }

    static void Txt(string t, float size, float cx, float cy, Color col, bool center)
    {
        using (var f = new Font("Malgun Gothic", size, FontStyle.Bold, GraphicsUnit.Pixel))
        {
            var sz = G.MeasureString(t, f);
            float x = center ? cx - sz.Width / 2 : cx, y = cy - sz.Height / 2;
            using (var sh = new SolidBrush(Color.FromArgb(200, 30, 14, 12))) G.DrawString(t, f, sh, x + 2, y + 2);
            using (var br = new SolidBrush(col)) G.DrawString(t, f, br, x, y);
        }
    }

    // One 720x1280 screen.
    public static Bitmap Screen(string partsDir, string bgPath, string girl, string slime, string coinBig, string coinSmall, string portraitFill, string panelText, string label)
    {
        P = partsDir;
        var cv = new Bitmap(720, 1280, PixelFormat.Format32bppArgb);
        G = Graphics.FromImage(cv);
        G.InterpolationMode = InterpolationMode.HighQualityBicubic;
        using (var bg = new Bitmap(bgPath))
            G.DrawImage(bg, new Rectangle(0, 0, 720, 1280), new Rectangle(bg.Width * 280 / 1280, 0, bg.Width * 720 / 1280, bg.Height), GraphicsUnit.Pixel);
        G.InterpolationMode = InterpolationMode.NearestNeighbor;
        G.PixelOffsetMode = PixelOffsetMode.Half;
        G.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        var white = Color.White;
        var ink = ColorTranslator.FromHtml(panelText);

        using (var g = new Bitmap(girl)) Put(g, 200, 470, 2);
        using (var s = new Bitmap(slime)) { Put(s, 420, 430, 2); Put(s, 470, 610, 1); }

        // portrait
        using (var br = new SolidBrush(ColorTranslator.FromHtml(portraitFill))) G.FillRectangle(br, 20, 20, 100, 100);
        using (var g = new Bitmap(girl)) G.DrawImage(g, new Rectangle(18, 12, 104, 104), 30, 22, 52, 52, GraphicsUnit.Pixel);
        using (var p = Part("frame_portrait")) Put(p, 10, 10, 1);
        using (var p = Part("badge_level")) Put(p, 24, 122, 1);
        Txt("Lv.25", 18, 68, 137, white, true);
        Txt("나이트메어", 22, 140, 30, white, false);

        // currency
        using (var p = Part("plate_currency"))
        {
            Nine(p, 300, 18, 190, p.Height, 26, 0, 26, 0);
            Nine(p, 508, 18, 150, p.Height, 26, 0, 26, 0);
        }
        using (var c = new Bitmap(coinBig)) Put(c, 276, 10, 1);
        using (var c = new Bitmap(coinSmall)) Put(c, 492, 12, 2);
        Txt("4,276,035", 18, 410, 44, white, true);
        Txt("19,999", 18, 596, 44, white, true);
        using (var p = Part("btn_plus")) Put(p, 660, 16, 1);

        // stage + progress
        using (var p = Part("plate_stage")) Nine(p, 140, 86, 500, p.Height, 20, 0, 20, 0);
        Txt("꿈속 바닷가  Stage 1-18        0/20", 20, 390, 111, white, true);
        using (var p = Part("bar_trough")) Nine(p, 170, 142, 440, p.Height, 16, 0, 16, 0);
        using (var p = Part("bar_fill_sample")) G.DrawImage(p, new Rectangle(170, 146, 250, p.Height), 0, 0, 200, p.Height, GraphicsUnit.Pixel);

        // side buttons
        using (var p = Part("btn_mail")) Put(p, 606, 190, 1);
        using (var p = Part("btn_rank")) Put(p, 606, 304, 1);

        // skill slots
        using (var p = Part("slot")) for (int k = 0; k < 5; k++) Put(p, 150 + k * 74, 800, 1);
        using (var p = Part("btn_sub")) Put(p, 540, 812, 1);
        Txt("AUTO", 17, 579, 832, white, true);

        // bottom panel (gold upgrade) with rows
        using (var p = Part("panel")) Nine(p, 12, 884, 696, 384, 24, 24, 24, 24);
        Txt("골드 강화", 26, 46, 920, ink, false);
        using (var close = Part("btn_close")) Put(close, 640, 892, 1);
        string[] rows = { "공격력  Lv.12     120 → 130", "체력     Lv.9      900 → 1,000" };
        for (int r = 0; r < 2; r++)
        {
            int y = 960 + r * 96;
            using (var s = Part("slot")) Put(s, 40, y, 1);
            Txt(rows[r], 22, 124, y + 33, ink, false);
            using (var bm = Part("btn_main")) Put(bm, 530, y + 6, 1);
            Txt("1,200", 20, 606, y + 33, white, true);
        }
        using (var bs = Part("btn_sub")) { Put(bs, 40, 1168, 1); Put(bs, 130, 1168, 1); Put(bs, 220, 1168, 1); }
        Txt("×1", 18, 79, 1188, white, true); Txt("×10", 18, 169, 1188, white, true); Txt("MAX", 18, 259, 1188, white, true);

        // label
        using (var br = new SolidBrush(Color.FromArgb(200, 0, 0, 0))) G.FillRectangle(br, 0, 1240, 720, 40);
        Txt(label, 22, 360, 1260, Color.Yellow, true);
        G.Dispose();
        return cv;
    }

    public static void Row(Bitmap[] screens, string outPath)
    {
        using (var o = new Bitmap(screens.Length * 730 - 10, 1280))
        using (var g = Graphics.FromImage(o))
        {
            g.Clear(Color.Black);
            for (int i = 0; i < screens.Length; i++) g.DrawImage(screens[i], i * 730, 0, 720, 1280);
            o.Save(outPath, ImageFormat.Png);
        }
    }
}
