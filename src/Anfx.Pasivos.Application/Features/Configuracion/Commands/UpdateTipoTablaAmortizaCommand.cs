using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class UpdateTipoTablaAmortizaCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required TipoTablaAmortizaDto Model { get; set; }
}

internal class UpdateTipoTablaAmortizaCommandHandler : ICommandHandler<UpdateTipoTablaAmortizaCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<TipoTablaAmortizaDto> _validator;

    public UpdateTipoTablaAmortizaCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<TipoTablaAmortizaDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateTipoTablaAmortizaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_TipoTablaAmortiza.SingleOrDefaultAsync(r => r.IdTipoTablaAmortiza == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Tabla Amortiza no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_TipoTablaAmortiza.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
