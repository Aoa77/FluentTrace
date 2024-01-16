using FluentTrace.NetStandard;
using System.Text.Json;

namespace FluentTrace.ConsoleDemo.NetCore;

internal static class Program
{
    private const int APP_THROTTLE = 0;
    private const int INPUT_THROTTLE = 100;
    private const int NUMBERS_IN_SET = 10;
    private const int WINNER_GOAL = 100;

    private static readonly CancellationTokenSource _cts = new();

    private static void Main(string[] args)
    {
        TraceLog.Config = TraceConfig.Create.RelativeToProject();
        TraceLog.Config.Json.WriteIndented = true;

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, e) =>
        {
            _cts.Cancel();
            e.Cancel = true;
        };

        while (!_cts.IsCancellationRequested)
        {
            Console.TreatControlCAsInput = false;
            AppLoop();
        }

        _cts.Dispose();
    }

    private static void AppLoop()
    {
        Console.Clear();
        WriteTitle();

        var app = new App(_cts,
                numbersInSet: NUMBERS_IN_SET,
                sleepMs: APP_THROTTLE,
                startingPosition: Console.CursorTop,
                winnerGoal: WINNER_GOAL);

        var finalPosition = WriteExitInstruction();
        app.Run();

        FlushInputBuffer();
        WriteFinalPrompt(finalPosition);
    }

    private static void FlushInputBuffer()
    {
        while (Console.KeyAvailable)
        {
            Thread.Sleep(INPUT_THROTTLE);
            Console.ReadKey(true);
        }
    }

    private static int WriteExitInstruction()
    {
        Console.SetCursorPosition(0,
            Console.CursorTop + NUMBERS_IN_SET + 1);

        Console.ForegroundColor = Colors.Exit;
        Console.WriteLine("   Press CTRL+C to exit.");
        return Console.CursorTop;
    }

    private static void WriteFinalPrompt(int position)
    {
        if (_cts.IsCancellationRequested)
        {
            return;
        }

        Console.SetCursorPosition(0, position);
        Console.ForegroundColor = Colors.RunAgain;
        Console.WriteLine("   Press ENTER to run again.");

        Console.TreatControlCAsInput = true;
        while (!_cts.IsCancellationRequested)
        {
            var input = Console.ReadKey(true);
            if (input.Key == ConsoleKey.Enter)
            {
                return;
            }
            if (input.Key == ConsoleKey.C &&
                input.Modifiers == ConsoleModifiers.Control)
            {
                _cts.Cancel();
            }
        }
    }

    private static void WriteTitle()
    {
        Console.WriteLine();
        Console.ForegroundColor = Colors.Title;
        Console.WriteLine("   -- DEMO APP --");
        Console.ForegroundColor = Colors.Subtitle;
        Console.WriteLine($"   First number in set to reach {WINNER_GOAL} is the winner.");
        Console.WriteLine();
    }
}