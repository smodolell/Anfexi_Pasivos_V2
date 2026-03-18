using Anfx.Pasivos.Application.Common.Models.StoredProcedures;
using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Commands;

public class CajaConfirmCommand :ICommand<Result>
{

    public required CajaDto Model { get; set; }
}


internal class CajaConfirmCommandHandler(IApplicationDbContext context,IValidator<CajaDto> validator) : ICommandHandler<CajaConfirmCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IValidator<CajaDto> _validator = validator;

    public async Task<Result> HandleAsync(CajaConfirmCommand message, CancellationToken cancellationToken = default)
    {
        var model = message.Model;


        var validationResult = await _validator.ValidateAsync(model, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        try
        {
            var movimientos = model.Movimientos.Where(r => r.Seleccionado).Select(s => new KeyItem
            {
                ID = s.ID
            });
            var resultSp = await _context.Procedures.usp_PSV_PagarCargosAsync(
                model.IdFondeador,
                model.IdContrato,
                model.IdUsuario,
                model.IdTipoPago,
                model.IdCuentaBancaria,
                model.FechaPago,
                model.Referencia??"",
                model.MontoPago,
                movimientos
            );
            var result = resultSp.FirstOrDefault();
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Error))
                {
                    return Result.SuccessWithMessage("Se ha realizado el pago correctamente.");
                }
                else
                {
                    return Result.Error(result.Error);
                }
            }
            return Result.Error("El procedimiento no produjo resultados");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}


