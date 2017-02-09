using System;

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

            var atcsLogFileManager = new LogFileManager(logDirectory);
            //atcsLogFileManager.ProcessDirectory();
            atcsLogFileManager.GetATCSLogs();
        }
    }
}
