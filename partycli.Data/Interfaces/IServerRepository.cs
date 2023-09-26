using PartyCli.Models;
using System.Collections.Generic;

namespace PartyCli.Data
{
    public interface IServerRepository
    {
        void StoreValue(string name, List<ServerModel> value, bool writeToConsole = true);

        List<ServerModel> RetrieveValue(string name);
    }
}