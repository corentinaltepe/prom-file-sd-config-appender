using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            using var provider = new ServiceCollection()
                   .AddLogging(conf => conf.AddConsole())
                   .AddTransient<TargetAdder>()
                   .AddSingleton(new JsonSerializerOptions
                   {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                   })
                   .BuildServiceProvider();

            var logger = provider.GetService<ILogger<Program>>();

            await Parser.Default.ParseArguments<AddOptions>(args)
                .WithParsedAsync(o => provider.GetService<TargetAdder>().Add(o))
                .ConfigureAwait(false);
        }
    }
}
