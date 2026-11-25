using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public static class GifMaker
{
    // frames: paths to PNGs (same size). scale: integer upscale. delayCs: delay in 1/100 s. bg: null = transparent
    public static void Make(string[] frames, string outPath, int scale, int delayCs, string bgHex, int lineY)
    {
        var bmps = new List<Bitmap>();
        foreach (var f in frames) bmps.Add(new Bitmap(f));
        int W = bmps[0].Width * scale, H = bmps[0].Height * scale;
        Color? bg = null;
        if (!string.IsNullOrEmpty(bgHex)) bg = ColorTranslator.FromHtml(bgHex);

        using (var fs = new FileStream(outPath, FileMode.Create))
        using (var w = new BinaryWriter(fs))
        {
            w.Write(new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' });
            w.Write((ushort)W); w.Write((ushort)H);
            w.Write((byte)0x00); w.Write((byte)0); w.Write((byte)0); // no global color table
            // loop forever
            w.Write(new byte[] { 0x21, 0xFF, 0x0B });
            w.Write(System.Text.Encoding.ASCII.GetBytes("NETSCAPE2.0"));
            w.Write(new byte[] { 0x03, 0x01, 0x00, 0x00, 0x00 });

            foreach (var b in bmps)
            {
                // build palette, index 0 = transparent
                var pal = new List<int>(); var map = new Dictionary<int, int>();
                pal.Add(0);
                var idx = new byte[W * H];
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        Color c = b.GetPixel(x / scale, y / scale);
                        int argb;
                        if (lineY >= 0 && y / scale == lineY) argb = unchecked((int)0xFFC0392B);
                        else if (c.A < 128) { if (bg == null) { idx[y * W + x] = 0; continue; } argb = bg.Value.ToArgb(); }
                        else argb = Color.FromArgb(255, c.R, c.G, c.B).ToArgb();
                        int k;
                        if (!map.TryGetValue(argb, out k))
                        {
                            if (pal.Count >= 256) { k = Nearest(pal, argb); }
                            else { k = pal.Count; pal.Add(argb); map[argb] = k; }
                        }
                        idx[y * W + x] = (byte)k;
                    }
                int bits = 1; while ((1 << bits) < pal.Count) bits++; if (bits < 2) bits = 2;
                int tableSize = 1 << bits;
                // graphic control extension: disposal 2 (restore bg), transparent index 0
                w.Write(new byte[] { 0x21, 0xF9, 0x04, (byte)((2 << 2) | 1) });
                w.Write((ushort)delayCs); w.Write((byte)0); w.Write((byte)0);
                // image descriptor with local color table
                w.Write((byte)0x2C); w.Write((ushort)0); w.Write((ushort)0); w.Write((ushort)W); w.Write((ushort)H);
                w.Write((byte)(0x80 | (bits - 1)));
                for (int i = 0; i < tableSize; i++)
                {
                    int c = i < pal.Count ? pal[i] : 0;
                    w.Write((byte)((c >> 16) & 255)); w.Write((byte)((c >> 8) & 255)); w.Write((byte)(c & 255));
                }
                w.Write((byte)bits);
                byte[] data = Lzw(idx, bits);
                int p = 0;
                while (p < data.Length) { int n = Math.Min(255, data.Length - p); w.Write((byte)n); w.Write(data, p, n); p += n; }
                w.Write((byte)0);
            }
            w.Write((byte)0x3B);
        }
        foreach (var b in bmps) b.Dispose();
    }

    static int Nearest(List<int> pal, int argb)
    {
        int r = (argb >> 16) & 255, g = (argb >> 8) & 255, bl = argb & 255, best = 1, bd = int.MaxValue;
        for (int i = 1; i < pal.Count; i++)
        {
            int c = pal[i]; int dr = ((c >> 16) & 255) - r, dg = ((c >> 8) & 255) - g, db = (c & 255) - bl;
            int d = dr * dr + dg * dg + db * db; if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    static byte[] Lzw(byte[] pixels, int minCode)
    {
        var outp = new List<byte>();
        int clear = 1 << minCode, eoi = clear + 1, codeSize = minCode + 1, next = eoi + 1;
        var dict = new Dictionary<long, int>();
        int bitBuf = 0, bitCnt = 0;
        Action<int> emit = code => {
            bitBuf |= code << bitCnt; bitCnt += codeSize;
            while (bitCnt >= 8) { outp.Add((byte)(bitBuf & 255)); bitBuf >>= 8; bitCnt -= 8; }
        };
        emit(clear);
        int prefix = pixels[0];
        for (int i = 1; i < pixels.Length; i++)
        {
            int k = pixels[i]; long key = ((long)prefix << 8) | (uint)k; int code;
            if (dict.TryGetValue(key, out code)) { prefix = code; continue; }
            emit(prefix);
            if (next < 4096)
            {
                dict[key] = next++;
                if (next > (1 << codeSize) && codeSize < 12) codeSize++;
            }
            else { emit(clear); dict.Clear(); codeSize = minCode + 1; next = eoi + 1; }
            prefix = k;
        }
        emit(prefix); emit(eoi);
        if (bitCnt > 0) outp.Add((byte)(bitBuf & 255));
        return outp.ToArray();
    }
}
