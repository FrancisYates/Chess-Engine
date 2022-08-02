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
        public enum MoveType
        {
            move,
            enPesant,
            doublePawnMove,
            capture,
            castle,
            promotionKnight,
            promotionRook,
            promotionQueen,
            promotionBishop,
            promotionKnightCapture,
            promotionRookCapture,
            promotionQueenCapture,
            promotionBishopCapture
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
            if (moveType == MoveType.promotionBishop || moveType == MoveType.promotionBishopCapture)
            {
                finalString += "b";
            }
            else if (moveType == MoveType.promotionRook || moveType == MoveType.promotionRookCapture)
            {
                finalString += "r";
            }
            else if (moveType == MoveType.promotionKnight || moveType == MoveType.promotionKnightCapture)
            {
                finalString += "n";
            }
            else if (moveType == MoveType.promotionQueen || moveType == MoveType.promotionQueenCapture)
            {
                finalString += "q";
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
