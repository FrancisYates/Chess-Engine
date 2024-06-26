﻿using ChessUI.Engine;
using ChessUI.Enums;
using Serilog;
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
        private readonly GameWindow _window;
        public GameInstance(GameWindow window, ThinkTimeCalculator thinkTimer)
        {
            _window = window;
            aiPlayer = new AIPlayer(thinkTimer, MoveSelectionType.ExhaustiveSearch, isWhite:false) {
                MaxSearchDepth = 4,
            };

            //aiPlayer.CreateBookTree();
            BoardManager.LoadBoardFromFile("startPosition.txt");
            
            BoardManager.UpdateAttackedPositions();
        }

        public void MakePlayerMove(Move move) {
            Debug.WriteLine($"Making player move {move}");
            (_, _) = MoveManager.MakeMove(move, BoardManager.Board);
            aiPlayer.UpdateBookPosition(move);

            Render.UpdateBoard(_window.Buttons, BoardManager.Board, move);
            //Render.HighlightSquare(_window.Buttons, selectedPosition);
            Render.RemoveHighlightFromSquare(_window.Buttons, move.sourceSquare);

            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions();
            OpponentMove();
            Render.UpdateBoard(_window.Buttons, BoardManager.Board);
        }
        private void OpponentMove()
        {
            if (BoardManager.FullMoves < 0)
            {
                BoardManager.UpdateSideToMove();
                if (TryMakeBookMove()) {
                    Debug.WriteLine("Making book Move");
                    return;
                }
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
            if (bookMove is null) return false; 
            Move move_ = bookMove ?? new Move(0, 0);
            Debug.WriteLine($"Making ai move {move_}");
            (_, _) = MoveManager.MakeMove(move_, BoardManager.Board);
            Render.UpdateBoard(_window.Buttons, BoardManager.Board, move_);

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions();
            return true;
        }

        private void MakeSearchMove() {
            Debug.WriteLine("Searching for move");
            Move? move = aiPlayer.MakeMove();
            if(move is null)
            {
                Log.Logger.Error("No Move found in board position: ");
                Log.Logger.Error(BoardManager.GetCurrentFen());
                throw new NullReferenceException(nameof(move));
            }
            Debug.WriteLine($"Making ai move {move}");
            (_, _) = MoveManager.MakeMove((Move)move, BoardManager.Board);
            Render.UpdateBoard(_window.Buttons, BoardManager.Board);
            Render.UpdateBoard(_window.Buttons, BoardManager.Board, (Move)move);

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions();
        }
    }
}
