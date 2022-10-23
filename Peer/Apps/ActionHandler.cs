using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Peer.Apps;

public class ActionHandler<TVerb> : IHandler<TVerb>
{
    private readonly Func<TVerb, IServiceCollection, CancellationToken, Task<int>> _handler;

    public ActionHandler(Func<TVerb, IServiceCollection, CancellationToken, Task<int>> handler)
    {
        _handler = handler;
    }


    public Task<int> HandleAsync(TVerb opts, IServiceCollection services, CancellationToken token = default)
    {
        return _handler.Invoke(opts, services, token);
    }
}
