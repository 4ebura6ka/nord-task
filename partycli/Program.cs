using Newtonsoft.Json;
using partycli.API;
using partycli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace partycli
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentState = States.none;
            string name = null;
            int argIndex = 1;

            ServerService serverService = new ServerService();

            foreach (string arg in args)
            {
                if (currentState == States.none)
                {
                    if (arg == "server_list")
                    {
                        currentState = States.server_list;
                        if (argIndex >= args.Count())
                        {
                            var serverList = serverService.getAllServersListAsync();
                            storeValue("serverlist", serverList, false);
                            log("Saved new server list: " + serverList);
                            displayList(serverList);
                        }
                    }
                    if (arg == "config")
                    {
                        currentState = States.config;
                    }
                }
                else if (currentState == States.config)
                {
                    if (name == null)
                    {
                        name = arg;
                    }
                    else
                    {
                        storeValue(proccessName(name), arg);
                        log("Changed " + proccessName(name) + " to " + arg);
                        name = null;
                    }
                }
                else if (currentState == States.server_list)
                {
                    if (arg == "--local")
                    {
                        if (!String.IsNullOrEmpty(Properties.Settings.Default.serverlist)) { 
                        displayList(Properties.Settings.Default.serverlist);
                        } else
                        {
                            Console.WriteLine("Error: There are no server data in local storage");
                        }
                    }
                    else if (arg == "--france")
                    {
                        //france == 74
                        //albania == 2
                        //Argentina == 10
                        var query = new VpnServerQuery(null,74,null,null,null, null);
                        var serverList = serverService.getAllServerByCountryListAsync(query.CountryId.Value); //France id == 74
                        storeValue("serverlist", serverList, false);
                        log("Saved new server list: " + serverList);
                        displayList(serverList);
                    }
                    else if (arg == "--TCP")
                    {
                        //UDP = 3
                        //Tcp = 5
                        //Nordlynx = 35
                        var query = new VpnServerQuery(5,null,null,null,null, null);
                        var serverList = serverService.getAllServerByProtocolListAsync((int)query.Protocol.Value);
                        storeValue("serverlist", serverList, false);
                        log("Saved new server list: " + serverList);
                        displayList(serverList);
                    }
                }
                argIndex = argIndex + 1;
            }

            if(currentState == States.none)
            {
                Console.WriteLine("To get and save all servers, use command: partycli.exe server_list");
                Console.WriteLine("To get and save France servers, use command: partycli.exe server_list --france");
                Console.WriteLine("To get and save servers that support TCP protocol, use command: partycli.exe server_list --TCP");
                Console.WriteLine("To see saved list of servers, use command: partycli.exe server_list --local ");
            }
            Console.Read();
        }

        static void storeValue(string name, string value, bool writeToConsole = true)
        {
            try { 
                var settings = Properties.Settings.Default;
                settings[name] = value;
                settings.Save();
                if (writeToConsole) { 
                Console.WriteLine("Changed " + name + " to " + value);
                }
            }
            catch {
                Console.WriteLine("Error: Couldn't save " + name + ". Check if command was input correctly." );
            }

        }

        static string proccessName(string name)
        {
            name = name.Replace("-", string.Empty);
            return name;
        }

 
        static void displayList(string serverListString)
        {
            var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
            Console.WriteLine("Server list: ");
            for (var index = 0; index < serverlist.Count; index++)
            {
                Console.WriteLine("Name: " + serverlist[index].Name);
            }
            Console.WriteLine("Total servers: " + serverlist.Count);
        }

        static void log(string action)
        {
            var newLog = new LogModel
            {
                Action = action,
                Time = DateTime.Now
            };
            List<LogModel> currentLog;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.log))
            {
                currentLog = JsonConvert.DeserializeObject<List<LogModel>>(Properties.Settings.Default.log);
                currentLog.Add(newLog);
            }
            else
            {
                currentLog = new List<LogModel> { newLog };
            }

            storeValue("log", JsonConvert.SerializeObject(currentLog), false);
        }
    }

    internal class VpnServerQuery
    {
             public int? Protocol { get; set; }

            public int? CountryId { get; set;}

            public int? CityId { get; set;}

            public int? RegionId { get; set;}

            public int? SpecificServcerId { get; set;}

            public int? ServerGroupId { get; set;}

            public VpnServerQuery(int? protocol, int? countryId, int? cityId, int? regionId, int? specificServcerId, int? serverGroupId)
            {
                Protocol = protocol;
                CountryId = countryId;
                CityId = cityId;
                RegionId = regionId;
                SpecificServcerId = specificServcerId;
                ServerGroupId = serverGroupId;
            }
    }

}
