using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FluentTrace.NetStandard
{
    public sealed class Trace
    {
        public static Call Capture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            return new Call(file, func, line);
        }

        public static Trace Session { get; private set; }

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

        public static void BeginSession(
            string rootDirectory,
            string logDirectory = null)
        {
            EndSession();
            Session = new Trace(rootDirectory, logDirectory);
        }

        public static void EndSession()
        {
            Log.CloseAndFlush();
            Thread.Sleep(1000);
            Session = null;
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

        public static void Write(Call caller)
        {
            var log = new StringBuilder();
            foreach (var frame in caller.Stack)
            {
                log.AppendLine(FormatFrame(Session.RootDirectory, frame));
            }
            foreach (var data in caller.Data)
            {
                log.AppendLine(FormatData(data));
            }
            Log.Debug(log.ToString());
        }

        public string ActiveLogFile { get; }
        public string LogDirectory { get; }
        public string RootDirectory { get; }

        private Trace(string rootDirectory, string logDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException(rootDirectory);
            }

            logDirectory = logDirectory ?? GetDefaultLogDirectory(rootDirectory);
            Directory.CreateDirectory(logDirectory).Refresh();

            RootDirectory = rootDirectory;
            LogDirectory = logDirectory;
            ActiveLogFile = Path.Combine(LogDirectory, @"active.log");

            var file = new FileInfo(ActiveLogFile);
            if (file.Exists)
            {
                var dumpLog = Path.Combine(LogDirectory,
                    string.Format(
                        Constants.DumpLogFileNameTemplate,
                        DateTime.Now.Ticks));

                file.MoveTo(dumpLog);
            }

            var config = new LoggerConfiguration();
            config.MinimumLevel.Debug();
            config.WriteTo.Console
            (
                outputTemplate: Constants.LogOutputTemplate
            );
            config.WriteTo.File
            (
                path: ActiveLogFile,
                outputTemplate: Constants.LogOutputTemplate,
                rollingInterval: RollingInterval.Infinite
            );
            Log.Logger = config.CreateLogger();
        }

        private static class Constants
        {
            public const string ActiveLogFileName
            = "active.log";

            public const string DumpLogFileNameTemplate
            = "dump.{0}.log";

            public const string LogFolderName
            = "mx.trace.logs";

            public const string LogOutputTemplate
            = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{Message}{NewLine}{NewLine}";
        }

        private static string GetDefaultLogDirectory(string rootDirectory)
        {
            return Path.Combine(rootDirectory, Constants.LogFolderName);
        }
    }
}
