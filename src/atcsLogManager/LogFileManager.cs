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
                string pattern = @"(\d{4})(\d{2})(\d{2})";
                var fileNameParts = Regex.Split(filename, pattern);

                if (fileNameParts.Length == 5)
                {
                    var atcsTerritory = fileNameParts[0];
                    var year = fileNameParts[1];
                    var month = fileNameParts[2];

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
                    }
                }
            }
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
