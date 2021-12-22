using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Show
    {
        private readonly IPullRequestService _pullRequestService;
        private readonly IListFormatter _formatter;
        private readonly IConsoleWriter _writer;
        private readonly ISorter<PullRequest>? _sorter;

        public Show(
            IPullRequestService prService,
            IListFormatter formatter,
            IConsoleWriter writer,
            ISorter<PullRequest>? sorter = null)
        {
            _pullRequestService = prService;
            _formatter = formatter;
            _writer = writer;
            _sorter = sorter;
        }

        public async Task<Result<None, ShowError>> ShowAsync(ShowArguments args, CancellationToken token = default)
        {
            var prs = await _pullRequestService.FetchAllPullRequests(token);
            var sorted = await (_sorter?.Sort(prs) ?? prs).Take(args.Count).ToListAsync(token);
            var lines = _formatter.FormatLines(sorted).ToList();
            _writer.Display(lines, token);
            return Maybe.None;
        }
    }

    public enum ShowError
    {
        Fire,
    }
}
