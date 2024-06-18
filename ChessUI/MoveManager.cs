using ChessUI.Engine;
using ChessUI.Enums;

namespace ChessUI
{
    public static class MoveManager
    {
        public static (int target, CastlingRights validCastling) MakeMove(Move move, int[] board)
        {
            BoardManager.EnPesantSquare = -1;
            int targetContents = board[move.targetSquare];
            int movedPiece = board[move.sourceSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            CastlingRights validCastling = BoardManager.CastleingRights;

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
                int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
                ulong caturedPieceMask = (0b_1ul << (move.sourceSquare + xDelta));
                UpdatePawnBitboard(caturedPieceMask, !isWhite);

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

        private static void UpdatePawnBitboard(ulong xorMask, bool updateWhite = true)
        {
            if (updateWhite)
            {
                BoardManager.WhiteBitboards.Pawns ^= xorMask;
                BoardManager.WhiteBitboards.AllPieces ^= xorMask;
            }
            else
            {
                BoardManager.BlackBitboards.Pawns ^= xorMask;
                BoardManager.BlackBitboards.AllPieces ^= xorMask;
            }
        }

        private static void PromotionMove(Move move, int side, int[] board)
        {
            ulong xorMask = (0b_1ul << move.sourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            PieceType type = move.GetPromotionPiece();

            Bitboards friendlyBitboards = side == 8 ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            Bitboards oponentBitboards = side != 8 ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;

            ulong mask = (0b_1ul << move.targetSquare);
            if (move.IsType(MoveType.capture))
            {
                int targetContents = board[move.targetSquare];
                ApplyMaskToAppropriateBitboard(mask, ref oponentBitboards, Piece.GetPieceType(targetContents));
            }
            PieceType pieceType = move.GetPromotionPiece();
            ApplyMaskToAppropriateBitboard(mask, ref friendlyBitboards, pieceType);
            board[move.targetSquare] = (int)type | side;
            board[move.sourceSquare] = 0;
        }

        private static void ApplyMaskToAppropriateBitboard(ulong mask, ref Bitboards bitboards, PieceType pieceType)
        {
            ref ulong bitBoard = ref BoardManager.GetBitboard(ref bitboards, pieceType);
            bitBoard ^= mask;
            bitboards.AllPieces ^= mask;
        }
        private static void CastleMove(Move move, bool isWhite, int[] board)
        {
            bool kingSide = move.sourceSquare - move.targetSquare < 0;
            int side = isWhite ? 8 : 0;
            var bitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong rookMask;
            ulong kingMask;
            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.sourceSquare + 1);
                kingMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);

                board[move.sourceSquare + 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.sourceSquare - 1);
                kingMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
                board[move.sourceSquare - 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
            }
            bitBoards.Rooks ^= rookMask;
            bitBoards.Kings ^= kingMask;
            bitBoards.AllPieces ^= rookMask;
            bitBoards.AllPieces ^= kingMask;

            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;
            CastlingRights castleingChange = (CastlingRights)(isWhite ? 0b_0011 : 0b_1100);
            BoardManager.CastleingRights &= castleingChange;

        }

        private static int StandardMove(Move move, bool isWhite, int movedPiece, int[] board)
        {
            int target = board[move.targetSquare];
            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            var targetBitBoards = isWhite ?  BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;

            if (Piece.IsType(movedPiece, PieceType.Rook))
            {
                if (!Piece.HasRookMoved(isWhite, move.sourceSquare))
                {
                    CastlingRights whiteOrBlack = (CastlingRights)(isWhite ? 0b_0011 : 0b_1100);
                    int rookKingSideSquare = isWhite ? 7 : 63;
                    CastlingRights kingOrQueen = (CastlingRights)(move.sourceSquare == rookKingSideSquare ? 0b_0101 : 0b_1010);
                    CastlingRights newCastleRights = kingOrQueen | whiteOrBlack;
                    BoardManager.CastleingRights &= newCastleRights;
                }
            }
            else if (Piece.IsType(movedPiece, PieceType.King))
            {
                CastlingRights castleingChange = (CastlingRights)(isWhite ? 0b_0011 : 0b_1100);
                BoardManager.CastleingRights &= castleingChange;
            }
            if (move.IsType(MoveType.capture))
            {
                PieceType targetType = Piece.GetPieceType(target);
                if(targetType == PieceType.Rook && (LookUps.castleStoppingRookCaptures & (1ul << move.targetSquare)) > 0)
                {
                    CastlingRights whiteOrBlack = (CastlingRights)(isWhite ? 0b_1100 : 0b_0011);
                    int rookKingSideSquare = isWhite ? 63 : 7;
                    CastlingRights kingOrQueen = (CastlingRights)(move.targetSquare == rookKingSideSquare ? 0 : 0b_1010);
                    int rookQueenSideSquare = isWhite ? 56 : 0;
                    kingOrQueen |= (CastlingRights)(move.targetSquare == rookQueenSideSquare ? 0 : 0b_0101);
                    CastlingRights newCastleRights = kingOrQueen | whiteOrBlack;
                    BoardManager.CastleingRights &= newCastleRights;
                }
            }
            ulong movedMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, Piece.GetPieceType(movedPiece));
            if(target != 0)
            {
                ulong targetMask = 0b_1ul << move.targetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, Piece.GetPieceType(target));
            }
            return target;
        }

