using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public struct Move
    {
        public int sourceSquare;
        public int targetSquare;
        public MoveType moveType;
        public PromotionType promotionType;
        public bool promotionCapture;
        public enum MoveType
        {
            move,
            enPesant,
            doublePawnMove,
            capture,
            castle,
            promotion
        }

        public enum PromotionType
        {
            queen = 0b_0000_0001,
            rook = 0b_0000_0010,
            bishop = 0b_0000_0100,
            knight = 0b_0000_1000,
        }

        public bool IsPromotionType(PromotionType type)
        {
            return (this.promotionType & type) == type;
        }

        public Piece.PieceType GetPromotionPiece()
        {
            Piece.PieceType type = Piece.PieceType.Queen;
            if (this.IsPromotionType(Move.PromotionType.queen))
            {
                type = Piece.PieceType.Queen;
            }
            else if (this.IsPromotionType(Move.PromotionType.knight))
            {
                type = Piece.PieceType.Knight;
            }
            else if (this.IsPromotionType(Move.PromotionType.rook))
            {
                type = Piece.PieceType.Rook;
            }
            else if (this.IsPromotionType(Move.PromotionType.bishop))
            {
                type = Piece.PieceType.Bishop;
            }
            return type;
        }

        public Move(int sourceSquare, int targetSquare) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveType = MoveType.move;
        }

        public Move(int sourceSquare, int targetSquare, MoveType moveType) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveType = moveType;
        }
        public Move(int sourceSquare, int targetSquare, MoveType moveType, PromotionType promotionType, bool isCapture) : this()
        {
            this.sourceSquare = sourceSquare;
            this.targetSquare = targetSquare;
            this.moveType = moveType;
            this.promotionType = promotionType;
            this.promotionCapture = isCapture;
        }

        public bool IsPromotion()
        {
            if(this.moveType == MoveType.move || this.moveType == MoveType.enPesant ||  
                this.moveType == MoveType.doublePawnMove || this.moveType == MoveType.capture || this.moveType == MoveType.castle)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            int x = this.sourceSquare % 8;
            int y = this.sourceSquare / 8 + 1;
            string startPos = ToLetter(x) + y.ToString();

            x = this.targetSquare % 8;
            y = this.targetSquare / 8 + 1;
            string endPos = ToLetter(x) + y.ToString();

            string finalString = startPos + endPos;
            if(this.moveType == MoveType.promotion)
            {
                if (this.IsPromotionType(PromotionType.bishop))
                {
                    finalString += "b";
                }
                else if (this.IsPromotionType(PromotionType.rook))
                {
                    finalString += "r";
                }
                else if (this.IsPromotionType(PromotionType.knight))
                {
                finalString += "n";
                }
                else if (this.IsPromotionType(PromotionType.queen))
                {
                finalString += "q";
                }
            }
            return finalString;

        }

        private string ToLetter(int xVal)
        {
            switch (xVal)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                case 7:
                    return "h";
            }
            return "x";
        }
    }
}
