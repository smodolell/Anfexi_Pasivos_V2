namespace Anfx.Pasivos.Application.Features.Sistema.Queries
{
    public record EmpresaExistsQuery(int Id) : IQuery<Result<bool>>;

    public class EmpresaExistsQueryHandler : IQueryHandler<EmpresaExistsQuery, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public EmpresaExistsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<Result<bool>> HandleAsync(EmpresaExistsQuery message, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _context.Empresas
                    .AnyAsync(e => e.IdEmpresa == message.Id, cancellationToken);

                return Result.Success(exists);
            }
            catch (Exception ex)
            {
                return Result.Error($"Error al verificar la existencia de la empresa: {ex.Message}");
            }
        }
    }
}
