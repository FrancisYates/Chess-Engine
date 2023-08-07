using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class Player
    {

        public static bool IsValidSelection(int[] board, int selectedSquare)
        {
            int selectedPiece = board[selectedSquare];

            if (selectedPiece == 0)
            {
                return false;
            }
            if (Piece.IsPieceWhite(selectedPiece) != BoardManager.WhiteToMove)
            {
                return false;
            }
            return true;
        }

        public static bool IsMoveValid( ref Move proposedMove)
        {
            Move[] moves = MoveGeneration.GenerateStrictLegalMoves(BoardManager.WhiteToMove);
            foreach (Move move in moves)
            {
                if (proposedMove.sourceSquare == move.sourceSquare && proposedMove.targetSquare == move.targetSquare) 
                {
                    proposedMove.moveFlag = move.moveFlag;
                    return true; 
                }
            }
            return false;
        }
    }
}
