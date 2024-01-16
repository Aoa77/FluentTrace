namespace FluentTrace.NetStandard
{
    public sealed class CallSite
    {
        public string File { get; }
        public string Func { get; }
        public int Line { get; }

        internal CallSite(string file, string func, int line)
        {
            File = file;
            Func = func;
            Line = line;
        }
    }
}
