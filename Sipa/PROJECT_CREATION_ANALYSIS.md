# Análisis de Cambios para Creación Parcial de Proyectos

## Objetivo
Soportar el flujo de "Creación Parcial de Proyectos (Flujo Consultante)" especificado en BACKEND_SPECS.md

---

## Resumen de Cambios Necesarios

| Componente | Estado Actual | Cambio Requerido |
|------------|---------------|------------------|
| `Organization` Entity | Parcialmente alineado | ✅ Agregar `Address` y `Description` |
| `Project` Entity | Alineado | ✅ Agregar método de generación de código |
| `ProjectStatus` Enum | ✅ Alineado | Ninguno |
| `ViabilityScenario` Enum | ✅ Alineado | Ninguno |
| DTOs de Proyectos | ❌ No existen | ✅ Crear todo el módulo |
| IProjectRepository | Parcialmente alineado | ✅ Agregar método de código único |
| IProjectService | ❌ No existe | ✅ Crear interfaz |
| ProjectService | ❌ No existe | ✅ Crear implementación |
| ProjectsController | ❌ No existe | ✅ Crear controlador |
| Policies de Auth | ❌ Parcial | ✅ Agregar policy "ConsultaOrAdmin" |

---

## 1. Cambios en Entity: Organization.cs

### Estado Actual
```csharp
public class Organization : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public OrganizationType Type { get; private set; }
    public string Identifier { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Municipality { get; private set; } = string.Empty;
    public string Region { get; private set; } = string.Empty;
    public string? ContactName { get; private set; }
    public OrganizationStatus Status { get; private set; }
    public Guid? UserId { get; private set; }
}
```

### Cambios Requeridos
**AGREGAR:**
- `public string? Description { get; private set; }` - Descripción del objeto social
- `public string? Address { get; private set; }` - Dirección física

**Razón:** El frontend envía estos campos en el request de creación de proyecto.

**Nueva firma del constructor:**
```csharp
public Organization(
    string name,
    OrganizationType type,
    string identifier,
    string email,
    string municipality,
    string region,
    string? description = null,
    string? address = null,
    OrganizationStatus status = OrganizationStatus.ACTIVE)
{
    Name = name;
    Type = type;
    Identifier = identifier;
    Email = email;
    Municipality = municipality;
    Region = region;
    Description = description;
    Address = address;
    Status = status;
}
```

---

## 2. Cambios en Entity: Project.cs

### Estado Actual
```csharp
public Project(
    string code,
    string organizationName,
    string municipality,
    string state,
    DateTime startDate,
    DateTime endDate,
    DateTime submissionDeadline,
    Guid organizationId,
    ProjectStatus status = ProjectStatus.ACTIVE,
    ViabilityScenario viabilityStatus = ViabilityScenario.PRE_HABILITADO)
{
    Code = code;
    OrganizationName = organizationName;
    Municipality = municipality;
    State = state;
    StartDate = startDate;
    EndDate = endDate;
    SubmissionDeadline = submissionDeadline;
    OrganizationId = organizationId;
    Status = status;
    ViabilityStatus = viabilityStatus;
    Progress = new ProjectProgress();
}
```

### Cambios Requeridos
**AGREGAR método estático:**
```csharp
public static string GenerateProjectCode()
{
    // Formato: PRJ-YYYY-XXXX
    var year = DateTime.UtcNow.Year;
    var random = new Random();
    var code = random.Next(1, 10000).ToString("D4");
    return $"PRJ-{year}-{code}";
}
```

**Razón:** El backend debe generar un código único automáticamente al crear el proyecto.

---

## 3. DTOs a Crear

### 3.1 Estructura de Carpetas
```
src/Pavis.Application/DTOs/
├── Projects/
│   ├── ProjectDTOs.cs           (DTOs de dominio)
│   ├── CreateProjectRequest.cs  (Request para crear proyecto)
│   └── UpdateProjectRequest.cs  (Request para actualizar proyecto)
└── Organizations/
    └── OrganizationDTO.cs       (DTO de organización)
```

### 3.2 DTOs Detallados

#### OrganizationDTO.cs
```csharp
public class OrganizationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;  // COMPANY/PERSON
    public string Identifier { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? ContactName { get; set; }
    public string Status { get; set; } = string.Empty;
}
```

#### OrganizationRequest (usado en CreateProjectRequest)
```csharp
public class OrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
```

