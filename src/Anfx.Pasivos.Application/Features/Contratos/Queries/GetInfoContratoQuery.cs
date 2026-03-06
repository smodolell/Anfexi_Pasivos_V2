using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetInfoContratoQuery : IQuery<Result<InfoContratoDto>>
{
}


internal class GetInfoContratoQueryHandler : IQueryHandler<GetInfoContratoQuery, Result<InfoContratoDto>>
{
    public GetInfoContratoQueryHandler()
    {
        
    }
    public Task<Result<InfoContratoDto>> HandleAsync(GetInfoContratoQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}