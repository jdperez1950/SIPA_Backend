# Configuración SMTP para Producción - PAVIS

## Configuración Completa (Gmail)

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-db-host;Database=PavisDb;Username=postgres;Password=your-password;Port=5432"
  },
  "JwtSettings": {
    "SecretKey": "TU_SECRET_KEY_MUY_SEGURA_MINIMO_32_CARACTERES",
    "Issuer": "Pavis.Api",
    "Audience": "Pavis.Client",
    "ExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "pavis@tu-empresa.com",
    "SmtpPassword": "xxxx xxxx xxxx xxxx",
    "EnableSsl": true,
    "FromEmail": "noreply@pavis.com",
    "FromName": "PAVIS System",
    "DevMode": false
  },
  "AllowedHosts": "https://tu-dominio.com"
}
```

---

## Docker Compose con Variables de Entorno

### docker-compose.prod.yml

```yaml
version: '3.8'

services:
  pavis-api:
    image: sipa-api:latest
    container_name: pavis-api
    ports:
      - "5000:8080"
    environment:
      # Database
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=PavisDb;Username=postgres;Password=${POSTGRES_PASSWORD};Port=5432

      # JWT
      - JwtSettings__SecretKey=${JWT_SECRET}
      - JwtSettings__Issuer=Pavis.Api
      - JwtSettings__Audience=Pavis.Client
      - JwtSettings__ExpirationMinutes=15
      - JwtSettings__RefreshTokenExpirationDays=7

      # Email (Gmail)
      - EmailSettings__SmtpServer=smtp.gmail.com
      - EmailSettings__SmtpPort=587
      - EmailSettings__SmtpUser=${SMTP_USER}
      - EmailSettings__SmtpPassword=${SMTP_PASSWORD}
      - EmailSettings__EnableSsl=true
      - EmailSettings__FromEmail=noreply@pavis.com
      - EmailSettings__FromName=PAVIS System
      - EmailSettings__DevMode=false

      # Hosting
      - AllowedHosts=${ALLOWED_HOSTS}

    depends_on:
      - pavis-postgres
    restart: unless-stopped

  pavis-postgres:
    image: postgres:16-alpine
    container_name: pavis-postgres
    environment:
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
      - POSTGRES_DB=PavisDb
    volumes:
      - pavis_postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  pavis-mailhog:
    image: mailhog/mailhog
    container_name: pavis-mailhog
    ports:
      - "8025:8025"
      - "1025:1025"
    restart: unless-stopped
```

---

## Archivo .env

```bash
# .env para Producción

# Database
POSTGRES_PASSWORD=tu_contraseña_segura_bd

# JWT
JWT_SECRET=tu_super_secret_key_muy_segura_minimo_32_caracteres_1234567890123456789012345

# Email (Gmail)
SMTP_USER=pavis@tu-empresa.com
SMTP_PASSWORD=tu_app_password_generada_en_google_accounts

# Hosting
ALLOWED_HOSTS=https://pavis.tu-empresa.com
```

---

## Pasos para Configurar

### 1. Generar App Password en Google

1. Entra a: https://myaccount.google.com/
2. Ve a "Seguridad"
3. Busca "2-Step Verification" → "Contraseñas de aplicación"
4. Selecciona "Mail" → "Crear nueva contraseña"
5. Nombre: `PAVIS Backend Producción`
6. Copia la contraseña: `xxxx xxxx xxxx xxxx` (solo se muestra una vez)

### 2. Configurar Google Account (Opcional pero recomendado)

Si es la primera vez que usas Gmail SMTP:
1. Entra a: https://accounts.google.com/DisplayUnlockCaptcha
2. Inicia sesión con tu cuenta de Google
3. Habilita el acceso para "aplicaciones menos seguras"

### 3. Reemplazar variables en `.env`

```bash
cp .env.example .env
nano .env  # o usa tu editor favorito
```

### 4. Ejecutar en Producción

```bash
# Detener contenedores actuales
docker-compose down

