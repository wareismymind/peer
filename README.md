# Peer - A pull request visualizer for github (and others)

Peer helps you track down and visualize your PRs across organizations and accounts (and eventually™ across git providers).

Peer is meant to be easy to use, cross platform and helpful!

## Statuses

Pull request statuses are all emoji to save space (and because why not?) the default format provider gives the following mapping:

<pre>
    Unknown =>          ❔
    Conflict =>         &#x2694;&#xFE0F;,
    Draft =>            📃,
    ActionsPending =>   ⏳,
    ActionsQueued =>    💤,
    Stale =>            🍞,
    FailedChecks =>     🔥,
    AwaitingReview =>   🚩,
    FixesRequested =>   ❌,
    ReadyToMerge =>     &#x2714;&#xFE0F;,
    Merged =>           🎊 (You shouldn't really see this one)
</pre>
## Installing peer

Peer is available as a standalone windows executable on scoop, a portable exe in our github releases and also as a dotnet tool (currently requires net6 rc2)

via scoop:

```
scoop add bucket wimm https://github.com/wareismymind/wareismy-scoop
scoop install peer
```

via dotnet:

```
dotnet tool install --global peer
```

via github releases

```txt
for windows:
https://github.com/wareismymind/peer/releases/latest/download/peer_win-x64.exe

for osx:
https://github.com/wareismymind/peer/releases/latest/download/peer_osx-x64

for linux:
https://github.com/wareismymind/peer/releases/latest/download/peer_linux-x64
```
