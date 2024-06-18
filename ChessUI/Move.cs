using ChessUI.Enums;

namespace ChessUI
{
    public record struct Move
    {
        public int sourceSquare;
        public int targetSquare;
        public int moveFlag;

        public Move(int sourceSquare, int targetSquare) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveFlag = 0;
        }

        public Move(int sourceSquare, int targetSquare, MoveType moveType) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveFlag = (int)moveType;
        }
        public Move(int sourceSquare, int targetSquare, MoveType moveType, PromotionPiece piece) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveFlag = (int)moveType | (int)piece;
        }

        public PromotionPiece GetPromotionType() => (PromotionPiece)(this.moveFlag & 0b_0011_1110);

        public bool IsType(MoveType type)
        {
            return (this.moveFlag & (int)type) == (int)type;
        }
        public MoveType GetMoveType()
        {
            return (MoveType)(this.moveFlag & 0b_1110_0011);
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

        public bool IsPromotion() => (this.moveFlag & (int)MoveType.promotion) == (int)MoveType.promotion;

        public override string ToString()
        {
            string[] letterLookup = { "a", "b", "c", "d", "e", "f", "g", "h" };
            int x = this.sourceSquare % 8;
            int y = this.sourceSquare / 8 + 1;
            string startPos = letterLookup[x] + y.ToString();

            x = this.targetSquare % 8;
            y = this.targetSquare / 8 + 1;
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
                };
            }
            return finalString;
        }
    }
}
