using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.Enums;

namespace Pavis.Infrastructure.Data.Seeding;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedUsersAsync(context);
        await SeedQuestionsAsync(context);
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var adminUser = new User(
            "Administrador del Sistema",
            "admin@pavis.com",
            BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            UserRole.ADMIN,
            UserStatus.ACTIVE);

        adminUser.UpdateAvatarColor("#EF4444");

        var advisorUser = new User(
            "Asesor Técnico",
            "asesor@pavis.com",
            BCrypt.Net.BCrypt.HashPassword("Asesor123!"),
            UserRole.ASESOR,
            UserStatus.ACTIVE);

        advisorUser.UpdateAvatarColor("#3B82F6");

        var spatUser = new User(
            "Supervisor de Proyectos",
            "spat@pavis.com",
            BCrypt.Net.BCrypt.HashPassword("Spat123!"),
            UserRole.SPAT,
            UserStatus.ACTIVE);

        spatUser.UpdateAvatarColor("#10B981");

        var orgUser = new User(
            "Usuario Organización",
            "org@pavis.com",
            BCrypt.Net.BCrypt.HashPassword("Org123!"),
            UserRole.ORGANIZACION,
            UserStatus.ACTIVE);

        orgUser.UpdateAvatarColor("#F59E0B");

        await context.Users.AddRangeAsync(adminUser, advisorUser, spatUser, orgUser);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Usuarios inicializados correctamente");
    }

    private static async Task SeedQuestionsAsync(ApplicationDbContext context)
    {
        if (await context.Questions.AnyAsync())
        {
            return;
        }

        var q1 = new QuestionDefinition("Q-TECH-001", QuestionAxis.TECNICO, 1, 
            "¿El proyecto cuenta con diseño técnico detallado?", QuestionControlType.SINGLE_SELECT, true);
        q1.AddOption("Sí", "yes");
        q1.AddOption("No", "no");
        q1.AddOption("En proceso", "in_progress");

        var q2 = new QuestionDefinition("Q-FIN-001", QuestionAxis.FINANCIERO, 1,
            "¿Cuál es el presupuesto estimado del proyecto?", QuestionControlType.TEXT_AREA, true);
        q2.SetHelpText("Incluye costos de materiales, mano de obra y equipos");

        var q3 = new QuestionDefinition("Q-LEGAL-001", QuestionAxis.JURIDICO, 1,
            "¿El proyecto tiene la licencia ambiental?", QuestionControlType.SINGLE_SELECT, true);
        q3.AddOption("Sí", "yes");
        q3.AddOption("No", "no");
        q3.AddOption("En trámite", "pending");

        var q4 = new QuestionDefinition("Q-SOC-001", QuestionAxis.SOCIAL, 1,
            "¿Se ha realizado consulta previa con la comunidad?", QuestionControlType.SINGLE_SELECT, true);
        q4.AddOption("Sí", "yes");
        q4.AddOption("No", "no");

        var questions = new List<QuestionDefinition> { q1, q2, q3, q4 };

        await context.Questions.AddRangeAsync(questions);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Preguntas inicializadas correctamente");
    }
}
