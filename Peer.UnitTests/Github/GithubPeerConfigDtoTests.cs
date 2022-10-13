using System;
using System.Collections.Generic;
using Peer.GitHub;
using Xunit;

namespace Peer.UnitTests.Github
{
    public class GitHubPeerConfigDtoTests
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
                Assert.Equal(GitHubConfigError.UsernameInvalid, res.Error);
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
                Assert.Equal(GitHubConfigError.AccessTokenInvalid, res.Error);
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            [InlineData(101)]
            public void PageSizeEmptyOrWhitespace_ReturnsPageSizeInvalid(int count)
            {
                var underTest = CreateConfig(x => x.Configuration.Count = count);
                var res = underTest.Into();

                Assert.True(res.IsError);
                Assert.Equal(GitHubConfigError.PageSizeInvalid, res.Error);
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
                Assert.Equal(GitHubConfigError.InvalidOrgConfig, res.Error);
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

        public static GitHubHandlerConfig CreateConfig(Action<GitHubHandlerConfig> customization = null)
        {
            var value = new GitHubHandlerConfig()
            {
                Name = "wat",
                Configuration = new GitHubPeerConfigDto
                {
                    AccessToken = "waka",
                    Username = "somevalidName",
                    ExcludedOrgs = new List<string>(),
                    Orgs = new List<string>() { "dotnet" },
                    Count = 10
                }
            };

            customization?.Invoke(value);
            return value;
        }
    }
}
