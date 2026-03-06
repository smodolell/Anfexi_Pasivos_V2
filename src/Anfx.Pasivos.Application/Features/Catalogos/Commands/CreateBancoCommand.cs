using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateBancoCommand : ICommand<Result<int>>
{
    public required BancoDto Model { get; set; }
}

internal class CreateBancoCommandHandler : ICommandHandler<CreateBancoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<BancoDto> _validator;

    public CreateBancoCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<BancoDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateBancoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_Banco>(model);
            await _context.PSV_Banco.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdBanco);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
