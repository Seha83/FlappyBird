namespace FlappyBird.Rendering;

/// <summary>
/// Gökyüzünde yavaşça süzülen bulutlar (en uzak katman).
/// </summary>
public class CloudLayer : ParallaxLayer
{
    public CloudLayer(float screenWidth) : base(40, screenWidth, 200, 260, 0.1f)
    {
    }

    protected override void DrawTile(Graphics g, float tileX)
    {
        using var brush = new SolidBrush(Color.FromArgb(235, 255, 255, 255));
        DrawCloud(g, brush, tileX + 20, Y + 20, 1.0f);
        DrawCloud(g, brush, tileX + 150, Y + 90, 0.7f);
    }

    private static void DrawCloud(Graphics g, Brush brush, float x, float y, float scale)
    {
        g.FillEllipse(brush, x, y + 10 * scale, 60 * scale, 30 * scale);
        g.FillEllipse(brush, x + 20 * scale, y, 50 * scale, 40 * scale);
        g.FillEllipse(brush, x + 45 * scale, y + 12 * scale, 55 * scale, 28 * scale);
    }
}

/// <summary>
/// Ufuktaki şehir silüeti (orta katman).
/// </summary>
public class CityLayer : ParallaxLayer
{
    private static readonly (float X, float W, float H)[] Buildings =
    [
        (0, 34, 70), (30, 28, 100), (56, 40, 55), (94, 26, 120),
        (118, 38, 80), (154, 30, 105), (182, 44, 65), (224, 36, 90)
    ];

    public CityLayer(float groundY, float screenWidth) : base(groundY - 120, screenWidth, 120, 260, 0.25f)
    {
    }

    protected override void DrawTile(Graphics g, float tileX)
    {
        using var building = new SolidBrush(Color.FromArgb(160, 210, 220));
        using var window = new SolidBrush(Color.FromArgb(200, 235, 240));
        float bottom = Y + Height;

        foreach (var (bx, bw, bh) in Buildings)
        {
            float x = tileX + bx;
            g.FillRectangle(building, x, bottom - bh, bw, bh);
            for (float wy = bottom - bh + 8; wy < bottom - 10; wy += 14)
            {
                for (float wx = x + 5; wx < x + bw - 6; wx += 10)
                {
                    g.FillRectangle(window, wx, wy, 4, 6);
                }
            }
        }
    }
}

/// <summary>
/// Zeminin hemen üstündeki çalılar (en yakın arka plan katmanı).
/// </summary>
public class BushLayer : ParallaxLayer
{
    public BushLayer(float groundY, float screenWidth) : base(groundY - 30, screenWidth, 30, 120, 0.5f)
    {
    }

    protected override void DrawTile(Graphics g, float tileX)
    {
        using var bush = new SolidBrush(Color.FromArgb(100, 190, 90));
        using var bushDark = new SolidBrush(Color.FromArgb(80, 165, 75));
        g.FillEllipse(bush, tileX - 10, Y, 70, 50);
        g.FillEllipse(bushDark, tileX + 40, Y + 8, 60, 44);
        g.FillEllipse(bush, tileX + 85, Y + 4, 50, 46);
    }
}
