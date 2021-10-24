using System;
using Peer.Domain;
using wimm.Secundatives;

namespace Peer.Parsing
{
    public class SortParser
    {
        public static Result<ISorter<PullRequest>, ParseError> ParseSortOption(string sortOption)
        {
            if (sortOption == null)
            {
                throw new ArgumentNullException(nameof(sortOption));
            }

            var split = sortOption.ToLower().Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (split.Length > 2)
            {
                return ParseError.TooManySections;
            }

            if (split.Length == 0)
            {
                return ParseError.NotEnoughSections;
            }

            //CN: Should see if we can do any reflecty witchcraft here to make this really generic.
            // till then maybe just have a specific set of things?

            var sortDirectionResult = GetSortDirection(split);

            if (sortDirectionResult.IsError)
            {
                return sortDirectionResult.Error;
            }

            var selector = GetSelector(split[0]);

            return selector.Map(selector =>
                (ISorter<PullRequest>)new SelectorSorter<PullRequest>(selector, sortDirectionResult.Value));

        }

        private static Result<SortDirection, ParseError> GetSortDirection(string[] sections)
        {
            if (sections.Length == 1)
                return SortDirection.Ascending;

            return sections[1] switch
            {
                "asc" => SortDirection.Ascending,
                "ascending" => SortDirection.Ascending,
                "desc" => SortDirection.Descending,
                "descending" => SortDirection.Descending,
                _ => ParseError.InvalidSortDirection
            };
        }

        private static Result<Func<PullRequest, IComparable>, ParseError> GetSelector(string name)
        {
            return name switch
            {
                "repo" => MakeSelector(x => x.Identifier.Repo),
                "id-lex" => MakeSelector(x => x.Id),
                "id" => MakeSelector(x => int.Parse(x.Id)),
                "owner" => MakeSelector(x => x.Identifier.Owner),
                "status" => MakeSelector(x => x.State.Status),
                "active" => MakeSelector(x => x.State.ActiveComments),
                _ => ParseError.UnknownSortKey
            };
        }

        //CN: Being lazy here but just want to be able to wrap it and use the switch :|
        private static Func<PullRequest, IComparable> MakeSelector<TProp>(Func<PullRequest, TProp> selector)
            where TProp : IComparable
        {
            return x => selector(x);
        }
    }
}
