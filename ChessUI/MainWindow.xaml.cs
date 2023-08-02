using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ChessUI.Enums;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<Button> buttons;
        int selectedPosition = -1;
        bool pieceSelected = false;
        public MoveType promotionSelection;
        public PromotionPiece promotionPiece;
        readonly AIPlayer aiPlayer;
        //readonly Player player;
        public MainWindow()
        {
            InitializeComponent();
            buttons = CreateButtonList();

            aiPlayer = new AIPlayer();
            aiPlayer.CreateBookTree();
            MoveGeneration.CalculateDirections();
            BoardManager.LoadBoard();
            Render.UpdateBoard(buttons, BoardManager.GetBoard());

            BoardManager.UpdateAttackedPositions(true);
            BoardManager.UpdateAttackedPositions(false);
        }
        public void SetPromotion(MoveType selection, PromotionPiece piece) 
        { 
            promotionSelection = selection;
            promotionPiece = piece;
        }

        private void HandelClick(short y, short x)
        {
            int thisPosition = 63 - (y * 8 + (7 - x));
            bool validSelection = Player.IsValidSelection(BoardManager.GetBoard(), thisPosition);
            if (validSelection)
            {
                if(selectedPosition != -1)
                {
                    Render.RemoveHighlightFromSquare(buttons, selectedPosition);
                }
                selectedPosition = thisPosition;
                pieceSelected = true;
                Render.HighlightSquare(buttons, selectedPosition);
            }
            if (!validSelection && pieceSelected)
            {
                Move move = new Move(selectedPosition, thisPosition);
                if (Player.IsMoveValid(ref move))
                {
                    if (move.IsPromotion())
                    {
                        PromotionSelection selectionWin = new PromotionSelection(this);
                        selectionWin.ShowDialog();
                        move.moveFlag = ((int)promotionSelection | (int)promotionPiece);
                    }
                    (_, _) = BoardManager.MakeMove(move);
                    aiPlayer.UpdateBookPosition(move);
                    Render.UpdateBoard(buttons, BoardManager.GetBoard());
                    Render.HighlightSquare(buttons, selectedPosition);
                    Render.RemoveHighlightFromSquare(buttons, selectedPosition);
                    selectedPosition = -1;
                    pieceSelected = false;

                    //BoardManager.UpdateSideToMove();
                    BoardManager.UpdateMoveCount();
                    BoardManager.UpdateAttackedPositions(BoardManager.whiteToMove);
                    OpponentMove();
                }
            }
        }

        private void OpponentMove()
        {
            if (BoardManager.fullMoves <= 5)
            {
                BoardManager.UpdateSideToMove();
                bool success = MakeBookMove();
                if (success) { return; }
                MakeSearchMove();
            }
            else
            {
                BoardManager.UpdateSideToMove();
                MakeSearchMove();
            }
        }

        private bool MakeBookMove()
        {
            Move? bookMove = aiPlayer.MakeBookMove();
            if (bookMove == null) { return false; }
            Move move_ = bookMove ?? new Move(0, 0);
            (_, _) = BoardManager.MakeMove(move_);
            Render.UpdateBoard(buttons, BoardManager.GetBoard());

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions(!BoardManager.whiteToMove);
            return true;
        }

        private void MakeSearchMove()
        {
            int maxDepth = 5;
            Move? move = aiPlayer.MakeBestEvaluatedMove(maxDepth);
            Move move_ = move ?? new Move(0, 0);
            (_, _) = BoardManager.MakeMove(move_);
            Render.UpdateBoard(buttons, BoardManager.GetBoard());

            BoardManager.UpdateSideToMove();
            BoardManager.UpdateMoveCount();
            BoardManager.UpdateAttackedPositions(!BoardManager.whiteToMove);
        }

        #region rank_1_ClickHandel
        private void ButtonA1Click(object sender, RoutedEventArgs e)
        {
            HandelClick(7, 0);
        }
        private void ButtonB1Click(object sender, RoutedEventArgs e)
        {
            HandelClick(7, 1);
        }
        private void ButtonC1Click(object sender, RoutedEventArgs e)
        {
            HandelClick(7, 2);
        }
        private void ButtonD1Click(object sender, RoutedEventArgs e)
        {
            HandelClick(7, 3);
        }
        private void ButtonE1Click(object sender, RoutedEventArgs e)
        {
             HandelClick(7, 4);
        }
        private void ButtonF1Click(object sender, RoutedEventArgs e)
        {
             HandelClick(7, 5);
        }
        private void ButtonG1Click(object sender, RoutedEventArgs e)
        {
             HandelClick(7, 6);
        }
        private void ButtonH1Click(object sender, RoutedEventArgs e)
        {
             HandelClick(7, 7);
        }
        #endregion

        #region rank_2_ClickHandel
        private void ButtonA2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 0);
        }
        private void ButtonB2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 1);
        }
        private void ButtonC2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 2);
        }
        private void ButtonD2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 3);
        }
        private void ButtonE2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 4);
        }
        private void ButtonF2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 5);
        }
        private void ButtonG2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 6);
        }
        private void ButtonH2Click(object sender, RoutedEventArgs e)
        {
            HandelClick(6, 7);
        }
        #endregion

        #region rank_3_ClickHandel
        private void ButtonA3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 0);
        }
        private void ButtonB3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 1);
        }
        private void ButtonC3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 2);
        }
        private void ButtonD3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 3);
        }
        private void ButtonE3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 4);
        }
        private void ButtonF3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 5);
        }
        private void ButtonG3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 6);
        }
        private void ButtonH3Click(object sender, RoutedEventArgs e)
        {
            HandelClick(5, 7);
        }
        #endregion

        #region rank_4_ClickHandel
        private void ButtonA4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 0);
        }
        private void ButtonB4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 1);
        }
        private void ButtonC4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 2);
        }
        private void ButtonD4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 3);
        }
        private void ButtonE4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 4);
        }
        private void ButtonF4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 5);
        }
        private void ButtonG4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 6);
        }
        private void ButtonH4Click(object sender, RoutedEventArgs e)
        {
            HandelClick(4, 7);
        }
        #endregion

        #region rank_5_ClickHandel
        private void ButtonA5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 0);
        }
        private void ButtonB5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 1);
        }
        private void ButtonC5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 2);
        }
        private void ButtonD5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 3);
        }
        private void ButtonE5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 4);
        }
        private void ButtonF5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 5);
        }
        private void ButtonG5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 6);
        }
        private void ButtonH5Click(object sender, RoutedEventArgs e)
        {
            HandelClick(3, 7);
        }
        #endregion

        #region rank_6_ClickHandel
        private void ButtonA6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 0);
        }
        private void ButtonB6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 1);
        }
        private void ButtonC6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 2);
        }
        private void ButtonD6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 3);
        }
        private void ButtonE6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 4);
        }
        private void ButtonF6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 5);
        }
        private void ButtonG6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 6);
        }
        private void ButtonH6Click(object sender, RoutedEventArgs e)
        {
            HandelClick(2, 7);
        }
        #endregion

        #region rank_7_ClickHandel
        private void ButtonA7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 0);
        }
        private void ButtonB7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 1);
        }
        private void ButtonC7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 2);
        }
        private void ButtonD7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 3);
        }
        private void ButtonE7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 4);
        }
        private void ButtonF7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 5);
        }
        private void ButtonG7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 6);
        }
        private void ButtonH7Click(object sender, RoutedEventArgs e)
        {
            HandelClick(1, 7);
        }
        #endregion

        #region rank_8_ClickHandel
        private void ButtonA8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 0);
        }
        private void ButtonB8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 1);
        }
        private void ButtonC8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 2);
        }
        private void ButtonD8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 3);
        }
        private void ButtonE8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 4);
        }
        private void ButtonF8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 5);
        }
        private void ButtonG8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 6);
        }
        private void ButtonH8Click(object sender, RoutedEventArgs e)
        {
            HandelClick(0, 7);
        }
        #endregion

        private List<Button> CreateButtonList()
        {
            List<Button> buttons = new List<Button>
            {
                A1,
                B1,
                C1,
                D1,
                E1,
                F1,
                G1,
                H1,

                A2,
                B2,
                C2,
                D2,
                E2,
                F2,
                G2,
                H2,

                A3,
                B3,
                C3,
                D3,
                E3,
                F3,
                G3,
                H3,

                A4,
                B4,
                C4,
                D4,
                E4,
                F4,
                G4,
                H4,

                A5,
                B5,
                C5,
                D5,
                E5,
                F5,
                G5,
                H5,

                A6,
                B6,
                C6,
                D6,
                E6,
                F6,
                G6,
                H6,

                A7,
                B7,
                C7,
                D7,
                E7,
                F7,
                G7,
                H7,

                A8,
                B8,
                C8,
                D8,
                E8,
                F8,
                G8,
                H8
            };

            return buttons;
        }
    }
}
