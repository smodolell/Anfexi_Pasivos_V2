namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;


public class UpdateTipoDireccionCommandValidator : AbstractValidator<UpdateTipoDireccionCommand>
{
    public UpdateTipoDireccionCommandValidator()
    {
        RuleFor(x => x.sTipoDireccion)
            .NotEmpty()
                .WithMessage("El tipo de dirección es requerido")
            .NotNull()
                .WithMessage("El tipo de dirección no puede ser nulo")
            .MaximumLength(200)
                .WithMessage("El tipo de dirección no puede exceder los 50 caracteres")
            .MinimumLength(3)
                .WithMessage("El tipo de dirección debe tener al menos 3 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El tipo de dirección solo puede contener letras y espacios");
    }
}
