using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FluentTrace.NetStandard
{
    public static class TraceLog
    {
        private static TraceConfig _config;
        public static TraceConfig Config
        {
            get => _config ?? throw new InvalidOperationException(
            $"{nameof(TraceLog)}.{nameof(Config)} has not been initialized. ");
            set => _config = value;
        }

        public static int SequenceNumber { get; private set; }

        public static Capture Capture(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            return new Capture(file, func, line);
        }

        internal static void Flush(Capture capture)
        {
            if (capture.Flushed)
            {
                throw new ObjectDisposedException(nameof(capture));
            }
            var log = new StringBuilder();
            foreach (var call in capture.Stack)
            {
                log.AppendLine(FormatCallSite(call));
            }
            foreach (var data in capture.Data)
            {
                log.AppendLine(FormatData(data));
            }
            File.WriteAllText(GetNextLogFilePath(), log.ToString());
            ++SequenceNumber;
        }

        private static string FormatCallSite(CallSite call)
        {
            string file = call.File.Replace(
                Config.CompiledRootDirectory, null);

            return $@"[{call.Func}] ""{file}:line {call.Line}""";
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

        private static string GetNextLogFilePath()
        {
            if (Config.LogDirectory == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Config.LogDirectory)} is null."
                );
            }

            Directory.CreateDirectory(Config.LogDirectory);

            string filename = SequenceNumber.ToString();
            if (filename.Length > Config.SequenceLength)
            {
                throw new InvalidOperationException("Sequence number overflow.");
            }

            return Path.Combine(
                Config.LogDirectory,
                $"{filename.PadLeft(Config.SequenceLength, '0')}.log"
            );
        }
    }
}
