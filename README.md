# FluentTrace
C# fluent-style extension methods for trace-logging call site metadata.

## Setup
Prior to logging your first `Capture`, the `TraceLog.Config` property must be initialized, either with its default constructor, or with one of the factory methods availble via `TraceConfig.Create`.

### Setup examples:
```
TraceLog.Config = TraceConfig.Create.RelativeToProject();
```

## Logging
More info coming soon.

### Capture examples:
```
TraceLog.Capture().WithData(
    (nameof(numbersInSet), numbersInSet, typeof(int)),
    (nameof(sleepMs), sleepMs, typeof(int)),
    (nameof(startingPosition), startingPosition, typeof(int)),
    (nameof(winnerGoal), winnerGoal, typeof(int))
).Flush();
```
