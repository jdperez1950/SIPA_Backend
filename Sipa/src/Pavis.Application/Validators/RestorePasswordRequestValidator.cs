using FluentValidation;
using Pavis.Application.DTOs.Auth;

namespace Pavis.Application.Validators;

public class RestorePasswordRequestValidator : AbstractValidator<RestorePasswordRequest>
{
    public RestorePasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email del usuario es requerido")
            .EmailAddress().WithMessage("Formato de email inválido");
    }
}
