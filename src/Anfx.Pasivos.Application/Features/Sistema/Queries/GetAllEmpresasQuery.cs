using Anfx.Pasivos.Application.Features.Sistema.DTOs;

namespace Anfx.Pasivos.Application.Features.Sistema.Queries;

public record GetAllEmpresasQuery : IQuery<Result<IEnumerable<EmpresaDto>>>;

public class GetAllEmpresasQueryHandler : IQueryHandler<GetAllEmpresasQuery, Result<IEnumerable<EmpresaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllEmpresasQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<IEnumerable<EmpresaDto>>> HandleAsync(GetAllEmpresasQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var empresas = await _context.Empresas
                .OrderBy(e => e.Empresa1)
                .ToListAsync(cancellationToken);

            var empresasDto = _mapper.Map<IEnumerable<EmpresaDto>>(empresas);
            return Result.Success(empresasDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener las empresas: {ex.Message}");
        }
    }
}
