using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.MergeRequests.Responses;
using Moq;
using Octokit;
using Peer.Connectors;
using Peer.Domain.Models;
using Peer.User;
using Xunit;

namespace Peer.Connectors.Tests
{
    public class GitlabPullRequestSourceTests
    {
        [Fact]
        public void GitlabPullRequestSourceTest()
        {
            var mockClient = new Mock<IClient<UserGitlabClient>>();
            var userClient = new UserGitlabClient(It.IsAny<AppConfig>());
            var gitlabClient = new Mock<IGitLabClient>();
            var mockpr = new MergeRequest();
            mockpr.Id = 1;
            gitlabClient.Setup(x => x.MergeRequests.GetAsync(It.IsAny<ProjectId>(), It.IsAny<Action<ProjectMergeRequestsQueryOptions>>())).Returns(Task.FromResult<IList<MergeRequest>>(new List<MergeRequest> { mockpr }));
            mockClient.Setup(x => x.CreateClient()).Returns(userClient);
            userClient.GitlabClient = gitlabClient.Object;
            var gitlabPullRequestSource = new GitlabPullRequestSource(It.IsAny<AppConfig>(), mockClient.Object);

            var res = gitlabPullRequestSource.FetchPullRequestsAsync();

            Assert.Equal(1, res.Result.First<PeerPullRequest>().Id);
        }
    }
}
