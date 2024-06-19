using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ChessUI.Enums;

namespace ChessUI.Engine
{
    public static class MoveGeneration
    {
        public static int[][] numSquaresInDirection = new int[64][];

        private static Bitboards FriendlyBitboards { get; set; } = new();
        private static Bitboards OpponentBitboards { get; set; } = new();
        private static PiecePositions FriendlyPositions { get; set; } = new();
        private static List<Move> GenerateMoves(bool whiteMoves, bool generateOnlyCaptures)
        {
            List<Move> moves = new(32);

            moves.AddRange(GeneratePawnMoves(whiteMoves, generateOnlyCaptures));

            moves.AddRange(GenerateKnightMoves(generateOnlyCaptures));

            moves.AddRange(GenerateKingMoves(generateOnlyCaptures, whiteMoves));

            moves.AddRange(GenerateRookMoves(generateOnlyCaptures));

            moves.AddRange(GenerateBishopMoves(generateOnlyCaptures));

            moves.AddRange(GenerateQueenMoves(generateOnlyCaptures));

            return moves;
        }

        public static List<int> GetPoistionsFromBitboard(ulong bitboard)
        {
            Span<int[]> lut = LookUps.SetByteIndexes.Value;
            Span<byte> bytes = BitConverter.GetBytes(bitboard);
            List<int> results = new(8);
            for (int i = 0; i < 8; i++)
            {
                Span<int> setIndecies = lut[bytes[i]];
                for (int j = 0; j < setIndecies.Length; j++)
                {
                    results.Add(setIndecies[j] - 1 + i * 8);
                }
            }
            return results;
        }

        public static Move[] GenerateStrictLegalMoves(bool whiteMoves, bool generateOnlyCaptures = false)
        {
            FriendlyBitboards = whiteMoves ? BoardManager.WhiteBitboards : BoardManager.BlackBitboards;
            OpponentBitboards = whiteMoves ? BoardManager.BlackBitboards : BoardManager.WhiteBitboards;

            FriendlyPositions = whiteMoves ? BoardManager.WhitePiecePositions : BoardManager.BlackPiecePositions;
            var moves = GenerateMoves(whiteMoves, generateOnlyCaptures);
            RemoveIllegalMoves(moves, whiteMoves);
            return moves.ToArray();
        }

        private static List<Move> GenerateRookMoves(bool generateOnlyCaptures)
        {
            List<Move> moves = new();
            var positions = FriendlyPositions.Rooks;
            foreach (var position in positions)
            {
                ulong potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) & ~(1ul << position);
                ulong blockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];

                ulong movesMask = LookUps.RookMoveDict[(position, blockers)] & ~FriendlyBitboards.AllPieces;
                var captureTargets = GetPoistionsFromBitboard(movesMask & OpponentBitboards.AllPieces);
                foreach (var target in captureTargets)
                {
                    moves.Add(new(position, target, MoveType.capture));
                }
                if (generateOnlyCaptures) return moves;

