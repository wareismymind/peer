using System;
using Peer.Domain;
using Xunit;

namespace Peer.UnitTests
{
    public class StateTests
    {
        public class Construct
        {
            [Fact]
            public void StatusUndefined_Throws()
            {
                Assert.Throws<ArgumentException>(() => new State((PullRequestStatus)int.MaxValue, 10, 10));
            }

            [Fact]
            public void ActiveCommentsGreaterThanTotalComments_Throws()
            {
                Assert.Throws<ArgumentException>(() => new State(PullRequestStatus.Conflict, 10, 11));
            }

            [Fact]
            public void ValidArgs_Construcsts()
            {
                _ = new State(PullRequestStatus.Conflict, 10, 10);
            }
        }
    }
}
