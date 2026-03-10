using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Anfx.Pasivos.Application.Features.Contratos.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;


public class Contratos : EndpointGroupBase
{

    public override string? GroupName => "contratos";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Contratos");

        // Endpoint para obtener información general de un contrato pasivo
        group.MapGet("{contratoPasivo}/info-general", GetInfoGeneral)
            .WithName("GetInfoGeneralContratoPasivo")
            .WithSummary("Obtiene información general de un contrato pasivo")
            .WithDescription("Retorna información general, saldo vencido, movimientos capturables y saldo insoluto de un contrato pasivo")
            .Produces<ApiResponseDto<InfoGeneralContratoPasivoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("tabla-amortiza/{idContrato}", GetTablaAmortizacionByTipo)
         .WithName("GetTablaAmortizacionByTipo")
         .WithSummary("Obtiene la tabla de amortización de un contrato por tipo")
         .Produces<ApiResponseDto<List<TablaAmortizaItemDto>>>(StatusCodes.Status200OK)
         .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
         .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }

    public async Task<IResult> GetInfoGeneral(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] string contratoPasivo,
    CancellationToken cancellationToken = default)
    {

        var query = new GetInfoGeneralQuery
        {
            ContratoPasivo = contratoPasivo
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTablaAmortizacionByTipo(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int idContrato,
        [FromQuery] int? idTipoTabla,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetGetTablaAmortizaQuery
        {
            IdContrato = idContrato,
            IdTipoTabla = idTipoTabla ?? 1
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);


        return result.ToCustomMinimalApiResult();
    }
}
