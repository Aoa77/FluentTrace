internal static class Constants
{
    public const string ActiveLogFileName
    = "active.log";

    public const string DumpLogFileNameTemplate
    = "dump.{0}.log";

    public const string LogFolderName
    = "logs";

    public const string LogOutputTemplate
     = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message}{NewLine}{NewLine}";

    public const string TraceSessionErrorMessage
    = "Trace session has not been started. Call Trace.BeginSession() first.";
}