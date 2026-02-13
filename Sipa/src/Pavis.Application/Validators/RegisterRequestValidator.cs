using FluentValidation;
using Pavis.Application.DTOs.Auth;

namespace Pavis.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("Formato de email inválido");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es requerido")
            .Must(role => new[] { "ADMIN", "ASESOR", "SPAT", "CONSULTA", "ORGANIZACION" }.Contains(role.ToUpper()))
            .WithMessage("Rol inválido. Roles válidos: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION");
    }
}
