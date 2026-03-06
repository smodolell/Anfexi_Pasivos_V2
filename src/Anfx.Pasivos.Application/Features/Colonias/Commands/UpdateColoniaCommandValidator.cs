namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public class UpdateColoniaCommandValidator : AbstractValidator<UpdateColoniaCommand>
{
    public UpdateColoniaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido")
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

        RuleFor(x => x.sColonia)
            .NotEmpty().WithMessage("El nombre de la colonia es requerido")
            .MaximumLength(100).WithMessage("El nombre de la colonia no puede exceder los 100 caracteres")
            .MinimumLength(3).WithMessage("El nombre de la colonia debe tener al menos 3 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es requerido")
            .MaximumLength(50).WithMessage("El estado no puede exceder los 50 caracteres")
            .MinimumLength(2).WithMessage("El estado debe tener al menos 2 caracteres");

        RuleFor(x => x.Municipio)
            .NotEmpty().WithMessage("El municipio es requerido")
            .MaximumLength(50).WithMessage("El municipio no puede exceder los 50 caracteres")
            .MinimumLength(2).WithMessage("El municipio debe tener al menos 2 caracteres");

        RuleFor(x => x.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es requerido")
            .Length(5).WithMessage("El código postal debe tener exactamente 5 caracteres")
            .Matches("^[0-9]+$").WithMessage("El código postal solo debe contener números");
    }
}