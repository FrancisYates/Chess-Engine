using ChessUI.Engine;
using ChessUI.Enums;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace ChessUI.Views
{

    struct EngineSearchOption
    {
        public string Name { get; set; }
        public MoveSelectionType Type{ get; set; }
        public EngineSearchOption(string name, MoveSelectionType type)
        {
            Name = name;
            Type = type;
        }
    }
    /// <summary>
    /// Interaction logic for DebugControls.xaml
    /// </summary>
    public partial class DebugControls : Window
    {
        GameInfo info;
        int positionIndex = 0;
        GameWindow GameWindow { get; set; }
        public DebugControls(GameWindow window)
        {
            InitializeComponent();
            GameWindow = window;
            EvaluationType.ItemsSource = searchOptions;
        }

        private static List<EngineSearchOption> searchOptions = [
            new ("Exhaustive", MoveSelectionType.ExhaustiveSearch),
            new ("Minimax", MoveSelectionType.Minimax),
            new ("Itterative Deepening", MoveSelectionType.ItterativeDeepening),
            ];

        private async void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            var result = openFileDialog.ShowDialog(); // Show the dialog.
            if (result is true) // Test result.
            {
                positionIndex = 0;
                string file = openFileDialog.FileName;
                var json = await File.ReadAllTextAsync(file);
                info = JsonSerializer.Deserialize<GameInfo>(json);
                BoardManager.ResetBoardToEmpty();
                BoardManager.LoadBoardFromFen(info.BoardStates.First());

                Move nextMove = info.Moves[positionIndex];
                UpdateBoard(info.BoardStates.First(), nextMove);
            }
        }

        private void NextPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            if(positionIndex < info.BoardStates.Count - 1) {
                string fen = info.BoardStates[++positionIndex];
                Move nextMove = info.Moves[positionIndex];
                UpdateBoard(fen, nextMove);
            }
        }
        private void PrevPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (positionIndex >0)
            {
                string fen = info.BoardStates[--positionIndex];
                Move nextMove = info.Moves[positionIndex];
                UpdateBoard(fen, nextMove);
            }
        }

        private void UpdateBoard(string fen, Move nextMove)
        {
            MoveNumberLabel.Content = positionIndex;
            NextMoveLabel.Content = nextMove.ToString();
            FenLabel.Text = fen;

            BoardManager.ResetBoardToEmpty();
            BoardManager.LoadBoardFromFen(fen);
            Render.Reset(GameWindow.Buttons);
            Render.UpdateBoard(GameWindow.Buttons, BoardManager.Board);
            Render.HighlightSquare(GameWindow.Buttons, nextMove.SourceSquare);
            Render.HighlightSquare(GameWindow.Buttons, nextMove.TargetSquare);
        }

        private class GameInfo
        {
            public bool InProgress { get; set; }
            public int HalfMoves { get; set; }
            public int FullMoves { get; set; }
            public List<Move> Moves { get; set; } = [];
            public List<string> BoardStates { get; set; } = [];
        }

        private void EvaluatePositionBtn_Click(object sender, RoutedEventArgs e)
        {
            EngineSearchOption? selected = (EngineSearchOption?)EvaluationType.SelectedItem;
            AIPlayer ai = new(isWhite: BoardManager.WhiteToMove)
            {
                MoveSelectionType = selected?.Type ?? MoveSelectionType.Minimax,
                MaxSearchDepth = int.Parse(EvaluationDepth.Text),
                ThinkTimeMs = 1000
            };
            ai.MakeMove();
            Debug.WriteLine(MoveEvaluation.EvaluateBoard([]));
        }
    }
}
