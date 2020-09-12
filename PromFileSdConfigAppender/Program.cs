using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PromFileSdConfigAppender
{
    class Program
    {
        static void Main(string[] args)
        {
            using var provider = new ServiceCollection()
                .AddLogging(conf => conf.AddConsole())
                .BuildServiceProvider();

            var logger = provider.GetService<ILogger<Program>>();
            logger.LogInformation("Configuring Prometheus file service discovery...");
        }
    }
}
