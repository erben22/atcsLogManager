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
            var atcsLogManager = new LogFileManager("C:\\Users\\erben\\Dropbox\\programming\\atcsLogManager\\testData");
            Assert.ThrowsException<NotImplementedException>(() => atcsLogManager.GetATCSLogs());
        }
    }
}
