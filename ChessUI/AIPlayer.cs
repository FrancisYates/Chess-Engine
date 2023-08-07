using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace ChessUI
{
    public class AIPlayer
    {
        public static BookNode bookMoveTree = new BookNode();

        public AIPlayer()
        {

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

        public Move? MakeBestEvaluatedMove(int maxDepth)
        {
            Node root = new();
            GenerateMoveTree(0, maxDepth, int.MinValue, int.MaxValue, BoardManager.WhiteToMove);

            //prints for observing effect of search settings.
            //int totalPositions = CountPositions(root);
            //Console.WriteLine("Total positions: " + totalPositions.ToString());
            //Console.WriteLine("Total Quiescence Positions: " + totalQuiescenceMoves.ToString());
            totalQuiescenceMoves = 0;
            return GetBestMove(root, BoardManager.WhiteToMove);
        }
        public Move? MakeBestEvaluatedMoveItterativeDeepening(int maxDepth, int maxTimeMS)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource(maxTimeMS);
            var sw1 = Stopwatch.StartNew();
            Node previousSearch = GenerateMoveTree(0, maxDepth, int.MinValue, int.MaxValue, BoardManager.WhiteToMove);
            sw1.Stop();
            Debug.WriteLine("Initial search time " + sw1.ElapsedMilliseconds);
            Node currentSearch = previousSearch;
            for (int i = 1; i < maxDepth; i++)
            {
                if (tokenSource.Token.IsCancellationRequested) break;
                var sw = Stopwatch.StartNew();
                currentSearch = GenerateMoveTreeID(0, maxDepth, int.MinValue, int.MaxValue, BoardManager.WhiteToMove, previousSearch, tokenSource.Token);
                sw.Stop();
                Debug.WriteLine($"Itteration {i} time taken {sw.ElapsedMilliseconds}");
            }

            totalQuiescenceMoves = 0;
            return GetBestMove(currentSearch, BoardManager.WhiteToMove);
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

        private Move? GetBestMove(Node root, bool isWhite)
        {
            Move? move = null;
            int bestEval = isWhite ? -10000000 : 10000000;
            foreach (Node child in root.children)
            {
                if (isWhite)
                {
                    if (child.evaluation > bestEval) UpdateBestMove(child);
                }
                else
                {
                    if (child.evaluation < bestEval) UpdateBestMove(child);
                }
            }

            return move;

            void UpdateBestMove(Node child)
            {
                move = child.move;
                bestEval = child.evaluation;
            }
        }

        private (int, int) Minimax(Node node, int depth, int maxDepth, bool isWhite, int alpha, int beta)
        {
            if (depth == maxDepth || node.children == null)
            {
                node.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.Board);
                return (node.evaluation, 1);
            }
            int totalSearched = 0;
            if (isWhite)
            {
                node.evaluation = -10000000;
                foreach(Node child in node.children)
                {
                    Search(child);
                    if (node.evaluation >= beta) break;
                    alpha = Math.Max(alpha, node.evaluation);
                }                
            }
            else
            {
                node.evaluation = 10000000;
                foreach (Node child in node.children)
                {
                    Search(child);
                    if (node.evaluation <= alpha) break;
                    beta = Math.Min(beta, node.evaluation);
                }
            }

            return (node.evaluation, totalSearched);

            void Search(Node child)
            {
                (int target, int castle) = MoveManager.MakeMove(child.move, BoardManager.Board);
                (int newValue, int searched) = Minimax(child, depth + 1, maxDepth, true, alpha, beta);
                MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);
                totalSearched += searched;
                node.evaluation = Math.Min(newValue, node.evaluation);
            }
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
            int source = ToNumber(selectedMove[0]) + ((int)Char.GetNumericValue(selectedMove[1]) - 1) * 8;
            int target = ToNumber(selectedMove[2]) + ((int)Char.GetNumericValue(selectedMove[3]) - 1) * 8;

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

        static int totalQuiescenceMoves = 0;
        private Node GenerateMoveTree(int currentSearchDepth, int maxDepth, int alpha, int beta, bool maximising, CancellationToken token = default)
        {            
            var node = new Node();
            if (currentSearchDepth == maxDepth)
            {
                (int eval, int movesExplored) = QuiescenceSearch(alpha, beta, !maximising, 0, 5);
                totalQuiescenceMoves += movesExplored;
                node.evaluation = eval;
                return node;
            }

            IEnumerable<Move> possibleMoves = MoveGeneration.GenerateStrictLegalMoves(maximising);
            possibleMoves = MoveEvaluation.MoveOrdering(possibleMoves);
            if (maximising)
            {
                node.evaluation = -10000000;
                foreach (Move move in possibleMoves)
                {
                    if (token.IsCancellationRequested) break;
                    if (node.evaluation >= beta) break;
                    Node child = GenerateChild(node.move, node, currentSearchDepth, maxDepth, alpha, beta, maximising);
                    node.AddChild(child);
                    alpha = Math.Max(alpha, node.evaluation);
                }
            }
            else
            {
                node.evaluation = 10000000;
                foreach (Move move in possibleMoves)
                {
                    if (token.IsCancellationRequested) break;
                    if (node.evaluation <= alpha) break;
                    Node child = GenerateChild(node.move, node, currentSearchDepth, maxDepth, alpha, beta, maximising);
                    node.AddChild(child);
                    beta = Math.Min(beta, node.evaluation);
                }
            }
            return node;
        }
        private Node GenerateMoveTreeID(int currentSearchDepth, int maxDepth , int alpha, int beta, bool maximising, Node previousSearch, CancellationToken token)
        {            
            var root = new Node();
            IEnumerable<Node> possibleMoves = MoveEvaluation.MoveOrderingID(previousSearch);

            if (maximising)
            {
                root.evaluation = -10000000;
                foreach (Node node in possibleMoves)
                {
                    if (token.IsCancellationRequested) break;
                    Node child = GenerateChildID(node.move, root, currentSearchDepth, maxDepth, alpha, beta, maximising, node, token);
                    if (root.evaluation >= beta) break;
                    root.AddChild(child);
                    alpha = Math.Max(alpha, root.evaluation);
                }
            }
            else
            {
                root.evaluation = 10000000;
                foreach (Node node in possibleMoves)
                {
                    if (token.IsCancellationRequested) break;
                    Node child = GenerateChildID(node.move, root, currentSearchDepth, maxDepth, alpha, beta, maximising, node, token);
                    if (root.evaluation <= alpha) break;
                    root.AddChild(child);
                    beta = Math.Min(beta, root.evaluation);
                }
            }
            return root;
        }

        private Node GenerateChildID(Move move, Node parent, int currentDepth, int maxDepth, int alpha, int beta, bool maximising, Node previousSearch, CancellationToken token)
        {
            (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
            Node child;
            if (previousSearch.children.Count > 0)
            {
                child = GenerateMoveTreeID(currentDepth + 1, maxDepth, alpha, beta, maximising, previousSearch, token);
            }
            else
            {
                child = GenerateMoveTree(currentDepth + 1, maxDepth, alpha, beta, maximising, token);
            }
            child.move = move;
            child.parent = parent;
            MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);

            if(maximising)
            {
                parent.evaluation = Math.Max(child.evaluation, parent.evaluation);
            }
            else
            {
                parent.evaluation = Math.Min(child.evaluation, parent.evaluation);
            }
            return child;
        }

        private Node GenerateChild(Move move, Node parent, int currentDepth, int maxDepth, int alpha, int beta, bool maximising)
        {
            
            (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
            Node child = GenerateMoveTree(currentDepth + 1, maxDepth, alpha, beta, maximising);
            MoveManager.UndoMove(child.move, target, castle, BoardManager.Board);

            if(maximising)
            {
                parent.evaluation = Math.Max(child.evaluation, parent.evaluation);
            }
            else
            {
                parent.evaluation = Math.Min(child.evaluation, parent.evaluation);
            }
            return child;
        }

        private (int, int) QuiescenceSearch(int alpha, int beta, bool maximising, int currentDepth, int maxDepth, CancellationToken token = default)
         {
            int exploredMoves = 1;
            int stand_pat = MoveEvaluation.EvaluateBoard(BoardManager.Board);
            if (stand_pat >= beta) return (beta, exploredMoves);

            int maxDelta = 900; // queen value

            if (stand_pat < alpha - maxDelta) return (alpha, exploredMoves);
            if (alpha < stand_pat) alpha = stand_pat;

            if(token.IsCancellationRequested) return (alpha, exploredMoves);

            IEnumerable<Move> captureMoves = MoveGeneration.GenerateStrictLegalMoves(maximising, generateOnlyCaptures: true);
            if(!captureMoves.Any()) return (alpha, exploredMoves);
            captureMoves = MoveEvaluation.MoveOrdering(captureMoves);

            foreach (Move move in captureMoves)
            {
                (int target, int castle) = MoveManager.MakeMove(move, BoardManager.Board);
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
                (Move[] furtherMoves, Dictionary<Move, int>  xxx) = FindMovesToSearchDepth(currentSearchDepth + 1, maxSearchDepth, prevMoves, !isWhite);
                movesAtLevel = movesAtLevel.Concat(furtherMoves).ToList();
                prevMoves.Remove(move);

                MoveManager.UndoMove(move, tempPiece, tempCastleRights, BoardManager.Board);
                //BoardManager.UndoPiecePositions(move);
            }

            return (movesAtLevel.ToArray(), positionsAftermove);
        }
    }
}
