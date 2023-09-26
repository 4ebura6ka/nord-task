using Newtonsoft.Json;
using partycli.API;
using partycli.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;

namespace partycli.Commands
{
    public class ServerListCommand : Command<ServerListCommand.Settings>
    {
        private readonly ILogger _logger;
        private readonly ServerService _serverService;
        private readonly IStorage _storage;

        public ServerListCommand(ServerService serverService, IStorage storage, ILogger logger) 
        {
            _serverService = serverService;
            _storage = storage;
            _logger = logger;
        }
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[OPTIONS]")]
            public string Options { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var param = settings.Options;

            if (string.IsNullOrWhiteSpace(settings.Options))
            {
                FetchAllServers();
                return 0;
            }

            if (param == "--local")
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.serverlist))
                {
                    DisplayList(Properties.Settings.Default.serverlist);
                }
                else
                {
                    Console.WriteLine("Error: There are no server data in local storage");
                }
                return 0;
            }

            if (Enum.TryParse<Country>(param, true, out Country country))
            {
                FetchCountryServers(country);
                return 0;
            }

            if (Enum.TryParse<Protocol>(param, true, out Protocol protocol))
            {
                FetchProtocolServer(protocol);
            }
            return 0;
        }

        private void FetchProtocolServer(Protocol protocol) 
        {
            var query = new VpnServerQuery((int)protocol, null, null, null, null, null);
            var serverList = _serverService.GetAllServerByProtocolListAsync((int)query.Protocol.Value);
            _storage.StoreValue("serverlist", serverList, false);
            _logger.Log("Saved new server list: " + serverList);
            DisplayList(serverList);
        }

        private void FetchCountryServers(Country country)
        {
            var query = new VpnServerQuery(null, (int)country, null, null, null, null);
            var serverList = _serverService.GetAllServerByCountryListAsync(query.CountryId.Value);
            _storage.StoreValue("serverlist", serverList, false);
            _logger.Log("Saved new server list: " + serverList);
            DisplayList(serverList);
        }

        private void FetchAllServers()
        {
            var serverList = _serverService.GetAllServersListAsync();
            _storage.StoreValue("serverlist", serverList, false);
            _logger.Log("Saved new server list: " + serverList);
            DisplayList(serverList);
        }

        public void DisplayList(string serverListString)
        {
            var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
            Console.WriteLine("Server list: ");
            for (var index = 0; index < serverlist.Count; index++)
            {
                AnsiConsole.MarkupLine($"Name: [blue]{serverlist[index].Name}[/]");
            }
            Console.WriteLine("Total servers: " + serverlist.Count);
        }
    }
}
