using ChessUI.Enums;
using System;
using System.Drawing;

namespace ChessUI.Time_Control
{
    public class TimeoutEventArgs : EventArgs
    {
        public Colour TimeoutSide { get; set; }
        public TimeoutEventArgs(Colour side)
        {
            TimeoutSide = side;
        }
    }
}