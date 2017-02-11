using System;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace atcsLogManager
{
    public class LogFileManager
    {
        private string atcsDirectory;

        public LogFileManager(string directory)
        {
            atcsDirectory = directory;
        }

        public void GetATCSLogs()
        {
            var files = System.IO.Directory.GetFiles(atcsDirectory, "*.log");
            foreach (var file in files)
            {
                var filename = System.IO.Path.GetFileNameWithoutExtension(file);
                var directory = System.IO.Path.GetDirectoryName(file);

                Console.WriteLine("File is: {0}", filename);
                string pattern = @"(\d{4})(\d{2})(\d{2})$";
                var fileNameParts = Regex.Split(filename, pattern);

                if (fileNameParts.Length == 5)
                {
                    var atcsTerritory = fileNameParts[0];
                    var year = fileNameParts[1];
                    var month = fileNameParts[2];
                    var day = fileNameParts[3];

                    if (day == DateTime.Now.Day.ToString())
                    {
                        Console.WriteLine("  Filename {0} is for the current day, not processing", filename);
                        continue;
                    }

                    if (IsFileInUse(file))
                    {
                        Console.WriteLine("  Filename {0} is in use, not processing", filename);
                        continue;
                    }

                    var zipFileName = directory + System.IO.Path.DirectorySeparatorChar + 
                    atcsTerritory + year + month + ".zip";

                    if (System.IO.File.Exists(zipFileName))
                    {
                        Console.WriteLine("  zipFileName ({0}) exists", zipFileName);

                        using (ZipArchive zipFile = ZipFile.Open(
                                zipFileName, ZipArchiveMode.Update))
                        {
                            zipFile.CreateEntryFromFile(file, 
                                System.IO.Path.GetFileName(file));
                        }
                    }
                    else
                    {
                        Console.WriteLine("  zipFileName ({0}) does not exist", zipFileName);

                        using (ZipArchive zipFile = ZipFile.Open(
                                zipFileName, ZipArchiveMode.Create))
                        {
                            zipFile.CreateEntryFromFile(file,
                                System.IO.Path.GetFileName(file));
                        }
                    }

                    var backupDir = System.IO.Path.Combine(directory, "backup");

                    System.IO.Directory.CreateDirectory(backupDir);

                    System.IO.File.Move(file, backupDir + 
                        System.IO.Path.DirectorySeparatorChar + 
                            System.IO.Path.GetFileName(file));
                }
            }
        }

        protected virtual bool IsFileInUse(String file)
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

        public void ProcessDirectory()
        {
            var files = System.IO.Directory.GetFiles(atcsDirectory, "*.zip");

            foreach (var file in files)
            {
                Console.WriteLine("  File in directory: {0}", file.ToString());

                /*var zipFile = System.IO.Compression.ZipFile.Open(
                    file.ToString(), System.IO.Compression.ZipArchiveMode.Update);

                foreach (var entry in zipFile.Entries)
                {
                    Console.WriteLine("    Zip file entries: {0}", entry.ToString());
                }*/

                using (System.IO.FileStream zipToOpen =
                       new System.IO.FileStream(file, System.IO.FileMode.Open))
                {
                    using (System.IO.Compression.ZipArchive archive =
                           new System.IO.Compression.ZipArchive(zipToOpen,
                                                                System.IO.Compression.ZipArchiveMode.Update))
                    {
                        System.IO.Compression.ZipArchiveEntry readmeEntry = archive.CreateEntry("Readme.txt");
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(readmeEntry.Open()))
                        {
                            writer.WriteLine("Information about this package.");
                            writer.WriteLine("========================");
                        }
                    }
                }
            }
        }
    }
}
