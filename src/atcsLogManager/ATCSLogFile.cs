using System;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Collections.Generic;

namespace atcsLogManager
{
    /// <summary>
    /// Handler for ATCS log files.
    /// </summary>
    public class ATCSLogFile
    {
        private string filePath;
        private ATCSSettings settings = ATCSSettingsSerializer.Deserialize();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Fully qualified path to an ATCS log file.</param>
        public ATCSLogFile(string path)
        {
            filePath = path;
        }

        /// <summary>
        /// Property for the name of the log file sans extension.
        /// </summary>
        public string FileNameWOExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        /// <summary>
        /// Property for the filename of the log file without the full path.
        /// </summary>
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(filePath);
            }
        }

        /// <summary>
        /// Property for the log file's directory.
        /// </summary>
        public string FileDirectory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(filePath);
            }
        }

        /// <summary>
        /// Archive the ATCS log file.
        /// </summary>
        public void ArchiveFile()
        {
            Console.WriteLine("File is: {0}", FileNameWOExtension);
            var fileNameParts = ParseLogFileName();

            if (fileNameParts == null)
            {
                return;
            }

            if (fileNameParts["day"] == DateTime.Now.Day.ToString() &&
                fileNameParts["month"] == DateTime.Now.Month.ToString() &&
                fileNameParts["year"] == DateTime.Now.Year.ToString())
            {
                Console.WriteLine("  Filename {0} is for the current day, not processing", FileNameWOExtension);
                return;
            }

            if (IsFileInUse(filePath))
            {
                Console.WriteLine("  Filename {0} is in use, not processing", FileNameWOExtension);
                return;
            }

            var zipFileName = FileDirectory + System.IO.Path.DirectorySeparatorChar +
                fileNameParts["atcsTerritory"] + fileNameParts["year"] +
                    fileNameParts["month"] + ".zip";

            var zipArchiveMode = System.IO.File.Exists(zipFileName) ? 
                ZipArchiveMode.Update : ZipArchiveMode.Create;

            using (ZipArchive zipFile = ZipFile.Open(zipFileName, zipArchiveMode))
            {
                zipFile.CreateEntryFromFile(filePath,
                    System.IO.Path.GetFileName(filePath));
            }

            BackupLogFile();
        }

        /// <summary>
        /// Parse the log file name and build a dictionary of the parts.
        /// </summary>
        /// <returns>
        /// Collection that contains keys and values for desired parts of 
        /// an ATCS log file.  If we cannot process the log file name properly,
        /// null is returned.
        /// </returns>
        private Dictionary<string, string> ParseLogFileName()
        {
            const string pattern = @"(\d{4})(\d{2})(\d{2})$";
            var fileNameParts = Regex.Split(FileNameWOExtension, pattern);

            if (fileNameParts.Length == 5)
            {
                var logFileElements = new Dictionary<string, string>();

                logFileElements.Add("atcsTerritory", fileNameParts[0]);
                logFileElements.Add("year", fileNameParts[1]);
                logFileElements.Add("month", fileNameParts[2]);
                logFileElements.Add("day", fileNameParts[3]);

                return logFileElements;
            }

            return null;
        }

        /// <summary>
        /// Backup a log file by moving it to our backup directory.
        /// </summary>
        private void BackupLogFile()
        {
            if (settings.backupLogs)
            {
                var backupDir = System.IO.Path.Combine(FileDirectory, "backup");

                System.IO.Directory.CreateDirectory(backupDir);

                System.IO.File.Move(filePath, backupDir +
                    System.IO.Path.DirectorySeparatorChar +
                        System.IO.Path.GetFileName(filePath));
            }
            else
            {
                System.IO.File.Delete(filePath);
            }
        }

        /// <summary>
        /// Determine if the current file is in use.  Sometimes, the ATCS 
        /// program may have a lock on the log file, so this method attempts
        /// to detect that and can be used to prevent manipulation in that 
        /// case.
        /// </summary>
        /// <param name="file">Fully qualified path to a file to check.</param>
        /// <returns>Boolean indicating if the file is in use.</returns>
        private bool IsFileInUse(String file)
        {
            bool isInUse = false;

            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(
                    file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    isInUse = fs.CanWrite;
                }
            }
            catch (System.IO.IOException)
            {
                isInUse = true;
            }

            return isInUse;
        }
    }
}
