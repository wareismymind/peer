using System;
using System.Linq;
using Peer.Domain;

namespace Peer.GitHub.GraphQL.PullRequestSearch
{
#nullable disable
    public class PullRequest
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public Uri Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string BaseRefName { get; set; }
        public string HeadRefName { get; set; }
        public ReviewThreads ReviewThreads { get; set; }
        public BaseRepository BaseRepository { get; set; }

        public Domain.PullRequest Into()
        {
            // todo: Calculate status.
            var status = PullRequestStatus.ReadyToMerge;
            var totalComments = ReviewThreads.Nodes.Count;
            var activeComments = ReviewThreads.Nodes.Count(t => !t.IsResolved);
            return new Domain.PullRequest(
                Number.ToString(),
                new Identifier(Number.ToString(), BaseRepository.Name, BaseRepository.Owner.Login, ProviderConstants.Github),
                Url,
                new Descriptor(Title, Body ?? string.Empty),
                new State(status, totalComments, activeComments),
                new GitInfo(HeadRefName, BaseRefName));
        }
    }

    public class BaseRepository
    {
        public string Name { get; set; }
        public Owner Owner { get; set; }
    }

    public class Owner
    {
        public string Login { get; set; }
    }
#nullable enable
}
