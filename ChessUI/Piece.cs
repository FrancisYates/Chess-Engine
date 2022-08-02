using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    internal static class Piece
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

        public static PieceType GetPieceType(int piece)
        {
            if(IsType(piece, PieceType.Pawn)) { return PieceType.Pawn; }
            if (IsType(piece, PieceType.Rook)) { return PieceType.Rook; }
            if (IsType(piece, PieceType.Knight)) { return PieceType.Knight; }
            if (IsType(piece, PieceType.Bishop)) { return PieceType.Bishop; }
            if (IsType(piece, PieceType.Queen)) { return PieceType.Queen; }
            return PieceType.King;
        }

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

            if (Piece.IsType(piece, Piece.PieceType.Pawn))
            {
                return 100;
            }
            else if (Piece.IsType(piece, Piece.PieceType.Rook))
            {
                return 500;
            }
            else if (Piece.IsType(piece, Piece.PieceType.Bishop))
            {
                return 300;
            }
            else if (Piece.IsType(piece, Piece.PieceType.Knight))
            {
                return 300;
            }
            else if (Piece.IsType(piece, Piece.PieceType.King))
            {
                return 100000;
            }
            else if (Piece.IsType(piece, Piece.PieceType.Queen))
            {
                return 900;
            }
            return 0;
        }

        public enum PieceType
        {
            Pawn = 0b_0000_0001,
            Knight = 0b_0000_0010,
            King = 0b_0000_0011,
            Rook = 0b_0000_0101,
            Bishop = 0b_0000_0110,
            Queen = 0b_0000_0111
        }
        public enum Colour
        {
            White = 0b_0000_1000,
            Black = 0b_0000_0000,
        }
    }
}
