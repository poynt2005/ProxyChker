Write-Host "[Builder][Info] ***** WebUI builder *****" -ForegroundColor DarkGreen

Write-Host "[Builder][Info] Build React" -ForegroundColor DarkGreen
Write-Host "[Builder][Info] Install React dependencies" -ForegroundColor DarkGreen

npm i
npm run build