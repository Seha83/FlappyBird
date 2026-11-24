namespace FlappyBird;

internal static class Program
{
    /// <summary>
    /// Uygulamanın giriş noktası.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length >= 2 && args[0] == "--screenshots")
        {
            using var form = new GameForm();
            form.CaptureDemoScreenshots(args[1]);
            return;
        }

        Application.Run(new GameForm());
    }
}
