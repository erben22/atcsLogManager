using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text.RegularExpressions;

namespace atcsLogManager
{
    /// <summary>
    ///     Handler for ATCS log files.
    /// </summary>
    public class ATCSLogFile
    {
        private string filePath;
        private ATCSSettings settings;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="path">Fully qualified path to an ATCS log file.</param>
        public ATCSLogFile(string path)
        {
            filePath = path;

            // Attempt to obtain settings from a persisted settings file.
            // If the settings do not exist, create a default settings object
            // and use that.

            try
            {
                settings = ATCSSettingsSerializer.Deserialize();
            }
            catch (FileNotFoundException)
            {
                settings = new ATCSSettings();
            }
        }

        /// <summary>
        ///     Property for the name of the log file sans extension.
        /// </summary>
        public string FileNameWOExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(filePath);
            }
        }

        /// <summary>
        ///     Property for the filename of the log file without the full path.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(filePath);
            }
        }

        /// <summary>
        ///     Property for the log file's directory.
        /// </summary>
        public string FileDirectory
        {
            get
            {
                return Path.GetDirectoryName(filePath);
            }
        }

        /// <summary>
        ///     Archive the ATCS log file.
        /// </summary>
        public void ArchiveFile()
        {
            var fileNameParts = ParseLogFileName();

            // If we could not obtain the parts of the filename from our
            // parsing, bail out, not a file we want to process.

            if (fileNameParts == null)
            {
                Console.WriteLine("  Filename {0} is not a supported ATCS log file to archive.",
                    FileNameWOExtension);
                return;
            }

            // Do not process the file if the timestamp on the filename is
            // for the current day.

            if (IsFileForCurrentDay(fileNameParts))
            {
                Console.WriteLine("  Filename {0} is for the current day, not processing.",
                    FileNameWOExtension);
                return;
            }

            // Do not process the file if it is in use.

            if (IsFileInUse(filePath))
            {
                Console.WriteLine("  Filename {0} is in use, not processing.", 
                    FileNameWOExtension);
                return;
            }

            // Build up the zip file name that will be used for the log file
            // to be archived into.  This is the YYYYMM portion of the log file
            // name so all logs for a month are put into the same zip file.

            var zipFileName = FileDirectory + Path.DirectorySeparatorChar +
                fileNameParts["atcsTerritory"] + fileNameParts["year"] +
                    fileNameParts["month"] + ".zip";

            var zipArchiveMode = File.Exists(zipFileName) ? 
                ZipArchiveMode.Update : ZipArchiveMode.Create;

            using (ZipArchive zipFile = ZipFile.Open(zipFileName, zipArchiveMode))
            {
                zipFile.CreateEntryFromFile(filePath,
                    Path.GetFileName(filePath));
            }

            // TODO:  How should I verify the CreateEntryFromFile method
            // actually worked before I attempt to back it up or delete it?

            // Backup the log file or delete it once we have archived it.

            BackupLogFile();
        }

        /// <summary>
        ///     Determine if the file name being parsed (via its parts collection) is for
        ///     the current day.
        /// </summary>
        /// <param name="fileNameParts">
        ///     Dictionary that contains keys and values for
        ///     parts of a file name we are processing.
        /// </param>
        /// <returns>
        ///     Boolean indicating whether the file name parts are for
        ///     the current day.
        /// </returns>
        private bool IsFileForCurrentDay(Dictionary<string, string> fileNameParts)
        {
            return (fileNameParts["day"] == DateTime.Now.Day.ToString() &&
                fileNameParts["month"] == DateTime.Now.Month.ToString() &&
                    fileNameParts["year"] == DateTime.Now.Year.ToString() ?
                        true : false);
        }

        /// <summary>
        ///     Parse the log file name and build a dictionary of the parts.
        /// </summary>
        /// <returns>
        ///     Collection that contains keys and values for desired parts of 
        ///     an ATCS log file.  If we cannot process the log file name properly,
        ///     null is returned.
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
        ///     Backup a log file by moving it to our backup directory.
        /// </summary>
        private void BackupLogFile()
        {
            if (settings.backupLogs)
            {
                var backupDir = Path.Combine(FileDirectory, "backup");

                Directory.CreateDirectory(backupDir);

                File.Move(filePath, backupDir +
                    Path.DirectorySeparatorChar +
                        Path.GetFileName(filePath));
            }
            else
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        ///     Determine if the current file is in use and has exclusive access.  
        ///     Sometimes, the ATCS program may have a lock on the log file, 
        ///     so this method attempts to detect that and can be used to prevent 
        ///     manipulation in that case.
        /// </summary>
        /// <param name="file">Fully qualified path to a file to check.</param>
        /// <returns>Boolean indicating if the file is in use.</returns>
        private bool IsFileInUse(String file)
        {
            bool isInUse = false;

            try
            {
                using (FileStream fs = new FileStream(
                    file, FileMode.Open, FileAccess.Read))
                {
                    // We were able to open the file for reading without an
                    // exception occurring, so it is not in exclusive use.

                    isInUse = false;
                }
            }
            catch (IOException)
            {
                isInUse = true;
            }

            return isInUse;
        }
    }
}
