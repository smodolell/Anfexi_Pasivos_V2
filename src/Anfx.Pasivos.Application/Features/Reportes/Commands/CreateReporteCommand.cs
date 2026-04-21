using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class CreateReporteCommand : ICommand<Result<int>>
{
    public required ReporteEditDto Model { get; set; }
}

public class CreateReporteCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<ReporteEditDto> validator,IParameterExtractor parameterExtractor
) : ICommandHandler<CreateReporteCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<ReporteEditDto> _validator = validator;
    private readonly IParameterExtractor _parameterExtractor = parameterExtractor;

    public async Task<Result<int>> HandleAsync(CreateReporteCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = message.Model;
            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Invalid(validationResult.AsErrors());

            var oReporte = new RSP_Reporte();
            _mapper.Map(model, oReporte);
            _context.RSP_Reporte.Add(oReporte);
            await _context.SaveChangesAsync(cancellationToken);

           
            if (!string.IsNullOrWhiteSpace(oReporte.StoredProcedure))
            {
                var parametros = await _parameterExtractor.ExtractAsync(oReporte.StoredProcedure, cancellationToken);

                foreach (var param in parametros)
                {
                    _context.RSP_Parametro.Add(new RSP_Parametro
                    {
                        Id = Guid.NewGuid(),
                        ReporteId = oReporte.Id,
                        NomParametro = param.Name,
                        TipoDato = param.DataType,
                        InputId = param.Direction == ParameterDirection.Input ? 1 : 0,
                        Display = param.Name.Replace("@", ""),
                        Order = param.Order
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return Result.Success(oReporte.Id);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
