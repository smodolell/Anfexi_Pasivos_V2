using Anfx.Pasivos.Application.Features.Empresas.DTOs;
using Azure.Core;

namespace Anfx.Pasivos.Application.Features.Empresas.Queries;

public record GetEmpresaByRfcQuery(string RFC) : IQuery<Result<EmpresaDto>>;

public class GetEmpresaByRfcQueryHandler : IQueryHandler<GetEmpresaByRfcQuery, Result<EmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmpresaByRfcQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmpresaDto>> HandleAsync(GetEmpresaByRfcQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.RFC == message.RFC, cancellationToken);

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
