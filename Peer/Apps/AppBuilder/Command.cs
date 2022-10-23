namespace Peer.Apps.AppBuilder;

public class Command
{
    public IVerb Verb { get; }
    public object Options { get; }

    public Command(IVerb verb, object options)
    {
        Verb = verb;
        Options = options;
    }
}
