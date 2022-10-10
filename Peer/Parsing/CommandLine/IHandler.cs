using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Peer.Parsing.CommandLine;

public interface IHandler<in TVerb>
{
    Task HandleAsync(TVerb opts, IServiceCollection collection, CancellationToken token = default);
}

public interface IHandler
{
    Task HandleAsync(object opts, IServiceCollection collection, CancellationToken token = default);
    Type Type { get; }
}

public class HandlerWrapper<T> : IHandler
{
    private readonly IHandler<T> _handler;
    public Type Type => typeof(T);

    public HandlerWrapper(IHandler<T> handler)
    {
        _handler = handler;
    }
    public Task HandleAsync(T opts, IServiceCollection collection, CancellationToken token = default)
    {
        return _handler.HandleAsync(opts, collection, token);
    }

    public Task HandleAsync(object opts, IServiceCollection collection, CancellationToken token = default)
    {
        return HandleAsync((T)opts, collection, token);
    }
}

