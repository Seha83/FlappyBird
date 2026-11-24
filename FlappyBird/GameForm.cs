namespace FlappyBird;

/// <summary>
/// Oyunun çizildiği ana pencere.
/// </summary>
public class GameForm : Form
{
    public const int GameWidth = 400;
    public const int GameHeight = 600;

    public GameForm()
    {
        Text = "Flappy Bird - C# WinForms";
        ClientSize = new Size(GameWidth, GameHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.SkyBlue;
    }
}
