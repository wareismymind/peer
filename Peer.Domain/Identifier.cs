using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsMatch(PartialIdentifier partial)
        {
            if (partial == null)
            {
                throw new ArgumentNullException(nameof(partial));
            }

            return partial.Segments.Reverse()
                .Zip(EnumerateValues())
                .All(((string segment, string local) x)
                    => x.segment.Equals(x.local, StringComparison.OrdinalIgnoreCase));

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
