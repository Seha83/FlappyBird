using FlappyBird.Core;

namespace FlappyBird.Entities;

/// <summary>
/// Ekranın altında sürekli kayan zemin. Kuş zemine değerse oyun biter.
/// </summary>
public class Ground : GameObject
{
    private const float StripeWidth = 24f;
    private float _offset;

    public Ground(float y, float width, float height) : base(0, y, width, height)
    {
    }

    /// <summary>Zeminin kayma hızı; boru hızıyla aynı tutulur.</summary>
    public float Speed { get; set; }

    public override void Update(float deltaTime)
    {
        _offset = (_offset + Speed * deltaTime) % (StripeWidth * 2);
    }

    public override void Draw(Graphics g)
    {
        using var dirt = new SolidBrush(Color.FromArgb(222, 216, 149));
        using var grassLight = new SolidBrush(Color.FromArgb(115, 191, 46));
        using var grassDark = new SolidBrush(Color.FromArgb(85, 160, 30));
        using var edge = new Pen(Color.FromArgb(84, 56, 71), 2);

        g.FillRectangle(dirt, X, Y, Width, Height);

        // Kayan çimen şeritleri hareket hissi verir
        const float grassHeight = 16f;
        g.FillRectangle(grassLight, X, Y, Width, grassHeight);
        for (float x = -_offset; x < Width; x += StripeWidth * 2)
        {
            PointF[] stripe =
            [
                new(x, Y + grassHeight),
                new(x + StripeWidth, Y + grassHeight),
                new(x + StripeWidth * 1.5f, Y),
                new(x + StripeWidth * 0.5f, Y)
            ];
            g.FillPolygon(grassDark, stripe);
        }

        g.DrawLine(edge, X, Y, X + Width, Y);
        g.DrawLine(edge, X, Y + grassHeight, X + Width, Y + grassHeight);
    }
}
