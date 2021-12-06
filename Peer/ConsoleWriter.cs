using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var consoleWidth = Math.Min(maxLength, Console.WindowWidth - 1);
            var writeHeight = Math.Max(lines.Count, _previousWriteHeight);

            var sb = new StringBuilder((consoleWidth + 1) * writeHeight);

            foreach (var line in lines)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var split = line.Split(new string[] { "\r\n", "\n"}, StringSplitOptions.None);

                //CN -- This should probably be done within the formatter but this is just here for now while I experiment
                foreach (var subline in split)
                {
                    var padded = subline.PadRight(consoleWidth);

                    if (padded.Length > consoleWidth)
                    {
                        var remaining = padded.AsSpan();
                        while (remaining.Length > consoleWidth)
                        {
                            sb.Append(remaining[..consoleWidth]);
                            sb.AppendLine();
                            remaining = remaining[consoleWidth..];
                        }
                        sb.Append(remaining);
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(padded);
                    }
                }

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
            DebugTimeStamp();
        }

        public void Clear()
        {
            Console.Clear();
        }

        [Conditional("DEBUG")]
        private static void DebugTimeStamp()
        {
            //CN: To make it easier to see if the console is actually refreshing or not.
            Console.WriteLine(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
