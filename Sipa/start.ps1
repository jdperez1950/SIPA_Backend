# Script simple para ejecutar PAVIS V2

Write-Host "Iniciando PAVIS V2 Backend..." -ForegroundColor Green
Write-Host ""

# Verificar .NET
Write-Host "Verificando .NET..." -ForegroundColor Yellow
$netVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET no esta instalado" -ForegroundColor Red
    Write-Host "Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}
Write-Host ".NET version: $netVersion" -ForegroundColor Green

# Verificar Docker
Write-Host ""
Write-Host "Verificando Docker..." -ForegroundColor Yellow
$dockerVersion = docker --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ADVERTENCIA: Docker no esta disponible" -ForegroundColor Yellow
    Write-Host "Asegurate que PostgreSQL este corriendo manualmente" -ForegroundColor Yellow
} else {
    Write-Host "Docker version: $dockerVersion" -ForegroundColor Green
    Write-Host "Iniciando PostgreSQL..." -ForegroundColor Cyan
    docker-compose up -d
    Start-Sleep -Seconds 3
}

# Restaurar dependencias
Write-Host ""
Write-Host "Restaurando dependencias..." -ForegroundColor Yellow
dotnet restore Pavis.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Error restaurando dependencias" -ForegroundColor Red
    exit 1
}

# Crear migraciones
Write-Host ""
Write-Host "Creando migraciones..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate --project src/Pavis.Infrastructure --startup-project src/Pavis.WebApi
if ($LASTEXITCODE -ne 0) {
    Write-Host "ADVERTENCIA: Error creando migraciones (puede que ya existan)" -ForegroundColor Yellow
}

# Actualizar base de datos
Write-Host ""
Write-Host "Actualizando base de datos..." -ForegroundColor Yellow
dotnet ef database update --project src/Pavis.Infrastructure --startup-project src/Pavis.WebApi
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Error actualizando base de datos" -ForegroundColor Red
    exit 1
}

# Compilar proyecto
Write-Host ""
Write-Host "Compilando proyecto..." -ForegroundColor Yellow
dotnet build Pavis.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Error compilando proyecto" -ForegroundColor Red
    exit 1
}

# Ejecutar aplicacion
Write-Host ""
Write-Host "Iniciando API..." -ForegroundColor Green
Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  PAVIS V2 API" -ForegroundColor White
Write-Host "  Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "  API Base: http://localhost:5000/api" -ForegroundColor Cyan
Write-Host "  MailHog: http://localhost:8025" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

dotnet run --project src/Pavis.WebApi