# Construir imagen
docker-compose -f docker-compose.prod.yml build api

# Iniciar con configuración de producción
docker-compose -f docker-compose.prod.yml up -d

# Ver logs
docker logs -f pavis-api
```

---

## Opciones de Servicios de Correo

### Gmail (Recomendado para inicio)
- **Gratis** para cuentas Gmail personales
- **500 emails/día** límite
- SSL/TLS incluido
- Puerto: 587 (TLS)

### Google Workspace (Recomendado para producción)
- **Sin límite** diario (o muy alto)
- **Reputación de IP** dedicada
- **Costo**: ~$6-12 USD/mes por usuario
- **Soporte técnico 24/7**
- SSL/TLS incluido
- Puerto: 587 (TLS)

### Outlook/Office 365
- **Integración** con Microsoft 365
- **Sin límite** diario
- **Costo**: ~$6-12 USD/mes por usuario
- SSL/TLS incluido
- Puerto: 587 (TLS)

### SendGrid (Servicio dedicado de email)
- **API RESTful**
- **Webhooks** para eventos
- **30,000 emails/mes** gratis
- **100 emails/día** límite gratuito
- **Costo**: ~$15/mes para 100,000 emails/mes
- **SDK disponible** para .NET

### Mailgun
- **Servicio dedicado** de email
- **API RESTful**
- **10,000 emails/mes** gratis
- **Webhooks** para eventos
- **Costo**: ~$35/mes para 50,000 emails/mes
- **SDK disponible** para .NET

---

## Testing del SMTP

### Test desde Postman

```bash
# 1. Login para obtener token
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@pavis.com",
  "password": "Admin123!"
}

# 2. Usar token para probar recuperación de contraseña
POST http://localhost:5000/api/auth/restore-password
Content-Type: application/json
Authorization: Bearer TU_TOKEN

{
  "email": "tu-email-de-prueba@gmail.com"
}
```

### Test con cURL

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@pavis.com","password":"Admin123!"}'

# Extraer token del response y usarlo para probar envío de email
curl -X POST http://localhost:5000/api/auth/restore-password \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_TOKEN" \
  -d '{"email":"test@example.com"}'
```

---

## Troubleshooting

### Error: "AuthenticationFailedException"

**Causa**: Credenciales incorrectas

**Solución**:
1. Verifica que usaste la App Password, NO la contraseña de la cuenta
2. Genera una nueva App Password si necesitas

### Error: "SmtpException: Authentication unsuccessful"

**Causa**: 2FA habilitada o App Password incorrecta

**Solución**:
1. Deshabilita temporalmente 2FA para probar
2. Regenera la App Password

### Error: "The operation has timed out"

**Causa**: Puerto incorrecto

**Solución**: Asegúrate de usar puerto **587** para Gmail

### Error: "Mailbox unavailable"

**Causa**: Cuenta suspendida por Google

**Solución**:
1. Verifica tu cuenta de Google
2. Verifica que no hay actividad sospechosa

### Error: "Too many emails"

**Causa**: Límite diario excedido (500 correos para Gmail gratuito)

**Solución**:
1. Espera 24 horas
2. Usa Google Workspace para mayor límite

---

## Recomendaciones de Seguridad

1. **Nunca commits credenciales en git**
   - Usa `.env` y añade a `.gitignore`
   - O usa Docker secrets/secrets manager

2. **Usa credenciales separadas para dev/prod**
   - `appsettings.Development.json` → MailHog
   - `appsettings.Production.json` → Gmail real

3. **Usa cuentas dedicadas para producción**
   - Crea `pavis-noreply@tudominio.com`
   - No uses tu email personal

4. **Habilita SPF, DKIM, DMARC**
   - Mejora entregabilidad
   - Reduce spam

5. **Monitorea la reputación de IP**
   - Usa IPs dedicadas
   - Evita usar servicios de email compartidos
