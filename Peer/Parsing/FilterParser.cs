﻿using System;
using System.Text.RegularExpressions;
using Peer.Domain;
using Peer.Domain.Exceptions;
using Peer.Domain.Filters;
using wimm.Secundatives;
using wimm.Secundatives.Extensions;

namespace Peer.Parsing
{
    public class FilterParser
    {
        public static Result<IFilter, FilterParseError> ParseFilterOption(string filterRaw)
        {
            if (filterRaw == null)
            {
                throw new ArgumentNullException(nameof(filterRaw));
            }

            var splitPoint = filterRaw.IndexOf(':');

            return splitPoint switch
            {
                -1 => FilterParseError.NotEnoughSections,
                0 => FilterParseError.NoFilterKeySpecified,
                var x when x == filterRaw.Length - 1 => FilterParseError.FilterContentEmpty,
                _ => ParseSections(new string[] { filterRaw[0..splitPoint].Trim(), filterRaw[(splitPoint + 1)..].Trim() })
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
                    return selector switch
                    {
                        PropertySelector<string> x => CreateRegexFilter(x, value, negated),
                        PropertySelector<int> x => CreateIntFilter(x, value, negated),
                        PropertySelector<PullRequestStatus> x => CreateEnumMatchFilter(x, value, negated),
                        _ => throw new UnreachableException()
                    };
                }).Flatten();
        }

        private static Result<IFilter, FilterParseError> CreateRegexFilter(PropertySelector<string> selector, string value, bool negate)
        {
            var parsed = ParseRegex(value);
            return parsed.OkOr(FilterParseError.UnknownMatchValue)
                .Map(v => new RegexFilter(selector, v, negate) as IFilter);
        }

        private static Maybe<Regex> ParseRegex(string raw)
        {
            try
            {
                return new Regex(raw, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException)
            {
                return Maybe.None;
            }
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
}
