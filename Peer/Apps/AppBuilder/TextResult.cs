namespace Peer.Apps.AppBuilder;

public abstract class TextResult
{
    public string Text { get; }

    protected TextResult(string text)
    {
        Text = text;
    }
}
