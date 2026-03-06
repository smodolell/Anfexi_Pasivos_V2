using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetBancoByIdQuery : IQuery<Result<BancoDto>>
{
    public int Id { get; set; }
}

internal class GetBancoByIdQueryHandler : IQueryHandler<GetBancoByIdQuery, Result<BancoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBancoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<BancoDto>> HandleAsync(GetBancoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_Banco.SingleOrDefaultAsync(r => r.IdBanco == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Banco no existe");
            }
            var dto = _mapper.Map<BancoDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
