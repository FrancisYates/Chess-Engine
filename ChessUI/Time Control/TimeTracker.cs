using ChessUI.Enums;
using System;
using System.Diagnostics;

namespace ChessUI.Time_Control
{
    public class TimeTracker
    {
        private (bool isWhite, Stopwatch sw)? _timer;
        public TimeSpan WhiteTime { get; set; }
        public TimeSpan BlackTime { get; set; }
        public TimeSpan WhiteInterval { get; set; }
        public TimeSpan BlackInterval { get; set; }

        public delegate void TimeoutEventHandler(object sender, TimeoutEventArgs e);
        public event TimeoutEventHandler Timeout;
        public void Increment(bool ShouldIncrementWhite)
        {
            if (ShouldIncrementWhite)
            {
                BlackTime += BlackInterval;
            }
            else
            {
                WhiteTime += WhiteInterval;
            }
        }

        public void StartTimer(bool shouldStartWhite)
        {
            _timer = (shouldStartWhite, new());
            _timer.Value.sw.Start();
        }
        public TimeSpan StopTimer()
        {
            if (_timer?.sw is null) throw new ArgumentNullException(nameof(_timer.Value.sw));
            _timer.Value.sw.Stop();
            var timerSide = _timer.Value.isWhite ? BlackTime : WhiteTime;
            timerSide -= _timer.Value.sw.Elapsed;
            if (timerSide.TotalMilliseconds <= 0)
            {
                var side = _timer.Value.isWhite ? Colour.White : Colour.Black;
                Timeout.Invoke(this, new(side));
            }
            return _timer.Value.sw.Elapsed;
        }

        public void ResetTimer()
        {
            _timer = null;
        }
    }
}
