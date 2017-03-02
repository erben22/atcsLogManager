using System;

namespace atcsLogManager
{
    /// <summary>
    ///     Main entry for the atcsLogManager program.  Supplied with a directory,
    ///     the program will dive in and process log files it finds.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // TODO:  Look into a console application argument parsing library.
            //        This gets us up and going for the moment...

            var logDirectory = String.Empty;

            if (args.Length >= 1)
            {
                logDirectory = args[0];
            }
            else
            {
                Console.WriteLine("ERROR:  A log directory argument is expected, exiting.");
                return;
            }

            if (!System.IO.Directory.Exists(logDirectory))
            {
                Console.WriteLine("ERROR:  Directory {0} does not exist, exiting.", logDirectory);
                return;
            }

            try
            {
                var atcsLogFileManager = new LogFileManager(logDirectory);
                atcsLogFileManager.ProcessDirectory();
            }
            catch (Exception e)
            {
                // TODO:  Figure out possible exceptions and catch those here, alerting
                //        as necessary.  Eventually will put these messages into a logger
                //        instead of blasting to the console.

                Console.WriteLine("ERROR: Exception encountered processing logs - {0}", e.ToString());
            }
        }
    }
}
