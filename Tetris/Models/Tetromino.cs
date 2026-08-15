using System;

namespace Tetris.Models
{
    /// <summary>
    /// Definiert die sieben Tetromino-Typen.
    /// </summary>
    public enum TetrominoType
    {
        /// <summary>I-Tetromino</summary>
        I = 1,
        /// <summary>O-Tetromino</summary>
        O = 2,
        /// <summary>T-Tetromino</summary>
        T = 3,
        /// <summary>S-Tetromino</summary>
        S = 4,
        /// <summary>Z-Tetromino</summary>
        Z = 5,
        /// <summary>J-Tetromino</summary>
        J = 6,
        /// <summary>L-Tetromino</summary>
        L = 7
    }

    /// <summary>
    /// Stellt einen einzelnen Tetromino-Baustein mit Form, Farbe und Rotation dar.
    /// </summary>
    public sealed class Tetromino
    {
        private static readonly int[,,] Shapes =
        {
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 1, 1 },
                { 0, 1, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 1, 0 },
                { 1, 1, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 1, 1, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 1, 0, 0, 0 },
                { 1, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 1, 0 },
                { 1, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            }
        };

        private static readonly string[] Colors =
        {
            "#00F0F0",
            "#F0F000",
            "#A000F0",
            "#00F000",
            "#F00000",
            "#0000F0",
            "#F0A000"
        };

        /// <summary>
        /// Ruft den Typ dieses Tetrominos ab.
        /// </summary>
        public TetrominoType Type { get; }

        /// <summary>
        /// Ruft die aktuelle Form als 4x4-Raster ab.
        /// </summary>
        public int[,] Shape { get; private set; }

        /// <summary>
        /// Ruft die Farbe dieses Tetrominos als Hexadezimalzeichenfolge ab.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Erstellt einen neuen Tetromino des angegebenen Typs.
        /// </summary>
        /// <param name="type">Der Tetromino-Typ.</param>
        public Tetromino(TetrominoType type)
        {
            Type = type;
            int index = (int)type - 1;
            Shape = new int[4, 4];
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    Shape[r, c] = Shapes[index, r, c];
                }
            }
            Color = Colors[index];
        }

        /// <summary>
        /// Dreht die Form um 90 Grad im Uhrzeigersinn.
        /// </summary>
        public void Rotate()
        {
            int[,] rotated = new int[4, 4];
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    rotated[c, 3 - r] = Shape[r, c];
                }
            }
            Shape = rotated;
        }

        /// <summary>
        /// Erstellt eine Kopie dieses Tetrominos.
        /// </summary>
        /// <returns>Eine neue Tetromino-Instanz mit derselben Form.</returns>
        public Tetromino Clone()
        {
            var clone = new Tetromino(Type);
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    clone.Shape[r, c] = Shape[r, c];
                }
            }
            return clone;
        }

        /// <summary>
        /// Erstellt einen zufälligen Tetromino.
        /// </summary>
        /// <returns>Ein neuer Tetromino mit zufälligem Typ.</returns>
        public static Tetromino GetRandom()
        {
            var values = Enum.GetValues(typeof(TetrominoType));
            var type = (TetrominoType)values.GetValue(new Random().Next(values.Length));
            return new Tetromino(type);
        }
    }
}
