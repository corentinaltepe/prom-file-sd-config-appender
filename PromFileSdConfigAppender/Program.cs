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

            await Parser.Default.ParseArguments<AddOptions, RemoveOptions>(args)
                .WithParsedAsync(
                    o => provider.GetService<TargetAdder>().Add(o),
                    )
                .ConfigureAwait(false);
        }
    }
}
