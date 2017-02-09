using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using atcsLogManager;

namespace atcsLogManagerTests
{
    [TestClass]
    public class LogFileManagerTests
    {
        [TestMethod]
        //[ExpectedException(typeof(NotImplementedException))]
        public void TestGetATCSLogs()
        {
            //C:\Users\erben\Dropbox\programming\atcsLogManager\testData
            var atcsLogManager = new LogFileManager(null);
            Assert.ThrowsException<NotImplementedException>(() => atcsLogManager.GetATCSLogs());
        }
    }
}
