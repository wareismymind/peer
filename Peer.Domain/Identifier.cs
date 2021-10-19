using System;
using System.Collections.Generic;
using System.Linq;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class Identifier
    {
        public string Id { get; }
        public string Repo { get; }
        public string Owner { get; }
        public string Provider { get; }

        public Identifier(string id, string repo, string owner, string provider)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }


        public Result<bool, MatchError> IsMatch(string partial)
        {
            if (partial == null)
            {
                throw new ArgumentNullException(nameof(partial));
            }

            var split = partial.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 0)
            {
                return MatchError.NoSegmentsToMatch;
            }

            if (split.Length > 4)
            {
                return MatchError.TooManySegments;
            }

            foreach (var (partialSegment, localSegment) in split.Reverse().Zip(EnumerateValues()))
            {
                if (!partialSegment.Equals(localSegment, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<string> EnumerateValues()
        {
            yield return Id;
            yield return Repo;
            yield return Owner;
            yield return Provider;
        }
    }
}
