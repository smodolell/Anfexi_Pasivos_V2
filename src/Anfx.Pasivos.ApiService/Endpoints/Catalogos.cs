using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Features.Catalogos.Commands;
using Anfx.Pasivos.Application.Features.Catalogos.DTOs;
using Anfx.Pasivos.Application.Features.Catalogos.Queries;
using Anfx.Pasivos.Application.Features.Colonias.Commands;
using Anfx.Pasivos.Application.Features.Colonias.DTOs;
using Anfx.Pasivos.Application.Features.Colonias.Queries;
using Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;
using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;
using Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Catalogos : EndpointGroupBase
{
    public override string? GroupName => "catalogos";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            //.RequireAuthorization()
        .WithTags("Catalogos");

        #region Banco
        // GET by id
        group.MapGet("banco/{id}", GetBancoById)
            .WithName("GetBancoById")
            .WithSummary("Obtiene un banco por ID")
            .Produces<ApiResponseDto<BancoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("banco/", GetPaginatedBanco)
            .WithSummary("Obtiene bancos paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<BancoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("banco/", CreateBanco)
            .WithName("CreateBanco")
            .WithSummary("Crea un nuevo banco")
            .Accepts<BancoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("banco/{id}", UpdateBanco)
            .WithName("UpdateBanco")
            .WithSummary("Actualiza un banco")
            .Accepts<BancoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("banco/{id}", DeleteBanco)
            .WithName("DeleteBanco")
            .WithSummary("Elimina un banco")
            .WithDescription("Elimina físicamente un banco del catálogo, identificado por su ID. Solo permite eliminar bancos que no tengan cuentas bancarias asociadas.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region CuentaBancaria
        // GET by id
        group.MapGet("cuenta-bancaria/{id}", GetCuentasBancariaById)
            .WithName("GetCuentaBancariaById")
            .WithSummary("Obtiene una cuenta bancaria por ID")
            .Produces<ApiResponseDto<CuentaBancariaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("cuenta-bancaria", GetPaginatedCuentasBancaria)
            .WithSummary("Obtiene cuentas bancarias paginadas y filtradas")
            .Produces<ApiResponseDto<PagedResultDto<CuentaBancariaListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("cuenta-bancaria", CreateCuentaBancaria)
            .WithName("CreateCuentaBancaria")
            .WithSummary("Crea una nueva cuenta bancaria")
            .Accepts<CuentaBancariaDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("cuenta-bancaria/{id}", UpdateCuentaBancaria)
            .WithName("UpdateCuentaBancaria")
            .WithSummary("Actualiza una cuenta bancaria")
            .Accepts<CuentaBancariaDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("cuenta-bancaria/{id}", DeleteCuentaBancaria)
            .WithName("DeleteCuentaBancaria")
            .WithSummary("Elimina una cuenta bancaria")
            .WithDescription("Elimina físicamente una cuenta bancaria del catálogo. Solo permite eliminar cuentas que no tengan pagos o movimientos asociados.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region Estatus Contrato

        // GET by id
        group.MapGet("estatus-contrato/{id}", GetEstatusContratoById)
            .WithName("GetEstatusContratoById")
            .WithSummary("Obtiene un estatus de contrato por ID")
            .Produces<ApiResponseDto<EstatusContratoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("estatus-contrato/", GetPaginatedEstatusContratos)
            .WithSummary("Obtiene estatus de contrato paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<EstatusContratoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("estatus-contrato/", CreateEstatusContrato)
            .WithName("CreateEstatusContrato")
            .WithSummary("Crea un nuevo estatus de contrato")
            .Accepts<EstatusContratoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("estatus-contrato/{id}", UpdateEstatusContrato)
            .WithName("UpdateEstatusContrato")
            .WithSummary("Actualiza un estatus de contrato")
            .Accepts<EstatusContratoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("estatus-contrato/{id}", DeleteEstatusContrato)
            .WithName("DeleteEstatusContrato")
            .WithSummary("Elimina un estatus de contrato")
            .WithDescription("Elimina físicamente un estatus de contrato del catálogo. Los estatus críticos del sistema (CAPTURADO=1, ACTIVO=2, CANCELADO=3, TERMINADO=4) no pueden ser eliminados.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region TipoPago


        // GET by id
        group.MapGet("tipo-pago/{id}", GetTipoPagoById)
            .WithName("GetTipoPagoById")
            .WithSummary("Obtiene un tipo de pago por ID")
            .Produces<ApiResponseDto<TipoPagoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipo-pago/", GetPaginatedTipoPago)
            .WithSummary("Obtiene tipos de pago paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<TipoPagoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("tipo-pago/", CreateTipoPago)
            .WithName("CreateTipoPago")
            .WithSummary("Crea un nuevo tipo de pago")
            .Accepts<TipoPagoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("tipo-pago/{id}", UpdateTipoPago)
            .WithName("UpdateTipoPago")
            .WithSummary("Actualiza un tipo de pago")
            .Accepts<TipoPagoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("tipo-pago/{id}", DeleteTipoPago)
            .WithName("DeleteTipoPago")
            .WithSummary("Elimina un tipo de pago")
            .WithDescription("Elimina físicamente un tipo de pago del catálogo, identificado por su ID. Solo permite eliminar tipos de pago que no tengan movimientos o pagos asociados.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #endregion

        #region TipoDireccion

        // GET all
        group.MapGet("tipo-direccion/all", GetTipoDireccionAll)
                .WithName("GetAllTiposDirecciones")
                .WithSummary("Obtiene todos los tipos de dirección")
                .Produces<ApiResponseDto<IEnumerable<TipoDireccionDto>>>(StatusCodes.Status200OK)
                .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET paginados
        group.MapGet("tipo-direccion/", GetTipoDireccionPaginados)
            .WithName("GetTiposDireccionesPaginados")
            .WithSummary("Obtiene tipos de dirección paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<TipoDireccionDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET by id
        group.MapGet("tipo-direccion/{id}", GetTipoDireccionById)
            .WithName("GetTipoDireccionById")
            .WithSummary("Obtiene un tipo de dirección por ID")
            .Produces<ApiResponseDto<TipoDireccionDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("tipo-direccion/", CreateTipoDireccion)
            .WithName("CreateTipoDireccion")
            .WithSummary("Crea un nuevo tipo de dirección")
            .Accepts<CreateTipoDireccionDto>("application/json")
            .Produces<ApiResponseDto<TipoDireccionDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("tipo-direccion/{id}", UpdateTipoDireccion)
            .WithName("UpdateTipoDireccion")
            .WithSummary("Actualiza un tipo de dirección existente")
            .Accepts<UpdateTipoDireccionDto>("application/json")
            .Produces<ApiResponseDto<TipoDireccionDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("tipo-direccion/{id}", DeleteTipoDireccion)
            .WithName("DeleteTipoDireccion")
            .WithSummary("Elimina un tipo de dirección")
            .Produces<ApiResponseDto>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET export to Excel
        group.MapGet("tipo-direccion/exportar", ExportToExcelTipoDireccion)
            .WithName("ExportTiposDirecciones")
            .WithSummary("Exporta tipos de direcciones filtrados a Excel")
            .Produces<byte[]>(StatusCodes.Status200OK, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region Colonia

        group.MapGet("colonia/all", GetColoniaAll)
            .WithName("GetAllColonias")
            .WithSummary("Obtiene todas las colonias")
            .Produces<ApiResponseDto<IEnumerable<ColoniaDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET paginados
        group.MapGet("colonia/", GetColoniaPaginados)
            .WithName("GetColoniasPaginados")
            .WithSummary("Obtiene colonias paginadas y filtradas")
            .Produces<Result<PagedResultDto<ColoniaDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET export to Excel
        group.MapGet("colonia/exportar", ExportToExcelColonia)
            .WithName("ExportColonias")
            .WithSummary("Exporta colonias filtradas a Excel")
            .Produces<byte[]>(StatusCodes.Status200OK, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET by id
        group.MapGet("colonia/{id}", GetColoniaById)
            .WithName("GetColoniaById")
            .WithSummary("Obtiene una colonia por ID")
            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET códigos postales
        group.MapGet("colonia/get-codigospostales", GetCodigosPostales)
            .WithName("GetCodigosPostales")
            .WithSummary("Obtiene códigos postales que coincidan con el parámetro")
            .Produces<ApiResponseDto<ICollection<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET colonias por código postal
        group.MapGet("colonia/get-cols-by-cp", GetColoniasByCodigoPostal)
            .WithName("GetColoniasByCodigoPostal")
            .WithSummary("Obtiene colonias por código postal")
            .Produces<Result<ColoniaComponentDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET colonias por ID (component)
        group.MapGet("colonia/get-cols-by-id/{id}", GetColoniasById)
            .WithName("GetColoniasById")
            .WithSummary("Obtiene colonias por ID")
            .Produces<ApiResponseDto<ColoniaComponentDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("colonia/", CreateColonia)
            .WithName("CreateColonia")
            .WithSummary("Crea una nueva colonia")
            .Accepts<CreateColoniaDto>("application/json")
            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("colonia/{id}", UpdateColonia)
            .WithName("UpdateColonia")
            .WithSummary("Actualiza una colonia existente")
            .Accepts<UpdateColoniaDto>("application/json")
            .Produces<ApiResponseDto<ColoniaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("colonia/{id}", DeleteColonia)
            .WithName("DeleteColonia")
            .WithSummary("Elimina una colonia")
            .Produces<ApiResponseDto>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #endregion


    }

    #region Banco

    public async Task<IResult> GetBancoById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetBancoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedBanco(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(BancoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetBancosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateBanco(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] BancoDto model)
    {
        var command = new CreateBancoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateBanco(
    [FromServices] ICommandMediator commandMediator,
  [FromRoute] int id,
    [FromBody] BancoDto model)
    {
        var command = new UpdateBancoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    public async Task<IResult> DeleteBanco(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int id
    )
    {
        var command = new DeleteBancoCommand
        {
            IdBanco = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion

    #region CuentaBancaria

    public async Task<IResult> GetCuentasBancariaById(
     [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id)
    {
        var result = await queryMediator.QueryAsync(new GetCuentaBancariaByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedCuentasBancaria(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(CuentaBancariaListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetCuentasBancariasQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateCuentaBancaria(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] CuentaBancariaDto model)
    {
        var command = new CreateCuentaBancariaCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateCuentaBancaria(
    [FromServices] ICommandMediator commandMediator,
    int id,
    [FromBody] CuentaBancariaDto model)
    {
        var command = new UpdateCuentaBancariaCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteCuentaBancaria(
    [FromServices] ICommandMediator commandMediator,
    [FromRoute] int id)
    {
        var command = new DeleteCuentaBancariaCommand
        {
            IdCuentaBancaria = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region Estatus Contrato


    public async Task<IResult> GetEstatusContratoById(
     [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id)
    {
        var result = await queryMediator.QueryAsync(new GetEstatusContratoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedEstatusContratos(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(EstatusContratoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetEstatusContratosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateEstatusContrato(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] EstatusContratoDto model)
    {
        var command = new CreateEstatusContratoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateEstatusContrato(
    [FromServices] ICommandMediator commandMediator,
   [FromRoute] int id,
    [FromBody] EstatusContratoDto model)
    {
        var command = new UpdateEstatusContratoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteEstatusContrato(
    [FromServices] ICommandMediator commandMediator,
    [FromRoute] int id)
    {
        var command = new DeleteEstatusContratoCommand
        {
            IdEstatusContrato = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region TipoPago

    public async Task<IResult> GetTipoPagoById(
    [FromServices] IQueryMediator queryMediator,
    int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoPagoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedTipoPago(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(TipoPagoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetTipoPagosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateTipoPago(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] TipoPagoDto model)
    {
        var command = new CreateTipoPagoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateTipoPago(
        [FromServices] ICommandMediator commandMediator,
        [FromRoute] int id,
        [FromBody] TipoPagoDto model)
    {
        var command = new UpdateTipoPagoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteTipoPago(
       [FromServices] ICommandMediator commandMediator,
       [FromRoute] int id)
    {
        var command = new DeleteTipoPagoCommand
        {
            IdTipoPago = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion


    #region TipoDireccion

    public async Task<IResult> GetTipoDireccionAll(
       [FromServices] IQueryMediator queryMediator)
    {

        var result = await queryMediator.QueryAsync(new GetTipoDireccionesQuery());
        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> GetTipoDireccionPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {

        var result = await queryMediator.QueryAsync(new GetTipoDireccionesPagedQuery
        {
            SearchTerm = q,
            Page = page,
            Size = size
        });

        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> GetTipoDireccionById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoDireccionByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> CreateTipoDireccion(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreateTipoDireccionDto createDto)
    {
        var command = new CreateTipoDireccionCommand
        {
            sTipoDireccion = createDto.sTipoDireccion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateTipoDireccion(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] UpdateTipoDireccionDto updateDto)
    {
        var command = new UpdateTipoDireccionCommand
        {
            Id = id,
            sTipoDireccion = updateDto.sTipoDireccion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteTipoDireccion(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var result = await commandMediator.SendAsync(new DeleteTipoDireccionCommand { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ExportToExcelTipoDireccion(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IExcelExportService excelExportService,
        [FromQuery] string? q = null)
    {
        try
        {
            var tiposDirecciones = await queryMediator.QueryAsync(new GetTiposDireccionesForExportQuery
            {
                SearchTerm = q
            });

            var excelBytes = excelExportService.ExportToExcel(
                tiposDirecciones,
                "TiposDirecciones",
                $"tipos_direcciones_{DateTime.Now:yyyyMMdd_HHmmss}"
            );

            return Results.File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"tipos_direcciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            var result = Result.Error($"Error al generar archivo Excel {ex.Message}");
            return result.ToCustomMinimalApiResult();
        }
    }

    #endregion


    #region Colonias

    public async Task<IResult> GetColoniaAll(
       [FromServices] IQueryMediator queryMediator)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasQuery());
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniaPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null)
    {
        // Validación de parámetros
        if (page < 1 || size < 1)
        {
            return Result.Invalid(new ValidationError("Los parámetros de paginación deben ser mayores a 0"))
                .ToCustomMinimalApiResult();
        }

        var result = await queryMediator.QueryAsync(new GetColoniasPagedQuery
        {
            SearchTerm = q,
            Page = page,
            Size = size,
            SortBy = sortBy,
            SortDir = sortDir
        });

        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniaById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetColoniaByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCodigosPostales(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string codigoPostal)
    {
        var result = await queryMediator.QueryAsync(new GetCodigosPostalesQuery { CodigoPostal = codigoPostal });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniasByCodigoPostal(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string codigoPostal)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasByCodigoPostalQuery { CodigoPostal = codigoPostal });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetColoniasById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetColoniasByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateColonia(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreateColoniaDto createDto)
    {
        var command = new CreateColoniaCommand
        {
            sColonia = createDto.sColonia,
            Estado = createDto.Estado,
            Municipio = createDto.Municipio,
            CodigoPostal = createDto.CodigoPostal
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateColonia(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] UpdateColoniaDto updateDto)
    {
        var command = new UpdateColoniaCommand
        {
            Id = id,
            sColonia = updateDto.sColonia,
            Estado = updateDto.Estado,
            Municipio = updateDto.Municipio,
            CodigoPostal = updateDto.CodigoPostal
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteColonia(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var result = await commandMediator.SendAsync(new DeleteColoniaCommand { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ExportToExcelColonia(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IExcelExportService excelExportService,
        [FromQuery] string? q = null)
    {
        try
        {
            var colonias = await queryMediator.QueryAsync(new GetColoniasForExportQuery
            {
                SearchTerm = q
            });

            // Verificar si el resultado es exitoso
            if (!colonias.IsSuccess)
            {
                return colonias.ToCustomMinimalApiResult();
            }

            var excelBytes = excelExportService.ExportToExcel(
                colonias.Value,
                "Colonias",
                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}"
            );

            return Results.File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"colonias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            var result = Result.Error($"Error al generar archivo Excel: {ex.Message}");
            return result.ToCustomMinimalApiResult();
        }
    }
    #endregion
}
