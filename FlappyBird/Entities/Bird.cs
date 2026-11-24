using System.Drawing.Drawing2D;
using FlappyBird.Core;

namespace FlappyBird.Entities;

/// <summary>
/// Oyuncunun kontrol ettiği kuş. Yerçekimi etkisiyle düşer, zıplama ile yükselir.
/// Kanat çırpma animasyonu ve hıza bağlı dönme efektine sahiptir.
/// </summary>
public class Bird : GameObject
{
    private const float Gravity = 1500f;        // piksel / saniye^2
    private const float JumpVelocity = -460f;   // piksel / saniye
    private const float MaxFallSpeed = 700f;
    private const float WingFrameDuration = 0.09f;
    private const float MinRotation = -25f;
    private const float MaxRotation = 90f;

    // Kanat karelerinin kuş gövdesine göre dikey ofsetleri (yukarı, orta, aşağı, orta)
    private static readonly float[] WingOffsets = [-7f, 0f, 6f, 0f];

    private readonly float _startX;
    private readonly float _startY;
    private float _animationTime;

    public Bird(float x, float y) : base(x, y, 34, 24)
    {
        _startX = x;
        _startY = y;
    }

    /// <summary>Dikey hız (pozitif = aşağı).</summary>
    public float VelocityY { get; private set; }

    /// <summary>Kuşun derece cinsinden dönüş açısı (pozitif = burun aşağı).</summary>
    public float Rotation { get; private set; }

    /// <summary>
    /// Çarpışma kutusu, görsel kenarlardan biraz küçültülerek daha adil bir oyun sağlanır.
    /// </summary>
    public override RectangleF Bounds => new(X + 3, Y + 3, Width - 6, Height - 6);

    private int WingFrame => (int)(_animationTime / WingFrameDuration) % WingOffsets.Length;

    /// <summary>Kuşu yukarı doğru zıplatır.</summary>
    public void Flap() => VelocityY = JumpVelocity;

    /// <summary>Kuşu başlangıç konumuna ve hızına döndürür.</summary>
    public void Reset()
    {
        X = _startX;
        Y = _startY;
        VelocityY = 0;
        Rotation = 0;
    }

    /// <summary>
    /// Menü ekranında kuşun yerinde süzülmesini sağlar.
    /// </summary>
    public void Hover(float totalTime)
    {
        Y = _startY + MathF.Sin(totalTime * 5f) * 8f;
        VelocityY = 0;
        Rotation = 0;
        _animationTime = totalTime;
    }

    /// <summary>
    /// Oyun bittiğinde kuşun zemine kadar düşmesini sağlar.
    /// </summary>
    public void FallTo(float groundY, float deltaTime)
    {
        if (Y + Height < groundY)
        {
            Update(deltaTime);
            Y = Math.Min(Y, groundY - Height);
        }
    }

    public override void Update(float deltaTime)
    {
        VelocityY = Math.Min(VelocityY + Gravity * deltaTime, MaxFallSpeed);
        Y += VelocityY * deltaTime;

        // Tavanın dışına çıkmasını engelle
        if (Y < 0)
        {
            Y = 0;
            VelocityY = 0;
        }

        // Yükselirken burun yukarı, hızla düşerken burun aşağı
        float targetRotation = VelocityY < 0
            ? MinRotation
            : Math.Clamp(VelocityY / MaxFallSpeed * MaxRotation * 1.3f, MinRotation, MaxRotation);
        Rotation += (targetRotation - Rotation) * Math.Min(1f, deltaTime * 10f);

        // Düşerken kanat çırpmayı yavaşlat
        _animationTime += VelocityY < 300 ? deltaTime : deltaTime * 0.3f;
    }

    public override void Draw(Graphics g)
    {
        GraphicsState saved = g.Save();

        // Kuşun merkezi etrafında döndür
        float cx = X + Width / 2f;
        float cy = Y + Height / 2f;
        g.TranslateTransform(cx, cy);
        g.RotateTransform(Rotation);

        float left = -Width / 2f;
        float top = -Height / 2f;

        using var bodyBrush = new LinearGradientBrush(
            new RectangleF(left, top, Width, Height), Color.Gold, Color.Orange, LinearGradientMode.Vertical);
        using var outline = new Pen(Color.FromArgb(90, 60, 10), 2);
        using var beakBrush = new SolidBrush(Color.OrangeRed);
        using var wingBrush = new SolidBrush(Color.LightYellow);

        g.FillEllipse(bodyBrush, left, top, Width, Height);
        g.DrawEllipse(outline, left, top, Width, Height);

        // Kanat (animasyonlu)
        float wingY = WingOffsets[WingFrame];
        var wing = new RectangleF(left + 2, wingY - 4, 16, 10);
        g.FillEllipse(wingBrush, wing);
        g.DrawEllipse(outline, wing);

        // Göz
        g.FillEllipse(Brushes.White, Width / 2f - 14, top + 3, 10, 10);
        g.DrawEllipse(Pens.Black, Width / 2f - 14, top + 3, 10, 10);
        g.FillEllipse(Brushes.Black, Width / 2f - 9, top + 6, 4, 4);

        // Gaga
        PointF[] beak =
        [
            new(Width / 2f - 4, -2),
            new(Width / 2f + 8, 2),
            new(Width / 2f - 4, 7)
        ];
        g.FillPolygon(beakBrush, beak);
        g.DrawPolygon(outline, beak);

        g.Restore(saved);
    }
}
