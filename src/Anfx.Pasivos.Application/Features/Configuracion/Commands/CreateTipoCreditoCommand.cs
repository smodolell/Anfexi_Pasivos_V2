using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class CreateTipoCreditoCommand : ICommand<Result<int>>
{
    public required TipoCreditoDto Model { get; set; }
}


internal class CreateTipoCreditoCommandHandler : ICommandHandler<CreateTipoCreditoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoCreditoDto> _validator;

    public CreateTipoCreditoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoCreditoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateTipoCreditoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_TipoCredito>(model);

            await _context.PSV_TipoCredito.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);


            return Result.Created(entity.IdTipoCredito);

        }
        catch (Exception ex)
        {

            return Result.Error(ex.Message);
        }
    }
}
