namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del usuario es requerido")
            .GreaterThan(0).WithMessage("El ID del usuario debe ser válido");

        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MaximumLength(100).WithMessage("El nombre completo no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.UsuarioNombre)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder 50 caracteres");

        // La contraseña es opcional, pero si se proporciona debe cumplir las reglas
        RuleFor(x => x.Contrasena)
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Contrasena));

        RuleFor(x => x.RolId)
            .NotEmpty().WithMessage("El rol es requerido")
            .GreaterThan(0).WithMessage("El rol debe ser válido");
    }
}