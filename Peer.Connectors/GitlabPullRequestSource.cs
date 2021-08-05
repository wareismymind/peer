using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient.Models.MergeRequests.Requests;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer.Connectors
{
    public class GitlabPullRequestSource : IPullRequestSource<GitlabPullRequestSource>
    {
        private readonly AppConfig _config;
        private static IClient<UserGitlabClient> _userClient;

        public GitlabPullRequestSource(AppConfig config, IClient<UserGitlabClient> userClient)
        {
            _config = config;
            _userClient = userClient;
        }

        public async Task<IEnumerable<PeerPullRequest>> FetchPullRequestsAsync()
        {
            var _gitlabClient = _userClient.CreateClient().GitlabClient;

            var prs = await _gitlabClient.MergeRequests.GetAsync(x => x.State = QueryMergeRequestState.Opened);

            var status = prs.Select(x => new PeerPullRequest(
               title: x.Title,
               assignee: x.Assignee.Name,
               id: x.Id
               )).ToList();

            return status;
        }
    }
}
