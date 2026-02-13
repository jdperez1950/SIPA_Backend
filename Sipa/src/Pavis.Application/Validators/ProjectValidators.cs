using FluentValidation;
using Pavis.Application.DTOs.Projects;

namespace Pavis.Application.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Organization)
            .NotNull().WithMessage("La organización es requerida");

        RuleFor(x => x.Organization.Name)
            .NotEmpty().WithMessage("El nombre de la organización es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Organization.Type)
            .NotEmpty().WithMessage("El tipo de organización es requerido")
            .Must(type => new[] { "COMPANY", "PERSON" }.Contains(type.ToUpper()))
            .WithMessage("Tipo inválido. Tipos válidos: COMPANY, PERSON");

        RuleFor(x => x.Organization.Identifier)
            .NotEmpty().WithMessage("El identificador (NIT) es requerido")
            .MaximumLength(50).WithMessage("El identificador no puede exceder 50 caracteres");

        RuleFor(x => x.Organization.Email)
            .NotEmpty().WithMessage("El email de la organización es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Organization.Municipality)
            .NotEmpty().WithMessage("El municipio es requerido");

        RuleFor(x => x.Organization.Region)
            .NotEmpty().WithMessage("El departamento/región es requerido");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("El departamento es requerido")
            .MaximumLength(100).WithMessage("El departamento no puede exceder 100 caracteres");

        RuleFor(x => x.Municipality)
            .NotEmpty().WithMessage("El municipio es requerido")
            .MaximumLength(100).WithMessage("El municipio no puede exceder 100 caracteres");

        RuleFor(x => x.Dates)
            .NotNull().WithMessage("Las fechas son requeridas");

        RuleFor(x => x.Dates.Start)
            .NotEmpty().WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.Dates.End)
            .NotEmpty().WithMessage("La fecha de fin es requerida")
            .GreaterThan(x => x.Dates.Start).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio");

        RuleFor(x => x.Dates.SubmissionDeadline)
            .NotEmpty().WithMessage("La fecha límite de envío es requerida")
            .GreaterThanOrEqualTo(x => x.Dates.Start).WithMessage("La fecha límite de envío debe ser posterior a la fecha de inicio")
            .LessThanOrEqualTo(x => x.Dates.End).WithMessage("La fecha límite de envío debe ser anterior o igual a la fecha de fin");

        When(x => x.ResponseTeam != null, () =>
        {
            RuleForEach(x => x.ResponseTeam).ChildRules(member =>
            {
                member.RuleFor(m => m.Name)
                    .NotEmpty().WithMessage("El nombre del miembro del equipo es requerido");

                member.RuleFor(m => m.Email)
                    .NotEmpty().WithMessage("El email del miembro del equipo es requerido")
                    .EmailAddress().WithMessage("El formato del email no es válido");

                member.RuleFor(m => m.RoleInProject)
                    .NotEmpty().WithMessage("El rol en el proyecto es requerido");

                member.RuleFor(m => m.DocumentNumber)
                    .NotEmpty().WithMessage("El número de documento es requerido");
            });
        });
    }
}

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del proyecto es requerido");

        When(x => !string.IsNullOrWhiteSpace(x.Status), () =>
        {
            RuleFor(x => x.Status)
                .Must(status => new[] { "ACTIVE", "SUSPENDED", "CERTIFIED", "BENEFICIARY" }.Contains(status.ToUpper()))
                .WithMessage("Estado inválido. Estados válidos: ACTIVE, SUSPENDED, CERTIFIED, BENEFICIARY");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ViabilityStatus), () =>
        {
            RuleFor(x => x.ViabilityStatus)
                .Must(status => new[] { "HABILITADO", "PRE_HABILITADO", "ALTA_POSIBILIDAD", "SIN_POSIBILIDAD" }.Contains(status.ToUpper()))
                .WithMessage("Estado de viabilidad inválido. Estados válidos: HABILITADO, PRE_HABILITADO, ALTA_POSIBILIDAD, SIN_POSIBILIDAD");
        });

        When(x => x.Dates != null, () =>
        {
            RuleFor(x => x.Dates!.Start)
                .NotEmpty().WithMessage("La fecha de inicio es requerida");

            RuleFor(x => x.Dates!.End)
                .NotEmpty().WithMessage("La fecha de fin es requerida")
                .GreaterThan(x => x.Dates!.Start).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio");

            RuleFor(x => x.Dates!.SubmissionDeadline)
                .NotEmpty().WithMessage("La fecha límite de envío es requerida")
                .GreaterThanOrEqualTo(x => x.Dates!.Start).WithMessage("La fecha límite de envío debe ser posterior a la fecha de inicio")
                .LessThanOrEqualTo(x => x.Dates!.End).WithMessage("La fecha límite de envío debe ser anterior o igual a la fecha de fin");
        });
    }
}
