using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;

public class UpdateTipoDireccionCommandHandler : ICommandHandler<UpdateTipoDireccionCommand, Result<TipoDireccionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateTipoDireccionCommand> _validator;

    public UpdateTipoDireccionCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<UpdateTipoDireccionCommand> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }


    public async Task<Result<TipoDireccionDto>> HandleAsync(UpdateTipoDireccionCommand request, CancellationToken cancellationToken = default)
    {

        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var tipoDireccion = await _context.TiposDirecciones.SingleOrDefaultAsync(r => r.Id == request.Id);
            if (tipoDireccion == null)
            {
                return Result.NotFound($"Tipo de dirección con ID {request.Id} no encontrado.");
            }

            tipoDireccion.sTipoDireccion = request.sTipoDireccion;

            await _context.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<TipoDireccionDto>(tipoDireccion);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }
}
