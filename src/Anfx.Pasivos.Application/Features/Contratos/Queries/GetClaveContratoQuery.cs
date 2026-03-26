namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetClaveContratoQuery : IQuery<Result<string>>
{
    public int IdTipoCredito { get; set; }
}


internal class GetClaveContratoQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetClaveContratoQuery, Result<string>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<string>> HandleAsync(
        GetClaveContratoQuery message,
        CancellationToken cancellationToken = default)
    {
        var tipoCredito = await _context.PSV_TipoCredito
            .SingleOrDefaultAsync(r => r.IdTipoCredito == message.IdTipoCredito, cancellationToken);

        if (tipoCredito == null)
            return Result.Invalid(new ValidationError("No existe el Tipo de Crédito"));

        var claveContrato = string.Format(
            "{0}{1}{2}",
            tipoCredito.Prefijo,
            tipoCredito.Sufijo,
            (tipoCredito.Contador + 1).ToString().PadLeft(5, '0'));

        return Result.Success(claveContrato);
    }
}