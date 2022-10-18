using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps;
using Peer.ConfigSections;
using Peer.Verbs;

namespace Peer.Handlers;

public class ConfigShowHandler : IHandler<ConfigShowOptions>
{
    public async Task<int> HandleAsync(
        ConfigShowOptions opts,
        IServiceCollection collection,
        CancellationToken token = default)
    {
        if (!File.Exists(Constants.DefaultConfigPath))
        {

            await Console.Error.WriteLineAsync($"You don't have a config! User config --init to create a default config");
            return 1;
        }

        var text = await File.ReadAllTextAsync(Constants.DefaultConfigPath, token);
        Console.WriteLine($"//Path: {Constants.DefaultConfigPath}");
        Console.WriteLine(text);
        return 0;
    }
}
