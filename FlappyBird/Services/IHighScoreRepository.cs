namespace FlappyBird.Services;

/// <summary>
/// En yüksek skorun saklanması için soyutlama.
/// Farklı saklama yöntemleri (dosya, veritabanı, bellek) bu arayüzü uygulayabilir.
/// </summary>
public interface IHighScoreRepository
{
    /// <summary>Kaydedilmiş en yüksek skoru okur; yoksa 0 döner.</summary>
    int Load();

    /// <summary>Yeni en yüksek skoru kaydeder.</summary>
    void Save(int highScore);
}
