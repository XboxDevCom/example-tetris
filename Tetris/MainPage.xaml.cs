using System;
using System.Collections.Generic;
using Tetris.Models;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Tetris
{
    /// <summary>
    /// Die Hauptseite der Tetris-Anwendung mit Spiellogik und Steuerung.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int BlockSize = 20;
        private const int PreviewBlockSize = 20;

        private GameBoard _board;
        private DispatcherTimer _timer;
        private readonly Dictionary<int, SolidColorBrush> _colorBrushes;

        /// <summary>
        /// Initialisiert eine neue Instanz der MainPage.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            _colorBrushes = new Dictionary<int, SolidColorBrush>
            {
                { 0, new SolidColorBrush(Windows.UI.Colors.Transparent) },
                { 1, new SolidColorBrush(Windows.UI.Colors.Cyan) },
                { 2, new SolidColorBrush(Windows.UI.Colors.Yellow) },
                { 3, new SolidColorBrush(Windows.UI.Colors.Purple) },
                { 4, new SolidColorBrush(Windows.UI.Colors.Green) },
                { 5, new SolidColorBrush(Windows.UI.Colors.Red) },
                { 6, new SolidColorBrush(Windows.UI.Colors.Blue) },
                { 7, new SolidColorBrush(Windows.UI.Colors.Orange) }
            };

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;

            _board = new GameBoard();
            Render();
        }

        /// <summary>
        /// Startet oder startet das Spiel neu.
        /// </summary>
        private void StartGame()
        {
            _board.Reset();
            GameOverText.Visibility = Visibility.Collapsed;
            StartButton.Content = "Neustart";
            UpdateTimerInterval();
            _timer.Start();
            Render();
        }

        /// <summary>
        /// Aktualisiert das Timer-Intervall basierend auf der aktuellen Stufe.
        /// </summary>
        private void UpdateTimerInterval()
        {
            int level = _board.Level;
            int interval = Math.Max(100, 800 - (level - 1) * 70);
            _timer.Interval = TimeSpan.FromMilliseconds(interval);
        }

        /// <summary>
        /// Wird bei jedem Timer-Tick aufgerufen und bewegt das Tetromino nach unten.
        /// </summary>
        private void Timer_Tick(object sender, object e)
        {
            if (_board.IsGameOver)
            {
                _timer.Stop();
                GameOverText.Visibility = Visibility.Visible;
                StartButton.Content = "Start";
                return;
            }

            _board.MoveDown();
            UpdateTimerInterval();
            Render();
        }

        /// <summary>
        /// Behandelt Tasten- und Gamepad-Eingaben.
        /// </summary>
        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_board.IsGameOver)
                return;

            bool handled = true;

            switch (e.Key)
            {
                case VirtualKey.Left:
                case VirtualKey.GamepadDPadLeft:
                case VirtualKey.GamepadLeftThumbstickLeft:
                    _board.MoveLeft();
                    break;
                case VirtualKey.Right:
                case VirtualKey.GamepadDPadRight:
                case VirtualKey.GamepadLeftThumbstickRight:
                    _board.MoveRight();
                    break;
                case VirtualKey.Down:
                case VirtualKey.GamepadDPadDown:
                case VirtualKey.GamepadLeftThumbstickDown:
                    _board.MoveDown();
                    break;
                case VirtualKey.Up:
                case VirtualKey.GamepadDPadUp:
                case VirtualKey.GamepadLeftThumbstickUp:
                    _board.HardDrop();
                    break;
                case VirtualKey.Space:
                case VirtualKey.GamepadA:
                    _board.Rotate();
                    break;
                default:
                    handled = false;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
                UpdateTimerInterval();
                Render();

                if (_board.IsGameOver)
                {
                    _timer.Stop();
                    GameOverText.Visibility = Visibility.Visible;
                    StartButton.Content = "Start";
                }
            }
        }

        /// <summary>
        /// Behandelt den Klick auf die Start-/Neustart-Schaltfläche.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        /// <summary>
        /// Rendert das gesamte Spielfeld, das aktuelle Tetromino und die Vorschau.
        /// </summary>
        private void Render()
        {
            RenderBoard();
            RenderCurrentPiece();
            RenderNextPiece();
            UpdateDisplay();
        }

        /// <summary>
        /// Rendert die festgesperrten Blöcke auf dem Spielfeld.
        /// </summary>
        private void RenderBoard()
        {
            GameArea.Children.Clear();

            for (int y = 0; y < GameBoard.Rows; y++)
            {
                for (int x = 0; x < GameBoard.Columns; x++)
                {
                    int cell = _board.GetCell(x, y);
                    if (cell != 0)
                    {
                        DrawBlock(GameArea, x, y, cell, BlockSize);
                    }
                }
            }
        }

        /// <summary>
        /// Rendert das aktuell fallende Tetromino.
        /// </summary>
        private void RenderCurrentPiece()
        {
            if (_board.CurrentPiece == null || _board.IsGameOver)
                return;

            var piece = _board.CurrentPiece;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (piece.Shape[r, c] != 0)
                    {
                        int x = _board.CurrentX + c;
                        int y = _board.CurrentY + r;
                        if (y >= 0)
                        {
                            DrawBlock(GameArea, x, y, (int)piece.Type, BlockSize);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rendert die Vorschau des nächsten Tetrominos.
        /// </summary>
        private void RenderNextPiece()
        {
            NextPieceArea.Children.Clear();

            if (_board.NextPiece == null)
                return;

            var piece = _board.NextPiece;
            double offsetX = 0;
            double offsetY = 0;

            int minCol = 4, maxCol = 0, minRow = 4, maxRow = 0;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (piece.Shape[r, c] != 0)
                    {
                        minCol = Math.Min(minCol, c);
                        maxCol = Math.Max(maxCol, c);
                        minRow = Math.Min(minRow, r);
                        maxRow = Math.Max(maxRow, r);
                    }
                }
            }

            int width = (maxCol - minCol + 1) * PreviewBlockSize;
            int height = (maxRow - minRow + 1) * PreviewBlockSize;
            offsetX = (NextPieceArea.Width - width) / 2.0;
            offsetY = (NextPieceArea.Height - height) / 2.0;

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (piece.Shape[r, c] != 0)
                    {
                        double px = offsetX + (c - minCol) * PreviewBlockSize;
                        double py = offsetY + (r - minRow) * PreviewBlockSize;
                        DrawBlockAt(NextPieceArea, px, py, (int)piece.Type, PreviewBlockSize);
                    }
                }
            }
        }

        /// <summary>
        /// Zeichnet einen Block an der angegebenen Rasterposition.
        /// </summary>
        private void DrawBlock(Canvas canvas, int gridX, int gridY, int colorIndex, int size)
        {
            double x = gridX * size;
            double y = gridY * size;
            DrawBlockAt(canvas, x, y, colorIndex, size);
        }

        /// <summary>
        /// Zeichnet einen Block an der angegebenen Pixelposition.
        /// </summary>
        private void DrawBlockAt(Canvas canvas, double x, double y, int colorIndex, int size)
        {
            var rect = new Rectangle
            {
                Width = size - 2,
                Height = size - 2,
                Fill = _colorBrushes[colorIndex],
                Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, x + 1);
            Canvas.SetTop(rect, y + 1);
            canvas.Children.Add(rect);
        }

        /// <summary>
        /// Aktualisiert die Punktestand-, Stufen- und Zeilenanzeige.
        /// </summary>
        private void UpdateDisplay()
        {
            ScoreText.Text = _board.Score.ToString();
            LevelText.Text = _board.Level.ToString();
            LinesText.Text = _board.Lines.ToString();
        }
    }
}
