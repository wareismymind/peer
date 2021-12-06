using System.Threading;
using System.Threading.Tasks;
using wimm.Secundatives;

namespace Peer.Domain.Commands
{
    public class Details
    {
        private readonly IPullRequestService _prService;
        private readonly IDetailsFormatter _formatter;
        private readonly IConsoleWriter _consoleWriter;

        public Details(
            IPullRequestService prService,
            IDetailsFormatter formatter,
            IConsoleWriter consoleWriter)
        {
            _prService = prService;
            _formatter = formatter;
            _consoleWriter = consoleWriter;
        }

        public async Task<Result<None, FindError>> DetailsAsync(DetailsArguments args, CancellationToken token = default)
        {
            var findResult = await _prService.FindByPartial(args.Partial, token);
            var formatted = findResult.Map(pr => _formatter.Format(pr));

            if (formatted.IsError)
            {
                return formatted.Error;
            }

            _consoleWriter.Display(formatted.Value, token);

            return Maybe.None;
        }
    }
}
