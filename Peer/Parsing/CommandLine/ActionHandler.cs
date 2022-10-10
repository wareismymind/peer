using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Peer.Parsing.CommandLine;

public class ActionHandler<TVerb> : IHandler<TVerb>
{
    private readonly Func<TVerb, IServiceCollection, CancellationToken, Task> _handler;

    public ActionHandler(Func<TVerb, IServiceCollection, CancellationToken, Task> handler)
    {
        _handler = handler;
    }


    public Task HandleAsync(TVerb opts, IServiceCollection collection, CancellationToken token = default)
    {
        return _handler.Invoke(opts, collection, token);
    }
}
