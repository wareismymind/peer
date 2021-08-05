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
    public class UserGithubClientTests
    {
        [Fact]
        public void CreateClientTest()
        {
            var config = new AppConfig();
            config.Github = new GithubConfig();
            config.Github.Token = "fakeToken";
            config.Github.ProductHeaderValue = "headerVal";

            var userClient = new UserGithubClient(config);

            var gitClient = userClient.CreateClient();

            Assert.Equal(userClient, gitClient);
            Assert.NotNull(userClient.GitClient);
        }
    }
}
