using Microsoft.Extensions.DependencyInjection;
using PartyCli.API;
using PartyCli.Commands;
using PartyCli.Infrastructure;
using PartyCli.Data;
using Spectre.Console.Cli;
using System;
using System.Configuration;

namespace PartyCli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddScoped<ILogger, Logger>();
            services.AddSingleton<IServerRepository, ServerRepository>();
            services.AddSingleton<IConfigRepository, ConfigRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();

            services.AddHttpClient<IServerService, ServerService>("nordvpn", client =>
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["NordVpn"]);
            });

            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(config =>
            {
                config.AddCommand<ServerListCommand>("server_list")
                .WithDescription("[green]To get and save servers[/]")
                .WithExample("[underline red]To get and save all servers, use command[/]: [italic]partycli server_list[/]")
                .WithExample("[yellow]To get and save France servers, use command[/]: [italic]partycli server_list --country france[/]")
                .WithExample("[yellow]To get and save servers that support TCP protocol, use command[/]:[italic]partycli server_list --protocol TCP[/]")
                .WithExample("To see saved list of servers, use command: [italic]partycli server_list --local[/]");

                config.AddCommand<ConfigCommand>("config")
                .IsHidden()
                .WithDescription("Saves the configuration")
                .WithExample("config", "newparameter", "value");
            });
            return app.Run(args);
        }
    }
}
