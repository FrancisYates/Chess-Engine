using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class PieceManager
    {
        public static List<int> whitePiecePositions = new List<int>();
        public static List<int> blackPiecePositions = new List<int>();
        public static int[] kingsPosition = new int[2];

        public static int[][] pawnsPosition = new int[2][];

        public static int GetKingPosition(bool whiteKing)
        {
            int idx = whiteKing ? 0 : 1;
            return kingsPosition[idx];
        }

        public static int[] GetPawnPositions(bool getWhite)
        {
            int idx = getWhite ? 0 : 1;
            return pawnsPosition[idx];
        }
    }
}
