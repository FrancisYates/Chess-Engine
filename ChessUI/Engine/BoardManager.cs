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

        public static Bitboards WhiteBitboards { get; set; } = new() { 
            PawnHomeRank = 0b1111111100000000,
            FinalRank = 0b1111111100000000000000000000000000000000000000000000000000000000
        };
        public static Bitboards BlackBitboards { get; set; } = new()
        {
            PawnHomeRank = 0b11111111000000000000000000000000000000000000000000000000,
            FinalRank = 0b11111111
        };

        public static PiecePositions WhitePiecePositions { get; set; } = new();
        public static PiecePositions BlackPiecePositions { get; set; } = new();

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
                    continue;
                }
                if (char.IsDigit(asciiValue))
                {
                    file += (int)char.GetNumericValue(asciiValue);
                    continue;
                }

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
                index = 8 * rank + file;
                Board[index] = pieceNumber;

                Bitboards bitboards = Piece.IsPieceWhite(pieceNumber)? WhiteBitboards : BlackBitboards;
                var pieceType = Piece.GetPieceType(pieceNumber);
                ref ulong bitboard = ref GetBitboard(ref bitboards, pieceType);
                bitboard |= 0b_1uL << index;
                bitboards.AllPieces |= 0b_1uL << index;

                AddPositionToPieceList(index, pieceType, Piece.IsPieceWhite(pieceNumber));

                file++;
            }
        }

        private static void AddPositionToPieceList(int position, PieceType type, bool isWhite)
        {
            PiecePositions positions = isWhite ? WhitePiecePositions : BlackPiecePositions;
            switch (type)
            {
                case PieceType.Pawn:
                    positions.Pawns.Add(position); 
                    break;
                case PieceType.Rook:
                    positions.Rooks.Add(position); 
                    break;
                case PieceType.Knight:
                    positions.Knights.Add(position); 
                    break;
                case PieceType.Bishop:
                    positions.Bishops.Add(position); 
                    break;
                case PieceType.Queen:
                    positions.Queens.Add(position); 
                    break;
                case PieceType.King:
                    positions.King = position; 
                    break;
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

            WhitePiecePositions = new();
            BlackPiecePositions = new();
        }

        private static void SetupCastleRights(string castleRightsString)
        {
            CastleingRights = 0;
            if (castleRightsString.Contains('K'))
            {
                CastleingRights |= CastlingRights.WhiteKingSide;
            }
            if (castleRightsString.Contains('Q'))
            {
                CastleingRights |= CastlingRights.WhiteQueenSide;
            }
            if (castleRightsString.Contains('k'))
            {
                CastleingRights |= CastlingRights.BlackKingSide;
            }
            if (castleRightsString.Contains('q'))
            {
                CastleingRights |= CastlingRights.BlackQueenSide;
            }
        }

        public static void UpdateSideToMove()
        {
            WhiteToMove = !WhiteToMove;
        }

        public static void UpdateAttackedPositions()
        {
            ResetAttackedBitBoards();

            UpdatePawnAttacked();

            UpdateKnightAttacked();

            UpdateKingAttacked();
            
            UpdateRookAttacked();

            UpdateBishopAttacked();
        }

        private static void ResetAttackedBitBoards()
        {
            WhiteBitboards.ControlledPositions &= 0b_0ul;
            BlackBitboards.ControlledPositions &= 0b_0ul;
        }

        private static void UpdateRookAttacked()
        {
            ulong potentialBlockers = (WhiteBitboards.AllPieces | BlackBitboards.AllPieces);
            ulong actualBlockers;

            foreach (var position in WhitePiecePositions.Rooks)
            {
                actualBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];
                WhiteBitboards.ControlledPositions |= LookUps.RookMoveDict[(position, actualBlockers)];
            }
            foreach (var position in WhitePiecePositions.Queens)
            {
                actualBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];
                WhiteBitboards.ControlledPositions |= LookUps.RookMoveDict[(position, actualBlockers)];
            }

            foreach (var position in BlackPiecePositions.Rooks)
            {
                actualBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];
                BlackBitboards.ControlledPositions |= LookUps.RookMoveDict[(position, actualBlockers)];
            }
            foreach (var position in BlackPiecePositions.Queens)
            {
                actualBlockers = potentialBlockers & LookUps.rookOccupancyBitboards[position];
                BlackBitboards.ControlledPositions |= LookUps.RookMoveDict[(position, actualBlockers)];
            }
        }

        private static void UpdateBishopAttacked()
        {
            ulong potentialBlockers = (WhiteBitboards.AllPieces | BlackBitboards.AllPieces);
            ulong actualBlockers;
            foreach (var position in WhitePiecePositions.Bishops)
            {
                actualBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];
                WhiteBitboards.ControlledPositions |= LookUps.BishopMoveDict[(position, actualBlockers)]; ;
            }
            foreach (var position in WhitePiecePositions.Queens)
            {
                actualBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];
                WhiteBitboards.ControlledPositions |= LookUps.BishopMoveDict[(position, actualBlockers)];
            }

            foreach (var position in BlackPiecePositions.Bishops)
            {
                actualBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];
                BlackBitboards.ControlledPositions |= LookUps.BishopMoveDict[(position, actualBlockers)]; ;
            }
            foreach (var position in BlackPiecePositions.Queens)
            {
                actualBlockers = potentialBlockers & LookUps.bishopOccupancyBitBoards[position];
                BlackBitboards.ControlledPositions |= LookUps.BishopMoveDict[(position, actualBlockers)]; ;
            }
        }

        private static void UpdatePawnAttacked()
        {
            foreach (var position in WhitePiecePositions.Pawns)
            {
                WhiteBitboards.ControlledPositions |= LookUps.whitePawnAttackBitBoard[position];
            }

            foreach (var position in BlackPiecePositions.Pawns)
            {
                BlackBitboards.ControlledPositions |= LookUps.blackPawnAttackBitBoard[position];
            }
        }

        private static void UpdateKnightAttacked()
        {
            foreach (var position in WhitePiecePositions.Knights)
            {
                WhiteBitboards.ControlledPositions |= LookUps.knightMoveBitboards[position];
            }

            foreach (var position in BlackPiecePositions.Knights)
            {
                BlackBitboards.ControlledPositions |= LookUps.knightMoveBitboards[position];
            }
        }

        private static void UpdateKingAttacked()
        {
            WhiteBitboards.ControlledPositions |= LookUps.kingMoveBitboards[WhitePiecePositions.King];

            BlackBitboards.ControlledPositions |= LookUps.kingMoveBitboards[BlackPiecePositions.King];
        }

        public static string GetCurrentFen()
        {
            string[] letterLookup = { "a", "b", "c", "d", "e", "f", "g", "h" };
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

            sb.Append(' ');
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
