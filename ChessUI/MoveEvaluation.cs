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
                    whiteMaterial += Piece.GetPieceValue(piece);
                }
                else
                {
                    blackMaterial += Piece.GetPieceValue(piece);
                }
            }

            return whiteMaterial - blackMaterial;            
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

        public static Move[] MoveOrdering(Move[] unorderedMoves)
        {
            List<Move> captureMoves = new List<Move>();
            List<Move> promotionCaptureMoves = new List<Move>();
            List<Move> promotionMoves = new List<Move>();
            List<Move> ordinaryMoves = new List<Move>();

            foreach (Move move in unorderedMoves)
            {
                switch (move.moveType)
                {
                    case Move.MoveType.capture:
                        captureMoves.Add(move);
                        break;
                    case Move.MoveType.enPesant:
                        captureMoves.Add(move);
                        break;
                    case Move.MoveType.promotionQueen:
                        promotionMoves.Add(move);
                        break;
                    case Move.MoveType.promotionRook:
                        promotionMoves.Add(move);
                        break;
                    case Move.MoveType.promotionBishop:
                        promotionMoves.Add(move);
                        break;
                    case Move.MoveType.promotionKnight:
                        promotionMoves.Add(move);
                        break;
                    case Move.MoveType.promotionQueenCapture:
                        promotionCaptureMoves.Add(move);
                        break;
                    case Move.MoveType.promotionRookCapture:
                        promotionCaptureMoves.Add(move);
                        break;
                    case Move.MoveType.promotionBishopCapture:
                        promotionCaptureMoves.Add(move);
                        break;
                    case Move.MoveType.promotionKnightCapture:
                        promotionCaptureMoves.Add(move);
                        break;
                    default:
                        ordinaryMoves.Add(move);
                        break;
                }
            }

            List<Move> orderedMoves = promotionCaptureMoves;
            captureMoves = CaptureOrdering(captureMoves);
            orderedMoves = orderedMoves.Concat(captureMoves).ToList();
            orderedMoves = orderedMoves.Concat(promotionMoves).ToList();
            orderedMoves = orderedMoves.Concat(ordinaryMoves).ToList();

            return orderedMoves.ToArray();
        }

        private static List<Move> CaptureOrdering(List<Move> captureMoves)
        {
            (int, int)[] x = new (int, int)[captureMoves.Count];
            int[] board = BoardManager.GetBoard();
            int idx = 0;
            foreach(Move move in captureMoves)
            {
                int capturingPiece = board[move.sourceSquare];
                int capturedPiece;
                if (move.moveType == Move.MoveType.enPesant)
                {
                    if (Piece.IsPieceWhite(capturingPiece))
                    {
                        capturedPiece = board[move.targetSquare - 8];
                    }
                    else
                    {
                        capturedPiece = board[move.targetSquare + 8];
                    }
                }
                else
                {
                    capturedPiece = board[move.targetSquare];
                }
                int capturingValue = Piece.GetPieceValue(capturingPiece);
                int capturedValue = Piece.GetPieceValue(capturedPiece);
                int valueDelta = capturingValue - capturedValue;
                x[idx] = (valueDelta, idx);
                idx++;
            }

            List<Move> sortedCaptures = new List<Move>();
            Array.Sort(x);
            foreach ((_, int index) in x)
            {
                sortedCaptures.Add(captureMoves[index]);
            }

            return sortedCaptures;
        }
    }
}
