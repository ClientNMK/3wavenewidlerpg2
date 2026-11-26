using System.Drawing;
using System.Drawing.Drawing2D;

public static class Nine
{
    // Draws src 9-sliced into dest rect. Insets are in source pixels; scale converts source px to dest px for the fixed borders.
    public static void Draw(Graphics g, Bitmap src, RectangleF dest, int l, int t, int r, int b, float scale)
    {
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        float[] sx = { 0, l, src.Width - r }, sw = { l, src.Width - l - r, r };
        float[] sy = { 0, t, src.Height - b }, sh = { t, src.Height - t - b, b };
        float dl = l * scale, dr = r * scale, dt = t * scale, db = b * scale;
        float[] dx = { dest.X, dest.X + dl, dest.Right - dr }, dw = { dl, dest.Width - dl - dr, dr };
        float[] dy = { dest.Y, dest.Y + dt, dest.Bottom - db }, dh = { dt, dest.Height - dt - db, db };
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (sw[j] <= 0 || sh[i] <= 0 || dw[j] <= 0 || dh[i] <= 0) continue;
                g.DrawImage(src, new RectangleF(dx[j], dy[i], dw[j], dh[i]), new RectangleF(sx[j], sy[i], sw[j], sh[i]), GraphicsUnit.Pixel);
            }
    }
}
