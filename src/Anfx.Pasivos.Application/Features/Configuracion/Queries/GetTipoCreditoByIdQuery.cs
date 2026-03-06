using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetTipoCreditoByIdQuery : IQuery<Result<TipoCreditoDto>>
{
    public int Id { get; set; }
}
