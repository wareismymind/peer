using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octokit;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer.Connectors
{
    public class GithubPullRequestSource : IPullRequestSource<GithubPullRequestSource>
    {
        private static readonly Regex _paramRegex = new Regex(@"(?<orgName>[\w\-]+)/(?<repoName>[\w\-]+)/issues/\d+");
        private readonly AppConfig _config;
        private static IClient<UserGithubClient> _userClient;
        private static GitHubClient _githubClient;

        public GithubPullRequestSource(AppConfig config, IClient<UserGithubClient> userClient)
        {
            _config = config;
            _userClient = userClient;
        }

        public async Task<IEnumerable<PeerPullRequest>> FetchPullRequests()
        {
            _githubClient = _userClient.CreateClient().GitClient;

            var issues = await _githubClient.Search.SearchIssues(new SearchIssuesRequest()
            {
                Archived = false,
                Involves = _config.Github.Name,
                User = _config.Github.Org,
                State = ItemState.Open,
                Type = IssueTypeQualifier.PullRequest
            });

            var prs = await Task.WhenAll(issues.Items.Select(x => GetInfoFromIssue(x)));

            var status = prs.Select(x => new PeerPullRequest(
               title: x.Title,
               assignee: x.Assignee
               )).ToList();

            return status;
        }
        private async Task<PullRequest> GetInfoFromIssue(Issue iss)
        {
            var match = _paramRegex.Match(iss.Url);
            var orgName = match.Groups["orgName"].Value;
            var repo = match.Groups["repoName"].Value;
            var pr = await _githubClient.PullRequest.Get(orgName, repo, iss.Number);
            return pr;
        }
    }
}
