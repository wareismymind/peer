using System;
using Peer.Domain;
using wimm.Secundatives;

namespace Peer.Parsing
{
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
