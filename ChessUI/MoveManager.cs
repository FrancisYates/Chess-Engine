using ChessUI.Engine;
using ChessUI.Enums;

namespace ChessUI
{
    public static class MoveManager
    {
        public static MoveChanges MakeMove(Move move, int[] board, bool isWhite)
        {
            MoveChanges changes = new()
            {
                EnPesantSquare = BoardManager.EnPesantSquare,
                CastingRights = BoardManager.CastleingRights
            };
            BoardManager.EnPesantSquare = -1;

            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = isWhite ? BoardManager.BlackPiecePositions : BoardManager.WhitePiecePositions;
            friendlyPositions.Remove(move.PieceType, move.SourceSquare);
            if (move.IsType(MoveType.promotion))
            {
                PromotionMove(move, isWhite, board, out var targetType);
                changes.TargetType = targetType;
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

                PieceType targetType = Piece.GetPieceType(board[move.SourceSquare + xDelta]);
                board[move.TargetSquare] = board[move.SourceSquare];
                board[move.SourceSquare] = 0;
                board[move.SourceSquare + xDelta] = 0;
                changes.TargetType = targetType;
                opponentPositions.Remove(targetType, move.SourceSquare + xDelta);
            }
            else if (move.IsType(MoveType.castle))
            {
                CastleMove(move, isWhite, board);
            }
            else
            {
                friendlyPositions.Add(move.PieceType, move.TargetSquare);
                var targetContents = StandardMove(move, isWhite, board);
                changes.TargetType = Piece.GetPieceType(targetContents);
            }
            BoardManager.UpdateAttackedPositions();
            return changes;
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

        private static void PromotionMove(Move move, bool isWhite, int[] board, out PieceType capturedType)
        {
            capturedType = PieceType.None;
            ulong xorMask = (0b_1ul << move.SourceSquare);
            UpdatePawnBitboard(xorMask, isWhite);
            PieceType type = move.GetPromotionPiece();
            Bitboards friendlyBitboards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            Bitboards oponentBitboards = !isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

            ulong mask = (0b_1ul << move.TargetSquare);
            if (move.IsType(MoveType.capture))
            {
                int targetContents = board[move.TargetSquare];
                PieceType targetType = Piece.GetPieceType(targetContents);
                capturedType = targetType;
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
            int side = isWhite ? 8 : 0;
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

        private static int StandardMove(Move move, bool isWhite, int[] board)
        {
            int target = board[move.TargetSquare];
            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            var targetBitBoards = isWhite ?  BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
            PiecePositions opponentPositions = isWhite ? BoardManager.BlackPiecePositions : BoardManager.WhitePiecePositions;

            board[move.TargetSquare] = board[move.SourceSquare];
            board[move.SourceSquare] = 0;

            if (move.PieceType == PieceType.Rook)
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
            else if (move.PieceType == PieceType.King)
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
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, move.PieceType);
            if(target != 0)
            {
                ulong targetMask = 0b_1ul << move.TargetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, Piece.GetPieceType(target));
            }
            return target;
        }

        public static void UndoMove(Move move, MoveChanges changes, bool isWhite)
        {
            int[] board = BoardManager.Board;
            BoardManager.EnPesantSquare = changes.EnPesantSquare;
            BoardManager.CastleingRights = changes.CastingRights;
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            friendlyPositions.Remove(move.PieceType, move.TargetSquare);

            if (move.IsType(MoveType.promotion))
            {
                UndoPromotionMove(move, isWhite, changes.TargetType, board);
                return;
            }
            else if (move.IsType(MoveType.doublePawnMove))
            {
                friendlyPositions.Pawns.Add(move.SourceSquare);
                UpdatePawnBitboard(move, isWhite);
                int offset = isWhite ? -8 : 8;

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
                board[move.SourceSquare + xDelta] = Piece.GetPositionRepresentation(changes.TargetType, !isWhite);

                friendlyPositions.Pawns.Add(move.SourceSquare);
                opponentPositions.Add(changes.TargetType, move.SourceSquare + xDelta);
                return;
            }
            else if (move.IsType(MoveType.castle))
            {
                UndoCastle(move, isWhite, board);
                return;
            }
            friendlyPositions.Add(move.PieceType, move.SourceSquare);
            UndoStandardMove(move, isWhite, changes.TargetType, board);
            BoardManager.UpdateAttackedPositions();
        }

        private static void UndoStandardMove(Move move, bool isWhite, PieceType targetType, int[] board)
        {
            if(move.IsType(MoveType.capture))
            {
                PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
                opponentPositions.Add(targetType, move.TargetSquare);
            }

            var movedBitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong movedMask = (0b_1ul << move.SourceSquare) | (0b_1ul << move.TargetSquare);
            ApplyMaskToAppropriateBitboard(movedMask, ref movedBitBoards, move.PieceType);
            if (targetType != PieceType.None)
            {
                var targetBitBoards = isWhite ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                ulong targetMask = 0b_1ul << move.TargetSquare;
                ApplyMaskToAppropriateBitboard(targetMask, ref targetBitBoards, targetType);
            }

            board[move.SourceSquare] = board[move.TargetSquare];
            board[move.TargetSquare] = Piece.GetPositionRepresentation(targetType, !isWhite);
        }

        private static void UndoPromotionMove(Move move, bool isWhite, PieceType targetType, int[] board)
        {
            Bitboards friendlyBitboards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

            friendlyPositions.Pawns.Add(move.SourceSquare);
            friendlyPositions.Remove(move.GetPromotionPiece(), move.TargetSquare);

            ulong xorMask = (0b_1ul << move.SourceSquare);
            UpdatePawnBitboard(xorMask, isWhite);
            ulong captureMask = (0b_1ul << move.TargetSquare);
            if (move.IsType(MoveType.capture))
            {
                Bitboards oponentBitboards = isWhite ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;
                PiecePositions opponentPositions = !isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;

                opponentPositions.Add(targetType, move.TargetSquare);
                ApplyMaskToAppropriateBitboard(captureMask, ref oponentBitboards, targetType);
            }
            ApplyMaskToAppropriateBitboard(captureMask, ref friendlyBitboards, Piece.GetPieceType(board[move.TargetSquare]));
            int side = isWhite ? 8 : 0;
            board[move.TargetSquare] = targetType == PieceType.None? 0 : (int)targetType | side;
            board[move.SourceSquare] = (int)PieceType.Pawn | side;
        }

        private static void UndoCastle(Move move, bool isWhite, int[] board)
        {
            PiecePositions friendlyPositions = isWhite ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            friendlyPositions.King = move.SourceSquare;

            bool kingSide = move.SourceSquare - move.TargetSquare < 0;
            var bitBoards = isWhite ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            ulong rookMask;
            ulong kingMask;

            int side = isWhite ? 8 : 0;
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

    public struct MoveChanges
    {
        public int EnPesantSquare { get; set; } = -1;
        public PieceType TargetType {get; set;} = PieceType.None;
        public CastlingRights CastingRights { get; set; }
        public MoveChanges()
        {

        }
    }
}
