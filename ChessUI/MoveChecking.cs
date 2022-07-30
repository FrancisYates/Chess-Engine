using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    internal static class MoveChecking
    {
        public static bool CheckValidMove(int[] board, Move move)
        {
            int toMoveColour = BoardManager.whiteToMove ? 8 : 0;
            int sourcePiece = board[move.sourceSquare];
            int targetPiece = board[move.targetSquare];

            if (Piece.IsPieceColour(targetPiece, toMoveColour))
            {
                return false;
            }

            if (Piece.IsType(sourcePiece, Piece.PieceType.Pawn))
            {
                return IsValidPawnMove(move, board);
            }
            if (Piece.IsSlidingPiece(sourcePiece))
            {
                return IsValidSlidingPieceMove(move, board);
            }
            if (Piece.IsType(sourcePiece, Piece.PieceType.Knight))
            {
                return IsValidKnightMove(move, board);
            }
            if (Piece.IsType(sourcePiece, Piece.PieceType.King))
            {
                return IsValidKingMove(move, board);
            }

            return false;
        }

        private static bool IsValidPawnMove(Move move, int[] board)
        {
            int pieceAtSource = board[move.sourceSquare];
            bool isWhite = Piece.IsPieceColour(pieceAtSource, 8); // check if piece is white. Enum is available but may be slower 
            int moveDirection = isWhite ? 1 : -1;

            int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
            int yDelta = move.targetSquare / 8 - move.sourceSquare / 8;

            if (Math.Abs(xDelta) > 1 || Math.Abs(yDelta) > 2 || (Math.Abs(xDelta) == 1 && Math.Abs(yDelta) != 1))
            {
                return false;
            }

            if ((isWhite && yDelta < 1) || (!isWhite && yDelta > -1) )
            {
                return false;
            }

            int pieceAtTarget = board[move.targetSquare];
            if (Math.Abs(xDelta) == 1)
            {
                return Piece.IsOpponentInSquare(isWhite, pieceAtTarget);
            }

            bool squareAheadClear = board[move.sourceSquare + 8 * moveDirection] == 0;
            if (Math.Abs(yDelta) == 1)
            {
                return squareAheadClear;
            }

            return squareAheadClear && board[move.targetSquare] == 0;
        }

        private static bool IsValidKnightMove(Move move, int[] board)
        {
            int[] offsets = { 15, 17, 10, -6, -15, -17, -10, 6 };
            int delta = move.sourceSquare - move.targetSquare;

            if (!offsets.Contains(delta))
            {
                return false;
            }

            int sourcePiece = board[move.sourceSquare];
            int targetPiece = board[move.targetSquare];
            return !Piece.IsSameColour(sourcePiece, targetPiece);
        }

        private static bool IsValidKingMove(Move move, int[] board)
        {
            int[] directionOffsets = { 8, -8, -1, 1, 7, 9, -7, 9, 2, -2};
            int delta = move.sourceSquare - move.targetSquare;
            if (!directionOffsets.Contains(delta))
            {
                return false;
            }

            int sourcePiece = board[move.sourceSquare];
            int targetPiece = board[move.targetSquare];
            if (delta != 2 || delta != -2)
            {
                return !Piece.IsSameColour(sourcePiece, targetPiece);
            }

            bool isWhite = Piece.IsPieceColour(sourcePiece, 8); // check if piece is white. Enum is available but may be slower 
            if (delta == 2)
            {
                return MoveGeneration.CanCastleKingSide(isWhite, move.sourceSquare, board);
            }
            if (delta == -2)
            {
                return MoveGeneration.CanCastleQueenSide(isWhite, move.sourceSquare, board);
            }

            return false;
        }

        private static bool IsValidSlidingPieceMove(Move move, int[] board)
        {
            int sourcePiece = board[move.sourceSquare];

            if(Piece.IsType(sourcePiece, Piece.PieceType.Rook))
            {
                return IsValidHorizontalMove(move, board);
            }
            if(Piece.IsType(sourcePiece, Piece.PieceType.Bishop))
            {
                return IsValidDiagonalMove(move, board);
            }

            return IsValidHorizontalMove(move, board) || IsValidDiagonalMove(move, board);
        }

        private static bool IsValidHorizontalMove(Move move, int[] board)
        {
            int xDelta = Math.Abs(move.sourceSquare % 8 - move.targetSquare % 8);
            int yDelta = Math.Abs(move.sourceSquare / 8 - move.targetSquare / 8);

            if (xDelta > 0 && yDelta > 0)
            {
                return false;
            }

            int delta = move.targetSquare - move.sourceSquare;
            int directionOffset = FindTravelDirectionOffset(move);
            int squaresInDirection = delta / directionOffset;

            for (int i = 1; i < squaresInDirection; i++)
            {
                int intermediateSquare = move.sourceSquare + i * directionOffset;
                int pieceAtIntermediate = board[intermediateSquare];
                if (pieceAtIntermediate != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValidDiagonalMove(Move move, int[] board)
        {
            int xDelta = Math.Abs(move.sourceSquare % 8 - move.targetSquare % 8);
            int yDelta = Math.Abs(move.sourceSquare / 8 - move.targetSquare / 8);

            if(xDelta != yDelta)
            {
                return false;
            }

            int delta = move.targetSquare - move.sourceSquare;
            int directionOffset = FindTravelDirectionOffset(move);
            int squaresInDirection = delta / directionOffset;

            for(int i = 1;i < squaresInDirection; i++)
            {
                int intermediateSquare = move.sourceSquare + i * directionOffset;
                int pieceAtIntermediate = board[intermediateSquare];
                if(pieceAtIntermediate != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static int FindTravelDirectionOffset(Move move)
        {
            int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
            int yDelta = move.targetSquare / 8 - move.sourceSquare / 8;

            int horizontalOffset;
            if (xDelta >= 0)
            {
                horizontalOffset = xDelta == 0? 0: 1;
            }
            else
            {
                horizontalOffset = -1;
            }

            int verticalOffset;
            if (yDelta >= 0)
            {
                verticalOffset = yDelta == 0 ? 0 : 8;
            }
            else
            {
                verticalOffset = -8;
            }

            return horizontalOffset + verticalOffset;
        }
    }
}

