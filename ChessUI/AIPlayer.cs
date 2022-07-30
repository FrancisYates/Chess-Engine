using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessUI
{
    public static class AIPlayer
    {
        public static BookNode bookMoveTree = new BookNode();
        public static Move? MakeRandomMove()
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

        public static Move? MakeBestEvaluatedMove(int maxDepth)
        {
            //MoveGeneration.GenerateStricLegalMoves(BoardManager.whiteToMove);
            Node root = new Node();
            GenerateMoveTree(1, maxDepth, root);
            (_, int searched) = Minimax(root, 1, maxDepth, BoardManager.whiteToMove, -10000000, 10000000);
            int totalPositions = CountPositions(root);
            Console.WriteLine("Total positions: " + totalPositions.ToString());
            Console.WriteLine("positions evaluated: " + searched.ToString());
            return GetBestMove(root, BoardManager.whiteToMove);
        }

        private static int CountPositions(Node root)
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
        private static Move? GetBestMove(Node root, bool isWhite)
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

        private static (int, int) Minimax(Node node, int depth, int maxDepth, bool isWhite, int alpha, int beta)
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
                    (int target, int castle) = BoardManager.MakeTempMove(child.move);
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
                    (int target, int castle) = BoardManager.MakeTempMove(child.move);
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

        public static void CreateBookTree()
        {
            string pgnAddress = "C:\\Users\\Jane\\source\\repos\\Chessv5\\ChessUI\\moveBook.txt";
            string[] games = File.ReadAllLines(pgnAddress);

            for (int i = 0; i < games.Length; i++)
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

        public static Move? MakeBookMove()
        {
            int posibleBookMoves = bookMoveTree.children.Count();
            if (posibleBookMoves == 0) { return null; }
            Random rnd = new Random();
            int randomIdx = rnd.Next(posibleBookMoves - 1);
            string selectedMove = bookMoveTree.children[randomIdx].rootMove;
            bookMoveTree = bookMoveTree.GetChild(selectedMove);

            return LAN_ToMove(selectedMove);
        }

        private static Move LAN_ToMove(string selectedMove)
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

        public static void UpdateBookPosition(Move move)
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

        private static void GenerateMoveTree(int currentSearchDepth, int maxSearchDepth, Node parentNode)
        {            
            Move[] possibleMoves = MoveGeneration.GenerateStricLegalMoves(BoardManager.whiteToMove);

            if (currentSearchDepth == maxSearchDepth)
            {
                foreach (Move move in possibleMoves)
                {
                    Node newNode = new Node(move, parentNode);
                    parentNode.AddChild(newNode);
                }
                return;
            }

            foreach (Move move in possibleMoves)
            {
                BoardManager.UpdateSideToMove();
                Node newNode = new Node(move, parentNode);
                parentNode.AddChild(newNode);
                (int tempPiece, int tempCastleRights) = BoardManager.MakeTempMove(move);

                GenerateMoveTree(currentSearchDepth + 1, maxSearchDepth, newNode);

                BoardManager.UndoMove(move, tempPiece, tempCastleRights);
                BoardManager.UpdateSideToMove();
            }

            //return parentNode;
        }

        public static (Move[], Dictionary<Move, int>) FindMovesToSearchDepth(int currentSearchDepth, int maxSearchDepth, List<Move> prevMoves, bool isWhite)
        {
            Dictionary<Move, int> positionsAftermove = new Dictionary<Move, int>();


            //bool isWhite = currentSearchDepth % 2 == 1 ? false : true;
            BoardManager.whiteToMove = isWhite;
            BoardManager.UpdateAttackedPositions(!isWhite);
            Move[] possibleMoves = MoveGeneration.GenerateStricLegalMoves(isWhite);
            //BoardManager.whiteToMove = !isWhite;

            if (currentSearchDepth == maxSearchDepth)
            {
                return (possibleMoves, positionsAftermove);
            }

            List<Move> movesAtLevel = new List<Move>();
            foreach (Move move in possibleMoves)
            {
                //BoardManager.UpdatePiecePositions(move);
                (int tempPiece, int tempCastleRights) = BoardManager.MakeTempMove(move);

                if (currentSearchDepth == 1)
                {
                    prevMoves = new List<Move>();
                }

                //BoardManager.UpdateAttackedPositions(!isWhite);
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
