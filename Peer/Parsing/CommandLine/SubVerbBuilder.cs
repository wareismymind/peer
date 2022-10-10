using Microsoft.Extensions.DependencyInjection;

namespace Peer.Parsing.CommandLine;

public class SubVerbBuilder<TSuper, TSub> : VerbBuilder<TSub>
{
    public SubVerbBuilder(IServiceCollection services) : base(services)
    {
    }
}
