using ChessUI.Enums;
using ChessUI.Time_Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class GameSetup
    {
        public static TimeControlOptions TimeControl { get; set; } = new();
        public static Colour PlayerColour { get; set; } = Colour.White;
        public static MoveSelectionType AiMoveSelection { get; set; } = MoveSelectionType.ItterativeDeepening;
    }
}
