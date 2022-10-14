using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Peer.Apps;
using Peer.Apps.AppBuilder;
using Xunit;

namespace Peer.UnitTests.Apps;

public class AppTests
{
    public class Construct
    {
        private readonly Mock<IVerb> _verb = new();
        private readonly Mock<ICommandLineParser> _parser = new();

        [Fact]
        public void Constructs()
        {
            var verbs = new List<IVerb> { _verb.Object };
            _ = new App(_parser.Object, verbs);
        }

        [Fact]
        public void VerbsEmpty_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => new App(_parser.Object, new List<IVerb>()));
        }
    }

    public class RunAsync
    {
        public static async Task ParseFails_ReturnsNonZero()
        {

        }
    }
}
