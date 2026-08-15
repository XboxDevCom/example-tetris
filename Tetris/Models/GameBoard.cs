using System;

namespace Tetris.Models
{
    /// <summary>
    /// Stellt das Tetris-Spielfeld mit Spiellogik dar.
    /// </summary>
    public sealed class GameBoard
    {
        /// <summary>
        /// Die Anzahl der Spalten des Spielfelds.
        /// </summary>
        public const int Columns = 10;

        /// <summary>
        /// Die Anzahl der Zeilen des Spielfelds.
        /// </summary>
        public const int Rows = 20;

        private readonly int[,] _board;

        /// <summary>
        /// Ruft das aktuelle fallende Tetromino ab.
        /// </summary>
        public Tetromino CurrentPiece { get; private set; }

        /// <summary>
        /// Ruft das nächste Tetromino ab.
        /// </summary>
        public Tetromino NextPiece { get; private set; }

        /// <summary>
        /// Ruft die X-Position des aktuellen Tetrominos ab.
        /// </summary>
        public int CurrentX { get; private set; }

        /// <summary>
        /// Ruft die Y-Position des aktuellen Tetrominos ab.
        /// </summary>
        public int CurrentY { get; private set; }

        /// <summary>
        /// Ruft den aktuellen Punktestand ab.
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// Ruft die aktuelle Stufe ab.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Ruft die Anzahl der gelöschten Zeilen ab.
        /// </summary>
        public int Lines { get; private set; }

        /// <summary>
        /// Ruft einen Wert ab, der angibt, ob das Spiel beendet ist.
        /// </summary>
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// Erstellt ein neues Spielfeld.
        /// </summary>
        public GameBoard()
        {
            _board = new int[Columns, Rows];
            Reset();
        }

        /// <summary>
        /// Setzt das Spielfeld auf den Anfangszustand zurück.
        /// </summary>
        public void Reset()
        {
            Array.Clear(_board, 0, _board.Length);
            Score = 0;
            Level = 1;
            Lines = 0;
            IsGameOver = false;
            NextPiece = Tetromino.GetRandom();
            SpawnPiece();
        }

        /// <summary>
        /// Erzeugt ein neues Tetromino in der Mitte des Spielfelds.
        /// </summary>
        private void SpawnPiece()
        {
            CurrentPiece = NextPiece;
            NextPiece = Tetromino.GetRandom();
            CurrentX = Columns / 2 - 2;
            CurrentY = 0;

            if (!CanPlace(CurrentPiece, CurrentX, CurrentY))
            {
                IsGameOver = true;
            }
        }

        /// <summary>
        /// Prüft, ob das aktuelle Tetromino an der angegebenen Position platziert werden kann.
        /// </summary>
        /// <param name="piece">Das zu prüfende Tetromino.</param>
        /// <param name="x">Die X-Position.</param>
        /// <param name="y">Die Y-Position.</param>
        /// <returns>True, wenn das Tetromino platziert werden kann.</returns>
        public bool CanPlace(Tetromino piece, int x, int y)
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (piece.Shape[r, c] == 0)
                        continue;

                    int boardX = x + c;
                    int boardY = y + r;

                    if (boardX < 0 || boardX >= Columns || boardY >= Rows)
                        return false;

                    if (boardY >= 0 && _board[boardX, boardY] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sperrt das aktuelle Tetromino in das Spielfeld ein.
        /// </summary>
        private void LockPiece()
        {
            int typeValue = (int)CurrentPiece.Type;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (CurrentPiece.Shape[r, c] != 0)
                    {
                        int boardX = CurrentX + c;
                        int boardY = CurrentY + r;
                        if (boardY >= 0 && boardY < Rows && boardX >= 0 && boardX < Columns)
                        {
                            _board[boardX, boardY] = typeValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Löscht alle vollständigen Zeilen und gibt die Anzahl zurück.
        /// </summary>
        /// <returns>Die Anzahl der gelöschten Zeilen.</returns>
        private int ClearLines()
        {
            int cleared = 0;
            for (int r = Rows - 1; r >= 0; r--)
            {
                bool full = true;
                for (int c = 0; c < Columns; c++)
                {
                    if (_board[c, r] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    cleared++;
                    for (int row = r; row > 0; row--)
                    {
                        for (int c = 0; c < Columns; c++)
                        {
                            _board[c, row] = _board[c, row - 1];
                        }
                    }
                    for (int c = 0; c < Columns; c++)
                    {
                        _board[c, 0] = 0;
                    }
                    r++;
                }
            }

            if (cleared > 0)
            {
                Lines += cleared;
                int[] points = { 0, 100, 300, 500, 800 };
                Score += points[cleared] * Level;
                Level = Lines / 10 + 1;
            }

            return cleared;
        }

        /// <summary>
        /// Bewegt das aktuelle Tetromino nach links.
        /// </summary>
        /// <returns>True, wenn die Bewegung erfolgreich war.</returns>
        public bool MoveLeft()
        {
            if (CanPlace(CurrentPiece, CurrentX - 1, CurrentY))
            {
                CurrentX--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bewegt das aktuelle Tetromino nach rechts.
        /// </summary>
        /// <returns>True, wenn die Bewegung erfolgreich war.</returns>
        public bool MoveRight()
        {
            if (CanPlace(CurrentPiece, CurrentX + 1, CurrentY))
            {
                CurrentX++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bewegt das aktuelle Tetromino nach unten oder sperrt es ein, falls es nicht weiter bewegt werden kann.
        /// </summary>
        /// <returns>True, wenn das Tetromino nach unten bewegt wurde; False, wenn es gesperrt wurde.</returns>
        public bool MoveDown()
        {
            if (CanPlace(CurrentPiece, CurrentX, CurrentY + 1))
            {
                CurrentY++;
                return true;
            }

            LockPiece();
            ClearLines();
            SpawnPiece();
            return false;
        }

        /// <summary>
        /// Dreht das aktuelle Tetromino um 90 Grad im Uhrzeigersinn.
        /// </summary>
        /// <returns>True, wenn die Rotation erfolgreich war.</returns>
        public bool Rotate()
        {
            var clone = CurrentPiece.Clone();
            clone.Rotate();

            if (CanPlace(clone, CurrentX, CurrentY))
            {
                CurrentPiece = clone;
                return true;
            }

            if (CanPlace(clone, CurrentX - 1, CurrentY))
            {
                CurrentX--;
                CurrentPiece = clone;
                return true;
            }

            if (CanPlace(clone, CurrentX + 1, CurrentY))
            {
                CurrentX++;
                CurrentPiece = clone;
                return true;
            }

            if (CanPlace(clone, CurrentX, CurrentY - 1))
            {
                CurrentY--;
                CurrentPiece = clone;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Führt einen harten Fall aus, bei dem das Tetromino sofort nach unten fällt.
        /// </summary>
        public void HardDrop()
        {
            int dropDistance = 0;
            while (CanPlace(CurrentPiece, CurrentX, CurrentY + 1))
            {
                CurrentY++;
                dropDistance++;
            }
            Score += dropDistance * 2;
            LockPiece();
            ClearLines();
            SpawnPiece();
        }

        /// <summary>
        /// Ruft den Wert an der angegebenen Position des Spielfelds ab.
        /// </summary>
        /// <param name="x">Die X-Position.</param>
        /// <param name="y">Die Y-Position.</param>
        /// <returns>Der Wert an der Position (0 = leer, 1-7 = Tetromino-Typ).</returns>
        public int GetCell(int x, int y)
        {
            return _board[x, y];
        }
    }
}
