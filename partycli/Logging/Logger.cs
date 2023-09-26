using Newtonsoft.Json;
using PartyCli.Data;
using PartyCli.Models;
using System;
using System.Collections.Generic;

namespace PartyCli
{
    public class Logger : ILogger
    {
        private readonly ILogRepository _storage;

        public Logger(ILogRepository storage)
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
            var storedLog = _storage.RetrieveValue("log");
            if (!string.IsNullOrEmpty(storedLog))
            {
                try
                {
                    currentLog = JsonConvert.DeserializeObject<List<LogModel>>(storedLog);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something weird stored in logs:{ex.Message}.Re-writing.");
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

