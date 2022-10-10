using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Peer.Parsing.CommandLine;

namespace Peer.App.AppBuilder;

public class CommandLineParser
{
    private readonly List<IVerb> _verbs;

    public CommandLineParser(IEnumerable<IVerb> verbs)
    {
        _verbs = verbs.ToList();
    }

    public ParserResult<object> Parse(string[] args)
    {
        //I'm just dealing with one layer of nesting right now. I'm lazy
        var noDash = args.Count(x => !x.StartsWith('-'));
        var topLevel = _verbs.FirstOrDefault(x => x.Name == args.FirstOrDefault());
        var subs = topLevel?.Subs.ToArray() ?? Array.Empty<IVerb>();
        var parser = new Parser(config =>
        {
            config.AutoHelp = true;
            config.AutoVersion = true;
            config.IgnoreUnknownArguments = false;
        });


        if (noDash == 1 || topLevel == null)
        {
            //CN - Being sneaky here
            if (subs.Any() && topLevel?.Handler == null)
            {
                return parser.ParseArguments(args[1..], subs.Select(x => x.Type).ToArray());
            }

            return parser.ParseArguments(args, _verbs.Select(x => x.Type).ToArray());
        }
        else
        {
            if (!subs.Any() || args.Length == 1)
            {
                return parser.ParseArguments(args, _verbs.Select(x => x.Type).ToArray());
            }

            return parser.ParseArguments(args[1..], topLevel.Subs.Select(x => x.Type).ToArray());
        }
    }
}