using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetFondeadorByIdQuery: IQuery<Result<FondeadorDto>>
{
    public int Id { get; set; }
}


internal class GetFondeadorByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetFondeadorByIdQuery, Result<FondeadorDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<Result<FondeadorDto>> HandleAsync(GetFondeadorByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var fondeador = await _context.PSV_Fondeador.SingleOrDefaultAsync(x => x.IdFondeador == request.Id);
            if (fondeador == null)
            {
                return Result.NotFound("Fondeador no encontrado");
            }
            var dto = _mapper.Map<FondeadorDto>(fondeador);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}