                var moveTargets = GetPoistionsFromBitboard(movesMask & ~OpponentBitboards.AllPieces);
                foreach (var target in moveTargets)
                {
                    moves.Add(new(position, target));
                }
            }
            return moves;
        }

        private static List<Move> GenerateBishopMoves(bool generateOnlyCaptures)
        {
            List<Move> moves = new();
            var positions = FriendlyPositions.Bishops;

            foreach (var position in positions)
            {
                ulong potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) & ~(1ul << position);
                ulong blockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];

                ulong movesMask = LookUps.BishopMoveDict[(position, blockers)] & ~FriendlyBitboards.AllPieces;
                var captureTargets = GetPoistionsFromBitboard(movesMask & OpponentBitboards.AllPieces);
                foreach (var target in captureTargets)
                {
                    moves.Add(new(position, target, MoveType.capture));
                }
                if (generateOnlyCaptures) continue;

                var moveTargets = GetPoistionsFromBitboard(movesMask & ~OpponentBitboards.AllPieces);
                foreach (var target in moveTargets)
                {
                    moves.Add(new(position, target));
                }
            }

            return moves;
        }

        private static List<Move> GenerateQueenMoves(bool generateOnlyCaptures)
        {
            List<Move> moves = new();
            var positions = FriendlyPositions.Queens;

            foreach (var position in positions)
            {
                ulong potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) & ~(1ul << position);
                ulong rookBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];
                ulong bishopBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];

                ulong rookMask = LookUps.RookMoveDict[(position, rookBlockers)];
                ulong bishopMask = LookUps.BishopMoveDict[(position, bishopBlockers)];
                ulong movesMask = (rookMask | bishopMask) & ~FriendlyBitboards.AllPieces;
                var captureTargets = GetPoistionsFromBitboard(movesMask & OpponentBitboards.AllPieces);
                foreach (var target in captureTargets)
                {
                    moves.Add(new(position, target, MoveType.capture));
                }
                if (generateOnlyCaptures) continue;

                var moveTargets = GetPoistionsFromBitboard(movesMask & ~OpponentBitboards.AllPieces);
                foreach (var target in moveTargets)
                {
                    moves.Add(new(position, target));
                }
            }

            return moves;
        }

        private static List<Move> GeneratePawnMoves(bool isWhite, bool generateOnlyCaptures)
        {
            List<Move> moves = [];
            var pawnPositions = FriendlyPositions.Pawns;
            foreach (var position in pawnPositions)
            {
                ulong attackBitBoard = isWhite ? LookUps.whitePawnAttackBitBoard[position] : LookUps.blackPawnAttackBitBoard[position];
                int moveDirection = isWhite ? 1 : -1;
                int targetSquare;

                if ((attackBitBoard & OpponentBitboards.AllPieces) > 0)
                {
                    moves.AddRange(GetPawnCaptureMoves(isWhite, position));
                }
                ulong epTarget = attackBitBoard & BoardManager.EnPesantBitBoard;
                if (epTarget > 0)
                {
                    moves.Add(new Move(position, GetPoistionsFromBitboard(epTarget).First(), MoveType.enPesant));
                }
                if (generateOnlyCaptures) return moves;
                targetSquare = position + 8 * moveDirection;
                if ((((FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) >> targetSquare) & 1) == 0)
                {
                    addNonCaptureMoves(position, targetSquare);
                    if (((1ul << position) & FriendlyBitboards.PawnHomeRank) > 0)
                    {
                        targetSquare = position + 16 * moveDirection;
                        if ((((FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) >> targetSquare) & 1) == 0)
                        {
                            addNonCaptureMoves(position, targetSquare, MoveType.doublePawnMove);
                        }
                    }
                }
            }
            return moves;
            void addNonCaptureMoves(int position, int targetSquare, MoveType moveType = MoveType.move)
            {
                if ((FriendlyBitboards.FinalRank & (1ul << targetSquare)) > 0)
                {
                    moves.Add(new Move(position, targetSquare, MoveType.promotion, PromotionPiece.bishop));
                    moves.Add(new Move(position, targetSquare, MoveType.promotion, PromotionPiece.rook));
                    moves.Add(new Move(position, targetSquare, MoveType.promotion, PromotionPiece.knight));
                    moves.Add(new Move(position, targetSquare, MoveType.promotion, PromotionPiece.queen));
                }
                else
                {
                    moves.Add(new Move(position, targetSquare, moveType));
                }
            }
        }

        private static List<Move> GetPawnCaptureMoves(bool isWhite, int sourceSquare)
        {
            List<Move> moves = new();
            int[] attackOffsets = LookUps.pawnAttackOffset[isWhite ? 1 : 0, sourceSquare];
            foreach (int offset in attackOffsets)
            {
                int targetSquare = sourceSquare + offset;
                if (((OpponentBitboards.AllPieces >> targetSquare) & 1) == 1)
                {
                    if ((FriendlyBitboards.FinalRank & (1ul << targetSquare)) > 0)
                    {
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.bishop));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.rook));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.knight));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.queen));
                        continue;
                    }
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture));
                }
            }

            return moves;
        }

        private static List<Move> GenerateKnightMoves(bool generateOnlyCaptures)
        {
            List<Move> moves = new();
            var positions = FriendlyPositions.Knights;
            foreach (var position in positions)
            {
                ulong captureMoveMask = LookUps.knightMoveBitboards[position] & OpponentBitboards.AllPieces;
                var candidateMoves = GetPoistionsFromBitboard(captureMoveMask);
                foreach (var target in candidateMoves)
                {
                    moves.Add(new Move(position, target, MoveType.capture));
                }
                if (generateOnlyCaptures) continue;

                ulong nonCaptureMoveMask = LookUps.knightMoveBitboards[position] &
                ~(FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces);
                candidateMoves = GetPoistionsFromBitboard(nonCaptureMoveMask);
                foreach (var target in candidateMoves)
                {
                    moves.Add(new Move(position, target));
                }
            }
            return moves;
        }

        private static List<Move> GenerateKingMoves(bool generateOnlyCaptures, bool isWhite)
        {
            List<Move> moves = new();
            var position = FriendlyPositions.King;
            if(position == -1) return moves;
                ulong allMoves = LookUps.kingMoveBitboards[position] &
                    ~(FriendlyBitboards.AllPieces | OpponentBitboards.ControlledPositions);
                ulong captureMoves = allMoves & OpponentBitboards.AllPieces;
                var moveTargets = GetPoistionsFromBitboard(captureMoves);
                foreach (var target in moveTargets)
                {
                    moves.Add(new(position, target, MoveType.capture));
                }
            if (generateOnlyCaptures) return moves;

                ulong nonCaptureMoves = allMoves & ~OpponentBitboards.AllPieces;
                moveTargets = GetPoistionsFromBitboard(nonCaptureMoves);
                foreach (var target in moveTargets)
                {
                    moves.Add(new(position, target));
                }
                if ((OpponentBitboards.ControlledPositions & FriendlyBitboards.Kings) > 0) return moves;
                if (CanCastleKingSide(isWhite))
                {
                    moves.Add(new Move(position, position + 2, MoveType.castle));
                }
                if (CanCastleQueenSide(isWhite))
                {
                    moves.Add(new Move(position, position - 2, MoveType.castle));
                }
            return moves;
        }

        public static bool CanCastleKingSide(bool isWhite)
        {
            CastlingRights rights = isWhite ? CastlingRights.WhiteKingSide : CastlingRights.BlackKingSide;
            bool castleingAvailable = BoardManager.CastleingRights.HasFlag(rights);
            if (!castleingAvailable) return false;

            ulong path = isWhite ? LookUps.whiteKingSideCastleKingPath : LookUps.blackKingSideCastleKingPath;
            ulong blockers = path & (OpponentBitboards.ControlledPositions | OpponentBitboards.AllPieces | FriendlyBitboards.AllPieces);
            return blockers == 0;
        }

        public static bool CanCastleQueenSide(bool isWhite)
        {
            CastlingRights rights = isWhite ? CastlingRights.WhiteQueenSide : CastlingRights.BlackQueenSide;
            bool castleingAvailable = BoardManager.CastleingRights.HasFlag(rights);
            if (!castleingAvailable) return false;

            ulong kingPath = isWhite ? LookUps.whiteQueenSideCastleKingPath : LookUps.blackQueenSideCastleKingPath;
            ulong fullPath = isWhite ? LookUps.whiteQueenSideCastlePath : LookUps.blackQueenSideCastlePath;
            ulong blockers = (fullPath & (OpponentBitboards.AllPieces | FriendlyBitboards.AllPieces)) | (kingPath & OpponentBitboards.ControlledPositions);

            return blockers == 0;
        }

        public static void CalculateDirections()
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    int distanceUp = 7 - rank;
                    int distanceDown = rank;
                    int distanceRight = 7 - file;
                    int distanceLeft = file;

                    int squareIndex = rank * 8 + file;

                    numSquaresInDirection[squareIndex] = new int[]{
                    distanceUp,
                    distanceDown,
                    distanceLeft,
                    distanceRight,
                    Math.Min(distanceLeft, distanceUp),
                    Math.Min(distanceRight, distanceUp),
                    Math.Min(distanceLeft, distanceDown),
                    Math.Min(distanceRight, distanceDown)
                    };
                }
            }

        }

        private static void RemoveIllegalMoves(List<Move> moves, bool whiteMoves)
        {
            List<Move> illegalMoves = new();
            int? kingPositionN = GetPoistionsFromBitboard(FriendlyBitboards.Kings).FirstOrDefault();
            if (kingPositionN is null)
            {
                moves.Clear();
                return;
            }
            int kingPosition = kingPositionN.Value;
            foreach (Move move in moves)
            {
                if (DoesMoveCauseCheck(kingPosition, move))
                {
                    illegalMoves.Add(move);
                }
            }

            foreach (Move illegalMove in illegalMoves)
            {
                moves.Remove(illegalMove);
            }
        }

        private static bool DoesKingMoveCauseCheck(Move move)
        {
            if (((1ul << move.targetSquare) & OpponentBitboards.ControlledPositions) > 0) return true;

            if (((1ul << move.sourceSquare) & OpponentBitboards.ControlledPositions) > 0)
            {
                ulong[] checkingMasks = FindCheckingPieces(move.sourceSquare);
                ulong allCheckingPieces = checkingMasks[0] | checkingMasks[1] | checkingMasks[2] | checkingMasks[3];
                int numChecingPieces = GetPoistionsFromBitboard(allCheckingPieces).Count();
                if (numChecingPieces == 1)
                {
                    int checkingPiecePosition = GetPoistionsFromBitboard(allCheckingPieces).First();
                    if (move.targetSquare == checkingPiecePosition) return false;
                    if (((1ul << checkingPiecePosition) & OpponentBitboards.SlidingPieces) == 0) return false;

                    int kingToMove = LookUps.directionIndex[move.sourceSquare, move.targetSquare];
                    int checkToKing = LookUps.directionIndex[checkingPiecePosition, move.sourceSquare];

                    return kingToMove == checkToKing;
                }

                var checkingPiecePositions = GetPoistionsFromBitboard(allCheckingPieces & OpponentBitboards.SlidingPieces);
                foreach (int checkingPosition in checkingPiecePositions)
                {
                    int kingToMove = LookUps.directionIndex[move.sourceSquare, move.targetSquare];
                    int checkToKing = LookUps.directionIndex[checkingPosition, move.sourceSquare];

                    if (kingToMove == checkToKing) return true;
                }
                return true;
            }
            return false;

        }
        public static ulong[] FindCheckingPieces(int kingPosition)
        {
            var slidingChecks = FindCheckingSlidingPiece(kingPosition);
            ulong knightChecks = FindCheckingKnight(kingPosition);
            ulong pawns = FindCheckingPawn(kingPosition);

            return [pawns, knightChecks, slidingChecks.rookChecks, slidingChecks.bishopChecks];
        }
        private static ulong FindCheckingPawn(int sourceSquare)
        {
            ulong attackBitBoard = OpponentBitboards.FinalRank == 255 ? LookUps.whitePawnAttackBitBoard[sourceSquare] : LookUps.blackPawnAttackBitBoard[sourceSquare];
            return attackBitBoard & OpponentBitboards.Pawns;
        }

        private static (ulong rookChecks, ulong bishopChecks) FindCheckingSlidingPiece(int sourceSquare)
        {
            ulong allPieceMask = FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces;
            ulong rookBlockers = LookUps.rookOccupancyBitboards[sourceSquare] & allPieceMask;
            ulong rookCheckersMask = LookUps.RookMoveDict[(sourceSquare, rookBlockers)] & (OpponentBitboards.Rooks | OpponentBitboards.Queens);

            ulong bishopBlockers = LookUps.bishopOccupancyBitBoards[sourceSquare] & allPieceMask;
            ulong bishopCheckersMask = LookUps.BishopMoveDict[(sourceSquare, bishopBlockers)] & (OpponentBitboards.Bishops | OpponentBitboards.Queens);
            return (rookCheckersMask, bishopCheckersMask);
        }
        private static ulong FindCheckingKnight(int sourceSquare)
        {
            return LookUps.knightMoveBitboards[sourceSquare] & OpponentBitboards.Knights;
        }

        private static bool IsKingPutInCheck(int kingPosition, Move move)
        {
            int king = BoardManager.Board[kingPosition];

            ulong[] checkingMasks = FindCheckingPieces(kingPosition);
            ulong allCheckingPieces = checkingMasks[0] | checkingMasks[1] | checkingMasks[2] | checkingMasks[3];
            BitOperations.PopCount(allCheckingPieces);
            ulong numChecingPieces = System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount(allCheckingPieces);
            if (numChecingPieces > 1) return true;

            if (allCheckingPieces == 0)
            {
                return IsSlidingCheckCreated(move, kingPosition);
            }

            int piecePosition = GetPoistionsFromBitboard(allCheckingPieces).First();
            if (((1ul << piecePosition) & OpponentBitboards.SlidingPieces) > 0)
            {
                if (!IsPieceInDirection(kingPosition, move.targetSquare, piecePosition)) return true;
                if (move.targetSquare == piecePosition)
                {
                    return IsSlidingCheckCreated(move, kingPosition);
                }
                if (IsCheckBlocked(kingPosition, move.targetSquare, piecePosition))
                {
                    return IsSlidingCheckCreated(move, kingPosition);
                }
                return true;
                
            }
            else
            {
                if (IsSlidingCheckCreated(move, kingPosition)) return true;

                if (move.IsType(MoveType.enPesant))
                {
                    int captureOffset = !Piece.IsPieceWhite(king) ? 8 : -8;
                    return move.targetSquare + captureOffset != piecePosition;
                }
                return move.targetSquare != piecePosition;
            }
        }

        private static bool DoesMoveCauseCheck(int kingPosition, Move move)
        {
            if (kingPosition == move.sourceSquare)
            {
                return DoesKingMoveCauseCheck(move);
            }

            if (move.IsType(MoveType.enPesant)) return DoesEnPesantCreatesCheck(kingPosition, move);

            if ((((1ul << kingPosition) | (1ul << move.sourceSquare)) & OpponentBitboards.ControlledPositions) > 0)
            {
                return IsKingPutInCheck(kingPosition, move);
            }
            return false;
        }

        private static bool DoesEnPesantCreatesCheck(int kingPosition, Move move)
        {
            ulong moveMask = LookUps.queenMoves[kingPosition];
            if ((moveMask & OpponentBitboards.SlidingPieces) == 0) return false;

            int deltaX = (move.targetSquare % 8) - (move.sourceSquare % 8);

            int enPesantPosition = move.sourceSquare + deltaX;

            ulong potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces);
            potentialBlockers ^= 1ul << move.sourceSquare;
            potentialBlockers ^= 1ul << move.targetSquare;
            potentialBlockers ^= 1ul << enPesantPosition;

            ulong rookBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[kingPosition];
            ulong bishopBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[kingPosition];

            ulong rookMask = LookUps.RookMoveDict[(kingPosition, rookBlockers)];
            ulong bishopMask = LookUps.BishopMoveDict[(kingPosition, bishopBlockers)];

            ulong rookCheckMask = rookMask & (OpponentBitboards.Rooks | OpponentBitboards.Queens);
            ulong bishopCheckMask = bishopMask & (OpponentBitboards.Bishops | OpponentBitboards.Queens);

            return (bishopCheckMask | rookCheckMask) > 0;
        }

        private static bool IsCheckBlocked(int kingPosition, int moveTarget, int checkingPiecePosition)
        {
            bool kingPositionLower = kingPosition - checkingPiecePosition < 0;
            return moveTarget - checkingPiecePosition < 0 == kingPositionLower;
        }

        private static bool IsSquareAttackedByOpponent(int squareIndex, bool isOpponentWhite)
        {
            ulong attackedIndicator = (isOpponentWhite ? BoardManager.WhiteBitboards.ControlledPositions : BoardManager.BlackBitboards.ControlledPositions) >> squareIndex & 0b_1ul;
            return attackedIndicator > 0;
        }

        private static bool IsSlidingCheckCreated(Move move, int kingPosition)
        {
            ulong potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) & ~(1ul << move.sourceSquare);
            ulong rookBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[move.sourceSquare];
            ulong bishopBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[move.sourceSquare];

            ulong rookMask = LookUps.RookMoveDict[(move.sourceSquare, rookBlockers)];
            ulong bishopMask = LookUps.BishopMoveDict[(move.sourceSquare, bishopBlockers)];
            ulong movesMask = (rookMask | bishopMask) & ~FriendlyBitboards.AllPieces;

            ulong checkMask = movesMask &
                (OpponentBitboards.SlidingPieces | FriendlyBitboards.Kings) &
                LookUps.queenMoves[kingPosition] &
                ~ (1ul << move.targetSquare);

            if(checkMask == 0) return false;

            potentialBlockers = (FriendlyBitboards.AllPieces | OpponentBitboards.AllPieces) & ~(1ul << kingPosition);
            potentialBlockers |= (1ul << move.targetSquare);
            potentialBlockers ^= (1ul << move.sourceSquare);
            rookBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[kingPosition];
            rookMask = LookUps.RookMoveDict[(kingPosition, rookBlockers)];

            ulong rookCheckMask = checkMask & (OpponentBitboards.Rooks | OpponentBitboards.Queens);
            if (rookCheckMask > 0 && (rookMask & rookCheckMask) > 0) return true;

            bishopBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[kingPosition];
            bishopMask = LookUps.BishopMoveDict[(kingPosition, bishopBlockers)];
            ulong bishopCheckMask = checkMask & (OpponentBitboards.Bishops | OpponentBitboards.Queens);

            return bishopCheckMask > 0 && (bishopMask & bishopCheckMask) > 0;
        }

        private static bool IsPieceInDirection(int kingPosition, int moveTargetPosition, int checkPiecePosition)
        {
            int kingToMoved = LookUps.directionIndex[kingPosition, moveTargetPosition];
            int kingToCheck = LookUps.directionIndex[kingPosition, checkPiecePosition];

            return kingToCheck != -1 && kingToMoved == kingToCheck;
        }
    }
}
