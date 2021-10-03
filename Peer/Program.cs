using System;
using System.Linq;
using System.Threading.Tasks;

namespace Peer
{
    public static class Program
    {
        private static readonly Random _rando = new();

        public static async Task Main(string[] _)
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

            for (var i = 0; i < 100; i++)
            {
                consoleJiggy.Display(lines.TakeLast(1 + _rando.Next(lines.Length)).OrderByDescending(x => x == "LAST_LINE" ? int.MinValue : _rando.Next(0, 200)).ToList(), default);
                await Task.Delay(30);
            }
        }
    }
}
