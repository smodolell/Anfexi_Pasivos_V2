using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Commands;

public class CreateRolCommandHandler : ICommandHandler<CreateRolCommand, Result<RolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<RolCreateDto> _validator;

    public CreateRolCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<RolCreateDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<RolDto>> HandleAsync(CreateRolCommand request, CancellationToken cancellationToken = default)
    {
        try
        {

            var model = request.Model;

            var validateResult = await _validator.ValidateAsync(model);
            if (!validateResult.IsValid)
            {
                return Result.Invalid(validateResult.AsErrors());
            }

            var rol = _mapper.Map<Rol>(model);

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync(cancellationToken);

            var rolDto = _mapper.Map<RolDto>(rol);
            return Result.Success(rolDto, "Rol creado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al crear el rol: {ex.Message}");
        }
    }
}
