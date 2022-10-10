using Microsoft.Extensions.DependencyInjection;

namespace Peer.App.AppBuilder;

public class SubVerbBuilder<TSuper, TSub> : VerbBuilder<TSub>
{
    public SubVerbBuilder(IServiceCollection services) : base(services)
    {
    }
}
