using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FluentTrace.NetStandard
{
    public static class CompiledTrace
    {
        private static TraceSession _session { get; set; }

        public static void BeginSession(
            string rootDirectory,
            string logDirectory,
            [CallerFilePath] string callerFilePath = null)
        {
            EndSession();
            _session = new TraceSession(rootDirectory, logDirectory);
        }

        public static void BeginSession(
            Func<DirectoryInfo, bool> rootLocator,
            string logDirectory,
            [CallerFilePath] string callerFilePath = null)
        {
            var dir = FindRootDirectory(rootLocator, callerFilePath);
            if (dir == null)
            {
                throw new DirectoryNotFoundException(callerFilePath);
            }
            BeginSession(dir.FullName, logDirectory);
        }

        private static DirectoryInfo FindRootDirectory(
            Func<DirectoryInfo, bool> rootDirectoryLocator,
            string callerFilePath)
        {
            var dir = new FileInfo(callerFilePath).Directory;
            while (dir != null)
            {
                if (!rootDirectoryLocator(dir))
                {
                    dir = dir.Parent;
                    continue;
                }
                break;
            }
            return dir;
        }

        public static void EndSession()
        {
            _session = null;
            Log.CloseAndFlush();
        }

        public static CallStack Capture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            RequireActiveSession();
            return new CallStack(file, func, line);
        }

        public static void Write(CallStack caller)
        {
            RequireActiveSession();
            var log = new StringBuilder();
            foreach (var frame in caller.Stack)
            {
                log.AppendLine(FormatRecord(_session.RootDirectory, frame));
            }
            foreach (var data in caller.Data)
            {
                log.AppendLine(FormatData(data));
            }
            Log.Debug(log.ToString());
        }

        private static void RequireActiveSession()
        {
            if (_session == null)
            {
                throw new InvalidOperationException(Constants.SessionError);
            }
        }

        private static string FormatData(TraceData data)
        {
            var value = data.Value.ToString();
            if (data.ParamType == typeof(string))
            {
                value = $@"""{value}""";
            }
            return $@"{data.Prefix}[{data.TypeName}] {data.ParamName}: {value}";
        }

        private static string FormatRecord(string root, CallSiteRecord record)
        {
            string file = record.File.Replace(root, null);
            return $@"[{record.Func}] ""{file}:line {record.Line}""";
        }
    }
}
