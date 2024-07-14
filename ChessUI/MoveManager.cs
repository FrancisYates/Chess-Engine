using ChessUI.Engine;
using ChessUI.Enums;

namespace ChessUI
{
    public static class MoveManager
    {
        public static (int target, CastlingRights validCastling) MakeMove(Move move, int[] board)
        {
            BoardManager.EnPesantSquare = -1;
            int targetContents = board[move.TargetSquare];
            int movedPiece = board[move.SourceSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            CastlingRights validCastling = BoardManager.CastleingRights;
            if (Piece.IsType(targetContents, PieceType.King)) ;

            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = isWhite ? BoardManager.BlackPiecePositions : BoardManager.WhitePiecePositions;
            friendlyPositions.Remove(Piece.GetPieceType(movedPiece), move.SourceSquare);
            if (move.IsType(MoveType.promotion))
            {
                PromotionMove(move, side, board, opponentPositions);
            }
            else if (move.IsType(MoveType.doublePawnMove))
            {
                friendlyPositions.Pawns.Add(move.TargetSquare);
                UpdatePawnBitboard(move, isWhite);
                int offset = isWhite ? -8 : 8;
                BoardManager.EnPesantSquare = move.TargetSquare + offset;

                board[move.TargetSquare] = board[move.SourceSquare];
                board[move.SourceSquare] = 0;
            }
            else if (move.IsType(MoveType.enPesant))
            {
                friendlyPositions.Pawns.Add(move.TargetSquare);
                UpdatePawnBitboard(move, isWhite);
                int xDelta = move.TargetSquare % 8 - move.SourceSquare % 8;
                ulong caturedPieceMask = (0b_1ul << (move.SourceSquare + xDelta));
                UpdatePawnBitboard(caturedPieceMask, !isWhite);

                targetContents = board[move.SourceSquare + xDelta];
                board[move.TargetSquare] = board[move.SourceSquare];
                board[move.SourceSquare] = 0;
                board[move.SourceSquare + xDelta] = 0;
                opponentPositions.Remove(Piece.GetPieceType(targetContents), move.SourceSquare + xDelta);
            }
            else if (move.IsType(MoveType.castle))
            {
                CastleMove(move, isWhite, board);
            }
            else
            {
                friendlyPositions.Add(Piece.GetPieceType(movedPiece), move.TargetSquare);
                targetContents = StandardMove(move, isWhite, movedPiece, board);
            }
            BoardManager.UpdateAttackedPositions();
            return (targetContents, validCastling);
        }

        private static void UpdatePawnBitboard(Move move, bool updateWhite = true) {
            ulong xorMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
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

        private static void PromotionMove(Move move, int side, int[] board, PiecePositions oponentPositions)
        {
            ulong xorMask = (0b_1ul << move.SourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            PieceType type = move.GetPromotionPiece();
            bool isWhite = side == 8;
            Bitboards friendlyBitboards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            Bitboards oponentBitboards = !isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

            ulong mask = (0b_1ul << move.TargetSquare);
            if (move.IsType(MoveType.capture))
            {
                int targetContents = board[move.TargetSquare];
                PieceType targetType = Piece.GetPieceType(targetContents);
                ApplyMaskToAppropriateBitboard(mask, ref oponentBitboards, targetType);
                opponentPositions.Remove(targetType, move.TargetSquare);

                if (targetType == PieceType.Rook && (LookUps.castleStoppingRookCaptures & (1ul << move.TargetSquare)) > 0)
                {
                    CastlingRights whiteOrBlack = (CastlingRights)(isWhite ? 0b_1100 : 0b_0011);
                    int rookKingSideSquare = isWhite ? 63 : 7;
                    CastlingRights kingOrQueen = (CastlingRights)(move.TargetSquare == rookKingSideSquare ? 0 : 0b_1010);
                    int rookQueenSideSquare = isWhite ? 56 : 0;
                    kingOrQueen |= (CastlingRights)(move.TargetSquare == rookQueenSideSquare ? 0 : 0b_0101);
                    CastlingRights newCastleRights = kingOrQueen | whiteOrBlack;
                    BoardManager.CastleingRights &= newCastleRights;
                }
            }
            PieceType pieceType = move.GetPromotionPiece();
            ApplyMaskToAppropriateBitboard(mask, ref friendlyBitboards, pieceType);
            board[move.TargetSquare] = (int)type | side;
            board[move.SourceSquare] = 0;
            friendlyPositions.Add(pieceType, move.TargetSquare);
        }

        private static void ApplyMaskToAppropriateBitboard(ulong mask, ref Bitboards bitboards, PieceType pieceType)
        {
            ref ulong bitBoard = ref BoardManager.GetBitboard(ref bitboards, pieceType);
            bitBoard ^= mask;
            bitboards.AllPieces ^= mask;
        }
        private static void CastleMove(Move move, bool isWhite, int[] board)
        {
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            friendlyPositions.King = move.TargetSquare;
            bool kingSide = move.SourceSquare - move.TargetSquare < 0;
            int side = isWhite ? 8 : 0;
            var bitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong rookMask;
            ulong kingMask;
            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.SourceSquare + 1);
                kingMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);

                board[move.SourceSquare + 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
                friendlyPositions.Rooks.Remove(rookPosition);
                friendlyPositions.Rooks.Add(move.SourceSquare + 1);
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.SourceSquare - 1);
                kingMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
                board[move.SourceSquare - 1] = (int)PieceType.Rook | side;
                board[rookPosition] = 0;
                friendlyPositions.Rooks.Remove(rookPosition);
                friendlyPositions.Rooks.Add(move.SourceSquare - 1);
            }
            bitBoards.Rooks ^= rookMask;
            bitBoards.Kings ^= kingMask;
            bitBoards.AllPieces ^= rookMask;
            bitBoards.AllPieces ^= kingMask;

            board[move.TargetSquare] = board[move.SourceSquare];
            board[move.SourceSquare] = 0;
            CastlingRights castleingChange = (CastlingRights)(isWhite ? 0b_0011 : 0b_1100);
            BoardManager.CastleingRights &= castleingChange;

        }

