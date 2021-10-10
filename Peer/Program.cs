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
                    (ShowOptions _) => Task.FromResult("This is where show will happen when it's done"),
                    (OpenOptions _) => Task.FromResult("This is where open will happen when it's done"),
                    (ConfigOptions _) => Task.FromResult("This is where config will happen when it's done"),
                    err => Task.FromResult("Saaad"));

            Console.WriteLine(res);
        }
    }
}
