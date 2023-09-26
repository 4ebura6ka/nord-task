using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PartyCli.Data;
using PartyCli.Models;

namespace PartyCli
{
    public class ServerRepository : IServerRepository, IDisposable
    {
         public void StoreValue(string name, List<ServerModel> servers, bool writeToConsole = true)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                settings[name] = JsonConvert.SerializeObject(servers);
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

        public List<ServerModel> RetrieveValue(string name)
        {
            try
            {
                var settings = partycli.Data.Properties.Settings.Default;
                var settingValue = settings[name].ToString();
                var servers = JsonConvert.DeserializeObject<List<ServerModel>>(settingValue);

                return servers;
            }
            catch
            {
                Console.WriteLine($"Error: Couldn't return value of {name}. Check if command was input correctly.");
            }

            return null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

