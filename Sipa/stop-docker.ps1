# Script para detener Docker
# Uso: .\stop-docker.ps1

Write-Host "Deteniendo contenedores Docker..." -ForegroundColor Yellow

docker-compose down

Write-Host ""
Write-Host "Contenedores detenidos exitosamente" -ForegroundColor Green
Write-Host ""
Write-Host "Para volver a iniciar:"
Write-Host ".\start-postgres-docker.ps1" -ForegroundColor Cyan
Write-Host "o"
Write-Host ".\rebuild-docker.ps1" -ForegroundColor Cyan
