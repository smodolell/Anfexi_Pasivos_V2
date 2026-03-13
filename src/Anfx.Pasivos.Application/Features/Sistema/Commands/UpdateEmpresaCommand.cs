using Anfx.Pasivos.Application.Features.Sistema.DTOs;

namespace Anfx.Pasivos.Application.Features.Sistema.Commands;

public record UpdateEmpresaCommand(int id, EmpresaUpdateDto Empresa) : ICommand<Result<EmpresaDto>>;

public class UpdateEmpresaCommandHandler : ICommandHandler<UpdateEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<EmpresaUpdateDto> _validator;

    public UpdateEmpresaCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<EmpresaUpdateDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }


    public async Task<Result<EmpresaDto>> HandleAsync(UpdateEmpresaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.id != request.Empresa.Id)
            {
                return Result.Conflict("El ID de la ruta no coincide con el ID de la empresa");
            }

            var validationResult = await _validator.ValidateAsync(request.Empresa, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }


            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.IdEmpresa == request.Empresa.Id, cancellationToken);

            if (empresa == null)
            {
                return Result.NotFound("Empresa no encontrada");
            }

            // Verificar si el RFC ya existe en otra empresa
            var rfcExists = await _context.Empresas
                .AnyAsync(e => e.RFC == request.Empresa.RFC && e.IdEmpresa != request.Empresa.Id, cancellationToken);

            if (rfcExists)
            {
                return Result.Invalid(new ValidationError("El RFC ya está registrado en otra empresa"));
            }

            // Actualizar propiedades
            empresa.Empresa1 = request.Empresa.sEmpresa;
            empresa.RFC = request.Empresa.RFC;
            empresa.RazonSocial = request.Empresa.RazonSocial;
            empresa.Telefono = request.Empresa.Telefono??"";
            empresa.Representante = request.Empresa.Representante ?? "";
            empresa.AvisosEstadodeCuenta = request.Empresa.AvisosEstadodeCuenta ?? "";
            empresa.AdvertenciasEstadodeCuenta = request.Empresa.AdvertenciasEstadodeCuenta ?? "";
            empresa.AclaracionesEstadodeCuenta = request.Empresa.AclaracionesEstadodeCuenta ?? "";
            empresa.UsaDesembolso = request.Empresa.UsaDesembolso;
            //empresa.Pasivo = request.Empresa.Pasivo;
            //empresa.TipoDireccionId = request.Empresa.TipoDireccionId;
            //empresa.Calle = request.Empresa.Calle ?? "";
            //empresa.NumExterior = request.Empresa.NumExterior ?? "";
            //empresa.NumInterior = request.Empresa.NumInterior ?? "";
            //empresa.ColoniaId = request.Empresa.ColoniaId;

            await _context.SaveChangesAsync(cancellationToken);

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Result.Success(empresaDto, "Empresa actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al actualizar la empresa: {ex.Message}");
        }
    }
}
