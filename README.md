# Peer - A pull request visualizer for github (and others)

Peer helps you track down and visualize your PRs across organizations and accounts (and eventuallyβ’ across git providers).

Peer is meant to be easy to use, cross platform and helpful!

## Statuses

Pull request statuses are all emoji to save space (and because why not?) The default format provider gives the following mapping:

<pre>
    Unknown =>          β
    Conflict =>         &#x2694;&#xFE0F;,
    Draft =>            π,
    ActionsPending =>   β³,
    ActionsQueued =>    π€,
    Stale =>            π,
    FailedChecks =>     π₯,
    AwaitingReview =>   π©,
    FixesRequested =>   β,
    ReadyToMerge =>     &#x2714;&#xFE0F;,
    Merged =>           π (You shouldn't really see this one)
</pre>

## Installing peer

Peer is available as a standalone windows executable on scoop, a portable exe in our github releases and also as a dotnet tool (currently requires net6 rc2)

via scoop:

```ps1
scoop bucket add wimm https://github.com/wareismymind/wareismy-scoop
scoop install peer
```

via dotnet:

```ps1
dotnet tool install --global peer
```

via github releases

```txt
for windows:
- x64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_win-x64.exe
- arm64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_win-arm64

for osx:
- x64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_osx-x64
- arm64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_osx-arm64

for linux:
- x64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_linux-x64
- arm64
  - https://github.com/wareismymind/peer/releases/latest/download/peer_linux-arm64
```

### Dependencies

Peer requires a few things to run properly on different platforms. This list may not be exhaustive but we're working on making sure.

- linux
  - ca-certificates (https communication)
  - openssl (https communication)
  - utf8 supporting terminal
- windows
  - utf8 terminal (Windows Terminal, the pwsh/powershell terminal for ex)
  - Font with emoji support ([Nerdfonts](https://www.nerdfonts.com/) are a good bet and are available on [scoop](https://scoop.sh/))

## Platform/Runtime support

- Tier 1 (We're definitely gonna fix these right away when they're busted because we have hardware/automation to do it)
  - All x64 targets
    - windows
    - linux
    - mac
- Tier 2 (Things we have hardware to test or can test automagically)
  - linux arm64
- Tier 3 (We don't have the hardware to test but we'll do our best!)
  - win-arm64
  - osx-arm64
