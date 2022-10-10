using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Peer.App;

public interface IHandler<in TVerb>
{
    Task HandleAsync(TVerb opts, IServiceCollection collection, CancellationToken token = default);
}

public interface IHandler
{
    Task HandleAsync(object opts, IServiceCollection collection, CancellationToken token = default);
    Type Type { get; }
}