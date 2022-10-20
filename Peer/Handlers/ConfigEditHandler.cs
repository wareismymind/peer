using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peer.Apps;
using Peer.ConfigSections;
using Peer.Domain.Commands;
using Peer.Verbs;
using wimm.Secundatives;

namespace Peer.Handlers;

public class ConfigEditHandler : IHandler<ConfigEditOptions>
{
    public async Task<int> HandleAsync(ConfigEditOptions opts, IServiceCollection services, CancellationToken token = default)
    {
        services.AddSingleton<ConfigEdit>();
        services.AddSingleton(sp =>
        {
             var config = sp.GetRequiredService<IConfiguration>();
             return new ConfigEditConfig(config["Editor"], Constants.DefaultConfigPath);
        });

        var sp = services.BuildServiceProvider();

        var command = sp.GetRequiredService<ConfigEdit>();
        return await command.RunAsync()
            .MapOr(_ => 0, _ => 1);
    }
}
