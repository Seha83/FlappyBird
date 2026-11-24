namespace FlappyBird.Core;

/// <summary>
/// Oyuncunun skoruna göre boru kayma hızını ve borular arası geçiş boşluğunu
/// dinamik olarak hesaplayan zorluk ölçekleyici.
/// </summary>
public static class DifficultyScaler
{
    public const float BaseSpeed = 160f;
    public const float MaxSpeed = 270f;
    public const float SpeedIncreasePerPoint = 3.2f;

    public const float BaseGapSize = 158f;
    public const float MinGapSize = 115f;
    public const float GapDecreasePerPoint = 1.1f;

    /// <summary>Verilen skora karşılık gelen boru kayma hızını hesaplar.</summary>
    public static float CalculateSpeed(int score) =>
        Math.Clamp(BaseSpeed + score * SpeedIncreasePerPoint, BaseSpeed, MaxSpeed);

    /// <summary>Verilen skora karşılık gelen dikey boru boşluğunu hesaplar.</summary>
    public static float CalculateGapSize(int score) =>
        Math.Clamp(BaseGapSize - score * GapDecreasePerPoint, MinGapSize, BaseGapSize);

    /// <summary>Mevcut zorluk seviyesini 1-5 arası yıldız/kademe olarak döndürür.</summary>
    public static int GetDifficultyLevel(int score) => score switch
    {
        >= 35 => 5,
        >= 25 => 4,
        >= 15 => 3,
        >= 7 => 2,
        _ => 1
    };
}
