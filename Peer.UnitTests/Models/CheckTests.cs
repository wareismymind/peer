using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests.Models
{
    public class CheckTests
    {
        public class Construct
        {
            [Fact]
            public void NameNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    new Check(
                    null,
                    "doot",
                    new Uri("https://github.com"),
                    CheckStatus.InProgress,
                    CheckResult.Unknown));
            }

            [Fact]
            public void UrlNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    new Check(
                    "waka",
                    "doot",
                    null,
                    CheckStatus.InProgress,
                    CheckResult.Unknown));
            }

            [Fact]
            public void StatusUndefined_Throws()
            {
                Assert.Throws<ArgumentException>(() =>
                    new Check(
                    "waka",
                    "doot",
                    new Uri("https://github.com"),
                    (CheckStatus)int.MaxValue,
                    CheckResult.Unknown));
            }

            [Fact]
            public void ResultUndefined_Throws()
            {
                Assert.Throws<ArgumentException>(() =>
                    new Check(
                    "waka",
                    "doot",
                    new Uri("https://github.com"),
                    CheckStatus.InProgress,
                    (CheckResult)int.MaxValue));
            }

            [Fact]
            public void ValidArgs_Constructs()
            {
                _ = new Check(
                    "waka",
                    "doot",
                    new Uri("https://github.com"),
                    CheckStatus.InProgress,
                    CheckResult.Unknown);
            }
        }
    }
}
