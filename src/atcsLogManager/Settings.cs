using System;
using System.Runtime.Serialization;

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
