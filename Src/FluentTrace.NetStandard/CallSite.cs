using System.IO;
using System.Runtime.CompilerServices;

namespace FluentTrace.NetStandard
{
    public sealed class CallSite
    {
        public string File { get; }
        public string Func { get; }
        public int Line { get; }

        public static CallSite CaptureCurrent(
            [CallerFilePath] string file = null,
            [CallerMemberName] string func = null,
            [CallerLineNumber] int line = 0
        )
        {
            return new CallSite(file, func, line);
        }

        public DirectoryInfo GetDirectoryInfo()
        {
            return new FileInfo(File).Directory;
        }

        internal CallSite(string file, string func, int line)
        {
            File = file;
            Func = func;
            Line = line;
        }
    }
}
