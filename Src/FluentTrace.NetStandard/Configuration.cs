namespace FluentTrace.NetStandard
{
    public sealed class Configuration
    {
        /// <summary>
        /// Full path to the root directory of the compiled application.
        /// </summary>
        /// <remarks>
        /// This is used to shorten the full path of the source file.
        /// </remarks>
        public string CompiledRootDirectory { get; set; }

        /// <summary>
        /// Full path to the log directory.
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// The string representation of null values displayed in log data.
        /// </summary>
        /// <remarks>
        /// The default is "[null]".
        /// </remarks>
        public string NullDataDisplay { get; set; } = "[null]";

        /// <summary>
        /// The stgarting sequence number for log files.
        /// </summary>
        /// <remarks>
        /// The default is 1000.
        /// </remarks>
        public int SequenceStart { get; set; } = 1000;
    }
}
