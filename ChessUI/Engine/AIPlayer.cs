using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using ChessUI.Enums;

namespace ChessUI.Engine
{
    public class AIPlayer
    {
        // Very high/low numbers which are not at risk over over/under flow.
        // Used
        public BookNode bookMoveTree = new();
        private readonly ThinkTimeCalculator _timeCalculator;
        public MoveSelectionType MoveSelectionType { get; init; }
        public int MaxSearchDepth { get; set; } = 3;
        private readonly Search _search;
        public AIPlayer(ThinkTimeCalculator timeCalculator, MoveSelectionType moveSelectionType = MoveSelectionType.ItterativeDeepening, bool isWhite = true)
        {
            MoveSelectionType = moveSelectionType;
            _timeCalculator = timeCalculator;
            _search = new() {
                MaxSearchDepth = MaxSearchDepth,
                IsWhiteMove = isWhite
            };
        }

        public Move? MakeMove()
        {
            _search.MaxSearchDepth = MaxSearchDepth;
            _search.IsWhiteMove = BoardManager.WhiteToMove;
            return MoveSelectionType switch
            {
                MoveSelectionType.Random => MakeRandomMove(),
                MoveSelectionType.Minimax => _search.MiniMaxSearch(),
                MoveSelectionType.ItterativeDeepening => _search.MakeItterativeDeepeningMove(_timeCalculator.GetThinkTimeMs()),
                MoveSelectionType.ExhaustiveSearch => _search.ExhaustiveSearch(),
                _ => throw new NotImplementedException($"MoveSelectionType is {MoveSelectionType}"),
            };
        }

        public Move? MakeRandomMove()
        {
            bool isWhite = BoardManager.WhiteToMove;
            Move[] moves = MoveGeneration.GenerateStrictLegalMoves(isWhite);

            Random random = new Random();
            int randomIndex = random.Next(0, moves.Length - 1);

            if (moves.Length == 0)
            {
                return null;
            }
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

        public void CreateBookTree()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ChessUI.MoveBook.txt";
            List<string> games = new List<string>();
            string[] y = assembly.GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                if (stream is null) return;
                while (!reader.EndOfStream) { games.Add(reader.ReadLine()); }
            }

            for (int i = 0; i < games.Count; i++)
            {
                BookNode node = bookMoveTree;
                int idx = 0;
                string[] gameMoves = games[i].Split(',');
                foreach (string move in gameMoves)
                {
                    if (node.HasChild(gameMoves[idx]))
                    {
                        node = node.GetChild(gameMoves[idx]);
                        idx++;
                        continue;
                    }
                    node.AddChild(new BookNode(gameMoves[idx], node));
                    node = node.GetChild(gameMoves[idx]);
                    idx++;
                }
            }
        }

        public Move? MakeBookMove()
        {
            int posibleBookMoves = bookMoveTree.children.Count();
            if (posibleBookMoves == 0) { return null; }
            Random rnd = new Random();
            int randomIdx = rnd.Next(posibleBookMoves - 1);
            string selectedMove = bookMoveTree.children[randomIdx].rootMove;
            bookMoveTree = bookMoveTree.GetChild(selectedMove);

            return LAN_ToMove(selectedMove);
        }

        private Move LAN_ToMove(string selectedMove)
        {
            int source = ToNumber(selectedMove[0]) + ((int)char.GetNumericValue(selectedMove[1]) - 1) * 8;
            int target = ToNumber(selectedMove[2]) + ((int)char.GetNumericValue(selectedMove[3]) - 1) * 8;

            return new Move(source, target);

            int ToNumber(char letter)
            {
                switch (letter)
                {
                    case 'a':
                        return 0;
                    case 'b':
                        return 1;
                    case 'c':
                        return 2;
                    case 'd':
                        return 3;
                    case 'e':
                        return 4;
                    case 'f':
                        return 5;
                    case 'g':
                        return 6;
                    case 'h':
                        return 7;
                }
                return 0;
            }
        }

        public void UpdateBookPosition(Move move)
        {
            string madeMove = move.ToString();
            if (bookMoveTree.HasChild(madeMove))
            {
                bookMoveTree = bookMoveTree.GetChild(madeMove);
            }
            else
            {
                bookMoveTree = new BookNode();
            }
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
                (int target, CastlingRights castle) = MoveManager.MakeMove(move, BoardManager.Board);
                (int score, int additionalMoves) = QuiescenceSearch(-beta, -alpha, !maximising, currentDepth + 1, maxDepth);
                score = -score;
                exploredMoves += additionalMoves;
                MoveManager.UndoMove(move, target, castle, BoardManager.Board);

                if (score >= beta) return (beta, exploredMoves);
                if (score > alpha) alpha = score;
            }
            return (alpha, exploredMoves);
        }

        public (Move[], Dictionary<Move, int>) FindMovesToSearchDepth(int currentSearchDepth, int maxSearchDepth, List<Move> prevMoves, bool isWhite)
        {
            Dictionary<Move, int> positionsAftermove = new Dictionary<Move, int>();

            BoardManager.WhiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions(!isWhite);
            Move[] possibleMoves = MoveGeneration.GenerateStrictLegalMoves(isWhite);
            if (currentSearchDepth == maxSearchDepth) return (possibleMoves, positionsAftermove);

            List<Move> movesAtLevel = new List<Move>();
            foreach (Move move in possibleMoves)
            {
                //BoardManager.UpdatePiecePositions(move);
                (int tempPiece, int tempCastleRights) = MoveManager.MakeMove(move, BoardManager.Board);

                if (currentSearchDepth == 1) prevMoves = new List<Move>();

                prevMoves.Add(move);
                (Move[] furtherMoves, Dictionary<Move, int> xxx) = FindMovesToSearchDepth(currentSearchDepth + 1, maxSearchDepth, prevMoves, !isWhite);
                movesAtLevel = movesAtLevel.Concat(furtherMoves).ToList();
                prevMoves.Remove(move);
                MoveManager.UndoMove(move, tempPiece, tempCastleRights, BoardManager.Board);
                //BoardManager.UndoPiecePositions(move);
            }

            return (movesAtLevel.ToArray(), positionsAftermove);
        }
        public List<string> FindReachablePositions(int currentSearchDepth, int maxSearchDepth, bool isWhite)
        {
            List<string> positions = new();

            BoardManager.WhiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions(!isWhite);
            Move[] possibleMoves = MoveGeneration.GenerateStrictLegalMoves(isWhite);
            if (currentSearchDepth == maxSearchDepth) {
                foreach (var move in possibleMoves) {
                    (int tempPiece, CastlingRights tempCastleRights) = MoveManager.MakeMove(move, BoardManager.Board);
                    string fen = BoardManager.GetCurrentFen();
                    positions.Add(fen);
                MoveManager.UndoMove(move, tempPiece, tempCastleRights, BoardManager.Board);
                }
                return positions;
            };

            foreach (Move move in possibleMoves)
            {
                //BoardManager.UpdatePiecePositions(move);
                (int tempPiece, CastlingRights tempCastleRights) = MoveManager.MakeMove(move, BoardManager.Board);
                string fen = BoardManager.GetCurrentFen();
                positions.Add(fen);

                List<string> futurePositions = FindReachablePositions(currentSearchDepth + 1, maxSearchDepth, !isWhite);
                positions.AddRange(futurePositions);
                MoveManager.UndoMove(move, tempPiece, tempCastleRights, BoardManager.Board);
                //BoardManager.UndoPiecePositions(move);
            }

            return positions;
        }
    }
}
