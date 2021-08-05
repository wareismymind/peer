using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Moq;
using Peer.Connectors;
using Peer.Domain.Models;
using Peer.User;
using Xunit;

namespace Peer.Connectors.Tests
{
    public class AzdoPullRequestSourceTests
    {
        [Fact]
        public void FetchPullRequestsTest()
        {
            /*var mockIuserClient = new Mock<IClient<UserAzdoClient>>();
            var mockUserClient = new Mock<UserAzdoClient>();
            var mockObj = new UserAzdoClient(It.IsAny<AppConfig>());
            var mockIobj = mockIuserClient.Object;
            var mockGithttpClient = new GitHttpClient(It.IsAny<Uri>(), It.IsAny<VssCredentials>());
            var mockProjectHttpClient = new ProjectHttpClient(It.IsAny<Uri>(), It.IsAny<VssCredentials>());
            mockObj.AzClient = mockGithttpClient;
            mockObj.AzProjClient = mockProjectHttpClient;
            var mockTeamProject = new Mock<TeamProjectReference>();
            var mockPullRequest = new Mock<GitPullRequest>().Object;
            var pagedList = new PagedList<TeamProjectReference> { mockTeamProject.Object };
            var mockPrList = new List<GitPullRequest> { mockPullRequest };
            mockPullRequest.Title = "test_title";
            mockIuserClient.Setup(x => x.CreateClient()).Returns(mockObj);
            mockProjectHttpClient.Setup(x => x.GetProjects(It.IsAny<ProjectState>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult<IPagedList<TeamProjectReference>>(pagedList));
            mockGithttpClient.Setup(x => x.GetPullRequestsByProjectAsync(It.IsAny<Guid>(), It.IsAny<GitPullRequestSearchCriteria>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<List<GitPullRequest>>(mockPrList));
            var azdoPullRequestSource = new AzdoPullRequestSource(It.IsAny<AppConfig>(), mockObj);

            var res = azdoPullRequestSource.FetchPullRequestsAsync();

            Assert.Equal("title", res.Result.First<PeerPullRequest>().Title);*/
        }
    }
}
