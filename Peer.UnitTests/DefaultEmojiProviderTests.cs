using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Domain.Exceptions;
using Xunit;

namespace Peer.UnitTests
{
    public class DefaultEmojiProviderTests
    {
        public class GetSymbolPRStatus
        {
            public static IEnumerable<object[]> AllStatuses =>
                Enum.GetValues<PullRequestStatus>()
                .Select(x => new object[] { x });

            [Fact]
            public void StatusUndefined_Throws()
            {
                var underTest = Construct();
                Assert.Throws<UnreachableException>(() =>
                    underTest.GetSymbol((PullRequestStatus)int.MaxValue));
            }

            [Theory]
            [MemberData(nameof(AllStatuses))]
            public void StatusDefined_ReturnsSymbol(PullRequestStatus status)
            {
                var underTest = Construct();

                var symbol = underTest.GetSymbol(status);

                Assert.NotNull(symbol);
                Assert.NotEmpty(symbol);
            }

            private static DefaultEmojiProvider Construct()
            {
                return new DefaultEmojiProvider();
            }
        }

        public class GetSymbolCheckStatusAndResult
        {
            [Fact]
            public void StatusResultComboHasNoSymbol_ReturnsNone()
            {
                var underTest = Construct();
                var symbol = underTest.GetSymbol(CheckStatus.Queued, CheckResult.Success);
                Assert.False(symbol.Exists);
            }

            [Fact]
            public void CheckStatusUndefined_Throws()
            {
                var underTest = Construct();
                Assert.Throws<ArgumentException>(
                    () => underTest.GetSymbol((CheckStatus)int.MaxValue, CheckResult.Success));

            }

            [Fact]
            public void CheckResultUndefined_Throws()
            {
                var underTest = Construct();
                Assert.Throws<ArgumentException>(
                    () => underTest.GetSymbol(CheckStatus.Queued, (CheckResult)int.MaxValue));
            }

            [Fact]
            public void StatusResultComboHasSymbol_ReturnsValue()
            {
                var underTest = Construct();
                var symbol = underTest.GetSymbol(CheckStatus.Completed, CheckResult.Success);
                Assert.True(symbol.Exists);
            }

            private static DefaultEmojiProvider Construct()
            {
                return new DefaultEmojiProvider();
            }
        }
    }
}
