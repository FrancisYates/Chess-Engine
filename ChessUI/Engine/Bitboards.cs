namespace ChessUI.Engine
{
    public class Bitboards
    {
        public ulong SlidingPieces => Rooks | Bishops | Queens;
        public ulong Pawns;
        public ulong Rooks;
        public ulong Knights;
        public ulong Bishops;
        public ulong Queens;
        public ulong Kings;
        public ulong ControlledPositions;
        public ulong AllPieces;
        public ulong FinalRank;
        public ulong PawnHomeRank;
    }
}
