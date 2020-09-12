using System.Collections.Generic;

namespace PromFileSdConfigAppender
{
    /// <summary>
    /// Structure of Prometheus File SD configuration
    /// </summary>
    class ConfigurationDto
    {
        public List<string> Targets { get; set; }
        public Dictionary<string, string> Labels { get; set; }
    }
}
