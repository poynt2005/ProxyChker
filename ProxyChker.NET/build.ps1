Write-Host "[Builder][Info] ***** GUI framework builder *****" -ForegroundColor DarkGreen

Write-Host "[Builder][Info] Build with dotnet sdk" -ForegroundColor DarkGreen
dotnet publish ./ProxyChker.NET.csproj -c Release -r win-x64 -p:DebugSymbols=false -p:DebugType=None