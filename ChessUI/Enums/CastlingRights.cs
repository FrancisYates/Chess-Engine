using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI.Enums
{
    [Flags]
    public enum CastlingRights
    {
        BlackQueenSide = 0b_1,
        BlackKingSide = 0b_1 << 1,
        WhiteQueenSide = 0b_1 << 2,
        WhiteKingSide = 0b_1 << 3,
    }
}
