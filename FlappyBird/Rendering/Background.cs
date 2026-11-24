using System.Drawing.Drawing2D;

namespace FlappyBird.Rendering;

/// <summary>
/// Gökyüzü gradyanı ve parallax katmanlarından oluşan arka planı bir arada yönetir (Composite).
/// </summary>
public sealed class Background : IDisposable
{
    private readonly List<ParallaxLayer> _layers;
    private readonly LinearGradientBrush _skyBrush;
    private readonly Rectangle _skyRect;

    public Background(int screenWidth, int groundY)
    {
        _skyRect = new Rectangle(0, 0, screenWidth, groundY);
        _skyBrush = new LinearGradientBrush(_skyRect,
            Color.FromArgb(78, 192, 202), Color.FromArgb(190, 235, 240), LinearGradientMode.Vertical);

        // Uzaktan yakına doğru sıralı çizim
        _layers =
        [
            new CloudLayer(screenWidth),
            new CityLayer(groundY, screenWidth),
            new BushLayer(groundY, screenWidth)
        ];
    }

    public void Update(float deltaTime, float worldSpeed)
    {
        foreach (ParallaxLayer layer in _layers)
        {
            layer.WorldSpeed = worldSpeed;
            layer.Update(deltaTime);
        }
    }

    public void Draw(Graphics g)
    {
        g.FillRectangle(_skyBrush, _skyRect);
        foreach (ParallaxLayer layer in _layers)
        {
            layer.Draw(g);
        }
    }

    public void Dispose() => _skyBrush.Dispose();
}
