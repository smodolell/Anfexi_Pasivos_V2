using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateTipoPagoCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required TipoPagoDto Model { get; set; }
}

internal class UpdateTipoPagoCommandHandler : ICommandHandler<UpdateTipoPagoCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoPagoDto> _validator;

    public UpdateTipoPagoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoPagoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTipoPagoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_TipoPago.SingleOrDefaultAsync(r => r.IdTipoPago == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Pago no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_TipoPago.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
