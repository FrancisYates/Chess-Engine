using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public enum MoveType
    {
        move =              0b_0000_0000,
        enPesant =          0b_0000_0001,
        doublePawnMove =    0b_0000_0010,
        capture =           0b_1000_0000,
        castle =            0b_0100_0000,
        promotion =         0b_0010_0000
    }

    public enum PromotionPiece
    {
        queen =             0b_0011_0000,
        rook =              0b_0010_1000,
        bishop =            0b_0010_0100,
        knight =            0b_0010_0010,
    }

    public struct Move
    {
        public int sourceSquare;
        public int targetSquare;
        public int moveFlag;

        public bool IsPromotionType(PromotionPiece piece)
        {
            return (this.moveFlag & (int)piece) == (int)piece;
        }

        public bool IsType(MoveType type)
        {
            return (this.moveFlag & (int)type) == (int)type;
        }
        public Piece.PieceType GetPromotionPiece()
        {
            Piece.PieceType type = Piece.PieceType.Queen;
            if (this.IsPromotionType(PromotionPiece.queen))
            {
                type = Piece.PieceType.Queen;
            }
            else if (this.IsPromotionType(PromotionPiece.knight))
            {
                type = Piece.PieceType.Knight;
            }
            else if (this.IsPromotionType(PromotionPiece.rook))
            {
                type = Piece.PieceType.Rook;
            }
            else if (this.IsPromotionType(PromotionPiece.bishop))
            {
                type = Piece.PieceType.Bishop;
            }
            return type;
        }

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

        public bool IsPromotion()
        {
            return (this.moveFlag & (int)MoveType.promotion) == (int)MoveType.promotion;
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
            if(this.IsType(MoveType.promotion))
            {
                if (this.IsPromotionType(PromotionPiece.bishop))
                {
                    finalString += "b";
                }
                else if (this.IsPromotionType(PromotionPiece.rook))
                {
                    finalString += "r";
                }
                else if (this.IsPromotionType(PromotionPiece.knight))
                {
                finalString += "n";
                }
                else if (this.IsPromotionType(PromotionPiece.queen))
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