        private static int StandardMove(Move move, bool isWhite, int movedPiece, int[] board)
        {
            int target = board[move.TargetSquare];
            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            var targetBitBoards = isWhite ?  BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
            PiecePositions opponentPositions = isWhite ? BoardManager.BlackPiecePositions : BoardManager.WhitePiecePositions;

            board[move.TargetSquare] = board[move.SourceSquare];
            board[move.SourceSquare] = 0;

            if (Piece.IsType(movedPiece, PieceType.Rook))
            {
                if (!Piece.HasRookMoved(isWhite, move.SourceSquare))
                {
                    CastlingRights whiteOrBlack = (CastlingRights)(isWhite ? 0b_0011 : 0b_1100);
                    int rookKingSideSquare = isWhite ? 7 : 63;
                    CastlingRights kingOrQueen = (CastlingRights)(move.SourceSquare == rookKingSideSquare ? 0b_0101 : 0b_1010);
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
                opponentPositions.Remove(targetType, move.TargetSquare);
                if (targetType == PieceType.Rook && (LookUps.castleStoppingRookCaptures & (1ul << move.TargetSquare)) > 0)
                {
                    CastlingRights whiteOrBlack = (CastlingRights)(isWhite ? 0b_1100 : 0b_0011);
                    int rookKingSideSquare = isWhite ? 63 : 7;
                    CastlingRights kingOrQueen = (CastlingRights)(move.TargetSquare == rookKingSideSquare ? 0 : 0b_1010);
                    int rookQueenSideSquare = isWhite ? 56 : 0;
                    kingOrQueen |= (CastlingRights)(move.TargetSquare == rookQueenSideSquare ? 0 : 0b_0101);
                    CastlingRights newCastleRights = kingOrQueen | whiteOrBlack;
                    BoardManager.CastleingRights &= newCastleRights;
                }
            }
            ulong movedMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, Piece.GetPieceType(movedPiece));
            if(target != 0)
            {
                ulong targetMask = 0b_1ul << move.TargetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, Piece.GetPieceType(target));
            }
            return target;
        }

