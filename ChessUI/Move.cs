using ChessUI.Engine;
using ChessUI.Enums;
using System;

namespace ChessUI
{
    public record Move
    {
        public int SourceSquare { get; set; }
        public int TargetSquare {get; set; }
        public int MoveFlag {get; set; }

        public Move()
        {

        }
        public Move(int sourceSquare, int targetSquare) : this()
        {
            this.SourceSquare = sourceSquare;
            this.TargetSquare = targetSquare;
            this.MoveFlag = 0;
        }

        public Move(int sourceSquare, int targetSquare, MoveType moveType) : this()
        {
            this.SourceSquare = sourceSquare;
            this.TargetSquare = targetSquare;
            this.MoveFlag = (int)moveType;
        }
        public Move(int sourceSquare, int targetSquare, MoveType moveType, PromotionPiece piece) : this()
        {
            this.SourceSquare = sourceSquare;
            this.TargetSquare = targetSquare;
            this.MoveFlag = (int)moveType | (int)piece;
        }

        public PromotionPiece GetPromotionType() => (PromotionPiece)(this.MoveFlag & 0b_0011_1110);

        public bool IsType(MoveType type)
        {
            return (this.MoveFlag & (int)type) == (int)type;
        }
        public MoveType GetMoveType()
        {
            return (MoveType)(this.MoveFlag & 0b_1110_0011);
        }

        public PieceType GetPromotionPiece()
        {
            return this.GetPromotionType() switch
            {
                PromotionPiece.queen => PieceType.Queen,
                PromotionPiece.knight => PieceType.Knight,
                PromotionPiece.bishop => PieceType.Bishop,
                PromotionPiece.rook => PieceType.Rook,
                _ => PieceType.Rook
            };
        }

        public bool IsPromotion() => (this.MoveFlag & (int)MoveType.promotion) == (int)MoveType.promotion;

        public override string ToString()
        {
            string[] letterLookup = { "a", "b", "c", "d", "e", "f", "g", "h" };
            int x = this.SourceSquare % 8;
            int y = this.SourceSquare / 8 + 1;
            string startPos = letterLookup[x] + y.ToString();

            x = this.TargetSquare % 8;
            y = this.TargetSquare / 8 + 1;
            string endPos = letterLookup[x] + y.ToString();

            string finalString = startPos + endPos;
            if( this.IsPromotion() )
            {
                finalString += this.GetPromotionType() switch
                {
                    PromotionPiece.queen => "q",
                    PromotionPiece.knight => "n",
                    PromotionPiece.rook => "r",
                    PromotionPiece.bishop => "b",
                    _ => throw new System.Exception()
                };
            }
            return finalString;
        }
        public static Move Parse(string moveString)
        {
            char[] chars = moveString.ToCharArray();
            char[] letterLookup = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

            int index = 0;
            for( int i = 0; i< letterLookup.Length; i++)
            {
                if (chars[0] == letterLookup[i])
                {
                    index = i;
                    break;
                }
            }
            int sourceSquare = (chars[1] - '1') * 8 + index;

            for (int i = 0; i < letterLookup.Length; i++)
            {
                if (chars[2] == letterLookup[i])
                {
                    index = i;
                    break;
                }
            }
            int targetSquare = (chars[3] - '1') * 8 + index;
            MoveType moveType = MoveType.move;
            if (Piece.IsType(BoardManager.Board[sourceSquare], PieceType.Pawn) &&
                Math.Abs(sourceSquare - targetSquare) == 16)
            {
                moveType = MoveType.doublePawnMove;
            }else if (targetSquare == BoardManager.EnPesantSquare)
            {
                moveType = MoveType.enPesant | MoveType.capture;
            }else if (BoardManager.Board[targetSquare] != 0)
            {
                moveType = MoveType.capture;
            }

            Move move = new(sourceSquare, targetSquare, moveType);

            if (chars.Length > 4)
            {
                PromotionPiece promotionPiece = chars[4] switch
                {
                    'q' => PromotionPiece.queen,
                    'n' => PromotionPiece.knight,
                    'r' => PromotionPiece.rook,
                    'b' => PromotionPiece.bishop,
                    _ => throw new System.NotImplementedException()
                };
                moveType = MoveType.promotion;
                if (BoardManager.Board[targetSquare] != 0)
                {
                    moveType = MoveType.promotion | MoveType.capture;
                }
                move = new Move(sourceSquare, targetSquare, moveType, promotionPiece);
            }

            return move;
        }
    }
}
