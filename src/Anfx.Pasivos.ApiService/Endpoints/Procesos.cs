using Anfx.Pasivos.ApiService.Requests.Procesos;
using Anfx.Pasivos.Application.Features.Procesos.Commands;
using Anfx.Pasivos.Application.Features.Procesos.DTOs;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Procesos : EndpointGroupBase
{
    public override string? GroupName => "procesos";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Procesos");

        group.MapPost("moratorios", ProcesarMoratorios)
            .WithName("ProcesarMoratorios")
            .WithSummary("Procesa moratorios para contratos")
            .WithDescription("Ejecuta para calcular y aplicar moratorios a contratos, de forma global o para un contrato específico.")
            .Accepts<ProcesaMoratorioRequest>("application/json")
            .Produces<ApiResponseDto<ProcesaMoratorioResultDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("vencimientos", ProcesarVencimientos)
            .WithName("ProcesarVencimientos")
            .WithSummary("Procesa vencimientos de contratos en un rango de fechas")
            .WithDescription("Ejecuta el procedimiento almacenado usp_PSV_ProcesaVencimientosAsync para generar vencimientos de contratos en el rango de fechas especificado, con filtros opcionales por fondeador y contrato.")
            .Accepts<ProcesaVencimientoRequest>("application/json")
            .Produces<ApiResponseDto<ProcesaVencimientoResultDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }


    public async Task<IResult> ProcesarMoratorios(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] ProcesaMoratorioRequest request)
    {
        var command = new ProcesaMoratorioCommand
        {
            FechaProcesamiento = request.FechaProcesamiento,
            IdContrato = request.IdContrato,
            ContratoPasivo = request.ContratoPasivo ?? ""
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    public async Task<IResult> ProcesarVencimientos(
       [FromServices] ICommandMediator commandMediator,
       [FromBody] ProcesaVencimientoRequest request)
    {
        var command = new ProcesaVencimientoCommand
        {
            FechaInicial = request.FechaInicial,
            FechaFinal = request.FechaFinal,
            IdFondeador = request.IdFondeador,
            IdContrato = request.IdContrato
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
}
