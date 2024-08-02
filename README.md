# FluentTrace
C# fluent-style extension methods for trace-logging call stack information via the System.Runtime.CompilerServices API.

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

### Quick demo:

Below is a quick video demonstrating trace logging in a simple C# console application.

https://github.com/Aoa77/FluentTrace/assets/4643190/3f6fd84a-0ad0-4a6a-85ff-5dc624b9ccf8


