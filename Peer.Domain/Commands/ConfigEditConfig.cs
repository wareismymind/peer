namespace Peer.Domain.Commands;

public class ConfigEditConfig
{
    public string? Editor { get; }
    public string ConfigPath { get; }

    public ConfigEditConfig(string? editor, string configPath)
    {
        Editor = editor;
        ConfigPath = configPath;
    }
}