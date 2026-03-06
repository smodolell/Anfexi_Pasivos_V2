using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class UpdateCuentaBancariaCommand : ICommand<Result>
{
    public int Id { get; set; }
    public required CuentaBancariaDto Model { get; set; }
}

internal class UpdateCuentaBancariaCommandHandler : ICommandHandler<UpdateCuentaBancariaCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CuentaBancariaDto> _validator;

    public UpdateCuentaBancariaCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<CuentaBancariaDto> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result> HandleAsync(UpdateCuentaBancariaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Model;

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var entity = await _context.PSV_CuentaBancaria.SingleOrDefaultAsync(r => r.IdCuentaBancaria == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Cuenta Bancaria no existe");
            }
            _mapper.Map(model, entity);

            _context.PSV_CuentaBancaria.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
