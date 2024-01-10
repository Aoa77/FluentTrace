namespace FluentTrace.NetStandard
{
    public sealed class Frame
    {
        public string File { get; }
        public string Func { get; }
        public int Line { get; }

        public Frame(string file, string func, int line)
        {
            File = file;
            Func = func;
            Line = line;
        }
    }
}
