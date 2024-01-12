using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FluentTrace.NetStandard
{
    public static class Trace
    {
        private static TraceSession _session { get; set; }

        public static void BeginSession(
            string rootDirectory,
            string logDirectory = null)
        {
            EndSession();
            _session = new TraceSession(rootDirectory, logDirectory);
        }

        public static void BeginSession(
            Func<DirectoryInfo, bool> rootDirectoryLocator,
            string logDirectory = null,
            [CallerFilePath] string callerFilePath = null)
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

            if (dir == null)
            {
                throw new DirectoryNotFoundException(callerFilePath);
            }

            BeginSession(
                rootDirectory: dir.FullName,
                logDirectory: logDirectory);
        }

        public static void EndSession()
        {
            _session = null;
            Log.CloseAndFlush();
        }

        public static Call Capture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            RequireActiveSession();
            return new Call(file, func, line);
        }

        public static void Write(Call caller)
        {
            RequireActiveSession();
            var log = new StringBuilder();
            foreach (var frame in caller.Stack)
            {
                log.AppendLine(FormatFrame(_session.RootDirectory, frame));
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
                throw new InvalidOperationException(Constants.TraceSessionErrorMessage);
            }
        }

        private static string FormatData(Data data)
        {
            var value = data.Value.ToString();
            if (data.ParamType == typeof(string))
            {
                value = $@"""{value}""";
            }
            return $@"{data.Prefix}[{data.TypeName}] {data.ParamName}: {value}";
        }

        private static string FormatFrame(string root, Frame site)
        {
            string file = site.File.Replace(root, null);
            return $@"[{site.Func}] ""{file}:line {site.Line}""";
        }
    }
}
