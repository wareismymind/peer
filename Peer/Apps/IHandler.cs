using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Peer.Apps;

public interface IHandler<in TVerb>
{
    Task<int> HandleAsync(TVerb opts, IServiceCollection services, CancellationToken token = default);
}

public interface IHandler
{
    Task HandleAsync(object opts, IServiceCollection collection, CancellationToken token = default);
    Type Type { get; }
}
