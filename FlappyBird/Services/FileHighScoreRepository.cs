namespace FlappyBird.Services;

/// <summary>
/// En yüksek skoru kullanıcının LocalAppData klasöründeki bir metin dosyasında saklar.
/// </summary>
public class FileHighScoreRepository : IHighScoreRepository
{
    private readonly string _filePath;

    public FileHighScoreRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FlappyBirdCSharp",
            "highscore.dat");
    }

    public int Load()
    {
        try
        {
            if (File.Exists(_filePath) &&
                int.TryParse(File.ReadAllText(_filePath).Trim(), out int value) &&
                value >= 0)
            {
                return value;
            }
        }
        catch (IOException)
        {
            // Dosya okunamazsa oyun 0 ile devam eder
        }
        catch (UnauthorizedAccessException)
        {
        }

        return 0;
    }

    public void Save(int highScore)
    {
        try
        {
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_filePath, highScore.ToString());
        }
        catch (IOException)
        {
            // Kayıt başarısız olsa bile oyun çökmemeli
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
