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

            var storedLog = _storage.RetrieveValue("log");
            if (storedLog != null)
            {
                storedLog.Add(newLog);
            }
            else
            {
                storedLog = new List<LogModel> { newLog };
            }

            _storage.StoreValue("log", storedLog, false);
        }
    }
}

