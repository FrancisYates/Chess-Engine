using ChessUI;
using ChessUI.Engine;
using ChessUI.Enums;
using ChessUI.UCI;
using System.Diagnostics;

namespace ChessEngineH2H
{
    public partial class Form1 : Form
    {
        private string whiteEnginePath = @"C:\Users\Jane\Documents\Chess Engines\Latest\UCIHead.exe";
        private string blackEnginePath = @"C:\Users\Jane\Documents\Chess Engines\Latest\UCIHead.exe";
        private MoveSelectionType whitesearchType = MoveSelectionType.ItterativeDeepening;
        private MoveSelectionType blacksearchType = MoveSelectionType.ItterativeDeepening;
        EngineManager engineManager = new();
        AIPlayer whitePlayer = new();
        AIPlayer blackPlayer = new(isWhite: false);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private (int, int, int) PlayGame()
        {
            UCIComandInterpreter whiteInterpreter = new(whitePlayer);
            UCIComandInterpreter blackInterpreter = new(blackPlayer);

            bool isWhite = false;
            whiteInterpreter.ProcessPositionCommand("position startpos");
            var move = whiteInterpreter.ProcessGoCommand("go movetime 1000");
            BoardManager.UpdateMoveCount();
            BoardManager.WhiteToMove = !BoardManager.WhiteToMove;
            while (move != "0000")
            {
                var toMove = isWhite ? whiteInterpreter : blackInterpreter;
                var moved = isWhite ? blackInterpreter : whiteInterpreter;
                string board = moved.GetFen();
                //Debug.WriteLine(board);
                toMove.ProcessPositionCommand($"position fen {board}");
                move = whiteInterpreter.ProcessGoCommand("go movetime 1000");
                BoardManager.WhiteToMove = !BoardManager.WhiteToMove;
                BoardManager.UpdateMoveCount();
                if (BoardManager.FullMoves > 100) return (0, 1, 0);
                isWhite = !isWhite;
            }
            return isWhite ? (1, 0, 0) : (0, 0, 1);
        }


        private void startBtn_Click(object sender, EventArgs e)
        {
            EngineOptions options = new()
            {
                BlackEnginePath = blackEnginePath,
                WhiteEnginePath = whiteEnginePath,
                BlackSelectionType = blacksearchType,
                WhiteSelectionType = whitesearchType,
                ThinkTimeMs = (int)ThinkTimeInput.Value,
                NumGames = (int)NumGamesInput.Value
            };
            engineManager.StartGame(options);
        }

        private void whiteMoveSelectBtn_Rnd_Click(object sender, EventArgs e)
        {
            whitePlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.Random;
            whitesearchType = MoveSelectionType.Random;
        }
        private void whiteMoveSelectBtn_Exhaustive_Click(object sender, EventArgs e)
        {
            whitePlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.ExhaustiveSearch;
            whitesearchType = MoveSelectionType.ExhaustiveSearch;
        }

        private void whiteMoveSelectBtn_MiniMax_Click(object sender, EventArgs e)
        {
            whitePlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.Minimax;
            whitesearchType = MoveSelectionType.Minimax;
        }

        private void whiteMoveSelectBtn_ItterDeep_Click(object sender, EventArgs e)
        {
            whitePlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.ItterativeDeepening;
            whitesearchType = MoveSelectionType.ItterativeDeepening;
        }

        private void blackMoveSelectBtn_Rnd_Click(object sender, EventArgs e)
        {
            blackPlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.Random;
            blacksearchType = MoveSelectionType.Random;
        }

        private void blackMoveSelectBtn_Exhaustive_Click(object sender, EventArgs e)
        {
            blackPlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.ExhaustiveSearch;
            blacksearchType = MoveSelectionType.ExhaustiveSearch;
        }

        private void blackMoveSelectBtn_MiniMax_Click(object sender, EventArgs e)
        {
            blackPlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.Minimax;
            blacksearchType = MoveSelectionType.Minimax;
        }

        private void blackMoveSelectBtn_ItterDeep_Click(object sender, EventArgs e)
        {
            blackPlayer.MoveSelectionType = ChessUI.Enums.MoveSelectionType.ItterativeDeepening;
            blacksearchType = MoveSelectionType.ItterativeDeepening;
        }

        private void selectEngineBtn_white_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            DialogResult result = openFileDialog.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                FileInfo fileInfo = new(file);
                whiteEnginePath = fileInfo.FullName;
                whiteEngineLabel.Text = fileInfo.Directory.Name;
                Debug.WriteLine(whiteEnginePath);
            }

        }

        private void selectEngineBtn_black_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            DialogResult result = openFileDialog.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog.FileName;
                FileInfo fileInfo = new(file);
                blackEnginePath = fileInfo.FullName;
                blackEngineLabel.Text = fileInfo.Directory.Name;
                Debug.WriteLine(blackEnginePath);
            }
        }

        private async void NextPlyBtn_Click(object sender, EventArgs e)
        {
            await engineManager.NextPly();
        }

        int whiteWins = 0;
        int draws = 0;
        int blackWins = 0;
        private async void RunGameBtn_Click(object sender, EventArgs e)
        {
            EngineOptions options = new()
            {
                BlackEnginePath = blackEnginePath,
                WhiteEnginePath = whiteEnginePath,
                BlackSelectionType = blacksearchType,
                WhiteSelectionType = whitesearchType,
                ThinkTimeMs = (int)ThinkTimeInput.Value,
                NumGames = (int)NumGamesInput.Value
            };
            GameInfo info;

            for (int i = 0; i < options.NumGames; i++)
            {
                engineManager.StartGame(options);
                do
                {
                    info = await engineManager.NextPly();
                } while (info.InProgress);
                switch (info.Outcome)
                {
                    case GameOutcome.WhiteWin:
                        whiteWins++;
                        WhiteWinsCountLabel.Text = whiteWins.ToString();
                        break;
                    case GameOutcome.Draw:
                        draws++;
                        DrawsCountLabel.Text = draws.ToString();
                        break;
                    case GameOutcome.BlackWin:
                        blackWins++;
                        BlackWinsCountLabel.Text = blackWins.ToString();
                        break;
                }
                Debug.WriteLine($"Game Outcome: {info.Outcome}");
                await engineManager.SaveGameState(info.Id, info);
            }
        }
    }
}
