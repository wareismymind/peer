using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Commands
{
    public class WatchShow
    {
        private readonly Show _show;
        private readonly IConsoleWriter _consoleWriter;

        public WatchShow(Show show, IConsoleWriter consoleWriter)
        {
            _show = show;
            _consoleWriter = consoleWriter;
        }

        public async Task<Result<None, ShowError>> WatchAsync(ShowArguments args, CancellationToken token)
        {
            var consecutiveFailures = 0;
            _consoleWriter.Clear();

            while (!token.IsCancellationRequested)
            {
                var res = await _show.ShowAsync(args, token);

                consecutiveFailures = res.IsError
                    ? consecutiveFailures + 1
                    : 0;

                if (consecutiveFailures > _show.Config.WatchMaxConsecutiveShowFailures)
                {
                    _consoleWriter.Clear();
                    _consoleWriter.Display(new List<string>
                    {
                        $"error: too many consecutive errors",
                    }, token);

                    return ShowError.Fire;
                }

                try
                {
                    await Task.Delay(_show.Config.WatchIntervalSeconds, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            return Maybe.None;
        }
    }
}
