using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Validators;

public class RolCreateDtoValidator : AbstractValidator<RolCreateDto>
{
    private readonly IApplicationDbContext _context;

    public RolCreateDtoValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.sRol)
            .NotEmpty().WithMessage("El nombre del rol es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
            .MustAsync(async (nombre, cancellationToken) =>
                !await _context.Roles
                    .AnyAsync(r => r.sRol == nombre, cancellationToken))
            .WithMessage("El nombre del rol ya está registrado"); ;

        RuleFor(x => x.Descripcion)
            .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres");
        
    }
}