        public static void UndoMove(Move move, int priorTargetContent, CastlingRights priorCastlingRights, int[] board)
        {
            int movedPiece = board[move.TargetSquare];
            var movedPieceType = Piece.GetPieceType(movedPiece);
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            BoardManager.CastleingRights = priorCastlingRights;
            PiecePositions friendlyPositions = side == 8 ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = side != 8 ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            friendlyPositions.Remove(movedPieceType, move.TargetSquare);

            if (move.IsType(MoveType.promotion))
            {
                UndoPromotionMove(move, side, priorTargetContent, board);
                return;
            }
            else if (move.IsType(MoveType.doublePawnMove))
            {
                friendlyPositions.Pawns.Add(move.SourceSquare);
                UpdatePawnBitboard(move, side == (int)Colour.White);
                int offset = isWhite ? -8 : 8;
                BoardManager.EnPesantSquare = move.TargetSquare + offset;

                board[move.SourceSquare] = board[move.TargetSquare];
                board[move.TargetSquare] = 0;
                return;
            }
            else if (move.IsType(MoveType.enPesant))
            {
                UpdatePawnBitboard(move, isWhite);
                int xDelta = move.TargetSquare % 8 - move.SourceSquare % 8;
                ulong caturedPieceMask = (0b_1ul << (move.SourceSquare + xDelta));
                UpdatePawnBitboard(caturedPieceMask, !isWhite);

                board[move.SourceSquare] = board[move.TargetSquare];
                board[move.TargetSquare] = 0;
                board[move.SourceSquare + xDelta] = priorTargetContent;

                friendlyPositions.Pawns.Add(move.SourceSquare);
                opponentPositions.Add(Piece.GetPieceType(priorTargetContent), move.SourceSquare + xDelta);
                return;
            }
            else if (move.IsType(MoveType.castle))
            {
                UndoCastle(move, side, board);
                return;
            }
            friendlyPositions.Add(movedPieceType, move.SourceSquare);
            UndoStandardMove(move, isWhite, priorTargetContent, board);
            BoardManager.UpdateAttackedPositions();
        }

        private static void UndoStandardMove(Move move, bool isWhite, int priorTargetContent, int[] board)
        {
            if(move.IsType(MoveType.capture))
            {
                PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
                opponentPositions.Add(Piece.GetPieceType(priorTargetContent), move.TargetSquare);
            }

            int movedPiece = board[move.TargetSquare];
            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong movedMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, Piece.GetPieceType(movedPiece));
            if (priorTargetContent != 0)
            {
                var targetBitBoards = isWhite ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                ulong targetMask = 0b_1ul << move.TargetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, Piece.GetPieceType(priorTargetContent));
            }

            board[move.SourceSquare] = board[move.TargetSquare];
            board[move.TargetSquare] = priorTargetContent;
        }

        private static void UndoPromotionMove(Move move, int side, int priorTargetContent, int[] board)
        {
            Bitboards friendlyBitboards = Piece.IsPieceWhite(side) ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            PiecePositions friendlyPositions = side == 8 ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

            friendlyPositions.Pawns.Add(move.SourceSquare);

            ulong xorMask = (0b_1ul << move.SourceSquare);
            UpdatePawnBitboard(xorMask, side == (int)Colour.White);
            ulong captureMask = (0b_1ul << move.TargetSquare);
            if (move.IsType(MoveType.capture))
            {
                Bitboards oponentBitboards = Piece.IsPieceWhite(side) ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                PiecePositions opponentPositions = side != 8 ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

                PieceType type = Piece.GetPieceType(priorTargetContent);
                opponentPositions.Add(type, move.TargetSquare);
                ApplyMaskToAppropriateBitboard(captureMask, ref oponentBitboards, type);
            }
            ApplyMaskToAppropriateBitboard(captureMask, ref friendlyBitboards, Piece.GetPieceType(board[move.TargetSquare]));
            board[move.TargetSquare] = priorTargetContent;
            board[move.SourceSquare] = (int)PieceType.Pawn | side;
        }

        private static void UndoCastle(Move move, int side, int[] board)
        {
            PiecePositions friendlyPositions = side == 8 ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            friendlyPositions.King = move.SourceSquare;

            bool isWhite = Piece.IsPieceWhite(board[move.TargetSquare]);
            bool kingSide = move.SourceSquare - move.TargetSquare < 0;
            var bitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong rookMask;
            ulong kingMask;

            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.SourceSquare + 1);
                kingMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
                board[move.SourceSquare + 1] = 0;
                board[rookPosition] = (int)PieceType.Rook | side;

                friendlyPositions.Rooks.Add(rookPosition);
                friendlyPositions.Rooks.Remove(move.SourceSquare + 1);
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                rookMask = (0b_1ul << rookPosition) | (0b_1ul << move.SourceSquare - 1);
                kingMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
                board[move.SourceSquare - 1] = 0;
                board[rookPosition] = (int)PieceType.Rook | side;

                friendlyPositions.Rooks.Add(rookPosition);
                friendlyPositions.Rooks.Remove(move.SourceSquare - 1);
            }
            bitBoards.Rooks ^= rookMask;
            bitBoards.Kings ^= kingMask;
            bitBoards.AllPieces ^= rookMask;
            bitBoards.AllPieces ^= kingMask;
            board[move.SourceSquare] = board[move.TargetSquare];
            board[move.TargetSquare] = 0;
        }
    }
}
