// 저렴한 겨울판: 기존 동작 프레임의 옷 색만 바꾸고(갈색→빨강, 크림 프릴→흰색) 귀마개를 머리 위치에 얹는다. 생성 비용 0.
using System;
using System.Drawing;

public static class WinterCheap
{
    static int[,] headT; static int headW, headH;
    const int HX = 36, HY = 28;           // idle frame_000에서 머리 본보기 위치
    const int EarX = 19, EarY = 23;       // idle frame_000 기준 귀마개 위치
    static Color[] frill = { C("#F9DAB2"), C("#F7D6A6"), C("#D0AE8D"), C("#C19F7E"), C("#B28F75"), C("#E0BA8B"), C("#F4D0A8") };
    static Color C(string h) { return ColorTranslator.FromHtml(h); }

    public static void LoadHead(string idle0)
    {
        using (var b = new Bitmap(idle0))
        {
            headW = 60; headH = 30; headT = new int[headW, headH];
            for (int y = 0; y < headH; y++) for (int x = 0; x < headW; x++)
            { var c = b.GetPixel(HX + x, HY + y); headT[x, y] = c.A > 0 ? c.R + c.G * 3 + c.B : -1; }
        }
    }

    // 머리 본보기를 프레임에서 찾아 idle frame_000 대비 이동량을 돌려준다
    public static int[] FindHead(Bitmap b)
    {
        int W = b.Width, H = b.Height; var a = new int[W, H];
        for (int y = 0; y < H; y++) for (int x = 0; x < W; x++) { var c = b.GetPixel(x, y); a[x, y] = c.A > 0 ? c.R + c.G * 3 + c.B : -1; }
        long best = long.MaxValue; int bx = 0, by = 0;
        for (int dy = -40; dy <= 40; dy++) for (int dx = -40; dx <= 40; dx++)
        {
            long s = 0;
            for (int y = 0; y < headH && s < best; y++) for (int x = 0; x < headW; x++)
            {
                int p = headT[x, y]; int X = HX + x + dx, Y = HY + y + dy;
                int q = (X < 0 || Y < 0 || X >= W || Y >= H) ? -1 : a[X, Y];
                if (p < 0 && q < 0) continue;
                s += (p < 0 || q < 0) ? 600 : Math.Abs(p - q);
            }
            if (s < best) { best = s; bx = dx; by = dy; }
        }
        return new int[] { bx, by, (int)(best / 100) };
    }

    static bool Near(Color c, Color[] list, int tol) { foreach (var f in list) if (Math.Abs(f.R - c.R) + Math.Abs(f.G - c.G) + Math.Abs(f.B - c.B) <= tol) return true; return false; }

    static Color FromHsl(double h, double s, double l)
    {
        double c = (1 - Math.Abs(2 * l - 1)) * s, hp = h / 60, x = c * (1 - Math.Abs(hp % 2 - 1)), m = l - c / 2, r = 0, g = 0, bb = 0;
        switch ((int)Math.Floor(hp)) { case 0: r = c; g = x; break; case 1: r = x; g = c; break; case 2: g = c; bb = x; break; case 3: g = x; bb = c; break; case 4: r = x; bb = c; break; default: r = c; bb = x; break; }
        return Color.FromArgb(255, (int)(255 * (r + m)), (int)(255 * (g + m)), (int)(255 * (bb + m)));
    }