#### CreateProjectRequest.cs
```csharp
public class CreateProjectRequest
{
    // Opcional: nombre del proyecto
    public string? Name { get; set; }

    // Datos de la organización
    public OrganizationRequest Organization { get; set; } = null!;

    // Ubicación
    public string Department { get; set; } = string.Empty;  // "State" en DB
    public string Municipality { get; set; } = string.Empty;

    // Fechas
    public DatesRequest Dates { get; set; } = null!;

    // Opcional: equipo de respuesta inicial
    public List<ResponseTeamMember>? ResponseTeam { get; set; }
}

public class DatesRequest
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime SubmissionDeadline { get; set; }
}

public class ResponseTeamMember
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
}
```

#### ProjectDTOs.cs
```csharp
public class ProjectDTO
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;  // Campo adicional
    public string OrganizationName { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;  // "Department"
    public string Status { get; set; } = string.Empty;
    public string ViabilityStatus { get; set; } = string.Empty;
    public AdvisorDTO? Advisor { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public DateTime? CorrectionDeadline { get; set; }
    public ProjectProgressDTO Progress { get; set; } = null!;
    public OrganizationDTO Organization { get; set; } = null!;
}

public class AdvisorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ProjectProgressDTO
{
    public int Technical { get; set; }
    public int Legal { get; set; }
    public int Financial { get; set; }
    public int Social { get; set; }
}
```

#### UpdateProjectRequest.cs
```csharp
public class UpdateProjectRequest
{
    public Guid Id { get; set; }

    // Campos opcionales para actualización parcial
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? ViabilityStatus { get; set; }
    public Guid? AdvisorId { get; set; }

    // Ejes activos (para Admin)
    public List<string>? ActiveAxes { get; set; }

    // Mesa técnica (para Admin)
    public List<TechnicalTableMember>? TechnicalTable { get; set; }

    // Equipo de respuesta (para Admin)
    public List<ResponseTeamMember>? ResponseTeam { get; set; }

    // Fechas (opcional)
    public DatesRequest? Dates { get; set; }
}

public class TechnicalTableMember
{
    public string AxisId { get; set; } = string.Empty;  // SOCIAL, FINANCIERO, etc.
    public Guid AdvisorId { get; set; }
}
```

---

## 4. Interfaces a Crear/Modificar

### 4.1 Crear IProjectService.cs
```csharp
using Pavis.Application.DTOs.Projects;

public interface IProjectService
{
    Task<ProjectDTO> CreateProjectAsync(CreateProjectRequest request);
    Task<ProjectDTO> UpdateProjectAsync(UpdateProjectRequest request);
    Task<ProjectDTO?> GetProjectByIdAsync(Guid id);
    Task<PagedResponse<ProjectDTO>> GetProjectsAsync(GetProjectsRequest request);
}
```

### 4.2 Modificar IProjectRepository.cs
```csharp
public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByCodeAsync(string code);
    Task<IEnumerable<Project>> GetByAdvisorAsync(Guid advisorId);
    Task<IEnumerable<Project>> GetByOrganizationAsync(Guid organizationId);
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status);
    Task<(IEnumerable<Project> projects, int total)> GetPaginatedAsync(int page, int limit, string? search = null);

    // AGREGAR: Método para verificar si existe código
    Task<bool> CodeExistsAsync(string code);
}
```

### 4.3 Modificar IOrganizationRepository.cs
```csharp
public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdentifierAsync(string identifier);
    Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId);

    // AGREGAR: Método para crear o buscar por identificador
    Task<Organization> GetOrCreateByIdentifierAsync(
        string identifier,
        string name,
        OrganizationType type,
        string email,
        string municipality,
        string region,
        string? description = null,
        string? address = null);
}
```

---

## 5. Servicios a Crear

### 5.1 ProjectService.cs
**Ubicación:** `src/Pavis.Application/Services/ProjectService.cs`

**Responsabilidades:**
1. Validar permisos del usuario (CONSULTA o ADMIN)
2. Buscar o crear organización por NIT/Identifier
3. Generar código único de proyecto
4. Crear proyecto con estado inicial
5. Inicializar progreso en ceros
6. Manejar errores de negocio

**Métodos principales:**
- `CreateProjectAsync(CreateProjectRequest request)`
- `UpdateProjectAsync(UpdateProjectRequest request)`
- `GetProjectByIdAsync(Guid id)`
- `GetProjectsAsync(GetProjectsRequest request)`

---

## 6. Repositorios a Modificar

