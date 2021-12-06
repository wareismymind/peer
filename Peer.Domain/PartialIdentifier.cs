using System;
using System.Collections.Generic;
using Peer.Domain.Util;
using wimm.Secundatives;

namespace Peer.Domain
{
    public class PartialIdentifier
    {
        public string Value { get; set; }
        public IEnumerable<string> Segments { get; set; }

        public static Result<PartialIdentifier, ParsePartialError> Parse(string toParse)
        {
            Validators.ArgIsNotNull(toParse);
            var split = toParse.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 0)
            {
                return ParsePartialError.NoSegmentsToMatch;
            }

            if (split.Length > 4)
            {
                return ParsePartialError.TooManySegments;
            }

            return new PartialIdentifier(toParse);
        }

        private PartialIdentifier(string rawPartial)
        {
            Value = rawPartial;
            Segments = rawPartial.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
