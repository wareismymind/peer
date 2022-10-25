using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps;
using Peer.ConfigSections;
using Peer.Domain.Commands;
using Peer.Parsing;
using Peer.Verbs;
using wimm.Secundatives;

namespace Peer.Handlers;

public class ConfigEditHandler : IHandler<ConfigEditOptions>
{
    private readonly PeerEnvironmentOptions _opts;

    public ConfigEditHandler(PeerEnvironmentOptions opts)
    {
        _opts = opts;
    }

    public async Task<int> HandleAsync(ConfigEditOptions opts, IServiceCollection services, CancellationToken token = default)
    {
        services.AddSingleton<ConfigEdit>();
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new ConfigEditConfig(_opts.Editor, _opts.ConfigPath);
        });

        var sp = services.BuildServiceProvider();

        var command = sp.GetRequiredService<ConfigEdit>();
        return await command.RunAsync()
            .MapOr(_ => 0, _ => 1);
    }
}
