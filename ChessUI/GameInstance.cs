using ChessUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChessUI
{
    public class GameInstance
    {
        readonly AIPlayer aiPlayer;
        MainWindow _window;
        public GameInstance(MainWindow window)
        {
            _window = window;
            aiPlayer = new AIPlayer();

            aiPlayer.CreateBookTree();
            MoveGeneration.CalculateDirections();
            BoardManager.LoadBoard();

            BoardManager.UpdateAttackedPositions(true);
            BoardManager.UpdateAttackedPositions(false);
        }

        public void MakePlayerMove(Move move)
        {
            (_, _) = MoveManager.MakeMove(move, BoardManager.Board);
            aiPlayer.UpdateBookPosition(move);

            Render.UpdateBoard(_window.Buttons, BoardManager.Board);
            //Render.HighlightSquare(_window.Buttons, selectedPosition);
            Render.RemoveHighlightFromSquare(_window.Buttons, move.sourceSquare);

            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions(BoardManager.WhiteToMove);
            OpponentMove();

        }
        private void OpponentMove()
        {
            if (BoardManager.FullMoves <= 5)
            {
                BoardManager.UpdateSideToMove();
                if ( TryMakeBookMove() ) return;
                MakeSearchMove();
            }
            else
            {
                BoardManager.UpdateSideToMove();
                MakeSearchMove();
            }
        }

        private bool TryMakeBookMove()
        {
            Move? bookMove = aiPlayer.MakeBookMove();
            if (bookMove is null) { return false; }
            Move move_ = bookMove ?? new Move(0, 0);
            (_, _) = MoveManager.MakeMove(move_, BoardManager.Board);
            Render.UpdateBoard(_window.Buttons, BoardManager.Board);

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions(!BoardManager.WhiteToMove);
            return true;
        }

        private void MakeSearchMove()
        {
            int maxDepth = 5;
            Move? move = aiPlayer.MakeBestEvaluatedMove(maxDepth);
            if(move is null) throw new NullReferenceException(nameof(move));
            (_, _) = MoveManager.MakeMove((Move)move, BoardManager.Board);
            Render.UpdateBoard(_window.Buttons, BoardManager.Board);

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions(!BoardManager.WhiteToMove);
        }
    }
}
