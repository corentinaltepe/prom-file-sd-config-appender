using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromFileSdConfigAppender
{
    /// <summary>
    /// Removes jobs or targets from the configuration file.
    /// </summary>
    class TargetRemover
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<TargetAdder> _logger;
        public TargetRemover(JsonSerializerOptions serializerOptions, ILogger<TargetAdder> logger)
        {
            _serializerOptions = serializerOptions;
            _logger = logger;
        }

        /// <summary>
        /// Removes the given target or the given job.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task Remove(RemoveOptions options)
        {
            _logger.LogInformation("Adding target to Prometheus file service discovery...");
            
            List<ConfigurationDto> configurations;
            if (File.Exists(options.ConfigFilePath))
            {
                // Read the current configuration
                var inputJson = await File.ReadAllTextAsync(options.ConfigFilePath).ConfigureAwait(false);
                configurations = JsonSerializer.Deserialize<List<ConfigurationDto>>(inputJson, _serializerOptions);
            }
            else
            {
                // If the files doesn't yet exist, create a new one
                _logger.LogInformation("No file {filepath} was found. It will be created.", options.ConfigFilePath);
                configurations = new List<ConfigurationDto>();
            }

            

            // Save the file
            var json = JsonSerializer.Serialize(configurations, _serializerOptions);
            await File.WriteAllTextAsync(options.ConfigFilePath, json).ConfigureAwait(false);
        }
    }
}
