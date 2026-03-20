using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetAnticipoQuery : IQuery<Result<AnticipoDto>>
{

    public string? ContratoPasivo { get; set; }
}

internal class GetAnticipoQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAnticipoQuery, Result<AnticipoDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AnticipoDto>> HandleAsync(GetAnticipoQuery message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.ContratoPasivo))
        {
            return Result.NotFound("No se estableció la clave de Contrato.");
        }
        var contratoPasivo = message.ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];

        var itemDb = await _context.PSV_Contrato
            .Include(r => r.PSV_TablaAmortiza)
            .FirstOrDefaultAsync(f => f.Contrato == contratoPasivo);

        if (itemDb == null)
        {
            return Result.NotFound($"El contrato Clave:[{contratoPasivo}] no fue encontrado.");
        }

        if (itemDb.IdEstatusContrato != 2)
        {
            return Result.Invalid(new ValidationError($"El contrato Clave:[{contratoPasivo}] no esta activo"));
        }


        var SaldoInsoluto = itemDb.PSV_TablaAmortiza.Where(w => !w.Procesado && w.VersionTabla == w.PSV_Contrato.VersionTabla && w.IdTipoTabla == 1)
            .Sum(s => (decimal?)s.Capital) ?? 0;

        if (SaldoInsoluto == 0)
        {
            return Result.Invalid(new ValidationError("El contrato Pasivo ya no tiene Vencimientos por procesar, para aplicar el anticipo, no se puede continuar."));
        }
        var result = new AnticipoDto
        {
            IdContrato = itemDb.IdContrato,
            IdTipoReduccion = 1,
            FechaAnticipo = DateTime.Now
        };

        var ta = itemDb.PSV_TablaAmortiza
            .Where(w => !w.Procesado && w.IdTablaAmortiza != -1)
            .OrderBy(o => o.NoPago)
            .FirstOrDefault();

        if (ta != null && ta.FecInicial != null)
        {
            result.FechaAnticipo = ta.FecInicial.Value;
        }

        return Result.Success(result);
    }
}
