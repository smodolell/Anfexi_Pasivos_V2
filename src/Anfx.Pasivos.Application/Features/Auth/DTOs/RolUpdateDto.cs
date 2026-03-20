using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class RolUpdateDto
{
    [Required(ErrorMessage = "El ID del rol es requerido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string sRol { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
    public string Descripcion { get; set; } = string.Empty;
}

public class RolUpdateDtoValidator : AbstractValidator<RolUpdateDto>
{
    private readonly IApplicationDbContext _context;

    public RolUpdateDtoValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del rol es requerido")
            .GreaterThan(0).WithMessage("El ID del rol debe ser mayor a cero");

        RuleFor(x => x.sRol)
            .NotEmpty().WithMessage("El nombre del rol es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
              .MustAsync(async (model, nombre, cancellationToken) =>
                !await _context.Roles
                    .AnyAsync(r => r.Titulo == nombre && r.IdRol != model.Id, cancellationToken))
            .WithMessage("El nombre del rol ya está registrado");

        RuleFor(x => x.Descripcion)
            .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));
        
    }
}