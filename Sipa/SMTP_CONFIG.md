# Configuración SMTP - PAVIS

## Configuración Actual (MailHog - Desarrollo)
```json
{
  "EmailSettings": {
    "SmtpServer": "localhost",
    "SmtpPort": 1025,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": true
  }
}
```

---

## Configuración para Gmail/Google Workspace

### Archivo: `appsettings.Development.json`

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "tu-correo@gmail.com",
    "SmtpPassword": "tu-app-password",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  }
}
```

### Archivo: `appsettings.Production.json` (para producción)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-db-host;Database=PavisDb;Username=postgres;Password=your-password;Port=5432"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "pavis@tudominio.com",
    "SmtpPassword": "tu-app-password",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  },
  "JwtSettings": {
    "SecretKey": "TU_SECRET_KEY_PRODUCCION_MINIMO_32_CARACTERES",
    "Issuer": "Pavis.Api",
    "Audience": "Pavis.Client",
    "ExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "AllowedHosts": "https://tu-dominio.com"
}
```

---

## Pasos para configurar Gmail SMTP

### 1. Generar Contraseña de Aplicación (App Password)

Gmail ya NO permite usar la contraseña normal de la cuenta. Debes generar una "App Password":

1. Ve a: https://myaccount.google.com/
2. Inicia sesión con tu cuenta de Google
3. Busca "Seguridad" > "Cómo iniciar sesión en Google" > "Contraseñas de aplicación"
4. Haz clic en "Selecionar aplicación" > "Mail"
5. Pon un nombre: `PAVIS Backend`
6. Haz clic en "Generar"
7. **COPIA** la contraseña generada (solo aparecerá una vez) en formato: `xxxx xxxx xxxx xxxx`

### 2. Actualizar archivo de configuración

Edita `appsettings.Development.json` con los datos:
- **SmtpUser**: tu correo de Gmail completo (ej: `pavis.tuempresa@gmail.com`)
- **SmtpPassword**: la contraseña de aplicación que acabas de generar
- **SmtpServer**: `smtp.gmail.com`
- **SmtpPort**: `587`
- **EnableSsl**: `true`
- **DevMode**: `false`

### 3. Configurar Google Account (Importante)

Si es la PRIMERA vez que usas Gmail SMTP:
1. Ve a: https://accounts.google.com/DisplayUnlockCaptcha
2. Inicia sesión con tu cuenta de Google
3. Habilita el acceso para "aplicaciones menos seguras"

### 4. Reiniciar la aplicación

```bash
docker-compose down
docker-compose up -d
```

---

## Configuración para Outlook/Office 365

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.office365.com",
    "SmtpPort": 587,
    "SmtpUser": "pavis@tudominio.onmicrosoft.com",
    "SmtpPassword": "tu-contraseña",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  }
}
```

---

## Configuración para SendGrid (Servicio dedicado)

### 1. Crear cuenta en SendGrid
1. Ve a: https://sendgrid.com/
2. Regístrate y verifica tu email
3. Ve a "Settings" > "API Keys"
4. Crea una API Key y cópiala

### 2. Actualizar configuración

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SmtpUser": "apikey",
    "SmtpPassword": "SENDGRID_API_KEY",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  }
}
```

---

## Notas Importantes

### Gmail Limitaciones
- **500 correos por día** para cuentas gratuitas
- **100 correos por día** si envías a más de 500 destinatarios
- Los correos pueden tardar unos minutos en llegar
- Puede ir a Spam la primera vez

### Google Workspace
- **No tiene límite** de envío diario (o muy alto)
- **Más confiable** para producción
- Costo: aprox $6-12 USD/mes por cuenta

### MailHog (Actual - Desarrollo)
- Solo guarda correos, no los envía realmente
- Se puede ver los correos en: http://localhost:8025
- No requiere configuración

### Testing
Para probar el envío, usa el endpoint de recuperación de contraseña con un email de prueba:

```bash
curl -X POST http://localhost:5000/api/auth/restore-password \
  -H "Content-Type: application/json" \
  -d '{
    "email": "tu-email@gmail.com"
  }'
```

---

## Errores Comunes y Soluciones

| Error | Causa | Solución |
|-------|--------|-----------|
| `AuthenticationFailedException` | Credenciales incorrectas | Verifica usuario y contraseña de aplicación |
| `SmtpException: Authentication unsuccessfull` | 2FA habilitada | Usa contraseña de aplicación |
| `SmtpException: The operation has timed out` | Puerto incorrecto | Usa puerto 587 para Gmail |
| `Mailbox unavailable` | Cuenta bloqueada | Verifica que la cuenta no esté suspendida |
| `Too many emails` | Límite diario excedido | Espera 24 horas o usa Google Workspace |

---

## Variables de Entorno (Opcional)

También puedes usar variables de entorno para no exponer credenciales:

### Docker Compose
```yaml
version: '3.8'

services:
  pavis-api:
    image: sipa-api
    environment:
      - EmailSettings__SmtpServer=smtp.gmail.com
      - EmailSettings__SmtpPort=587
      - EmailSettings__SmtpUser=pavis@empresa.com
      - EmailSettings__SmtpPassword=tu-app-password
      - EmailSettings__EnableSsl=true
      - EmailSettings__DevMode=false
      - EmailSettings__FromEmail=noreply@pavis.com
      - EmailSettings__FromName=PAVIS System
```

### Archivo `.env`
```bash
# .env
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=pavis@empresa.com
SMTP_PASSWORD=tu-app-password
SMTP_SSL=true
```

---

## Recomendación

Para **producción**, usa **Google Workspace** o **SendGrid** por:
- Mayor confiabilidad
- No hay límites diarios
- Mejor reputación de IP
- Soporte técnico

Para **desarrollo**, puedes mantener **MailHog** (DevMode: true) para no enviar correos reales mientras desarrollas.
