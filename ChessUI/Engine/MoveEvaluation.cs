﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChessUI.Enums;

namespace ChessUI.Engine
{
    public static class MoveEvaluation
    {

        public static int EvaluateBoard(int[] board)
        {
            int evaluation = 0;

            evaluation += MaterialDifference(board);
            evaluation += ControlledSquares();
            evaluation += EvaluatePawnStructure(board);

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
            int whiteControlled = BitOperations.PopCount(BoardManager.WhiteBitboards.ControlledPositions);;
            int blackControlled = BitOperations.PopCount(BoardManager.BlackBitboards.ControlledPositions); ;
            
            return whiteControlled - blackControlled;
        }
        private static int EvaluatePawnStructure(int[] board) {
            int whiteStructure = 0;
            int blackStructure = 0;
            for (int i = 0; i < 64; i++) {
                int piece = board[i];
                if(Piece.IsType(piece, PieceType.Pawn)) {
                    int pieceSide = Piece.IsPieceWhite(piece) ? 1 : 0; //1 if white, 0 if black
                    int[] attackOffsets = LookUps.pawnAttackOffset[pieceSide, i];
                    var defended = attackOffsets.Count(offset => {
                        var targetPiece = board[i+ offset];
                        return Piece.IsType(targetPiece, PieceType.Pawn) && Piece.IsSameColour(targetPiece, piece);
                        });
                    whiteStructure += defended * pieceSide;
                    blackStructure += defended * 1 - pieceSide;
                }
            }

            return whiteStructure - blackStructure;
        }

        public static IEnumerable<Move> MoveOrdering(IEnumerable<Move> unorderedMoves)
        {
            List<Move> captureMoves = new();
            List<Move> promotionCaptureMoves = new();
            List<Move> promotionMoves = new();
            List<Move> ordinaryMoves = new();

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

        public static IEnumerable<Node> MoveOrderingID(Node previousEvaluation)
        {
            return previousEvaluation.children.OrderByDescending(n => n.evaluation);
        }

        private static IEnumerable<Move> CaptureOrdering(List<Move> captureMoves)
        {
            (int, int)[] moveValuesDeltas = new (int, int)[captureMoves.Count];
            int[] board = BoardManager.Board;
            int idx = 0;
            foreach (Move move in captureMoves)
            {
                int capturingPiece = board[move.sourceSquare];
                int capturedPiece;
                if (move.IsType(MoveType.enPesant))
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
