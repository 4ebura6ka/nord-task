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
                config.ValidateExamples();

                config.AddCommand<ServerListCommand>("server_list")
                .WithExample(new[] {"server_list", "--country", "france"})
                .WithExample(new[] {"server_list", "--protocol", "TCP"})
                .WithExample(new[] {"server_list", "--local"})
                .WithDescription(
                "[green]To get and save France servers, use command[/]: [italic]partycli server_list --country france[/]\n"+
                "[green]To get and save servers that support TCP protocol, use command[/]: [italic]partycli server_list --protocol TCP[/]\n"+
                "[green]To see saved list of servers, use command[/]: [italic]partycli server_list --local[/]\n");

                config.AddCommand<ConfigCommand>("config")
                .IsHidden()
                .WithDescription("Saves the configuration")
                .WithExample(new[] {"config", "newparam", "value"});
            });

            return app.Run(args);
        }
    }
}
