using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Commands;

public class CreateCargoAdicionalCommand : ICommand<Result<int>>
{
    public required CargoAdicionalDto Model { get; set; }
}

internal class CreateCargoAdicionalCommandHandler(IApplicationDbContext context, IMapper mapper) : ICommandHandler<CreateCargoAdicionalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> HandleAsync(CreateCargoAdicionalCommand message, CancellationToken cancellationToken = default)
    {
        var model = message.Model;
        try
        {
            var contrato = await _context.PSV_Contrato.SingleOrDefaultAsync(f => f.IdContrato == model.IdContrato, cancellationToken);
            if (contrato == null) return Result.Invalid(new ValidationError("Contrato no existe"));

            var movimiento = new PSV_Movimiento
            {
                IdFondeador = contrato.IdFondeador,
                FecUltimoCambio = DateTime.Now
            };

            _context.PSV_Movimiento.Add(movimiento);

            _mapper.Map(model, movimiento);
            movimiento.SaldoCapital = model.Capital;
            movimiento.SaldoInteres = model.Interes;
            movimiento.SaldoIVA = model.IVA;
            movimiento.SaldoTotal = model.Capital + model.Interes + model.IVA;


            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(movimiento.IdMovimiento);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }
}