using System;
using System.Threading.Tasks;
using CommandLine;
using Peer.Verbs;

namespace Peer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var res = await Parser.Default.ParseArguments<ShowOptions, OpenOptions>(args)
                .MapResult(
                    (ShowOptions opts) => Task.FromResult("Whee"),
                    err => Task.FromResult("Saaad"));

            Console.WriteLine(res);
        }
    }
}
