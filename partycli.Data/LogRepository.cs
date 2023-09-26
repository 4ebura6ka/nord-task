using System;
using PartyCli.Data;

namespace PartyCli
{
    public class LogRepository : ILogRepository
    {
        public void StoreValue(string name, string value, bool writeToConsole = true)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                settings[name] = value;
                settings.Save();
                if (writeToConsole)
                {
                    Console.WriteLine($"Changed {name} to {value}");
                }
            }
            catch
            {
                Console.WriteLine($"Error: Couldn't save {name}. Check if command was input correctly.");
            }
        }

        public string RetrieveValue(string name)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                var settingValue = settings[name];
                return (string)settingValue;
            }
            catch
            {
                Console.WriteLine($"Error: Couldn't return value of {name}. Check if command was input correctly.");
            }

            return string.Empty;
        }
    }
}

