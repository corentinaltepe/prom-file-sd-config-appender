using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AddOptions>(args).WithParsed(Add);
        }

        static void Add(AddOptions options)
        {
            using var provider = new ServiceCollection()
                .AddLogging(conf => conf.AddConsole())
                .BuildServiceProvider();

            var logger = provider.GetService<ILogger<Program>>();
            logger.LogInformation("Adding target to Prometheus file service discovery...");
        }
    }
}
