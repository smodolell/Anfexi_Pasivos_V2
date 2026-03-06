using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;

public class CreateTipoDireccionCommandHandler : ICommandHandler<CreateTipoDireccionCommand, Result<TipoDireccionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTipoDireccionCommand> _validator;

    public CreateTipoDireccionCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<CreateTipoDireccionCommand> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<TipoDireccionDto>> HandleAsync(CreateTipoDireccionCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }
            var tipoDireccion = new TipoDireccion
            {
                sTipoDireccion = request.sTipoDireccion
            };

            _context.TiposDirecciones.Add(tipoDireccion);

            await _context.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<TipoDireccionDto>(tipoDireccion);

            return Result.Created(result);
        }
        catch (Exception ex)
        {

            return Result.Error(ex.Message);
        }


    }
}
