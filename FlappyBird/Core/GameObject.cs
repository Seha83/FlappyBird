namespace FlappyBird.Core;

/// <summary>
/// Oyundaki tüm görünür nesnelerin soyut temel sınıfı.
/// Konum, boyut ve çarpışma kutusu gibi ortak özellikleri kapsüller;
/// güncelleme ve çizim davranışını alt sınıflara bırakır (polimorfizm).
/// </summary>
public abstract class GameObject
{
    protected GameObject(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public float X { get; protected set; }
    public float Y { get; protected set; }
    public float Width { get; protected set; }
    public float Height { get; protected set; }

    /// <summary>
    /// Nesnenin dünya koordinatlarındaki sınırlayıcı dikdörtgeni.
    /// </summary>
    public virtual RectangleF Bounds => new(X, Y, Width, Height);

    /// <summary>
    /// Nesnenin durumunu geçen süreye (saniye) göre günceller.
    /// </summary>
    public abstract void Update(float deltaTime);

    /// <summary>
    /// Nesneyi verilen grafik yüzeyine çizer.
    /// </summary>
    public abstract void Draw(Graphics g);

    /// <summary>
    /// İki nesnenin çarpışma kutularının kesişip kesişmediğini döndürür.
    /// </summary>
    public bool Intersects(GameObject other) => Bounds.IntersectsWith(other.Bounds);
}
