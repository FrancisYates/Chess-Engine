// See https://aka.ms/new-console-template for more information

using ChessUI.UCI;

UCIComandInterpreter interpreter = new();
string command = string.Empty;
while (command != "quit")
{
	try
    {
        command = Console.ReadLine() ?? string.Empty;
        interpreter.ReceiveCommand(command);
    }
	catch (Exception e)
	{
		Console.WriteLine($"Exception Thrown: {e.Message}");
		Console.WriteLine(e.StackTrace);
	}
}