using ChessUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineH2H
{
    public class GameInfo
    {
        public bool InProgress { get; set; }
        public GameOutcome Outcome { get; set; } = GameOutcome.None;
        public int HalfMoves { get; set; }
        public int FullMoves { get; set; }
        public string Id{ get; set; }
        public List<Move> Moves { get; set; } = [];
        public List<string> BoardStates { get; set; } = [];
    }

    public enum GameOutcome
    {
        None,
        WhiteWin,
        Draw,
        BlackWin
    }
}
