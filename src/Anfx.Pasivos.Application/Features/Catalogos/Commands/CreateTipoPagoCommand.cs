using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateTipoPagoCommand : ICommand<Result<int>>
{
    public required TipoPagoDto Model { get; set; }
}

internal class CreateTipoPagoCommandHandler : ICommandHandler<CreateTipoPagoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoPagoDto> _validator;

    public CreateTipoPagoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoPagoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateTipoPagoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_TipoPago>(model);
            await _context.PSV_TipoPago.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdTipoPago);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
