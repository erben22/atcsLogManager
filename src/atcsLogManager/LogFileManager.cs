using System;

namespace atcsLogManager
{
    public class LogFileManager
    {
        private string atcsDirectory;

        public LogFileManager(string directory)
        {
            atcsDirectory = directory;
        }

        public void ProcessDirectory()
        {
            var files = GetATCSLogs();

            foreach (var file in files)
            {
                var atcsLogFile = new ATCSLogFile(file);
                atcsLogFile.ArchiveFile();
            }
        }

        private string[] GetATCSLogs()
        {
            const string logFileSearchPattern = "*.log";
            return (System.IO.Directory.GetFiles(atcsDirectory, logFileSearchPattern));
        }
    }
}
