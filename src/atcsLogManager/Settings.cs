using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace atcsLogManager
{
    /// <summary>
    /// Settings class for the program.  Decorated as a DataContract
    /// to allow for serialization/deserialization.
    /// </summary>
    [DataContract]
    public class ATCSSettings
    {
        [DataMember]
        internal Boolean backupLogs = false;
    }
}
