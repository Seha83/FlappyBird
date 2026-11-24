using System.Drawing.Drawing2D;

namespace FlappyBird.Core;

/// <summary>
/// Oyuncunun skoruna göre kazanabileceği madalya dereceleri.
/// </summary>
public enum MedalType
{
    None,
    Bronze,
    Silver,
    Gold,
    Platinum
}

/// <summary>
/// Skor değerine göre madalya hesaplar ve oyun sonu paneline madalya çizer.
/// </summary>
public static class MedalCalculator
{
    public static MedalType GetMedal(int score) => score switch
    {
        >= 40 => MedalType.Platinum,
        >= 25 => MedalType.Gold,
        >= 15 => MedalType.Silver,
        >= 5 => MedalType.Bronze,
        _ => MedalType.None
    };

    public static string GetDisplayName(MedalType medal) => medal switch
    {
        MedalType.Bronze => "BRONZ",
        MedalType.Silver => "GÜMÜŞ",
        MedalType.Gold => "ALTIN",
        MedalType.Platinum => "PLATİN",
        _ => "YOK"
    };

    public static void DrawMedal(Graphics g, MedalType medal, float centerX, float centerY, float radius)
    {
        var rect = new RectangleF(centerX - radius, centerY - radius, radius * 2, radius * 2);

        if (medal == MedalType.None)
        {
            using var emptyBrush = new SolidBrush(Color.FromArgb(190, 182, 120));
            using var dashedPen = new Pen(Color.FromArgb(130, 115, 75), 2) { DashStyle = DashStyle.Dash };
            g.FillEllipse(emptyBrush, rect);
            g.DrawEllipse(dashedPen, rect);
            return;
        }

        (Color light, Color dark, Color rim) = medal switch
        {
            MedalType.Bronze => (Color.FromArgb(235, 165, 105), Color.FromArgb(165, 95, 45), Color.FromArgb(110, 60, 25)),
            MedalType.Silver => (Color.FromArgb(245, 248, 252), Color.FromArgb(160, 172, 185), Color.FromArgb(100, 110, 125)),
            MedalType.Gold => (Color.FromArgb(255, 240, 120), Color.FromArgb(220, 165, 20), Color.FromArgb(145, 100, 10)),
            _ => (Color.FromArgb(225, 250, 255), Color.FromArgb(120, 205, 235), Color.FromArgb(65, 130, 165))
        };

        // Kurdele
        PointF[] ribbon =
        [
            new(centerX - radius * 0.45f, centerY - radius * 1.25f),
            new(centerX + radius * 0.45f, centerY - radius * 1.25f),
            new(centerX + radius * 0.30f, centerY - radius * 0.75f),
            new(centerX - radius * 0.30f, centerY - radius * 0.75f)
        ];
        using var ribbonBrush = new SolidBrush(Color.Crimson);
        g.FillPolygon(ribbonBrush, ribbon);

        using var grad = new LinearGradientBrush(rect, light, dark, LinearGradientMode.ForwardDiagonal);
        using var rimPen = new Pen(rim, 3);
        g.FillEllipse(grad, rect);
        g.DrawEllipse(rimPen, rect);

        // İç halka ve parıltı
        var inner = RectangleF.Inflate(rect, -5, -5);
        using var innerPen = new Pen( Color.FromArgb(160, light), 1.5f);
        g.DrawEllipse(innerPen, inner);

        // Yıldız
        DrawStar(g, centerX, centerY, radius * 0.48f, radius * 0.22f, Color.FromArgb(230, 255, 255, 255));
    }

    private static void DrawStar(Graphics g, float cx, float cy, float outerR, float innerR, Color color)
    {
        var pts = new PointF[10];
        for (int i = 0; i < 10; i++)
        {
            double angle = -Math.PI / 2 + i * Math.PI / 5;
            float r = i % 2 == 0 ? outerR : innerR;
            pts[i] = new PointF(cx + (float)Math.Cos(angle) * r, cy + (float)Math.Sin(angle) * r);
        }

        using var starBrush = new SolidBrush(color);
        g.FillPolygon(starBrush, pts);
    }
}
