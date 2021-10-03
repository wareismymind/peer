using System;

namespace Peer.Domain
{
    public class GitInfo
    {
        public string Source { get; }
        public string Target { get; }

        public GitInfo(string source, string target)
        {
            Validators.ArgIsNotNullEmptyOrWhitespace(source, nameof(source));
            Validators.ArgIsNotNullEmptyOrWhitespace(target, nameof(target));

            Source = source;
            Target = target;
        }
    }
}
