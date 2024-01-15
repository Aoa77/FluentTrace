namespace FluentTrace.NetStandard
{
    public sealed class CallSiteRecord
    {
        public string File { get; }
        public string Func { get; }
        public int Line { get; }

        internal CallSiteRecord(string file, string func, int line)
        {
            File = file;
            Func = func;
            Line = line;
        }
    }
}
