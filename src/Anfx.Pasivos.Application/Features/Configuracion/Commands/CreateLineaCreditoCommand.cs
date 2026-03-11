using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class CreateLineaCreditoCommand : ICommand<Result<int>>
{
    public required LineaCreditoEditDto Model { get; set; }
}

internal class CreateLineaCreditoCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<LineaCreditoEditDto> validator
) : ICommandHandler<CreateLineaCreditoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<LineaCreditoEditDto> _validator = validator;

    public async Task<Result<int>> HandleAsync(CreateLineaCreditoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_LineaCredito>(model);

            // Calcular montos iniciales
            entity.MontoDisponible = entity.MontoAprobado;
            entity.MontoDispuesto = 0;
            entity.NoDisposiciones = 0;

            await _context.PSV_LineaCredito.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdLineaCredito);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}