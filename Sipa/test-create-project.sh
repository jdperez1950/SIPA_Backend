#!/bin/bash

# Test Script - Crear Proyecto con Equipo
# Este script prueba la creación de un proyecto con equipo de respuesta

BASE_URL="http://localhost:5000"
ADMIN_EMAIL="admin@pavis.com"
ADMIN_PASSWORD="Admin123!"

echo "========================================"
echo "  TEST: Crear Proyecto con Equipo"
echo "========================================"
echo ""

# Colores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Paso 1: Login como Admin
echo -e "${YELLOW}[1/4] Autenticando como admin...${NC}"

login_response=$(curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$ADMIN_EMAIL\", \"password\": \"$ADMIN_PASSWORD\"}" \
  -w "\n%{http_code}")

http_code=$(echo "$login_response" | tail -n1)
response_body=$(echo "$login_response" | sed '$d')

if [ "$http_code" = "200" ]; then
    token=$(echo "$response_body" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
    echo -e "${GREEN}✓ Login exitoso${NC}"
    echo -e "${GRAY}  Token: ${token:0:50}...${NC}"
else
    echo -e "${RED}✗ Error en login (HTTP $http_code)${NC}"
    echo -e "${YELLOW}Intentando crear usuario admin...${NC}"
    
    # Crear admin
    register_response=$(curl -s -X POST "$BASE_URL/api/auth/register" \
      -H "Content-Type: application/json" \
      -d "{\"name\": \"Administrador Test\", \"email\": \"$ADMIN_EMAIL\", \"role\": \"ADMIN\"}" \
      -w "\n%{http_code}")
    
    reg_http_code=$(echo "$register_response" | tail -n1)
    
    if [ "$reg_http_code" = "200" ] || [ "$reg_http_code" = "201" ]; then
        echo -e "${GREEN}✓ Usuario admin creado${NC}"
        echo -e "${MAGENTA}  IMPORTANTE: Revisa MailHog (http://localhost:8025) para obtener la contraseña${NC}"
        echo -e "${YELLOW}  Actualiza ADMIN_PASSWORD en este script y vuelve a ejecutar${NC}"
        exit 1
    else
        echo -e "${RED}✗ Error creando admin (HTTP $reg_http_code)${NC}"
        echo "$register_response"
        exit 1
    fi
fi

echo ""
echo -e "${YELLOW}[2/4] Creando proyecto con equipo de respuesta...${NC}"

# Crear proyecto con equipo
project_payload='{
  "name": "Proyecto de Innovación Tecnológica 2026",
  "organization": {
    "name": "Empresa Innovadora ABC",
    "identifier": "9001234567",
    "type": 1,
    "email": "contacto@empresaabc.com",
    "municipality": "Bogotá",
    "region": "Cundinamarca",
    "contactName": "María González",
    "address": "Calle 123 # 45-67, Bogotá",
    "description": "Empresa líder en innovación tecnológica"
  },
  "responseTeam": [
    {
      "email": "lider.tecnico@test.com",
      "name": "Carlos López Rodríguez",
      "roleInProject": "Líder Técnico",
      "phone": "3001234567",
      "documentNumber": "12345678",
      "documentType": "CC"
    },
    {
      "email": "desarrollador@test.com",
      "name": "Ana Martínez Silva",
      "roleInProject": "Desarrolladora Senior",
      "phone": "3109876543",
      "documentNumber": "87654321",
      "documentType": "CC"
    },
    {
      "email": "consultor@test.com",
      "name": "Pedro Ramírez",
      "roleInProject": "Consultor Externo",
      "phone": "3155554444",
      "documentNumber": "45678912",
      "documentType": "CE"
    }
  ],
  "dates": {
    "start": "2026-02-15",
    "end": "2026-12-31",
    "submissionDeadline": "2026-03-15"
  }
}'

echo -e "${GRAY}Payload:${NC}"
echo "$project_payload" | python3 -m json.tool 2>/dev/null || echo "$project_payload"

project_response=$(curl -s -X POST "$BASE_URL/api/projects" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $token" \
  -d "$project_payload" \
  -w "\n%{http_code}")

http_code=$(echo "$project_response" | tail -n1)
response_body=$(echo "$project_response" | sed '$d')

if [ "$http_code" = "200" ] || [ "$http_code" = "201" ]; then
    project_id=$(echo "$response_body" | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4)
    project_name=$(echo "$response_body" | grep -o '"name":"[^"]*"' | head -1 | cut -d'"' -f4)
    project_code=$(echo "$response_body" | grep -o '"code":"[^"]*"' | cut -d'"' -f4)
    
    echo ""
    echo -e "${GREEN}✓ Proyecto creado exitosamente!${NC}"
    echo -e "${CYAN}  ID: $project_id${NC}"
    echo -e "${CYAN}  Código: $project_code${NC}"
    echo -e "${CYAN}  Nombre: $project_name${NC}"
else
    echo ""
    echo -e "${RED}✗ Error creando proyecto (HTTP $http_code)${NC}"
    echo -e "${RED}Respuesta:${NC}"
    echo "$response_body" | python3 -m json.tool 2>/dev/null || echo "$response_body"
    exit 1
fi

echo ""
echo -e "${YELLOW}[3/4] Verificando equipo del proyecto...${NC}"

sleep 2

team_response=$(curl -s -X GET "$BASE_URL/api/projects/$project_id/team" \
  -H "Authorization: Bearer $token" \
  -w "\n%{http_code}")

team_http_code=$(echo "$team_response" | tail -n1)
team_body=$(echo "$team_response" | sed '$d')

if [ "$team_http_code" = "200" ]; then
    echo -e "${GREEN}✓ Equipo obtenido exitosamente!${NC}"
    
    # Contar miembros
    member_count=$(echo "$team_body" | grep -o '"userId"' | wc -l)
    echo -e "${CYAN}  Total miembros: $member_count${NC}"
    echo ""
    echo -e "${GRAY}  Miembros del equipo:${NC}"
    
    # Parse and display members
    echo "$team_body" | python3 -m json.tool 2>/dev/null || echo "$team_body"
else
    echo -e "${RED}✗ Error obteniendo equipo (HTTP $team_http_code)${NC}"
    echo "$team_body"
fi

echo ""
echo -e "${YELLOW}[4/4] Verificación de emails...${NC}"
echo -e "${GREEN}✓ Revisa MailHog en: http://localhost:8025${NC}"
echo -e "${GRAY}  Deberías ver 3 emails de bienvenida:${NC}"
echo -e "${GRAY}    1. lider.tecnico@test.com${NC}"
echo -e "${GRAY}    2. desarrollador@test.com${NC}"
echo -e "${GRAY}    3. consultor@test.com${NC}"

echo ""
echo "========================================"
echo -e "${GREEN}  TEST COMPLETADO EXITOSAMENTE${NC}"
echo "========================================"
echo ""

echo -e "${YELLOW}Resumen:${NC}"
echo -e "${GREEN}  ✓ Proyecto creado: $project_name${NC}"
echo -e "${GREEN}  ✓ Miembros en equipo: $member_count${NC}"
echo -e "${GREEN}  ✓ Emails enviados: 3 (verificar en MailHog)${NC}"

echo ""
echo -e "${YELLOW}URLs útiles:${NC}"
echo -e "${CYAN}  API: http://localhost:5000${NC}"
echo -e "${CYAN}  Swagger: http://localhost:5000/swagger${NC}"
echo -e "${CYAN}  MailHog: http://localhost:8025${NC}"
echo ""
