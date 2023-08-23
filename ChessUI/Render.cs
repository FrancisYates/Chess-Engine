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
            int idx = 0;

            foreach (Button btn in buttons)
            {
                int pieceAtPosition = board[idx];
                //string text = GetSquareString(pieceAtPosition);
                string img = GetSquarePieceImg(pieceAtPosition);
                //SetButtonText(btn, text);
                SetButtonImg(btn, img);

                idx++;
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

        private static void SetButtonImg(Button btn, string img)
        {
            btn.Content = new Image
            {
                Source = new BitmapImage(new Uri("\\Images\\" + img, UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.UniformToFill,
                Height = 60,
                Width = 60
            };
        }

        private static string GetSquarePieceImg(int piece)
        {
            switch (piece)
            {
                case 0:
                    return "";
                case 9:
                    return "wPawn.png";
                case 1:
                    return "bPawn.png";
                case 10:
                    return "wKnight.png";
                case 2:
                    return "bKnight.png";
                case 11:
                    return "wKing.png";
                case 3:
                    return "bKing.png";
                case 13:
                    return "wRook.png";
                case 5:
                    return "bRook.png";
                case 14:
                    return "wBishop.png";
                case 6:
                    return "bBishop.png";
                case 15:
                    return "wQueen.png";
                case 7:
                    return "bQueen.png";
                default:
                    return "";
            }
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
