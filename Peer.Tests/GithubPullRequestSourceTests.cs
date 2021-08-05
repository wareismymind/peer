using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Octokit;
using Peer.Connectors;
using Peer.Domain.Models;
using Peer.User;
using Xunit;

namespace Peer.Connectors.Tests
{
    public class GithubPullRequestSourceTests
    {
        [Fact()]
        public void GithubPullRequestSourceTest()
        {
            var config = new AppConfig();
            config.Github = new GithubConfig();
            config.Github.Name = "A";
            config.Github.Org = "B";
            var mockClient = new Mock<IClient<UserGithubClient>>();
            var userClient = new UserGithubClient(config);
            var githubClient = new Mock<IGitHubClient>();
            var searchIssuesResult = new Mock<SearchIssuesResult>().Object;
            var mockpr = new PullRequest();
            var listpr = new List<PullRequest> { mockpr };
            githubClient.Setup(x => x.Search.SearchIssues(It.IsAny<SearchIssuesRequest>())).Returns(Task.FromResult<SearchIssuesResult>(searchIssuesResult));
            githubClient.Setup(x => x.PullRequest.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult<PullRequest>(new PullRequest(1)));
            mockClient.Setup(x => x.CreateClient()).Returns(userClient);
            userClient.GitClient = githubClient.Object;
            var azdoPullRequestSource = new GithubPullRequestSource(config, mockClient.Object);

            var res = azdoPullRequestSource.FetchPullRequestsAsync();

            Assert.Equal(1, res.Result.First<PeerPullRequest>().Id);
        }
    }
}
