using PartyCli.Models;
using System.Collections.Generic;

namespace PartyCli.Data
{
    public interface ILogRepository
    {
        void StoreValue(string name, List<LogModel> log, bool writeToConsole = true);

        List<LogModel> RetrieveValue(string name);
    }
}