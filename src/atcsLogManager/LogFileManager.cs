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

        public void GetATCSLogs()
        {
            throw new NotImplementedException();
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
