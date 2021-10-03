using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Peer
{
    public class ConsoleWriter
    {
        private int _previousWriteHeight;

        public void Display(IList<string> lines, CancellationToken token)
        {
            var maxLength = lines.Max(x => x.Length);

            var consoleWidth = Math.Max(maxLength, Console.BufferWidth);
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

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());

            _previousWriteHeight = lines.Count;
        }
    }
}
