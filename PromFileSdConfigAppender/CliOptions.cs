using CommandLine;
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

    [Verb("remove", HelpText = "Remove a target or job from the configuration file. Specifying a job " +
        "without specifiying a target will remove all associated targets with it.")]
    class RemoveOptions
    {
        [Option('j', "job", Required = true, HelpText = "Removes the given job and all its associated targets if a target isn't specified.")]
        public string Job { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target to be removed. Its job name must be specified.")]
        public string Target { get; set; }

        [Option('c', "config-path", Default = "targets.json", HelpText = "Path to the configuration file for Prometheus File SD.")]
        public string ConfigFilePath { get; set; }
    }
}
