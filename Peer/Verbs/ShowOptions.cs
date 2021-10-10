using CommandLine;

namespace Peer.Verbs
{
    [Verb("show", isDefault: true, HelpText = "Display pull requests assigned to your account")]
    public class ShowOptions
    {
        //CN: No filters or anything atm. 
    }
}
