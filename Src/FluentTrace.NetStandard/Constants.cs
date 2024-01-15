namespace FluentTrace.NetStandard
{
    internal static class Constants
    {
        public const string ActiveLog
    = "active.log";

        public const string DumpLog
    = "dump.{0}.log";

        public const string LogFolder
    = "logs";

        public const string LogTemplate
     = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message}{NewLine}{NewLine}";

        public const string NullData
        = "{null}";

        public const string SessionError
    = "Trace session has not been started. Call Trace.BeginSession() first.";
    }
}