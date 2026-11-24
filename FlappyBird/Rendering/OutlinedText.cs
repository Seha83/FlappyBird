using System.Drawing.Drawing2D;

namespace FlappyBird.Rendering;

/// <summary>
/// Oyun ekranında kenarlıklı (outline) yazı çizmek için yardımcı sınıf.
/// </summary>
public static class OutlinedText
{
    /// <summary>
    /// Yazıyı verilen merkez noktasına yatay ortalanmış şekilde çizer.
    /// </summary>
    public static void DrawCentered(Graphics g, string text, float fontSize, float centerX, float y,
        Color fill, Color outline, float outlineWidth = 4f)
    {
        using var family = new FontFamily("Segoe UI");
        using var path = new GraphicsPath();
        using var format = new StringFormat { Alignment = StringAlignment.Center };

        float emSize = g.DpiY * fontSize / 72f;
        path.AddString(text, family, (int)FontStyle.Bold, emSize, new PointF(centerX, y), format);

        using var pen = new Pen(outline, outlineWidth) { LineJoin = LineJoin.Round };
        using var brush = new SolidBrush(fill);
        g.DrawPath(pen, path);
        g.FillPath(brush, path);
    }
}
