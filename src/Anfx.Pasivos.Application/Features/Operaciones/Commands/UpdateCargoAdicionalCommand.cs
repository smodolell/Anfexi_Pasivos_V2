using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Commands;

public class UpdateCargoAdicionalCommand : ICommand<Result>
{
    public int IdMovimiento { get; set; }
    public required CargoAdicionalDto Model { get; set; }
}


internal class UpdateCargoAdicionalCommandHandler(IApplicationDbContext context, IMapper mapper) : ICommandHandler<UpdateCargoAdicionalCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;


    public async Task<Result> HandleAsync(UpdateCargoAdicionalCommand message, CancellationToken cancellationToken = default)
    {
        var model = message.Model;

        try
        {
            var movimiento = await _context.PSV_Movimiento.SingleOrDefaultAsync(f => f.IdMovimiento == message.IdMovimiento, cancellationToken);

            if (movimiento == null) return Result.NotFound("Movimiento no encontrado");

            _mapper.Map(model, movimiento);

            _context.PSV_Movimiento.Update(movimiento);


            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}