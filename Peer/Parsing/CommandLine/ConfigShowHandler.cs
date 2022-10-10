using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Peer.App;
using Peer.ConfigSections;
using Peer.Verbs;

namespace Peer.Parsing.CommandLine;

public class ConfigShowHandler : IHandler<ConfigShowOptions>
{
    public async Task HandleAsync(
        ConfigShowOptions opts,
        IServiceCollection collection,
        CancellationToken token = default)
    {
        if (!File.Exists(Constants.DefaultConfigPath))
        {

            Console.WriteLine($"You don't have a config! User config --init to create a default config");
            return;
        }

        var text = await File.ReadAllTextAsync(Constants.DefaultConfigPath, token);
        Console.WriteLine(text);

    }
}
