using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Peer.Connectors;
using Peer.Domain;
using Peer.Domain.Models;
using Xunit;

namespace Peer.ConnectorApi.Tests
{
    public class PeerPullRequestAPITest<T> where T : class
    {
        public static void GetPullRequestsTest()
        {
            var mockGithubPullRequestSource = new Mock<IPullRequestSource<T>>();
            var mocklist = new Mock<IEnumerable<PeerPullRequest>>();
            var mockpr = new PeerPullRequest("title", "CreatedBy", 1);
            var listpr = new List<PeerPullRequest> { mockpr };
            mockGithubPullRequestSource.Setup(x => x.FetchPullRequestsAsync()).Returns(Task.FromResult<IEnumerable<PeerPullRequest>>(listpr));
            var peerPullRequestAPI = new PeerPullRequestAPI<T>(new List<IPullRequestSource<T>> { mockGithubPullRequestSource.Object });

            var res = peerPullRequestAPI.GetPullRequests();

            Assert.Equal("title", res.Result[0].Title);
            Assert.Equal("CreatedBy", res.Result[0].Assignee);
        }
    }

    public class PeerPullRequestApiTests
    {
        [Fact]
        public void TestGithubPullRequestAPI()
        {
            PeerPullRequestAPITest<GithubPullRequestSource>.GetPullRequestsTest();
        }

        [Fact]
        public void TestAzdoPullRequestAPI()
        {
            PeerPullRequestAPITest<AzdoPullRequestSource>.GetPullRequestsTest();
        }

        [Fact]
        public void TestGitlabPullRequestAPI()
        {
            PeerPullRequestAPITest<GitlabPullRequestSource>.GetPullRequestsTest();
        }
    }
}
