using System;

namespace Peer.Domain.Filters
{
    public interface IPropertySelector
    {
        Func<PullRequest, IComparable> Selector { get; }
        Type ReturnType { get; }
    }
}
