using ChessUI;
using ChessUI.Engine;
using ChessUI.Time_Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace xUnitTests_Chess
{
    public class EvaluationPerfTests
    {
        private readonly ITestOutputHelper output;

        public EvaluationPerfTests(ITestOutputHelper output) {
            this.output = output;
        }

        private readonly ThinkTimeCalculator timeControl = new(new TimeControlOptions {
            WhiteInitialTimeMs = int.MaxValue,
            BlackInitialTimeMs = int.MaxValue,
        },
            int.MaxValue);
        private string PositionDirectory => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Positions\\"));
        private string BoardDirectory => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\TestBoards\\"));
        [Theory]
        [InlineData(true, "start.txt", 3)]
        [InlineData(true, "position2.txt", 3)]
        [InlineData(true, "position3.txt", 3)]
        [InlineData(true, "position4.txt", 3)]
        [InlineData(true, "position5.txt", 3)]
        [InlineData(true, "position6.txt", 3)]
        [InlineData(true, "start.txt", 4)]
        [InlineData(true, "position2.txt", 4)]
        [InlineData(true, "position3.txt", 4)]
        [InlineData(true, "position4.txt", 4)]
        [InlineData(true, "position5.txt", 4)]
        [InlineData(true, "position6.txt", 4)]
        public void CreatePositionFile(bool isWhite, string boardFile, int maxPly) {
            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFile(BoardDirectory + boardFile);
            MoveGeneration.CalculateDirections();
            BoardManager.UpdateAttackedPositions();

            AIPlayer aiPlayer = new(timeControl);
            var sw = Stopwatch.StartNew();
            List<string> positions = aiPlayer.FindReachablePositions(1, maxPly, isWhite);
            sw.Stop();
            Debug.WriteLine($"Found {positions.Count} positions in {sw.ElapsedMilliseconds}ms");

            var distinct = positions.Distinct();
            Debug.WriteLine($"{distinct.Count()} distinct positions of {positions.Count} total positions");

            string csv = String.Join(",", distinct.ToArray());
            string fileName = boardFile.Split(".")[0] + $"-{maxPly}ply.csv";
            File.WriteAllText(PositionDirectory + fileName, csv);
        }
        [Theory]
        [InlineData("start-3ply.csv")]
        [InlineData("position2-3ply.csv")]
        [InlineData("position3-3ply.csv")]
        [InlineData("position4-3ply.csv")]
        [InlineData("position5-3ply.csv")]
        [InlineData("position6-3ply.csv")]
        [InlineData("start-4ply.csv")]
        [InlineData("position2-4ply.csv")]
        [InlineData("position3-4ply.csv")]
        [InlineData("position4-4ply.csv")]
        [InlineData("position5-4ply.csv")]
        [InlineData("position6-4ply.csv")]
        public void TestEvaluationSpeed(string positionFile) {
            var csv = File.ReadAllText(PositionDirectory + positionFile);
            var positions = csv.Split(',');
            output.WriteLine($"{positions.Count()} total position");
            foreach (var position in positions) {
                BoardManager.ResetBoardToEmpty();
                BoardManager.LoadBoardFromFen(position);
                MoveGeneration.CalculateDirections();
                BoardManager.UpdateAttackedPositions();

                var evaluation = MoveEvaluation.EvaluateBoard(BoardManager.Board);
            }
        }
    }    
}
