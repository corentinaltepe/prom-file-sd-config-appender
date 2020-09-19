using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromFileSdConfigAppender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var provider = new ServiceCollection()
                   .AddLogging(conf => conf.AddConsole())
                   .AddTransient<TargetAdder>()
                   .AddTransient<TargetRemover>()
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

            try
            {
                await Parser.Default
                    .ParseArguments<AddOptions, RemoveOptions>(args)
                    .MapResult(
                      async (AddOptions o) => await provider.GetService<TargetAdder>().Add(o),
                      async  (RemoveOptions o) => await provider.GetService<TargetRemover>().Remove(o),
                      errs => Task.FromResult(1));
            }
            catch(Exception ex)
            {
                logger.LogCritical(ex, "Critical error");
                Environment.ExitCode = 1;
            }
        }
    }
}
