using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public class CreateColoniaCommandHandler : ICommandHandler<CreateColoniaCommand, Result<ColoniaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateColoniaCommand> _validator;

    public CreateColoniaCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<CreateColoniaCommand> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<ColoniaDto>> HandleAsync(CreateColoniaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }


            var colonia = new Colonia
            {
                sColonia = request.sColonia,
                Estado = request.Estado,
                Municipio = request.Municipio,
                CodigoPostal = request.CodigoPostal
            };

            _context.Colonias.Add(colonia);
            await _context.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<ColoniaDto>(colonia);

            return Result.Created(result, $"/api/colonias/{result.Id}");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);

        }

    }
}
