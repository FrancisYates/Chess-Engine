using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using ChessUI.Enums;
using System.Diagnostics;

namespace ChessUI.Engine
{
    public class AIPlayer
    {
        Dictionary<string, Move[]> bookMoveDict = new();
        public BookNode bookMoveTree = new();
        public Action<string> OnMoveChosen;
        private readonly ThinkTimeCalculator _timeCalculator;
        public MoveSelectionType MoveSelectionType { get; set; }
        public int MaxSearchDepth { get; set; } = 5;
        public int ThinkTimeMs { get; set; }
        public bool IsThinking { get; set; }
        private CancellationTokenSource SearchTokenSource { get; set; }

        private readonly Search _search;
        Random random = new();
        public AIPlayer(MoveSelectionType moveSelectionType = MoveSelectionType.ItterativeDeepening, bool isWhite = true, bool useBookMove = false)
        {
            if (useBookMove) CreateBookTree();
            MoveSelectionType = moveSelectionType;
            _timeCalculator = new();
            _search = new()
            {
                MaxSearchDepth = MaxSearchDepth,
                IsWhiteMove = isWhite
            };
        }
        public AIPlayer(ThinkTimeCalculator timeCalculator, MoveSelectionType moveSelectionType = MoveSelectionType.ItterativeDeepening, bool isWhite = true, bool useBookMove = false)
        {
            if (useBookMove) CreateBookTree();
            MoveSelectionType = moveSelectionType;
            _timeCalculator = timeCalculator;
            _search = new() {
                MaxSearchDepth = MaxSearchDepth,
                IsWhiteMove = isWhite
            };
        }

        public Move? MakeMove()
        {
            var bookMove = MakeBookMove();
            if (bookMove is not null) return bookMove;
            _search.MaxSearchDepth = MaxSearchDepth;
            _search.IsWhiteMove = BoardManager.WhiteToMove;
            var move =  MoveSelectionType switch
            {
                MoveSelectionType.Random => MakeRandomMove(),
                MoveSelectionType.Minimax => _search.MiniMaxSearch(),
                MoveSelectionType.ItterativeDeepening => _search.MakeItterativeDeepeningMove(ThinkTimeMs),
                MoveSelectionType.ExhaustiveSearch => _search.ExhaustiveSearch(),
                _ => throw new NotImplementedException($"MoveSelectionType is {MoveSelectionType}"),
            };
            OnMoveChosen?.Invoke(move?.ToString() ?? "0000");
            return move;
        }

        public Move? MakeRandomMove()
        {
            bool isWhite = BoardManager.WhiteToMove;
            List<Move> moves = MoveGeneration.GenerateStrictLegalMoves(isWhite);


            if (moves.Count == 0)
            {
                return null;
            }
            int randomIndex = random.Next(0, moves.Count - 1);
            return moves[randomIndex];
        }

