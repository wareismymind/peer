using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Peer.Domain;

namespace Peer.Filters
{
    public class RegexFilter : IFilter
    {
        private readonly PropertySelector<string> _selector;
        private readonly Regex _regex;
        private readonly bool _negated;

        public RegexFilter(PropertySelector<string> selector, Regex regex, bool negated)
        {
            _selector = selector;
            _regex = regex;
            _negated = negated;
        }

        public IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests)
        {
            return pullRequests.Where(pr =>
            {
                var prop = _selector.Selector(pr);
                return _regex.IsMatch(prop) ^ _negated;
            });
        }
    }
}
