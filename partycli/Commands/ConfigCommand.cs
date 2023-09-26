using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace partycli.Commands
{
    public class ConfigCommand : Command<ConfigCommand.Settings>
    {
        private readonly IStorage _storage;
        private readonly ILogger _logger;

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[NAME]")]
            public string ConfigName { get; set; }
        }

        public ConfigCommand(IStorage storage, ILogger logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var name = settings.ConfigName;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "config";
            }

            _storage.StoreValue(ProccessName(name), "config");
            _logger.Log("Changed " + ProccessName(name) + " to " + "config");

            return 0;
        }

        private string ProccessName(string name)
        {
            name = name.Replace("-", string.Empty);
            return name;
        }
    }
}
