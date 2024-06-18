using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ChessUI.Enums;

namespace ChessUI.Engine
{
    public static class BoardManager
    {
        private readonly static int[] _board = new int[64];
        public static CastlingRights CastleingRights { get; set; }
        public static bool WhiteToMove { get; set; }
        public static int HalfMoves { get; set; }
        public static int FullMoves { get; set; }
        public static int[] Board => _board;
        public static int EnPesantSquare { get; set; } = -1;
        public static ulong EnPesantBitBoard => EnPesantSquare == -1 ? 0ul : 1ul << EnPesantSquare;
        static readonly int[] directionOffsets = [8, -8, -1, 1, 7, 9, -9, -7];

        public static Bitboards WhiteBitboards { get; set; } = new() { 
            PawnHomeRank = 0b1111111100000000,
            FinalRank = 0b1111111100000000000000000000000000000000000000000000000000000000
        };
        public static Bitboards BlackBitboards { get; set; } = new()
        {
            PawnHomeRank = 0b11111111000000000000000000000000000000000000000000000000,
            FinalRank = 0b11111111
        };
        

        public static CastlingRights GetCastilingRights()
        {
            return CastleingRights;
        }
        public static void SetCastilingRights(CastlingRights rights)
        {
            CastleingRights = rights;
        }

        public static bool CanWhiteCastleKS()
        {
            int rook = Board[7];
            bool validRook = rook == 13;
            return validRook && CastleingRights.HasFlag(CastlingRights.WhiteQueenSide);
        }
        public static bool CanWhiteCastleQS()
        {
            int rook = Board[0];
            bool validRook = rook == 13;
            return validRook && CastleingRights.HasFlag(CastlingRights.WhiteQueenSide);
        }
        public static bool CanBlackCastleKS()
        {
            int rook = Board[63];
            bool validRook = rook == 5;
            return validRook && CastleingRights.HasFlag(CastlingRights.BlackKingSide);
        }
        public static bool CanBlackCastleQS()
        {
            int rook = Board[56];
            bool validRook = rook == 5;
            return validRook && CastleingRights.HasFlag(CastlingRights.BlackQueenSide);
        }

