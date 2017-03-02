namespace atcsLogManager
{
    /// <summary>
    ///     Manager class for processing log files.
    /// </summary>
    public class LogFileManager
    {
        private string atcsDirectory;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="directory">
        ///     Fully qualified directory path of log files to process.
        /// </param>
        public LogFileManager(string directory)
        {
            atcsDirectory = directory;
        }

        /// <summary>
        ///     Process the log directory, applying our desired options to the logs.
        ///     Right now, this consists of archiving the logs.
        /// </summary>
        public void ProcessDirectory()
        {
            var files = GetATCSLogs();

            foreach (var file in files)
            {
                var atcsLogFile = new ATCSLogFile(file);
                atcsLogFile.ArchiveFile();
            }
        }

        /// <summary>
        ///     Get all the ATCS log files from the directory.
        /// </summary>
        /// <returns>Array of log files from our directory.</returns>
        private string[] GetATCSLogs()
        {
            const string logFileSearchPattern = "*.log";
            return (System.IO.Directory.GetFiles(atcsDirectory, logFileSearchPattern));
        }
    }
}
