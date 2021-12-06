using System;
using System.IO;
using System.Threading.Tasks;

namespace Peer.Domain.Commands
{
    public class Config
    {
        private static readonly string _configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "peer.json");

        private const string _configHelp = @"
{
  ""Peer"": {
    //optional: The amount of time between calls to providers when using the --watch flag
    ""WatchIntervalSeconds"": 30
  },
  ""Providers"": {
    //The type of the provider you're configuring (currently there's only github!)
    ""github"": [{
        //required: a friendly name for this provider
        ""Name"": ""required: a friendly name for this provider"",
        ""Configuration"": {
          //required: your API token
          ""AccessToken"": """",
          //optional: the github username you're interested in investigating, alternatively we'll fetch yours from the api
          ""Username"": """",
          //optional: Orgs can be either be traditional (github, wareismymind) or a username for user's repos 
          // if left empty we'll look at all orgs available to your user
          ""Orgs"": [""myorg"", ""wareismymind"", ""someuser""],
          //optional: Orgs that you'd like to exclude from the output, only really makes sense if no orgs are set
          ""ExcludedOrgs"": [],
          //optional: indicates the number of pull requests that will be listed, should be number between 1 and 100.
          // if not provided will default to 20.
          ""Count"": 20
        }
    }]
  }
}
";
        public static Task ConfigAsync()
        {
            Console.Error.WriteLine("Hey lets get you set up and working with Peer!");
            Console.Error.WriteLine($"Toss the following into this location: {_configFile} and fill in values for your github account");
            Console.WriteLine(_configHelp);
            return Task.CompletedTask;
        }
    }
}
