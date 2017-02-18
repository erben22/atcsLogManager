using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Reflection;

namespace atcsLogManager
{
    [DataContract]
    public class ATCSSettings
    {
        [DataMember]
        internal Boolean backupLogs;

        public void SetDefaults()
        {
            this.backupLogs = false;
        }

        public void Serialize()
        {
            var settingsFilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

            using (FileStream settingsFile = new FileStream(
                settingsFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (settingsFile != null)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ATCSSettings));
                    serializer.WriteObject(settingsFile, this);
                }
            }
        }

        public static ATCSSettings Deserialize()
        {
            var settingsFilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

            using (FileStream settingsFile = new FileStream(
                settingsFilePath, FileMode.Open, FileAccess.Read))
            {
                if (settingsFile != null)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ATCSSettings));
                    return (ATCSSettings)serializer.ReadObject(settingsFile);
                }
            }

            return null;
        }

    }
}
