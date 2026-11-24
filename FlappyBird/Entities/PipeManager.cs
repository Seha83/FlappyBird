namespace FlappyBird.Entities;

/// <summary>
/// Boruların üretilmesinden, hareket ettirilmesinden ve temizlenmesinden sorumludur.
/// </summary>
public class PipeManager
{
    private const float SpawnDistance = 220f;   // ardışık borular arası yatay mesafe
    private const float MinMargin = 70f;        // boşluğun kenarlara minimum uzaklığı

    private readonly List<Pipe> _pipes = [];
    private readonly Random _random = new();
    private readonly float _screenWidth;
    private readonly float _playfieldHeight;

    public PipeManager(float screenWidth, float playfieldHeight)
    {
        _screenWidth = screenWidth;
        _playfieldHeight = playfieldHeight;
    }

    /// <summary>Boruların kayma hızı (piksel/saniye).</summary>
    public float Speed { get; set; } = 160f;

    /// <summary>Üst ve alt boru arasındaki boşluk yüksekliği.</summary>
    public float GapSize { get; set; } = 160f;

    public IReadOnlyList<Pipe> Pipes => _pipes;

    public void Reset() => _pipes.Clear();

    /// <summary>Kuşun herhangi bir boruya çarpıp çarpmadığını kontrol eder.</summary>
    public bool CheckCollision(Bird bird) => _pipes.Any(p => p.CollidesWith(bird.Bounds));

    /// <summary>
    /// Kuşun yeni geçtiği boruları işaretler ve sayısını döndürür.
    /// </summary>
    public int CountNewlyPassed(Bird bird)
    {
        int count = 0;
        foreach (Pipe pipe in _pipes)
        {
            if (!pipe.Passed && pipe.X + pipe.Width < bird.X)
            {
                pipe.Passed = true;
                count++;
            }
        }

        return count;
    }

    public void Update(float deltaTime)
    {
        // Son boru yeterince ilerlediyse yeni boru üret
        if (_pipes.Count == 0 || _pipes[^1].X < _screenWidth - SpawnDistance)
        {
            SpawnPipe();
        }

        foreach (Pipe pipe in _pipes)
        {
            pipe.Speed = Speed;
            pipe.Update(deltaTime);
        }

        _pipes.RemoveAll(p => p.IsOffScreen);
    }

    public void Draw(Graphics g)
    {
        foreach (Pipe pipe in _pipes)
        {
            pipe.Draw(g);
        }
    }

    private void SpawnPipe()
    {
        float minCenter = MinMargin + GapSize / 2f;
        float maxCenter = _playfieldHeight - MinMargin - GapSize / 2f;
        float center = minCenter + (float)_random.NextDouble() * (maxCenter - minCenter);

        _pipes.Add(new Pipe(_screenWidth + 10, center, GapSize, _playfieldHeight) { Speed = Speed });
    }
}
