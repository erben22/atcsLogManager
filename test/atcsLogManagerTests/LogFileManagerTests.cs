using System.Collections.Generic;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using atcsLogManager;

namespace atcsLogManagerTests
{
    [TestClass]
    public class LogFileManagerTests
    {
        private System.IO.DirectoryInfo testDirectory;

        /// <summary>
        ///     Initialize our test environment before each test.  For this
        ///     test class, create a directory and files we can use to exercise
        ///     the LogFileManager class.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Create a directory with our desired test files in it.

            testDirectory = new System.IO.DirectoryInfo(
                System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                    System.IO.Path.GetRandomFileName()));

            testDirectory.Create();
            CreateTestLogFiles();
            CreateTestZipFiles();
        }

        /// <summary>
        ///     Using the test directory, create some known log files in the directory
        ///     for our tests to operate on.
        /// </summary>
        private void CreateTestLogFiles()
        {
            // List of files to create in the test Directory.
            //  - log file we don't want to archive
            //  - log file we want to archive into an existing zip.
            //  - log file we want to archive into a non-existent zip.
            //  - another log file we want to archive into a non-existent zip
            //    that will be created processing a previous file.
            //  - log file we want to archive into a non-existent zip number two.

            // Dictionary of filename mapping to file contents.

            Dictionary<string, string> testLogFiles = new Dictionary<string, string>()
            {
                {"20170102-UP-Geneva-Sub.log", "Replay=2016/07/05 01:09:27 6738303236303138363007F2180000B2BFF6"},
                {"UP Omaha Server20170132.log", "Replay=2017/02/01 00:00:19 6738303236383030373007FC0900012192F6"},
                {"UP Omaha Server20170201.log", "Replay=2017/02/01 00:00:19 6738303236383030373007FC0900012192F6"},
                {"UP Omaha Server20170202.log", "Replay=2017/02/02 00:00:07 6738303236383137393007FC130200C0F5F6"},
                {"UP Nampa TestLog20170201.log", "Replay=2017/02/01 00:00:19 6738303236383030373007FC0900012192F6"}
            };

            foreach(var logFile in testLogFiles)
            {
                using (System.IO.StreamWriter file = System.IO.File.CreateText(
                    System.IO.Path.Combine(testDirectory.FullName, logFile.Key)))
                {
                    file.Write(logFile.Value);
                }
            }
        }

        /// <summary>
        /// Using the test directory, create some known zip files in the directory
        /// for our tests to operate on.
        /// </summary>
        private void CreateTestZipFiles()
        {
            List<string> testZipFiles = new List<string>()
            {
                "UP Omaha Server201701.zip"
            };

            foreach (var zipFile in testZipFiles)
            {
                ZipArchive file = ZipFile.Open(
                    System.IO.Path.Combine(testDirectory.FullName, zipFile), 
                        ZipArchiveMode.Create);
                file.Dispose();
            }
        }

        /// <summary>
        ///     After completion of tests, cleanup any data we created and may have
        ///     mucked with for the test execution, leaving the slate clean for 
        ///     the next test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            // Remove the test directory we created for the current test.

            System.IO.Directory.Delete(testDirectory.FullName, true);
        }

        /// <summary>
        ///     Test the ProcessDirectory method with a valid directory with ATCS
        ///     log files in it.
        /// </summary>
        [TestMethod]
        public void TestProcessDirectorySucceeds()
        {
            var atcsLogFileManager = new LogFileManager(testDirectory.FullName);
            atcsLogFileManager.ProcessDirectory();

            // After execution of ProcessDirectory on our testDirectory, we
            // should have 1 log file and 3 zip files.

            var logFiles = testDirectory.GetFiles("*.log");
            var zipFiles = testDirectory.GetFiles("*.zip");

            Assert.AreEqual(1, logFiles.Length);
            Assert.AreEqual(3, zipFiles.Length);
        }

        /// <summary>
        ///     Test the ProcessDirectory method with an invalid directory and ensure
        ///     it throws an exception.
        /// </summary>
        [TestMethod]
        public void TestProcessDirectoryFailsNonExistentDirectory()
        {
            string nonExistantPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                    System.IO.Path.GetRandomFileName());

            Assert.IsFalse(System.IO.Directory.Exists(nonExistantPath));

            var atcsLogFileManager = new LogFileManager(nonExistantPath);
            Assert.ThrowsException<System.IO.DirectoryNotFoundException>(
                () => atcsLogFileManager.ProcessDirectory());
        }
    }
}
