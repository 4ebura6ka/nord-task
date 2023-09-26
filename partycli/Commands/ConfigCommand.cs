using Spectre.Console.Cli;
using System;

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

            [CommandArgument(0, "[VALUE]")]
            public string ConfigValue { get; set; }
        }

        public ConfigCommand(IStorage storage, ILogger logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var name = settings.ConfigName;
            var value = settings.ConfigValue;

            // shouldn't happen
            if (string.IsNullOrWhiteSpace(name)) return 1;

            var processedName = ProccessName(name);
            _storage.StoreValue(processedName, value);
            _logger.Log($"Changed {processedName} to {value}");

            return 0;
        }

        private string ProccessName(string name)
        {
            name = name.Replace("-", string.Empty);
            return name;
        }
    }
}
