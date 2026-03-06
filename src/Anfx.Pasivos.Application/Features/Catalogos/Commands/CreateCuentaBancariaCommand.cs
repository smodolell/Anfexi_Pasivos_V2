using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class CreateCuentaBancariaCommand : ICommand<Result<int>>
{
    public required CuentaBancariaDto Model { get; set; }
}

internal class CreateCuentaBancariaCommandHandler : ICommandHandler<CreateCuentaBancariaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CuentaBancariaDto> _validator;

    public CreateCuentaBancariaCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<CuentaBancariaDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<int>> HandleAsync(CreateCuentaBancariaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = _mapper.Map<PSV_CuentaBancaria>(model);
            await _context.PSV_CuentaBancaria.AddAsync(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Created(entity.IdCuentaBancaria);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
