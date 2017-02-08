using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace atcsLogManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var logDirectory = String.Empty;

            if (args.Length >= 1)
            {
                logDirectory = args[0];
            }
            else
            {
                Console.WriteLine("A log directory is expected.");
                return;
            }

            if (System.IO.Directory.Exists(logDirectory))
            {
                Console.WriteLine("Directory {0} exists.", logDirectory);
            }
            else
            {
                Console.WriteLine("Directory {0} does not exist.", logDirectory);
                return;
            }

            var files = System.IO.Directory.GetFiles(logDirectory, "*.zip");

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
