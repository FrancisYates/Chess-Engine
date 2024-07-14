using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessUI.Engine;

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
            List<Move> moves = MoveGeneration.GenerateStrictLegalMoves(BoardManager.WhiteToMove);
            foreach (Move move in moves)
            {
                if (proposedMove.SourceSquare == move.SourceSquare && proposedMove.TargetSquare == move.TargetSquare) 
                {
                    proposedMove.MoveFlag = move.MoveFlag;
                    return true; 
                }
            }
            return false;
        }
    }
}
