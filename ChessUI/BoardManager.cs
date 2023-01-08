using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class BoardManager
    {
        private static int castleingRights;
        public static bool whiteToMove;
        public static int halfMoves;
        public static int fullMoves;
        private readonly static int[] board = new int[64];
        public static int enPesantSquare = -1;
        public static int[] attackPositionBoard = new int[64];
        static readonly int[] directionOffsets = { 8, -8, -1, 1, 7, 9, -9, -7 };
        public static List<int> whitePiecePositions = new List<int>();
        public static List<int> blackPiecePositions = new List<int>();

        public static int GetCastilingRights()
        {
            return castleingRights;
        }
        public static void SetCastilingRights(int rights)
        {
            castleingRights = rights;
        }

        public static bool CanWhiteCastleKS()
        {
            int rook = board[7];
            bool validRook = rook == 13;
            return validRook && (castleingRights & 8) == 8;
        }
        public static bool CanWhiteCastleQS()
        {
            int rook = board[0];
            bool validRook = rook == 13;
            return validRook && (castleingRights & 4) == 4;
        }
        public static bool CanBlackCastleKS()
        {
            int rook = board[63];
            bool validRook = rook == 5;
            return validRook && (castleingRights & 2) == 2;
        }
        public static bool CanBlackCastleQS()
        {
            int rook = board[56];
            bool validRook = rook == 5;
            return validRook && (castleingRights & 1) == 1;
        }

        public static void UpdateMoveCount()
        {
            fullMoves += halfMoves;
            halfMoves = (halfMoves + 1) % 2;
        }

        private static string[] GetFEN(string saveFile)
        {
            string FEN;
            if (saveFile == "")
            {
                FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            }
            else
            {
                FEN = System.IO.File.ReadAllText(saveFile);
            }
            return FEN.Split(' ');
        }

        private static void PopulateBoard(string boardFen)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(boardFen);

            int rank = 7;
            int file = 0;

            int index;
            foreach (char asciiValue in asciiBytes)
            {
                if (asciiValue == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(asciiValue))
                    {
                        file += (int)Char.GetNumericValue(asciiValue);
                    }
                    else
                    {
                        int pieceNumber = getPieceNum(asciiValue);
                        index = 8 * rank + file;
                        board[index] = pieceNumber;
                        if (Piece.IsPieceWhite(pieceNumber))
                        {
                            whitePiecePositions.Add(index);
                        }
                        else
                        {
                            blackPiecePositions.Add(index);
                        }
                        file++;
                    }
                }
            }

            int getPieceNum(char aciiVal)
            {
                switch (aciiVal)
                {
                    case 'P':
                        return 9;
                    case 'p':
                        return 1;

                    case 'R':
                        return 13;
                    case 'r':
                        return 5;

                    case 'N':
                        return 10;
                    case 'n':
                        return 2;

                    case 'B':
                        return 14;
                    case 'b':
                        return 6;

                    case 'K':
                        return 11;
                    case 'k':
                        return 3;

                    case 'Q':
                        return 15;
                    case 'q':
                        return 7;
                    default:
                        return 0;
                }
            }
        }

        public static void LoadBoard(string saveFile = "")
        {
            string[] FENSplit = GetFEN(saveFile);

            PopulateBoard(FENSplit[0]);

            whiteToMove = (FENSplit[1] == "w");
            string castlingRightsString = FENSplit[2];
            SetupCastleRights(castlingRightsString);

            string enPesantString = FENSplit[3];
            enPesantSquare = -1;
            if (enPesantString != "-")
            {
                enPesantSquare = GetSquareFromNotation(enPesantString);
            }
            halfMoves = Int32.Parse(FENSplit[4]);
            fullMoves = Int32.Parse(FENSplit[4]);

            int GetSquareFromNotation(string positionNotation)
            {
                int notationRank = (int)Char.GetNumericValue(positionNotation[1]) - 1;
                char notationFileChar = positionNotation[0];
                int notationFile = 0;
                switch (notationFileChar)
                {
                    case 'a':
                        notationFile = 0;
                        break;
                    case 'b':
                        notationFile = 1;
                        break;
                    case 'c':
                        notationFile = 2;
                        break;
                    case 'd':
                        notationFile = 3;
                        break;
                    case 'e':
                        notationFile = 4;
                        break;
                    case 'f':
                        notationFile = 5;
                        break;
                    case 'g':
                        notationFile = 6;
                        break;
                    case 'h':
                        notationFile = 7;
                        break;
                }
                return notationRank * 8 + notationFile;
            }
        }

        public static void ResetBoardToEmpty()
        {
            for (int i = 0; i < 64; i++)
            {
                board[i] = 0;
            }
        }

        private static void UpdatePiecePositions(Move move)
        {
            int piece = board[move.sourceSquare];
            if (Piece.IsPieceWhite(piece))
            {
                whitePiecePositions.Remove(move.sourceSquare);
                whitePiecePositions.Add(move.targetSquare);
            }
            else
            {
                blackPiecePositions.Remove(move.sourceSquare);
                blackPiecePositions.Add(move.targetSquare);
            }
        }

        private static void UndoPiecePositions(Move move)
        {
            int piece = board[move.sourceSquare];
            if (Piece.IsPieceWhite(piece))
            {
                whitePiecePositions.Remove(move.targetSquare);
                whitePiecePositions.Add(move.sourceSquare);
            }
            else
            {
                blackPiecePositions.Remove(move.targetSquare);
                blackPiecePositions.Add(move.sourceSquare);
            }
        }

        private static void AddPiecePosition(int position, int piece)
        {
            if (Piece.IsPieceWhite(piece))
            {
                whitePiecePositions.Add(position);
            }
            else
            {
                blackPiecePositions.Add(position);
            }
        }

        private static void RemovePiecePosition(int position, int piece)
        {
            //int piece = board[position];
            if (Piece.IsPieceWhite(piece))
            {
                whitePiecePositions.Remove(position);
            }
            else
            {
                blackPiecePositions.Remove(position);
            }
        }

        private static void SetupCastleRights(string castleRightsString)
        {
            castleingRights = 0;
            if (castleRightsString.Contains("K"))
            {
                castleingRights |= 8;
            }
            if (castleRightsString.Contains("Q"))
            {
                castleingRights |= 4;
            }
            if (castleRightsString.Contains("k"))
            {
                castleingRights |= 2;
            }
            if (castleRightsString.Contains("q"))
            {
                castleingRights |= 1;
            }
        }

        public static void UpdateSideToMove()
        {
            whiteToMove = !whiteToMove;
        }

        public static int[] GetBoard()
        {
            return board;
        }

        public static (int, int) MakeMove(Move move)
        {
            enPesantSquare = -1;
            int targetContents = board[move.targetSquare];
            int movedPiece = board[move.sourceSquare];
            bool isWhite = Piece.IsPieceWhite(movedPiece);
            int side = isWhite ? 8 : 0;
            int validCastling = castleingRights;

            //UpdatePiecePositions(move);
            switch (move.moveType)
            {
                case Move.MoveType.doublePawnMove:
                    int offset = isWhite ? -8 : 8;
                    enPesantSquare = move.targetSquare + offset;

                    board[move.targetSquare] = board[move.sourceSquare];
                    board[move.sourceSquare] = 0;
                    break;

                case Move.MoveType.enPesant:
                    int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;

                    targetContents = board[move.sourceSquare + xDelta];
                    //RemovePiecePosition(move.sourceSquare + xDelta, targetContents);
                    board[move.targetSquare] = board[move.sourceSquare];
                    board[move.sourceSquare] = 0;
                    board[move.sourceSquare + xDelta] = 0;
                    break;

                case Move.MoveType.castle:
                    CastleMove(move, isWhite);
                    break;

                case Move.MoveType.promotionBishop:
                    targetContents = 0;
                    board[move.targetSquare] = (int)Piece.PieceType.Bishop | side;
                    board[move.sourceSquare] = 0;
                    break;

                case Move.MoveType.promotionBishopCapture:
                    targetContents = board[move.targetSquare];
                    board[move.targetSquare] = (int)Piece.PieceType.Bishop | side;
                    board[move.sourceSquare] = 0;
                    break;

                case Move.MoveType.promotionRook:
                    targetContents = 0;
                    board[move.targetSquare] = (int)Piece.PieceType.Rook | side;
                    board[move.sourceSquare] = 0;
                    break;
                case Move.MoveType.promotionRookCapture:
                    targetContents = board[move.targetSquare];
                    board[move.targetSquare] = (int)Piece.PieceType.Rook | side;
                    board[move.sourceSquare] = 0;
                    break;

                case Move.MoveType.promotionKnight:
                    targetContents = 0;
                    board[move.targetSquare] = (int)Piece.PieceType.Knight | side;
                    board[move.sourceSquare] = 0;
                    break;
                case Move.MoveType.promotionKnightCapture:
                    targetContents = board[move.targetSquare];
                    board[move.targetSquare] = (int)Piece.PieceType.Knight | side;
                    board[move.sourceSquare] = 0;
                    break;

                case Move.MoveType.promotionQueen:
                    targetContents = 0;
                    board[move.targetSquare] = (int)Piece.PieceType.Queen | side;
                    board[move.sourceSquare] = 0;
                    break;
                case Move.MoveType.promotionQueenCapture:
                    targetContents = board[move.targetSquare];
                    board[move.targetSquare] = (int)Piece.PieceType.Queen | side;
                    board[move.sourceSquare] = 0;
                    break;

                default:
                    targetContents = StandardMove(move, isWhite, movedPiece);
                    break;
            }

            return (targetContents, validCastling);
        }
        private static void CastleMove(Move move, bool isWhite)
        {
            bool kingSide = move.sourceSquare - move.targetSquare < 0;
            int side = isWhite ? 8 : 0;

            if (kingSide)
            {
                int rookPosition = isWhite ? 7 : 63;
                board[move.sourceSquare + 1] = (int)Piece.PieceType.Rook | side;
                board[rookPosition] = 0;
            }
            else
            {
                int rookPosition = isWhite ? 0 : 56;
                board[move.sourceSquare - 1] = (int)Piece.PieceType.Rook | side;
                board[rookPosition] = 0;
            }

            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;
            int castleingChange = isWhite ? 0b_0011 : 0b_1100;
            castleingRights &= castleingChange;

        }

        private static int StandardMove(Move move, bool isWhite, int movedPiece)
        {
            int target = board[move.targetSquare];
            board[move.targetSquare] = board[move.sourceSquare];
            board[move.sourceSquare] = 0;

            if (Piece.IsType(movedPiece, Piece.PieceType.Rook))
            {
                if (!Piece.HasRookMoved(isWhite, move.sourceSquare))
                {
                    int whiteOrBlack = isWhite ? 0b_0011 : 0b_1100;
                    int rookKingSideSquare = isWhite ? 7 : 63;
                    int kingOrQueen = move.sourceSquare == rookKingSideSquare ? 0b_0101 : 0b_1010;
                    int newCastleRights = kingOrQueen | whiteOrBlack;
                    castleingRights &= newCastleRights;
                }
            }
            else if (Piece.IsType(movedPiece, Piece.PieceType.King))
            {
                int castleingChange = isWhite ? 0b_0011 : 0b_1100;
                castleingRights &= castleingChange;
            }

            return target;
        }

        public static void UndoMove(Move move, int priorTargetContent, int priorCastlingRights)
        {
            int movedPiece = board[move.targetSquare];
            int side = Piece.IsPieceWhite(movedPiece) ? 8 : 0;
            castleingRights = priorCastlingRights;

            switch (move.moveType)
            {
                case Move.MoveType.doublePawnMove:
                    int offset = Piece.IsPieceWhite(movedPiece) ? -8 : 8;
                    enPesantSquare = move.targetSquare + offset;

                    board[move.sourceSquare] = board[move.targetSquare];
                    board[move.targetSquare] = 0;
                    break;

                case Move.MoveType.enPesant:
                    int xDelta = move.targetSquare % 8 - move.sourceSquare % 8;
                    board[move.sourceSquare] = board[move.targetSquare];
                    board[move.targetSquare] = 0;
                    board[move.sourceSquare + xDelta] = priorTargetContent;
                    //AddPiecePosition(move.sourceSquare + xDelta, priorTargetContent);
                    break;

                case Move.MoveType.castle:
                    UndoCastle(move, side);
                    break;

                case Move.MoveType.promotionBishop:
                    board[move.targetSquare] = 0;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;
                case Move.MoveType.promotionBishopCapture:
                    board[move.targetSquare] = priorTargetContent;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;

                case Move.MoveType.promotionRook:
                    board[move.targetSquare] = 0;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;
                case Move.MoveType.promotionRookCapture:
                    board[move.targetSquare] = priorTargetContent;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;

                case Move.MoveType.promotionKnight:
                    board[move.targetSquare] = 0;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;
                case Move.MoveType.promotionKnightCapture:
                    board[move.targetSquare] = priorTargetContent;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;

                case Move.MoveType.promotionQueen:
                    board[move.targetSquare] = 0;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;
                case Move.MoveType.promotionQueenCapture:
                    board[move.targetSquare] = priorTargetContent;
                    board[move.sourceSquare] = (int)Piece.PieceType.Pawn | side;
                    break;

                default:
                    board[move.sourceSquare] = board[move.targetSquare];
                    board[move.targetSquare] = priorTargetContent;
                    //AddPiecePosition(move.targetSquare, priorTargetContent);
                    break;
            }
            //UndoPiecePositions(move);
        }

        private static void UndoCastle(Move move, int side)
        {
            bool isWhite = Piece.IsPieceWhite(board[move.targetSquare]);
            bool kingSide = move.sourceSquare - move.targetSquare < 0;

            if (kingSide)
            {
                board[move.sourceSquare + 1] = 0;
                int rookPosition = isWhite ? 7 : 63;
                board[rookPosition] = (int)Piece.PieceType.Rook | side;
            }
            else
            {
                board[move.sourceSquare - 1] = 0;
                int rookPosition = isWhite ? 0 : 56;
                board[rookPosition] = (int)Piece.PieceType.Rook | side;
            }
            board[move.sourceSquare] = board[move.targetSquare];
            board[move.targetSquare] = 0;
        }

        public static int GetKingPosition(bool whiteKing)
        {
            for(int position = 0; position < 64; position++)
            {
                int piece = board[position];
                if(Piece.IsType(piece, Piece.PieceType.King))
                {
                    if (Piece.IsPieceWhite(piece) == whiteKing)
                    {
                        return position;
                    }
                }
            }
            return -1;
        }

        public static void UpdateAttackedPositions(bool whiteMoves)
        {
            ResetAttackedSquares(whiteMoves);

            for (int sourceSquare = 0; sourceSquare < 64; sourceSquare++)
            {
                int pieceAtPosition = board[sourceSquare];

                if (pieceAtPosition == 0 || Piece.IsPieceWhite(pieceAtPosition) != whiteMoves) continue;

                if (Piece.IsType(pieceAtPosition, Piece.PieceType.Pawn))
                {
                    UpdatePawnAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsSlidingPiece(pieceAtPosition))
                {
                    UpdateSlidingAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, Piece.PieceType.Knight))
                {
                    UpdateKnightAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, Piece.PieceType.King))
                {
                    UpdateKingAttacked(sourceSquare, pieceAtPosition);
                }
            }
        }

        private static void ResetAttackedSquares(bool resetWhite)
        {
            int resetMask = resetWhite ? 1 : 2;
            for (int i = 0; i < 64; i++)
            {
                attackPositionBoard[i] = attackPositionBoard[i] & resetMask;
            }
        }

        private static void UpdateSlidingAttacked(int sourceSquare, int piece)
        {
            int startIdx = Piece.IsType(piece, Piece.PieceType.Bishop) ? 4 : 0;
            int endIdx = Piece.IsType(piece, Piece.PieceType.Rook) ? 4 : 8;

            int[][] numSquaresInDirection = MoveGeneration.numSquaresInDirection;
            int attackIndicator = Piece.IsPieceWhite(piece) ? 2 : 1;
            for (int directionIdx = startIdx; directionIdx < endIdx; directionIdx++)
            {
                for (int i = 1; i <= numSquaresInDirection[sourceSquare][directionIdx]; i++)
                {
                    int targetSquare = sourceSquare + directionOffsets[directionIdx] * i;
                    int pieceAtTarget = board[targetSquare];

                    attackPositionBoard[targetSquare] = attackPositionBoard[targetSquare] | attackIndicator;

                    if (pieceAtTarget != 0)
                    {
                        break;
                    }
                }
            }
        }

        private static void UpdatePawnAttacked(int sourceSquare, int piece)
        {

            int[] attackOffsets = LookUps.pawnAttackOffset[Piece.IsPieceWhite(piece) ? 1 : 0, sourceSquare];

            int attackIndicator = Piece.IsPieceWhite(piece) ? 2 : 1;
            foreach (int offset in attackOffsets)
            {
                int targetSquare = sourceSquare + offset;
                if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }
                attackPositionBoard[targetSquare] = attackPositionBoard[targetSquare] | attackIndicator;
            }
        }

        private static void UpdateKnightAttacked(int sourceSquare, int piece)
        {
            int[] offsets = LookUps.knightOffset[sourceSquare];

            int attackIndicator = Piece.IsPieceWhite(piece) ? 2 : 1;
            for (int i = 0; i < offsets.Length; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }
                attackPositionBoard[targetSquare] = attackPositionBoard[targetSquare] | attackIndicator;
            }
        }

        private static void UpdateKingAttacked(int sourceSquare, int piece)
        {
            int[] offsets = LookUps.kingOffset[sourceSquare];
            int attackIndicator = Piece.IsPieceWhite(piece) ? 2 : 1;

            for (int i = 0; i < offsets.Length; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }
                attackPositionBoard[targetSquare] = attackPositionBoard[targetSquare] | attackIndicator;
            }
        }

        public static List<int> FindCheckingPieces(int kingPosition, int king)
        {
            List<int> checkingPieces = new List<int>();

            List<int> slidingPieces = FindCheckingSlidingPiece(kingPosition, king, board);
            foreach(int piece in slidingPieces)
            {
                checkingPieces.Add(piece);
            }

            List<int> knights = FindCheckingKnight(kingPosition, king, board);
            foreach (int piece in knights)
            {
                checkingPieces.Add(piece);
            }

            List<int> pawns = FindCheckingPawn(kingPosition, king, board);
            foreach (int piece in pawns)
            {
                checkingPieces.Add(piece);
            }
            return checkingPieces;
        }

        public static bool IsSlidingPieceInDirection(int sourceSquare, int direction, int sideColour)
        {

            int maxOffset = MoveGeneration.numSquaresInDirection[sourceSquare][direction];
            for (int i = 0; i < maxOffset; i++)
            {
                int targetSquare = sourceSquare + (i + 1) * directionOffsets[direction];
                int pieceAtTarget = board[targetSquare];

                if(pieceAtTarget == 0) { continue; }

                if (Piece.IsSameColour(sideColour, pieceAtTarget))
                {
                    break;
                }

                if (pieceAtTarget != 0)
                {
                    if (Piece.IsSlidingPiece(pieceAtTarget))
                    {
                        if(direction < 4)
                        {
                            return Piece.IsType(pieceAtTarget, Piece.PieceType.Rook) || Piece.IsType(pieceAtTarget, Piece.PieceType.Queen);
                        }
                        return Piece.IsType(pieceAtTarget, Piece.PieceType.Bishop) || Piece.IsType(pieceAtTarget, Piece.PieceType.Queen);
                    }
                    break;
                }
            }

            return false;
        }

        private static List<int> FindCheckingSlidingPiece(int sourceSquare, int piece, int[] board)
        {
            List<int> pieces = new List<int>();

            for (int directionIdx = 0; directionIdx < 8; directionIdx++)
            {
                int maxOffset = MoveGeneration.numSquaresInDirection[sourceSquare][directionIdx];
                for (int i = 0; i < maxOffset; i++)
                {
                    int targetSquare = sourceSquare + (i + 1) * directionOffsets[directionIdx];
                    int pieceAtTarget = board[targetSquare];

                    if(pieceAtTarget == 0) { continue; }

                    if (Piece.IsSameColour(piece, pieceAtTarget))
                    {
                        break;
                    }

                    if (pieceAtTarget != 0)
                    {
                        if (Piece.IsSlidingPiece(pieceAtTarget))
                        {
                            if(directionIdx < 4 && !Piece.IsType(pieceAtTarget, Piece.PieceType.Bishop))
                            {
                                pieces.Add(targetSquare);
                                break;
                            }
                            if(directionIdx > 3 && !Piece.IsType(pieceAtTarget, Piece.PieceType.Rook))
                            {
                                pieces.Add(targetSquare);
                                break;

                            }
                        }
                        break;
                    }
                }
            }

            return pieces;
        }

        private static List<int> FindCheckingKnight(int sourceSquare, int piece, int[] board)
        {
            List<int> offsets = new List<int> { 15, 17, 10, -6, -15, -17, -10, 6 };
            removeInvalidOffset();
            List<int> pieces = new List<int>();

            for (int i = 0; i < offsets.Count; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                if (targetSquare < 0 || targetSquare > 63) continue;

                int targetPiece = board[targetSquare];
                if (Piece.IsSameColour(piece, targetPiece)) continue;

                if (Piece.IsType(targetPiece, Piece.PieceType.Knight)) pieces.Add(targetSquare);
            }

            return pieces;

            void removeInvalidOffset()
            {
                int xPos = sourceSquare % 8;
                int yPos = sourceSquare / 8;

                if (xPos < 2 || xPos > 5)
                {
                    switch (xPos)
                    {
                        case 0:
                            offsets.Remove(6);
                            offsets.Remove(15);
                            offsets.Remove(-10);
                            offsets.Remove(-17);
                            break;
                        case 1:
                            offsets.Remove(6);
                            offsets.Remove(-10);
                            break;
                        case 6:
                            offsets.Remove(10);
                            offsets.Remove(-6);
                            break;
                        case 7:
                            offsets.Remove(-6);
                            offsets.Remove(-15);
                            offsets.Remove(10);
                            offsets.Remove(17);
                            break;
                    }
                }
                if (yPos < 2 || yPos > 5)
                {
                    switch (yPos)
                    {
                        case 0:
                            offsets.Remove(-6);
                            offsets.Remove(-15);
                            offsets.Remove(-10);
                            offsets.Remove(-17);
                            break;
                        case 1:
                            offsets.Remove(-15);
                            offsets.Remove(-17);
                            break;
                        case 6:
                            offsets.Remove(15);
                            offsets.Remove(17);
                            break;
                        case 7:
                            offsets.Remove(6);
                            offsets.Remove(15);
                            offsets.Remove(10);
                            offsets.Remove(17);
                            break;
                    }
                }
            }
        }

        private static List<int> FindCheckingPawn(int sourceSquare, int piece, int[] board)
        {
            int rank = sourceSquare / 8;
            int file = sourceSquare % 8;
            bool opponentIsWhite = !Piece.IsPieceWhite(piece);
            List<int> pieces = new List<int>();

            if (opponentIsWhite && rank == 0 || !opponentIsWhite && rank == 7) return pieces;

            List<int> attackOffsets = !opponentIsWhite ? new List<int> { 7, 9 } : new List<int> { -9, -7 };
            if (file == 0)
            {
                attackOffsets.Remove(attackOffsets[0]);
            }
            else if (file == 7)
            {
                attackOffsets.Remove(attackOffsets[1]);
            }

            for (int i = 0; i < attackOffsets.Count; i++)
            {
                int targetSquare = sourceSquare + attackOffsets[i];
                int targetPiece = board[targetSquare];
                if(targetPiece == 0) continue;
                if (Piece.IsSameColour(piece, targetPiece)) continue;
                if (Piece.IsType(targetPiece, Piece.PieceType.Pawn)) pieces.Add(targetSquare);
            }

            return pieces;
        }
    }
}
