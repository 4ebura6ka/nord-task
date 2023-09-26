using System;

namespace PartyCli.Data
{
    public interface IServerRepository
    {
        void StoreValue(string name, string value, bool writeToConsole = true);

        string RetrieveValue(string name);
    }
}