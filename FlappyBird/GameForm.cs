using System.Diagnostics;
using FlappyBird.Core;
using FlappyBird.Entities;
using FlappyBird.Rendering;
using FlappyBird.Services;

namespace FlappyBird;

/// <summary>
/// Oyunun çizildiği ana pencere. Oyun döngüsünü (update + render) ve
/// oyun durumları arasındaki geçişleri yönetir.
/// </summary>
public class GameForm : Form
{
    public const int GameWidth = 400;
    public const int GameHeight = 600;
    public const int GroundHeight = 80;
    public const int PlayfieldHeight = GameHeight - GroundHeight;

    private const float RestartDelay = 0.6f;

    private readonly System.Windows.Forms.Timer _gameTimer;
    private readonly Stopwatch _stopwatch = new();
    private long _lastTicks;

    private readonly Bird _bird = new(80, GameHeight / 2f - 40);
    private readonly PipeManager _pipeManager = new(GameWidth, PlayfieldHeight);
    private readonly Ground _ground = new(PlayfieldHeight, GameWidth, GroundHeight);
    private readonly IHighScoreRepository _highScoreRepository;

    private GameState _state = GameState.Menu;
    private int _score;
    private int _highScore;
    private bool _isNewHighScore;
    private float _totalTime;
    private float _stateTime;

    public GameForm() : this(new FileHighScoreRepository())
    {
    }

    public GameForm(IHighScoreRepository highScoreRepository)
    {
        _highScoreRepository = highScoreRepository;
        _highScore = _highScoreRepository.Load();
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

    #region Durum yönetimi

    private void ChangeState(GameState newState)
    {
        _state = newState;
        _stateTime = 0;
    }

    private void StartNewGame()
    {
        _bird.Reset();
        _pipeManager.Reset();
        _score = 0;
        ChangeState(GameState.Playing);
        _bird.Flap();
    }

    private void EndGame()
    {
        _isNewHighScore = _score > _highScore;
        if (_isNewHighScore)
        {
            _highScore = _score;
            _highScoreRepository.Save(_highScore);
        }

        ChangeState(GameState.GameOver);
    }

    /// <summary>
    /// Oyuncunun "zıpla" girdisini mevcut duruma göre yorumlar.
    /// </summary>
    private void HandleFlapInput()
    {
        switch (_state)
        {
            case GameState.Menu:
                StartNewGame();
                break;
            case GameState.Playing:
                _bird.Flap();
                break;
            case GameState.GameOver when _stateTime >= RestartDelay:
                _bird.Reset();
                _pipeManager.Reset();
                ChangeState(GameState.Menu);
                break;
        }
    }

    #endregion

    #region Güncelleme

    private void UpdateGame(float deltaTime)
    {
        _totalTime += deltaTime;
        _stateTime += deltaTime;

        switch (_state)
        {
            case GameState.Menu:
                _bird.Hover(_totalTime);
                ScrollWorld(deltaTime, movePipes: false);
                break;

            case GameState.Playing:
                _bird.Update(deltaTime);
                ScrollWorld(deltaTime, movePipes: true);
                _score += _pipeManager.CountNewlyPassed(_bird);
                if (HasCollision())
                {
                    EndGame();
                }

                break;

            case GameState.GameOver:
                _bird.FallTo(_ground.Y, deltaTime);
                break;
        }
    }

    private void ScrollWorld(float deltaTime, bool movePipes)
    {
        if (movePipes)
        {
            _pipeManager.Update(deltaTime);
        }

        _ground.Speed = _pipeManager.Speed;
        _ground.Update(deltaTime);
    }

    /// <summary>Kuşun zemine veya bir boruya çarpıp çarpmadığını döndürür.</summary>
    private bool HasCollision() =>
        _bird.Bounds.Bottom >= _ground.Y || _pipeManager.CheckCollision(_bird);

    #endregion

    #region Girdi

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.KeyCode)
        {
            case Keys.Space or Keys.Up or Keys.W or Keys.Enter:
                HandleFlapInput();
                break;
            case Keys.Escape:
                Close();
                break;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
        {
            HandleFlapInput();
        }
    }

    #endregion

    #region Çizim

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.SkyBlue);

        _pipeManager.Draw(g);
        _ground.Draw(g);
        _bird.Draw(g);

        switch (_state)
        {
            case GameState.Menu:
                DrawMenu(g);
                break;
            case GameState.Playing:
                DrawScore(g);
                break;
            case GameState.GameOver:
                DrawGameOver(g);
                break;
        }
    }

    private void DrawScore(Graphics g) =>
        OutlinedText.DrawCentered(g, _score.ToString(), 36, GameWidth / 2f, 40, Color.White, Color.Black, 6);

    private void DrawMenu(Graphics g)
    {
        OutlinedText.DrawCentered(g, "FLAPPY BIRD", 34, GameWidth / 2f, 110, Color.Gold, Color.SaddleBrown, 7);
        OutlinedText.DrawCentered(g, "Başlamak için SPACE / Tıkla", 14, GameWidth / 2f, 380, Color.White, Color.Black, 4);
        OutlinedText.DrawCentered(g, $"En İyi: {_highScore}", 16, GameWidth / 2f, 420, Color.Gold, Color.Black, 4);
    }

    private void DrawGameOver(Graphics g)
    {
        OutlinedText.DrawCentered(g, "OYUN BİTTİ", 32, GameWidth / 2f, 120, Color.OrangeRed, Color.White, 7);

        var panel = new RectangleF(60, 200, GameWidth - 120, 150);
        using var panelBrush = new SolidBrush(Color.FromArgb(222, 216, 149));
        using var border = new Pen(Color.FromArgb(84, 56, 71), 3);
        g.FillRectangle(panelBrush, panel);
        g.DrawRectangle(border, panel.X, panel.Y, panel.Width, panel.Height);

        OutlinedText.DrawCentered(g, "SKOR", 14, GameWidth / 2f, panel.Y + 18, Color.OrangeRed, Color.White, 3);
        OutlinedText.DrawCentered(g, _score.ToString(), 30, GameWidth / 2f, panel.Y + 40, Color.White, Color.Black, 5);
        OutlinedText.DrawCentered(g, "EN İYİ", 14, GameWidth / 2f, panel.Y + 85, Color.OrangeRed, Color.White, 3);
        OutlinedText.DrawCentered(g, _highScore.ToString(), 22, GameWidth / 2f, panel.Y + 107, Color.White, Color.Black, 4);

        if (_isNewHighScore)
        {
            OutlinedText.DrawCentered(g, "YENİ REKOR!", 12, panel.Right - 50, panel.Y + 90, Color.Red, Color.White, 3);
        }

        if (_stateTime >= RestartDelay)
        {
            OutlinedText.DrawCentered(g, "Devam için SPACE / Tıkla", 14, GameWidth / 2f, 400, Color.White, Color.Black, 4);
        }
    }

    #endregion

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _gameTimer.Stop();
        _gameTimer.Dispose();
        base.OnFormClosed(e);
    }
}
