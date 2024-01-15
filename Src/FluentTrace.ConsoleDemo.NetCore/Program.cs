namespace FluentTrace.ConsoleDemo.NetCore;

internal static class Program
{
    private const int NUMBERS_IN_SET = 10;
    private const int SLEEP_MS = 0;
    private const int WINNER_GOAL = 100;

    private static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.TreatControlCAsInput = false;

        Console.Clear();
        WriteTitle();

        var app = new App(
                numbersInSet: NUMBERS_IN_SET,
                sleepMs: SLEEP_MS,
                startingPosition: Console.CursorTop,
                winnerGoal: WINNER_GOAL);

        var finalPosition = WriteExitInstruction();
        app.Run();

        WriteFinalPrompt(finalPosition);
        Main(args);
    }

    private static int WriteExitInstruction()
    {
        Console.SetCursorPosition(0,
            Console.CursorTop + NUMBERS_IN_SET + 1);

        Console.ForegroundColor = Colors.Instruct;
        Console.WriteLine("   Press CTRL+C to exit.");
        return Console.CursorTop;
    }

    private static void WriteFinalPrompt(int position)
    {
        Console.SetCursorPosition(0, position);
        Console.ForegroundColor = Colors.Instruct;
        Console.WriteLine("   Press ENTER to run again.");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter);
    }

    private static void WriteTitle()
    {
        Console.WriteLine();
        Console.ForegroundColor = Colors.Title;
        Console.WriteLine("   -- DEMO APP --");
        Console.ForegroundColor = Colors.Instruct;
        Console.WriteLine($"   First number in set to reach {WINNER_GOAL} is the winner.");
        Console.WriteLine();
    }
}