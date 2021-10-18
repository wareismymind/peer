using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Peer.Domain.Commands;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class PeerApplication : IPeerApplication
    {
        private readonly List<IPullRequestFetcher> _fetchers;
        private readonly IPullRequestFormatter _formatter;
        private readonly IConsoleWriter _writer;

        public PeerApplication(
            IEnumerable<IPullRequestFetcher> fetchers,
            IPullRequestFormatter formatter,
            IConsoleWriter writer)
        {
            _fetchers = fetchers.ToList();
            _formatter = formatter;
            _writer = writer;
        }

        public async Task<Result<None, ShowError>> ShowAsync(Show showOptions, CancellationToken token = default)
        {
            var tasks = _fetchers.Select(async x => await x.GetPullRequestsAsync());
            var prs = await Task.WhenAll(tasks);

            var combined = prs.SelectMany(x => x);

            _writer.Display(_formatter.FormatLines(combined).ToList(), token);

            return Maybe.None;
        }
    }
}
