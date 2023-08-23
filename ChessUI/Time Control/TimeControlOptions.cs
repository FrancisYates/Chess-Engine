namespace ChessUI.Time_Control
{
    public class TimeControlOptions
    {
        public int WhiteInitialTimeMs { get; set; } = 300_000;
        public int BlackInitialTimeMs { get; set; } = 300_000;
        public int WhiteIncrementMs { get; set; } = 5_000;
        public int BlackIncrementMs { get; set; } = 5_000;
    }
}
