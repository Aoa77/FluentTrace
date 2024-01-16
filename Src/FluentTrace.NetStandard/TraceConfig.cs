using System;
using System.Text.Json;

namespace FluentTrace.NetStandard
{
    public sealed class TraceConfig
    {
        /// <summary>
        /// Exposes a factory for creating configurations.
        /// </summary>
        public static readonly Factory Create = new Factory();

        /// <summary>
        /// Gets or sets the full path to the root directory of the compiled application.
        /// </summary>
        /// <remarks>
        /// This is used to shorten [CallerFilePath] values in log data by making them relative to the root directory.
        /// </remarks>
        public string CompiledRootDirectory { get; set; }

        /// <summary>
        /// Gets or sets the full path to the log directory.
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// Gets or sets the string representation of null values displayed in log data.
        /// </summary>
        /// <remarks>
        /// The default is "[null]".
        /// </remarks>
        public string NullDataDisplay { get; set; } = "[null]";

        /// <summary>
        /// Gets or sets the starting sequence number for log files.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item> Default/Min: 4</item>
        /// <item> Max: 260 (Based on the maximum regular filename length in Windows.)</item>
        /// </list>
        /// </remarks>
        public int SequenceLength
        {
            get => _sequenceLength;
            set
            {
                if (value < SequenceLengthMin || value > SequenceLengthMax)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(SequenceLength),
                        $"Min value is {SequenceLengthMin}. Max value is {SequenceLengthMax}");
                }
                _sequenceLength = value;
            }
        }
        private const int SequenceLengthMin = 4;
        private const int SequenceLengthMax = 260;

        /// <summary>
        /// Exposes the options used to serialize log data.
        /// </summary>
        public readonly JsonSerializerOptions Json 
            = new JsonSerializerOptions();

        private int _sequenceLength;
        public TraceConfig()
        {
            SequenceLength = SequenceLengthMin;
        }
    }
}
