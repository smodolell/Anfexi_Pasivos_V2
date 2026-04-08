using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetContratosAsignadosQuery:IQuery<Result<List<ContratosAsignadosDto>>>
{
    public int IdContratoPasivo { get; set; }
}
internal class GetContratosAsignadosQueryHandler : IQueryHandler<GetContratosAsignadosQuery, Result<List<ContratosAsignadosDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetContratosAsignadosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<ContratosAsignadosDto>>> HandleAsync(GetContratosAsignadosQuery request, CancellationToken cancellationToken)
    {
        var contratos = await _context.View_ContratosAsignados
            .Where(c => c.IdContratoPasivo == request.IdContratoPasivo)
            .Select(c => new ContratosAsignadosDto
            {
                IdContrato = c.IdContrato,
                Contrato = c.Contrato,
                Capital = c.Capital,
                FecActivacion = c.FecActivacion,
                TipoCredito = c.TipoCredito,
                FechaAsignacion = c.FechaAsignacion,
                IdContratoPasivo = c.IdContratoPasivo
            })
            .ToListAsync(cancellationToken);
        return Result.Success(contratos);
    }
}

