# Pruebas de API - Gestión de Usuarios

## Test Results

### 1. Login (Obtener Token)
✅ **EXITOSO**

**Request:**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@pavis.com",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "f44ba49d-3357-43ea-b3e8-fad58a5cfcf1",
      "name": "Administrador del Sistema",
      "email": "admin@pavis.com",
      "role": "ADMIN",
      "status": "ACTIVE",
      "avatarColor": "#EF4444",
      "projectsAssigned": 0
    }
  }
}
```

---

### 2. Obtener Lista de Usuarios
✅ **EXITOSO**

**Request:**
```bash
GET http://localhost:5000/api/auth/users?page=1&limit=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```json
{
  "success": true,
  "message": "Usuarios obtenidos exitosamente",
  "data": {
    "data": [
      {
        "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
        "name": "Asesor Técnico",
        "email": "asesor@pavis.com",
        "role": "ASESOR",
        "status": "ACTIVE",
        "avatarColor": "#3B82F6",
        "projectsAssigned": 0
      },
      {
        "id": "3553a8cc-af66-4ac8-83cf-783d3b3eefe3",
        "name": "Supervisor de Proyectos",
        "email": "spat@pavis.com",
        "role": "SPAT",
        "status": "ACTIVE",
        "avatarColor": "#10B981",
        "projectsAssigned": 0
      },
      {
        "id": "53983305-9878-4c81-b66e-558f1fc4e876",
        "name": "Usuario Organización",
        "email": "org@pavis.com",
        "role": "ORGANIZACION",
        "status": "ACTIVE",
        "avatarColor": "#F59E0B",
        "projectsAssigned": 0
      },
      {
        "id": "f44ba49d-3357-43ea-b3e8-fad58a5cfcf1",
        "name": "Administrador del Sistema",
        "email": "admin@pavis.com",
        "role": "ADMIN",
        "status": "ACTIVE",
        "avatarColor": "#EF4444",
        "projectsAssigned": 0
      }
    ],
    "total": 4,
    "page": 1,
    "limit": 10,
    "totalPages": 1
  }
}
```

---

### 3. Actualizar Usuario
✅ **EXITOSO**

**Request:**
```bash
PATCH http://localhost:5000/api/auth/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
  "name": "Asesor Actualizado"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Usuario actualizado exitosamente",
  "data": {
    "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
    "message": "Usuario actualizado exitosamente",
    "user": {
      "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
      "name": "Asesor Actualizado",
      "email": "asesor@pavis.com",
      "role": "ASESOR",
      "status": "ACTIVE",
      "avatarColor": "#3B82F6",
      "projectsAssigned": 0
    }
  }
}
```

---

### 4. Desactivar Usuario
✅ **EXITOSO**

**Request:**
```bash
PATCH http://localhost:5000/api/auth/users/status
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
  "status": "INACTIVE"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Usuario inactive exitosamente",
  "data": {
    "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
    "message": "Usuario inactive exitosamente",
    "user": {
      "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
      "name": "Asesor Actualizado",
      "email": "asesor@pavis.com",
      "role": "ASESOR",
      "status": "INACTIVE",
      "avatarColor": "#3B82F6",
      "projectsAssigned": 0
    }
  }
}
```

---

### 5. Intentar Login con Usuario INACTIVE
✅ **EXITOSO** (Login rechazado como esperado)

**Request:**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "asesor@pavis.com",
  "password": "Asesor123!"
}
```

**Response:**
```json
{
  "success": false,
  "message": "Usuario no encontrado o inactivo",
  "data": null,
  "errors": []
}
```

**Status Code:** 401 Unauthorized

---

### 6. Reactivar Usuario
✅ **EXITOSO**

**Request:**
```bash
PATCH http://localhost:5000/api/auth/users/status
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
  "status": "ACTIVE"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Usuario active exitosamente",
  "data": {
    "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
    "message": "Usuario active exitosamente",
    "user": {
      "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
      "name": "Asesor Actualizado",
      "email": "asesor@pavis.com",
      "role": "ASESOR",
      "status": "ACTIVE",
      "avatarColor": "#3B82F6",
      "projectsAssigned": 0
    }
  }
}
```

---

### 7. Login con Usuario Reactivado
✅ **EXITOSO**

**Request:**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "asesor@pavis.com",
  "password": "Asesor123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "18b23f42-3e10-4c88-ab1e-621af8c32f84",
      "name": "Asesor Actualizado",
      "email": "asesor@pavis.com",
      "role": "ASESOR",
      "status": "ACTIVE",
      "avatarColor": "#3B82F6",
      "projectsAssigned": 0
    }
  }
}
```

---

## Resumen de Tests

| Test | Estado | Descripción |
|------|--------|-------------|
| 1. Login | ✅ PASS | Login exitoso con admin |
| 2. Obtener Usuarios | ✅ PASS | Lista de usuarios obtenida correctamente |
| 3. Actualizar Usuario | ✅ PASS | Usuario actualizado correctamente |
| 4. Desactivar Usuario | ✅ PASS | Usuario desactivado correctamente |
| 5. Login INACTIVE | ✅ PASS | Login rechazado para usuario inactivo |
| 6. Reactivar Usuario | ✅ PASS | Usuario reactivado correctamente |
| 7. Login Reactivado | ✅ PASS | Login exitoso con usuario reactivado |

**Total:** 7/7 Tests Pasados ✅

---

## Comandos de Test (Para replicar)

### 1. Login
```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@pavis.com","password":"Admin123!"}'
```

### 2. Obtener Usuarios
```bash
curl -X GET "http://localhost:5000/api/auth/users?page=1&limit=10" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 3. Actualizar Usuario
```bash
curl -X PATCH "http://localhost:5000/api/auth/users" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  --data-raw '{"id":"USER_ID_HERE","name":"Nuevo Nombre"}'
```

### 4. Desactivar Usuario
```bash
curl -X PATCH "http://localhost:5000/api/auth/users/status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  --data-raw '{"id":"USER_ID_HERE","status":"INACTIVE"}'
```

### 5. Activar Usuario
```bash
curl -X PATCH "http://localhost:5000/api/auth/users/status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  --data-raw '{"id":"USER_ID_HERE","status":"ACTIVE"}'
```

---

## Observaciones Importantes

1. **Seguridad:** Los endpoints están correctamente protegidos con `[Authorize(Policy = "AdminOnly")]`
2. **Validación:** Los requests son validados correctamente con FluentValidation
3. **Bloqueo de Usuarios:** Los usuarios INACTIVE no pueden iniciar sesión
4. **Actualización Parcial:** Es posible actualizar solo campos específicos del usuario
5. **Respuestas Consistentes:** Todos los endpoints siguen el formato de respuesta estándar `ApiResponse<T>`

---

## Swagger UI

Para ver la documentación interactiva de la API:
- URL: http://localhost:5000/swagger/index.html
- Permite probar todos los endpoints directamente desde el navegador
