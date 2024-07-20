using ChessUI;
using ChessUI.Engine;
using ChessUI.Enums;
using ChessUI.UCI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChessEngineH2H
{
    public class EngineManager
    {
        Process white;
        Process black;
        UCIComandInterpreter whiteCmd;
        UCIComandInterpreter blackCmd;
        EngineOptions _options = new();
        AIPlayer whitePlayer = new(MoveSelectionType.Random, true);
        AIPlayer blackPlayer = new(MoveSelectionType.Minimax, false) { MaxSearchDepth = 3 };
        GameInfo gameInfo;
        string gameId;
        int whiteMaxDepth = 8;
        int blackMaxDepth = 8;
        bool whiteToMove = true;
        Process StartProcess(string enginePath)
        {
            Process process = new();
            process.StartInfo.FileName = enginePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            return process;
        }
        internal void StartGame(EngineOptions options)
        {
            _options = options;
            if (white is not null) white.Kill();
            if(black is not null) black.Kill();
            whitePlayer.ThinkTimeMs = options.ThinkTimeMs;
            blackPlayer.ThinkTimeMs = options.ThinkTimeMs;
            white = StartProcess(options.WhiteEnginePath);
            black = StartProcess(options.BlackEnginePath);
            //whiteCmd = new(new() { MaxSearchDepth = 3});
            //blackCmd = new(new() { MaxSearchDepth = 3 });

            //whiteCmd.ReceiveCommand($"setoption search-type {(int)options.WhiteSelectionType}");
            //blackCmd.ReceiveCommand($"setoption search-type {(int)options.BlackSelectionType}");
            //whiteCmd.ReceiveCommand($"position startpos");
            //blackCmd.ReceiveCommand($"position startpos");
            white.StandardInput.WriteLine($"setoption search-type {(int) options.WhiteSelectionType}");
            black.StandardInput.WriteLine($"setoption search-type {(int) options.BlackSelectionType}");
            white.StandardInput.WriteLine("position startpos");
            black.StandardInput.WriteLine("position startpos");
            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFile("startPosition.txt");

            gameId = DateTime.UtcNow.ToFileTimeUtc().ToString();
            gameInfo = new()
            {
                Id = gameId,
                FullMoves = BoardManager.FullMoves,
                HalfMoves = BoardManager.HalfMoves,
                InProgress = true
            };
        }

        public async Task<GameInfo> NextPly()
        {
            string move;
            const int moveTimeMs = 1000;

            string fen = BoardManager.GetCurrentFen();
            if (BoardManager.WhiteToMove)
            {
                string bmove = await FindMove(white, fen, _options.ThinkTimeMs, whiteMaxDepth);
                move = bmove.Split(' ')[1];
            }
            else
            {
                string bmove = await FindMove(black, fen, _options.ThinkTimeMs, blackMaxDepth);
                move = bmove.Split(' ')[1];
            }
            gameInfo.FullMoves = BoardManager.FullMoves;
            if (move == "0000")
            {
                gameInfo.Outcome = BoardManager.WhiteToMove ? GameOutcome.BlackWin : GameOutcome.WhiteWin;
                gameInfo.InProgress = false;
                return gameInfo;
            }
            if (gameInfo.FullMoves > 100)
            {
                gameInfo.InProgress = false;
                gameInfo.Outcome = GameOutcome.Draw;
                return gameInfo;
            }

            Move m = Move.Parse(move);
            gameInfo.BoardStates.Add(fen);
            gameInfo.Moves.Add(m);
            Debug.WriteLine(m.ToString());
            MoveManager.MakeMove(m, BoardManager.Board);
            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            //await SaveGameState(gameId, gameInfo);
            whiteToMove = !whiteToMove;
            return gameInfo;
        }

        public async Task SaveGameState(string gameId, GameInfo gameInfo)
        {
            if (!Directory.Exists("Games")) Directory.CreateDirectory("Games");
            string fileName = $"Games/{gameId}.txt";
            string json = JsonSerializer.Serialize(gameInfo);
            await File.WriteAllTextAsync(fileName, json);
        }

        private async Task<string> GetCurrentBoard(Process process)
        {
            string fen = "";
            process.StandardInput.WriteLine("d");
            fen = process.StandardOutput.ReadLine();
            while (fen is null)
            {
                await Task.Delay(20);
                fen = process.StandardOutput.ReadLine();
            }
            return fen;
        }
        private async Task<string> FindMove(Process process, string fen, int moveTimeMs, int maxDepth)
        {
            process.StandardInput.WriteLine($"position fen {fen}");
            process.StandardInput.WriteLine($"go depth {maxDepth} movetime {moveTimeMs}");
            //await Task.Delay(moveTimeMs);
            string move = process.StandardOutput.ReadLine();
            while (move is null)
            {
                await Task.Delay(20);
                move = process.StandardOutput.ReadLine();
            }
            return move;
        }
        private async Task<string> FindMove(UCIComandInterpreter UciCmd, string fen, int moveTimeMs, int maxDepth)
        {
            UciCmd.ReceiveCommand($"position fen {fen}");
            string move = UciCmd.ProcessGoCommand($"go depth {maxDepth} movetime {moveTimeMs}");
            return "bestmove " + move;
        }
    }

    internal class EngineOptions
    {
        public string WhiteEnginePath { get; set; }
        public string BlackEnginePath { get; set; }
        public MoveSelectionType WhiteSelectionType { get; set; }
        public MoveSelectionType BlackSelectionType { get; set; }
        public int ThinkTimeMs { get; set; }
        public int NumGames { get; set; }
    }

}
