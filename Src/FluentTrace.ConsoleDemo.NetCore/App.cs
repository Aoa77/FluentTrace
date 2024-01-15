using FluentTrace.NetStandard;
using System.Text.Json;

namespace FluentTrace.ConsoleDemo.NetCore;

internal sealed class App
{
    private static readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true
    };

    private readonly CancellationTokenSource _cts;
    private readonly Random _random;
    private readonly Dictionary<int, Record> _set;
    private readonly int _sleepMs;
    private readonly int _startingPosition;
    private readonly int _winnerGoal;
    private int? _winnerKey;

    public App(CancellationTokenSource cts,
        int numbersInSet,
        int sleepMs,
        int startingPosition,
        int winnerGoal)
    {
        TraceLog.Capture().WithData(
            (nameof(numbersInSet), numbersInSet, typeof(int)),
            (nameof(sleepMs), sleepMs, typeof(int)),
            (nameof(startingPosition), startingPosition, typeof(int)),
            (nameof(winnerGoal), winnerGoal, typeof(int))
        ).Flush();

        _random = new();
        _set = new();
        for (int i = 0; i < numbersInSet; i++)
        {
            _set.Add(i, new(i, 0, Colors.Default));
        }
        _cts = cts;
        _sleepMs = sleepMs;
        _startingPosition = startingPosition;
        _winnerGoal = winnerGoal;
    }

    public void Run()
    {
        WriteSet();
        while (!_cts.IsCancellationRequested && !_winnerKey.HasValue)
        {
            var key = GenerateRandomNumber(0, _set.Count);
            IncrementRecord(key);
            UpdateSet();
            WriteSet();
            Thread.Sleep(_sleepMs);
        }
    }

    private int GenerateRandomNumber(int min, int max)
    {
        return _random.Next(min, max);
    }

    private void IncrementRecord(int key)
    {
        var record = _set[key];
        _set[key] = record with
        { Frequency = record.Frequency + 1 };

        record = _set[key];
        if (record.Frequency == _winnerGoal)
        {
            _winnerKey = key;
        }
    }

    private void UpdateSet()
    {
        var frequencies = _set.Values.Select(x => x.Frequency);
        foreach (var record in _set.Values)
        {
            if (record.Frequency == frequencies.Max())
            {
                _set[record.Key] = record with
                { Color = Colors.High };
                continue;
            }
            if (record.Frequency == frequencies.Min())
            {
                _set[record.Key] = record with
                { Color = Colors.Low };
                continue;
            }
            _set[record.Key] = record with
            { Color = Colors.Default };
        }
    }

    private void WriteRecord(int key)
    {
        var record = _set[key];
        var frequency = record.Frequency.ToString().PadLeft(3);
        var result = $"    num {key} : {frequency}";

        var isWinner = (key == _winnerKey);
        if (isWinner)
        {
            result += " :: WINNER!";
            record = record with { Color = Colors.Winner };
        }

        result = result.PadRight(Console.WindowWidth);
        Console.ForegroundColor = record.Color;
        Console.WriteLine(result);
    }

    private void WriteSet()
    {
        Console.SetCursorPosition(0, _startingPosition);
        foreach (var key in _set.Keys)
        {
            WriteRecord(key);
        }

        var json = JsonSerializer.Serialize(_set, _json);
        TraceLog.Capture()
            .WithMessage(json)
            .Flush();
    }
}
