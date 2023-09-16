using System;
using Peer.Domain;

namespace Peer.Filters
{
    public interface IPropertySelector
    {
        Func<PullRequest, IComparable> Selector { get; }
        Type ReturnType { get; }
    }
}
