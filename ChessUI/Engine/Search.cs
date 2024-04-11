using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessUI.Engine
{
    public class Search
    {
        private const int negativeInfinity = int.MinValue + 50_000;
        private const int positiveInfinity = int.MaxValue - 50_000;

        public required bool IsWhiteMove { get; set; }
        public bool IsBlackMove => !IsWhiteMove;
        public required int MaxSearchDepth { get; set; }
        public int MaxQuiescenceSearchDepth { get; set; } = 3;
        public int PositionsEvaluated { get; set; }
        public int QuiescenceMovesEvaluated { get; set; }

        #region Exhaustive Search
        public Move? ExhaustiveSearch() {
            var sw = Stopwatch.StartNew();
            PositionsEvaluated = 0;
            Node root = GenerateMoveTree(0, IsWhiteMove);
            sw.Stop();
            Debug.WriteLine($"Evaluated {PositionsEvaluated} positions in {sw.ElapsedMilliseconds} ms");
            return GetBestMove(root);
        }
        #endregion
        private Node GenerateMoveTree(int currentSearchDepth, bool maximising, CancellationToken token = default) {
            var node = new Node();
            if (currentSearchDepth == MaxSearchDepth) {
                PositionsEvaluated++;
                node.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.Board);
                (int eval, int movesExplored) = (0, 0);
                QuiescenceMovesEvaluated += movesExplored;
                if (maximising) {
                    node.evaluation = Math.Max(node.evaluation, eval);
                } else {
                    node.evaluation = Math.Min(node.evaluation, eval);
                }
                return node;
            }
            IEnumerable<Move> possibleMoves = MoveGeneration.GenerateStrictLegalMoves(maximising);
            if (maximising) {
                node.evaluation = negativeInfinity;
                foreach (Move move in possibleMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    Node child = GenerateChild(move, node, currentSearchDepth, !maximising);
                    child.move = move;
                    node.AddChild(child);
                }
            } else {
                node.evaluation = positiveInfinity;
                foreach (Move move in possibleMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    Node child = GenerateChild(move, node, currentSearchDepth, !maximising);
                    child.move = move;
                    node.AddChild(child);
                }
            }
            return node;
        }
        private Node GenerateChild(Move move, Node parent, int currentDepth, bool maximising, Node? previousSearch = null, CancellationToken token = default) {
            (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
            Node child = GenerateMoveTree(currentDepth + 1,maximising, token);
            child.move = move;
            child.parent = parent;
            MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);

            if (maximising) {
                parent.evaluation = Math.Max(child.evaluation, parent.evaluation);
            } else {
                parent.evaluation = Math.Min(child.evaluation, parent.evaluation);
            }
            return child;
        }
        #region MiniMax
        public Move? MiniMaxSearch() {
            var sw = Stopwatch.StartNew();
            PositionsEvaluated = 0;
            Node root = GenerateMoveTree(0, negativeInfinity, positiveInfinity, IsWhiteMove);
            sw.Stop();
            Debug.WriteLine($"Evaluated {PositionsEvaluated} positions in {sw.ElapsedMilliseconds}ms");
            return GetBestMove(root);
        }

        private Node GenerateMoveTree(int currentSearchDepth, int alpha, int beta, bool maximising, CancellationToken token = default) {
            var node = new Node();
            if (currentSearchDepth == MaxSearchDepth) {
                PositionsEvaluated++;
                node.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.Board);
                //(int eval, int movesExplored) = QuiescenceSearch(alpha, beta, maximising, 0);
                (int eval, int movesExplored) = (0, 0);
                QuiescenceMovesEvaluated += movesExplored;
                //if(maximising) {
                //    node.evaluation = Math.Max(node.evaluation, eval);
                //} else {
                //    node.evaluation = Math.Min(node.evaluation, eval);
                //}
                return node;
            }
            IEnumerable<Move> possibleMoves = MoveGeneration.GenerateStrictLegalMoves(maximising);
            possibleMoves = MoveEvaluation.MoveOrdering(possibleMoves);
            if (maximising) {
                node.evaluation = negativeInfinity;
                foreach (Move move in possibleMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    if (node.evaluation >= beta) break;
                    Node child = GenerateChild(move, node, currentSearchDepth, alpha, beta, !maximising);
                    child.move = move;
                    node.AddChild(child);
                    alpha = Math.Max(alpha, node.evaluation);
                }
            } else {
                node.evaluation = positiveInfinity;
                foreach (Move move in possibleMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    if (node.evaluation <= alpha) break;
                    Node child = GenerateChild(move, node, currentSearchDepth, alpha, beta, !maximising);
                    child.move = move;
                    node.AddChild(child);
                    beta = Math.Min(beta, node.evaluation);
                }
            }
            return node;
        }

        private Node GenerateChild(Move move, Node parent, int currentDepth, int alpha, int beta, bool maximising, Node? previousSearch = null, CancellationToken token = default) {
            (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
            Node child = GenerateMoveTree(currentDepth + 1, alpha, beta, maximising, token);            
            child.move = move;
            child.parent = parent;
            MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);

            if (maximising) {
                parent.evaluation = Math.Min(child.evaluation, parent.evaluation);
            } else {
                parent.evaluation = Math.Max(child.evaluation, parent.evaluation);
            }
            return child;
        }
        #endregion

        #region Itterative Deapening MiniMax

        public Move? MakeItterativeDeepeningMove(int maxTimeMS) {
            var sw = Stopwatch.StartNew();
            PositionsEvaluated = 0;
            maxTimeMS = int.MaxValue;
            Debug.WriteLine($"Making ID MiniMax move with max think time of {maxTimeMS}ms");
            int maxIdDepth = MaxSearchDepth;
            MaxSearchDepth = 1;
            CancellationTokenSource tokenSource = new (maxTimeMS);
            var sw1 = Stopwatch.StartNew();
            Node previousSearch = GenerateMoveTree(0, negativeInfinity, positiveInfinity, BoardManager.WhiteToMove);
            sw1.Stop();
            Debug.WriteLine("Initial search time " + sw1.ElapsedMilliseconds);
            Node currentSearch = previousSearch;
            while (MaxSearchDepth < maxIdDepth) {
                if (tokenSource.Token.IsCancellationRequested) {
                    Debug.WriteLine("Token Cancelled");
                    break;
                }
                MaxSearchDepth++;
                sw1 = Stopwatch.StartNew();
                var nextSearch = GenerateMoveTreeID(0, negativeInfinity, positiveInfinity, BoardManager.WhiteToMove, currentSearch, tokenSource.Token);
                sw1.Stop();
                Debug.WriteLine($"Itteration {MaxSearchDepth} time taken {sw.ElapsedMilliseconds}");
                currentSearch = nextSearch;
            }
            MaxSearchDepth = maxIdDepth;
            sw.Stop();
            Debug.WriteLine($"Evaluated {PositionsEvaluated} positions in {sw.ElapsedMilliseconds}ms");
            return GetBestMove(currentSearch);
        }
        private Node GenerateMoveTreeID(int currentSearchDepth, int alpha, int beta, bool maximising, Node previousSearch, CancellationToken token = default) {
            var root = new Node();
            if (currentSearchDepth == MaxSearchDepth) {
                PositionsEvaluated++;
                root.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.Board);
                //(int eval, int movesExplored) = QuiescenceSearch(alpha, beta, maximising, 0);
                (int eval, int movesExplored) = (0, 0);
                QuiescenceMovesEvaluated += movesExplored;
                if (maximising) {
                    root.evaluation = Math.Max(root.evaluation, eval);
                } else {
                    root.evaluation = Math.Min(root.evaluation, eval);
                }
                return root;
            }
            IEnumerable<Node> orderedMoves = MoveEvaluation.MoveOrderingID(previousSearch);

            if (maximising) {
                root.evaluation = negativeInfinity;
                foreach (Node node in orderedMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    Node child = GenerateChildID(node.move, root, currentSearchDepth, alpha, beta, maximising, node, token);
                    if (root.evaluation >= beta) break;
                    root.AddChild(child);
                    alpha = Math.Max(alpha, root.evaluation);
                }
            } else {
                root.evaluation = positiveInfinity;
                foreach (Node node in orderedMoves) {
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Token Cancelled");
                        break;
                    }
                    Node child = GenerateChildID(node.move, root, currentSearchDepth, alpha, beta, maximising, node, token);
                    if (root.evaluation <= alpha) break;
                    root.AddChild(child);
                    beta = Math.Min(beta, root.evaluation);
                }
            }
            return root;
        }
        private Node GenerateChildID(Move move, Node parent, int currentDepth, int alpha, int beta, bool maximising, Node previousSearch, CancellationToken token = default) {
            (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
            Node child;
            if (previousSearch.children.Count > 0) {
                child = GenerateMoveTreeID(currentDepth + 1, alpha, beta, maximising, previousSearch, token);
            } else {
                child = GenerateMoveTree(currentDepth + 1, alpha, beta, maximising, token);
            }
            child.move = move;
            child.parent = parent;
            MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);

            if (maximising) {
                parent.evaluation = Math.Max(child.evaluation, parent.evaluation);
            } else {
                parent.evaluation = Math.Min(child.evaluation, parent.evaluation);
            }
            return child;
        }
        #endregion


        private (int, int) QuiescenceSearch(int alpha, int beta, bool maximising, int currentDepth, CancellationToken token = default) {
            int exploredMoves = 1;
            int stand_pat = MoveEvaluation.EvaluateBoard(BoardManager.Board);
            if (stand_pat >= beta) return (beta, exploredMoves);

            int maxDelta = 900; // queen value

            if (stand_pat < alpha - maxDelta) return (alpha, exploredMoves);
            if (alpha < stand_pat) alpha = stand_pat;

            if (token.IsCancellationRequested || currentDepth == MaxQuiescenceSearchDepth) return (alpha, exploredMoves);

            IEnumerable<Move> captureMoves = MoveGeneration.GenerateStrictLegalMoves(maximising, generateOnlyCaptures: true);
            if (!captureMoves.Any()) return (alpha, exploredMoves);
            captureMoves = MoveEvaluation.MoveOrdering(captureMoves);

            foreach (Move move in captureMoves) {
                (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
                (int score, int additionalMoves) = QuiescenceSearch(-beta, -alpha, !maximising, currentDepth + 1, token);
                score = -score;
                exploredMoves += additionalMoves;
                MoveManager.UndoMove(move, target, castle, BoardManager.Board);

                if (score >= beta) return (beta, exploredMoves);
                if (score > alpha) alpha = score;
                if (token.IsCancellationRequested) return (alpha, exploredMoves);
            }
            return (alpha, exploredMoves);
        }

        private Move? GetBestMove(Node root) {
            List<Move> comperableMoves = new();
            int bestEval = IsWhiteMove ? negativeInfinity : positiveInfinity;
            foreach (Node child in root.children) {
                bool isMoveBetter = IsWhiteMove ? child.evaluation > bestEval : child.evaluation < bestEval;
                if (isMoveBetter) {
                    comperableMoves.Clear();
                    comperableMoves.Add(child.move);
                    bestEval = child.evaluation;
                }else if (child.evaluation == bestEval) {
                    comperableMoves.Add(child.move);
                }
            }
            if (comperableMoves.Count == 0) return null;
            Random rnd = new();
            return comperableMoves[rnd.Next(comperableMoves.Count - 1)];
        }
    }
}