        public static void UndoMove(Move move, int priorTargetContent, CastlingRights priorCastlingRights, int[] board)
        {            
            int movedPiece = board[move.targetSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            BoardManager.CastleingRights = priorCastlingRights;

            if (move.IsType(MoveType.promotion))
            {
                UndoPromotionMove(move, side, priorTargetContent, board);
                return;
            }
            else if (move.IsType(MoveType.doublePawnMove))
            {
                UpdatePawnBitboard(move, side == (int)Colour.White);
                int offset = isWhite ? -8 : 8;
                BoardManager.EnPesantSquare = move.targetSquare + offset;

                board[move.sourceSquare] = board[move.targetSquare];
                board[move.targetSquare] = 0;
                return;
            }
            else if (move.IsType(MoveType.enPesant))
            {
                UpdatePawnBitboard(move, isWhite);
                int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
                ulong caturedPieceMask = (0b_1ul << (move.sourceSquare + xDelta));
                UpdatePawnBitboard(caturedPieceMask, !isWhite);

                board[move.sourceSquare] = board[move.targetSquare];
                board[move.targetSquare] = 0;
                board[move.sourceSquare + xDelta] = priorTargetContent;
                return;
            }
            else if (move.IsType(MoveType.castle))
            {
                UndoCastle(move, side, board);
                return;
            }
            UndoStandardMove(move, isWhite, priorTargetContent, board);
            BoardManager.UpdateAttackedPositions(true);
            BoardManager.UpdateAttackedPositions(false);
        }

        private static void UndoStandardMove(Move move, bool isWhite, int priorTargetContent, int[] board)
        {
            int movedPiece = board[move.targetSquare];
            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong movedMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, Piece.GetPieceType(movedPiece));
            if (priorTargetContent != 0)
            {
                var targetBitBoards = isWhite ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                ulong targetMask = 0b_1ul << move.targetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, Piece.GetPieceType(priorTargetContent));
            }

            board[move.sourceSquare] = board[move.targetSquare];
            board[move.targetSquare] = priorTargetContent;
        }

        private static void UndoPromotionMove(Move move, int side, int priorTargetContent, int[] board)
        {
            ulong xorMask = (0b_1ul << move.sourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            ulong captureMask = (0b_1ul << move.targetSquare);
            Bitboards friendlyBitboards = Piece.IsPieceWhite(side) ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            if (move.IsType(MoveType.capture))
            {
                Bitboards oponentBitboards = Piece.IsPieceWhite(side) ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                PieceType type = Piece.GetPieceType(priorTargetContent);
                ApplyMaskToAppropriateBitboard(captureMask, ref oponentBitboards, type);
            }
            ApplyMaskToAppropriateBitboard(captureMask, ref friendlyBitboards, Piece.GetPieceType(board[move.targetSquare]));
            board[move.targetSquare] = priorTargetContent;
            board[move.sourceSquare] = (int)PieceType.Pawn | side;
        }

        private static void UndoCastle(Move move, int side, int[] board)
        {
            bool isWhite = Piece.IsPieceWhite(board[move.targetSquare]);
            bool kingSide = move.sourceSquare - move.targetSquare < 0;
            var bitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong rookMask;
            ulong kingMask;

            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.sourceSquare + 1);
                kingMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
                board[move.sourceSquare + 1] = 0;
                board[rookPosition] = (int)PieceType.Rook | side;
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.sourceSquare - 1);
                kingMask = (0b_1ul << move.sourceSquare) | (0b_1ul << move.targetSquare);
                board[move.sourceSquare - 1] = 0;
                board[rookPosition] = (int)PieceType.Rook | side;
            }
            bitBoards.Rooks ^= rookMask;
            bitBoards.Kings ^= kingMask;
            bitBoards.AllPieces ^= rookMask;
            bitBoards.AllPieces ^= kingMask;
            board[move.sourceSquare] = board[move.targetSquare];
            board[move.targetSquare] = 0;
        }
    }
}
