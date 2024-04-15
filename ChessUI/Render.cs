using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    internal static class Render
    {
        public static void UpdateBoard(List<Button> buttons, int[] board)
        {
            for (int i = 0; i < buttons.Count; i++) {
                UpdateSquareAtIndex(buttons, board, i);
            }
        }
        public static void UpdateBoard(List<Button> buttons, int[] board, Move move) {
            UpdateSquareAtIndex(buttons, board, move.sourceSquare);
            UpdateSquareAtIndex(buttons, board, move.targetSquare);
        }
        
        public static void RenderBitBoardBoard(List<Button> buttons, ulong bitBoard, SolidColorBrush brushColour) {
            StringBuilder bb = new();
            for (int i = 0;i < buttons.Count;i++) {
                bb.Append(bitBoard & 0b_1ul);
                if ((bitBoard & 0b_1ul) == 1) {
                    Button selectedButton = buttons[i];
                    selectedButton.Background = brushColour;
                }

                bitBoard >>= 1;
            }
            Debug.WriteLine($"Rendering bitboard {bb.ToString()}");
        }

        public static void UpdateSquareAtIndex(List<Button> buttons, int[] board, int index)
        {
            int pieceAtPosition = board[index];
            string img = GetSquarePieceImg(pieceAtPosition);
            SetButtonImg(buttons[index], img, index.ToString());
            bool isFileOdd = (index % 8) % 2 == 1;
            bool isRankOdd = (index / 8) % 2 == 1;
            if (isRankOdd ^ isFileOdd)
            {
                buttons[index].Background = Brushes.Beige;
            }
            else
            {
                buttons[index].Background = Brushes.SaddleBrown;
            }
        }


        public static void HighlightSquare(List<Button> buttons, int position)
        {
            Button selectedButton = buttons[position];
            selectedButton.Background = Brushes.Green;
        }

        public static void HighlightPossibleMoves(List<Button> buttons, IEnumerable<Move> moves)
        {
            foreach (Move move in moves)
            {
                Button selectedButton = buttons[move.targetSquare];
                selectedButton.Background = Brushes.LightGreen;
            }
        }
        public static void RemovePossibleMovesHighlight(List<Button> buttons, IEnumerable<Move> moves)
        {
            foreach (Move move in moves)
            {
                bool isFileOdd = (move.targetSquare % 8) % 2 == 1;
                bool isRankOdd = (move.targetSquare / 8) % 2 == 1;
                Button selectedButton = buttons[move.targetSquare];
                if (isRankOdd ^ isFileOdd)
                {
                    selectedButton.Background = Brushes.Beige;
                }
                else
                {
                    selectedButton.Background = Brushes.SaddleBrown;
                }
            }
        }

        public static void Reset(List<Button> buttons) {
            for (int i = 0; i < 64; i++) {
                bool isFileOdd = (i % 8) % 2 == 1;
                bool isRankOdd = (i / 8) % 2 == 1;
                Button selectedButton = buttons[i];
                if (isRankOdd ^ isFileOdd) {
                    selectedButton.Background = Brushes.Beige;
                } else {
                    selectedButton.Background = Brushes.SaddleBrown;
                }
            }
        }

        public static void RemoveHighlightFromSquare(List<Button> buttons, int position)
        {
            bool isFileOdd = (position % 8) % 2 == 1;
            bool isRankOdd = (position / 8) % 2 == 1;

            Button selectedButton = buttons[position];
            if (isRankOdd ^ isFileOdd)
            {
                selectedButton.Background = Brushes.Beige;
            }
            else
            {
                selectedButton.Background = Brushes.SaddleBrown;
            }
        }

        private static void SetButtonText(Button btn, string text)
        {
            btn.Content = text;
        }

        private static void SetButtonImg(Button btn, string img, string id = "")
        {
            Image image = new()
            {
                Source = new BitmapImage(new Uri("\\Images\\" + img, UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.UniformToFill,
                Height = 60,
                Width = 60
            };
            Grid grid = new();
            grid.Children.Add(image);
#if DEBUG
            TextBlock txt = new();
            txt.Text = id;
            grid.Children.Add(txt);
#endif
            btn.Content = grid;
        }

        private static string GetSquarePieceImg(int piece)
        {
            return piece switch {
                0 => "",
                9 => "wPawn.png",
                1 => "bPawn.png",
                10 => "wKnight.png",
                2 => "bKnight.png",
                11 => "wKing.png",
                3 => "bKing.png",
                13 => "wRook.png",
                5 => "bRook.png",
                14 => "wBishop.png",
                6 => "bBishop.png",
                15 => "wQueen.png",
                7 => "bQueen.png",
                _ => "",
            };
        }

        private static string GetSquareString(int piece)
        {
            switch (piece)
            {
                case 0:
                    return "";
                case 9:
                    return "White\nPawn";
                case 1:
                    return "Black\nPawn";
                case 10:
                    return "White\nKnight";
                case 2:
                    return "Black\nKnight";
                case 11:
                    return "White\nKing";
                case 3:
                    return "Black\nKing";
                case 13:
                    return "White\nRook";
                case 5:
                    return "Black\nRook";
                case 14:
                    return "White\nBishop";
                case 6:
                    return "Black\nBishop";
                case 15:
                    return "White\nQueen";
                case 7:
                    return "Black\nQueen";
                default:
                    return "";
            }
        }
    }
}
