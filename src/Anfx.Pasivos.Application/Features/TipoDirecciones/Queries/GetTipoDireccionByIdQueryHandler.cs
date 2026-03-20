using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public class GetTipoDireccionByIdQueryHandler : IQueryHandler<GetTipoDireccionByIdQuery, Result<TipoDireccionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTipoDireccionByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<TipoDireccionDto>> HandleAsync(GetTipoDireccionByIdQuery request, CancellationToken cancellationToken = default)
    {
        var tipoDireccion = await _context.TiposDirecciones
         .SingleOrDefaultAsync(td => td.Id == request.Id);

        if (tipoDireccion == null)
        {
            return Result.NotFound("Tipo de direccion no encontrado");
        }

        var result = _mapper.Map<TipoDireccionDto>(tipoDireccion);

        return Result.Success(result);
    }
}
