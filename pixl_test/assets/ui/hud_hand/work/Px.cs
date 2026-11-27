using System;
using System.Drawing;
using System.Drawing.Imaging;

// 손으로 찍는 도트 UI용 작은 캔버스. 좌표는 모두 1배 픽셀.
public class Px
{
    public int W, H;
    public int[,] p;

    public Px(int w, int h) { W = w; H = h; p = new int[w, h]; }

    public static int C(string hex) { return unchecked((int)0xFF000000) | Convert.ToInt32(hex.TrimStart('#'), 16); }

    // 모서리를 줄별로 깎은 사각형. ins = 위(아래)에서부터 각 줄을 좌우로 몇 px 들여쓸지
    public bool[,] RRect(int x, int y, int w, int h, int[] ins)
    {
        var m = new bool[W, H];
        for (int j = 0; j < h; j++)
        {
            int k = Math.Min(j, h - 1 - j);
            int i = k < ins.Length ? ins[k] : 0;
            for (int a = x + i; a < x + w - i; a++) m[a, y + j] = true;
        }
        return m;
    }

    public bool[,] Erode(bool[,] m)
    {
        var r = new bool[W, H];
        for (int y = 1; y < H - 1; y++)
            for (int x = 1; x < W - 1; x++)
                r[x, y] = m[x, y] && m[x - 1, y] && m[x + 1, y] && m[x, y - 1] && m[x, y + 1];
        return r;
    }

    // dy = -1: 위쪽 가장자리 줄, +1: 아래쪽 가장자리 줄
    public bool[,] Edge(bool[,] m, int dy)
    {
        var r = new bool[W, H];
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                int n = y + dy;
                r[x, y] = m[x, y] && (n < 0 || n >= H || !m[x, n]);
            }
        return r;
    }

    public void Fill(bool[,] m, string c) { Fill(m, c, 0, H - 1); }

    public void Fill(bool[,] m, string c, int y0, int y1)
    {
        int v = C(c);
        for (int y = Math.Max(0, y0); y <= Math.Min(H - 1, y1); y++)
            for (int x = 0; x < W; x++)
                if (m[x, y]) p[x, y] = v;
    }

    public void Dot(int x, int y, string c) { if (x >= 0 && y >= 0 && x < W && y < H) p[x, y] = C(c); }

    public void Rect(int x, int y, int w, int h, string c)
    {
        for (int j = 0; j < h; j++) for (int i = 0; i < w; i++) Dot(x + i, y + j, c);
    }

    // pal = "K=#2A1A14;C=#F8E0C8" 형식. 지도에 없는 글자는 투명
    public void Art(int x, int y, string[] rows, string pal)
    {
        var d = new System.Collections.Generic.Dictionary<char, string>();
        foreach (var kv in pal.Split(';')) { var s = kv.Split('='); if (s.Length == 2) d[s[0][0]] = s[1]; }
        for (int j = 0; j < rows.Length; j++)
            for (int i = 0; i < rows[j].Length; i++)
                if (d.ContainsKey(rows[j][i])) Dot(x + i, y + j, d[rows[j][i]]);
    }

    public void Paste(Px s, int x, int y)
    {
        for (int j = 0; j < s.H; j++) for (int i = 0; i < s.W; i++)
            if (s.p[i, j] != 0 && x + i >= 0 && y + j >= 0 && x + i < W && y + j < H) p[x + i, y + j] = s.p[i, j];
    }

    public void PasteFile(string path, int x, int y, bool flip)
    {
        using (var b = new Bitmap(path))
            for (int j = 0; j < b.Height; j++) for (int i = 0; i < b.Width; i++)
            {
                var c = b.GetPixel(flip ? b.Width - 1 - i : i, j);
                if (c.A < 128) continue;
                int X = x + i, Y = y + j;
                if (X >= 0 && Y >= 0 && X < W && Y < H) p[X, Y] = unchecked((int)0xFF000000) | (c.ToArgb() & 0xFFFFFF);
            }
    }

    public void Save(string path, int s)
    {
        using (var b = new Bitmap(W * s, H * s, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
            {
                var c = Color.FromArgb(p[x, y]);
                for (int j = 0; j < s; j++) for (int i = 0; i < s; i++) b.SetPixel(x * s + i, y * s + j, c);
            }
            b.Save(path, ImageFormat.Png);
        }
    }
}
