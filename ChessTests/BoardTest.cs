using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChessUI;

namespace ChessTests
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void TestLoadBoard()
        {
            Game game = new Game("C:/Users/Jane/source/repos/Chessv5/ChessUI/bin/Debug/start.txt");
        }
    }
}