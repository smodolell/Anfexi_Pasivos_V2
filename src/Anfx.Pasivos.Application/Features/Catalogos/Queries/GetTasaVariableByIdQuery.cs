using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class TasaVariableDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<TasaValorListItemDto> Valores { get; set; } = new();
}

public class GetTasaVariableByIdQuery : IQuery<Result<TasaVariableDetalleDto>>
{
    public int Id { get; set; }
}

internal class GetTasaVariableByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTasaVariableByIdQuery, Result<TasaVariableDetalleDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<TasaVariableDetalleDto>> HandleAsync(GetTasaVariableByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.Tasa
                .Include(t => t.TasaValors)
                .SingleOrDefaultAsync(t => t.IdTasa == request.Id && t.EsVariable == true, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Tasa variable no encontrada");
            }

            var dto = new TasaVariableDetalleDto
            {
                Id = entity.IdTasa,
                Nombre = entity.Tasa1,
                Valores = entity.TasaValors
                    .OrderByDescending(v => v.FecValorTasa)
                    .Select(v => new TasaValorListItemDto
                    {
                        Id = v.IdTasaValor,
                        ValorTasa = v.ValorTasa,
                        FecValorTasa = v.FecValorTasa,
                        FecRegistroTasa = v.FecRegistroTasa
                    })
                    .ToList()
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
