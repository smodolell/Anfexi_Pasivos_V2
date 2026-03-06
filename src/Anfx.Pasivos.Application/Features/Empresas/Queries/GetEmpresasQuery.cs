using Anfx.Pasivos.Application.Features.Empresas.DTOs;

namespace Anfx.Pasivos.Application.Features.Empresas.Queries;

public record GetEmpresasQuery(int Page, int Size, string? SearchTerm) : IQuery<Result<PagedResultDto<EmpresaDto>>>;

public class GetEmpresasQueryHandler : IQueryHandler<GetEmpresasQuery, Result<PagedResultDto<EmpresaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaginator _paginator;

    public GetEmpresasQueryHandler(IApplicationDbContext context,IPaginator paginator)
    {
        _context = context;
        this._paginator = paginator;
    }



    public async Task<Result<PagedResultDto<EmpresaDto>>> HandleAsync(GetEmpresasQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Empresas.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(e => e.sEmpresa.Contains(request.SearchTerm) ||
                                       e.RFC.Contains(request.SearchTerm) ||
                                       e.RazonSocial.Contains(request.SearchTerm));
            }


            var empresas = query.OrderBy(e => e.sEmpresa);


            var result = await _paginator.PaginateAsync<Empresa, EmpresaDto>(
                empresas,
                request.Page,
                request.Size
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error($"Error al obtener las empresas: {ex.Message}");
        }
     
    }
}
