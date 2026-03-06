using Anfx.Pasivos.Application.Features.Empresas.DTOs;

namespace Anfx.Pasivos.Application.Features.Empresas.Validators;

public class EmpresaUpdateDtoValidator : AbstractValidator<EmpresaUpdateDto>
{
    public EmpresaUpdateDtoValidator()
    {
        // Validaciones básicas
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID de la empresa debe ser mayor que cero");

        RuleFor(x => x.sEmpresa)
            .NotEmpty().WithMessage("El nombre de la empresa es requerido")
            .MaximumLength(180).WithMessage("El nombre no puede exceder 180 caracteres");

        RuleFor(x => x.RFC)
            .NotEmpty().WithMessage("El RFC es requerido")
            .MaximumLength(13).WithMessage("El RFC no puede exceder 13 caracteres")
            .Must(BeValidRfcFormat).WithMessage("El RFC no tiene un formato válido");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(180).WithMessage("La razón social no puede exceder 180 caracteres");

        // Validaciones opcionales con condiciones
        RuleFor(x => x.Telefono)
            .MaximumLength(12).WithMessage("El teléfono no puede exceder 12 caracteres")
            .Matches(@"^[0-9+\-\s]*$").When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("El teléfono solo puede contener números, +, - y espacios");

        RuleFor(x => x.Representante)
            .MaximumLength(150).WithMessage("El representante no puede exceder 150 caracteres");

        // Validaciones de dirección
        RuleFor(x => x.TipoDireccionId)
            .GreaterThan(0)
            .WithMessage("El tipo de dirección es requerido");

        RuleFor(x => x.ColoniaId)
            .GreaterThan(0)
            .WithMessage("La colonia es requerida");

        // Validaciones condicionales para dirección
        When(x => !string.IsNullOrEmpty(x.Calle) ||
                  !string.IsNullOrEmpty(x.NumExterior) ||
                  !string.IsNullOrEmpty(x.NumInterior), () =>
                  {
                      RuleFor(x => x.Calle)
                          .NotEmpty().WithMessage("La calle es requerida cuando se especifica dirección")
                          .MaximumLength(200).WithMessage("La calle no puede exceder 200 caracteres");

                      RuleFor(x => x.NumExterior)
                          .MaximumLength(20).WithMessage("El número exterior no puede exceder 20 caracteres");

                      RuleFor(x => x.NumInterior)
                          .MaximumLength(20).WithMessage("El número interior no puede exceder 20 caracteres");
                  });

        // Validación de formato RFC (México)
        RuleFor(x => x.RFC)
            .Must(BeValidRfcFormat)
            .When(x => !string.IsNullOrEmpty(x.RFC))
            .WithMessage("El RFC debe tener formato válido (Persona moral: 12 caracteres, Persona física: 13 caracteres)");
    }

    private bool BeValidRfcFormat(string rfc)
    {
        if (string.IsNullOrEmpty(rfc)) return true;

        // RFC Persona Moral: 12 caracteres (XXXX000000XX)
        // RFC Persona Física: 13 caracteres (XXXX000000XXX)
        return rfc.Length == 12 || rfc.Length == 13;
    }
}