        private int CountPositions(Node root)
        {
            int count = 0;
            if (root.children.Count == 0) return 1;
            foreach (Node child in root.children)
            {
                count += CountPositions(child);
            }
            return count;
        }
        private void CreateBookTree()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ChessUI.book.dat";
            using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new NullReferenceException();
            using StreamReader reader = new(stream);
            Dictionary<string, Move[]> bookMoveDict = new();

            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(':');
                string fen = line[0];
                string[] moves = line[1].Split(",");
                bookMoveDict[fen] = moves.Select(Move.Parse).ToArray();
            }
        }

        public Move? MakeBookMove()
        {
            int halfMoves = BoardManager.HalfMoves;
            int fullMoves = BoardManager.FullMoves;
            BoardManager.HalfMoves = 0;
            BoardManager.FullMoves = 0;
            string fen = BoardManager.GetCurrentFen();
            BoardManager.HalfMoves = halfMoves;
            BoardManager.FullMoves = fullMoves;
            if (!bookMoveDict.TryGetValue(fen, out Move[]? possibleMoves))
            {
                return null;
            }
            Debug.WriteLine($"Found {possibleMoves.Length} possible book moves");

            Random rnd = new ();
            int randomIdx = rnd.Next(possibleMoves.Length - 1);
            return possibleMoves[randomIdx];
        }



        private (int, int) QuiescenceSearch(int alpha, int beta, bool maximising, int currentDepth, int maxDepth, CancellationToken token = default)
        {
            int exploredMoves = 1;
            int stand_pat = MoveEvaluation.EvaluateBoard(BoardManager.Board);
            if (stand_pat >= beta) return (beta, exploredMoves);

            int maxDelta = 900; // queen value

            if (stand_pat < alpha - maxDelta) return (alpha, exploredMoves);
            if (alpha < stand_pat) alpha = stand_pat;

            if (token.IsCancellationRequested) return (alpha, exploredMoves);

            IEnumerable<Move> captureMoves = MoveGeneration.GenerateStrictLegalMoves(maximising, generateOnlyCaptures: true);
            if (!captureMoves.Any()) return (alpha, exploredMoves);
            captureMoves = MoveEvaluation.MoveOrdering(captureMoves);

            foreach (Move move in captureMoves)
            {
                MoveChanges changes = MoveManager.MakeMove(move, BoardManager.Board, maximising);
                (int score, int additionalMoves) = QuiescenceSearch(-beta, -alpha, !maximising, currentDepth + 1, maxDepth, token);
                score = -score;
                exploredMoves += additionalMoves;
                MoveManager.UndoMove(move, changes, maximising);

                if (score >= beta) return (beta, exploredMoves);
                if (score > alpha) alpha = score;
            }
            return (alpha, exploredMoves);
        }

        public (int, Dictionary<Move, int>) FindMovesToSearchDepth(int currentSearchDepth, int maxSearchDepth, bool isWhite)
        {
            Dictionary<Move, int> positionsAftermove = new();

            BoardManager.WhiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions();
            List<Move> possibleMoves = MoveGeneration.GenerateStrictLegalMoves(isWhite);
            if (currentSearchDepth == maxSearchDepth)
            {
                foreach (var move in possibleMoves)
                {
                    positionsAftermove.Add(move, 1);
                }

                return (possibleMoves.Count, positionsAftermove);
            }

            int movesAtLevel = 0;
            foreach (Move move in possibleMoves)
            {
                MoveChanges changes = MoveManager.MakeMove(move, BoardManager.Board, isWhite);

                (int furtherMoves, Dictionary<Move, int> xxx) = FindMovesToSearchDepth(currentSearchDepth + 1, maxSearchDepth, !isWhite);
                
                positionsAftermove.Add(move, furtherMoves);
                movesAtLevel += furtherMoves;
                MoveManager.UndoMove(move, changes, isWhite);
            }

            return (movesAtLevel, positionsAftermove);
        }
        public List<string> FindReachablePositions(int currentSearchDepth, int maxSearchDepth, bool isWhite)
        {
            List<string> positions = new();

            BoardManager.WhiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions();
            List<Move> possibleMoves = MoveGeneration.GenerateStrictLegalMoves(isWhite);
            if (currentSearchDepth == maxSearchDepth) {
                foreach (var move in possibleMoves) {
                    MoveChanges changes = MoveManager.MakeMove(move, BoardManager.Board, isWhite);
                    string fen = BoardManager.GetCurrentFen();
                    positions.Add(fen);
                MoveManager.UndoMove(move, changes, isWhite);
                }
                return positions;
            };

            foreach (Move move in possibleMoves)
            {
                MoveChanges changes = MoveManager.MakeMove(move, BoardManager.Board, isWhite);
                string fen = BoardManager.GetCurrentFen();
                positions.Add(fen);

                List<string> futurePositions = FindReachablePositions(currentSearchDepth + 1, maxSearchDepth, !isWhite);
                positions.AddRange(futurePositions);
                MoveManager.UndoMove(move, changes, isWhite);
            }

            return positions;
        }

        internal void Stop()
        {
            SearchTokenSource.Cancel();
        }

        internal void StartNewGame()
        {
            throw new NotImplementedException();
        }
    }
}
