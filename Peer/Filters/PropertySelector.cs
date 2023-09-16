using System;
using Peer.Domain;

namespace Peer.Filters
{
    public class PropertySelector<T> : IPropertySelector
    where T : IComparable
    {
        public Func<PullRequest, T> Selector { get; }
        public Type ReturnType => typeof(T);

        Func<PullRequest, IComparable> IPropertySelector.Selector => pr => Selector(pr);

        public PropertySelector(Func<PullRequest, T> func)
        {
            Selector = pr => func(pr);
        }
    }
}
