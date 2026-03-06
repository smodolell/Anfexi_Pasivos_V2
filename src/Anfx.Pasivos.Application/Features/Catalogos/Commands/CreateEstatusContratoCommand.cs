using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateEstatusContratoCommand : ICommand<Result<int>>
{
    public required EstatusContratoDto Model { get; set; }
}

internal class CreateEstatusContratoCommandHandler : ICommandHandler<CreateEstatusContratoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<EstatusContratoDto> _validator;

    public CreateEstatusContratoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<EstatusContratoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateEstatusContratoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_EstatusContrato>(model);
            await _context.PSV_EstatusContrato.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdEstatusContrato);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
