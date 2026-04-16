using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetTasaFijaByIdQuery : IQuery<Result<TasaFijaDto>>
{
    public int Id { get; set; }
}

internal class GetTasaFijaByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTasaFijaByIdQuery, Result<TasaFijaDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<TasaFijaDto>> HandleAsync(GetTasaFijaByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.Tasa
                .SingleOrDefaultAsync(t => t.IdTasa == request.Id && t.EsVariable == false, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Tasa fija no encontrada");
            }

            var dto = new TasaFijaDto
            {
                Nombre = entity.Tasa1,
                ValorTasa = entity.ValorTasa ?? 0,
                FecTasa = entity.FecTasa
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
