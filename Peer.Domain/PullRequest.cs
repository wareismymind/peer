using System;
using Peer.Domain.Util;

namespace Peer.Domain
{
    public class PullRequest
    {
        public string Id { get; }
        public Identifier Identifier { get; }
        public Uri Url { get; }
        public Descriptor Descriptor { get; }
        public State State { get; }
        public GitInfo GitInfo { get; }

        public PullRequest(string id, Identifier identifier, Uri url, Descriptor descriptor, State state, GitInfo gitInfo)
        {
            Validators.ArgIsNotNullEmptyOrWhitespace(id, nameof(id));

            Id = id;
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            State = state ?? throw new ArgumentNullException(nameof(state));
            GitInfo = gitInfo ?? throw new ArgumentNullException(nameof(gitInfo));
        }
    }
}
