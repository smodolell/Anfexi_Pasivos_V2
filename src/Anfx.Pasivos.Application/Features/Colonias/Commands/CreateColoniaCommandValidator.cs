namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public class CreateColoniaCommandValidator : AbstractValidator<CreateColoniaCommand>
{
    public CreateColoniaCommandValidator()
    {
        RuleFor(x => x.sColonia)
            .NotEmpty().WithMessage("El nombre de la colonia es requerido")
            .MaximumLength(100).WithMessage("El nombre de la colonia no puede exceder los 100 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es requerido")
            .MaximumLength(50).WithMessage("El estado no puede exceder los 50 caracteres");

        RuleFor(x => x.Municipio)
            .NotEmpty().WithMessage("El municipio es requerido")
            .MaximumLength(50).WithMessage("El municipio no puede exceder los 50 caracteres");

        RuleFor(x => x.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es requerido")
            .Length(5).WithMessage("El código postal debe tener exactamente 5 caracteres")
            .Matches("^[0-9]+$").WithMessage("El código postal solo debe contener números");
    }
}
