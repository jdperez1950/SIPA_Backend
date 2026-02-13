# Configuración SMTP Rápida - PAVIS

## Opción 1: Desarrollo con MailHog (Recomendado)

### Configuración actual
Ya está configurado para usar **MailHog** (localhost:1025)
- Requiere solo ejecutar `docker-compose up`
- Puedes ver los correos en: http://localhost:8025
- **Sin límite** de envío

### Para probar
```bash
curl -X POST http://localhost:5000/api/auth/restore-password \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com"}'
```

---

## Opción 2: Gmail (Producción)

### Paso 1: Generar App Password
1. Ve a: https://myaccount.google.com/
2. Busca: **Seguridad** → **Cómo iniciar sesión en Google** → **Contraseñas de aplicación**
3. Selecciona **Mail** → **Crear**
4. Nombre: `PAVIS Backend`
5. Clic en **Generar** y **COPIA** la contraseña (formato: `xxxx xxxx xxxx xxxx`)

### Paso 2: Configurar
Edita `appsettings.Development.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "TU_CORREO@gmail.com",
    "SmtpPassword": "TU_APP_PASSWORD",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  }
}
```

### Paso 3: Habilitar cuenta (PRIMERA VEZ)
Ve a: https://accounts.google.com/DisplayUnlockCaptcha
Inicia sesión y habilita el acceso para "aplicaciones menos seguras"

### Paso 4: Reiniciar
```bash
docker-compose down
docker-compose up -d
```

---

## Opción 3: Usar variables de entorno (.env)

### Paso 1: Copia el ejemplo
```bash
cp .env.example .env
```

### Paso 2: Edita .env
```bash
nano .env # o tu editor favorito
```

Completa estos valores:
```bash
# Email Settings
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=58
SMTP_USER=pavis@empresa.com
SMTP_PASSWORD=tu_app_password_aqui
SMTP_SSL=true
```

### Paso 3: docker-compose.yml ya está configurado
No necesitas editar docker-compose.yml, ya usa `.env`

### Paso 4: Reiniciar
```bash
docker-compose down
docker-compose up -d
```

---

## Troubleshooting

### Gmail: "Authentication failed"
- Causa: No usaste App Password, usaste la contraseña normal
- Solución: Genera App Password en https://myaccount.google.com/apppasswords

### Gmail: "Too many emails"
- Causa: Límite diario de 500 correos excedido
- Solución: Espera 24 horas o usa Google Workspace

### Outlook: "Mailbox unavailable"
- Causa: Puerto incorrecto o cuenta suspendida
- Solución: Usa puerto 587 y verifica tu cuenta

### Puerto incorrecto
- Gmail: Usa puerto **587**
- Outlook: Usa puerto **587**
- SendGrid: Usa puerto **587**

---

## Recomendaciones

| Uso | Servicio Recomendado |
|-----|-------------------|
| Desarrollo | **MailHog** (ya configurado) |
| Producción personal | **Gmail** (gratis, 500/día) |
| Producción empresarial | **Google Workspace** (sin límite) |
| Producción dedicado | **SendGrid** ($15/mes) |

---

## Testing rápido

### Recuperación de contraseña
```bash
curl -X POST http://localhost:5000/api/auth/restore-password \
  -H "Content-Type: application/json" \
  -d '{"email":"tu-email@prueba.com"}'
```

### Creación de proyecto (con email de organización)
```bash
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN" \
  -d '{
    "name": "Proyecto de Prueba",
    "organization": {
      "name": "Org Test",
      "type": "COMPANY",
      "email": "contacto@test.com",
      "municipality": "Bogotá",
      "region": "Cundinamarca"
    },
    "department": "Cundinamarca",
    "municipality": "Bogotá",
    "dates": {
      "start": "2026-01-01",
      "end": "2026-12-31",
      "submissionDeadline": "2026-02-01"
    }
  }'
```

---

## Documentación completa

Para más detalles, ver:
- `SMTP_CONFIG.md` - Configuración detallada
- `SMTP_PRODUCTION.md` - Configuración para producción
