using Anfx.Pasivos.Application.Features.Sistema.DTOs;

namespace Anfx.Pasivos.Application.Features.Sistema.Queries;

public record GetEmpresaByIdQuery(int Id) : IQuery<Result<EmpresaDto>>;

public class GetEmpresaByIdQueryHandler : IQueryHandler<GetEmpresaByIdQuery, Result<EmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmpresaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmpresaDto>> HandleAsync(GetEmpresaByIdQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.IdEmpresa== message.Id, cancellationToken);

            if (empresa == null)
            {
                return Result.NotFound("Empresa no encontrada");
            }

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result.Success(empresaDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener la empresa: {ex.Message}");
        }
    }
}
