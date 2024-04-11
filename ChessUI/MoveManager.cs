using ChessUI.Engine;
using ChessUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class MoveManager
    {
        public static (int target, int validCastling) MakeMove(Move move, int[] board)
        {
            BoardManager.EnPesantSquare = -1;
            int targetContents = board[move.targetSquare];
            int movedPiece = board[move.sourceSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            int validCastling = BoardManager.CastleingRights;

            //UpdatePiecePositions(move);
            if (move.IsType(MoveType.promotion))
            {
                PromotionMove(move, side, board);
            }
            else if (move.IsType(MoveType.doublePawnMove)) {
                UpdatePawnBitboard(move, isWhite);
                int offset = isWhite ? -8 : 8;
                BoardManager.EnPesantSquare = move.targetSquare + offset;

                board[move.targetSquare] = board[move.sourceSquare];
                board[move.sourceSquare] = 0;
            }
            else if (move.IsType(MoveType.enPesant)) {
                UpdatePawnBitboard(move, isWhite);
                ulong caturedPieceMask = (0b_1ul << move.targetSquare);
                UpdatePawnBitboard(caturedPieceMask, !isWhite);
                int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;

                targetContents = board[move.sourceSquare + xDelta];
                board[move.targetSquare] = board[move.sourceSquare];
                board[move.sourceSquare] = 0;
                board[move.sourceSquare + xDelta] = 0;
            }
            else if (move.IsType(MoveType.castle))
            {
                CastleMove(move, isWhite, board);
            }
            else
            {
                targetContents = StandardMove(move, isWhite, movedPiece, board);
            }
            BoardManager.UpdateAttackedPositions(true);
            BoardManager.UpdateAttackedPositions(false);
            return (targetContents, validCastling);
        }

        private static void UpdatePawnBitboard(Move move, bool updateWhite = true) {
            ulong xorMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
            UpdatePawnBitboard(xorMask, updateWhite);
        }
        
        private static void UpdatePawnBitboard(ulong xorMask, bool updateWhite = true) {
            if (updateWhite) {
                BoardManager.WhitePawnBitboard ^= xorMask;
            } else {
                BoardManager.BlackPawnBitboard ^= xorMask;
            }
        }

        private static void PromotionMove(Move move, int side, int[] board) {
            ulong xorMask = (0b_1ul << move.sourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            PieceType type = move.GetPromotionPiece();
            board[move.targetSquare] = (int)type | side;
            board[move.sourceSquare] = 0;
        }
        private static void CastleMove(Move move, bool isWhite, int[] board)
        {
            bool kingSide = move.sourceSquare - move.targetSquare < 0;
            int side = isWhite ? 8 : 0;

            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                board[move.sourceSquare + 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                board[move.sourceSquare - 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
            }

            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;
            int castleingChange = isWhite ? 0b_0011 : 0b_1100;
            BoardManager.CastleingRights &= castleingChange;

        }

        private static int StandardMove(Move move, bool isWhite, int movedPiece, int[] board)
        {
            int target = board[move.targetSquare];
            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;

            if (Piece.IsType(movedPiece, PieceType.Rook))
            {
                if (!Piece.HasRookMoved(isWhite, move.sourceSquare))
                {
                    int whiteOrBlack = isWhite ? 0b_0011 : 0b_1100;
                    int rookKingSideSquare = isWhite ? 7 : 63;
                    int kingOrQueen = move.sourceSquare == rookKingSideSquare ? 0b_0101 : 0b_1010;
                    int newCastleRights = kingOrQueen | whiteOrBlack;
                    BoardManager.CastleingRights &= newCastleRights;
                }
            }
            else if (Piece.IsType(movedPiece, PieceType.King))
            {
                int castleingChange = isWhite ? 0b_0011 : 0b_1100;
                BoardManager.CastleingRights &= castleingChange;
            }else if(Piece.IsType(movedPiece, PieceType.Pawn)) {
                UpdatePawnBitboard(move, isWhite);
            }
            if(Piece.IsType(target, PieceType.Pawn)) {
                ulong caturedPieceMask = (0b_1ul << move.targetSquare);
                UpdatePawnBitboard(caturedPieceMask, !isWhite);
            }

            return target;
        }

        public static void UndoMove(Move move, int priorTargetContent, int priorCastlingRights, int[] board)
        {
            int movedPiece = board[move.targetSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            BoardManager.CastleingRights = priorCastlingRights;

            if (move.IsType(MoveType.promotion))
            {
                UndoPromotionMove(move, side, priorTargetContent, board);
                return;
            }else if (move.IsType(MoveType.doublePawnMove)) {
                UpdatePawnBitboard(move, side == (int)Colour.White);
                int offset = isWhite ? -8 : 8;
                BoardManager.EnPesantSquare = move.targetSquare + offset;

                board[move.sourceSquare] = board[move.targetSquare];
                board[move.targetSquare] = 0;
                return;
            }else if (move.IsType(MoveType.enPesant)) {
                UpdatePawnBitboard(move, isWhite);
                ulong caturedPieceMask = (0b_1ul << move.targetSquare);
                UpdatePawnBitboard(caturedPieceMask, !isWhite);

                int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
                board[move.sourceSquare] = board[move.targetSquare];
                board[move.targetSquare] = 0;
                board[move.sourceSquare + xDelta] = priorTargetContent;
                return;
            }else if (move.IsType(MoveType.castle))
            {
                UndoCastle(move, side, board);
                return;
            }
            if(Piece.IsType(movedPiece, PieceType.Pawn)) UpdatePawnBitboard(move, isWhite);
            if(Piece.IsType(priorTargetContent, PieceType.Pawn)) {
                ulong mask = 0b_1ul << move.targetSquare;
                UpdatePawnBitboard(mask, !isWhite);
            }
            board[move.sourceSquare] = board[move.targetSquare];
            board[move.targetSquare] = priorTargetContent;
            BoardManager.UpdateAttackedPositions(true);
            BoardManager.UpdateAttackedPositions(false);
        }

        private static void UndoPromotionMove(Move move, int side, int priorTargetContent, int[] board) {
            ulong xorMask = (0b_1ul << move.sourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            board[move.targetSquare] = priorTargetContent;
            board[move.sourceSquare] = (int)PieceType.Pawn | side;
        }

        private static void UndoCastle(Move move, int side, int[] board)
        {
            bool isWhite = Piece.IsPieceWhite(board[move.targetSquare]);
            bool kingSide = move.sourceSquare - move.targetSquare < 0;

            if (kingSide)
            {
                board[move.sourceSquare + 1] = 0;
                int rookPosition = isWhite ? 7 : 63;
                board[rookPosition] = (int)PieceType.Rook | side;
            }
            else
            {
                board[move.sourceSquare - 1] = 0;
                int rookPosition = isWhite ? 0 : 56;
                board[rookPosition] = (int)PieceType.Rook | side;
            }
            board[move.sourceSquare] = board[move.targetSquare];
            board[move.targetSquare] = 0;
        }
    }
}