    // 누운 프레임(dead 후반): 위아래 기준 대신 몸 왼쪽 끝(부츠) bootW px만 빼고 옷 색을 바꾼다. 귀마개는 얹지 않음
    public static string ApplyLying(string src, string dst, int bootW, int minY)
    {
        using (var b0 = new Bitmap(src))
        using (var img = new Bitmap(b0))
        {
            int left = img.Width;
            for (int y = minY; y < img.Height; y++) for (int x = 0; x < img.Width; x++)
            { var c = img.GetPixel(x, y); if (c.A > 0 && c.GetBrightness() <= 0.37 && (c.GetHue() <= 40 || c.GetHue() >= 340) && c.GetBrightness() >= 0.13 && x < left && x > 30) left = x; }
            // 부츠 바로 오른쪽 갈색에서 시작해 갈색·프릴로 이어진 부분만 칠한다 (얼굴은 피부에 막혀 안 닿음)
            int W = img.Width, H = img.Height; var seen = new bool[W, H]; var q = new System.Collections.Generic.Queue<Point>();
            // 밝은 머리카락에서 2px 안쪽은 얼굴 쪽이라 칠하지 않는다
            var nearHair = new bool[W, H];
            for (int y = 0; y < H; y++) for (int x = left + bootW + 8; x < W; x++)
            {
                var c = img.GetPixel(x, y); if (c.A == 0) continue;
                double mx = Math.Max(c.R, Math.Max(c.G, c.B)) / 255.0, mn = Math.Min(c.R, Math.Min(c.G, c.B)) / 255.0, L = (mx + mn) / 2, S = mx == mn ? 0 : (mx - mn) / (1 - Math.Abs(2 * L - 1));
                if ((L > 0.6 && S < 0.45 && c.GetHue() >= 15 && c.GetHue() <= 35) || (L > 0.7 && S > 0.6 && c.GetHue() >= 12 && c.GetHue() <= 30 && !Near(c, frill, 14)))
                    for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++) { int X = x + dx, Y = y + dy; if (X >= 0 && Y >= 0 && X < W && Y < H) nearHair[X, Y] = true; }
            }
            Func<Color, bool> dark = c => c.A > 0 && c.GetBrightness() >= 0.13 && c.GetBrightness() <= 0.37 && (c.GetHue() <= 40 || c.GetHue() >= 340);
            for (int y = minY; y < H; y++) for (int x = left + bootW; x < left + bootW + 6 && x < W; x++) if (dark(img.GetPixel(x, y))) { seen[x, y] = true; q.Enqueue(new Point(x, y)); }
            var hit = new System.Collections.Generic.List<Point>();
            while (q.Count > 0)
            {
                var p = q.Dequeue(); hit.Add(p);
                for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = p.X + dx, ny = p.Y + dy; if (nx < left + bootW || ny < minY || nx >= W || ny >= H || seen[nx, ny]) continue;
                    var c = img.GetPixel(nx, ny); if (!(dark(c) || Near(c, frill, 14))) continue;
                    seen[nx, ny] = true; q.Enqueue(new Point(nx, ny));
                }
            }
            foreach (var p in hit)
            {
                if (nearHair[p.X, p.Y]) continue;
                var c = img.GetPixel(p.X, p.Y); float ll = c.GetBrightness();
                if (Near(c, frill, 14)) { int v = Math.Min(255, (int)(ll * 255 * 1.08 + 10)); img.SetPixel(p.X, p.Y, Color.FromArgb(255, v - 6, v - 4, v)); }
                else img.SetPixel(p.X, p.Y, FromHsl(355, 0.62, Math.Min(0.6, ll * 1.2 + 0.05)));
            }
            img.Save(dst);
            return "left " + left;
        }
    }

    public static string Apply(string src, string dst, string earPath)
    {
        using (var b0 = new Bitmap(src))
        using (var img = new Bitmap(b0))
        using (var ear = new Bitmap(earPath))
        {
            var h = FindHead(img);
            int top = HY + h[1];
            int feet = -1;
            for (int y = img.Height - 1; y >= 0 && feet < 0; y--) for (int x = 0; x < img.Width; x++) if (img.GetPixel(x, y).A > 0) { feet = y; break; }
            int W = img.Width, H = img.Height;
            // 밝은 머리카락 바로 옆(1px)은 머리 그림자일 수 있어 칠하지 않는다
            var nearHair = new bool[W, H];
            for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
            {
                var c = img.GetPixel(x, y); if (c.A == 0) continue;
                double mx = Math.Max(c.R, Math.Max(c.G, c.B)) / 255.0, mn = Math.Min(c.R, Math.Min(c.G, c.B)) / 255.0, L = (mx + mn) / 2, S = mx == mn ? 0 : (mx - mn) / (1 - Math.Abs(2 * L - 1));
                if (L > 0.6 && S < 0.45 && c.GetHue() >= 15 && c.GetHue() <= 35)
                    for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++) { int X = x + dx, Y = y + dy; if (X >= 0 && Y >= 0 && X < W && Y < H) nearHair[X, Y] = true; }
            }
            for (int y = top + 30; y < H; y++) for (int x = 0; x < W; x++)
            {
                var c = img.GetPixel(x, y); if (c.A == 0) continue;
                float hh = c.GetHue(), ll = c.GetBrightness();
                double mx = Math.Max(c.R, Math.Max(c.G, c.B)) / 255.0, mn = Math.Min(c.R, Math.Min(c.G, c.B)) / 255.0, L = (mx + mn) / 2, S = mx == mn ? 0 : (mx - mn) / (1 - Math.Abs(2 * L - 1));
                bool inEye = x >= 60 + h[0] && x <= 86 + h[0] && y >= 52 + h[1] && y <= 70 + h[1];
                bool isFrill = y >= top + 46 && (Near(c, frill, 14) || (hh >= 31 && hh <= 45 && S > 0.35 && L > 0.55 && L < 0.93));
                if (isFrill) { int v = Math.Min(255, (int)(ll * 255 * 1.08 + 10)); img.SetPixel(x, y, Color.FromArgb(255, v - 6, v - 4, v)); }
                else if (!inEye && !nearHair[x, y] && y < feet - 11 && ll >= 0.13 && ll <= 0.37 && (hh <= 40 || hh >= 340)) img.SetPixel(x, y, FromHsl(355, 0.62, Math.Min(0.6, ll * 1.2 + 0.05)));
            }
            // 머리를 못 찾은 프레임(흩어짐·구르기 등)은 귀마개를 얹지 않는다
            if (h[2] <= 3000) using (var g = Graphics.FromImage(img)) g.DrawImageUnscaled(ear, EarX + h[0], EarY + h[1]);
            img.Save(dst);
            return h[0] + "," + h[1] + " (score " + h[2] + ")";
        }
    }
}
