// See https://aka.ms/new-console-template for more information
using ChessUI;
using ChessUI.Engine;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

internal class Program
{
    private static void Main(string[] args)
    {
        bool validInput = false;
        Console.WriteLine("Enter path to games file");
        string bookLocation;
        do
        {
            bookLocation = Console.ReadLine();
            validInput = File.Exists(bookLocation ?? "");
        } while (!validInput);

        Console.Write("Max Depth: ");
        int maxDepth = 0;
        validInput = false;
        do
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int i))
            {
                maxDepth = i;
                validInput = true;
            }

        } while (!validInput);

        CreateBook(bookLocation, maxDepth);

        Console.WriteLine($"exceptions: {exceptions}");
    }
    static void CreateBook(string sourceLocation, int maxDepth)
    {
        var moveLines = File.ReadAllLines(sourceLocation);
        List<List<string>> moves = moveLines.Select(l => l.Split(',').ToList()).ToList();

        var moveGrouping = moves.GroupBy(m => m[0]);

        BookNode root = new();
        AddRootNodes(moves, root, 1, maxDepth);

        BoardManager.ResetBoardToEmpty();
        BoardManager.LoadBoardFromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0");
        ConvertToFenDictionary(root);

        var output = kvp.Select(kvp => $"{kvp.Key}:{string.Join(',', kvp.Value)}");

        File.WriteAllLines("book.dat", output);
    }

    static Dictionary<string, HashSet<string>> kvp = new();
    static Stack<Move> moveStack = new();
    static int exceptions = 0;
    private static void ConvertToFenDictionary(BookNode root)
    {
        foreach (var node in root.Children)
        {
            Move move = Move.Parse(node.Move);
            string t = BoardManager.GetCurrentFen();
            BoardManager.UpdateSideToMove();
            string fen = BoardManager.GetCurrentFen();
            if (kvp.TryGetValue(fen, out HashSet<string>? value))
            {
                value.Add(node.Move);
            }
            else
            {
                kvp.Add(fen, [node.Move]);
            }
            moveStack.Push(move);
            try
            {
                var (priorContents, castling) = MoveManager.MakeMove(move, BoardManager.Board);
                ConvertToFenDictionary(node);
                MoveManager.UndoMove(move, priorContents, castling, BoardManager.Board);
                BoardManager.UpdateSideToMove();
            }
            catch (Exception)
            {
                exceptions++;
            }
            moveStack.Pop();
        }
    }

    static void AddRootNodes(IEnumerable<IEnumerable<string>> moveLines, BookNode root, int currentDepth, int maxDepth)
    {
        var moveGrouping = moveLines.GroupBy(m => m.First());

        foreach (var group in moveGrouping)
        {
            BookNode node = new(group.Key);
            root.AddChild(node);
            var tail = group.Where(g => g.Count() > 1).Select(x => x.TakeLast(x.Count() - 1)).ToList();
            if(currentDepth < maxDepth)
            {
                AddRootNodes(tail, node, currentDepth+1, maxDepth);
            }
        }
    }
}