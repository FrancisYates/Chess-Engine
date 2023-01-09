using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class MoveGeneration
    {
        public static int[][] numSquaresInDirection = new int[64][];
        private static readonly short[] directionOffsets = { 8, -8, -1, 1, 7, 9, -9, -7};
        private readonly static List<Move> moves = new List<Move>();

        private static void GenerateMoves(bool whiteMoves, bool generateOnlyCaptures)
        {
            moves.Clear();
            int[] board = BoardManager.GetBoard();
            int toMoveColour = whiteMoves ? 8 : 0;
            List<int> piecePositions = whiteMoves ? BoardManager.whitePiecePositions : BoardManager.blackPiecePositions;
            //PERF Store Piece position rather than looping
            for (int sourceSquare = 0; sourceSquare < 64; sourceSquare++)
            //foreach (int sourceSquare in piecePositions)
            {
                int pieceAtPosition = board[sourceSquare];

                if (pieceAtPosition == 0 || !Piece.IsPieceColour(pieceAtPosition, toMoveColour))
                {
                    continue;
                }

                if (Piece.IsType(pieceAtPosition, Piece.PieceType.Pawn))
                {
                    GeneratePawnMoves(sourceSquare, pieceAtPosition, board, generateOnlyCaptures);
                    continue;
                }
                if (Piece.IsSlidingPiece(pieceAtPosition)){
                    GenerateSlidingMoves(sourceSquare, pieceAtPosition, board, generateOnlyCaptures);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, Piece.PieceType.Knight))
                {
                    GenerateKnightMoves(sourceSquare, pieceAtPosition, board, generateOnlyCaptures);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, Piece.PieceType.King))
                {
                    GenerateKingMoves(sourceSquare, pieceAtPosition, board, generateOnlyCaptures);
                }
            }
        }


        public static Move[] GenerateStricLegalMoves(bool whiteMoves, bool generateOnlyCaptures = false)
        {
            GenerateMoves(whiteMoves, generateOnlyCaptures);
            RemoveIllegalMoves(whiteMoves);
            return moves.ToArray();
        }

        private static void GenerateSlidingMoves(int sourceSquare, int piece, int[] board, bool generateOnlyCaptures)
        {
            int startIdx = Piece.IsType(piece, Piece.PieceType.Bishop)? 4 : 0;
            int endIdx = Piece.IsType(piece, Piece.PieceType.Rook) ? 4 : 8;

            for (int directionIdx = startIdx; directionIdx < endIdx; directionIdx++)
            {
                for (int i = 1; i <= numSquaresInDirection[sourceSquare][directionIdx]; i++)
                {
                    int targetSquare = sourceSquare + i * directionOffsets[directionIdx];
                    int pieceAtTarget = board[targetSquare];

                    if (pieceAtTarget != 0)
                    {
                        if (Piece.IsSameColour(piece, pieceAtTarget))
                        {
                            break;
                        }
                        else
                        {
                            moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture));
                            break;
                        }
                    }
                    if (!generateOnlyCaptures) { moves.Add(new Move(sourceSquare, targetSquare)); }
                }
            }
        }

        private static void GeneratePawnMoves(int sourceSquare, int piece, int[] board, bool generateOnlyCaptures)
        {
            bool isWhite = Piece.IsPieceColour(piece, 8); // check if piece is white. Enum is available but may be slower 
            int moveDirection = isWhite ? 1 : -1;

            int[] attackOffsets = LookUps.pawnAttackOffset[isWhite ? 1 : 0, sourceSquare];
            bool enPesantPossible = BoardManager.enPesantSquare != -1;
            foreach (int offset in attackOffsets)
            {
                int targetSquare = sourceSquare + offset;
                if(targetSquare < 0 || targetSquare > 63)
                {
                   continue;
                }
                int pieceAtTarget = board[targetSquare];

                if (enPesantPossible && targetSquare == BoardManager.enPesantSquare)
                {
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.enPesant));
                    continue;
                }
                if (pieceAtTarget == 0)
                {
                    continue;
                }
                if (!Piece.IsSameColour(pieceAtTarget, piece))
                {
                    if (Piece.IsAtFinalRank(isWhite, targetSquare))
                    {
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.bishop));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.rook));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.knight));
                        moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture, PromotionPiece.queen));
                        continue;
                    }
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.capture));
                    continue;
                }
            }

            if (generateOnlyCaptures) { return; }

            int forwardMoves = Piece.HasPawnMoved(isWhite, sourceSquare) ? 1 : 2;
            for(int i = 1; i <= forwardMoves; i++)
            {
                int targetSquare = sourceSquare + (8 * i * moveDirection);
                if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }
                int pieceAtTarget = board[targetSquare];

                if (pieceAtTarget != 0)
                {
                    break;
                }
                if (Piece.IsAtFinalRank(isWhite, targetSquare))
                {
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.promotion, PromotionPiece.bishop));
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.promotion, PromotionPiece.rook));
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.promotion, PromotionPiece.knight));
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.promotion, PromotionPiece.queen));
                    continue;
                }
                if(i == 2)
                {
                    moves.Add(new Move(sourceSquare, targetSquare, MoveType.doublePawnMove));
                    continue;
                }
                moves.Add(new Move(sourceSquare, targetSquare));
            }
        }

        private static void GenerateKnightMoves(int sourceSquare, int piece, int[] board, bool generateOnlyCaptures)
        {
            int[] offsets = LookUps.knightOffset[sourceSquare];
            for (int i = 0; i < offsets.Length; i++){
                int targetSquare = sourceSquare + offsets[i];
                if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }
                int targetPiece = board[targetSquare];
                
                if(targetPiece == 0 && !generateOnlyCaptures)
                {
                    moves.Add((Move)new Move(sourceSquare, targetSquare));
                    continue;
                }
                if(!Piece.IsSameColour(piece, targetPiece))
                {
                    moves.Add((Move)new Move(sourceSquare,targetSquare, MoveType.capture));
                }
            }
        }

        private static void GenerateKingMoves(int sourceSquare, int piece, int[] board, bool generateOnlyCaptures)
        {
            int[] offsets = LookUps.kingOffset[sourceSquare];

            for (int i = 0; i < offsets.Length; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                int targetPiece = board[targetSquare];
                
                if(targetPiece == 0 && !generateOnlyCaptures)
                {
                    moves.Add((Move)new Move(sourceSquare, targetSquare));
                    continue;
                }
                if (!Piece.IsSameColour(piece, targetPiece))
                {
                    moves.Add((Move)new Move(sourceSquare, targetSquare, MoveType.capture));
                }
            }

            if (generateOnlyCaptures) { return; }

            bool isWhite = Piece.IsPieceWhite(piece);

            if (CanCastleKingSide(isWhite, sourceSquare, board))
            {
                moves.Add((Move)new Move(sourceSquare, sourceSquare + 2, MoveType.castle));
            }
            if (CanCastleQueenSide(isWhite, sourceSquare, board))
            {
                moves.Add((Move)new Move(sourceSquare, sourceSquare - 2, MoveType.castle));
            }
        }

        public static bool CanCastleKingSide(bool isWhite, int sourceSquare, int[] board)
        {
            bool castleingAvailable = isWhite ? BoardManager.CanWhiteCastleKS() : BoardManager.CanBlackCastleKS();

            if (IsSquareAttackedByOpponent(sourceSquare, !isWhite))
            {
                return false;
            }
            if (castleingAvailable)
            {
                int target1 = sourceSquare + 1;
                int target2 = sourceSquare + 2;
                bool isBlocked = board[target1] != 0 || board[target2] != 0;
                bool isControlled = IsSquareAttackedByOpponent(target1, !isWhite) || IsSquareAttackedByOpponent(target2, !isWhite);
                return !isBlocked && !isControlled;
            }

            return false;
        }

        public static bool CanCastleQueenSide(bool isWhite, int sourceSquare, int[] board)
        {
            bool castleingAvailable = isWhite ? BoardManager.CanWhiteCastleQS() : BoardManager.CanBlackCastleQS();

            if(IsSquareAttackedByOpponent(sourceSquare, !isWhite))
            {
                return false;
            }
            if (castleingAvailable)
            {
                int target1 = sourceSquare - 1;
                int target2 = sourceSquare - 2;
                int target3 = sourceSquare - 3;
                bool isBlocked = board[target1] != 0 || board[target2] != 0 || board[target3] != 0;
                bool isControlled = IsSquareAttackedByOpponent(target1, !isWhite) || IsSquareAttackedByOpponent(target2, !isWhite);
                return !isBlocked && !isControlled;
            }

            return false;
        }

        public static void CalculateDirections()
        {
            for(int file = 0; file < 8; file++)
            {
                for(int rank = 0; rank < 8; rank++)
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

        private static void RemoveIllegalMoves(bool whiteMoves)
        {
            List<Move> illegalMoves = new List<Move>();
            int kingPosition = BoardManager.GetKingPosition(whiteMoves);
            if(kingPosition == -1)
            {
                moves.Clear();
                return;
            }
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
            int king = BoardManager.GetBoard()[move.sourceSquare];
            bool isOpponentWhite = !Piece.IsPieceWhite(king);

            if (IsSquareAttackedByOpponent(move.targetSquare, isOpponentWhite))
            {
                return true;
            }
            if (IsSquareAttackedByOpponent(move.sourceSquare, isOpponentWhite))
            {
                List<int> checkingPieces = BoardManager.FindCheckingPieces(move.sourceSquare, king);
                if (checkingPieces.Count == 1)
                {
                    int checkingPiecePosition = checkingPieces[0];
                    int checkingPiece = BoardManager.GetBoard()[checkingPiecePosition];
                    if (move.targetSquare == checkingPiecePosition)
                    {
                        return false;
                    }
                    if (Piece.IsSlidingPiece(checkingPiece))
                    {
                        int kingToMove = LookUps.directionIndex[move.sourceSquare, move.targetSquare];
                        int checkToKing = LookUps.directionIndex[checkingPiecePosition, move.sourceSquare];

                        if (checkToKing == -1)
                        {
                            return false;
                        }
                        return kingToMove == checkToKing;
                    }
                    return false;
                }

                foreach (int checkingPosition in checkingPieces)
                {
                    int checkingPiece = BoardManager.GetBoard()[checkingPosition];
                    if (Piece.IsSlidingPiece(checkingPiece))
                    {
                        int kingToMove = LookUps.directionIndex[move.sourceSquare, move.targetSquare];
                        int checkToKing = LookUps.directionIndex[checkingPosition, move.sourceSquare];

                        if (checkToKing == -1)
                        {
                            return false;
                        }
                        if( kingToMove == checkToKing)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }

        private static bool IsCheckBlocked(int kingPosition, Move move)
        {
            int king = BoardManager.GetBoard()[kingPosition];

            List<int> checkingPieces = BoardManager.FindCheckingPieces(kingPosition, king);

            if (checkingPieces.Count > 1)
            {
                return true;
            }

            if (checkingPieces.Count == 0)
            {
                //TODO This should never happen, but it does
                return IsCheckCreatedInDirection(kingPosition, move);
            }

            int piecePosition = checkingPieces[0];
            int checkingPiece = BoardManager.GetBoard()[piecePosition];
            if (Piece.IsSlidingPiece(checkingPiece))
            {
                if (IsPieceInDirection(kingPosition, move.targetSquare, piecePosition))
                {
                    if (move.targetSquare == piecePosition)
                    {
                        return IsCheckCreatedInDirection(kingPosition, move);
                    }
                    if (IsCheckBlocked(kingPosition, move.targetSquare, piecePosition))
                    {
                        return IsCheckCreatedInDirection(kingPosition, move);
                    }
                    return true;
                }
                return true;
            }
            else
            {
                if (IsCheckCreatedInDirection(kingPosition, move))
                {
                    return true;
                }
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
            int king = BoardManager.GetBoard()[kingPosition];
            bool isOpponentWhite = !Piece.IsPieceWhite(king);
            int movedPiece = BoardManager.GetBoard()[move.sourceSquare];

            if (Piece.IsType(movedPiece, Piece.PieceType.King))
            {
                return DoesKingMoveCauseCheck(move);
            }

            if (IsSquareAttackedByOpponent(kingPosition, isOpponentWhite))
            {
                return IsCheckBlocked(kingPosition, move);
            }
            else
            {
                if (move.IsType( MoveType.enPesant)) return DoesEnPesantCreatesCheck(kingPosition, move);
                if (IsSquareAttackedByOpponent(move.sourceSquare, isOpponentWhite))return IsCheckCreatedInDirection(kingPosition, move);
                return false;
            }
        }

        private static bool DoesEnPesantCreatesCheck(int kingPosition, Move move)
        {
            int king = BoardManager.GetBoard()[kingPosition];

            int capturedOffest = move.targetSquare % 8 - move.sourceSquare % 8;
            int capturedPosition = move.sourceSquare + capturedOffest;

            int sideColour = Piece.IsPieceWhite(king) ? 8 : 0;
            int directionToKing = LookUps.directionIndex[move.sourceSquare, kingPosition];
            if (directionToKing == -1)
            {
                return false;
            }
            int maxOffset = numSquaresInDirection[move.sourceSquare][directionToKing];
            for (int i = 0; i < maxOffset; i++)
            {
                int targetSquare = move.sourceSquare + (i + 1) * directionOffsets[directionToKing];
                int pieceAtTarget = BoardManager.GetBoard()[targetSquare];

                if (pieceAtTarget == 0) { continue; }

                if (targetSquare == capturedPosition) { continue; }

                if (targetSquare == kingPosition)
                {
                    break;
                }
                return false;
            }

            int directionIdx = LookUps.directionIndex[kingPosition, move.sourceSquare];
            if (directionIdx == -1)
            {
                return false;
            }

            maxOffset = numSquaresInDirection[move.sourceSquare][directionIdx];
            for (int i = 0; i < maxOffset; i++)
            {
                int targetSquare = move.sourceSquare + (i + 1) * directionOffsets[directionIdx];

                if(targetSquare == capturedPosition )
                {
                    continue;
                }

                int pieceAtTarget = BoardManager.GetBoard()[targetSquare];

                if (pieceAtTarget == 0) { continue; }

                if (Piece.IsSameColour(sideColour, pieceAtTarget))
                {
                    break;
                }

                if (pieceAtTarget != 0)
                {
                    if (Piece.IsSlidingPiece(pieceAtTarget))
                    {
                        if (directionIdx < 4)
                        {
                            return Piece.IsType(pieceAtTarget, Piece.PieceType.Rook) || Piece.IsType(pieceAtTarget, Piece.PieceType.Queen);
                        }
                        return Piece.IsType(pieceAtTarget, Piece.PieceType.Bishop) || Piece.IsType(pieceAtTarget, Piece.PieceType.Queen);
                    }
                    return false;
                }
            }

            return false;
        }

        private static bool IsCheckBlocked(int kingPosition, int moveTarget, int checkingPiecePosition)
        {
            bool kingPositionLower = kingPosition - checkingPiecePosition < 0;
            return moveTarget - checkingPiecePosition < 0 == kingPositionLower;
        }

        private static bool IsSquareAttackedByOpponent(int squareIndex, bool isOpponentWhite)
        {
            int attackedIndicator = isOpponentWhite ? 2 : 1;
            int positionAndIndicator = BoardManager.attackPositionBoard[squareIndex] & attackedIndicator;
            return positionAndIndicator == attackedIndicator;
        }

        private static bool IsCheckCreatedInDirection(int kingPosition, Move move)
        {
            int xDelta = move.sourceSquare % 8 - kingPosition % 8;
            int yDelta = move.sourceSquare / 8 - kingPosition / 8;

            if (xDelta != 0 && yDelta != 0 && Math.Abs(xDelta) != Math.Abs(yDelta))
            {
                return false;
            }

            if (!IsPathToKingExist(kingPosition, move.sourceSquare))
            {
                return false;
            }

            //PERF create default to discard pieces not in line with king
            int directionFromKing = LookUps.directionIndex[kingPosition, move.sourceSquare];
            int directionToMove = LookUps.directionIndex[move.sourceSquare, move.targetSquare];
            if(directionToMove == directionFromKing)
            {
                return false;
            }
            int directionToKing = LookUps.directionIndex[move.sourceSquare, kingPosition];
            if (directionToMove == directionToKing)
            {
                return false;
            }

            int king = BoardManager.GetBoard()[kingPosition];
            int sideColour = Piece.IsPieceWhite(king) ? 8 : 0;
            return BoardManager.IsSlidingPieceInDirection(move.sourceSquare, directionFromKing, sideColour);
        }

        private static bool IsPathToKingExist(int kingPosition, int movedPiecePosition)
        {
            short[] directionOffsets = { 8, -8, -1, 1, 7, 9, -9, -7 };

            int directionIdx = LookUps.directionIndex[movedPiecePosition, kingPosition];
            if(directionIdx == -1)
            {
                return false;
            }

            int maxOffset = MoveGeneration.numSquaresInDirection[movedPiecePosition][directionIdx];
            for (int i = 0; i < maxOffset; i++)
            {
                int targetSquare = movedPiecePosition + (i + 1) * directionOffsets[directionIdx];
                int pieceAtTarget = BoardManager.GetBoard()[targetSquare];

                if (pieceAtTarget == 0) { continue; }

                if(targetSquare == kingPosition) { return true; }

                return false;
            }

            return false;
        }

        private static bool IsPieceInDirection(int kingPosition, int moveTargetPosition, int checkPiecePosition)
        {
            int kingToMoved = LookUps.directionIndex[kingPosition, moveTargetPosition];
            int kingToCheck = LookUps.directionIndex[kingPosition, checkPiecePosition];

            return kingToCheck != -1 && (kingToMoved == kingToCheck);
        }
    }

}
