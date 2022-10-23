function Run-Test($Command, $Name) {
    $res = iex "dotnet run -c release --no-build -- $Command";
    
    # Write-Output "COMMAND: $COMMAND"
    # Write-Output "NAME: $NAME"
    # Write-Output "LEC: $LASTEXITCODE"
    $emoji = $null
    if ($LASTEXITCODE -eq 0) {
        $emoji = "✔️"
        $status = "Succeeded"
    }
    else{
        $emoji = "❌"
        $status = "Failed"
    }
    if($env:Verbose -eq $true)
    {
        Write-Output $res
    }
    Write-Output "[ $emoji  ][$Name] Completed - Result : $status"
}

function Run-TestBlock($BlockName, $Tests, [scriptblock]$BeforeAllAction, [scriptblock]$FinallyAction)
{
    try {

    
    Invoke-Command $($BeforeAllAction ?? {})
    Write-Output "[$BlockName]"
    foreach($test in $Tests){
        $res = Run-Test @test
        Write-Output "  $res"
    }
    }
    finally
    {
        Invoke-Command $($FinallyAction ?? {})
    }
}


cd ./Peer
dotnet build -c release

$detailsCommands = @(
    @{ Command = "details 159";                             Name = "Details 1 segment"},
    @{ Command = "details peer/159";                        Name = "Details 2 segment"},
    @{ Command = "details wareismymind/peer/159";           Name = "Details 3 segment"},
    @{ Command = "details github/wareismymind/peer/159";    Name = "Details 4 segment"}
    @{ Command = "details --help";                          Name = "Details help flag"}
    @{ Command = "details --version";                       Name = "Details version flag"}
)

$showCommands = @(
    @{ Command = "show";                Name = "Show no args"},
    @{ Command = "show -f id:159";      Name = "Show with filter"},
    @{ Command = "show -s id:asc";      Name = "Show id asc"},
    @{ Command = "show -s id:desc";     Name = "Show id desc"},
    @{ Command = "show --help";         Name = "Show help flag"},
    @{ Command = "show --version";      Name = "Show version flag"}
)

$configShowCommands = @(
    @{ Command = "config show";             Name = "Config show no args"}
    @{ Command = "config show --help";      Name = "Config show help flag"}
    @{ Command = "config show --version";   Name = "Config show version flag"}

)

$configInitCommands = @(
    @{ Command = "config init";             Name = "Config init no args"}
    @{ Command = "config init -f";          Name = "Config init force"}
    @{ Command = "config init --help";      Name = "Config init help flag"}
    @{ Command = "config init --version";   Name = "Config init version flag"}
)

Run-TestBlock -BlockName "Details" -Tests $detailsCommands
Run-TestBlock -BlockName "Show" -Tests $showCommands
Run-TestBlock -BlockName "Config Show" -Tests $configShowCommands
Run-TestBlock -BlockName "Config init" `
    -Tests $configInitCommands `
    -BeforeAllAction { cp ~/.config/peer.json ~/.config/peer.json.bak }`
    -FinallyAction { mv ~/.config/peer.json.bak ~/.config/peer.json}

cd ..