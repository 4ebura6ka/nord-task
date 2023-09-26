using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PartyCli.Models;

namespace PartyCli.Data
{
    public class LogRepository : ILogRepository
    {
        public void StoreValue(string name, List<LogModel> log, bool writeToConsole = true)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                settings[name] = JsonConvert.SerializeObject(log);
                settings.Save();
                if (writeToConsole)
                {
                    Console.WriteLine($"Changed {name} to {settings[name]}");
                }
            }
            catch
            {
                Console.WriteLine($"Error: Couldn't save {name}. Check if command was input correctly.");
            }
        }

        public List<LogModel> RetrieveValue(string name)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                var settingValue = settings[name].ToString();
                List<LogModel> record;

                try
                {
                    record = JsonConvert.DeserializeObject<List<LogModel>>(settingValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something weird stored in logs:{ex.Message}.Re-writing.");
                    record = new List<LogModel>();
                }
                return record;
            }
            catch
            {
                Console.WriteLine($"Error: Couldn't return value of {name}. Check if command was input correctly.");
            }

            return null;
        }
    }
}

