using System.Collections.Generic;
using Peer.Domain.Util;

namespace Peer.Domain.Formatters
{
    //q(cn): Is there just an object that represents the parts of the format? 
    //  if we're talking format it's include/exclude different properties and max
    //  widths...
    //var x = new
    //{
    //    Id = ("Id", 0, 4, true),
    //    Title = ("Title", 1, 40, true),
    //    Status = ("S", 2, 1, true),
    //    Url = ("Url", 3, 35, true)
    //}; or similar sort of structure?
    public class CompactFormatter : IPullRequestFormatter
    {
        private readonly ISymbolProvider _symbolProvider;
        private const char _ellipsis = '\u2026';
        private const string _constructionSign = "\ud83d\udea7";
        private const string _speechBaloon = "\ud83d\udcac";

        private static readonly string _header = $"{"Id",-4} {"Title",-40} {_speechBaloon,-5} {_constructionSign}  Url";
        public CompactFormatter(ISymbolProvider symbolProvider)
        {
            _symbolProvider = symbolProvider;
        }

        public IEnumerable<string> FormatLines(IEnumerable<PullRequest> pullRequests)
        {
            Validators.ArgIsNotNull(pullRequests, nameof(pullRequests));

            yield return _header;

            foreach (var pullRequest in pullRequests)
            {
                yield return CreatePullRequestLine(pullRequest);
            }
        }

        private string CreatePullRequestLine(PullRequest pr)
        {
            var id = PadOrTruncate(pr.Id, 4);
            var title = PadOrTruncate(pr.Descriptor.Title, 40);
            var comments = PadOrTruncate($"{pr.State.ResolvedComments}/{pr.State.TotalComments}", 5);
            var status = _symbolProvider.GetSymbol(pr.State.Status);

            return $"{id} {title} {comments} {status}  {pr.Url}";
        }

        private string PadOrTruncate(string value, int length)
        {
            return value.Length > length
                ? value[0..(length - 1)] + _ellipsis
                : value.PadRight(length);
        }
    }
}