        public static void UpdateMoveCount()
        {
            FullMoves += HalfMoves;
            HalfMoves = (HalfMoves + 1) % 2;
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
            foreach (char asciiValue in asciiBytes.Select(v => (char)v))
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
                        file += (int)char.GetNumericValue(asciiValue);
                    }
                    else
                    {
                        int pieceNumber = asciiValue switch
                        {
                            'P' => 9,
                            'p' => 1,
                            'R' => 13,
                            'r' => 5,
                            'N' => 10,
                            'n' => 2,
                            'B' => 14,
                            'b' => 6,
                            'K' => 11,
                            'k' => 3,
                            'Q' => 15,
                            'q' => 7,
                            _ => 0
                        };
                        bool isPawn = pieceNumber == 1 || pieceNumber == 9;
                        index = 8 * rank + file;
                        Board[index] = pieceNumber;
                        Bitboards bitboards;
                        if (Piece.IsPieceWhite(pieceNumber))
                        {
                            bitboards = WhiteBitboards;
                        }
                        else
                        {
                            bitboards = BlackBitboards;
                        }
                        var pieceType = Piece.GetPieceType(pieceNumber);
                        ref ulong bitboard = ref GetBitboard(ref bitboards, pieceType);
                        bitboard |= 0b_1uL << index;
                        bitboards.AllPieces |= 0b_1uL << index;
                        file++;
                    }
                }
            }
        }

        public static ref ulong GetBitboard(ref Bitboards bitboards, PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return ref bitboards.Pawns;
                case PieceType.Rook:
                    return ref bitboards.Rooks;
                case PieceType.Knight:
                    return ref bitboards.Knights;
                case PieceType.Bishop:
                    return ref bitboards.Bishops;
                case PieceType.Queen:
                    return ref bitboards.Queens;
                case PieceType.King:
                    return ref bitboards.Kings;
                default:
                    throw new NotImplementedException();
            }
        }
        public static void LoadBoardFromFile(string saveFile)
        {
            string[] FENSplit = GetFEN(saveFile);

            PopulateBoard(FENSplit[0]);

            WhiteToMove = FENSplit[1] == "w";
            string castlingRightsString = FENSplit[2];
            SetupCastleRights(castlingRightsString);

            string enPesantString = FENSplit[3];
            EnPesantSquare = -1;
            if (enPesantString != "-")
            {
                EnPesantSquare = GetSquareFromNotation(enPesantString);
            }
            HalfMoves = int.Parse(FENSplit[4]);
            FullMoves = int.Parse(FENSplit[4]);
        }
        public static void LoadBoardFromFen(string fen)
        {
            string[] FENSplit = fen.Split(' ');

            PopulateBoard(FENSplit[0]);

            WhiteToMove = FENSplit[1] == "w";
            string castlingRightsString = FENSplit[2];
            SetupCastleRights(castlingRightsString);

            string enPesantString = FENSplit[3];
            EnPesantSquare = -1;
            if (enPesantString != "-")
            {
                try {

                EnPesantSquare = GetSquareFromNotation(enPesantString);
                } catch (Exception) {

                    throw;
                }
            }
            HalfMoves = int.Parse(FENSplit[4]);
            FullMoves = int.Parse(FENSplit[4]);
        }
        private static int GetSquareFromNotation(string positionNotation)
        {
            int notationRank = (int)char.GetNumericValue(positionNotation[1]) - 1;
            char notationFileChar = positionNotation[0];
            int notationFile = notationFileChar switch
            {
                'a' => 0,
                'b' => 1,
                'c' => 2,
                'd' => 3,
                'e' => 4,
                'f' => 5,
                'g' => 6,
                'h' => 7,
                _ => throw new NotImplementedException()
            };
            return notationRank * 8 + notationFile;
        }

        public static void ResetBoardToEmpty()
        {
            for (int i = 0; i < 64; i++)
            {
                Board[i] = 0;
            }
            WhiteBitboards = new()
            {
                PawnHomeRank = 0b1111111100000000,
                FinalRank = 0b1111111100000000000000000000000000000000000000000000000000000000
            };
            BlackBitboards = new()
            {
                PawnHomeRank = 0b11111111000000000000000000000000000000000000000000000000,
                FinalRank = 0b11111111
            };
        }

        private static void SetupCastleRights(string castleRightsString)
        {
            CastleingRights = 0;
            if (castleRightsString.Contains("K"))
            {
                CastleingRights |= CastlingRights.WhiteKingSide;
            }
            if (castleRightsString.Contains("Q"))
            {
                CastleingRights |= CastlingRights.WhiteQueenSide;
            }
            if (castleRightsString.Contains("k"))
            {
                CastleingRights |= CastlingRights.BlackKingSide;
            }
            if (castleRightsString.Contains("q"))
            {
                CastleingRights |= CastlingRights.BlackQueenSide;
            }
        }

        public static void UpdateSideToMove()
        {
            WhiteToMove = !WhiteToMove;
        }

        public static int GetKingPosition(bool whiteKing)
        {
            for (int position = 0; position < 64; position++)
            {
                int piece = Board[position];
                if (Piece.IsType(piece, PieceType.King))
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
            ResetAttackedBitBoards();

            for (int sourceSquare = 0; sourceSquare < 64; sourceSquare++)
            {
                int pieceAtPosition = Board[sourceSquare];

                if (Piece.IsType(pieceAtPosition, PieceType.Pawn))
                {
                    UpdatePawnAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsSlidingPiece(pieceAtPosition))
                {
                    UpdateSlidingAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, PieceType.Knight))
                {
                    UpdateKnightAttacked(sourceSquare, pieceAtPosition);
                    continue;
                }
                if (Piece.IsType(pieceAtPosition, PieceType.King))
                {
                    UpdateKingAttacked(sourceSquare, pieceAtPosition);
                }
            }
        }

        private static void ResetAttackedBitBoards()
        {
            WhiteBitboards.ControlledPositions &= 0b_0ul;
            BlackBitboards.ControlledPositions &= 0b_0ul;
        }

        private static void UpdateSlidingAttacked(int sourceSquare, int piece)
        {
            int startIdx = Piece.IsType(piece, PieceType.Bishop) ? 4 : 0;
            int endIdx = Piece.IsType(piece, PieceType.Rook) ? 4 : 8;

            int[][] numSquaresInDirection = MoveGeneration.numSquaresInDirection;
            for (int directionIdx = startIdx; directionIdx < endIdx; directionIdx++)
            {
                for (int i = 1; i <= numSquaresInDirection[sourceSquare][directionIdx]; i++)
                {
                    int targetSquare = sourceSquare + directionOffsets[directionIdx] * i;
                    int pieceAtTarget = Board[targetSquare];
                    AddPieceToMask((ulong)piece, targetSquare);

                    if (pieceAtTarget != 0)
                    {
                        break;
                    }
                }
            }
        }

        private static void UpdatePawnAttacked(int sourceSquare, int piece)
        {
            int[] attackOffsets = LookUps.pawnAttackOffset[(piece & 8) >> 3, sourceSquare];

            foreach (int offset in attackOffsets)
            {
                int targetSquare = sourceSquare + offset;
                AddPieceToMask((ulong)piece, targetSquare);
            }
        }

        private static void UpdateKnightAttacked(int sourceSquare, int piece)
        {
            int[] offsets = LookUps.knightOffset[sourceSquare];

            for (int i = 0; i < offsets.Length; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                AddPieceToMask((ulong)piece, targetSquare);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void AddPieceToMask(ulong piece, int targetSquare)
        {
            ulong whiteMask = ((piece & 8) >> 3) << targetSquare;
            WhiteBitboards.ControlledPositions |= whiteMask;
            ulong blackMask = ((piece ^ 8) >> 3) << targetSquare;
            BlackBitboards.ControlledPositions |= blackMask;
        }

        private static void UpdateKingAttacked(int sourceSquare, int piece)
        {
            int[] offsets = LookUps.kingOffset[sourceSquare];

            for (int i = 0; i < offsets.Length; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                /*if (targetSquare < 0 || targetSquare > 63)
                {
                    continue;
                }*/
                AddPieceToMask((ulong)piece, targetSquare);
            }
        }

        public static List<int> FindCheckingPieces(int kingPosition, int king)
        {
            List<int> checkingPieces = new();

            List<int> slidingPieces = FindCheckingSlidingPiece(kingPosition, king, Board);
            foreach (int piece in slidingPieces)
            {
                checkingPieces.Add(piece);
            }

            List<int> knights = FindCheckingKnight(kingPosition, king, Board);
            foreach (int piece in knights)
            {
                checkingPieces.Add(piece);
            }

            List<int> pawns = FindCheckingPawn(kingPosition, king, Board);
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
                int pieceAtTarget = Board[targetSquare];

                if (pieceAtTarget == 0) { continue; }
                if (Piece.IsSameColour(sideColour, pieceAtTarget)) return false;
                if (!Piece.IsSlidingPiece(pieceAtTarget)) return false;

                var type = direction < 4 ? PieceType.Rook : PieceType.Bishop;
                return Piece.IsType(pieceAtTarget, type) || Piece.IsType(pieceAtTarget, PieceType.Queen);
            }

            return false;
        }

        private static List<int> FindCheckingSlidingPiece(int sourceSquare, int piece, int[] board)
        {
            List<int> pieces = new();

            for (int directionIdx = 0; directionIdx < 8; directionIdx++)
            {
                int maxOffset = MoveGeneration.numSquaresInDirection[sourceSquare][directionIdx];
                for (int i = 0; i < maxOffset; i++)
                {
                    int targetSquare = sourceSquare + (i + 1) * directionOffsets[directionIdx];
                    int pieceAtTarget = board[targetSquare];

                    if (pieceAtTarget == 0) { continue; }
                    if (Piece.IsSameColour(piece, pieceAtTarget)) break;

                    var type = directionIdx < 4 ? PieceType.Bishop : PieceType.Rook;
                    if (Piece.IsSlidingPiece(pieceAtTarget) && !Piece.IsType(pieceAtTarget, type))
                    {
                        pieces.Add(targetSquare);
                        break;
                    }
                    break;
                }
            }

            return pieces;
        }

        private static List<int> FindCheckingKnight(int sourceSquare, int piece, int[] board)
        {
            List<int> offsets = new() { 15, 17, 10, -6, -15, -17, -10, 6 };
            removeInvalidOffset();
            List<int> pieces = new();

            for (int i = 0; i < offsets.Count; i++)
            {
                int targetSquare = sourceSquare + offsets[i];
                if (targetSquare < 0 || targetSquare > 63) continue;

                int targetPiece = board[targetSquare];
                if (Piece.IsSameColour(piece, targetPiece)) continue;

                if (Piece.IsType(targetPiece, PieceType.Knight)) pieces.Add(targetSquare);
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
            List<int> pieces = new();

            if (opponentIsWhite && rank <= 1 || !opponentIsWhite && rank >= 6) return pieces;

            int[] attackOffsets = file switch
            {
                0 when opponentIsWhite => new[] { -7 },
                0 => new[] { 9 },
                7 when opponentIsWhite => new[] { -9 },
                7 => new[] { 7 },
                _ when opponentIsWhite => new[] { -9, -7 },
                _ => new[] { 9, 7 },
            };

            for (int i = 0; i < attackOffsets.Length; i++)
            {
                int targetSquare = sourceSquare + attackOffsets[i];
                int targetPiece = board[targetSquare];
                if (targetPiece == 0) continue;
                if (Piece.IsSameColour(piece, targetPiece)) continue;
                if (Piece.IsType(targetPiece, PieceType.Pawn)) pieces.Add(targetSquare);
            }

            return pieces;
        }

        static string[] letterLookup = { "a", "b", "c", "d", "e", "f", "g", "h" };
        public static string GetCurrentFen()
        {
            var sb = new StringBuilder();
            int emptySpace = 0;
            for (int i = 0; i < 64; i++)
            {
                int contents = Board[i];
                if (contents == 0)
                {
                    emptySpace++;
                }
                else
                {
                    sb.Append(Piece.GetPieceCharacterRepresentation(contents));
                }

                if((i + 1) % 8 == 0)
                {
                    if(emptySpace > 0) sb.Append(emptySpace);
                    emptySpace = 0;
                    sb.Append('/');
                }
            }

            sb.Append(WhiteToMove? " w" : " b");

            sb.Append(" ");
            if (CastleingRights.HasFlag(CastlingRights.WhiteKingSide)) sb.Append('K');
            if(CastleingRights.HasFlag(CastlingRights.WhiteQueenSide)) sb.Append('Q');
            if(CastleingRights.HasFlag(CastlingRights.BlackKingSide)) sb.Append('k');
            if(CastleingRights.HasFlag(CastlingRights.BlackQueenSide)) sb.Append('q');

            if(EnPesantSquare >= 0) {
                int x = EnPesantSquare % 8;
                int y = EnPesantSquare / 8 + 1;
                sb.Append(" "+ letterLookup[x] + y.ToString());
            }
            else
            {
                sb.Append(" -");
            }

            sb.Append($" {HalfMoves % 2}");
            sb.Append($" {FullMoves}");

            return sb.ToString();
        }
    }
}
