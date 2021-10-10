using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peer.Domain;

namespace Peer
{
    public class GitHubRequestFetcher: IPullRequestFetcher
    {
        public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync()
        {
            await Task.CompletedTask;

            return new [] {
                new PullRequest(
                    "14",
                    new Uri("https://www.github.com/wareismymind/notarepo/pull/14"),
                    new Descriptor("Do a thing", "Change the source such that a thing is done"),
                    new State(PullRequestStatus.AwaitingReview, 13, 4),
                    new GitInfo("refs/heads/do-thing", "refs/heads/main")
                ),
                new PullRequest(
                    "27",
                    new Uri("https://www.github.com/wareismymind/notarepo/pull/27"),
                    new Descriptor("Fix the bug", "Change the source such that the bug is fixed"),
                    new State(PullRequestStatus.ReadyToMerge, 3, 0),
                    new GitInfo("refs/heads/fix-bug", "refs/heads/main")
                ),
                new PullRequest(
                    "35",
                    new Uri("https://www.github.com/wareismymind/notarepo/pull/35"),
                    new Descriptor("Expirement with idea", "Change the source such that it contains an expirement"),
                    new State(PullRequestStatus.Draft, 3, 2),
                    new GitInfo("refs/heads/expirement", "refs/heads/main")
                )
            };
        }
    }
}

