using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChessUI.Enums;

namespace ChessUI.Engine
{
    public static class MoveEvaluation
    {
        const int pawnStructureValue = 10;
        const int passedPawnValue = 50;
        const int isolatedPawnValue = -20;

        const int controlValue = 2;

        const int kingMobilityValue = -3;

        public static int EvaluateBoard(int[] board)
        {
            int evaluation = 0;

            evaluation += MaterialDifference();
            evaluation += ControlledSquares();
            evaluation += EvaluatePawnStructure();
            evaluation += EvaluateKingSafety();

            return evaluation;
        }

        private static int MaterialDifference()
        {
            return BoardManager.WhitePiecePositions.TotalPieceValue - BoardManager.BlackPiecePositions.TotalPieceValue;
        }

        private static int ControlledSquares()
        {
            int whiteControlled = BitOperations.PopCount(BoardManager.WhiteBitboards.ControlledPositions);;
            int blackControlled = BitOperations.PopCount(BoardManager.BlackBitboards.ControlledPositions); ;
            
            return (whiteControlled - blackControlled) * controlValue;
        }
        private static int EvaluatePawnStructure()
        {
            int structure = 0;
            ulong whitePawns = BoardManager.WhiteBitboards.Pawns;
            ulong blackPawns = BoardManager.WhiteBitboards.Pawns;
            foreach (var position in BoardManager.WhitePiecePositions.Pawns)
            {
                structure += BitOperations.PopCount(LookUps.whitePawnAttackBitBoard[position] & whitePawns) * pawnStructureValue;

                bool isPassed = (LookUps.whitePassedPawnMasks[position] & blackPawns) == 0;
                if (isPassed) structure += passedPawnValue;

                bool isIsolated = (LookUps.isolatedPawnMasks[position] & blackPawns) == 0;
                if(isIsolated) structure += isolatedPawnValue;
            }

            foreach (var position in BoardManager.BlackPiecePositions.Pawns)
            {
                structure -= BitOperations.PopCount(LookUps.blackPawnAttackBitBoard[position] & whitePawns) * pawnStructureValue;

                bool isPassed = (LookUps.blackPassedPawnMasks[position] & whitePawns) == 0;
                if (isPassed) structure -= passedPawnValue;

                bool isIsolated = (LookUps.isolatedPawnMasks[position] & whitePawns) == 0;
                if(isIsolated) structure -= isolatedPawnValue;
            }

            return structure;
        }

        private static int EvaluateKingSafety()
        {
            int evaluation = 0;
            

            int whiteKingPos = BoardManager.WhitePiecePositions.King;
            int blackKingPos = BoardManager.BlackPiecePositions.King;

            ulong blockers = BoardManager.WhiteBitboards.AllPieces & LookUps.queenMoves[whiteKingPos];
            int whiteMobility = BitOperations.PopCount(LookUps.GetPosibleQueenMoves(whiteKingPos, blockers));
            int blackMobility = BitOperations.PopCount(LookUps.GetPosibleQueenMoves(blackKingPos, blockers));

            evaluation += (whiteMobility - blackMobility) * kingMobilityValue;

            return evaluation;
        }

        public static IEnumerable<Move> MoveOrdering(IEnumerable<Move> unorderedMoves)
        {
            List<Move> captureMoves = new(8);
            List<Move> promotionCaptureMoves = new(4);
            List<Move> promotionMoves = new(4);
            List<Move> ordinaryMoves = new(16);

            foreach (Move move in unorderedMoves)
            {
                switch (move.GetMoveType())
                {
                    case MoveType.capture:
                        if (!move.IsPromotion())
                        {
                            promotionCaptureMoves.Add(move);
                            break;
                        }
                        captureMoves.Add(move);
                        break;
                    case MoveType.promotion:
                        promotionMoves.Add(move);
                        break;
                    case MoveType.enPesant:
                        captureMoves.Add(move);
                        break;
                    default:
                        ordinaryMoves.Add(move);
                        break;
                }
            }

            foreach (Move move in promotionCaptureMoves)
            {
                yield return move;
            }
            foreach (Move move in promotionMoves)
            {
                yield return move;
            }
            foreach (Move move in CaptureOrdering(captureMoves))
            {
                yield return move;
            }
            foreach (Move move in ordinaryMoves)
            {
                yield return move;
            }
        }

        public static IEnumerable<Node> MoveOrdering(Node previousEvaluation, bool isWhite)
        {
            if (isWhite)
            {
                return previousEvaluation.children.OrderByDescending(n => n.evaluation);
            }
            return previousEvaluation.children.OrderBy(n => n.evaluation);
        }

        private static IEnumerable<Move> CaptureOrdering(List<Move> captureMoves)
        {
            (int, int)[] moveValuesDeltas = new (int, int)[captureMoves.Count];
            int[] board = BoardManager.Board;
            int idx = 0;
            foreach (Move move in captureMoves)
            {
                int capturingPiece = board[move.SourceSquare];
                int capturedPiece;
                if (move.IsType(MoveType.enPesant))
                {
                    if (Piece.IsPieceWhite(capturingPiece))
                    {
                        capturedPiece = board[move.TargetSquare - 8];
                    }
                    else
                    {
                        capturedPiece = board[move.TargetSquare + 8];
                    }
                }
                else
                {
                    capturedPiece = board[move.TargetSquare];
                }
                int capturingValue = Piece.GetPieceValue(capturingPiece);
                int capturedValue = Piece.GetPieceValue(capturedPiece);
                int valueDelta = capturingValue - capturedValue;

                moveValuesDeltas[idx] = (valueDelta, idx);
                idx++;
            }

            Array.Sort(moveValuesDeltas);
            foreach ((_, int index) in moveValuesDeltas)
            {
                yield return captureMoves[index];
            }

        }
    }
}
