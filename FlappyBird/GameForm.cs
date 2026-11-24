using System.Diagnostics;
using FlappyBird.Entities;

namespace FlappyBird;

/// <summary>
/// Oyunun çizildiği ana pencere. Oyun döngüsünü (update + render) yönetir.
/// </summary>
public class GameForm : Form
{
    public const int GameWidth = 400;
    public const int GameHeight = 600;

    private readonly System.Windows.Forms.Timer _gameTimer;
    private readonly Stopwatch _stopwatch = new();
    private long _lastTicks;

    public const int GroundHeight = 80;
    public const int PlayfieldHeight = GameHeight - GroundHeight;

    private readonly Bird _bird = new(80, GameHeight / 2f - 12);
    private readonly PipeManager _pipeManager = new(GameWidth, PlayfieldHeight);
    private readonly Ground _ground = new(PlayfieldHeight, GameWidth, GroundHeight);

    public GameForm()
    {
        Text = "Flappy Bird - C# WinForms";
        ClientSize = new Size(GameWidth, GameHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.SkyBlue;
        KeyPreview = true;

        // Titremeyi önlemek için çift tamponlu çizim
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;

        _gameTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _gameTimer.Tick += OnGameTick;
        _stopwatch.Start();
        _gameTimer.Start();
    }

    /// <summary>
    /// Her timer tetiklenmesinde geçen süreyi hesaplar, oyunu günceller ve yeniden çizer.
    /// </summary>
    private void OnGameTick(object? sender, EventArgs e)
    {
        long now = _stopwatch.ElapsedTicks;
        float deltaTime = (float)(now - _lastTicks) / Stopwatch.Frequency;
        _lastTicks = now;

        // Pencere sürüklenirken oluşan büyük sıçramaları sınırla
        deltaTime = Math.Min(deltaTime, 0.05f);

        UpdateGame(deltaTime);
        Invalidate();
    }

    private void UpdateGame(float deltaTime)
    {
        _bird.Update(deltaTime);
        _pipeManager.Update(deltaTime);
        _ground.Speed = _pipeManager.Speed;
        _ground.Update(deltaTime);

        // Geçici: çarpışmada oyunu sıfırla (durum makinesi sonraki adımda eklenecek)
        if (HasCollision())
        {
            _bird.Reset();
            _pipeManager.Reset();
        }
    }

    /// <summary>Kuşun zemine veya bir boruya çarpıp çarpmadığını döndürür.</summary>
    private bool HasCollision() =>
        _bird.Bounds.Bottom >= _ground.Y || _pipeManager.CheckCollision(_bird);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode is Keys.Space or Keys.Up or Keys.W)
        {
            _bird.Flap();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            _bird.Flap();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.SkyBlue);

        _pipeManager.Draw(g);
        _ground.Draw(g);
        _bird.Draw(g);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _gameTimer.Stop();
        _gameTimer.Dispose();
        base.OnFormClosed(e);
    }
}
