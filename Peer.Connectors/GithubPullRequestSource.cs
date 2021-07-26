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
    public class GithubPullRequestSource : IPullRequestSource
    {
        private static readonly Regex _paramRegex = new Regex(@"(?<orgName>[\w\-]+)/(?<repoName>[\w\-]+)/issues/\d+");
        private readonly ConfigModel _config;
        private static IClient<GithubClient> _user;

        public GithubPullRequestSource(ConfigModel config, IClient<GithubClient> user)
        {
            _config = config;
            _user = user;
        }

        public async Task<IEnumerable<PeerPullRequest>> FetchPullRequests()
        {
            var userclient = _user.CreateClient().GitClient;

            var issues = await userclient.Search.SearchIssues(new SearchIssuesRequest()
            {
                Archived = false,
                Involves = _config.Github.Name,
                User = _config.Github.Org,
                State = ItemState.Open,
                Type = IssueTypeQualifier.PullRequest
            });

            var prs = await Task.WhenAll(issues.Items.Select(x => GetInfoFromIssue(x)));

            var status = prs.Select(x => new PeerPullRequest(
               Title: x.Title,
               Assignee: x.Assignee
               )).ToList();

            return status;
        }
        private async Task<PullRequest> GetInfoFromIssue(Issue iss)
        {
            var match = _paramRegex.Match(iss.Url);
            var orgName = match.Groups["orgName"].Value;
            var repo = match.Groups["repoName"].Value;
            var pr = await _user.Client.GitClient.PullRequest.Get(orgName, repo, iss.Number);
            return pr;
        }
    }
}
