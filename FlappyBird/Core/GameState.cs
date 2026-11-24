namespace FlappyBird.Core;

/// <summary>
/// Oyunun bulunabileceği durumlar.
/// </summary>
public enum GameState
{
    /// <summary>Başlangıç menüsü, oyuncunun başlamasını bekler.</summary>
    Menu,

    /// <summary>Oyun aktif olarak oynanıyor.</summary>
    Playing,

    /// <summary>Oyun duraklatıldı.</summary>
    Paused,

    /// <summary>Kuş çarptı, oyun bitti.</summary>
    GameOver
}
