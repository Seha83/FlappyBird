using FlappyBird.Core;

namespace FlappyBird.Entities;

/// <summary>
/// Oyuncunun kontrol ettiği kuş. Yerçekimi etkisiyle düşer, zıplama ile yükselir.
/// </summary>
public class Bird : GameObject
{
    private const float Gravity = 1500f;        // piksel / saniye^2
    private const float JumpVelocity = -460f;   // piksel / saniye
    private const float MaxFallSpeed = 700f;

    private readonly float _startX;
    private readonly float _startY;

    public Bird(float x, float y) : base(x, y, 34, 24)
    {
        _startX = x;
        _startY = y;
    }

    /// <summary>Dikey hız (pozitif = aşağı).</summary>
    public float VelocityY { get; private set; }

    /// <summary>
    /// Çarpışma kutusu, görsel kenarlardan biraz küçültülerek daha adil bir oyun sağlanır.
    /// </summary>
    public override RectangleF Bounds => new(X + 3, Y + 3, Width - 6, Height - 6);

    /// <summary>Kuşu yukarı doğru zıplatır.</summary>
    public void Flap() => VelocityY = JumpVelocity;

    /// <summary>Kuşu başlangıç konumuna ve hızına döndürür.</summary>
    public void Reset()
    {
        X = _startX;
        Y = _startY;
        VelocityY = 0;
    }

    /// <summary>
    /// Menü ekranında kuşun yerinde süzülmesini sağlar.
    /// </summary>
    public void Hover(float totalTime)
    {
        Y = _startY + MathF.Sin(totalTime * 5f) * 8f;
        VelocityY = 0;
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
    }

    public override void Draw(Graphics g)
    {
        using var bodyBrush = new SolidBrush(Color.Gold);
        using var outline = new Pen(Color.FromArgb(90, 60, 10), 2);
        using var beakBrush = new SolidBrush(Color.OrangeRed);

        g.FillEllipse(bodyBrush, X, Y, Width, Height);
        g.DrawEllipse(outline, X, Y, Width, Height);

        // Göz
        g.FillEllipse(Brushes.White, X + Width - 14, Y + 3, 10, 10);
        g.FillEllipse(Brushes.Black, X + Width - 9, Y + 6, 4, 4);

        // Gaga
        PointF[] beak =
        [
            new(X + Width - 4, Y + Height / 2f - 2),
            new(X + Width + 8, Y + Height / 2f + 2),
            new(X + Width - 4, Y + Height / 2f + 6)
        ];
        g.FillPolygon(beakBrush, beak);
    }
}
