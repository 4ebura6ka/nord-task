using Newtonsoft.Json;
using partycli.API;
using partycli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SimpleInjector;

namespace partycli
{
    static class Program
    {
        static readonly Container container;

        static Program()
        {
            container = new Container();
            container.Register<ServerService>(Lifestyle.Scoped);
            container.Register<ILogger, Logger>(Lifestyle.Scoped);
            container.Register<IStorage, Storage>(Lifestyle.Singleton);

            container.Verify();
        }

        static void Main(string[] args)
        {
            var currentState = State.None;
            var name = string.Empty;
            var argIndex = 1;

            var logger = container.GetInstance<Logger>();
            var serverService = container.GetInstance<ServerService>();
            var storage = container.GetInstance<Storage>();

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
                                    storage.StoreValue("serverlist", serverList, false);
                                    logger.Log("Saved new server list: " + serverList);
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
                                storage.StoreValue(ProccessName(name), arg);
                                logger.Log("Changed " + ProccessName(name) + " to " + arg);
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
                                storage.StoreValue("serverlist", serverList, false);
                                logger.Log("Saved new server list: " + serverList);
                                DisplayList(serverList);
                            }
                            else if (arg == "--TCP")
                            {
                                //UDP = 3
                                //Tcp = 5
                                //Nordlynx = 35
                                var query = new VpnServerQuery(5, null, null, null, null, null);
                                var serverList = serverService.getAllServerByProtocolListAsync((int)query.Protocol.Value);
                                storage.StoreValue("serverlist", serverList, false);
                                logger.Log("Saved new server list: " + serverList);
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


    }



}
