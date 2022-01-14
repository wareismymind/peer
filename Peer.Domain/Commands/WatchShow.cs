using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class WatchShow
    {
        private const int _maxConsecutiveErrors = 5;

        private readonly Show _show;
        private readonly IConsoleWriter _consoleWriter;

        public WatchShow(Show show, IConsoleWriter consoleWriter)
        {
            _show = show;
            _consoleWriter = consoleWriter;
        }

        public async Task<Result<None, ShowError>> WatchAsync(WatchArguments watchConfig, ShowArguments args, CancellationToken token)
        {
            int consecutiveErrors = 0;
            _consoleWriter.Clear();

            while (!token.IsCancellationRequested)
            {
                var res = await _show.ShowAsync(args, token);

                consecutiveErrors = res.IsError
                    ? consecutiveErrors + 1
                    : 0;

                if (consecutiveErrors > _maxConsecutiveErrors)
                {
                    _consoleWriter.Clear();
                    _consoleWriter.Display(new List<string>
                    {
                        $"error: too many consecutive errors",
                    }, token);

                    return ShowError.Fire;
                }

                await Task.Delay(watchConfig.IntervalSeconds, token);
            }

            return Maybe.None;
        }
    }
}
