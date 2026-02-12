using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Pavis.Application.Interfaces;
using Pavis.Application.Mappings;
using Pavis.Application.Services;
using Pavis.Application.Validators;
using Pavis.Domain.Interfaces;
using Pavis.Infrastructure.Configurations;
using Pavis.Infrastructure.Services;
using Pavis.Infrastructure.Data;
using Pavis.Infrastructure.Repositories;
using Pavis.Infrastructure.Services;
using Pavis.Infrastructure.Data.Seeding;
using FluentValidation;
using Pavis.WebApi.Middleware;
using Pavis.WebApi.Swagger;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/pavis-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PAVIS V2 API",
        Version = "v1",
        Description = "API para PAVIS V2 - Sistema de gestión de proyectos",
        Contact = new OpenApiContact
        {
            Name = "PAVIS Team",
            Email = "admin@pavis.com"
        }
    });

    // Deshabilitar referencias de esquemas para enums (mostrar como string)
    c.UseInlineDefinitionsForEnums();

    // Incluir documentación XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Agregar filter para corregir el tipo de Role (string en lugar de enum)
    c.SchemaFilter<RolePropertySchemaFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtConfig = builder.Configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>();
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig!.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfig.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "ADMIN"));
    options.AddPolicy("ConsultaOrAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "CONSULTA", "ADMIN"));
    options.AddPolicy("AdvisorOnly", policy => policy.RequireClaim(ClaimTypes.Role, "ASESOR", "ADMIN"));
    options.AddPolicy("SpatOnly", policy => policy.RequireClaim(ClaimTypes.Role, "SPAT", "ADMIN"));
    options.AddPolicy("OrganizationOnly", policy => policy.RequireClaim(ClaimTypes.Role, "ORGANIZACION", "ADMIN"));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IQuestionDefinitionRepository, QuestionDefinitionRepository>();
builder.Services.AddScoped<IProjectResponseRepository, ProjectResponseRepository>();
builder.Services.AddScoped<IAxisRepository, AxisRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApplicationAuthService, ApplicationAuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Servicio de correo - Fácil de reemplazar:
// Para usar servicio real (SendGrid, AWS SES, etc.), crea una clase que implemente IEmailService
// y cambia la línea de abajo por: builder.Services.AddScoped<IEmailService, TuServicioEmail>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://colombiansofture.com",
                "http://colombiansofture.com:30199",
                "https://colombiansofture.com:30199"
            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Para desarrollo, permitir cualquier origen
    options.AddPolicy("AllowDevelopment", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger habilitado en todos los entornos para desarrollo
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAVIS V2 API v1");
    c.RoutePrefix = "swagger";
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(app.Environment.IsDevelopment() ? "AllowDevelopment" : "AllowSpecificOrigins");

app.UseAuthentication();
 app.UseAuthorization();

 app.MapControllers();

 using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
    await ApplicationDbContextSeeder.SeedAsync(context);
}

 try
 {
    Log.Information("Starting PAVIS V2 API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}

