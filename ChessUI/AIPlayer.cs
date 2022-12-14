using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

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
            bool isWhite = BoardManager.whiteToMove;
            Move[] moves = MoveGeneration.GenerateStricLegalMoves(isWhite);

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
            Node root = new Node();
            int negativeIninity = -1000000;
            int positiveInfinity = 1000000;
            GenerateMoveTree(0, maxDepth, root, negativeIninity, positiveInfinity, BoardManager.whiteToMove);

            //prints for observing effect of search settings.
            //int totalPositions = CountPositions(root);
            //Console.WriteLine("Total positions: " + totalPositions.ToString());
            //Console.WriteLine("Total Quiescence Positions: " + totalQuiescenceMoves.ToString());
            totalQuiescenceMoves = 0;
            return GetBestMove(root, BoardManager.whiteToMove);
        }

        private int CountPositions(Node root)
        {
            int count = 0;
            if (root.children.Count == 0)
            {
                return 1;
            }
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
                    if (child.evaluation > bestEval)
                    {
                        move = child.move;
                        bestEval = child.evaluation;
                    }
                }
                else
                {
                    if (child.evaluation < bestEval)
                    {
                        move = child.move;
                        bestEval = child.evaluation;
                    }
                }
            }

            return move;
        }

        private (int, int) Minimax(Node node, int depth, int maxDepth, bool isWhite, int alpha, int beta)
        {
            if (depth == maxDepth || node.children == null)
            {
                node.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.GetBoard());
                return (node.evaluation, 1);
            }
            int totalSearched = 0;
            if (isWhite)
            {
                node.evaluation = -10000000;
                foreach(Node child in node.children)
                {
                    (int target, int castle) = BoardManager.MakeMove(child.move);
                    (int newValue, int searched) = Minimax(child, depth + 1, maxDepth, false, alpha, beta);
                    BoardManager.UndoMove(child.move, target, castle);
                    totalSearched += searched;
                    node.evaluation = Math.Max(newValue, node.evaluation);
                    if (node.evaluation >= beta)
                    {
                        break;
                    }
                    alpha = Math.Max(alpha, node.evaluation);
                }                
                return (node.evaluation, totalSearched);
            }
            else
            {
                node.evaluation = 10000000;
                foreach (Node child in node.children)
                {
                    (int target, int castle) = BoardManager.MakeMove(child.move);
                    (int newValue, int searched) = Minimax(child, depth + 1, maxDepth, true, alpha, beta);
                    BoardManager.UndoMove(child.move, target, castle);
                    totalSearched += searched;
                    node.evaluation = Math.Min(newValue, node.evaluation);
                    if (node.evaluation <= alpha)
                    {
                        break;
                    }
                    beta = Math.Min(beta, node.evaluation);
                }
                return (node.evaluation, totalSearched);
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
        private void GenerateMoveTree(int currentSearchDepth, int maxSearchDepth, Node node, int alpha, int beta, bool maximising)
        {            
            
            if (currentSearchDepth == maxSearchDepth)
            {
                //node.evaluation = MoveEvaluation.EvaluateBoard(BoardManager.GetBoard());
                (int eval, int movesExplored) = QuiescenceSearch(alpha, beta, !maximising, 0, 5);
                totalQuiescenceMoves += movesExplored;
                node.evaluation = eval;
                return;
            }
            Move[] possibleMoves = MoveGeneration.GenerateStricLegalMoves(maximising);
            possibleMoves = MoveEvaluation.MoveOrdering(possibleMoves);

            if (maximising)
            {
                node.evaluation = -10000000;
                foreach (Move move in possibleMoves)
                {
                    Node child = new Node(move, node);
                    (int target, int castle) = BoardManager.MakeMove(move);
                    GenerateMoveTree(currentSearchDepth + 1, maxSearchDepth, child, alpha, beta, !maximising);
                    BoardManager.UndoMove(child.move, target, castle);
                    node.evaluation = Math.Max(child.evaluation, node.evaluation);
                    if (node.evaluation >= beta)
                    {
                        break;
                    }
                    node.AddChild(child);
                    alpha = Math.Max(alpha, node.evaluation);
                }
                return;
            }
            else
            {
                node.evaluation = 10000000;
                foreach (Move move in possibleMoves)
                {
                    Node child = new Node(move, node);
                    (int target, int castle) = BoardManager.MakeMove(move);
                    GenerateMoveTree(currentSearchDepth + 1, maxSearchDepth, child, alpha, beta, !maximising);
                    BoardManager.UndoMove(move, target, castle);
                    node.evaluation = Math.Min(child.evaluation, node.evaluation);
                    if (node.evaluation <= alpha)
                    {
                        break;
                    }
                    node.AddChild(child);
                    beta = Math.Min(beta, node.evaluation);
                }
                return;
            }
        }

        private (int, int) QuiescenceSearch(int alpha, int beta, bool maximising, int currentDepth, int maxDepth)
         {
            int exploredMoves = 1;
            int stand_pat = MoveEvaluation.EvaluateBoard(BoardManager.GetBoard());
            if (stand_pat >= beta)
            {
                return (beta, exploredMoves);
            }

            int maxDelta = 500; // queen value

            if (stand_pat < alpha - maxDelta)
            {
                return (alpha, exploredMoves);
            }
            if (alpha < stand_pat) { alpha = stand_pat; }

            if(currentDepth == maxDepth) { return (alpha, exploredMoves); }

            Move[] captureMoves = MoveGeneration.GenerateStricLegalMoves(maximising, generateOnlyCaptures: true);
            if(captureMoves.Length == 0) { return (alpha, exploredMoves); }
            captureMoves = MoveEvaluation.MoveOrdering(captureMoves);

            foreach (Move move in captureMoves)
            {
                (int target, int castle) = BoardManager.MakeMove(move);
                (int score, int additionalMoves) = QuiescenceSearch(-beta, -alpha, !maximising, currentDepth + 1, maxDepth);
                score = -score;
                exploredMoves += additionalMoves;
                BoardManager.UndoMove(move, target, castle);

                if (score >= beta)
                    return (beta, exploredMoves);
                if (score > alpha)
                    alpha = score;
            }
            return (alpha, exploredMoves);

        }

        public (Move[], Dictionary<Move, int>) FindMovesToSearchDepth(int currentSearchDepth, int maxSearchDepth, List<Move> prevMoves, bool isWhite)
        {
            Dictionary<Move, int> positionsAftermove = new Dictionary<Move, int>();

            BoardManager.whiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions(!isWhite);
            Move[] possibleMoves = MoveGeneration.GenerateStricLegalMoves(isWhite);

            if (currentSearchDepth == maxSearchDepth)
            {
                return (possibleMoves, positionsAftermove);
            }

            List<Move> movesAtLevel = new List<Move>();
            foreach (Move move in possibleMoves)
            {
                //BoardManager.UpdatePiecePositions(move);
                (int tempPiece, int tempCastleRights) = BoardManager.MakeMove(move);

                if (currentSearchDepth == 1)
                {
                    prevMoves = new List<Move>();
                }

                prevMoves.Add(move);
                (Move[] furtherMoves, Dictionary<Move, int>  xxx) = FindMovesToSearchDepth(currentSearchDepth + 1, maxSearchDepth, prevMoves, !isWhite);
                movesAtLevel = movesAtLevel.Concat(furtherMoves).ToList();
                prevMoves.Remove(move);

                BoardManager.UndoMove(move, tempPiece, tempCastleRights);
                //BoardManager.UndoPiecePositions(move);
            }

            return (movesAtLevel.ToArray(), positionsAftermove);
        }
    }
}
