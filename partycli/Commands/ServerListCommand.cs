using Newtonsoft.Json;
using PartyCli.API;
using PartyCli.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyCli.Commands
{
    public class ServerListCommand : AsyncCommand<ServerListCommand.Settings>
    {
        private readonly ILogger _logger;
        private readonly IServerService _serverService;
        private readonly IStorage _storage;

        public ServerListCommand(IServerService serverService, IStorage storage, ILogger logger) 
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

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var param = settings.Options;

            if (string.IsNullOrWhiteSpace(settings.Options))
            {
                await FetchAllServers();
                return 0;
            }

            if (param == "--local")
            {
                var serverlist = Properties.Settings.Default.serverlist;
                if (!string.IsNullOrEmpty(serverlist))
                {
                    var servers = JsonConvert.DeserializeObject<List<ServerModel>>(serverlist);
                    DisplayList(servers);
                }
                else
                {
                    Console.WriteLine("Error: There are no server data in local storage");
                }
                return 0;
            }

            if (Enum.TryParse<Country>(param, true, out Country country))
            {
                await FetchCountryServers(country);
                return 0;
            }

            if (Enum.TryParse<Protocol>(param, true, out Protocol protocol))
            {
                await FetchProtocolServers(protocol);
            }
            return 0;
        }

        private async Task FetchProtocolServers(Protocol protocol)
        {
            var query = new VpnServerQuery((int)protocol, null, null, null, null, null);
            var servers = await _serverService.GetAllServersByProtocol((int)query.Protocol.Value);
            ManageServers(servers);
        }

        private void ManageServers(List<ServerModel> servers)
        {
            var serversJson = JsonConvert.SerializeObject(servers);
            
            _storage.StoreValue("serverlist", serversJson, false);
            _logger.Log($"Saved new server list: {serversJson}");
            DisplayList(servers);
        }

        private async Task FetchCountryServers(Country country)
        {
            var query = new VpnServerQuery(null, (int)country, null, null, null, null);
            var servers = await _serverService.GetAllServersByCountry(query.CountryId.Value);
            ManageServers(servers);
        }

        private async Task FetchAllServers()
        {
            var servers = await _serverService.GetAllServers();
            ManageServers(servers);
        }

        private void DisplayList(List<ServerModel> servers)
        {
            Console.WriteLine("Server list: ");
            foreach (var server in servers)
            {
                AnsiConsole.MarkupLine($"Name: [green]{server.Name}[/]");
            }
            Console.WriteLine($"Total servers: [red]{servers.Count}[/]");
        }
    }
}
