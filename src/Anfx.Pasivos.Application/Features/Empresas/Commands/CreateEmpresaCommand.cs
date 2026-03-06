using Anfx.Pasivos.Application.Features.Empresas.DTOs;

namespace Anfx.Pasivos.Application.Features.Empresas.Commands;

public record CreateEmpresaCommand(EmpresaCreateDto Empresa) : ICommand<Result<EmpresaDto>>;
public class CreateEmpresaCommandHandler : ICommandHandler<CreateEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<EmpresaCreateDto> _validator;

    public CreateEmpresaCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<EmpresaCreateDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<EmpresaDto>> HandleAsync(CreateEmpresaCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(message.Empresa, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            // Verificar si el RFC ya existe
            var rfcExists = await _context.Empresas
                .AnyAsync(e => e.RFC == message.Empresa.RFC, cancellationToken);

            if (rfcExists)
            {
                return Result.Invalid(new ValidationError("El RFC ya está registrado"));
            }

            var empresa = _mapper.Map<Empresa>(message.Empresa);

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync(cancellationToken);

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result.Created(empresaDto, "Empresa creada exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al crear la empresa: {ex.Message}");
        }
    }
}
