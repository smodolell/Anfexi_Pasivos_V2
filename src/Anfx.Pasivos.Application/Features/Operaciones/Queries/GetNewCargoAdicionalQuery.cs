using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetNewCargoAdicionalQuery : IQuery<Result<CargoAdicionalDto>>
{
    public int IdContrato { get; set; }
}


internal class GetNewCargoAdicionalQueryHandler(IApplicationDbContext context) : IQueryHandler<GetNewCargoAdicionalQuery, Result<CargoAdicionalDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CargoAdicionalDto>> HandleAsync(GetNewCargoAdicionalQuery message, CancellationToken cancellationToken = default)
    {
        var contrato = await _context.PSV_Contrato.SingleOrDefaultAsync(r => r.IdContrato == message.IdContrato);
        if (contrato == null) return Result.NotFound("Contrato no encontrado");

        if (contrato.IdEstatusContrato != 2)
        {
            return Result.Invalid(new ValidationError($"El contrato Clave:[{contrato.Contrato}] no esta activo"));
        }

        var model = new CargoAdicionalDto
        {
            IdContrato = contrato.IdContrato,
            FecMovimiento = DateTime.Now,
            PorcIVA = 0.16m,
            Descripcion = ""
        };

        return Result.Success(model);
    }
}
