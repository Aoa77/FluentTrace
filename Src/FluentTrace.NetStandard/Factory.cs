using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FluentTrace.NetStandard
{
    public sealed class Factory
    {
        private const string DefaultLogFolderName = "logs";
        internal Factory() { }

        /// <summary>
        /// Creates a configuration relative to the calling file. Sets root path and log directory relative to the first matching [*.csproj] project file.
        /// </summary>
        /// <remarks>
        /// Default folder name is "logs".
        /// </remarks>
        public TraceConfig RelativeToProject(
            string logFolderName = DefaultLogFolderName,
            [CallerFilePath] string callerFilePath = null)
        {
            return RelativeToFileSystemInfo(
                "*.csproj", logFolderName, callerFilePath);
        }
        
        /// <summary>
        /// Creates a configuration relative to the calling file. Sets root path and log directory relative to the first matching [*.sln] solution file.
        /// </summary>
        /// <remarks>
        /// Default folder name is "logs".
        /// </remarks>
        public TraceConfig RelativeToSolution(
            string logFolderName = DefaultLogFolderName,
            [CallerFilePath] string callerFilePath = null)
        {
            return RelativeToFileSystemInfo(
                "*.sln", logFolderName, callerFilePath);
        }
        
        /// <summary>
        /// Creates a configuration relative to the calling file. Sets root path and log directory relative to the first matching "lookFor" file or directory.
        /// </summary>
        /// <remarks>
        /// Default folder name is "logs".
        /// </remarks>
        public TraceConfig RelativeToFileSystemInfo(
            string lookFor,
            string logFolderName = DefaultLogFolderName,
            [CallerFilePath] string callerFilePath = null)
        {
            DirectoryInfo dir = new FileInfo(callerFilePath).Directory;
            while (dir != null)
            {
                if (dir.GetFileSystemInfos(lookFor).Any())
                {
                    return new TraceConfig
                    {
                        CompiledRootDirectory = dir.FullName,
                        LogDirectory = Path.Combine(dir.FullName, logFolderName)
                    };
                }
                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException(
                "Unable to locate a suitable root directory.");
        }
    }
}
