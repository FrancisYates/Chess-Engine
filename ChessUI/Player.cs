using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    internal static class Player
    {

        public static bool IsValidSelection(int[] board, int selectedSquare)
        {
            int selectedPiece = board[selectedSquare];

            if (selectedPiece == 0)
            {
                return false;
            }
            if (Piece.IsPieceWhite(selectedPiece) != BoardManager.whiteToMove)
            {
                return false;
            }
            return true;
        }

        public static bool IsMoveValid( ref Move proposedMove)
        {
            Move[] moves = MoveGeneration.GenerateStricLegalMoves(BoardManager.whiteToMove);
            foreach (Move move in moves)
            {
                if (proposedMove.ToString() == move.ToString()) 
                {
                    proposedMove.moveType = move.moveType;
                    return true; 
                }
            }
            return false;
        }
    }
}
