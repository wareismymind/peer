using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Peer.Domain;

namespace Peer
{
    public class ConsoleConfig
    {
        public bool Inline { get; }

        public ConsoleConfig(bool inline)
        {
            Inline = inline;
        }
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly ConsoleConfig _config;
        private int _previousWriteHeight;

        public ConsoleWriter(ConsoleConfig config)
        {
            _config = config;
        }

        public void Display(IList<string> lines, CancellationToken token)
        {
            var maxLength = lines.Max(x => x.Length);
            var consoleWidth = Math.Max(maxLength, Console.WindowWidth - 1);
            var writeHeight = Math.Max(lines.Count, _previousWriteHeight);

            var sb = new StringBuilder((consoleWidth + 1) * writeHeight);

            foreach (var line in lines)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var padded = line.PadRight(consoleWidth);
                sb.AppendLine(padded);
            }

            var filler = new string(' ', consoleWidth);
            var fillAmount = writeHeight - lines.Count;

            for (var i = 0; i <= fillAmount; i++)
            {
                sb.AppendLine(filler);
            }

            if (!_config.Inline)
            {
                Console.SetCursorPosition(0, 0);
            }

            Console.Write(sb.ToString());

            _previousWriteHeight = lines.Count;
        }
    }
}
