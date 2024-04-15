using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI.Engine
{
    public class Bitboards
    {
        public ulong Pawns;
        public ulong Rooks;
        public ulong Knights;
        public ulong Bishops;
        public ulong Queens;
        public ulong Kings;
        public ulong ControlledPositions;
    }
}
