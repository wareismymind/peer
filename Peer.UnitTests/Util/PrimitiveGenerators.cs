using System;
using System.Collections.Generic;
using System.Text;
using Peer.Domain;

namespace Peer.UnitTests.Util
{
    public static class PrimitiveGenerators
    {
        private static readonly Random _rando = new();

        public static int GetInt(int min = 0, int max = int.MaxValue)
        {
            return _rando.Next(min, max);
        }

        public static string GetString(int length)
        {
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                sb.Append(GetChar());
            }

            return sb.ToString();
        }

        public static T RandomEnumValue<T>() where T : struct, Enum
        {
            var values = Enum.GetValues<T>();
            var value = values[GetInt(max: values.Length)];
            return value;
        }

        public static string RandomBranchName()
        {
            return $"refs/heads/{GetString(10)}";
        }

        public static char GetChar()
        {
            return (char)_rando.Next('a', 'z');
        }
    }

    public static class ValueGenerators
    {
        public static PullRequest CreatePullRequest()
        {
            var id = PrimitiveGenerators.GetInt(max: 99999);
            return new PullRequest(
                   id.ToString(),
                   new Identifier(id.ToString(), "wareismymind", "doot", "github"),
                   new Uri($"https://github.com/wareismymind/doot/pulls/{id}"),
                   new Descriptor(PrimitiveGenerators.GetString(20), PrimitiveGenerators.GetString(30)),
                   new State(PrimitiveGenerators.RandomEnumValue<PullRequestStatus>(), 10, 10),
                   new GitInfo(PrimitiveGenerators.RandomBranchName(), PrimitiveGenerators.RandomBranchName()),
                   new List<Check>());
                
        }
    }
}
