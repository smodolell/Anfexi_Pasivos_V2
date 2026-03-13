using Anfx.Pasivos.Application.Features.Sistema.DTOs;

namespace Anfx.Pasivos.Application.Features.Sistema.Commands;

public record CreateEmpresaCommand(EmpresaCreateDto Empresa) : ICommand<Result<EmpresaDto>>;
public class CreateEmpresaCommandHandler : ICommandHandler<CreateEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConsecutivoService _consecutivoService;
    private readonly IMapper _mapper;
    private readonly IValidator<EmpresaCreateDto> _validator;

    public CreateEmpresaCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IConsecutivoService consecutivoService, IMapper mapper, IValidator<EmpresaCreateDto> validator)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _consecutivoService = consecutivoService;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<EmpresaDto>> HandleAsync(CreateEmpresaCommand message, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
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

            var consecutivo = await _consecutivoService.ObtenerSiguienteConsecutivoAsync("Empresa", cancellationToken);
            if (!consecutivo.Success)
            {
                return Result.Invalid(new ValidationError("Error al generar el Consecutivo"));
            }

            var empresa = new Empresa { IdEmpresa = consecutivo.ConsecutivoGenerado };

            _mapper.Map(message.Empresa, empresa);

            _context.Empresas.Add(empresa);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result.Created(empresaDto, "Empresa creada exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Error($"Error al crear la empresa: {ex.Message}");
        }
    }
}
