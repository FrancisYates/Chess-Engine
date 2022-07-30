using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessUI;
using ChessUI.Pieces;
using System.Collections.Generic;
//using NUnit.Framework;
using Xunit;

namespace ChessTests
{
    [TestClass]
    internal class PieceTests
    {
        private Game game = new("C:/Users/Jane/source/repos/Chessv5/ChessUI/bin/Debug/TestBoards/start.txt");

        /*
        [TestMethod]
        private Game LoadBoard()
        {
            return new Game("C:/Users/Jane/source/repos/Chessv5/ChessUI/bin/Debug/TestBoards/start.txt");
        }
        */
        private short[,] SetupGameBoard(string boardFile)
        {
            string path = "C:/Users/Jane/source/repos/Chessv5/ChessUI/bin/Debug/TestBoards/" + boardFile;
            Game game = new(path);
            return game.GetBoard();

        }

        [TestMethod]
        [Theory]
        [InlineData(1, 0, true, "start.txt", 2)]
        [InlineData(6, 0, false, "start.txt", 2)]
        [InlineData(3, 1, true, "test1.txt", 3)]
        [InlineData(6, 0, false, "test1.txt", 1)]
        public void Pawn_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            Pawn testPawn = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int),(int, int))[] moves;

            moves = testPawn.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

        [TestMethod]
        [Theory]
        [InlineData(0, 0, true, "start.txt", 0)]
        [InlineData(0, 1, true, "test1.txt", 6)]
        [InlineData(4, 7, false, "test1.txt", 9)]
        [InlineData(7, 0, false, "test1.txt", 0)]
        public void Rook_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            Rook testRook = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int), (int, int))[] moves;

            moves = testRook.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

        [TestMethod]
        [Theory]
        [InlineData(0, 1, true, "start.txt", 0)]
        [InlineData(1, 3, true, "test1.txt", 7)]
        [InlineData(7, 2, false, "test1.txt", 1)]
        [InlineData(7, 5, false, "test1.txt", 4)]
        public void Bishop_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            Bishop testBishop = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int), (int, int))[] moves;

            moves = testBishop.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

        [TestMethod]
        [Theory]
        [InlineData(0, 2, true, "start.txt", 2)]
        [InlineData(3, 3, true, "test1.txt", 6)]
        [InlineData(0, 2, false, "test1.txt", 6)]
        public void Knight_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            Knight testKnight = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int), (int, int))[] moves;

            moves = testKnight.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

        [TestMethod]
        [Theory]
        [InlineData(0, 3, true, "start.txt", 0)]
        [InlineData(6, 1, true, "test1.txt", 12)]
        [InlineData(0, 4, false, "test1.txt", 12)]
        public void Queen_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            Queen testQueen = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int), (int, int))[] moves;

            moves = testQueen.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

        [TestMethod]
        [Theory]
        [InlineData(0, 4, true, "start.txt", 0)]
        [InlineData(3, 2, true, "test1.txt", 5)]
        [InlineData(7, 4, false, "test1.txt", 2)]
        public void King_FindSudoLegalMoves(short yPosition, short xPosition, bool isWhite, string boardFile, int expectedResult)
        {
            King testKing = new(yPosition, xPosition, isWhite);
            short[,] board = SetupGameBoard(boardFile);
            ((int, int), (int, int))[] moves;

            moves = testKing.FindSudoLegalMoves(board);
            int numMoves = moves.Length;

            Assert.Equals(numMoves, expectedResult);
        }

    }
}
