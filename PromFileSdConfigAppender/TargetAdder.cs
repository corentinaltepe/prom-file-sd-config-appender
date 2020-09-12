using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromFileSdConfigAppender
{
    /// <summary>
    /// Adds jobs and targets to the configuration file.
    /// </summary>
    class TargetAdder
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<TargetAdder> _logger;
        public TargetAdder(JsonSerializerOptions serializerOptions, ILogger<TargetAdder> logger)
        {
            _serializerOptions = serializerOptions;
            _logger = logger;
        }

        /// <summary>
        /// Adds the given target to the given job. Creates the job if it doesn't exist.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task Add(AddOptions options)
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

            // Find the configuration with the right job or create it if it doesn't exist yet
            var config = configurations.FirstOrDefault(c => c.Labels.ContainsKey("job") && c.Labels["job"] == options.Job);
            if (config == null)
            {
                _logger.LogInformation("Job {job} not found in config file. It will be created.", options.Job);
                config = new ConfigurationDto
                {
                    Targets = new List<string>(),
                    Labels = new Dictionary<string, string> { { "job", options.Job } },
                };
                configurations.Add(config);
            }

            // If the target already exist, do nothing
            if (config.Targets.Contains(options.Target))
            {
                _logger.LogInformation("The target {target} already exists with job {job}. The config file will not be overwritten.",
                    options.Target, options.Job);
                return;
            }

            // Finally, add the target
            config.Targets.Add(options.Target);

            // Save the file
            var json = JsonSerializer.Serialize(configurations, _serializerOptions);
            await File.WriteAllTextAsync(options.ConfigFilePath, json).ConfigureAwait(false);
        }
    }
}
