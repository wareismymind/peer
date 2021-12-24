using System;
using Moq;
using Peer.Domain;
using Peer.Domain.Commands;
using Peer.UnitTests.Util;
using Xunit;

namespace Peer.UnitTests.Formatters
{
    public class DetailsFormatterTests
    {
        public class Format
        {
            private readonly Mock<ICheckSymbolProvider> _symbolProvider = new();

            [Fact]
            public void PullRequestNull_Throws()
            {
                var underTest = Construct();
                Assert.Throws<ArgumentNullException>("pullRequest", () => underTest.Format(null));
            }

            [Fact]
            public void NoChecksExist_DoesntWriteChecks()
            {
                var pr = ValueGenerators.CreatePullRequest();
                var underTest = Construct();

                var results = underTest.Format(pr);
                Assert.DoesNotContain("Checks", results);
            }

            private DetailsFormatter Construct()
            {
                return new DetailsFormatter(_symbolProvider.Object);
            }
        }
    }
}
