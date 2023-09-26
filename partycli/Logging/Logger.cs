using System;
using Newtonsoft.Json;
using partycli.Models;
using System.Collections.Generic;

namespace partycli
{
    public class Logger : ILogger
    {
        private readonly IStorage _storage;

        public Logger(IStorage storage)
        {
            _storage = storage;
        }

        public void Log(string action)
        {
            var newLog = new LogModel
            {
                Action = action,
                Time = DateTime.Now
            };

            List<LogModel> currentLog;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.log))
            {
                try
                {
                    currentLog = JsonConvert.DeserializeObject<List<LogModel>>(Properties.Settings.Default.log);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something weird stored in logs:{ex.Message}. Re-writing.");
                    currentLog = new List<LogModel>();
                }
                currentLog.Add(newLog);
            }
            else
            {
                currentLog = new List<LogModel> { newLog };
            }

            _storage.StoreValue("log", JsonConvert.SerializeObject(currentLog), false);
        }
    }
}

