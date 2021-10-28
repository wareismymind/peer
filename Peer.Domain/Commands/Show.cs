using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Show
    {
        private readonly List<IPullRequestFetcher> _fetchers;
        private readonly IPullRequestFormatter _formatter;
        private readonly IConsoleWriter _writer;
        private readonly ISorter<PullRequest>? _sorter;

        public Show(
            IEnumerable<IPullRequestFetcher> fetchers,
            IPullRequestFormatter formatter,
            IConsoleWriter writer,
            ISorter<PullRequest>? sorter = null)
        {
            _fetchers = fetchers.ToList();
            _formatter = formatter;
            _writer = writer;
            _sorter = sorter;
        }

        public async Task<Result<None, ShowError>> ShowAsync(ShowArguments _, CancellationToken token = default)
        {
            var prs = await FetchAllSources(token);
            var sorted = _sorter?.Sort(prs) ?? prs;
            _writer.Display(_formatter.FormatLines(sorted).ToList(), token);
            return Maybe.None;
        }

        private async Task<IEnumerable<PullRequest>> FetchAllSources(CancellationToken token)
        {
            var tasks = _fetchers.Select(async x => await x.GetPullRequestsAsync(token));
            var prs = await Task.WhenAll(tasks);
            var combined = prs.SelectMany(x => x);

            return combined;
        }
    }

    public enum ShowError
    {
        Fire,
    }
}
