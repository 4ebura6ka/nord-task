using Newtonsoft.Json;
using partycli.API;
using partycli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using partycli.Commands;
using Microsoft.Extensions.DependencyInjection;
using partycli.Infrastructure;
using System.Configuration;

namespace partycli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var name = string.Empty;

            var services = new ServiceCollection();
            services.AddScoped<ILogger, Logger>();
            services.AddSingleton<IStorage, Storage>();
            services.AddHttpClient<IServerService, ServerService>("nordvpn", client =>
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["NordVpn"]);
            });

            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(config =>
            {
                config.AddCommand<ServerListCommand>("server_list")
                .WithDescription("[underline red]To get and save all servers, use command[/]: partycli.exe server_list")
                .WithDescription("To get and save France servers, use command: partycli.exe server_list --france")
                .WithDescription("To get and save servers that support TCP protocol, use command: partycli.exe server_list --TCP")
                .WithDescription("To see saved list of servers, use command: partycli.exe server_list --local ")
                .WithExample("server_list");

                config.AddCommand<ConfigCommand>("config")
              //  .IsHidden()
                .WithDescription("Saves the configuration")
                .WithExample("config");
            });
            return app.Run(args);
        }
    }
}
