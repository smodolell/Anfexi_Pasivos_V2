using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetTipoTablaAmortizaInfoQuery : IQuery<Result<TipoTablaAmortizaInfoDto>>
{
    public int IdTipoTablaAmortiza { get; set; }
}

internal class GetTipoTablaAmortizaInfoQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetTipoTablaAmortizaInfoQuery, Result<TipoTablaAmortizaInfoDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<TipoTablaAmortizaInfoDto>> HandleAsync(
        GetTipoTablaAmortizaInfoQuery message,
        CancellationToken cancellationToken = default)
    {
        var tipoTabla = await _context.PSV_TipoTablaAmortiza
            .SingleOrDefaultAsync(r => r.IdTipoTablaAmortiza == message.IdTipoTablaAmortiza, cancellationToken);

        if (tipoTabla == null)
        {
            return Result.NotFound("No se ha encontrado el Tipo de Tabla Amortiza");
        }

        var result = new TipoTablaAmortizaInfoDto
        {
            EsCapitalizable = tipoTabla.EsCapitalizable,
            Error = string.Empty,
            TipoCapitalizacion = await GetTipoCapitalizacionList(message.IdTipoTablaAmortiza, cancellationToken),
            TipoPagoCapital = await GetTipoPagoCapitalList(message.IdTipoTablaAmortiza, cancellationToken)
        };

        return Result<TipoTablaAmortizaInfoDto>.Success(result);
    }

    private async Task<List<SelectItemDto>> GetTipoCapitalizacionList(int idTipoTablaAmortiza, CancellationToken cancellationToken)
    {
        var items = new List<SelectItemDto>();

        // Obtener tipos de pago de capital según el filtro
        var tiposPagoCapital = await _context.PSV_TipoTablaAmortizaTipoCapitalizacion
            .Include(i => i.PSV_TipoCapitalizacion)
            .Where(r => r.IdTipoTablaAmortiza == idTipoTablaAmortiza)
            .Select(f => new SelectItemDto
            {
                Value = f.PSV_TipoCapitalizacion.IdTipoCapitalizacion,
                Text = f.PSV_TipoCapitalizacion.TipoCapitalizacion
            })
            .ToListAsync(cancellationToken);

        items.AddRange(tiposPagoCapital);
        return items;
      
    }

    private async Task<List<SelectItemDto>> GetTipoPagoCapitalList(int idTipoTablaAmortiza, CancellationToken cancellationToken)
    {
        var items = new List<SelectItemDto>();

        // Obtener tipos de pago de capital según el filtro
        var tiposPagoCapital = await _context.PSV_TipoTablaAmortizaTipoPagoCapital
            .Include(i => i.PSV_TipoPagoCapital)
            .Where(r => r.IdTipoTablaAmortiza == idTipoTablaAmortiza)
            .Select(f => new SelectItemDto
            {
                Value = f.PSV_TipoPagoCapital.IdTipoPagoCapital,
                Text = f.PSV_TipoPagoCapital.TipoPagoCapital
            })
            .ToListAsync(cancellationToken);

        items.AddRange(tiposPagoCapital);
        return items;
    }
}