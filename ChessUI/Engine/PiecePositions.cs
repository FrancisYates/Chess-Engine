
using ChessUI.Enums;
using System.Collections.Generic;

namespace ChessUI.Engine
{
    public class PiecePositions
    {
        public HashSet<int> Pawns { get; set; } = [];
        public HashSet<int> Rooks { get; set; } = [];
        public HashSet<int> Knights { get; set; } = [];
        public HashSet<int> Bishops { get; set; } = [];
        public HashSet<int> Queens { get; set; } = [];
        public int King { get; set; }

        public void Remove(PieceType type, int position)
        {
            switch (type) {

                case PieceType.Pawn:
                    Pawns.Remove(position);
                    break;
                case PieceType.Rook:
                    Rooks.Remove(position);
                    break;
                case PieceType.Knight:
                    Knights.Remove(position);
                    break;
                case PieceType.Bishop:
                    Bishops.Remove(position);
                    break;
                case PieceType.Queen:
                    Queens.Remove(position);
                    break;
                case PieceType.King:
                    King = -1;
                    break;
            }
        }
        public void Add(PieceType type, int position)
        {
            switch (type) {

                case PieceType.Pawn:
                    Pawns.Add(position);
                    break;
                case PieceType.Rook:
                    Rooks.Add(position);
                    break;
                case PieceType.Knight:
                    Knights.Add(position);
                    break;
                case PieceType.Bishop:
                    Bishops.Add(position);
                    break;
                case PieceType.Queen:
                    Queens.Add(position);
                    break;
                case PieceType.King:
                    King = position;
                    break;
            }
        }
    }
}
