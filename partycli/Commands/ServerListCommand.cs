using Newtonsoft.Json;
using PartyCli.API;
using PartyCli.Models;
using PartyCli.Data;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyCli.Commands
{
    public class ServerListCommand : AsyncCommand<ServerListCommand.Settings>
    {
        private readonly ILogger _logger;
        private readonly IServerService _serverService;
        private readonly IServerRepository _storage;

        public ServerListCommand(IServerService serverService, IServerRepository storage, ILogger logger) 
        {
            _serverService = serverService;
            _storage = storage;
            _logger = logger;
        }
        public class Settings : CommandSettings
        {
            [CommandOption("--local")]
            public bool? Local { get; set; }

            [CommandOption("-c|--country")]
            public string Country { get; set; }

            [CommandOption("-p|--protocol")]
            public string Protocol { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            if (settings.Local != null && (bool)settings.Local)
            {
                var servers = _storage.RetrieveValue("serverlist");
                if (servers != null)
                {
                    DisplayList(servers);
                }
                else
                {
#if DEBUG
                    Console.WriteLine("Error: There are no server data in local storage");
#else
                    _logger.Log("Error: There are no server data in local storage");
#endif
                }
                return 0;
            }

            if (!string.IsNullOrEmpty(settings.Country) && Enum.TryParse<Country>(settings.Country, true, out Country country))
            {
                await FetchCountryServers(country);
                return 0;
            }

            if (!string.IsNullOrEmpty(settings.Protocol) && Enum.TryParse<Protocol>(settings.Protocol, true, out Protocol protocol))
            {
                await FetchProtocolServers(protocol);
                return 0;
            }

            await FetchAllServers();
            
            return 0;
        }

        private async Task FetchProtocolServers(Protocol protocol)
        {
            var servers = await _serverService.GetAllServersByProtocol((int)protocol);
            ManageServersInformation(servers);
        }

        private void ManageServersInformation(List<ServerModel> servers)
        {
            var serversJson = JsonConvert.SerializeObject(servers);
            
            _storage.StoreValue("serverlist", servers, false);
            _logger.Log($"Saved new server list: {serversJson}");
            DisplayList(servers);
        }

        private async Task FetchCountryServers(Country country)
        {
            var servers = await _serverService.GetAllServersByCountry((int)country);
            ManageServersInformation(servers);
        }

        private async Task FetchAllServers()
        {
            var servers = await _serverService.GetAllServers();
            ManageServersInformation(servers);
        }

        private void DisplayList(List<ServerModel> servers)
        {
            AnsiConsole.MarkupLine("[bold]Server list:[/]");
            foreach (var server in servers)
            {
                AnsiConsole.MarkupLine($"Name: [green]{server.Name}[/]. Load: {server.Load}. Status: {server.Status}");
            }
            AnsiConsole.MarkupLine($"Total servers: [red]{servers.Count}[/]");
        }
    }
}
