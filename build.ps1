Write-Host "[Builder][Info] ***** Sample ProxyChker project builder *****" -ForegroundColor DarkGreen
Write-Host "[Builder][Info] Make sure you have dotnet sdk 8.x, MSBuild v143 (VS2022) and nodejs(with npm) 20.x installed on this machine to complete the building"  -ForegroundColor DarkGreen

Write-Host "[Builder][Info] Building libproxychker core lib..."
Push-Location .\\libproxychker
& .\\build.ps1
Pop-Location


Write-Host "[Builder][Info] Building React UI project (including a simple http server)..."  -ForegroundColor DarkGreen
Push-Location .\\ui
& .\\build.ps1
Pop-Location


Write-Host "[Builder][Info] Building Winform gui project..."  -ForegroundColor DarkGreen
Push-Location .\\ProxyChker.NET
& .\\build.ps1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[Builder][Error] Building WinForm gui project failed, aborting..." -ForegroundColor DarkRed
    Exit -1
}
Pop-Location

Write-Host "[Builder][Info] Moving files..."  -ForegroundColor DarkGreen
Move-Item -Force .\\ProxyChker.NET\\bin\\Release\\net8.0-windows\\win-x64\\publish .\\publish
Move-Item -Force .\\ui\\build .\\publish\\Statics

Write-Host "[Builder][Info] Compressing release binaries..."  -ForegroundColor DarkGreen
Compress-Archive -Force -Path publish/* -DestinationPath .\\Release.zip -CompressionLevel "Optimal"

Remove-Item -Force -Recurse .\\publish

Write-Host "[Builder][Info] Building completed"  -ForegroundColor DarkGreen