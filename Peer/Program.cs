using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peer.Domain;
using Peer.Domain.Formatters;

namespace Peer
{
    public static class Program
    {
        private static readonly Random _rando = new();

        public static async Task Main(string[] _)
        {
            await Show();
        }

        public static async Task Show()
        {
            var fetcher = new GitHubRequestFetcher();
            var pullRequests = await fetcher.GetPullRequestsAsync();
            var formatter = new CompactFormatter(new DefaultEmojiProvider());
            var writer = new ConsoleWriter();
            var output = formatter.FormatLines(pullRequests).ToList();
            writer.Display(output, true, default);
        }

        public static async Task OldMain(string[] _)
        {
            var consoleJiggy = new ConsoleWriter();

            var lines = new[]
            {
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "doot",
                "waka",
                "fsd;lkfjsd;lfkajsd;lkfjasdf",
                "sdfka;sldkfjas;ldkfja;sdlkfjas;dlfkjasd;flk",
                "asjsdlfkjds",
                "dooooot",
                "wakaaaa",
                "ASDJKHASLKJDHASLKJDHALKSJD",
                "LAST_LINE"
            };

            //Console.Clear();
            //for (var i = 0; i < 100; i++)
            //{
            //    consoleJiggy.Display(lines.TakeLast(1 + _rando.Next(lines.Length)).OrderByDescending(x => x == "LAST_LINE" ? int.MinValue : _rando.Next(0, 200)).ToList(), default);
            //    await Task.Delay(30);
            //}
            Console.OutputEncoding = Encoding.UTF8;
            var prList = Enumerable.Range(0, Enum.GetValues<PullRequestStatus>().Length).Select(x =>
            {
                return new PullRequest(
                   x.ToString(),
                   new Uri("https://github.com/wareismymind/doot/pulls/123"),
                   //new Uri("https://github.com/titleThingasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd/doot/pulls/123"),
                   new Descriptor("titleThingasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasd", "Descriptybitasdasdaasdjfhgbaskdjhfgasdkjfhgaskdjfhgaskdjfhgasdkfs"),
                   new State((PullRequestStatus)x, 10, 10),
                   new GitInfo("refs/heads/doot", "refs/heads/main"));
            }).ToList();

            var compactFormatter = new CompactFormatter(new DefaultEmojiProvider());
            var formatted = compactFormatter.FormatLines(prList).ToList();
            consoleJiggy.Display(formatted, true, default);
        }
    }
}
