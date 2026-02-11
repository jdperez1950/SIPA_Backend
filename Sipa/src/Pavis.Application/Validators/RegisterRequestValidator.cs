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

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("El rol es requerido")
            .Must(role => new[] { "ADMIN", "ASESOR", "SPAT", "CONSULTA", "ORGANIZACION" }.Contains(role.ToUpper()))
            .WithMessage("Rol inválido. Roles válidos: ADMIN, ASESOR, SPAT, CONSULTA, ORGANIZACION");
    }
}
