using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class UpdateTipoCreditoCommand : ICommand<Result>
{
    public int Id { get; set; }

    public required TipoCreditoDto Model { get; set; }
}



internal class UpdateTipoCreditoCommandHandler : ICommandHandler<UpdateTipoCreditoCommand, Result>
{

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoCreditoDto> _validator;

    public UpdateTipoCreditoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoCreditoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTipoCreditoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_TipoCredito.SingleOrDefaultAsync(r => r.IdTipoCredito == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Credito no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_TipoCredito.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
        catch (Exception ex)
        {

            return Result.Error(ex.Message);
        }
    }
}