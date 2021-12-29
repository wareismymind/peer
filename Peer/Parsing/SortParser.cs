using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Peer.Domain;
using Peer.Domain.Exceptions;
using Peer.Domain.Filters;
using wimm.Secundatives;
using wimm.Secundatives.Extensions;

namespace Peer.Parsing
{
    public static class SelectorMap
    {
        private static readonly Dictionary<string, IPropertySelector> _selectorMap = new()
        {
            ["repo"] = new PropertySelector<string>(pr => pr.Identifier.Repo),
            ["id-lex"] = new PropertySelector<string>(pr => pr.Id),
            ["id"] = new PropertySelector<int>(pr => int.Parse(pr.Id)),
            ["owner"] = new PropertySelector<string>(pr => pr.Identifier.Owner),
            ["author"] = new PropertySelector<string>(pr => pr.Identifier.Author),
            ["status"] = new PropertySelector<PullRequestStatus>(pr => pr.State.Status),
            ["active"] = new PropertySelector<int>(pr => pr.State.ActiveComments),
        };

        public static IEnumerable<string> Keys => _selectorMap.Keys;

        public static Maybe<IPropertySelector> TryGetSelector(string key)
        {
            _selectorMap.TryGetValue(key.ToLowerInvariant(), out var selector);
            return selector!.AsMaybe();
        }
    }

    public class FilterParser
    {
        public static Result<IFilter, FilterParseError> ParseFilterOption(string filterRaw)
        {
            if (filterRaw == null)
            {
                throw new ArgumentNullException(nameof(filterRaw));
            }

            var split = filterRaw.ToLower()
                .Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            return split.Length switch
            {
                0 or 1 => FilterParseError.NotEnoughSections,
                2 => ParseSections(split),
                _ => FilterParseError.TooManySections
            };
        }

        private static Result<IFilter, FilterParseError> ParseSections(string[] sections)
        {
            var negated = sections[0].StartsWith('!');
            //CN: Bit ugly
            var key = sections[0][(negated ? 1 : 0)..].ToLowerInvariant();
            var value = sections[1];

            var maybeSelector = SelectorMap.TryGetSelector(key);
            return maybeSelector.OkOr(FilterParseError.UnknownFilterKey)
                .Map(selector =>
                {
                    //pr: Lazy initial impl cause otherwise we have to create a big factory that ends up basically no better than this
                    // could do. It's a bit of a purity/code tradeoff. 
                    return selector switch
                    {
                        PropertySelector<string> => new ActionFilter(pr => new Regex(value).IsMatch((string)selector.Selector(pr)), negated),
                        PropertySelector<int> x => CreateIntFilter(x, value, negated),
                        PropertySelector<PullRequestStatus> x => CreateEnumMatchFilter(x, value, negated),
                        _ => throw new UnreachableException()
                    };
                }).Flatten();
        }

        private static Result<IFilter, FilterParseError> CreateIntFilter(PropertySelector<int> selector, string value, bool negate)
        {
            var parsed = ParseInt(value);
            return parsed.OkOr(FilterParseError.UnknownMatchValue)
                .Map(v => new ActionFilter(pr => selector.Selector(pr) == v, negate) as IFilter);
        }

        private static Result<IFilter, FilterParseError> CreateEnumMatchFilter<T>(PropertySelector<T> selector, string value, bool negate)
            where T : struct, IComparable
        {
            var parsed = ParseEnum<T>(value);
            return parsed.OkOr(FilterParseError.UnknownMatchValue)
                .Map(v => new EnumMatchingFilter<T>(selector, v, negate) as IFilter);
        }

        private static Maybe<int> ParseInt(string raw)
        {
            if (int.TryParse(raw, out var result))
            {
                return result;
            }

            return Maybe.None;
        }

        private static Maybe<T> ParseEnum<T>(string raw) where T : struct
        {
            if (Enum.TryParse<T>(raw, true, out var result))
            {
                return result;
            }

            return Maybe.None;
        }
    }

    public enum FilterParseError
    {
        TooManySections,
        NotEnoughSections,
        UnknownFilterKey,
        UnknownMatchValue
    }

    public class SortParser
    {
        public static Result<ISorter<PullRequest>, SortParseError> ParseSortOption(string sortOption)
        {
            if (sortOption == null)
            {
                throw new ArgumentNullException(nameof(sortOption));
            }

            var split = sortOption.ToLower().Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var sortDirectionResult = split.Length switch
            {
                0 => SortParseError.NotEnoughSections,
                1 or 2 => GetSortDirection(split),
                _ => SortParseError.TooManySections,
            };

            if (sortDirectionResult.IsError)
            {
                return sortDirectionResult.Error;
            }

            var selector = GetSelector(split[0]);

            return selector.Map(selector =>
                (ISorter<PullRequest>)new SelectorSorter<PullRequest>(selector, sortDirectionResult.Value));

        }

        private static Result<SortDirection, SortParseError> GetSortDirection(string[] sections)
        {
            if (sections.Length == 1)
                return SortDirection.Ascending;

            return sections[1] switch
            {
                "asc" => SortDirection.Ascending,
                "ascending" => SortDirection.Ascending,
                "desc" => SortDirection.Descending,
                "descending" => SortDirection.Descending,
                _ => SortParseError.InvalidSortDirection
            };
        }

        private static Result<Func<PullRequest, IComparable>, SortParseError> GetSelector(string name)
        {
            return SelectorMap.TryGetSelector(name)
                .OkOr(SortParseError.UnknownSortKey)
                .Map(x => x.Selector);
        }
    }
}
