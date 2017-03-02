using System;
using System.Runtime.Serialization;

namespace atcsLogManager
{
    /// <summary>
    ///     Settings class for the program.  Decorated as a DataContract
    ///     to allow for serialization/deserialization.
    /// </summary>
    [DataContract]
    public class ATCSSettings
    {
        /// <summary>
        ///     Boolean flag used to indicate whether or not we should backup
        ///     a log file once we have successfully archived it.  Defaults to
        ///     false.
        /// </summary>
        [DataMember]
        internal Boolean backupLogs = false;
    }
}
