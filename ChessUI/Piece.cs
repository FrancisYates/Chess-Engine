using ChessUI.Enums;
using System;

namespace ChessUI
{
    public static class Piece
    {

        public static bool IsSameColour(int piece1, int piece2)
        {
            return (piece1 & 8) == (piece2 & 8);
        }

        public static bool IsPieceColour(int pieceColour, int playColour)
        {
            return(pieceColour & 8) == playColour;
        }
        public static bool IsPieceWhite(int pieceColour)
        {
            return (pieceColour & 8) == 8;
        }

        public static bool IsSlidingPiece(int piece)
        {
            return (piece & 4) == 4;
        }

        public static bool IsType(int piece, PieceType pieceType)
        {
            return (piece & 7) == (int)pieceType;
        }

        public static PieceType GetPieceType(int piece) => (PieceType)(piece & 7);

        public static bool IsAtFinalRank(bool isWhite, int targetSquare)
        {
            int finalRank = isWhite ? 7 : 0;
            int targetRank = targetSquare / 8;

            return targetRank == finalRank;
        }

        public static bool HasPawnMoved(bool isWhite, int currentPosition)
        {
            int expectedRank = isWhite ? 1 : 6;
            int currentRank = currentPosition / 8;

            return currentRank != expectedRank;
        }

        public static bool HasRookMoved(bool isWhite, int currentPosition)
        {
            int expectedRank = isWhite ? 0 : 7;
            int currentRank = currentPosition / 8;
            if (currentRank == expectedRank)
            {
                int currentFile = currentPosition % 8;
                return !(currentFile == 0 || currentFile == 7);
            }
            return false;
        }

        public static bool IsOpponentInSquare(bool isWhite, int pieceInSquare)
        {
            int opponentColour = isWhite ? 0 : 8;
            if(pieceInSquare == 0) { return false; }
            return IsPieceColour(pieceInSquare, opponentColour);
        }

        public static int GetPieceValue(int piece)
        {

            if (IsType(piece, PieceType.Pawn))
            {
                return 100;
            }
            else if (IsType(piece, PieceType.Rook))
            {
                return 500;
            }
            else if (IsType(piece, PieceType.Bishop))
            {
                return 300;
            }
            else if (IsType(piece, PieceType.Knight))
            {
                return 300;
            }
            else if (IsType(piece, PieceType.King))
            {
                return 100000;
            }
            else if (IsType(piece, PieceType.Queen))
            {
                return 900;
            }
            return 0;
        }

        internal static char GetPieceCharacterRepresentation(int piece)
        {
            char pieceChar = piece switch
            {
                1 => 'p',
                2 => 'n',
                3 => 'k',
                5 => 'r',
                6 => 'b',
                7 => 'q',
                9 => 'P',
                10 => 'N',
                11 => 'K',
                13 => 'R',
                14 => 'B',
                15 => 'Q',
                _ => throw new NotImplementedException()
            };
            return pieceChar;
        }
    }
}
