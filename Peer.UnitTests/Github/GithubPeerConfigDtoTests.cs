using System;
using System.Collections.Generic;
using Peer.GitHub;
using Xunit;

namespace Peer.UnitTests.Github
{
    public class GithubPeerConfigDtoTests
    {
        public class Into
        {
            [Theory]
            [InlineData("")]
            [InlineData("\r\t\r\n  ")]
            public void UsernameEmptyOrWhitespace_ReturnsUsernameInvalid(string username)
            {
                var underTest = CreateConfig(x => x.Configuration.Username = username);
                var res = underTest.Into();

                Assert.True(res.IsError);
                Assert.Equal(GithubConfigError.UsernameInvalid, res.Error);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("\r\t\r\n  ")]
            public void AccessTokenNullEmptyOrWhitespace_ReturnsBadToken(string token)
            {
                var underTest = CreateConfig(x => x.Configuration.AccessToken = token);
                var res = underTest.Into();

                Assert.True(res.IsError);
                Assert.Equal(GithubConfigError.AccessTokenInvalid, res.Error);
            }

            [Fact]
            public void OrgsAndExcludedOrgsDefined_ReturnsInvalidOrgConfig()
            {
                var underTest = CreateConfig(x =>
                {
                    x.Configuration.ExcludedOrgs = new List<string>() { "wareismymind" };
                    x.Configuration.Orgs = new List<string>() { "dotnet" };
                });

                var res = underTest.Into();
                Assert.True(res.IsError);
                Assert.Equal(GithubConfigError.InvalidOrgConfig, res.Error);
            }

            [Fact]
            public void ValidConfig_Succeeds()
            {
                var underTest = CreateConfig();
                var res = underTest.Into();
                Assert.True(res.IsValue);
            }

            [Fact]
            public void ValidConfigNullUsername_Succeeds()
            {
                var underTest = CreateConfig(x => x.Configuration.Username = null);
                var res = underTest.Into();
                Assert.True(res.IsValue);
            }
        }

        public static GithubHandlerConfig CreateConfig(Action<GithubHandlerConfig> customization = null)
        {
            var value = new GithubHandlerConfig()
            {
                Name = "wat",
                Configuration = new GithubPeerConfigDto
                {
                    AccessToken = "waka",
                    Username = "somevalidName",
                    ExcludedOrgs = new List<string>(),
                    Orgs = new List<string>() { "dotnet" }
                }
            };

            customization?.Invoke(value);
            return value;
        }
    }
}
