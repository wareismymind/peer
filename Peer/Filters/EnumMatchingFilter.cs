using System;
using System.Collections.Generic;
using System.Linq;
using Peer.Domain;

namespace Peer.Filters
{
    public class EnumMatchingFilter<T> : IFilter
        where T : struct, IComparable
    {
        private readonly T _value;
        private readonly bool _negated;
        private readonly PropertySelector<T> _selector;

        public EnumMatchingFilter(PropertySelector<T> selector, T value, bool negated)
        {
            _value = value;
            _negated = negated;
            _selector = selector;
        }

        public IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests)
        {
            return pullRequests.Where(pr =>
            {
                var prop = _selector.Selector(pr);
                return prop.Equals(_value) ^ _negated;
            });
        }
    }
}
