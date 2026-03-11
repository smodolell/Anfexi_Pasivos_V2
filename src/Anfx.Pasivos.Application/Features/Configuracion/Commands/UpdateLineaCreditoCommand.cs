using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class UpdateLineaCreditoCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required LineaCreditoEditDto Model { get; set; }
}

internal class UpdateLineaCreditoCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<LineaCreditoEditDto> validator
) : ICommandHandler<UpdateLineaCreditoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<LineaCreditoEditDto> _validator = validator;

    public async Task<Result> HandleAsync(UpdateLineaCreditoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var lineaCredito = await _context.PSV_LineaCredito
                .SingleOrDefaultAsync(x => x.IdLineaCredito == request.Id, cancellationToken);

            if (lineaCredito == null)
            {
                return Result.NotFound("Línea de crédito no encontrada");
            }

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            _mapper.Map(model, lineaCredito);
            _context.PSV_LineaCredito.Update(lineaCredito);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
