# Script para reconstruir Docker

Write-Host "Deteniendo y eliminando contenedores..." -ForegroundColor Yellow
docker-compose down -v

Write-Host "Eliminando imagen Docker anterior..." -ForegroundColor Yellow
docker rmi pavis-api 2>$null

Write-Host "Reconstruyendo imagen Docker..." -ForegroundColor Yellow
docker-compose build --no-cache

Write-Host "Iniciando contenedores..." -ForegroundColor Green
docker-compose up -d

Write-Host ""
Write-Host "Servicios disponibles:" -ForegroundColor Cyan
Write-Host "  API:        http://localhost:5000" -ForegroundColor Cyan
Write-Host "  Swagger:    http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "  MailHog:    http://localhost:8025 (correos)" -ForegroundColor Cyan
