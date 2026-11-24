using FlappyBird.Core;

namespace FlappyBird.Rendering;

/// <summary>
/// Yatayda tekrar eden ve kendi hız çarpanıyla kayan arka plan katmanı.
/// Uzak katmanlar daha yavaş kayarak derinlik (parallax) hissi oluşturur.
/// Alt sınıflar yalnızca tek bir "karo"nun nasıl çizileceğini tanımlar (Template Method).
/// </summary>
public abstract class ParallaxLayer : GameObject
{
    private float _offset;

    protected ParallaxLayer(float y, float screenWidth, float height, float tileWidth, float speedFactor)
        : base(0, y, screenWidth, height)
    {
        TileWidth = tileWidth;
        SpeedFactor = speedFactor;
    }

    /// <summary>Tekrar eden desenin genişliği.</summary>
    protected float TileWidth { get; }

    /// <summary>Dünya hızına göre bu katmanın hız çarpanı (0 = sabit, 1 = zemin hızı).</summary>
    public float SpeedFactor { get; }

    /// <summary>Dünyanın (boruların) kayma hızı.</summary>
    public float WorldSpeed { get; set; }

    public override void Update(float deltaTime)
    {
        _offset = (_offset + WorldSpeed * SpeedFactor * deltaTime) % TileWidth;
    }

    public override void Draw(Graphics g)
    {
        for (float x = -_offset; x < Width; x += TileWidth)
        {
            DrawTile(g, x);
        }
    }

    /// <summary>Tek bir karoyu verilen x konumuna çizer.</summary>
    protected abstract void DrawTile(Graphics g, float tileX);
}
