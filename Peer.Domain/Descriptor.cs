using System;

namespace Peer.Domain
{
    public class Descriptor
    {
        public string Title { get; }
        public string Description { get; }

        public Descriptor(string title, string description)
        {
            Validators.ArgIsNotNullEmptyOrWhitespace(title, nameof(title));

            Title = title;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
