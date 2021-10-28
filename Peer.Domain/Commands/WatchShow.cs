using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
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

        public async Task<Result<None, ShowError>> WatchAsync(WatchArguments watchConfig, ShowArguments args, CancellationToken token)
        {
            _consoleWriter.Clear();

            while (!token.IsCancellationRequested)
            {
                var res = await _show.ShowAsync(args, token);

                if (res.IsError)
                {
                    return res;
                }

                if (!token.IsCancellationRequested)
                {
                    await Task.Delay(watchConfig.IntervalSeconds, token);
                }
            }

            return Maybe.None;
        }
    }
}
