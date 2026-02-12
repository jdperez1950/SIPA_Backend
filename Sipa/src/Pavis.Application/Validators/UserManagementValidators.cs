using FluentValidation;
using Pavis.Application.DTOs.Auth;

namespace Pavis.Application.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido");

        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre no puede estar vacío")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email no puede estar vacío")
                .EmailAddress().WithMessage("El formato del email no es válido")
                .MaximumLength(255).WithMessage("El email no puede exceder 255 caracteres");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Role), () =>
        {
            RuleFor(x => x.Role)
                .Must(role => new[] { "ADMIN", "ASESOR", "SPAT", "CONSULTA", "ORGANIZACION" }.Contains(role.ToUpper()))
                .WithMessage("Rol inválido. Roles válidos: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION");
        });

        When(x => !string.IsNullOrWhiteSpace(x.AvatarColor), () =>
        {
            RuleFor(x => x.AvatarColor)
                .MaximumLength(50).WithMessage("El color del avatar no puede exceder 50 caracteres");
        });
    }
}

public class ToggleUserStatusRequestValidator : AbstractValidator<ToggleUserStatusRequest>
{
    public ToggleUserStatusRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("El estado es requerido")
            .Must(status => new[] { "ACTIVE", "INACTIVE" }.Contains(status.ToUpper()))
            .WithMessage("Estado inválido. Estados válidos: ACTIVE, INACTIVE");
    }
}
