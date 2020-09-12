using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PromFileSdConfigAppender
{
    [Verb("add", HelpText = "Add a target and job to the configuration file.")]
    class AddOptions
    {
        [Option('j', "job", Required = true, HelpText = "Unique name of the job to which the target belongs.")]
        public string Job { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target to be added.")]
        public string Target { get; set; }

        [Option('c', "config-path", Default = "targets.json", HelpText = "Path to the configuration file for Prometheus File SD.")]
        public string ConfigFilePath { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<AddOptions>(args)
                .WithParsedAsync(Add).ConfigureAwait(false);
        }

        static async Task Add(AddOptions options)
        {
            using var provider = new ServiceCollection()
                .AddLogging(conf => conf.AddConsole())
                .BuildServiceProvider();

            var logger = provider.GetService<ILogger<Program>>();
            logger.LogInformation("Adding target to Prometheus file service discovery...");

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
            List<ConfigurationDto> configurations;
            if(File.Exists(options.ConfigFilePath))
            {
                // Read the current configuration
                var inputJson = await File.ReadAllTextAsync(options.ConfigFilePath).ConfigureAwait(false);
                configurations = JsonSerializer.Deserialize<List<ConfigurationDto>>(inputJson, serializerOptions);
            }
            else
            {
                // If the files doesn't yet exist, create a new one
                logger.LogInformation("No file {filepath} was found. It will be created.", options.ConfigFilePath);
                configurations = new List<ConfigurationDto> {
                    new ConfigurationDto
                    {
                        Targets = new List<string> { options.Target },
                        Labels = new Dictionary<string, string> { { "job", options.Job } },
                    }
                };
            }

            var json = JsonSerializer.Serialize(configurations, serializerOptions);
            await File.WriteAllTextAsync(options.ConfigFilePath, json).ConfigureAwait(false);
        }
    }
}
