using Anfx.Pasivos.Application.Features.Empresas.DTOs;

namespace Anfx.Pasivos.Application.Features.Empresas.Validators;

public class EmpresaCreateDtoValidator : AbstractValidator<EmpresaCreateDto>
{
    public EmpresaCreateDtoValidator()
    {

        RuleFor(x => x.sEmpresa)
            .NotEmpty().WithMessage("El nombre de la empresa no puede estar vacío")
            .MaximumLength(180).WithMessage("El nombre no puede exceder 180 caracteres");

        RuleFor(x => x.RFC)
            .NotEmpty().WithMessage("El RFC no puede estar vacío")
            .MaximumLength(13).WithMessage("El RFC no puede exceder 13 caracteres")
            .Must(BeValidRfcFormat).When(x => !string.IsNullOrEmpty(x.RFC))
            .WithMessage("El RFC debe tener 12 o 13 caracteres");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social no puede estar vacía")
            .MaximumLength(180).WithMessage("La razón social no puede exceder 180 caracteres");

        // Validaciones de campos opcionales
        RuleFor(x => x.Telefono)
            .MaximumLength(12).WithMessage("El teléfono no puede exceder 12 caracteres")
            .Matches(@"^[0-9+\-\s]*$").When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("El teléfono solo puede contener números, +, - y espacios");

        RuleFor(x => x.Representante)
            .MaximumLength(150).WithMessage("El representante no puede exceder 150 caracteres");

        // Validaciones de campos de texto con longitud máxima
        RuleFor(x => x.AvisosEstadodeCuenta)
            .MaximumLength(500).WithMessage("Los avisos no pueden exceder 500 caracteres");

        RuleFor(x => x.AdvertenciasEstadodeCuenta)
            .MaximumLength(500).WithMessage("Las advertencias no pueden exceder 500 caracteres");

        RuleFor(x => x.AclaracionesEstadodeCuenta)
            .MaximumLength(500).WithMessage("Las aclaraciones no pueden exceder 500 caracteres");

        // Validaciones de dirección
        RuleFor(x => x.TipoDireccionId)
            .GreaterThan(0)
            .WithMessage("El tipo de dirección debe ser válido");

        RuleFor(x => x.ColoniaId)
            .GreaterThan(0)
            .WithMessage("La colonia debe ser válida");

        // Validaciones condicionales de dirección
        When(x => !string.IsNullOrEmpty(x.Calle) ||
                  !string.IsNullOrEmpty(x.NumExterior) ||
                  !string.IsNullOrEmpty(x.NumInterior), () =>
                  {
                      RuleFor(x => x.Calle)
                          .NotEmpty().WithMessage("La calle es requerida")
                          .MaximumLength(200).WithMessage("La calle no puede exceder 200 caracteres");

                      RuleFor(x => x.NumExterior)
                          .MaximumLength(20).WithMessage("El número exterior no puede exceder 20 caracteres");

                      RuleFor(x => x.NumInterior)
                          .MaximumLength(20).WithMessage("El número interior no puede exceder 20 caracteres");
                  });

        // Validación de coherencia
        RuleFor(x => x)
            .Must(HaveConsistentAddress)
            .WithMessage("Los datos de dirección son inconsistentes")
            .WithName("address_consistency");
    }

    private bool BeValidRfcFormat(string rfc)
    {
        if (string.IsNullOrEmpty(rfc)) return true;
        return rfc.Length == 12 || rfc.Length == 13;
    }

    private bool HaveConsistentAddress(EmpresaCreateDto dto)
    {
        // Si hay datos de dirección, debe haber al menos calle o número exterior
        if (!string.IsNullOrEmpty(dto.Calle) ||
            !string.IsNullOrEmpty(dto.NumExterior) ||
            !string.IsNullOrEmpty(dto.NumInterior))
        {
            return dto.TipoDireccionId > 0 && dto.ColoniaId > 0;
        }
        return true;
    }
}