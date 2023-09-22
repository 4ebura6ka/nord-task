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
            var currentState = State.None;
            var name = string.Empty;
            var argIndex = 1;

            var serverService = new ServerService();

            foreach (var arg in args)
            {
                switch (currentState)
                {
                    case State.None:
                        {
                            if (arg == "server_list")
                            {
                                currentState = State.ServerList;
                                if (argIndex >= args.Count())
                                {
                                    var serverList = serverService.getAllServersListAsync();
                                    StoreValue("serverlist", serverList, false);
                                    Log("Saved new server list: " + serverList);
                                    DisplayList(serverList);
                                }
                            }
                            if (arg == "config")
                            {
                                currentState = State.Config;
                            }
                        }; break;

                    case State.Config:
                        {
                            if (name == null)
                            {
                                name = arg;
                            }
                            else
                            {
                                StoreValue(ProccessName(name), arg);
                                Log("Changed " + ProccessName(name) + " to " + arg);
                                name = null;
                            }
                        }; break;
                    case State.ServerList:
                        {
                            if (arg == "--local")
                            {
                                if (!String.IsNullOrEmpty(Properties.Settings.Default.serverlist))
                                {
                                    DisplayList(Properties.Settings.Default.serverlist);
                                }
                                else
                                {
                                    Console.WriteLine("Error: There are no server data in local storage");
                                }
                            }
                            else if (arg == "--france")
                            {
                                //france == 74
                                //albania == 2
                                //Argentina == 10
                                var query = new VpnServerQuery(null, 74, null, null, null, null);
                                var serverList = serverService.getAllServerByCountryListAsync(query.CountryId.Value); //France id == 74
                                StoreValue("serverlist", serverList, false);
                                Log("Saved new server list: " + serverList);
                                DisplayList(serverList);
                            }
                            else if (arg == "--TCP")
                            {
                                //UDP = 3
                                //Tcp = 5
                                //Nordlynx = 35
                                var query = new VpnServerQuery(5, null, null, null, null, null);
                                var serverList = serverService.getAllServerByProtocolListAsync((int)query.Protocol.Value);
                                StoreValue("serverlist", serverList, false);
                                Log("Saved new server list: " + serverList);
                                DisplayList(serverList);
                            }
                        }; break;

                }
                argIndex++;
            }

            if (currentState == State.None)
            {
                Console.WriteLine("To get and save all servers, use command: partycli.exe server_list");
                Console.WriteLine("To get and save France servers, use command: partycli.exe server_list --france");
                Console.WriteLine("To get and save servers that support TCP protocol, use command: partycli.exe server_list --TCP");
                Console.WriteLine("To see saved list of servers, use command: partycli.exe server_list --local ");
            }
            Console.Read();
        }

        static void StoreValue(string name, string value, bool writeToConsole = true)
        {
            try
            {
                var settings = Properties.Settings.Default;
                settings[name] = value;
                settings.Save();
                if (writeToConsole)
                {
                    Console.WriteLine("Changed " + name + " to " + value);
                }
            }
            catch
            {
                Console.WriteLine("Error: Couldn't save " + name + ". Check if command was input correctly.");
            }

        }

        static string ProccessName(string name)
        {
            name = name.Replace("-", string.Empty);
            return name;
        }


        static void DisplayList(string serverListString)
        {
            var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
            Console.WriteLine("Server list: ");
            for (var index = 0; index < serverlist.Count; index++)
            {
                Console.WriteLine("Name: " + serverlist[index].Name);
            }
            Console.WriteLine("Total servers: " + serverlist.Count);
        }

        static void Log(string action)
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

            StoreValue("log", JsonConvert.SerializeObject(currentLog), false);
        }
    }



}
