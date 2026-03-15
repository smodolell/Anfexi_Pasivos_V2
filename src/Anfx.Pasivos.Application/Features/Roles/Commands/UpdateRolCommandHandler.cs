using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Commands;

public class UpdateRolCommandHandler : ICommandHandler<UpdateRolCommand, Result<RolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<RolUpdateDto> _validator;

    public UpdateRolCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<RolUpdateDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }


    public async Task<Result<RolDto>> HandleAsync(UpdateRolCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;   
            if (model == null)
            {
                return Result.Conflict("El modelo de datos es nulo");
            }

            if (request.Id != request.Model.Id)
            {
                return Result.Conflict("El ID de la ruta no coincide con el ID de la empresa");
            }

            if (request.Id != request.Model.Id)
            {
                return Result.Invalid(new ValidationError
                {
                  Identifier = nameof(request.Model.Id),
                  ErrorMessage = $"El ID de la ruta ({request.Id}) no coincide con el ID del rol ({request.Model.Id})"
                  
                });
            }

            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.IdRol == request.Model.Id, cancellationToken);

            if (rol == null)
            {
                return Result.NotFound("Rol no encontrado");
            }

            var validateResult = await _validator.ValidateAsync(model);
            if (!validateResult.IsValid)
            {
                return Result.Invalid(validateResult.AsErrors());
            }

            rol.Titulo = request.Model.sRol;
            rol.Descripcion = request.Model.Descripcion;

            await _context.SaveChangesAsync(cancellationToken);

            var rolDto = _mapper.Map<RolDto>(rol);
            return Result.Success(rolDto, "Rol actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al actualizar el rol: {ex.Message}");
        }
    }
}
