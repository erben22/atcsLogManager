using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace atcsLogManager
{
    /// <summary>
    /// Serialization/Deserialization support for the ATCSSettings class.
    /// </summary>
    public class ATCSSettingsSerializer
    {
        private const string settingsFile = "settings.json";

        /// <summary>
        /// Using the AppContext.BaseDirectory property to root 
        /// us at the application directory for our settings file.
        /// </summary>
        public static string SettingsFilePath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, settingsFile);
            }
        }

        /// <summary>
        /// Serialize our data members to our storage backing.
        /// Currently configured to store to a JSON file.
        /// </summary>
        public static void Serialize(ATCSSettings atcsSettings)
        {
            using (FileStream settingsFile = new FileStream(
                SettingsFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (settingsFile != null)
                {
                    DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(ATCSSettings));
                    serializer.WriteObject(settingsFile, atcsSettings);
                }
            }
        }

        /// <summary>
        /// Consume the values for our data members from our backing store.
        /// Currently implemented to read from a JSON file.
        /// </summary>
        /// <returns>ATCSSettings instance</returns>
        public static ATCSSettings Deserialize()
        {
            using (FileStream settingsFile = new FileStream(
                SettingsFilePath, FileMode.Open, FileAccess.Read))
            {
                if (settingsFile != null)
                {
                    DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(typeof(ATCSSettings));
                    return (ATCSSettings)serializer.ReadObject(settingsFile);
                }
            }

            return null;
        }
    }
}
