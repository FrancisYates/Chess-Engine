using ChessUI.Time_Control;
using System;

namespace ChessUI
{
    public class ThinkTimeCalculator
    {
        public bool UseMaxThinkTime { get; set; } = false;
        public bool UseFixedThinkTime { get; set; } = false;
        public int FixedThinkTime { get; set; }
        public int WhiteTimeRemaining { get; set; }
        public int BlackTimeRemaining { get; set; }
        public int WhiteIncrement { get; set; }
        public int BlackIncrement { get; set; }
        public int MaxThinkTime { get; init; }
        public ThinkTimeCalculator(TimeControlOptions options, int? maxThinkTime = null)
        {
            UseMaxThinkTime = maxThinkTime is not null;
            MaxThinkTime = maxThinkTime ?? 0;
            WhiteTimeRemaining = options.WhiteInitialTimeMs;
            BlackTimeRemaining = options.BlackInitialTimeMs;
            WhiteIncrement = options.WhiteIncrementMs;
            WhiteIncrement = options.BlackIncrementMs;
        }
        public ThinkTimeCalculator(int fixedThinkTime)
        {
            UseFixedThinkTime = true;
            FixedThinkTime = fixedThinkTime;
        }
        public int GetThinkTimeMs()
        {
            if (UseFixedThinkTime) return FixedThinkTime;
            int time = BoardManager.WhiteToMove ? WhiteTimeRemaining : BlackTimeRemaining;
            int increment = BoardManager.WhiteToMove ? WhiteIncrement : BlackIncrement;


            int thinkTime;

            if(time < increment)
            {
                thinkTime = (int)Math.Floor(increment * 0.75);
            }
            else
            {
                thinkTime = time / (10 + Math.Max(30 - BoardManager.FullMoves, 0));
                thinkTime += (int)Math.Floor(increment * 0.2);
            }

            if(UseMaxThinkTime)
            {
                thinkTime = Math.Max(MaxThinkTime, thinkTime);
            }
            return thinkTime;
        }

        public void IncrementTime()
        {
            if (BoardManager.WhiteToMove)
            {
                WhiteTimeRemaining += WhiteIncrement;
            }
            else
            {
                BlackTimeRemaining += BlackIncrement;
            }
        }
    }
}
