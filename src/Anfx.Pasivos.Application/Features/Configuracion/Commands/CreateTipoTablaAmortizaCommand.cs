using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class CreateTipoTablaAmortizaCommand : ICommand<Result<int>>
{
    public required TipoTablaAmortizaDto Model { get; set; }
}

internal class CreateTipoTablaAmortizaCommandHandler : ICommandHandler<CreateTipoTablaAmortizaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoTablaAmortizaDto> _validator;

    public CreateTipoTablaAmortizaCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoTablaAmortizaDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateTipoTablaAmortizaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_TipoTablaAmortiza>(model);
            await _context.PSV_TipoTablaAmortiza.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdTipoTablaAmortiza);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
