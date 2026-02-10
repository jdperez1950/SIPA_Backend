# Script para limpiar PAVIS V2

Write-Host "Limpiando PAVIS V2..." -ForegroundColor Green
Write-Host ""

# Eliminar migraciones
Write-Host "Eliminando migraciones..." -ForegroundColor Yellow
Remove-Item -Path "src\Pavis.Infrastructure\Migrations" -Recurse -Force -ErrorAction SilentlyContinue

# Eliminar carpetas bin
Write-Host ""
Write-Host "Eliminando carpetas bin/..." -ForegroundColor Yellow
Get-ChildItem -Path "." -Include "bin" -Recurse -Directory -Force -ErrorAction SilentlyContinue | ForEach-Object {
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

# Eliminar carpetas obj
Write-Host ""
Write-Host "Eliminando carpetas obj/..." -ForegroundColor Yellow
Get-ChildItem -Path "." -Include "obj" -Recurse -Directory -Force -ErrorAction SilentlyContinue | ForEach-Object {
    Remove-Item -Path $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "Limpieza completada!" -ForegroundColor Green
Write-Host ""
Write-Host "Siguiente paso: .\start.ps1"
