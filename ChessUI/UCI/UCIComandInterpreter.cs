using ChessUI.Engine;
using System;
using System.Numerics;

namespace ChessUI.UCI
{
    public class UCIComandInterpreter
    {
        static readonly string[] positionLabels = new[] { "position", "fen", "moves" };
        static readonly string[] goLabels = new[] { "go", "movetime", "wtime", "btime", "winc", "binc", "movestogo", "depth" };

        private readonly ThinkTimeCalculator _timeCalculator = new();
        readonly AIPlayer player;

        public Action<string> OnResponse;

        public UCIComandInterpreter()
        {
            player = new();
            player.OnMoveChosen += OnMoveChosen;
        }
        public UCIComandInterpreter(AIPlayer aiPlayer)
        {
            player = aiPlayer;
            player.OnMoveChosen += OnMoveChosen;
        }

        public void ReceiveCommand(string message)
        {
            //Console.WriteLine(message);
            message = message.Trim();
            string messageType = message.Split(' ')[0].ToLower();

            switch (messageType)
            {
                case "uci":
                    Respond("uciok");
                    break;
                case "isready":
                    Respond("readyok");
                    break;
                case "ucinewgame":
                    player.StartNewGame();
                    break;
                case "position":
                    ProcessPositionCommand(message);
                    break;
                case "go":
                    ProcessGoCommand(message);
                    break;
                case "stop":
                    if (player.IsThinking)
                    {
                        player.Stop();
                    }
                    break;
                case "quit":
                    player.Stop();
                    break;
                case "d":
                    Console.WriteLine(GetFen());
                    break;
                default:
                    throw new NotImplementedException("Unrecognised comand " + message);
            }
        }

        public string GetFen()
        {
            return BoardManager.GetCurrentFen();
        }

        void OnMoveChosen(string move)
        {
            Respond("bestmove " + move);
        }

        public string ProcessGoCommand(string message)
        {
            if (message.Contains("depth"))
            {
                int depth = TryGetLabelledValueInt(message, "depth", goLabels, 0);
                player.MaxSearchDepth = depth;

            }
            if (message.Contains("movetime"))
            {
                int moveTimeMs = TryGetLabelledValueInt(message, "movetime", goLabels, 0);
                player.ThinkTimeMs = moveTimeMs;
            }
            else
            {
                int timeRemainingWhiteMs = TryGetLabelledValueInt(message, "wtime", goLabels, 0);
                int timeRemainingBlackMs = TryGetLabelledValueInt(message, "btime", goLabels, 0);
                int incrementWhiteMs = TryGetLabelledValueInt(message, "winc", goLabels, 0);
                int incrementBlackMs = TryGetLabelledValueInt(message, "binc", goLabels, 0);

                _timeCalculator.WhiteTimeRemaining = timeRemainingWhiteMs;
                _timeCalculator.WhiteIncrement = incrementWhiteMs;
                _timeCalculator.BlackTimeRemaining = timeRemainingBlackMs;
                _timeCalculator.BlackIncrement = incrementBlackMs;

                int thinkTime = _timeCalculator.GetThinkTimeMs(BoardManager.WhiteToMove, BoardManager.FullMoves);
                player.ThinkTimeMs = thinkTime;
            }

            return player.MakeMove().ToString() ?? "0000";
        }
        public void ProcessPositionCommand(string message)
        {
            // FEN
            if (message.ToLower().Contains("startpos"))
            {
                BoardManager.ResetBoardToEmpty();
                BoardManager.LoadBoardFromFile("startPosition.txt");
            }
            else if (message.ToLower().Contains("fen"))
            {
                string customFen = TryGetLabelledValue(message, "fen", positionLabels);
                BoardManager.ResetBoardToEmpty();
                BoardManager.LoadBoardFromFen(customFen);
            }
            else
            {
                Console.WriteLine("Invalid position command (expected 'startpos' or 'fen')");
            }
        }

        private void Respond(string message)
        {
            Console.WriteLine($"{message}");
            OnResponse?.Invoke(message);
        }
        static int TryGetLabelledValueInt(string text, string label, string[] allLabels, int defaultValue = 0)
        {
            string valueString = TryGetLabelledValue(text, label, allLabels, defaultValue + "");
            if (int.TryParse(valueString.Split(' ')[0], out int result))
            {
                return result;
            }
            return defaultValue;
        }
        static string TryGetLabelledValue(string text, string label, string[] allLabels, string defaultValue = "")
        {
            text = text.Trim();
            if (text.Contains(label))
            {
                int valueStart = text.IndexOf(label) + label.Length;
                int valueEnd = text.Length;
                foreach (string otherID in allLabels)
                {
                    if (otherID != label && text.Contains(otherID))
                    {
                        int otherIDStartIndex = text.IndexOf(otherID);
                        if (otherIDStartIndex > valueStart && otherIDStartIndex < valueEnd)
                        {
                            valueEnd = otherIDStartIndex;
                        }
                    }
                }

                return text.Substring(valueStart, valueEnd - valueStart).Trim();
            }
            return defaultValue;
        }
    }
}
