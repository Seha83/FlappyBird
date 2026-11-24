namespace FlappyBird.Services;

/// <summary>
/// Oyunda çalınabilecek ses efektleri.
/// </summary>
public enum SoundEffect
{
    Flap,
    Score,
    Hit,
    Medal
}

/// <summary>
/// Ses çalma işlemleri için soyutlama; oyun mantığını somut ses altyapısından ayırır.
/// </summary>
public interface ISoundService
{
    /// <summary>Seslerin kapalı olup olmadığı.</summary>
    bool IsMuted { get; set; }

    /// <summary>Verilen ses efektini çalar (sessizdeyse hiçbir şey yapmaz).</summary>
    void Play(SoundEffect effect);
}
