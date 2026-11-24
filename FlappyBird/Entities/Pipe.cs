using System.Drawing.Drawing2D;
using FlappyBird.Core;

namespace FlappyBird.Entities;

/// <summary>
/// Arasında geçiş boşluğu bulunan üst ve alt boru çifti.
/// </summary>
public class Pipe : GameObject
{
    public const float PipeWidth = 64f;
    private const float CapHeight = 24f;
    private const float CapOverhang = 4f;

    private readonly float _gapCenterY;
    private readonly float _gapSize;
    private readonly float _playfieldHeight;

    public Pipe(float x, float gapCenterY, float gapSize, float playfieldHeight)
        : base(x, 0, PipeWidth, playfieldHeight)
    {
        _gapCenterY = gapCenterY;
        _gapSize = gapSize;
        _playfieldHeight = playfieldHeight;
    }

    /// <summary>Borunun sola doğru kayma hızı (piksel/saniye).</summary>
    public float Speed { get; set; }

    /// <summary>Kuşun bu borudan geçip geçmediği (skor bir kez sayılsın diye).</summary>
    public bool Passed { get; set; }

    /// <summary>Boru tamamen ekranın solundan çıktı mı?</summary>
    public bool IsOffScreen => X + Width < 0;

    public RectangleF TopRect => new(X, 0, Width, _gapCenterY - _gapSize / 2f);

    public RectangleF BottomRect
    {
        get
        {
            float top = _gapCenterY + _gapSize / 2f;
            return new RectangleF(X, top, Width, _playfieldHeight - top);
        }
    }

    /// <summary>Verilen dikdörtgenin üst veya alt boruya çarpıp çarpmadığını kontrol eder.</summary>
    public bool CollidesWith(RectangleF rect) =>
        TopRect.IntersectsWith(rect) || BottomRect.IntersectsWith(rect);

    public override void Update(float deltaTime)
    {
        X -= Speed * deltaTime;
    }

    public override void Draw(Graphics g)
    {
        DrawPipePart(g, TopRect, capAtBottom: true);
        DrawPipePart(g, BottomRect, capAtBottom: false);
    }

    private static void DrawPipePart(Graphics g, RectangleF body, bool capAtBottom)
    {
        if (body.Height <= 0)
        {
            return;
        }

        using var bodyBrush = new LinearGradientBrush(
            new RectangleF(body.X, body.Y, body.Width, Math.Max(body.Height, 1)),
            Color.FromArgb(120, 200, 60), Color.FromArgb(40, 120, 20),
            LinearGradientMode.Horizontal);
        using var outline = new Pen(Color.FromArgb(30, 70, 10), 2);

        g.FillRectangle(bodyBrush, body);
        g.DrawRectangle(outline, body.X, body.Y, body.Width, body.Height);

        float capY = capAtBottom ? body.Bottom - CapHeight : body.Y;
        var cap = new RectangleF(body.X - CapOverhang, capY, body.Width + CapOverhang * 2, CapHeight);
        using var capBrush = new LinearGradientBrush(cap,
            Color.FromArgb(140, 220, 80), Color.FromArgb(50, 140, 30),
            LinearGradientMode.Horizontal);
        g.FillRectangle(capBrush, cap);
        g.DrawRectangle(outline, cap.X, cap.Y, cap.Width, cap.Height);
    }
}
