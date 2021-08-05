using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain.Models;
using Peer.User;
using Xunit;

namespace Peer.User.Tests
{
    public class UserGitlabClientTests
    {
        [Fact]
        public void UserGitlabClientTest()
        {
            var config = new AppConfig();
            config.Gitlab = new GitlabConfig();
            config.Gitlab.Token = "fakeToken";
            config.Gitlab.CollectionUri = "https://beyondtrust.com";

            var userClient = new UserGitlabClient(config);

            var gitClient = userClient.CreateClient();

            Assert.Equal(userClient, gitClient);
            Assert.NotNull(userClient.GitlabClient);
        }
    }
}
