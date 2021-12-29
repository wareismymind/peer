using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Peer.Domain.Filters
{
    public interface IFilter
    {
        IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests);
    }

    public class ActionFilter : IFilter
    {
        private readonly Func<PullRequest, bool> _func;
        private readonly bool _negated;

        public ActionFilter(Func<PullRequest, bool> func, bool negated)
        {
            _func = func;
            _negated = negated;
        }

        public IAsyncEnumerable<PullRequest> Filter(IAsyncEnumerable<PullRequest> pullRequests)
        {
            return pullRequests.Where(x => _func(x) ^ _negated);
        }
    }

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

    public interface IPropertySelector
    {
        Func<PullRequest, IComparable> Selector { get; }
        Type ReturnType { get; }
        bool Is<T>() => ReturnType == typeof(T);
    }

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