### 6.1 OrganizationRepository.cs
**Agregar método:**
```csharp
public async Task<Organization> GetOrCreateByIdentifierAsync(
    string identifier,
    string name,
    OrganizationType type,
    string email,
    string municipality,
    string region,
    string? description = null,
    string? address = null)
{
    var existingOrg = await GetByIdentifierAsync(identifier);
    if (existingOrg != null)
    {
        return existingOrg;
    }

    var newOrg = new Organization(
        name,
        type,
        identifier,
        email,
        municipality,
        region,
        description,
        address);

    await _dbSet.AddAsync(newOrg);
    await _context.SaveChangesAsync();
    return newOrg;
}
```

### 6.2 ProjectRepository.cs
**Agregar método:**
```csharp
public async Task<bool> CodeExistsAsync(string code)
{
    return await _dbSet
        .AnyAsync(p => p.Code == code && p.DeletedAt == null);
}
```

---

## 7. Controladores a Crear

### 7.1 ProjectsController.cs
**Ubicación:** `src/Pavis.WebApi/Controllers/ProjectsController.cs`

**Endpoints:**

#### POST /api/projects (Creación parcial - Flujo Consultante)
- **Auth:** `[Authorize(Policy = "ConsultaOrAdmin")]`
- **Request:** `CreateProjectRequest`
- **Response:** `ProjectDTO`
- **Validación:**
  - Campos obligatorios: organization, department, municipality, dates
  - Campos opcionales: name, responseTeam

#### PATCH /api/projects/{id} (Completitud - Flujo Admin)
- **Auth:** `[Authorize(Policy = "AdminOnly")]`
- **Request:** `UpdateProjectRequest`
- **Response:** `ProjectDTO`

#### GET /api/projects (Listado)
- **Auth:** `[Authorize]`
- **Query params:** page, limit, search, status, viabilityStatus
- **Response:** `PagedResponse<ProjectDTO>`

#### GET /api/projects/{id} (Detalle)
- **Auth:** `[Authorize]`
- **Response:** `ProjectDTO`

---

## 8. AutoMapper Configuración

**Crear/Modificar:** `src/Pavis.Application/Mappings/ProjectMappingProfile.cs`

```csharp
public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        // Organization
        CreateMap<Organization, OrganizationDTO>();
        CreateMap<OrganizationRequest, Organization>();

        // Project
        CreateMap<Project, ProjectDTO>()
            .ForMember(dest => dest.Advisor,
                       opt => opt.MapFrom<AdvisorResolver>())
            .ForMember(dest => dest.Organization,
                       opt => opt.MapFrom<OrganizationResolver>());

        // Progress
        CreateMap<ProjectProgress, ProjectProgressDTO>();
    }
}
```

---

## 9. Policies de Autorización

**Modificar:** `Program.cs`

**Agregar policy:**
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("ADMIN"));

    options.AddPolicy("ConsultaOrAdmin", policy =>
        policy.RequireRole("CONSULTA", "ADMIN"));
});
```

---

## 10. Validadores a Crear

### 10.1 CreateProjectRequestValidator.cs
**Ubicación:** `src/Pavis.Application/Validators/CreateProjectRequestValidator.cs`

**Reglas de validación:**
- Organization: requerida, con identifier, name, type, email, municipality, region
- Department: requerido
- Municipality: requerido
- Dates: requerido con fechas válidas (start < end < submissionDeadline)

### 10.2 UpdateProjectRequestValidator.cs
**Reglas de validación:**
- Id: requerido
- Campos opcionales validados individualmente si se proporcionan

---

## 11. Migrations de Base de Datos

**Cambios requeridos:**

### Tabla `organizations`
```sql
ALTER TABLE organizations
ADD COLUMN description TEXT,
ADD COLUMN address VARCHAR(255);
```

### Tabla `projects`
**Ningún cambio estructural requerido** - ya tiene todos los campos necesarios

---

## 12. Flujo de Creación Parcial (Lógica Detallada)

### Paso 1: Validar Request
- Verificar que el usuario tenga rol CONSULTA o ADMIN
- Validar campos obligatorios usando FluentValidation

### Paso 2: Manejar Organización
- Buscar organización por `identifier` (NIT)
- Si existe → vincularla al proyecto
- Si no existe → crear nueva organización con:
  - Type: COMPANY (default) o PERSON
  - Status: ACTIVE (default)
  - Description y Address (si se proporcionan)

### Paso 3: Generar Código de Proyecto
- Formato: `PRJ-YYYY-XXXX`
- Verificar que el código no exista en BD
- Si existe, regenerar

### Paso 4: Crear Proyecto
- Code: código único generado
- OrganizationId: ID de la organización (existente o nueva)
- OrganizationName: desnormalizado para búsquedas
- Municipality/State: del request
- StartDate/EndDate/SubmissionDeadline: del request
- Status: ACTIVE (o DRAFT si se implementa)
- ViabilityStatus: PRE_HABILITADO (default)
- AdvisorId: NULL (sin asignar)
- Progress: todos los ejes en 0

### Paso 5: Manejar Response Team (Opcional)
- Si se proporciona, crear usuarios con rol ORGANIZACION
- Vincular usuarios a la organización

### Paso 6: Persistir
- Guardar organización (si es nueva)
- Guardar proyecto
- Commit transaction

### Paso 7: Retornar Respuesta
- ProjectDTO con toda la información creada
- Incluir objeto Organization completo
- Advisor: NULL

---

## 13. Ejemplo de Request/Response

### Request
```json
POST /api/projects
Authorization: Bearer {token}

