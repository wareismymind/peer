using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps;
using Peer.Parsing;
using Peer.Verbs;

namespace Peer.Handlers;

public class ConfigShowHandler : IHandler<ConfigShowOptions>
{
    private readonly PeerEnvironmentOptions _opts;

    public ConfigShowHandler(PeerEnvironmentOptions opts)
    {
        _opts = opts;
    }

    public async Task<int> HandleAsync(
        ConfigShowOptions opts,
        IServiceCollection services,
        CancellationToken token = default)
    {
        if (!File.Exists(_opts.ConfigPath))
        {

            await Console.Error.WriteLineAsync($"You don't have a config! User config --init to create a default config");
            return 1;
        }

        var text = await File.ReadAllTextAsync(_opts.ConfigPath, token);
        Console.WriteLine($"// Path: {_opts.ConfigPath}");
        Console.WriteLine(text);
        return 0;
    }
}
