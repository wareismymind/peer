using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Octokit;
using Peer.Domain;
using Peer.Domain.Models;
using Peer.User;

namespace Peer.Connectors
{
    public class GithubPullRequestSource : IPullRequestSource
    {
        private static IClient _user;
        private readonly IConfiguration _config;
        private static readonly Regex _paramRegex = new Regex(@"(?<orgName>[\w\-]+)/(?<repoName>[\w\-]+)/issues/\d+");

        GithubPullRequestSource(IConfiguration config, IClient user)
        {
            _config = config;
            _user = user;
        }

        public async Task<IEnumerable<PeerPullRequest>> FetchPullRequests()
        {
            var userclient = _user.CreateClient().Client;

            var queries = new List<Task<SearchIssuesResult>>();
            queries.Add(userclient.Search.SearchIssues(new SearchIssuesRequest()
            {
                Archived = false,
                Involves = _config["UserConfig:Name"],
                User = _config["UserConfig:Name"],
                Is = new List<IssueIsQualifier> { IssueIsQualifier.PullRequest, IssueIsQualifier.Open },
            }));
            var issues = await Task.WhenAll(queries);
            var prs = await Task.WhenAll(issues.SelectMany(x => x.Items).Select(x => GetInfoFromIssue(x)));

            var status = prs.Select(x => new PeerPullRequest(
               Title: x.pr.Title,
               Assignee: x.pr.Assignee
               )).ToList();

            return status;
        }
        private async Task<(Repository repo, PullRequest pr, Issue iss)> GetInfoFromIssue(Issue iss)
        {
            var match = _paramRegex.Match(iss.Url);
            var orgName = match.Groups["orgName"].Value;
            var repo = match.Groups["repoName"].Value;
            var pr = await _user.Client.Client.PullRequest.Get(orgName, repo, iss.Number);
            var repoObj = await _user.Client.Client.Repository.Get(orgName, repo);
            return (repoObj, pr, iss);
        }
    }
}
