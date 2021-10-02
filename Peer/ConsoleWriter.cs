using System.Text;

namespace Peer
{
    public class ConsoleWriter
    {
        private int _previousWriteLength;

        public void Display(IList<string> lines, CancellationToken token)
        {
            var consoleWidth = Console.BufferWidth;
            var sb = new StringBuilder();
            
            foreach (var line in lines)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (consoleWidth < line.Length)
                {
                    consoleWidth = line.Length;
                }

                var padded = line.PadRight(consoleWidth);
                sb.AppendLine(padded);
            }

            var filler = new string(' ', consoleWidth);
            var fillAmount = _previousWriteLength - lines.Count;
            for (int i = 0; i <= fillAmount; i++)
            {
                sb.AppendLine(filler);
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());

            _previousWriteLength = Console.CursorTop + 1;
        }
    }
}
