using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetInfoGeneralQuery : IQuery<Result<InfoGeneralDto>>
{
    public string Contrato { get; set; } = "";
}



public class GetInfoGeneralQueryHandler : IQueryHandler<GetInfoGeneralQuery, Result<InfoGeneralDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInfoGeneralQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<InfoGeneralDto>> HandleAsync(GetInfoGeneralQuery request, CancellationToken cancellationToken)
    {
        var contrato = await _context.PSV_Contrato.FirstOrDefaultAsync(r => r.Contrato.Equals(request.Contrato));
        if (contrato is null)
        {
            return Result.NotFound("Contrato no encontrado");
        }
            
        var infoGeneral = new InfoGeneralDto
        {
            Contrato = contrato.Contrato,
        };
        return Result.Success(infoGeneral);
    }
}