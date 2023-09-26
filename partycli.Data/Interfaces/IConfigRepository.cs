using System;

namespace PartyCli.Data
{
    public interface IConfigRepository
    {
        void StoreValue(string name, string value, bool writeToConsole = true);

        string RetrieveValue(string name);
    }
}