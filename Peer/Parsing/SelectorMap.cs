using System.Collections.Generic;
using Peer.Domain;
using Peer.Filters;
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
            ["title"] = new PropertySelector<string>(pr => pr.Descriptor.Title)
        };

        public static IEnumerable<string> Keys => _selectorMap.Keys;

        public static Maybe<IPropertySelector> TryGetSelector(string key)
        {
            _selectorMap.TryGetValue(key.ToLowerInvariant(), out var selector);
            return selector!.AsMaybe();
        }
    }
}
