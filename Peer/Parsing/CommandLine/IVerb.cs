using System;
using System.Collections.Generic;
using Peer.Verbs;

namespace Peer.Parsing.CommandLine;

public interface IVerb
{
    string Name { get; }
    Type Type { get; }
    IEnumerable<IVerb> Subs { get; }
    IHandler? Handler { get; }
    IHelpTextFormatter? CustomHelp { get; }
}
