using Xunit;
using ChessUI;
using Xunit.Sdk;
using System;
using Xunit.Abstractions;
using System.Collections.Generic;
using ChessUI.Time_Control;
using ChessUI.Engine;
using System.IO;

namespace xUnitTests_Chess
{
    public class MoveGenerationTests
    {
        private readonly ITestOutputHelper output;

        private string BoardDirectory => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\TestBoards\\"));

        private ThinkTimeCalculator timeControl = new( new TimeControlOptions { 
                WhiteInitialTimeMs = int.MaxValue,
                BlackInitialTimeMs = int.MaxValue,
            },
            int.MaxValue);
        public MoveGenerationTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(true, "start.txt", 1, 20)]
        [InlineData(true, "start.txt", 2, 400)]
        [InlineData(true, "start.txt", 3, 8902)]
        [InlineData(true, "start.txt", 4, 197281)]
        [InlineData(true, "start.txt", 5, 4865609)]
        //[InlineData(true, "start.txt", 6, 119060324)]
        //[InlineData(true, "start.txt", 7, 3195901860)]
        //[InlineData(true, "start.txt", 8, 84998978956)]
        [InlineData(true, "position2.txt", 1, 48)]
        [InlineData(true, "position2.txt", 2, 2039)]
        [InlineData(true, "position2.txt", 3, 97862)]
        [InlineData(true, "position2.txt", 4, 4085603)]
        //[InlineData(true, "position2.txt", 5, 193690690)]
        //[InlineData(true, "position2.txt", 6, 8031647685)]
        [InlineData(true, "position3.txt", 1, 14)]
        [InlineData(true, "position3.txt", 2, 191)]
        [InlineData(true, "position3.txt", 3, 2812)]
        [InlineData(true, "position3.txt", 4, 43238)]
        [InlineData(true, "position3.txt", 5, 674624)]
        //[InlineData(true, "position3.txt", 6, 11030083)]
        //[InlineData(true, "position3.txt", 7, 178633661)]
        [InlineData(true, "position4.txt", 1, 6)]
        [InlineData(true, "position4.txt", 2, 264)]
        [InlineData(true, "position4.txt", 3, 9467)]
        [InlineData(true, "position4.txt", 4, 422333)]
        //[InlineData(true, "position4.txt", 5, 15833292)]
        //[InlineData(true, "position4.txt", 6, 706045033)]
        [InlineData(false, "position4Mirrored.txt", 1, 6)]
        [InlineData(false, "position4Mirrored.txt", 2, 264)]
        [InlineData(false, "position4Mirrored.txt", 3, 9467)]
        [InlineData(false, "position4Mirrored.txt", 4, 422333)]
        //[InlineData(false, "position4Mirrored.txt", 5, 15833292)]
        //[InlineData(false, "position4Mirrored.txt", 6, 706045033)]
        [InlineData(true, "position5.txt", 1, 44)]
        [InlineData(true, "position5.txt", 2, 1486)]
        [InlineData(true, "position5.txt", 3, 62379)]
        [InlineData(true, "position5.txt", 4, 2103487)]
        //[InlineData(true, "position5.txt", 5, 89941194)]
        [InlineData(true, "position6.txt", 1, 46)]
        [InlineData(true, "position6.txt", 2, 2079)]
        [InlineData(true, "position6.txt", 3, 89890)]
        [InlineData(true, "position6.txt", 4, 3894594)]
        //[InlineData(true, "position6.txt", 5, 164075551)]
        //[InlineData(true, "position6.txt", 6, 6923051137)]
        public void AI_FindMovesToDepth_TestCount(bool isWhite, string boardFile, int maxPly, long expectedResult)
        {
            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFile(BoardDirectory + boardFile);
            MoveGeneration.CalculateDirections();
            BoardManager.UpdateAttackedPositions();

            AIPlayer aiPlayer = new AIPlayer(timeControl);
            List<Move> moves;
            List<Move> prevMoves = new();
            Dictionary<Move, int> positionsAftermove;
            (moves, positionsAftermove) = aiPlayer.FindMovesToSearchDepth(1, maxPly, prevMoves, isWhite);
            int numMoves = moves.Count;

            foreach (Move move in positionsAftermove.Keys)
            {
                output.WriteLine($"{move}: {positionsAftermove[move]}");
            }
            Assert.Equal(expectedResult, numMoves);
        }

        [Theory]
        [InlineData(true, "start.txt", 6, 119060324)]
        [InlineData(true, "position2.txt", 5, 193690690)]
        [InlineData(true, "position3.txt", 6, 11030083)]
        [InlineData(true, "position4.txt", 5, 15833292)]
        [InlineData(true, "position5.txt", 5, 89941194)]
        [InlineData(true, "position6.txt", 5, 164075551)]
        public void AI_FindMovesToDepth_TestPerf(bool isWhite, string boardFile, int maxPly, long expectedResult)
        {
            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFile(BoardDirectory + boardFile);
            MoveGeneration.CalculateDirections();
            BoardManager.UpdateAttackedPositions();

            AIPlayer aiPlayer = new AIPlayer(timeControl);
            List<Move> moves;
            List<Move> prevMoves = new();
            (moves, _) = aiPlayer.FindMovesToSearchDepth(1, maxPly, prevMoves, isWhite);
            int numMoves = moves.Count;


            Assert.Equal(expectedResult, numMoves);
        }

        [Theory]
        //[InlineData(true, "start.txt", 1, 20)]
        //[InlineData(true, "position2.txt", 1, 48)]
        //[InlineData(true, "position3.txt", 1, 14)]
        //[InlineData(true, "position4.txt", 1, 6)]
        //[InlineData(true, "position5.txt", 1, 44)]
        [InlineData(true, "special.txt", 3, 62379)]
        [InlineData(false, "special2.txt", 2, 1623)]
        [InlineData(true, "special3.txt", 1, 33)]
        public void SpecialTesting(bool isWhite, string boardFile, int maxPly, int expectedResult)
        {
            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFile(BoardDirectory + boardFile);
            MoveGeneration.CalculateDirections();
            BoardManager.UpdateAttackedPositions();

            AIPlayer aiPlayer = new AIPlayer(timeControl);
            List<Move> moves;
            List<Move> prevMoves = new();
            Dictionary<Move, int> positionsAftermove;
            (moves, positionsAftermove) = aiPlayer.FindMovesToSearchDepth(1, maxPly, prevMoves, isWhite);
            int numMoves = moves.Count;

            foreach (Move move in positionsAftermove.Keys)
            {
                output.WriteLine($"{move}: {positionsAftermove[move]}");
            }

            try
            {
                Assert.Equal(expectedResult, numMoves);
                output.WriteLine("Generated Moves:");
            }
            catch (XunitException e)
            {
                output.WriteLine($"{e.Message}");
                foreach (Move move in moves)
                {
                    //output.WriteLine(move.ToString());
                }
            }
            Assert.Equal(expectedResult, numMoves);
        }
    }
}
