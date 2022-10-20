using CommandLine;

namespace Peer.Verbs
{
    [Verb("config", isDefault: false, HelpText = "Create or edit the configuration for peer")]
    public class ConfigOptions
    {
    }

    [Verb("init", isDefault: false, HelpText = "Create a default configuration file")]
    public class ConfigInitOptions
    {
        [Option(shortName: 'f', longName: "force", Required = false, HelpText = "Overwrite any existing configuration")]
        public bool Force { get; set; }
    }

    [Verb("show", isDefault: false, HelpText = "Print the current config and it's location")]
    public class ConfigShowOptions
    { }

    [Verb("edit", isDefault: false, HelpText = "Open your config in your default text editor")]
    public class ConfigEditOptions
    {

    }
}
