using System;
using System.IO;

namespace Peer.ConfigSections;

public static class Constants
{
    public static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.json");
}
