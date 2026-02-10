# Script para iniciar PostgreSQL y MailHog en Docker
# Uso: .\start-postgres-docker.ps1

Write-Host "Iniciando servicios Docker..." -ForegroundColor Green

# Iniciar servicios postgres y mailhog
docker-compose up -d postgres mailhog

Write-Host ""
Write-Host "Servicios iniciados exitosamente:" -ForegroundColor Green
Write-Host "  - PostgreSQL: localhost:5432" -ForegroundColor Cyan
Write-Host "  - MailHog: http://localhost:8025" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ahora ejecuta: .\start.ps1" -ForegroundColor Yellow
Write-Host ""
Write-Host "O ejecuta manualmente:" -ForegroundColor Yellow
Write-Host "dotnet restore Pavis.sln" -ForegroundColor Cyan
Write-Host "dotnet ef migrations add InitialCreate --project src/Pavis.Infrastructure --startup-project src/Pavis.WebApi" -ForegroundColor Cyan
Write-Host "dotnet ef database update --project src/Pavis.Infrastructure --startup-project src/Pavis.WebApi" -ForegroundColor Cyan
Write-Host "dotnet run --project src/Pavis.WebApi" -ForegroundColor Cyan
Write-Host ""
Write-Host "API disponible en: http://localhost:5000" -ForegroundColor Green
