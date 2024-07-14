using ChessUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineH2H
{
    internal class GameState
    {
        public List<(bool, Move)> Moves { get; set; } = [];
        public List<string> Boards { get; set; } = [];
    }
}
