using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetCajaQuery : IQuery<Result<CajaDto>>
{

    public string? ContratoPasivo { get; set; }


}

internal class GetCajaQueryHandler(IApplicationDbContext context,IMapper mapper) : IQueryHandler<GetCajaQuery, Result<CajaDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<CajaDto>> HandleAsync(GetCajaQuery message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.ContratoPasivo))
            return Result.Invalid(new ValidationError("No se estableció la clave de Contrato."));


        var contrato = await _context.PSV_Contrato
            .Include(i => i.PSV_Fondeador)
            .FirstOrDefaultAsync(f => f.Contrato.Equals(message.ContratoPasivo));

        if (contrato == null) return Result.NotFound("Contrato no encontrado");
        if (contrato.IdEstatusContrato != 2) return Result.Invalid(new ValidationError("El Contrato no se encuentra Activo"));
        var result = new CajaDto
        {
            IdFondeador = contrato.IdFondeador,
            Fondeador = contrato.PSV_Fondeador.Fondeador,
            IdContrato = contrato.IdContrato,
            ContratoPasivo = contrato.Contrato,
        };
        if (result.IdFondeador != null)
        {
            var movimientos = _context.PSV_Movimiento
                .Where(w => w.IdFondeador == result.IdFondeador 
                    && (w.IdContrato == result.IdContrato || result.IdContrato == null) 
                    && w.SaldoTotal > 0)
                .ToList();

            result.Movimientos = _mapper.Map<List<MovimientoPagoItem>>(movimientos);
            result.FechaPago = DateOnly.FromDateTime(DateTime.Now);
            result.IdUsuario = 1;

            return Result.Success(result);
        }

        return Result.Error("No se han encontrado coincidencias");



    }
}

