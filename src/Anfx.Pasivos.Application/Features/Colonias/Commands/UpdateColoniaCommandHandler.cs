using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public class UpdateColoniaCommandHandler : ICommandHandler<UpdateColoniaCommand,Result< ColoniaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateColoniaCommand> _validator;

    public UpdateColoniaCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<UpdateColoniaCommand> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<ColoniaDto>> HandleAsync(UpdateColoniaCommand request, CancellationToken cancellationToken = default)
    {

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var colonia = await _context.Colonias.FindAsync(request.Id);
        if (colonia == null)
            return Result.NotFound($"Colonia con ID {request.Id} no encontrada.");

        colonia.sColonia = request.sColonia;
        colonia.Estado = request.Estado;
        colonia.Municipio = request.Municipio;
        colonia.CodigoPostal = request.CodigoPostal;

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<ColoniaDto>(colonia);

        return Result.Success(result);
    }
}
