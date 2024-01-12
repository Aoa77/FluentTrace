using Serilog;
using System;
using System.IO;

namespace FluentTrace.NetStandard
{
    public sealed class TraceSession
    {
        public string ActiveLogFile { get; }
        public string LogDirectory { get; }
        public string RootDirectory { get; }

        internal TraceSession(string rootDirectory, string logDirectory)
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

        private static string GetDefaultLogDirectory(string rootDirectory)
        {
            return Path.Combine(rootDirectory, Constants.LogFolderName);
        }
    }
}
