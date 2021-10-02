using System.Text;

namespace Peer
{
    public class ConsoleWriter
    {
        private int _previousWriteLength;

        public void Display(IList<string> lines, CancellationToken token)
        {
            var maxLength = lines.Max(x => x.Length);
            var consoleWidth = Console.BufferWidth > maxLength ? Console.BufferWidth : maxLength;
            var sb = new StringBuilder((consoleWidth+1)*_previousWriteLength);
            
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
