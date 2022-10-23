using Microsoft.Extensions.DependencyInjection;

namespace Peer.Apps.AppBuilder;

public sealed class SubVerbBuilder<TSuper, TSub> : VerbBuilder<TSub>
{
    public SubVerbBuilder(IServiceCollection services) : base(services)
    {
    }
}
