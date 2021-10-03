using System;

namespace Peer.Domain
{
    public class PullRequest
    {
        public string Id { get; }
        public Uri Url { get; }
        public Descriptor Descriptor { get; }
        public State State { get; }
        public GitInfo GitInfo { get; }

        public PullRequest(string id, Uri url, Descriptor descriptor, State state, GitInfo gitInfo)
        {
            Validators.ArgIsNotNullEmptyOrWhitespace(id, nameof(id));

            Id = id;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            State = state ?? throw new ArgumentNullException(nameof(state));
            GitInfo = gitInfo ?? throw new ArgumentNullException(nameof(gitInfo));
        }
    }
}