{
  "name": "Proyecto Piloto Educación",
  "organization": {
    "name": "Fundación Educativa ABC",
    "type": "COMPANY",
    "identifier": "900123456-1",
    "description": "Fundación dedicada a la educación rural",
    "address": "Calle 100 #15-20",
    "email": "contacto@fundacion.edu.co",
    "municipality": "Soacha",
    "region": "Cundinamarca"
  },
  "department": "Cundinamarca",
  "municipality": "Soacha",
  "dates": {
    "start": "2024-03-01",
    "end": "2024-12-31",
    "submissionDeadline": "2024-04-15"
  }
}
```

### Response (201 Created)
```json
{
  "success": true,
  "message": "Proyecto creado exitosamente",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "code": "PRJ-2024-1234",
    "name": "Proyecto Piloto Educación",
    "organizationName": "Fundación Educativa ABC",
    "municipality": "Soacha",
    "state": "Cundinamarca",
    "status": "ACTIVE",
    "viabilityStatus": "PRE_HABILITADO",
    "advisor": null,
    "startDate": "2024-03-01T00:00:00Z",
    "endDate": "2024-12-31T00:00:00Z",
    "submissionDeadline": "2024-04-15T00:00:00Z",
    "correctionDeadline": null,
    "progress": {
      "technical": 0,
      "legal": 0,
      "financial": 0,
      "social": 0
    },
    "organization": {
      "id": "b2c3d4e5-f6g7-8901-bcde-f23456789012",
      "name": "Fundación Educativa ABC",
      "type": "COMPANY",
      "identifier": "900123456-1",
      "email": "contacto@fundacion.edu.co",
      "municipality": "Soacha",
      "region": "Cundinamarca",
      "description": "Fundación dedicada a la educación rural",
      "address": "Calle 100 #15-20",
      "contactName": null,
      "status": "ACTIVE"
    }
  }
}
```

---

## 14. Lista Tarea de Implementación

### Orden Recomendado:

1. ✅ Modificar Entity `Organization.cs`
2. ✅ Modificar Entity `Project.cs` (agregar GenerateProjectCode)
3. ✅ Crear DTOs en folder `Projects/`
4. ✅ Crear DTOs en folder `Organizations/`
5. ✅ Modificar `IProjectRepository.cs`
6. ✅ Modificar `IOrganizationRepository.cs`
7. ✅ Implementar en `OrganizationRepository.cs`
8. ✅ Implementar en `ProjectRepository.cs`
9. ✅ Crear `IProjectService.cs`
10. ✅ Crear `ProjectService.cs`
11. ✅ Crear validadores
12. ✅ Configurar AutoMapper profiles
13. ✅ Agregar policy en `Program.cs`
14. ✅ Crear `ProjectsController.cs`
15. ✅ Generar migration
16. ✅ Ejecutar migration
17. ✅ Tests de integración

---

## 15. Consideraciones Importantes

### Seguridad
- Validar que el usuario tenga el rol correcto
- Prevenir inyección SQL (usar parámetros)
- Sanitizar inputs de organización

### Manejo de Errores
- Organización duplicada → vincular, no crear
- Código duplicado → regenerar
- Fechas inválidas → validar lógica de negocio
- Usuario no autorizado → 403 Forbidden

### Transacciones
- Usar `_unitOfWork` para manejar transacciones
- Hacer rollback si falla alguna parte del proceso

### Logging
- Loggear cada paso del proceso de creación
- Registrar errores con contexto suficiente

---

## 16. Próximos Pasos

Una vez aprobado este análisis, proceder con:

1. **Fase 1:** Modificaciones en Entities y Enums
2. **Fase 2:** Creación de DTOs y Mappings
3. **Fase 3:** Repositorios y Servicios
4. **Fase 4:** Controladores y Policies
5. **Fase 5:** Migrations y Tests

¿Procedemos con la implementación?
