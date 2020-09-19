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
            _logger.LogInformation("Removing job and/or target from Prometheus file service discovery...");
            
            List<ConfigurationDto> configurations;
            if (!File.Exists(options.ConfigFilePath))
            {
                // If the files doesn't yet exist, create a new one
                _logger.LogInformation("No file {filepath} was found. Nothing to remove.", options.ConfigFilePath);
                return;
            }

            // Read the current configuration
            var inputJson = await File.ReadAllTextAsync(options.ConfigFilePath).ConfigureAwait(false);
            configurations = JsonSerializer.Deserialize<List<ConfigurationDto>>(inputJson, _serializerOptions);

            // Find the specified job
            var job = configurations.FirstOrDefault(c => c.Labels.ContainsKey("job") && c.Labels["job"] == options.Job);
            if(job is null)
            {
                _logger.LogInformation("Job {job} not found in configuration file. Nothing will be removed.", options.Job);
                return;
            }

            if(options.Target != null)
            {
                // Find the specified target if any specified and remove it from the job configuration
                var success = job.Targets.Remove(options.Target);
                if (!success)
                {
                    _logger.LogInformation("Sepcified target {target} was not found in job {job}. Nothing will be removed.",
                        options.Target, options.Job);
                    return;
                }
            }
            else
                // Remove the full job configuration otherwise
                configurations.Remove(job);

            // Save the file
            var json = JsonSerializer.Serialize(configurations, _serializerOptions);
            await File.WriteAllTextAsync(options.ConfigFilePath, json).ConfigureAwait(false);
        }
    }
}
