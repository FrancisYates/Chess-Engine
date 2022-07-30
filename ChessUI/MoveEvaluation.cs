using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class MoveEvaluation
    {

        public static int EvaluateBoard(int[] board)
        {
            int evaluation = 0;

            evaluation += MaterialDifference(board);
            evaluation += ControlledSquares();

            return evaluation;
        }

        private static int MaterialDifference(int[] board)
        {
            int whiteMaterial = 0;
            int blackMaterial = 0;

            for (int i = 0; i < 64; i++)
            {
                int piece = board[i];
                if (piece == 0)
                {
                    continue;
                }
                if (Piece.IsPieceWhite(piece))
                {
                    whiteMaterial += GetPieceValue(piece);
                }
                else
                {
                    blackMaterial += GetPieceValue(piece);
                }
            }

            return whiteMaterial - blackMaterial;

            int GetPieceValue(int piece)
            {
                if(Piece.IsType(piece, Piece.PieceType.Pawn))
                {
                    return 100;
                }
                else if(Piece.IsType(piece, Piece.PieceType.Rook))
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
        }

        private static int ControlledSquares()
        {
            int whiteControlled = 0;
            int blackControlled = 0;

            for (int i = 0; i < 64; i++)
            {
                int position = BoardManager.attackPositionBoard[i];
                whiteControlled += (position & 2) / 2;
                blackControlled += position & 1;
            }

            return whiteControlled - blackControlled;
        }

    }
}
