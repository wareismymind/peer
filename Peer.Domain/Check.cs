using System;
using Peer.Domain.Util;

namespace Peer.Domain
{
    public class Check
    {
        public string Name { get; }
        public string? Description { get; }
        public CheckStatus Status { get; }
        public CheckResult Result { get; }
        public Uri Url { get; }

        public Check(string name, string? description, Uri url, CheckStatus status, CheckResult result)
        {
            Validators.ArgIsDefined(status);
            Validators.ArgIsDefined(result);

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Status = status;
            Result = result;
        }
    }
}
