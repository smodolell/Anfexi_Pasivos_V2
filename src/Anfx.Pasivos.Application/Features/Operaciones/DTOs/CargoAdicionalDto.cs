using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Operaciones.DTOs;

public class CargoAdicionalDto
{
    public int IdContrato { get; set; }
 
    public int NoPago { get; set; }
    public string Descripcion { get; set; }=string.Empty;   
    public System.DateTime FecMovimiento { get; set; }
    public int IdTipoMovimiento { get; set; }

    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal PorcIVA { get; set; }
    public int GeneraIVA_Capital { get; set; }
    public int GeneraIVA_Interes { get; set; }
}

public class CargoAdicionalDtoValidator : AbstractValidator<CargoAdicionalDto>
{
    public CargoAdicionalDtoValidator()
    {
        // Reglas de validación con mensajes personalizados
        RuleFor(x => x.IdContrato)
            .NotEmpty().WithMessage("El ID del contrato es requerido")
            .GreaterThan(0).WithMessage("El ID del contrato debe ser válido");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción del cargo es requerida")
            .Length(3, 150).WithMessage("La descripción debe tener entre 3 y 150 caracteres");

        RuleFor(x => x.FecMovimiento)
            .NotEmpty().WithMessage("La fecha de movimiento es requerida")
            .Must(fecha => fecha <= DateTime.Now).WithMessage("La fecha no puede ser futura");

        RuleFor(x => x.IdTipoMovimiento)
            .NotEmpty().WithMessage("El tipo de movimiento es requerido")
            .GreaterThan(0).WithMessage("Seleccione un tipo de movimiento válido");

        RuleFor(x => x.Total)
            .NotEmpty().WithMessage("El monto total es requerido")
            .GreaterThan(0).WithMessage("El monto total debe ser mayor a cero")
            .PrecisionScale(18, 2, false).WithMessage("El monto total debe tener máximo 2 decimales");

        RuleFor(x => x.Capital)
            .GreaterThanOrEqualTo(0).WithMessage("El capital no puede ser negativo");

        RuleFor(x => x.Interes)
            .GreaterThanOrEqualTo(0).WithMessage("El interés no puede ser negativo");

        RuleFor(x => x.IVA)
            .GreaterThanOrEqualTo(0).WithMessage("El IVA no puede ser negativo");

        RuleFor(x => x.PorcIVA)
            .InclusiveBetween(0, 1).WithMessage("El porcentaje de IVA debe estar entre 0% y 100%");

        RuleFor(x => x.GeneraIVA_Capital)
            .Must(valor => valor == 0 || valor == 1)
            .WithMessage("El indicador de IVA en capital debe ser 0 o 1");

        RuleFor(x => x.GeneraIVA_Interes)
            .Must(valor => valor == 0 || valor == 1)
            .WithMessage("El indicador de IVA en intereses debe ser 0 o 1");
    }
}
