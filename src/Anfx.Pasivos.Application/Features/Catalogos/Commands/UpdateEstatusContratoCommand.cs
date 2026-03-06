using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateEstatusContratoCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required EstatusContratoDto Model { get; set; }
}

internal class UpdateEstatusContratoCommandHandler : ICommandHandler<UpdateEstatusContratoCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<EstatusContratoDto> _validator;

    public UpdateEstatusContratoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<EstatusContratoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateEstatusContratoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_EstatusContrato.SingleOrDefaultAsync(r => r.IdEstatusContrato == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Estatus de Contrato no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_EstatusContrato.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
