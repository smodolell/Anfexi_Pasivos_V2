namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteBancoCommand : ICommand<Result>
{
    public int IdBanco { get; set; }
}


internal class DeleteBancoCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteBancoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteBancoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var banco = await _context.PSV_Banco
                .Include(b => b.PSV_CuentaBancaria)
                .SingleOrDefaultAsync(r => r.IdBanco == message.IdBanco, cancellationToken);
            if (banco == null) return Result.NotFound("El banco no fue encontrado");

            if (banco.PSV_CuentaBancaria != null && banco.PSV_CuentaBancaria.Any())
            {
                var cuentas = string.Join(", ", banco.PSV_CuentaBancaria.Select(c => c.CuentaBancaria));
                return Result.Invalid(new ValidationError(
                    $"No se puede eliminar el banco porque tiene cuentas bancarias asociadas: {cuentas}"));
            }

            _context.PSV_Banco.Remove(banco);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("El banco fue eliminado");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);

        }

    }
}